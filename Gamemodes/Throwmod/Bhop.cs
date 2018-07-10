// WARNING. This requires:
// ../../Server-Stuff/onPlayerProcessTick.cs

function BHop_Schedule() {
    if(isEventPending($BHop_Schedule))
        cancel($BHop_Schedule);
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        if(!isObject(%player = %client.player))
            continue;
        if(%player.isCrouching) %process = %process @ "CROUCH";
        else %process = strReplace(%process, "CROUCH", "");
        if(%player.isJumping) %process = %process SPC "JUMP";
        else %process = strReplace(%process, "JUMP", "");
        if(%player.processTickAD != 0) %process = %process SPC (%player.processTickAD < 0 ? "A" : "D");
        else %process = stripChars(%process, "AD");
        if(%player.processTickWS != 0) %process = %process @ (%player.processTickWS < 0 ? "S" : "W");
        else %process = stripChars(%process, "WS");
        %player.process = %process = %process SPC mFloatLength(%player.processTickYaw, 1);
        if(%player.lastPosition $= %player.position)
            %player.lastPosTick++;
        initContainerRadiusSearch(vectorSub(%player.position, "0 0 1"), "0 0 1", $TypeMasks::fxBrickObjectType);
        if(isObject(%this=containerSearchNext())) {
            //
        }
        %AD = mAbs(%player.processTickYaw) * 0.8;
        if(%client.BHop_Mode $= "only_a" && %player.processTickAD > 0) {
            %client.instantRespawn();
            if(GetSimTime() - %client.ONLY_lastTimeForMessage > 1000)
                %client.chatMessage("\c6Pressing D isn't allowed in \c3only_a \c6mode");
            %client.ONLY_lastTimeForMessage = getSimTime();
            continue;
        }
        else if(%client.BHop_Mode $= "only_d" && %player.processTickAD < 0) {
            %client.instantRespawn();
            %client.chatMessage("\c6Pressing A isn't allowed in \c3only_d \c6mode");
            continue;
        }
        if(%client.processTickWS > 0) {
            %player.setVelocity(vectorScale(%player.getVelocity(), -0.1));
        }
        %eye = %player.getEyeVector();
        %eyeZ = getWord(%eye, 2);
        if(%eyeZ > 0) {
            %vec = vectorLen(getWords(%player.getVelocity(), 0, 1) SPC "0");
            //%vecZ = getWord(%player.getVelocity(), 2);
            if(%vec > 25) {
                %hit = containerRayCast(%player.position, "0 0 15", $TypeMasks::FxBrickObjectType, %player);
                if(isObject(%hit)) {
                    if(isObject(%hit.dataBlock)) {
                        %icon = %hit.getDatablock().iconName;
                        %pos = strPos(%icon, "ModTer");
                        if(%pos != -1) {
                            %player.setVelocity(vectorAdd(%player.getVelocity(), vectorScale("0 0 " @ (%eyeZ/2), 1.1)));
                        }
                    }
                }
            }
        }
        %player.SetVelocity(vectorAdd(%player.getVelocity(), vectorScale(%player.getForwardVector(), %AD * 1.5)));
        %player.lastvelocity = %player.getVelocity();
        %player.lastPosition = %player.position;
    }
    $BHop_Schedule = schedule(15, 0, BHop_Schedule);
}

function serverCmdBHOP_Help(%client) {
    %client.chatMessage("\c5BHop\c6, created by \c3Ashleyz\c6, using MovementTick by \c3Port\c6, inspired by \c5CS:GO");
    %client.chatMessage("\c6Hold down \"W\" while \c5Strafing \c6to gain speed \c7(\c6see below\c7)");
    %client.chatMessage("\c6When you move right, use your \"D\" key, and move your mouse to the right. This movement is called Strafing.");
    %client.chatMessage("\c6Strafing too fast in mid air, or using the wrong key combination to strafe will make you lose speed.");
    %client.chatMessage("\c6Other players in the way? Hide them with \c5/hide \c6or \c5/hideplayers");
    %client.chatMessage("\c6Players being annoying? You can also watch other players with \c5/spec");
}

function serverCmdSpec(%client, %victim) {
    if(%client.isSpectating != 0) {
        %client.isSpectating.chatMessage("\c3" @ %client.getPlayerName() SPC "\c6is no longer spectating you");
        %client.isSpectating = false;
        if(isObject(%client.player))
            %client.setControlObject(%client.player);
        if(isObject(%bot=%client.bot))
            %bot.delete();
        return;
    }
    if(%victim $= "")
        return %client.chatMessage("\c5BHop_Spectate\c6: You can spectate someone by using \c3/spec namehere \c6or watch the WR with \c3/spec wr");
    %N = %victim;
    if(findClientByName(%victim) == %client)
        return %client.chatMessage("\c6You cannot spectate yourself!");
    if(isObject(%victim=findClientByName(%victim))) {
        %vP = %victim.player;
        if(!isObject(%vP.camera[%client.getPlayerName()])) {
            %cam = %vP.camera[%client.getPlayerName()] = new Camera() {
                datablock = "Observer";
                watch = %client;
            };
            %cam.setOrbitMode(%vP, "0 0 4 0 0 0 4 4", 1, 10, 10, 1);
        }
        else
            %cam = %vP.camera[%client.getPlayerName()];
        %client.setControlObject(%cam);
        %client.isSpectating = %victim;
        %client.lastSpectated = getSimTime();
        %client.pushStats(%victim);
        %victim.chatMessage("\c3" @ %client.getPlayerName() SPC "\c6is now spectating you");
    }
    else
        %client.chatMessage("\c6There's noone in the server with the name\c3" SPC %N);
}
