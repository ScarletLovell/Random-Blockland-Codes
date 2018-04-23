$defaultFont = "<font:impact:18>";

function Player::GrabPlayer(%player, %victim) {
    if(%player.minigame !$= "") {
        return;
    }
    %player.mountObject(%victim, 0);

    //%victim.client.camera.setMode("Grabbed");
    //%victim.client.setControlObject(%victim.client.camera);
//    %victim.client.camera.setorbitmode(%player, 0, 5, 10, 5, 0);
    %victim.canDismount = 0;
    %victim.unMountImage(0);
    %victim.setLookLimits(0.5,0.5);
    %victim.setTransform(%player.position SPC "0 0 -0.85 180");
    %victim.playThread(2, death1);

    %player.playThread(2, armreadyboth);
}

$throwmodFontBackwards = false;
$throwmodEscapeFont = 15;
function throwmodSchedule() {
    cancel($throwmodSchedule);
    if($throwmodA >= 11 || $throwmodA < 0)
        $throwmodA = 0;
    $throwmodA+=1;
    eventSchedule();
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        if(!isObject(%client.minigame) || !isObject(%client.player))
            continue;
        if($throwmodEscapeFont > 50) {
            $throwmodEscapeFont = 50;
            $throwmodFontBackwards = true;
        } else if($throwmodEscapeFont < 15) {
            $throwmodEscapeFont = 15;
            $throwmodFontBackwards = false;
        }
        if(!$throwmodFontBackwards)
            $throwmodEscapeFont++;
        else
            $throwmodEscapeFont--;
        if(!%client.player.canDismount) {
            if(!isObject(%client.player.getObjectMount()))
                %client.player.canDismount = true;
            else
                %client.centerPrint("<font:impact:"@$throwmodEscapeFont@">\c0RAPID CLICK 5 TIMES to ESCAPE \c7["@%client.player.rapidClicking@"]", 1);
        }
        if(%client.player.deathCount $= "")
            %client.player.deathCount = 120;
        if(%client.player.minigame !$= "") {
            if(%client.player.minigame $= "FP") {
                %ui = "\c5Currently in Minigame";
            }
        } else if(%client.isSpectating) {
            continue;
        } else {
            %ui = "<font:impact:20>\c5Score\c6: " @ %client.gameScore SPC "\c3Speed\c6: " @ vectorLen(getWords(%client.player.getVelocity(), 0, 1));
            if($throwmodDeathActive) {
                if($throwmodA >= 11) {
                    if(!isObject(%client.player.getMountedObject(0)))
                        %client.player.deathCount-=1;
                    else
                        %client.player.deathCount = 120;
                }
                %ui = %ui@"<just:right>\c0Death In\c6: " @ %client.player.deathCount@"s ";
            }
        }
        %client.bottomPrint(%ui, 1, 1);
    }
    $throwmodSchedule = schedule(85, 0, throwmodSchedule);
}
throwmodSchedule();

function initThrow(%mount) {
    %cl = %mount.client;
    //%cl.setControlObject(%mount);

    %mount.setLookLimits(1, 0);
    %mount.playThread(2, root);
    %mount.canDismount = true;
    %mount.unMount();
}

function GameConnection::cM(%c,%m) {
    %c.chatMessage(%m);
}

function BuyUpgrade(%client, %amount) {
    %client.gameScore -= %amount;
    if(%client.gameScore < 0)
        %client.gameScore = 0;
}
function serverCmdUpgrade(%client, %upgrade) {
    %client.cM("Please note! Upgrades do not save currently!!");
    %upgrade = strLwr(%upgrade);
    if(%upgrade $= "speed") {
        %req = 20 * %client.upgrade["speed"];
        if(%client.gameScore >= %req) {
            %client.upgrade["speed"] = %client.upgrade["speed"] $= "" ? 2 : %client.upgrade["speed"]+1;
            %client.cM("\c6You've upgraded your speed to speed level to level \c3"@%client.upgrade["speed"]);
            buyUpgrade(%client, %req);
        } else {
            %client.cM("\c6You need \c5" @ %req @ " \c6score to buy this item!");
        }
    } else {
        %client.cM("\c6The buyable \c5Upgrade\c6s are: \c3Speed");
        %client.cM("\c3/Upgrade Speed");
    }
}

