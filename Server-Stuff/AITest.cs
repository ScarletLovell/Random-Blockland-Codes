// 2016

function AIPlayer::setActivate(%ai, %data){
    talk(%ai);
}


if(isObject(bob))
{
    cancel($bobDoThings);
    Bob.delete();
}
$bobthebot = new AIPlayer(Bob) {
    name = "Bo2b";
    playerName = "b1ob";
    datablock = "PlayerStandardArmor";
    position = getRandom(50) SPC getRandom(50) SPC "1";
    scale = "1 1 1";
    team = "";
};
%r = getRandom(0, 1);
if(%r == 0) {
    // 100% racist
    bob.setNodeColor("headSkin", "1 0.878 0.612 1");
    bob.setNodeColor("rHand", "1 0.878 0.612 1");
    bob.setNodeColor("lHand", "1 0.878 0.612 1");
} else {
    // 100% not racist
    bob.setNodeColor("headSkin", "0.45 0.30 0.14 1");
    bob.setNodeColor("rHand", "0.45 0.30 0.14 1");
    bob.setNodeColor("lHand", "0.45 0.30 0.14 1");
}
bob.setNodeColor("chest", getRandom(1) SPC getRandom(1) SPC getRandom(1) SPC "1");
bob.setNodeColor("pants", getRandom(1) SPC getRandom(1) SPC getRandom(1) SPC "1");
bob.setNodeColor("pack", getRandom(1) SPC getRandom(1) SPC getRandom(1) SPC "1");
bob.unHideNode("Pack");
bob.setDecalName("hoodie");
bob.setShapeName("Bob", "8564862");

package bobCommands {
    function serverCmdFetch(%client, %person) {
        if(%person !$= "bob")
            return parent::serverCmdFetch(%client, %person);
        if(!%client.isAdmin)
            return;
        %player = %client.player;
        if(!isObject(bob))
            return messageClient(%client, '', "\c6Bot does not exist!");
        bob.position = %player.position;
    }
    function serverCmdFind(%client, %person) {
        if(%person !$= "bob")
            return parent::serverCmdFind(%client, %person);
        if(!%client.isAdmin)
            return;
        %player = %client.player;
        if(!isObject(bob))
            return messageClient(%client, '', "\c6Bot does not exist!");
        %player.position = bob.position;
        //serverCmdFind(%client);
    }
    function Armor::onTrigger(%armor, %player, %trigger, %active) {
        parent::onTrigger(%armor, %player, %trigger, %active);
        if(%trigger == 4 && %active == 1 && %player.getClassName() !$= "AIPlayer") {
            %player.isJetting = 1;
        }
        else if(%trigger == 4 && %active == 0 && %player.getClassName() !$= "AIPlayer")
            %player.isJetting = 0;
        if(%trigger == 2 && %active == 1) {
            %player.isJumping = 1;
        }
        else if(%trigger == 2 && %active == 0 && %player.getClassName() !$= "AIPlayer")
            %player.isJumping = 0;
    }
};
activatePackage(bobCommands);

function Player::SetValue(%pl, %value) {
    if(%value $= "isJumping")
        %pl.isJumping = 0;
    else
        %pl.isJetting = 0;
}

