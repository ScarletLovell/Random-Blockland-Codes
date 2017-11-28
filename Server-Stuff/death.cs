// 2016-2017

deleteVariables("$DeathMessage_*");

function DeathMSG(%id, %client, %victim) {
	if(%client == %victim) %id = 2;
	%clN = strReplace(%client.name, %client.name, "\c3" @ %client.name @ "\c0");
	%viN = strReplace(%victim.name, %victim.name, "\c2" @ %victim.name @ "\c0");
 	if(%id == 2) { // Suicide
		%msg[0] = %clN SPC "killed themselves";
		%msg[1] = %clN SPC "had too much to drink";
		%msg[2] = %clN SPC "tried too hard";
		%msg[3] = %clN SPC "tried to be a chia-pet";
		%ci = "skull";
	} else if(%id == 6) { // Fall Damage
		%msg[0] = %clN SPC "was thrown back down";
		%msg[1] = %clN SPC "fell too far";
		%msg[2] = %clN SPC "hit the ground too hard";
		%msg[3] = %clN SPC "tried to be a space ship";
		%ci = "crater";
	} else if(%id == 7) { // Getting ran over
		%msg[0] = %clN SPC "got ran over by" SPC %viN;
		%msg[1] = %viN SPC "made" SPC %clN SPC "into dough";
		%ci = "car";
	} else if(%id == 8) { // Vehicle Explosion
		%msg[0] = %clN @ "\'s car exploded";
		%ci = "carExplosion";
	} else if(%id == 9) { // Drowning
		%msg[0] = %clN SPC "drowned";
		%msg[1] = %clN SPC "tried to swim";
		%msg[2] = %clN SPC "wanted to be a fish";
	} else if(%id == 10) { // Hammer
		%msg[0] = %clN SPC "was hammered by" SPC %viN;
		%ci = "hammer";
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
        if(%sourceClient.getClassName() $= "AiPlayer") {
			%sourceClient.name = "Bot";
			return DeathMSG(%damageType, %client, %sourceClient);
		} else DeathMSG(%damageType, %client, %sourceClient.client);
	}
};
activatePackage(deathMessages);