package Throwmod {
    function Armor::onEnterLiquid(%this, %obj, %coverage, %type) {
        parent::onEnterLiquid(%this, %obj, %coverage, %type);
        if(isObject(%obj.client) && isObject(%obj.client.minigame)) {
            %obj.lavaDamage(500);
        }
    }
    function Player::ActivateStuff(%player) {
        if(%player.lastActivate !$= "")
            %time = getSimTime() - %player.lastActivate;
        else
            %time = 150;
        if(%time < 150)
            return;
        %player.lastActivate = getSimTime();
        parent::activateStuff(%player);
    }
    function Player::UnMount(%player) {
        parent::UnMount(%player);
        %player.canDismount = 1;
    }
    function fxDTSBrick::onActivate(%obj, %player, %client, %pos, %vec) {
        parent::onActivate(%obj, %player, %client, %pos, %vec);
        %name = %obj.getName();
        if(%name $= "_Force_Door") {
            announce($defaultFont@"\c3"@%player.client.getPlayerName() @ " \c6is trying to break them up!!");
            //%obj.disapear(3);
        }
    }
    function GameConnection::SpawnPlayer(%client) {
        parent::SpawnPlayer(%client);
        %player = %client.player;
        %speed = %client.upgrade["speed"] / 3;
        %player.setMaxForwardSpeed(15+%speed);
    }
    function Armor::onTrigger(%armor, %player, %trigger, %active) {
        parent::onTrigger(%armor, %player, %trigger, %active);
        if(!isObject(%player.client.minigame))
            return;
        if(%trigger == 4 && %active == 1) {
            if(isObject(%player.getMountedObject(0))) {
                %player.client.centerPrint("\c6You gently let \c3"@%player.getMountedObject(0).client.getPlayerName()@"\c6 down", 1);
                %player.getMountedObject(0).client.centerPrint("\c6The person who was holding you gently put you down", 1);
                initThrow(%player.getMountedObject(0));
                %player.setVelocity("0 0 0");
            }
        }
        if(%trigger == 0 && %active == 1) {
            if(isObject(%mount=%player.getObjectMount()) && %mount.getClassName() $= "Player") {
                if((%cT=%player.lastClickTime) !$= "") {
                    %t = (getSimTime() - %ct);
                    if(%t < 400) {
                        if(%player.rapidClicking >= 5) {
                            initThrow(%player);
                            %player.rapidClicking = 0;
                            %player.client.centerPrint("<font:impact:"@$throwmodEscapeFont@">\c2You escaped!", 1);
                        } else {
                            %player.rapidClicking+=1;
                        }
                    } else
                        %player.rapidClicking = 0;
                } else
                    %player.rapidClicking = 0;
                %player.lastClickTime = getSimTime();
            } else if(isObject(%mount=%player.getMountedObject(0))) {
                initThrow(%mount);
                %mount.client.hitter = %player.client;
                %player.setVelocity("0 0 0");
                //schedule(5, 0, eval, %mount@".invulnerable = 0;");
                //%mount.position = (vectorScale(%pos, vectorLen(%player.getVelocity())));
                %vec = vectorScale(%player.getEyeVector(), 50);
                if(%player.minigame !$= "") {
                    %vec = vectorScale(%player.getEyeVector(), 2);
                }
                %mount.setVelocity(%vec);
            } else {
                %look = containerRayCast(%player.getEyePoint(), vectorAdd(%player.getEyePoint(), VectorScale(%player.getEyeVector(), 8)), $TypeMasks::PlayerObjectType, %player);
                if(isObject(%look) && isObject(%look.client)) {
                    if(!isObject(%look.client.minigame))
                        return;
                    %player.GrabPlayer(%look);
                    %look.canDismount = 0;
                }
            }
        }
    }

    function serverCmdActivateStuff(%client) {
        if(%client.rapidClickTime !$= "") {
            %time = getSimTime() - %client.rapidClickTime;
            if(%time <= 150)
                return;
        }
        %client.rapidClickTime = getSimTime();
        parent::activateStuff(%client);
    }
};
activatePackage(Throwmod);

deleteVariables("$DeathMessage_*");

