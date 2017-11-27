if(!isObject(Shape_Col1))
	datablock StaticShapeData(Shape_Col1)
	{
	        shapeFile = "config/Cube_Col1.dts";
	};
if(isObject($Brickie[0]))
    $Brickie[0].delete();
if(!isObject($Brickie[0]))
    $Brickie[0] = new StaticShape() {
            dataBlock = Shape_Col1;
            //position = "0 0 355";
            position = "-2 -8 6";
            scale = "5 15 5";
    };
if(isObject($Brickie[1]))
    $Brickie[1].delete();
if(!isObject($Brickie[1]))
    $Brickie[1] = new StaticShape() {
            dataBlock = Shape_Col1;
            //position = "0 0 355";
            position = "-80 -8 6";
            scale = "5 15 5";
    };

function Bleechers(%client) {
    %player = %client.player;
    %r = getRandom(0, 1);
    if(%r < 1) {
        %start = "-12.3 -34.4 9";
        %end = "-68.9 -34.4 9";
    } else {
        %start = "-68.9 18.3 9";
        %end = "-12.3 18.3 9";
    }
    for(%i=0;%i < 3;%i++) {
        %s[%i] = getWord(%start, %i);
        %e[%i] = getWord(%end, %i);
    }
    %player.position = getRandom(%s0, %e0) SPC getRandom(%s1, %e1) SPC getRandom(%s2, %e2);

    //%player.u
}

function giveBall(%player) {
    if(!isObject(%player))
        return;
    %player.mountImage("DodgeballImage", 0);
}
function moveCheck() {
    if(isEventPending($dodgeBallMove))
        cancel($dodgeBallMove);
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        if(!isObject(%client.player) || !isObject(%client))
            continue;
        if(%client.player.inBleechers > 0 || !isObject(%client.minigame))
            continue;
        if(%client.player.lastPos == %client.player.getPosition()) {
            %client.centerPrint("<font:impact:25>\c6Keep moving\c0" SPC %client.player.moving--, 1.1);
            if(%client.player.moving <= 0) {
                bleechers(%client);
                %client.player.inBleechers = 1;
                %client.slyrTeam.living --;
                checkWin(%client);
                continue;
            }
        }
        else
            %client.player.moving = 10;
        %client.player.lastPos = %client.player.getPosition();
    }
    $dodgeBallMove = schedule(1000, 0, moveCheck);
} moveCheck();

function checkWin(%client) {
    if(%client.slyrTeam.living <= 0) {
        if(%client.slyrTeam.name $= "red")
            %client.minigame.endRound($team["blue"]);
        else
            %client.minigame.endRound($team["red"]);
    }
}

function clearBalls() {
    for(%i=missionCleanup.getCount()-1;%i>=0;%i--)
    {
        %obj = missionCleanup.getObject(%i);
        if(%obj.getClassName() $= "Projectile" || %obj.getClassName() $= "Item")
        {
            %obj.delete();
            %projectileCount++;
        }
    }
}

