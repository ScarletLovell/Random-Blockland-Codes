//[Created by Anthonyrules144 - BL_ID 9999]
//Keep in mind, do not say this mod was yours, it is not. You may own this modification and edit it, but please, give credit where it's due, thanks!
//exec("./resources/sExec.cs");
if(!isObject(ctOS_line))
    datablock StaticShapeData(ctOS_line) {
        shapeFile = "config/cube.dts";
        scale = "1 1 1";
        allowPlayerStep = false;
    };

// Main Functions //
// ~ ctOS 2.0-1
// The future of connected communities.
function Player::ReadHit (%player) {
    return containerRayCast(%player.getEyePoint(), vectorAdd(%player.getEyePoint(), VectorScale(%player.getEyeVector(), 20)), $TypeMasks::ALL & ~$TypeMasks::PhysicalZoneObjectType, %player);
}
function Player::ctOS_schedule (%player) {
    if(isEventPending(%player.ctOS_schedule))
        cancel(%player.ctOS_schedule);
    if(!(%client = %player.client).ctOS_phone) {
        %player.ctOS_schedule = %player.schedule(550, ctOS_schedule);
        return;
    }
    if(%client.ctOS_Job $= "") {
        if($ctOS_reading["STATUS" @ (%r=getRandom(1,$CTOS_reading["STATUS" @ "max"])) @ "hasMultiple"] == true)
            %client.ctOS_Status = ($ctOS_reading["STATUS" @ %r @ "m" @ getRandom(1,$ctOS_reading["STATUS" @ %r @ "sM"])] SPC
                $ctOS_reading["STATUS" @ %r]);
        else
            %client.ctOS_Status = $ctOS_reading["STATUS" @ %r];
        %client.ctOS_Job = $ctOS_reading["INCOME&JOBS" @ getRandom(1,$CTOS_reading["INCOME&JOBS" @ "max"])];
    }
    if(!(%life = %player.client.ctOS_LifeSpan)) {
        %client.LifeSpanSchedule();
        %client.ctOS_LifeSpan = 60;
    }

    if(!isObject(%client.ctOS_line)) {
        %obj = %client.ctOS_line = new StaticShape() { datablock = "ctOS_line2"; position = %player.position; };
        %obj.setNetFlag(6, 1);
    } else %obj = %client.ctOS_line;
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %c = clientGroup.getObject(%i);
        if(%c == %client)
            continue;
        %obj.clearScopeToClient(%c);
    }
    %obj.position = %player.position;
    //%obj.rotation = %player.rotation;

    %hit = %player.ReadHit();
    if((%_H=isObject(%hit)) && (%_C=%hit.getClassName()) $= "WheeledVehicle" || %_H && %_C $= "AIPlayer" || %_H && %_C $= "Player") {
        %obj.scopeToClient(%client);
        %player.superHit = %hit;
        %_a = vectorSub(%player.position, %hit.position);
        %_b = vectornormalize(%_a);
        %obj.setScale(0.1 SPC 0.1 SPC vectorLen(%_a) * 0.5);
        %obj.rotation = vectornormalize(vectorcross("0 0 1", %_b)) SPC mradtodeg(macos(vectordot("0 0 1", %_b))) * -1;
        %fov = %player.getCameraFov() / 55;
        %dist = vectorDist(%hit.position, %player.position) / 4;
        %dist2 = vectorDist(getWord(%hit.position, 2), getWord(%player.position, 2)) / 5;
        %obj.position = vectoradd(vectorAdd(getWords(%player.position, 0, 1) SPC getWord(%player.position, 2) + getWord(%player.getScale(), 2), vectorScale(%player.getForwardvector(), 0.95)), vectorscale(%_b, 0.05 + %fov - %dist-%dist2));
    } else {
        %player.superHit = 0;
        %obj.setScale("0.1 0.1 0.1");
        %obj.position = %player.position;
        %obj.clearScopeToClient(%client);
    }

    %player.ctOS_schedule = %player.schedule(15, ctOS_schedule);
}
function GameConnection::LifeSpanSchedule (%client) {
    if(isEventPending(%client.ctOS_LifeSpanReceive))
        cancel(%client.ctOS_LifeSpanReceive);
    %client.hasLifeSpanScheduleRunning = true;
    if(%client.ctOS_LifeSpan > 60)
        return %client.ctOS_LifeSpanReceive = %client.schedule(6500, LifeSpanSchedule);
    commandToClient(%client, 'ctOS_ReceiveNewLifeSpan', %client.ctOS_LifeSpan+=5);
    %client.ctOS_LifeSpanReceive = %client.schedule(4500, LifeSpanSchedule);
}