function AIPlayer::FollowPlayer(%ai, %player) {
    if(isEventPending(%ai.followPlayer))
        cancel(%ai.followPlayer);
    if(!isObject(%player) || getSimTime() - %ai.followTime < 50) {
        %ai.JetFollowing = 0;
        return %ai.isFollowing = false;
    }
    else {

        %ai.stop();
        if(%player.isJetting || getWord(%player.getPosition(), 2) > getWord(%ai.getPosition(), 2) + 2) {
            %jumping = 1;
            %ai.setJumping(1);
            %ai.setJetting(1);
            %ai.JetFollowing = 1;
        }
        else {
            %ai.setJetting(0);
            %ai.JetFollowing = 0;
        }
        if(getWord(%player.getPosition(), 2) < getWord(%ai.getPosition(), 2) + -2)
            %ai.setCrouching(1);
        if(%player.isJumping)
            %ai.setJumping(1);
        else if(!%jumping || !%player.isJumping)
            %ai.setJumping(0);
        %ai.isFollowing = true;
        %ai.followTime = getSimTime();
    }
    //if(isObject(%ai.getControlObject()))
    //    if(%ai.getControlObject().getClassName() $= "WheeledVehicle")
    //        %ai.dismount();
    if(%ai.stuck < 4)
        initContainerBoxSearch(%ai.getPosition(), "3 3 3", $typeMasks::PlayerObjectType);
    while(isObject(%find = containerSearchNext())) {
        if(%find == %player)
            return %ai.schedule(450, FollowPlayer, %player);
    }
    if(isObject(containerRayCast(%ai.getHackPosition(), vectorAdd(%ai.getHackPosition(), VectorScale(%ai.getForwardVector(), 4)), $TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::VehicleObjectType, %ai))) {
        %wall = 1;
        %ai.setJumping(1);
        %ai.setjetting(1);
    }

    %ai.setMoveDestination(%player.getPosition());
    %pos = getWord(%ai.position, 2) + 3;
    if(getWord(%player.position, 2) > %pos && !%ai.JetFollowing) {
        %ai.jumps++;
        if(%ai.jumps > 10) {
            serverCmdMessageSent(%ai, "I lost track of the player" SPC %player.client.name);
            %ai.jumps = 0;
            if(%ai.isJumping())
                %ai.setJumping(0);
            %ai.setMoveDestination(%ai.position);
            %ai.isFollowing = false;
            %ai.JetFollowing = 0;
            return %ai.smoothWander();
        }
        %ai.setJumping(1);
    }
    else {
        %ai.jumps = 0;
        if(%ai.isJumping() && !%wall)
            %ai.setJumping(0);
    }
    %ai.setAimObject(%player);
    %ai.followPlayer = %ai.schedule(150, FollowPlayer, %player);
}
function AIPlayer::AvoidPlayer(%ai, %player) {
    if(isEventPending(%ai.avoidPlayer))
        cancel(%ai.avoidPlayer);
    if(!isObject(%player) || !isObject(%ai))
        return;
    %ai.setAimObject(%player);
    initContainerBoxSearch(bob.getPosition(), "20 20 20", $typeMasks::PlayerObjectType);
    while(isObject(%find = containerSearchNext())) { // check for people
        if(%find == %player) {
            //talk("found" SPC %find.client.name SPC "attempting to avoid");

        }
    }
    %ai.avoidPlayer = %ai.schedule(150, AvoidPlayer, %player);
}
function BobDoThings()
{
    cancel($BobDoThings);
    %ai = bob;
    if(!isObject(%ai))
        return cancel($BobDoThings);
    %actions = "setjetting setjumping setcrouching activateStuff";
    %r = getRandom(0, 10);
    if(%r == getRandom(0, 10))
        %ai.smoothWander();
    if(%ai.isCrouching())
        %ai.setCrouching(0);
    if(getWord(%ai.position, 2) > getWord(%ai.lastPosition, 2) && getRandom(5) == 0)
        %ai.setCrouching(1);
    if(%ai.isJumping())
        %ai.schedule(150, setJumping, 0);
    if(%ai.isJetting() && !%ai.JetFollowing)
        %ai.schedule(350, setJetting, 0);
    initContainerBoxSearch(bob.getPosition(), "10 10 10", $typeMasks::PlayerObjectType);
    while(isObject(%find = containerSearchNext())) {
        if(%find.getClassName() !$= "AIPlayer") {
            %r = getRandom(0, 5);
            if(%r == getRandom(0, 5))
                continue;
            %ai.setAimObject(%find);
            %rad = getRandom(15);
            if(%r = getRandom(15) == %rad)
                %ai.playThread(0, activate2);
            if(!%ai.isFollowing)
                %ai.setMoveDestination(%find.getTransform());
            continue;
        }
    }
    if(!%ai.isFollowing)
        %ai.setmoveDestination(getRandom(50) SPC getRandom(50) SPC getRandom(50));
    %action = getWord(%actions, getRandom(0, 3));
    if(%action $= "setCrouching" && %ai.JetFollowing)
        return $BobDoThings = schedule(1000, 0, BobDoThings);
    %ai.lastPosition = %ai.position;
    %ai.schedule(150, %action, getRandom(0, 1));
    $BobDoThings = schedule(1000, 0, BobDoThings);
}

if(isObject(bob))
    bobDoThings();
