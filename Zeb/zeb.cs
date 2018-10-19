echo("   Preparing...");

luaExec("config/codes/zeb.lua");
exec("config/codes/chat.cs");
exec("config/codes/package.cs");
exec("config/codes/hacky.cs");
exec("config/codes/sim.cs");
//%pos = luaCall("getPlayerPosition", %pl);
setModPaths(getModPaths());

$defaultSayings = "";

function zebSchedule(%i) {
    if(isEventPending($zebSchedule))
        cancel($zebSchedule);
    if(%i != 1) {
        $zebIsActive = 0;
        moveForward(0);
        $mvYaw = 0;
        return;
    } else {
        $zebIsActive = 1;
    }
    if(!isObject(serverConnection) || !isObject(serverConnection.getControlObject())) {
        $zebSchedule = schedule(5000, 0, zebSchedule, %i);
        return;
    }
    if(isEventPending($ZebFollowPlayer))
		return;
    %noUpdate = false;
    %self = serverConnection.getControlObject();
    %db = %self.getDatablock();
    %jumpForce = %db.jumpForce / 216;
    %pos = %self.getPosition();
    if(vectorDist($lastPos, %pos) < 2 || getWord(fpsMetricsCallback(), 5) > 100) {
        if($lastPosTimeout > 20 && $lastPosTimeout < 30) {
            $zebCanOnlyAimAt = 0;
        }
        if($lastPosTimeout > 50) {
            //crouch(1);
            $mvYaw = 0.1;
        }
        if($lastPosTimeout > 80) {
            jump(1);
            commandToServer('alarm');
            commandToServer('activateStuff');
            commandToServer('sit');
            $lastPosTimeout = 0;
        }
        $lastPosTimeout++;
    } else {
        $lastPosTimeout = 0;
        $lastPos = %pos;
        crouch(0);
        jump(0);
    }
    %front = vectorSub(vectorAdd(vectorScale(%self.getForwardVector(), 2), %pos), "0 0 1");
    //commandToServer('messageSent', %front);
    //commandToServer('messageSent', %pos);
    %left = vectorAdd(vectorAdd(%pos, vectorCross(%self.getForwardVector(), "0 0 -1")), "0 0 1");
    %right = vectorAdd(vectorAdd(%pos, vectorCross(%self.getForwardVector(), "0 0 1")), "0 0 1");
    if(isObject(%brick=Zeb_isAnythingInFront(%self, %pos))) {
        if(Zeb_canJump(%self, %pos, %jumpForce)) {
            if(Zeb_isAnythingAbove(%self, %pos) == 0) {
                jump(1);
                schedule(100, 0, jump, 0);
            }
        } else {
            if(brickIsDoor(%brick)) {
                commandToServer('activateStuff', 1);
            }
        }
        %noUpdate = true;
    }
    if($zebCanOnlyAimAt != 0) {
        %b = $zebCanOnlyAimAt;
        if(vectorDist("0 0" SPC getWord(%pos, 2), "0 0" SPC getWord(%b.getPosition(), 2) > 2))
            $zebStuckAim = 0;
        %ray = initClientRaycast(getMyeyePoint(), %b.getPosition());
        if(!isObject(%b) || %ray != %b) {
            $zebStuckAim++;
            if($zebStuckAim > 5)
                $zebCanOnlyAimAt = 0;
        } else
            $zebStuckAim = 0;
        //else {
            doAim(%b.getPosition());
            %dist = vectorDist(%b.getPosition(), %pos);
            if(%dist < 6) {
                $zebRecentHit[%b] = getSimTime();
                $zebCanOnlyAimAt = 0;
                moveForward(0);
            } else {
                moveForward(1);
            }
        //}
    } else {
        %b = Zeb_GetFarthestWall(%self, getMyEyePoint(), 80);
        if(%b != 0) {
            $zebCanOnlyAimAt = %b;
            $zebLastAimBrick = %b;
            doAim(%b);
            moveForward(1);
            commandToServer('setZebPos', %pos, %b.getPosition());
        } else {
            if(Zeb_isAnythingInFront(%self, %pos) != 0) {
                if(Zeb_isAnythingLeft(%self, %pos) != 0) {
                    if(Zeb_isAnythingRight(%self, %pos) != 0)
                        $mvYaw = 3.14;
                    else
                        $mvYaw = getRandom() + 0.5;
                } else
                    $mvYaw = (getRandom() + 0.5) * -1;
            }
            %front = vectorAdd(vectorAdd(vectorScale(%self.getForwardVector(), 1.2), %pos), "0 0 1");
            initClientBrickSearch(%front, "1 1 1");
            if(isObject(%brick=clientBrickSearchNext())) {
                $mvYaw = getRandom(-1, 1);
                %noUpdate = true;
            }
            moveForward(1);
        }
    }
    %noUpdate = 1;
    if(getWord(%pos, 2) < 0.2) {
        if($isOnGround) {
            $mvYaw = (getRandom(0,1) ? getRandom()+1.9 : (getRandom()+1.9)*-1);
            $isOnGround = 0;
            %noUpdate = true;
        } else {
            if($noGround > 40) {
            }
            $noGround++;
        }
    } else {
        $noGround = $isOnGround = 1;
    }
    $zebSchedule = schedule(100, 0, zebSchedule, %i);
}
if($zebIsActive $= "")
    $zebIsActive = 0;

function Zeb_shuttingDown(%i) {
    if(%i > 20)
        return;
    $mvPitch = 0.1;
    schedule(50, 0, Zeb_shuttingDown, %i+1);
}
function Zeb_turnOn(%i) {
    if(%i > 15)
        return;
    $mvPitch = -0.1;
    schedule(50, 0, Zeb_turnOn, %i+1);
}

echo("   Ready!");