// Functions //
function ctOS_GatherInfo () {
// A new status for someone can be created through the info file, such as "Owns multiple pets" \\
// Jobs and Incomes are also created through the Info file \\
// Since we have the hackers being able to see information, this is a nessecity. \\
    %f = new FileObject();
    %f.openforRead(%path="config/Info.txt");
    discoverFile(%path);
    deleteVariables("$ctOS_reading*");
    $ctOS_Jobs["max"] = $ctOS_Stats["max"] = %reading = %i = %o = %_OLD = 0;
    while(!%f.isEOF()) {
        if((%line = %f.readLine()) $= "" || strPos(%line, "-") == -1 && %reading $= "Jobs")
            continue;
        if(getSubStr(%line, 0, 1) $= "[" && getSubStr(%line, strLen(%line)-1, 1) $= "]") {
            %reading = strReplace(strReplace(%line, "]", ""), "[", "");
            %i=0;
            continue;
        }
        if(%reading !$= 0) {
            $ctOS_reading[%reading @ "max"] = %i++;
            if(getSubStr(%line, 0, 1) $= "[" && strPos(%line, "]") != -1) {
                %_N = getSubStr(%line, strPos(%line, "]"), strLen(%line));
                %newL = getSubStr(%line, strPos(%line, "]")+2, strLen(%_N));
                %realL = getSubStr(%line, strPos(%line, "[")+1, strPos(%line, "]")-1);
                talk(%realL);
                %wordCount = getWordCount(%realL);
                while(%_W < %wordCount) {
                    %_W++;
                    if((%_L=strPos(%line, "<", %_OLD)) != -1 && (%_LE=strPos(%line, ">", %_L)) != -1) {
                        %_word=getSubStr(%line, %_L+1, %_LE-2);
                        %a=$ctOS_reading[%reading @ %i @ "m" @ %o++] = %_word;
                        %_OLD = %_L-2;
                        %_W+=getWordCount(%_word);
                    } else {
                        %a=$ctOS_reading[%reading @ %i @ "m" @ %o++] = getWord(%realL, %o);
                        %_OLD = strPos(%line, getWord(%newL, %iL), %o);
                    }
                    $ctOS_reading[%reading @ %i @ "sM"] = %o;
                    $ctOS_reading[%reading @ %i @ "hasMultiple"] = true;
                    $ctOS_reading[%reading @ %i] = %newL;
                }
                %_OLD = %o = %_W = 0;
            }
            else
                $ctOS_reading[%reading @ %i] = %line;
        }
	}
    %f.close();
    %f.delete();
} ctOS_GatherInfo();