function DeathMSG(%id, %client, %victim) {
	if(%client == %victim) %id = 2;
	%clN = strReplace(%client.name, %client.name, "\c3" @ %client.name @ "\c0");
	%viN = strReplace(%victim.name, %victim.name, "\c2" @ %victim.name @ "\c0");
	if(%id $= "hitter") { // Throwmod
		%msg[0] = "\c2" @ %viN SPC "\c0threw\c3" SPC %clN;
		%msg[1] = "\c2" @ %viN SPC "\c0flung\c3" SPC %clN;
		%msg[2] = "\c2" @ %viN SPC "\c0tapped\c3" SPC %clN;
		%msg[3] = "\c2" @ %viN SPC "\c0sent\c3" SPC %clN SPC "\c0into space";
        %msg[4] = "\c2" @ %viN SPC "\c0dunked\c3" SPC %clN;
        %msg[5] = "\c2" @ %viN SPC "\c0put\c3" SPC %clN SPC "\c0in their place";
        %msg[6] = "\c2" @ %viN SPC "\c0baptized\c3" SPC %clN;
		%victim.setScore(%victim.gameScore++);
		%ci = "death";
	} else if(%id == 2) { // Suicide
		%msg[0] = %clN SPC "tried too hard";
		%msg[1] = %clN SPC "suicided";
		%msg[2] = %clN SPC "failed the matrix";
		%msg[3] = "RIP" SPC %clN;
		%ci = "skull";
	} else if(%id == 5) {
		%msg[0] = %clN SPC "hit a wall";
		%ci = "splat";
	} else if(%id == 6) { // Fall Damage
		%msg[0] = %clN SPC "was thrown back down";
		%msg[1] = %clN SPC "fell too far";
		%msg[2] = %clN SPC "hit the ground too hard";
		%msg[3] = %clN SPC "tried to be a space ship";
		%ci = "crater";
	} else if(%id == 7) { // Getting ran over
		if(!%vin) %msg[0] = "Someone ran over" SPC %clN;
		%msg[0] = %clN SPC "got ran over by" SPC %viN;
		%msg[1] = %viN SPC "made" SPC %clN SPC "into dough";
		%ci = "car";
	} else if(%id == 8) { // Vehicle Explosion
		%msg[0] = %clN @ "\'s car exploded";
		%msg[1] = %clN SPC "just exploded";
		%ci = "carExplosion";
	} else if(%id == 9) { // Drowning
		%msg[0] = %clN SPC "drowned";
		%msg[1] = %clN SPC "tried to swim";
		%msg[2] = %clN SPC "became a fish";
		%msg[3] = %clN SPC "went too far in the deep end";
		%msg[4] = %clN SPC "took a detour";
        %msg[5] = %clN SPC "wanted to see the pools temperature";
	} else if(%id == 10) { // Hammer
		%msg[0] = %clN SPC "was hammered by" SPC %viN;
		%ci = "hammer";
	} else if(%id == 13) {
		%msg[0] = %viN SPC "made" SPC %clN SPC "go boom";
		%msg[1] = %clN SPC "exploded from " SPC %clN;
	} else if(%id == 14) {
		%msg[0] = %viN SPC "slayed" SPC %clN;
	} else if(%id == 15) { // Spear
		%msg[0] = %viN SPC "speared" SPC %clN;
		%ci = "generic";
	} else if(%id == 18) { // Rocket L
		%msg[0] = %viN SPC "exploded" SPC %clN;
		%ci = "generic";
	} else if(%id == 19 || %id == 20) { // Gun    // Gun Ambiko
		%msg[0] = %viN SPC "shot" SPC %clN;
		%ci = "generic";
	} else if(%id == 21) { // Bow
		%msg[0] = %clN SPC "was hit to the knee by" SPC %viN;
		%ci = "generic";
	} else if(%id == 23) {
		%msg[0] = %clN SPC "got eaten by a shark";
		%msg[1] = %clN SPC "was chewed on";
	} else return;
	for(%i=0;%i < 15;%i++)
		if(%msg[%i] $= "") {
			%max = %i - 1;
			break;
		}
	%r = getRandom(0, %max);
	for(%i=0;%i < clientGroup.getCount();%i++) {
		if(ClientGroup.getObject(%i).deathMessages < 1)
		messageClient(ClientGroup.getObject(%i), '', "<bitmap:base/client/ui/ci/" @ %ci @ ">" SPC %msg[%r]);
	}
}

function serverCmdToggleDeathMessages(%client) {
	if(%client.deathMessages <= 0) %client.deathMessages = 1;
	else %client.deathMessages = 0;
	messageClient(%client, '', "\c6Death Messages for you are now" SPC (%client.deathMessages < 1 ? "\c2on" : "\c0off"));
}

package deathMessages {
	function GameConnection::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc) {
		parent::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc);
        if(isObject(%client.hitter)) { // BumperKart minigame
			DeathMSG("hitter", %client, %client.hitter);
			return %client.hitter = "";
		} else if(%sourceClient.getClassName() $= "AiPlayer") {
			%sourceClient.name = "Bot";
			return DeathMSG(%damageType, %client, %sourceClient);
		} else {
			if(isObject(%sourceClient.getMountedObject(0)))
				return DeathMSG(%damageType, %client, %sourceClient.getMountedObject(0).client);
			DeathMSG(%damageType, %client, %sourceClient.client);
		}
	}
};
activatePackage(deathMessages);
