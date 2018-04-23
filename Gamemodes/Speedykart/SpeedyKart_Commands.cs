function serverCmdHelp(%client) {
    %client.chatMessage("\c6SpeedyKart help \c7[\c5v"@$SpeedyKartVersion@"\c7]");
    %client.chatMessage("<font:arial:25>\c6USE MOUSE STEERING, \c7Options -> Controls -> Strafe Steering");
    %client.chatMessage("\c21. \c6- Use \c5/stereo \c6to have a stereo/radio on your player");
    %client.chatMessage("\c21.1. \c6- Use \c5/autostereo \c6after \c5/stereo \c6for an automated vehicle stereo");
    %client.chatMessage("\c22. \c6- Press your \c5light key\c6 to use powerups \c7(which are WIP)");
    %client.chatMessage("\c22.1. \c6- You can aim backwards with certain powerups with left mouse button");
    %client.chatMessage("\c23. \c6- In order to get different vehicles, click your \c5Vehicle Spawn");
    %client.chatMessage("\c24. \c6- Win a race, or hit someone with a powerup to gain EXP");
    %client.chatMessage("\c25. \c6- You earn achievements now \c7(which are WIP)\c6, to see them say \c5/achievements");
    %client.chatMessage("\c26. \c6- Want to reset all your data? Use \c5/reset");
    %client.chatMessage("\c27. \c6- Want to see someones stats? Use \c5/stats NAME \c6or \c5BL_ID");
    %client.chatMessage("\c28. \c6- You can\'t die easily in water. Want to turn this off for you, just say \c5/hardmode");
    %client.chatMessage("\c6If you have any suggestions, maps, or anything please tell us on our Discord server!");
    %client.chatMessage("\c5Discord\c6: <a:discord.gg/0xZ6AuKBFXWkCz9p>http://discord.gg/0xZ6AuKBFXWkCz9p</a>");
    %client.chatMessage("\c6You made need to \c5Page Up \c6to see this.");
}

function serverCmdRGB(%client, %time, %r, %g, %b) {
    if(isEventPending(%client.rgb["pend"])) {
        cancel(%client.rgb["pend"]);
        return messageClient(%client, '', "\c6RGB off");
    }
    %client.chatMessage("\c6RGB on at \c2" @ %client.rgb["time"] SPC "\c6time. Say \c2/rgbhelp \c6if you want to do more.");
    if(%time < 150) %time = 150;
    rgb(%client, %time, %r, %g, %b);
}

function serverCmdRGBhelp(%client) {
    %client.chatMessage("\c6Use \c3/rgb \c7[\c3time in ms\c7] \c7[\c1color1\c7] \c7[\c1color2\c7] \c7[\c1color3\c7]");
    %client.chatMessage("\c6Example: \c3/rgb 150 \c00.3 \c21 \c12 \c7(This will make it so rainbow's delay is set to 150, and colors are set to their own values)");
    %client.chatMessage("\c6If the time is set below 150 ms, it will not register and instead just have a 150 ms delay by default");
}

function rgb(%client, %time, %r, %g, %b) {
    if(isEventPending(%client.rgb["pend"]))
        cancel(%client.rgb["pend"]);
    if(!isObject(%client))
        return;
    %player = %client.player;
    if(!isObject(%player) || !%player.getControlObject())
        return schedule(3500, 0, rgb, %client, %r, %g, %b);
    %vehicle = %client.player.getControlObject() | 0;
    %brick = %vehicle.spawnBrick;
    %vehicle.setNodeColor("ALL",
                (%r > 0 ? getRandom(0.1, %r) : 1) SPC
                (%g > 0 ? getRandom(0.1, %g) : 1) SPC
                (%b > 0 ? getRandom(0.1, %b) : 1) SPC
                (%client.isShielded ? 0.4 : 1));
    %client.rgb["pend"] = schedule(150, 0, rgb, %client, %time, %r, %g, %b);
}

function serverCmdRespawn(%client) {
    if(!isObject(%player=%client.player) || !isObject(%mount=%player.getObjectMount()) || %mount.lastSavedPosition $= "" || $defaultMinigame.hasJustStarted)
        return;
    %mount.timeStamp = 0;
    %mount.isTimeWarping = 0;
    %mount.setTransform(%mount.lastSavedPosition);
}


function serverCmdRTV(%client) {
    if(isEventPending($RTV["schedule"]))
        cancel($RTV["schedule"]);
    if(strPos($RTV["clients"], %client) != -1)
        return %client.chatMessage("\c6You've already voted once!");
    $RTV["clients"] = %client SPC $RTV["clients"];
    announce("\c3" @ %client.getPlayerName() SPC "\c6rocked the vote! \c5" @ ($RTV["votes"]+=1) @ "\c6/\c5" @ (mCeil($Server::PlayerCount / 1.6)));
    if($RTV["votes"] >= (mCeil($Server::PlayerCount / 1.6))) {
        endRTV();
        return;
    }
    announce("\c530 \c6seconds left until the vote will end!");
    $RTV["schedule"] = schedule(30000, 0, endRTV);
}
function endRTV() {
    if(isEventPending($RTV["schedule"]))
        cancel($RTV["schedule"]);
    $RTV["clients"] = "";
    if($RTV["votes"] < (mCeil($Server::PlayerCount / 1.6))) {
        announce("\c3RTV failed! You can use \c3/rtv \c6to rock the vote next time!");
        $RTV["votes"] = 0;
        return;
    }
    $RTV["votes"] = 0;
    SK_NextTrack();
}

function serverCmdHardMode(%client) {
    %h=%client.hardMode = %client.hardMode ? 0 : 1;
    %client.chatMessage("\c0HardMode " @ (%h ? "on" : "off"));
}
