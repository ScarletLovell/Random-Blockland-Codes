$FPColors = "red yellow green cyan blue pink white black";
$FPColorCodes = "0 3 4 5 6 24 42 45 52";
// a b c d e f

function FP_getPlatformList() {
    %letters = "a b c d e f";
    for(%i=0;%i < 6;%i++) {
        %letter = getWord(%letters, %i);
        for(%o=0;%o < 6;%o++) {
            if(%plat $= "") {
                %plat = "_"@%letter@%o+1;
            } else {
                %plat = %plat SPC "_"@%letter@%o+1;
            }
        }
    }
    return %plat;
}

function FP_randomizePlatforms() {
    cancel($FPSchedule);
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %player = clientGroup.getObject(%i).player;
        if(!isObject(%player) || %player.minigame !$= "FP")
            continue;
        %player.client.gamescore+=0.2;
    }
    %colors = "red yellow green cyan blue pink white black";
    %plat = FP_getPlatformList();
    //talk(%plat);
    %colors = "0|red 3|yellow 4|green 5|cyan 6|blue 42|pink 45|white 52|black";
    for(%i=0;%i < getWordCount(%plat)-1;%i++) {
        %p = getWord(%plat, %i);
        %col = strReplace(getWord(%colors, getRandom(0,getWordCount(%colors))), "|", "\t");
        //talk(%col);
        if(getWord(%col, 0) $= "") {
            %col = "0\tred";
        }
        %p.setColor(getField(%col, 0));
        %p.colorType = getField(%col, 1);
    }
    if($Minigame["FP", "players"] > 3)
        $FPSchedule = schedule(1000, 0, FP_doPlatformCheck);
    else
        $FPSchedule = schedule(5000, 0, FP_randomizePlatforms);
}

function FP_doPlatformCheck() {
    %colorsReal = "\c0red \c3yellow \c2green \c4cyan \c1blue \c5pink \c6white \c8black";
    %colors = "red yellow green cyan blue pink white black";
    %random = getRandom(0, getWordCount(%colors));
    %color = getWord(%colors, %random-1);
    %colorReal = getWord(%colorsReal, %random-1);
    if(%colorReal $= "")
        return FP_doPlatformCheck();
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        %player = %client.player;
        if(%player.minigame $= "FP") {
            %client.centerPrint("<font:arial:30>"@%colorReal, 5);
        }
    }
    schedule(2500, 0, FP_makePlatformsFall, %color);
}

function FP_makePlatformsFall(%color) {
    //if(%color $= "")
        //return FP_doPlatformCheck();
    %plat = FP_getPlatformList();
    for(%i=0;%i < getWordCount(%plat);%i++) {
        %p = getWord(%plat, %i);
        if(%p.colorType !$= %color) {
            %p.disappear(5);
        }
    }
    schedule(5500, 0, FP_RandomizePlatforms);
}

function FP_ZoneSchedule(%client, %type, %banBrick) {
    //if(%client.bl_id == 9999)
    //    talk(%type);
    if(!isObject(%client.player))
        return;
    if((getSimTime() - %client.minigameTempBanned) <= 30000) {
        %client.chatMessage($defaultFont@"\c6You are banned from this minigame for 30 seconds.");
        %client.player.setTransform(%banBrick.getPosition());
        return;
    }
    if(%client.lastEnterMinigame !$= "") {
        %time = getSimTime() - %client.lastEnterMinigame;
        if(%time < 10000) {
            %client.spamEnterMinigames++;
        } else
            %client.spamEnterMinigames = 0;
        if(%client.spamEnterMinigames >= 4) {
            announce($defaultFont@"\c3"@%client.getPlayerName() SPC "\c6is now banned from minigames for 30 seconds for spamming entry!!");
            %client.minigameTempBanned = getSimTime();
            return;
        }
    }
    %player = %client.player;
    if(%type $= "joined") {
        %player.minigame = "FP";
        if($Minigame["FP", "Players"] $= "")
            $Minigame["FP", "Players"] = %client TAB %player SPC "";
        else
            $Minigame["FP", "Players"] = $Minigame["FP", "Players"] SPC %client TAB %player;
    } else {
        %player.minigame = "";
    }
    announce($defaultFont@"\c3" @ %client.getPlayerName() SPC "\c6"@%type@" the \c5Falling Platforms \c6minigame");
    %client.lastEnterMinigame = getSimTime();
}

package FallingPlatforms {
    function fxDtsBrick::onPlayerLeaveZone(%this, %player) {
        parent::onPlayerLeaveZone(%this, %player);
        %name = %this.getName();
        if(%name $= "_FP_enter") {
            if(%player.minigame $= "FP")
                return;
            cancel($FP_ZoneSchedule[%player]);
            $FP_ZoneSchedule[%player] = schedule(150, 0, FP_ZoneSchedule, %player.client, "joined", "_FP_banBrick");
        } else if(%name $= "_FP_exit") {
            if(%player.minigame !$= "FP")
                return;
            cancel($FP_ZoneSchedule[%player]);
            $FP_ZoneSchedule[%player] = schedule(150, 0, FP_ZoneSchedule, %player.client, "left", "_FP_banBrick");
        }
    }
};
activatePackage(FallingPlatforms);