// Server Cmds // (Client -> Server)
function serverCmdctOS_phone (%client, %active) {
    commandToClient(%client, 'ReceivePhoneUpdate', 1);
    %player = %client.player;
    %client.ctOS_phone = %active;
    if(%active != 1) {
        commandToClient(%client, 'ctOS_ReceiveGrab', 0);
        %client.ctOS_line.delete();
        return cancel(%player.ctOS_schedule);
    }
    if(!isEventPending(%player.ctOS_schedule))
        %player.ctOS_Schedule();
}
function serverCmdctOS_grab (%client, %active) {
    if(!%client.ctOS_phone)
        return -1;
    if((%_S = (%player = %client.player).superhit).getClassName() $= "AIPlayer") {
        if(%_S.ctOS_Job $= "") {
            %_S.ctOS_Status = $ctOS_reading["STATUS" @ getRandom(1,$CTOS_reading["STATUS" @ "max"])];
            %_S.ctOS_Job = $ctOS_reading["INCOME&JOBS" @ getRandom(1,$CTOS_reading["INCOME&JOBS" @ "max"])];
            %_S.name = $ctOS_reading["BOTNAMES" @ getRandom(1,$CTOS_reading["BOTNAMES" @ "max"])];
        }
        commandToClient(%client, 'ctOS_ReceiveGrab', %active,
            "[BOT]" SPC %_S.name TAB
            %_S.ctOS_Status TAB
            %_S.ctOS_Job);
    }
    else if(%_S.client)
        commandToClient(%client, 'ctOS_ReceiveGrab', %active,
            %_S.client.getPlayerName() TAB
            %_S.client.ctOS_Status TAB
            %_S.client.ctOS_Job);
    else if(%_S.spawnBrick) {
        commandToClient(%client, 'ctOS_ReceiveGrab', %active,
            %_S.getDatablock().uiName TAB
            "Owned by: " @ %_S.spawnBrick.getGroup().name);
    }
    else {
        commandToClient(%client, 'ctOS_ReceiveGrab', 0);
        return -1;
    }
}
function serverCmdctOS_useTopItem (%client) {
    if((%veh=(%player = %client.player).ReadHit()).getClassName() $= "WheeledVehicle") %veh.Force(%client, "fwd"); }
function serverCmdctOS_useLeftItem (%client) {
    if((%veh=(%player = %client.player).ReadHit()).getClassName() $= "WheeledVehicle") %veh.Force(%client, "left"); }
function serverCmdctOS_useRightItem (%client) {
    if((%veh=(%player = %client.player).ReadHit()).getClassName() $= "WheeledVehicle") %veh.Force(%client, "right"); }
function serverCmdctOS_useBottomItem (%client) {
    if((%veh=(%player = %client.player).ReadHit()).getClassName() $= "WheeledVehicle") %veh.Force(%client, "bck"); }

// Vehicle Functions //
function WheeledVehicle::Force (%mount, %client, %dir) {
    if(!isObject(%client))
        return -1;
    if(%client.ctOS_LifeSpan < 15)
        return -1;
    commandToClient(%client, 'ctOS_ReceiveNewLifeSpan', %client.ctOS_LifeSpan -= 20);
    cancel(%ctOS.ctOS_LifeSpanReceive);
    if(isObject(%client.TestAI))
        %client.TestAI.delete();
    %ai = %client.TestAI = new AIPlayer() {
        datablock = "PlayerStandardArmor";
        position = "0 0 1000";
    };
    %ai.setControlObject(%mount);
    %ai.hideNode("ALL");
    if (%dir $= "fwd")
        %ai.setMoveY(1);
    else if (%dir $= "left") {
        %ai.setMoveY(1);
        %ai.schedule(450, setMoveYaw, -1);
    } else if (%dir $= "right") {
        %ai.setMoveY(1);
        %ai.schedule(450, setMoveYaw, 1);
    } else if (%dir $= "bck") {
        %ai.setMoveY(-1);
    }
    %ai.schedule(1500, delete);
    %mount.schedule(1500, GiveOldControl);
    return 1;
}
function WheeledVehicle::GiveOldControl (%mount) {
    %pl = %mount.getMountedObject(0);
    %vehicleData = %vehicle.getDatablock();
    for(%i=0;%i < %vehicleData.nummountpoints;%I++)
        if(%player == %vehicle.getMountNodeObject(%i)) {
            %seat = %i;
            break;
        }
    if(%seat == 0) {
        %pl.setControlObject(%mount);
        %pl.unMount();
        %mount.mountObject(%pl, 0);
    }
}
