


// --- CUSTOM TIRES --- //
$SpecialTire[0] = "SpeedKartTire";
 $SpecialTire["Price" @ 0] = 1;
$SpecialTire[1] = "BusesTire";
 $SpecialTire["Price" @ 1] = 5;
$SpecialTire[2] = "KeiCarTire";
 $SpecialTire["Price" @ 2] = 25;
$SpecialTire[3] = "VenomTire";
 $SpecialTire["Price" @ 3] = 35;
$SpecialTire[4] = "TankTire";
 $SpecialTire["Price" @ 4] = 45;
$SpecialTire[5] = "EvasionTire";
 $SpecialTire["Price" @ 5] = 55;
$SpecialTire[6] = "CrownVictoriaTire";
 $SpecialTire["Price" @ 6] = 55;
$SpecialTire[7] = "CronusTire";
 $SpecialTire["Price" @ 7] = 55;
$SpecialTire[8] = "HachirokuTire";
 $SpecialTire["Price" @ 8] = 55;
$SpecialTire[9] = "bSkateboardTire";
 $SpecialTire["Price" @ 9] = 65;
$SpecialTire["max"] = 10;
 //$SpecialTire
function serverCmdSpecialTires(%client, %vehicle) {
    if(%vehicle $= "confirm" && %client.isBuyingSpecialTire != -1 && %client.isBuyingSpecialTire !$= "") {
        %client.chatMessage("\c6You bought the tires");
        %client.chatMessage("\c6If you would like to use these tires, type \c5/specialTires #");
        %client.specialTires[$SpecialTire[%client.isBuyingSpecialTire]] = 1;
        %client.points-=$SpecialTire["Price" @ %client.isBuyingSpecialTire];
        %client.isBuyingSpecialTire = -1;
        return;
    }
    if(!isObject($SpecialTire[%vehicle])) {
        %client.chatMessage("\c6Special Tire doesn't exist!");
        %client.chatMessage("\c6Use \c5/specialTires # \c6to buy or use that vehicle");
        for(%i=0;%i < $SpecialTire["max"];%i++)
            if(isObject($SpecialTire[%i]))
                messageClient(%client, '', "\c7[\c5"@ %i @"\c7]\c3" @ (strReplace($SpecialTire[%i], "Vehicle", "")) @ " \c6- " @ (%client.specialTires[$SpecialTire[%i]] > 0 ? "\c5You have this vehicle" : "You don't have this vehicle's tires; they cost: " @ $SpecialTire["Price" @ %i]));
    } else {
        if(%client.specialTires[$SpecialTire[%vehicle]] $= "" || %client.specialTires[$SpecialTire[%vehicle]] < 1) {
            if(%client.points < $SpecialTire["Price" @ %vehicle])
                return %client.chatMessage("\c6You don't have that many points! You require \c5" @ $SpecialTire["Price" @ %vehicle] @ " \c6points for that!");
            %client.chatMessage("\c6Would you like to buy these special tires \c7[\c5" @ $SpecialTire[%vehicle] @ "\c7]");
            %client.chatMessage("\c6Type \c5/specialTires confirm \c6if you would.");
            %client.isBuyingSpecialTire = %vehicle;
            return;
        } else {
            if(%client.hasCustomTires == -1) {
                %client.chatMessage("\c6You are now using special tires \c7[\c5" @ $SpecialTire[%vehicle] @ "\c7]");
                %client.hasCustomTires = $SpecialTire[%vehicle];
                if(%client.player && (%mount = %client.player.getObjectMount())) {
                    for(%i=0;%i < 4;%i++)
                        %mount.setWheelTire(%i, %client.hasCustomTires);
                }
            } else {
                %client.chatMessage("\c6You are no longer using special tires...");
                %client.hasCustomTires = -1;
                if(%client.player && (%mount = %client.player.getObjectMount())) {
                    for(%i=0;%i < 4;%i++)
                        %mount.setWheelTire(%i, %mount.getDatablock().defaultTire);
                }
            }
        }
    }
}


// --- SPECIAL VEHICLES --- //
$Special[0] = "BallVehicle";
 $Special["Price" @ 0] = 2;
$Special[0] = "LawnMowerVehicle";
 $Special["Price" @ 1] = 15;
$Special[1] = "bSkateboardVehicle";
 $Special["Price" @ 2] = 20;
$Special[2] = "VespaVehicle";
 $Special["Price" @ 3] = 35;
$Special[3] = "BMXVehicle";
 $Special["Price" @ 4] = 45;
$Special[4] = "DirtBikeVehicle";
 $Special["Price" @ 5] = 60;
$Special[5] = "KeiCarLifeVehicle";
 $Special["Price" @ 6] = 80;
$Special["max"] = 7;