package DodgeBall {
	function serverCmdLeaveMinigame(%client) {
		%client.slyrTeam.living-=1;
		checkWin(%client);
		parent::serverCmdLeaveMinigame(%client);
	}
	function servercmdSit(%client) {
		if(%client.player.inBleechers < 1)
			return;
		parent::serverCmdSit(%client);
	}
    function Slayer_MinigameSO::onReset(%this) {
        parent::onReset(%this);
        clearBalls();
        for(%i=0;%i < clientGroup.getCount();%i++) {
            %client = clientGroup.getObject(%i);
            if(isObject(%client.minigame)) {
				%client.player.lastClick = getSimTime();
                %client.team = %client.slyrTeam;
                $team[%client.slyrTeam.name] = %client.slyrTeam;
                %client.slyrTeam.living = %client.slyrTeam.getLiving();
                giveBall(%client.player);
            }
        }
    }
    function GameConnection::onClientLeaveGame(%client) {
        parent::onClientLeaveGame(%client);
        if(isObject(%client.minigame) && %client.team != 0) {
            checkWin(%client);
            %client.team.living --;
        }
    }
    function Armor::onTrigger(%armor, %player, %trigger, %active) {
        parent::onTrigger(%armor, %player, %trigger, %active);
        if(%trigger != 0 && %active != 1 )
            return;
		%a = (%player.lastClick - getSimTime());
		if(%a < 1500)
			return %player.client.centerPrint("\c6You cannot catch yet" SPC %player.lastClick, 0.5);
		%player.lastClick = getSimTime();
        %r = getRandom(0, 5);
		%pos = vectorAdd(%player.getPosition(), vectorScale(%player.getForwardVector(), 4));
		//talk(%player.client.name);
        initContainerBoxSearch(%pos, "2 2 5", ($TypeMasks::ProjectileObjectType | $TypeMasks::ItemObjectType));
        if(isObject(%targ = containerSearchNext()) && isObject(%player.client.minigame) && !%targ.hasBounced && !%player.inBleechers && !%player.hasSportBall && %targ.namebase.player.inBleechers < 1 && %targ.nameBase.name !$= %player.client.name && %targ.nameBase && %targ.team !$= %player.client.slyrTeam.name) {
            centerPrint(%player.client, "<shadow:2:5><font:impact:30>\c6CAUGHT", 0.5);
            //talk(%targ.nameBase.name);
            bleechers(%targ.nameBase);
            %targ.namebase.player.inBleechers = 1;
            %targ.nameBase.slyrTeam.living -=1;
            announce("\c2" @ %player.client.name SPC "\c0caught\c3" SPC %targ.nameBase.name @ "\c0's ball\c6,\c3" SPC
                %targ.namebase.slyrTeam.living SPC "\c6left on the" SPC %targ.namebase.slyrTeam.name SPC "team");
            giveBall(%player);
            checkWin(%targ.nameBase);
            %targ.delete();
        }
    }
    function player::activateStuff(%this) {
        parent::activateStuff(%this);
    }
    function dodgeBallProjectile::onExplode(%this, %obj) {
        %item = new item() {
    		dataBlock = "dodgeBallItem";
    		scale = %obj.getScale();
    		minigame = getMiniGameFromObject(%obj);//%obj.minigame;
            position = getWord(%obj.getPosition(), 0) SPC getWord(%obj.getPosition(), 1) SPC 0.8;
    		//spawnBrick = %obj.spawnBrick;
    	};
        //talk(%item.getClassNAme());
        parent::onExplode(%this, %obj);
    }
    function dodgeballProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal) {
        //initContainerBoxSearch(%pos, "1 1 1", $TypeMasks::PlayerObjectType);
		%targ = %col;
        if(isObject(%col.client.minigame) && !%obj.hasBounced) {
            %client = %col.client;
            if(%client.slyrTeam.name !$= %obj.team && %col.inBleechers < 1) {
                if(%col.hasSportBall) {
                    %r = getRandom(0, 20);
                    if(%r == getRandom(0, 20))
                        return centerPrint(%client, '', "\c6your ball BLOCKED", 0.5);
                }
                //%targ.instantRespawn();
                %col.spawnExplosion("PlayerSootProjectile", "1.2 1.2 1.2");
                //talk(getRegion(%col, %col.getPosition(), 1));
                //%targ.hideNode(getRegion(%col, %col.getPosition(), 1));
                %client.slyrTeam.living -=1;
                bleechers(%client);
                %col.inBleechers = 1;
                %col.unMountImage(0);
                %r = getRandom(0, 3);
                    if(%r == 0) %a = ("\c3" @ %client.name SPC "\c0was sent downtown by\c2" SPC %obj.namebase.name);
                    else if(%r == 1) %a = ("\c3" @ %client.name SPC "\c0didn't dodge very well from\c2" SPC %obj.nameBase.name);
                    else if(%r == 2) %a = ("\c3" @ %client.name SPC "\c0was hit by\c2" SPC %obj.nameBase.name);
                    else if(%r == 3) %a = ("\c3" @ %client.name SPC "\c0tried too hard to dodge\c2" SPC %obj.nameBase.name);
                    else if(%r == 4) %a = ("\c3" @ %client.name SPC "\c0got slapped with a ball from\c2" SPC %obj.nameBase.name);
				if(isObject(%obj.nameBase))
                    announce(%a @ "\c6, \c3" @ %client.slyrTeam.living SPC "\c6left on the" SPC %client.slyrTeam.name SPC "team");
				else
					announce("\c3" @ %client.name SPC "\c0got hit by something unknown..." SPC "\c3" @ %client.slyrTeam.living SPC "\c6left on the" SPC %client.slyrTeam.name SPC "team");
                checkWin(%client);
            }
            %obj.lastHit = %targ.client.slyrTeam.name;
        }
    	parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
    }
    function dodgeballImage::onFire(%this,%obj,%slot) {
        parent::onFire(%this, %obj, %slot);
        initContainerBoxSearch(%pos, "1 1 1", $TypeMasks::ProjectileObjectType);
        if(isObject(%targ = containerSearchNext())) {
            %targ.team = %obj.client.slyrTeam.name;
            %targ.nameBase = %obj.client;
        }
    }
    function serverCmdSuicide(%client) {
        if(%client.minigame)
            return messageClient(%client, '', "\c6Suicide is disabled in the minigame");
        parent::serverCmdSuicide(%client);
    }
};
activatePackage(DodgeBall);
