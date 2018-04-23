$SpeedyKartVersion = "0.9.0";

// Loading comes first before everything
//  We don't want people to crash.
// - Created by Buddy, taken from Bumperkart.
function SK_LoadTrack_Phase1(%filename)
{
    $RTV["votes"] = 0;
    if(isEventPending($RTV["schedule"]))
        cancel($RTV["schedule"]);
    //suspend minigame resets
    $SK::MapChange = 1;

    //put everyone in observer mode
    %mg = $defaultMiniGame;
    if(!isObject(%mg))
    {
        error("ERROR: SK_LoadTrack( " @ %filename  @ " ) - default minigame does not exist");
        return;
    }
    for(%i = 0; %i < %mg.numMembers; %i++) {
        %client = %mg.member[%i];
        %player = %client.player;
        if(isObject(%player)) {
            %player.delete();
            %client.createPlayer("0 0 " @ getRandom(999, 999999));
        }
        %camera = %client.camera;
        %camera.setFlyMode();
        %camera.mode = "Observer";
        %client.setControlObject(%camera);
        if(%trans !$= "")
            %client.camera.setOrbitPointMode(%trans, 0);
    }

   $loadMap = %fileName;
   $mapLoading = true;
   clearBricks2(0, 0);
}
// - Created by Buddy, taken from Bumperkart.
function SK_LoadTrack_Phase2(%filename)
{
    announce("\c6Loading SpeedKart track\c3" SPC %filename);
    $SK:CurrentTrack = %filename;

    //load environment if it exists
    %envFile = filePath(%fileName) @ "/environment.txt";
    if(isFile(%envFile)) {
        //echo("parsing env file " @ %envFile);
        //usage: GameModeGuiServer::ParseGameModeFile(%filename, %append);
        //if %append == 0, all minigame variables will be cleared
        %res = GameModeGuiServer::ParseGameModeFile(%envFile, 1);

        EnvGuiServer::getIdxFromFilenames();
        EnvGuiServer::SetSimpleMode();

        if(!$EnvGuiServer::SimpleMode) {
            EnvGuiServer::fillAdvancedVarsFromSimple();
            EnvGuiServer::SetAdvancedMode();
        }
    }

    //load save file
    schedule(10, 0, serverDirectSaveFileLoad, %fileName, 3, "", 2, 1);
    for(%i=0;%i < clientGroup.getCount();%i++) {
        if(isObject(%p=clientgroup.getObject(%i).player)) {
            %p.kill();
        }
    }
    schedule(1500, 0,  eval, "$mapLoading = false;");
}
// - Created by Buddy, taken from Bumperkart.
function clearBricks2(%bgI,%initBC)
{
    cancel($clearBricks2);
    if(%initBC $= "" || %initBC < 10)
        %initBC = getBrickCount();
    if(%bgI >= mainBrickGroup.getCount())
    {
        cancel($clearBricks3);
        cancel($clearBricks4);
        $clearBricks3 = schedule(%GD + 2500,0,SK_LoadTrack_Phase2,$loadMap);
        $clearBricks4 = schedule(%GD,0,"announce","\c6Cleared bricks, loading: \c3"@ $loadMap);
        return centerprintAll("\c6100% Bricks: 0/"@ %initBC,10);
    }
    //centerprintAll("\c6"@ 100 - ((%bc=getBrickCount()) / %initBC)*100 @"% Bricks: "@ %bc @"/"@ %initBC,10);
    centerPrintAll("Cleaning up all bricks and switching map...");
    %bg = mainBrickGroup.getObject(%bgI);
    for(%I=0;%I<25;%I++)
    {
        if(%bg.getCount() <= 0)
        {
            %bgI++;
            break;
        }
        %o = %bg.getObject(0);
        if(%o.getDatablock().brickSizeX > 16)
        {
            %o.respawn();
            %o.disappear(-1);
            %o.delete();
            return $clearBricks2 = schedule(15, 0, clearBricks2, %bgI,%initBC);
        }
        %o.delete();
    }
    $clearBricks2 = schedule(1, 0, clearBricks2, %bgI,%initBC);
}