function serverCmdSpecialVehicle(%client, %vehicle) {
    if(%vehicle $= "confirm" && %client.isBuyingSpecialVehicle != -1 && %client.isBuyingSpecialVehicle !$= "") {
        %client.chatMessage("\c6You bought the vehicle");
        %client.chatMessage("\c6If you would like to use this vehicle, type \c5/specialVehicle #");
        %client.specials[$Special[%client.isBuyingSpecialVehicle]] = 1;
        %client.points-=$Special["Price" @ %client.isBuyingSpecialVehicle];
        %client.isBuyingSpecialVehicle = -1;
        return;
    }
    if(!isObject($Special[%vehicle])) {
        %client.chatMessage("\c6Special Vehicle doesn't exist!");
        %client.chatMessage("\c6Use \c5/specialVehicle # \c6to buy or use that vehicle");
        for(%i=0;%i < $Special["max"];%i++)
            if(isObject($Special[%i]))
                messageClient(%client, '', "\c7[\c5"@ %i @"\c7]\c3" @ (strReplace($Special[%i], "Vehicle", "")) @ " \c6- " @ (%client.specials[$Special[%i]] > 0 ? "\c5You have this vehicle" : "You don't have this vehicle; it costs: " @ $Special["Price" @ %i]));
    } else {
        if(%client.specials[$Special[%vehicle]] $= "" || %client.specials[$Special[%vehicle]] < 1) {
            if(%client.points < $Special["Price" @ %vehicle])
                return %client.chatMessage("\c6You don't have that many points! You require \c5" @ $Special["Price" @ %vehicle] @ " \c6points for that!");
            %client.chatMessage("\c6Would you like to buy this special vehicle \c7[\c5" @ $Special[%vehicle] @ "\c7]");
            %client.chatMessage("\c6Type \c5/specialVehicle confirm \c6if you would.");
            %client.isBuyingSpecialVehicle = %vehicle;
            return;
        } else {
            if(%client.isUsingSpecial == -1) {
                %client.chatMessage("\c6You are now using a special vehicle \c7[\c5" @ $Special[%vehicle] @ "\c7]");
                %client.isUsingSpecial = %vehicle;
            } else {
                %client.chatMessage("\c6You are no longer using a special vehicle...");
                %client.isUsingSpecial = -1;
            }
        }
    }
}


// OTHER
function serverCmdPoints(%client) {
    messageClient(%client, '', "\c6You have \c2" @ %client.points @ " \c6points");
    messageClient(%client, '', "\c6You can use points to buy \c3/specialVehicle\c6's ");
}

function serverCmdHelp(%client) {
    %client.chatMessage("\c6You can view your points with \c5/points");
    %client.chatMessage("\c6You can buy vehicles with points with \c5/specialVehicle");
    %client.chatMessage("\c6You can buy new tires for your vehicles with \c5/specialTires");
    %client.chatMessage("\c6You can also turn your car into a rainbow, see \c5/RGBHelp");
    %client.chatMessage("\c6You can jump with your vehicle using your activate key \c7[\c5Default is E\c7]");
    %client.chatMessage("\c6You can turn off death messages with \c5/toggleDeathMessages");
    %client.chatMessage("\c6If you want updates on this gamemode you can also join my discord server at <a:discord.gg/0xZ6AuKBFXWkCz9p>http://discord.gg/0xZ6AuKBFXWkCz9p");
}

function serverCmdToggleDeathMessages(%client) {
	if(%client.deathMessages <= 0) %client.deathMessages = 1;
	else %client.deathMessages = 0;
	messageClient(%client, '', "\c6Death Messages for you are now" SPC (%client.deathMessages < 1 ? "\c2on" : "\c0off"));
}

function serverCmdBumperKartVersion(%client) {
    return messageClient(%client, '', "\c3" @ $BumperKart["version"]);
}


// RGB
function serverCmdRGB(%client, %time, %r, %g, %b, %t) {
    if(%client.rgb["active"] == true) {
        %client.rgb["active"] = false;
        cancel(%client.rgb["pend"]);
        return messageClient(%client, '', "\c6RGB off");
    }
    if(%time >= 150)
        %client.rgb["time"] = %time;
    else
        %client.rgb["time"] = 1500;
    %colors = "R" @ %r SPC "G" @ %g SPC "B" @ %b SPC "T" @ %t;
    for(%i=0;%i < 4;%i++)
        %client.rgb[getSubStr(getWord(%colors, %i), 0, 1)] = getSubStr(getWord(%colors, %i), 1, 2);
    %client.rgb["active"] = true;
    messageClient(%client, '', "\c6RGB on at \c2" @ %client.rgb["time"] SPC "\c6time. Say \c2/rgbhelp \c6if you want to do more.");
    rgb(%client);
}

function serverCmdRGBhelp(%client) {
    messageClient(%client, '', "\c6Use \c7/rgb \c7[\c3time\c7] \c7[\c0R\c7] \c7[\c2G\c7] \c7[\c1B\c7] \c7[\c8Transparency\c7] \c6for more options.");
    messageClient(%client, '', "\c6Use \c0R\c2G\c1B \c6as is, 1 or 0");
    messageClient(%client, '', "\c6Make sure the time is at-least 150");
}

function rgb(%client) {
    if(isEventPending(%client.rgb["pend"]))
        cancel(%client.rgb["pend"]);
    if(!isObject(%client))
        return;
    %player = %client.player;
    if(!isObject(%player) || !%player.getControlObject())
        return schedule(1500, 0, rgb, %client);
    %vehicle = %client.player.getControlObject() | 0;
    %brick = %vehicle.spawnBrick;
    %vehicle.setNodeColor("ALL", (%client.rgb["R"] > 0 ? getRandom() : 1) SPC (%client.rgb["G"] > 0 ? getRandom() : 1) SPC (%client.rgb["B"] > 0 ? getRandom() : 1) SPC (%client.rgb["t"] > 0 ? getRandom() : 1));
    if(%client.rgb["time"] >= 150)
        %client.rgb["pend"] = schedule(%client.rgb["time"], 0, rgb, %client);
    else
        %client.rgb["pend"] = schedule(1500, 0, rgb, %client);
}
