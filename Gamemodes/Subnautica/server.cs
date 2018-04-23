if(!isObject(SubnauticaBotGroup)) {
    new SimGroup(SubnauticaBotGroup) { };
}

package Subnautica {
    function SubnauticaSchedule(%_15) {
        if(isEventPending($SubnauticaSchedule))
            cancel($SubnauticaSchedule);
        if(%_35 > 35) {
            %_35 = 0;
            %_35Finished = true;
        }
        %_35+=1;
        %waterHeight = $EnvGuiServer::WaterHeight-1.3;
        for(%i=0;%i < clientGroup.getCount();%i++) {
            %client = clientGroup.getObject(%i);
            if(!isObject(%player=%client.player)) {
                continue;
            }
            %playerHeight = getWord(%player.position, 2);
            %bot = %client.standOnWaterBot;
            if(!isObject(%bot)) {
                %client.standOnWaterBot = %bot = new AIPlayer() {
                    datablock = "PlayerStandardArmor";
                    position = "500000 500000 5";
                    scale = "0.1 0.1 0.1";
                };
                %bot.client = %client;
                SubnauticaBotGroup.add(%bot);
                %client.standOnWaterBot.HideNode("ALL");
            }
            if(%playerHeight >= %waterHeight-0.2) {
                if(%player.isCrouching) {
                    %bot.position = "500000 500000 5";
                    continue;
                } else {
                    if(%player.running && %player.isGrounded) {

                        %client.centerPrint("Running", 1);
                    }
                }
                %player.isInWater = false;
                %bot.position = getWords(%player.position, 0, 1) SPC %waterHeight - 0.1;
                %player.bgOxygen+=0.05;
                if(%player.bgOxygen >= 1) {
                    %player.oxygen+=getRandom(3, 7);
                    %player.bgOxygen = 0;
                }
                if(%player.oxygen >= 100)
                    %player.oxygen = 100;
                %client.bottomPrint("\c2Oxygen\c6: " @ %player.oxygen);
            } else {
                %player.isInWater = true;
                if(%playerHeight <= (%waterHeight / 2)) {
                    %player.bgOxygen+=0.3;
                    %danger = true;
                } else
                    %player.bgOxygen+=0.15;
                if(%player.bgOxygen >= 1) {
                    %player.oxygen-=1;
                    %player.bgOxygen = 0;
                }
                if(%player.oxygen <= 0) {
                    // do more
                    %player.oxygen = 0;
                }
                %client.bottomPrint((%danger ? "\c0" : "\c4") @ "Oxygen\c6: " @ %player.oxygen);
                %bot.position = "500000 500000 5";
            }
        }
        if(%_35Finished) {
            for(%i=0;%i < SubnauticaBotGroup.getCount();%i++) {
                %bot = SubnauticaBotGroup.getObject(%i);
                if(!isObject(%bot.client)) {
                    %bot.delete();
                }
            }
        }
        $SubnauticaSchedule = schedule(150, 0, SubnauticaSchedule);
    }
    function GameConnection::SpawnPlayer(%client) {
        parent::SpawnPlayer(%client);
        %player = %client.player;
        %player.position = getRandom(-256, -243) SPC getRandom(-256, -242) SPC "150";
        %player.oxygen+=15;
        %client.chatMessage("\c6Welcome to the \c5Subnautica \c6world! \c7This world is not finished yet");
        %client.chatMessage("\c6You have been auto-spawned above ground for oxygen sake");
    }
};
activatePackage(Subnautica);
SubnauticaSchedule();

function onPlayerProcessTick(%player, %moveArg) {
    %client = %player.client;
    if(!isObject(%client))
        return;
    %a = (getSimTime() - %player.lastTick);
    %player.lasttick = getSimTime();

    %z = getWord(%player.position, 2);
    if(%player.lastZ >= %z+0.1 || %player.lastZ <= %z-0.1) {
        %player.isGrounded = false;
        %player.lastZTick = getSimTime();
    } else {
        if((getSimTime() - %player.lastZTick) >= 300) {
            %player.isGrounded = true;
        } else
            %player.running = false;
    }
    %player.lastZ = %z;

    %ad = getWord(%moveArg, 0);
    %ws = getWord(%moveArg, 1);
    if(%ws == 1 || %ws == -1) {
        if((getSimTime() - %player.nextWSPress) >= 100) {
            %time = getSimTime() - %player.lastRunPress;
            if(%time <= 250) {
                %player.running = true;
            }
            %player.lastRunPress = getSimTime();
        }
        if(%player.isInWater) {
            if(%ws == 1) {
                %player.addVelocity("0 0 " @ getWord(%player.getEyeVector(), 2));
            }
            if(%ws == -1) {
                %player.addVelocity("0 0 " @ getWord(%player.getEyeVector(), 2) * -1);
            }
        }
        %player.nextWSPress = getSimTime();
    } else {
        %player.running = false;
    }

    %player.processTickWS = %ws;
    %player.processTickAD = %ad;
    %roll = getWord(%moveArg, 2);
    %player.processTickRoll = %roll;
    %yaw = getWord(%moveArg, 3);
    %player.processTickYaw = %yaw;
    %pitch = getWord(%moveArg, 4);
    %player.processTickPitch = %pitch;
    %rollControl = getWord(%moveArg, 5);
    %key = getWord(%moveArg, 6);
    if(%key >= 64)
    {
        %key -= 64; //lag?
        %lag = 1;
    }
    if(%key >= 32)
    {
        %key -= 32;
        %jet = 1;
    }
    if(%key >= 16)
    {
        %key -= 16;
        %crouch = 1;
    }
    if(%key >= 8)
    {
        %key -= 8;
        %jump = 1;
    }
    if(%key >= 2)
    {
        %key -= 2;
        %firing = 1;
    }
    if(%key >= 1)
    {
        %key -= 1;
        %freelook = 1;
    }
    %player.isCrouching = %crouch ? 1 : 0;
}