$SK_BG = BrickGroup_888888;
// Speedykart package
if(!$SpeedyKart_HasStarted) {
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientgroup.getObject(%i);
        if(!isFile(%path="config/server/SpeedyKartSaves/" @ %client.BL_ID @ ".txt"))
            continue;
        %file = new FileObject();
        %file.openForRead(%path);
        %line = %file.readLine();
        %client.SpeedyKart["level"] = getField(%line, 0);
        %client.SpeedyKart["exp"] = getField(%line, 1);
        %client.SpeedyKart["wins"] = getField(%line, 2);
        %client.SpeedyKart["achievements"] = getField(%line, 3);
        %file.close();
        %file.delete();
    }
    $SpeedyKart_HasStarted = true;
}
function serverCmdKillAll(%client) {
    if(!%client.isAdmin)
        return;
    $killingAll = true;
    for(%i=0;%i < clientGroup.getCount();%i++)
        if(isObject(%player=clientGroup.getObject(%i).player))
            %player.kill();
    $killingAll = false;
}
package SpeedyKart {
    function GameConnection::setCameraBrick(%client, %posBrickName, %targetBrickName) {
        if(!isObject($defaultMinigame.first) || $defaultMinigame.hasJustStarted)
            parent::setCameraBrick(%client, %posBrickName, %targetBrickName);
    }
    function fxDTSBrick::disappear(%brick, %number) {
        if(!isObject($defaultMinigame.first) || $defaultMinigame.hasJustStarted)
            parent::disappear(%brick, %number);
    }
    function WheeledVehicleData::onAdd(%data, %obj) {
		parent::onAdd(%data, %obj);
        schedule(100, 0, SpeedyKart_SetLastPos, %obj);
    }
    function SpeedyKart_SetLastPos(%vehicle) { %vehicle.lastSavedPosition = vectorAdd(%vehicle.position, vectorScale(%vehicle.getForwardVector(), 3)); }
    function VehicleData::onEnterLiquid(%data, %obj, %coverage, %type) {
        if(%obj.isTimeWarping)
            return;
        if(!isObject(%player=%obj.getMountedObject(0)) || %obj.lastSavedPosition $= "" || %player.client.hardMode)
            return parent::onEnterLiquid(%data, %obj, %coverage, %type);
        if(%obj.lastEnteredWater !$= "" && (getSimTime() - %obj.lastEnteredWater) < 1500)
            return parent::onEnterLiquid(%data, %obj, %coverage, %type);
        doTimeTransform(%obj);
	}
    function WheeledVehicleData::onCollision(%this, %vehicle, %victim, %d, %e, %f) {
        parent::onCollision(%this, %vehicle, %victim, %d, %e, %f);
        if(isObject(%victim) && %victim.getClassName() $= "WheeledVehicle") {
            %vehicle.setVelocity("0 0 0");
            %victim.setVelocity("0 0 0");
        }
    }
    function serverCmdLight(%client, %two) {
        if(!isObject(%client.minigame) || !isObject(%player=%client.player) || !isPackage(GameModeSpeedKartPackage) || %two $= "light")
            return parent::serverCmdLight(%client);
        if(%player.powerup !$= "") {
            %client.UsePowerup();
        }
        else
            %client.centerPrint("\c0You have no powerup!", 1);
    }
    function GameConnection::SpawnPlayer(%client) {
        parent::SpawnPlayer(%client);
        if(!isObject(%client.minigame)) {
            %client.player.position = getRandom(0, 100) SPC getRandom(0, 100) @ " 100";
            return;
        }
        if(%client.SpeedyKart["hasSpawned"] && isObject(%client.minigame)) {
            SpeedyKart_RandomizePowerup(%client.player, 1);
            return;
        }
        if(isFile(%path="config/server/SpeedyKartSaves/" @ %client.BL_ID @ ".txt")) {
            %file = new FileObject();
            %file.openForRead(%path);
            %line = %file.readLine();
            %client.SpeedyKart["level"] = getField(%line, 0);
            %client.SpeedyKart["exp"] = getField(%line, 1);
            %client.SpeedyKart["wins"] = getField(%line, 2);
            %client.SpeedyKart["achievements"] = getField(%line, 3);
            %file.close();
            %file.delete();
        } else
            SpeedyKart_SetDefaults(%client);
        %client.SpeedyKart["hasSpawned"] = true;
    }
    function MiniGameSO::reset(%mg, %client) {
        if($mapLoading)
            return;
        %mg.hasJustStarted = true;
        %mg.ended = false;
        parent::reset(%mg, %client);
        schedule(25000, 0, eval, %mg @ ".hasJustStarted = 0;");
        SpeedyKart_ResetMinigame(%mg);
    }
    function GameConnection::onClientLeaveGame(%client) {
        echo("leaving");
        if(!%client.SpeedyKart["hasSpawned"])
            return parent::onClientLeaveGame(%client);
        echo("SAVED!");
        %file = new FileObject();
        %file.openForWrite("config/server/SpeedyKartSaves/" @ %client.BL_ID @ ".txt");
        %file.writeLine(%client.SpeedyKart["level"] TAB %client.SpeedyKart["exp"] TAB
                        %client.SpeedyKart["wins"] TAB %client.SpeedyKart["achievements"]);
        %file.close();
        %file.delete();
        parent::onClientLeaveGame(%client);
    }
    function GameConnection::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc) {
		parent::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc);
        if($mapLoading || !isObject(%mg=%client.minigame) || %mg.numMembers < 5 || isObject(%mg.alreadyWon) || isObject(%mg.first) || $killingAll)
            return;
        for(%i=0;%i < %mg.numMembers;%i++) {
            %member = %mg.member[%i];
            if(!isObject(%member.player) || %member == %client) {
                %members++;
                continue;
            }
            %alive = %member;
        }
        if(%members == (%mg.numMembers-1)) {
            %mg.alreadyWon = %alive;
            %alive.AddExp(5);
            %alive.SpeedyKart["wins"]++;
            announce("\c3" @ %alive.getPlayerName() @ " \c6won this race as the last person standing and gained \c55 EXP\c6!");
            messageClient(fcbn(Anth), '', %members SPC %alive.getPlayerName());
        }
    }
    function serverCmdActivateStuff(%client) {
        parent::serverCmdActivateStuff(%client);
        if(!isObject(%player=%client.player))
            return;
        if(%player.SpeedyKart["receivingPowerup"] !$= "") {
            %client.centerPrint("\c6You now have a new powerup", 0.2);
            %player.powerup = %player.SpeedyKart["receivingPowerup"];
            %player.numOfPowerups = %player.SpeedyKart["receivingPowerup", "num"];
            %player.lastPowerup = %player.SpeedyKart["receivingPowerup"];
            %player.SpeedyKart["receivingPowerup"] = "";
        }
    }
    function Armor::onTrigger(%armor, %player, %trigger, %active) {
        parent::onTrigger(%armor, %player, %trigger, %active);
        if(%active == 1 && %trigger == 0) {
            if(isObject(%player.getObjectMount())) {
                %player.lookingBehind = true;
                return;
            }
            %hit = containerRayCast(%player.getEyePoint(), vectorAdd(%player.getEyePoint(), VectorScale(%player.getEyeVector(), 5)), $TypeMasks::FxBrickAlwaysObjectType, %player);
            if(isObject(%hit) && isObject(%hit.dataBlock) && %hit.getDataBlock().getName() $= "brickVehicleSpawnData") {
                %client = %player.client;
                %client.wrenchBrick = %hit;
            	%client.wrenchBrick.sendWrenchVehicleSpawnData(%client);
                %client.brickName = %hit.getName();
        		commandToClient(%client, 'openWrenchVehicleSpawnDlg', "Set Vehicle", 1);
            }
        } else if(%active == 0 && %trigger == 0)
            %player.lookingBehind = 0;
    }
    function serverCmdSetWrenchData(%client, %data) {
        %data = strReplace(%data, getField(%data, 0),"N" SPC %client.brickName);
        parent::serverCmdSetWrenchData(%client, %data);
    }
    function WheeledVehicle::setNodeColor(%vehicle, %node, %color) {
        parent::setNodeColor(%vehicle, %node, %color);
        if(%node $= "RHand") %vehicle.RHand = %color;
        if(%node $= "LHand") %vehicle.LHand = %color;
        if(%node $= "ALL") %vehicle.color = %color;
    }
};
activatePackage(SpeedyKart);

