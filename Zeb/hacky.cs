
function Zeb_getFarthestWall(%self, %pos, %away) {
    %farthestBrick = %index = %farthest = 0;
    %bricks = "";
    %start = getSimTime();
    initClientBrickSearch(%pos, %away SPC %away SPC 1);
    while(%brick=clientBrickSearchNext()) {
        if($zebLastAimBrick == %brick)
            continue;
        if(%lastBrick == %brick)
            continue;
        if(vectorLen(%brick.getObjectBox()) < 1.5)
            continue;
        if((getSimTime() - $zebRecentHit[%brick]) < 60000)
            continue;
        if(!%brick.isRendering())
            continue;
        %lastBrick = %brick;
        //echo("new brick " @ vectorDist(%brick.getPosition(), %pos) SPC "|" SPC %brick);
        %bricks[%index] = %brick;
        %index++;
    }
    for(%i=0;%i < %index;%i++) {
        %brick = %bricks[%i];
        if(vectorDist(getWords(%brick.getPosition(),0,1) SPC 0, getWords(%pos,0,1) SPC 0) > %farthest) {
            %ray = initClientRaycast(%pos, %brick.getPosition(), 0.5);
            if(%ray != 0 && %ray == %brick) {
                %farthestBrick = %brick;
                %farthest = vectorDist(%brick.getPosition(), %pos);
            }
        }
    }
    //schedule(50, 0, commandToServer, 'setZebPos', %farthest.getPosition());
    return %farthestBrick;
}

function Zeb_isAnythingAbove(%self, %pos) {
    %up = vectorAdd(%pos, "0 0 5");
    initClientBrickSearch(%up, "1 1 5");
    if(isObject(%brick=clientBrickSearchNext())) {
        return %brick;
    }
    return false;
}

function Zeb_isAnythingInFront(%self, %pos) {
    %fwd = %self.getForwardVector();
    %front = vectorAdd(vectorAdd(%pos, vectorScale(%fwd, 1.2)), "0 0 2");
    //%front = vectorSub(vectorAdd(vectorScale(%self.getForwardVector(), %i), %pos), "0 0 1");
    initClientBrickSearch(%front, "1 1 1");
    if(isObject(%brick=clientBrickSearchNext())) {
        %front = vectorAdd(vectorAdd(%pos, vectorScale(%fwd, -2)), "0 0 2");
        initClientBrickSearch(%front, "1 1 1");
        if(!isObject(clientBrickSearchNext()))
            return %brick;
    }
    return false;
}
function Zeb_isAnythingLeft(%self, %pos) {
    for(%i=1;%i < 2;%i++) {
        %left = vectorAdd(vectorAdd(%pos, vectorCross(%self.getForwardVector(), "0 0 "@%i)), "0 0 1");
        initClientBrickSearch(%left, "1 1 6");
        if(isObject(%brick=clientBrickSearchNext()))
            return %brick;
    }
    return false;
}
function Zeb_isAnythingRight(%self, %pos) {
    for(%i=1;%i < 2;%i++) {
        %right = vectorAdd(vectorAdd(%pos, vectorCross(%self.getForwardVector(), "0 0 "@%i*-1)), "0 0 1");
        initClientBrickSearch(%right, "1 1 6");
        if(isObject(%brick=clientBrickSearchNext()))
            return %brick;
    }
    return false;
}
function Zeb_isAnythingInFrontFar(%self, %pos) {
    for(%i=5;%i < 15;%i++) {
        %front = vectorSub(vectorAdd(vectorScale(%self.getForwardVector(), %i), %pos), "0 0 1");
        initClientBrickSearch(%front, "1 1 6");
        if(isObject(clientBrickSearchNext()))
            return true;
    }
    return false;
}
function Zeb_canJump(%self, %pos, %jumpForce) {
    %fwd = %self.getForwardVector();
    %front = vectorAdd(vectorAdd(%pos, vectorScale(%fwd, 1.2)), "0 0 6");
    initClientBrickSearch(%front, "1 1 3");
    if(!isObject(%brick=clientBrickSearchNext())) {
        return true;
    }
    return false;
}

function Zeb_isOnGround(%self, %pos) {
    %fwd = %self.getForwardVector();
    for(%i=0;%i < 3;%i++) {
        %front = vectorSub(vectorAdd(%pos, vectorScale(%fwd, 2)), "0 0 "@%i);
        //%front = vectorSub(vectorAdd(vectorScale(%self.getForwardVector(), %i), %pos), "0 0 1");
        initClientBrickSearch(%front, "1 1 3");
        if(isObject(%brick=clientBrickSearchNext()))
            return true;
    }
    echo("no ground?");
    return false;
}

function BrickIsDoor(%brick) {
    return (strPos(%brick.getDatablock().iconName, "Brick_Doors") != -1);
}

function doAim(%Aim, %self) {
    // nah
}
function getmyeyepoint(%self) {
    // nah
}

function initClientRaycast(%start,%end,%step)
{
    // nah, if person who made this wants this out then so be it
    // otherwise, no.
    return 0;
}

function ConfirmPlayer(%playerName) {
	for(%i=serverConnection.getCount();%i > 0;%i--) {
        %player = serverConnection.getObject(%i);
        if(!isObject(%player))
            continue;
        if(%player.getClassName() $= "Player") {
            if(strPos(%player.getShapeName(), %playerName) != -1)
                return %player;
        }
    }
	return 0;
}

function ZebFollowPlayer(%off, %player) {
    // nah
}
