function GameConnection::SendUI(%client) {
    if(!isObject(%client.player)) {
        cancel(%client.sched);
        return;
    }
    if(isObject(%client.player.getObjectMount())) {
        %speed = mFloor(vectorLen(%client.player.getObjectMount().getVelocity()));
        if(%speed < 1) {
            if(!isEventPending($countDown["active"])) {
                if(!$BK_DebugMode)
                    %client.player.isDying++;
                %ui = "<just:center>\c0You are stopped! \c3"@%client.player.isDying@" > 50 = death\c0!";
                if(%client.player.isDying > 50) {
                    %client.player.kill();
                }
                %client.bottomPrint(%ui);
            }
            else
                %ui = "<just:center>\c6You are in spawn";
        } else {
            %client.player.isDying = 0;
            %ui = "\c6Speed: \c3" @ mFloor(vectorLen(%client.player.getObjectMount().getVelocity()) / 0.5) @ "\c6BPS";
        }
    }
    else
        return;
    if(%client.player.isDrifting)
        %ui = %ui @ "<just:right>\c6Drifting!!";
    %client.bottomPrint(%ui);
    %client.sched = %client.schedule(128, SendUI);
}