function doTimeTransform(%veh) {
    if(%veh.timeStamp <= 0) {
        %veh.isTimeWarping = 0;
        %veh.lastEnteredWater = getSimTime();
        %veh.setvelocity(%veh.timeSaveVel);
        return;
    }
    %veh.isTimeWarping = true;
    %veh.timeStamp -= 1;
    %veh.rotation = %veh.lastTimeRot[%veh.timeStamp];
    %veh.setTransform(%veh.lastTimePos[%veh.timeStamp]);
    %veh.setVelocity("0 0 0");
    schedule(10, 0, doTimeTransform, %veh);
}

function SpeedyKart_ResetMinigame(%mg) {
    if(isEventPending($SK_StartRaceSchedule))
        cancel($SK_StartRaceSchedule);
    if(isEventPending($SpeedyKart["EndRace"]))
        cancel($SpeedyKart_EndRace);
    %mg.first = %mg.second = %mg.third = "";
    %mg.alreadyWon = %mg.hasStar = "";
    for(%i=0;%i < $SK_BG.NTObjectCount_["startingGateBar"];%i++) {
        $SK_BG.NTObject_["startingGateBar", %i].disappear(0);
    }
    $SK_StartRaceSchedule = schedule(25000, 0, SpeedyKart_StartRace);
}
function SpeedyKart_StartRace() {
    for(%i=0;%i < $SK_BG.NTObjectCount_["startingGateBar"];%i++) {
        $SK_BG.NTObject_["startingGateBar", %i].disappear(10);
    }
}
// achievements
$SpeedyKart["achievements"] =
        "Gotta_Go_Fast!_|_(Get_over_45_speed) First_Hit_|_(Hit_someone_with_a_powerup) SONIC_SPEED_|_(Got_over_135_speed_somehow) " @
        "Viagra_|_(Used_growth_once) Winner!_|_(Win_your_first_race) Five_Wins!?_|_(Get_5_wins_in_total) Shielded_|_(Get_hit_while_having_a_shield)" @
        "";
