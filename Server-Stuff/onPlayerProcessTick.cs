// ~ MoveHandler - Created by qoh
// ? https://github.com/qoh/MoveHandler
// ? Used to grab the movements of players, record them, and use them for spectating and strafing
//  onPlayerProcessTick(PLAYER, moveArg [6 fields])

function onPlayerProcessTick(%player, %moveArg) {
    %client = %player.client;
    if(!isObject(%client))
        return;
    %a = (getSimTime() - %player.lastTick);
    %player.lasttick = getSimTime();
    %ad = getWord(%moveArg, 0);
    %ws = getWord(%moveArg, 1);
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
}