function SpeedyKart_GetAchievement(%number) {
    if((%ach=strReplace(getWord($SpeedyKart["achievements"], %number), "_", " ")) $= "")
        return -1;
    return %ach;
}
function SpeedyKart_GiveAchievement(%client, %number) {
    if(strPos(%client.SpeedyKart["achievements"], %number) != -1 || getWord($SpeedyKart["achievements"], %number) $= "" || (%a = SpeedyKart_GetAchievement(%number)) == -1)
        return;
    %ach = "\c3" @ %client.getPlayerName() @ " \c6earned an achievement:\c5 " @ getSubStr(%a, 0, strPos(%a, "|"));
    announce(%ach);
    %client.SpeedyKart["achievements"] = %number SPC %client.SpeedyKart["achievements"];
}
function serverCmdAchievements(%client) {
    %client.chatMessage("\c6Showing your achievements:");
    for(%i=0;%i < getWordCount(%client.SpeedyKart["achievements"]);%i++) {
        %id = getWord(%client.SpeedyKart["achievements"], %i);
        %client.chatMessage("\c6" @ %i @ "\c6: \c5" @ SpeedyKart_GetAchievement(%id));
    }
}

// level stuff
function SpeedyKart_SetDefaults(%client) {
    %client.SpeedyKart["level"] = 0;
    %client.SpeedyKart["exp"] = 0;
    %client.SpeedyKart["wins"] = 0;
    %client.SpeedyKart["achievements"] = "";
}
function GameConnection::AddExp(%client, %exp) {
    %client.SpeedyKart["exp"] += %exp;
    if(%client.SpeedyKart["exp"] >= mCeil(%client.SpeedyKart["level"] * (20 / 1.5))) {
        %l=%client.SpeedyKart["level"] += 1;
        %client.SpeedyKart["exp"] = 0;
        announce("\c3" @ %client.getPlayerName() @ "\c6 leveled up to level \c5" @ %l);
    }
}
function serverCmdReset(%client) {
    if(%client.reset !$= "" && (getSimTime() - %client.reset) < 10000) {
        SpeedyKart_SetDefaults(%client);
        %client.chatMessage("\c6All your stats have been reset.");
        %client.reset = "";
        return;
    }
    %client.chatMessage("\c6IF YOU'RE SURE YOU WANT TO RESET, SAY \c3/RESET \c6AGAIN");
    %client.reset = getSimTime();
}
function serverCmdStats(%client, %victim) {
    if(isfile(%path="config/server/SpeedyKartSaves/" @ %victim @ ".txt")) {
        %user = %victim;
        %file = new FileObject();
        %file.openForRead(%path);
        %line = %file.readLine();
        %level = getField(%line, 0);
        %exp = getField(%line, 1);
        %wins = getField(%line, 2);
        %file.close();
        %file.delete();
    } else if(trim(%victim) !$= "" && isObject(%victim=findclientByName(%victim))) {
        %user = %victim.getPlayerName();
        %level = %victim.SpeedyKart["level"];
        %exp = %victim.SpeedyKart["exp"];
        %wins = %victim.SpeedyKart["wins"];
    } else {
        %user = %client.getPlayerName();
        %level = %client.SpeedyKart["level"];
        %exp = %client.SpeedyKart["exp"];
        %wins = %client.SpeedyKart["wins"];
    }
    %client.chatMessage("\c3"@%user@"\'s Stats");
    %client.chatMessage("\c5Level: \c6"@%level);
    %client.chatMessage("\c5EXP: \c6"@%exp);
    %client.chatMessage("\c5Wins: \c6"@%wins);
}

// schedule
$SpeedyKart["vehicleTimer"] = 30;
$SpeedyKartPosition[1] = $SpeedyKartPosition[2] = $SpeedyKartPosition[3] = 0;
$SpeedyKartPosition[1, "len"] = $SpeedyKartPosition[2, "len"] = $SpeedyKartPosition[3, "len"] = "60000";
function SpeedyKartSchedule(%a, %b, %c, %d) {
    if(isEventPending($SpeedyKartSchedule))
        cancel($SpeedyKartSchedule);
    %second = %minute = %halfMinute = false;
    if(%a >= 1)
        %time = true;
    if(%a >= 10) {
        %second = true;
        %a = 0;
        if(%b >= 60) {
            %minute = true;
            %b = 0;
        }
        %b+=1;
        if(%c >= 30) {
            %halfMinute = true;
            %c = 0;
        }
        %c+=1;
        if(%d >= 5) {
            %canSave = true;
            %d = 0;
        }
        %d+=1;
    }
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        if(!isObject(%mg=%client.minigame)) {
            %client = %player = "";
            continue;
        }
        if(isObject(%mg.first)) {
            if(!isObject(%mg.alreadyWon) && !%mg.ended) {
                %client.centerPrint(
                        "<just:right>\c6First: \c3" @ (isObject(%first=%mg.first) ? %first.getPlayerName() : "\c0N/A") NL
                        "\c6Second: \c3" @ (isObject(%second=%mg.second) ? %second.getPlayerName() : "\c0N/A") NL
                        "\c6Third: \c3" @ (isObject(%third=%mg.third) ? %third.getPlayerName() : "\c0N/A") NL
                        "\c5Time Left: \c5" @ mCeil(getTimeRemaining($SpeedyKart["EndRace"])/1000) SPC "seconds", 1);
            } else {
                %client.centerPrint("<just:right>\c6Round ended", 1);
            }
        }
        if(!isObject(%player=%client.player)) {
            if(isObject(%orbit=%client.camera.getOrbitObject()) && isObject(%orbit.client)) {
                %bp = "<font:impact:22>\c6Spectating \c3" @ %orbit.client.getPlayerName();
                %bp = %bp NL "\c6Powerup: \c5" @ ((%p=%orbit.powerup)!$="" ? %p : "\c0N/A") SPC "\c7("@%orbit.numOfPowerups@"\c7)" ;
                if(isObject(%orbitMount=%orbit.getObjectMount()))
                    %bp = %bp @ "\c6, Speed: \c5" @ (!$defaultMinigame.hasJustStarted ? mCeil(vectorLen(%orbitMount.getVelocity())) : "\c0N/A");
                %bp = %bp @ "<just:right>\c6Level: \c5" @ %orbit.client.SPEEDYKART["Level"];
                %bp = %bp @ "\c6, EXP: \c5" @ %orbit.client.SPEEDYKART["EXP"] @ " \c6/ \c5" @ mCeil(%orbit.client.SpeedyKart["level"] * (20 / 1.5));
                %client.bottomPrint(%bp @ " ", 1, 5);
            }
            %client = %player = "";
            continue;
        }
        if(%halfMinute || %minute) {
            SpeedyKart_RandomizePowerup(%player, 1);
            %client.centerPrint("<just:right>\c6Gained new powerup", 2);
        }
        if(%player.vehicleTimer $= "" || %player.vehicleTimer > $SpeedyKart["vehicleTimer"])
            %player.vehicleTimer = $SpeedyKart["vehicleTimer"];
        if(!isObject(%mount=%player.getObjectMount())) {
            if(%player.vehicleTimer <= 0) {
                %client.bottomPrint("<just:center>\c0Dead.", 2, 1);
                %player.kill();
                %client = %player = "";
                continue;
            }
            %client.bottomPrint("<just:center>\c6You will be killed for not being in a vehicle in \c5" @ %player.vehicleTimer @ " seconds", 2, 1);
            if(%second)
                %player.vehicleTimer--;
        } else {
            if(!%mount.isTimeWarping) {
                %mount.lastTimePos[%mount.timeStamp] = %mount.getTransform();
                %mount.lastTimeRot[%mount.timeStamp] = %mount.rotation;
                %mount.timeStamp += 1;
            }
            initContainerBoxSearch(vectorSub(%mount.position, "0 0 1"), "1 1 1", $TypeMasks::FxBrickObjectType);
            %hit = containerSearchNext();
            if(isObject(%hit)) {
                %mount.isOnGround = true;
                if(!$defaultMinigame.hasJustStarted && %canSave && mCeil(vectorLen(%mount.getVelocity())) >= 5) {
                    %mount.lastSavedPosition = %mount.getTransform();
                    %mount.lastSavedTime = getSimTime();
                    %mount.timeStamp = 0;
                    %mount.timeSaveVel = %mount.getVelocity();
                } else {

                }
                %saveTime = mCeil(getMax(10000-(getSimTime()-%mount.lastSavedTime), 0) * 0.001);
            } else
                %mount.isOnGround = false;
            %speed = (!$defaultMinigame.hasJustStarted ? mCeil(vectorLen(%mount.getVelocity())) : "\c0N/A");
            if(!%client.minigame.hasJustStarted && %speed <= 1) {
                %client.bottomPrint("<just:center>\c0You're too slow! You will be killed in " @ %player.vehicleTimer, 1);
                if(%second) {
                    if(%player.vehicleTimer <= 0) {
                        %player.kill();
                        continue;
                    }
                    %player.vehicleTimer--;
                }
                %player = %client = %speed = "";
                continue;
            }
            if(%speed >= 45)
                SpeedyKart_GiveAchievement(%client, 0);
            if(%speed >= 135) SpeedyKart_GiveAchievement(%client, 2);
            //if(vectorLen(%player.position, "_winBrick".position) < $SpeedyKartPosition[1])
                //$SpeedyKartPosition[1] =
            if((%np=%player.SpeedyKart["receivingPowerup"]) !$= "") {
                if(%second)
                    %nPTime = %player.SpeedyKart["receivingPowerup", "time"]--;
                %nPTime = %player.SpeedyKart["receivingPowerup", "time"];
                %nPL = "<just:center>\c6Press your \c2Paint Key\c6 to receive the new powerup: \c5"@%nP @ "\c6, \c7" @ %nPTime;
                if(%nPTime <= 0)
                    %player.SpeedyKart["receivingPowerup"] = "";
            }
            %lB = (%player.lookingBehind?"\c7[Aiming backwards]":"");
            %client.bottomPrint(
                "<font:impact:22>" @
                "\c6Speed: \c5" @ %speed @ "\c6, " @
                "<just:right>\c6Level: \c5" @ %client.SPEEDYKART["Level"] @ "\c6, " @
                "\c6EXP: \c5" @ %client.SPEEDYKART["EXP"] @ " \c6/ \c5" @ mCeil(%client.SpeedyKart["level"] * (20 / 1.5)) SPC
                "<just:left>\c6Power-Up: \c5" @ ((%p=%player.powerup)!$="" ? %p : "\c0N/A") SPC "\c7("@%player.numOfPowerups@"\c7)" SPC %lB NL %nPL
            , 2, 1);
            %player.vehicleTimer = $SpeedyKart["vehicleTimer"];
            %nPL = %nPTime = %lB = %speed = %mount = "";
        }
        %client = %player = "";
    }
    %a+=1;
    $SpeedyKartSchedule = schedule(100, 0, SpeedyKartSchedule, %a, %b, %c, %d);
} SpeedyKartSchedule();


// Racing
function GameConnection::winRace(%client) {
    %mg = %client.miniGame;
    if(!isObject(%mg=$DefaultMinigame))
        return;
    if(isObject(%mg.alreadyWon))
        return;

    %alive = 0;
    for(%i=0;%i < %mg.numMembers;%i++) {
        %member = %mg.member[%i];
        if(isObject(%member.player) && %mg.first != %member && %mg.second != %member && %mg.third != %member)
            %alive++;
    }
    if(%mg.first == %client || %mg.second == %client || %mg.third == %client)
        return;
    if(!isObject(%mg.first)) {
        %alive-=1;
        commandToClient(%client, 'GetAchievement', "ACH_SPEEDKART_WIN");
        %client.AddExp(5);
        //SpeedyKart_EndRace(%mg);
        announce("\c3" @ %client.getPlayerName() @ " \c6got first place in this race and gained \c55 EXP\c6!");
        %client.SpeedyKart["wins"]++;
        if(%client.SpeedyKart["wins"] >= 5)
            SpeedyKart_GiveAchievement(%client, 5);
        SpeedyKart_GiveAchievement(%client, 4);
        if(%alive >= 2) {
            %mg.first = %client;
            $SpeedyKart["EndRace"] = schedule(30000, 0, SpeedyKart_EndRace, %mg);
        } else {
            SpeedyKart_EndRace(%mg);
            %mg.alreadyWon = %player;
        }
    } else if(!isObject(%mg.second)) {
        %alive-=1;
        %mg.second = %client;
        %client.AddExp(3);
        announce("\c3" @ %client.getPlayerName() @ " \c6got second in this race and gained \c53 EXP\c6!");
        if(%alive >= 2) {
            if(isEventPending($SpeedyKart["EndRace"]))
                cancel($SpeedyKart["EndRace"]);
            $SpeedyKart["EndRace"] = schedule(15000, 0, SpeedyKart_EndRace, %mg);
        } else {
            %mg.alreadyWon = %player;
            SpeedyKart_EndRace(%mg);
        }
    } else if(!isObject(%mg.third)) {
        %mg.first = "";
        %mg.alreadyWon = %client;
        %client.AddExp(1);
        %mg.alreadyWon = %client;
        announce("\c3" @ %client.getPlayerName() @ " \c6got third in this race and gained \c51 EXP\c6!");
        SpeedyKart_EndRace(%mg, 1);
    }
}

function SpeedyKart_EndRace(%mg, %t) {
    if(%t <= 0 || %t $= "")
        announce("\c6There's not enough players to continue, resetting...");
    if($defaultMinigame.hasJustStarted) {
        cancel($SpeedyKart_EndRace);
        return;
    }
    %mg.ended = true;
    %startTime = %mg.raceStartTime;
    if(%startTime <= 0)
        %startTime = %mg.lastResetTime;
    %elapsedTime = getSimTime() - %startTime;
    %elapsedTime = mFloor(%elapsedTime / 1000);
    %mg.schedule(2000, 0, reset);
}
