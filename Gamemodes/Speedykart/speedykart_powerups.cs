
$SK_powerups = "Fireball Boost Growth Rocket Jump Shield";
function SpeedyKart_RandomizePowerup(%player, %am) {
    if(isEventPending(%player.randomizePowerup))
        cancel(%player.randomizePowerup);
    %r = getRandom(0, getWordCount($SK_powerups));
    %player.client.centerPrint("<just:left>" @ getWord($SK_powerups, %r), 1);
    if(%am >= 10) {
        SpeedyKart_GiveRandomPowerup(%player, %r);
        return;
    }
    %player.randomizePowerup = schedule(20, 0, SpeedyKart_RandomizePowerup, %player, %am+=1);
}

function SpeedyKart_GiveRandomPowerup(%player, %r) {
    switch(%r) {
        case 0: %powerup = "Fireball"; %numOfPowerups = 1;
        case 1: %powerup = "Boost"; %numOfPowerups = 1;
        case 2: SpeedyKart_GiveRandomPowerup(%player);//%powerup = "Growth"; %numOfPowerups = 1;
        case 3: %powerup = "Rocket"; %numOfPowerups = 3;
        case 4: %powerup = "Jump"; %numOfPowerups = 3;
        case 5: %powerup = "Shield"; %numOfPowerups = 1;
        case 6:
            if($defaultMinigame.hasJustStarted || isObject($defaultMinigame.hasStar) || $defaultMinigame.lastStar == %player.client) {
                SpeedyKart_GiveRandomPowerup(%player);
                return;
            } else {
                $defaultMinigame.lastStar = %player.client;
                %powerup = "Star"; %numOfPowerups = 1;
                $defaultMinigame.hasStar = %player.client;
                announce("\c3" @ %player.client.getPlayerName() SPC "\c6got the \c3STAR \c6powerup. Use it wisely. \c7[Only one person gets this per round]");
            }
    }
    if(%player.lastPowerup $= %powerup) {
        SpeedyKart_GiveRandomPowerup(%player);
        return;
    }
    if(%player.powerup !$= "" && %player.SpeedyKart["receivingPowerup"] $= "") {
        %player.SpeedyKart["receivingPowerup", "time"] = 10;
        %player.SpeedyKart["receivingPowerup"] = %powerup;
        %player.SpeedyKart["receivingPowerup", "num"] = %numOfPowerups;
    } else {
        %player.numOfPowerups = %numOfPowerups;
        %player.powerup = %powerup;
        %player.lastPowerup = %powerup;
    }
}
function GameConnection::UsePowerup(%client) {
    if(!isObject(%player=%client.player) && trim(%powerup=%player.powerup) $= "") {
        return;
    }
    if(!isObject(%mount=%player.getObjectMount())) {
        %client.centerPrint("You are not in a vehicle!", 1);
        return;
    }
    if(isObject(%player.UsedPowerup)) {
        %client.centerPrint("You aleady have a powerup in progress, wait for it to die out.", 1);
        return;
    }
    if(%player.powerup $= "Fireball") {
        %emitter = %player.usedPowerup = new ParticleEmitterNode() {
            dataBlock = GenericEmitterNode;
            emitter = "BurnEmitterA";
            name = "Fireball";
            shooter = %client;
            vehicle = %player.getObjectMount();
            forward = (%player.lookingBehind ? vectorScale(%player.getForwardVector(), -1) : %player.getForwardVector());
            speed = mCeil(vectorLen(%player.getVelocity()) / 5500);
        };
        %emitter.setEmitterDataBlock("BurnEmitterA");
        %emitter.setTransform(%player.getTransform());
        MissionCleanup.add(%emitter);
        SpeedyKart_ChangePowerupLoc(%emitter);
    } else if(%player.powerup $= "Boost") {
        %emitter = %player.usedPowerup = new ParticleEmitterNode() {
            dataBlock = GenericEmitterNode;
            emitter = "FogEmitterA";
            name = "Fireball";
            shooter = %client;
            vehicle = %player.getObjectMount();
            forward = %player.getForwardVector();
            speed = mCeil(vectorLen(%player.getVelocity()) / 5500);
        };
        %emitter.setEmitterDataBlock("FogEmitterA");
        %emitter.setTransform(%player.getTransform());
        MissionCleanup.add(%emitter);
        %player.getObjectMount().setVelocity(vectorScale(%player.getObjectMount().getVelocity(), 2));
        SpeedyKart_PowerupFollowPlayer(%player, %emitter);
        %emitter.schedule(5000, delete);
    } else if(%player.powerup $= "Growth") {
        %player.getObjectMount().setScale("1.5 1.5 1.5");
        %player.setScale("1.5 1.5 1.5");
        %player.powerup = "";
        %player.getObjectMount().schedule(15000, setScale, "1 1 1");
        %player.schedule(15000, setScale, "1 1 1");
        SpeedyKart_Giveachievement(%client, 3);
    } else if(%player.powerup $= "Rocket") {
        %vel = vectorScale(%mount.getVelocity(), 3);
        %p = new Projectile() {
            dataBlock = RocketLauncherProjectile;
            initialVelocity = (%player.lookingBehind ? vectorScale(%vel, -1) : %vel);
            initialPosition = vectorAdd(vectorAdd(%mount.position, vectorScale(%mount.getForwardvector(), 2)), "0 0 1");
            sourceObject = %player;
            sourceSlot = 1;
            client = %client;
            shooter = %player;
            velocity = vectorLen(%vel);
            canAutoAim = true;
            minigame = %client.minigame;
        };
        MissionCleanup.add(%p);
        SpeedyKart_ProjectileSchedule(%p);
    } else if(%player.powerup $= "Jump") {
        if(%mount.isOnGround) {
            %mount.setVelocity(vectorAdd(getWords(%mount.getVelocity(), 0, 1) SPC "0", "0 0 15"));
        } else
            return;
    } else if(%player.powerup $= "Shield") {
        %player.isShielded = true;
        %color = %mount.color;
        %mount.setNodeColor("ALL", getWords(%color, 0, 2) SPC "0.3");
        %emitter.schedule(5000, delete);
        %mount.schedule(5000, setNodeColor, "ALL", getWords(%color, 0, 3));
        %mount.schedule(5000, setNodeColor, "RHand", %mount.RHand);
        %mount.schedule(5000, setNodeColor, "LHand", %mount.LHand);
        schedule(5000, 0, eval, %player @ ".isShielded = 0;");
    } else if(%player.powerup $= "Star") {
        %emitter = new ParticleEmitterNode() {
            dataBlock = GenericEmitterNode;
            emitter = "WinStarEmitter";
            name = "Star";
            shooter = %client;
            vehicle = %player.getObjectMount();
            forward = %player.getForwardVector();
            speed = mCeil(vectorLen(%player.getVelocity()) / 5500);
        };
        %emitter.setEmitterDataBlock("WinStarEmitter");
        %emitter.setTransform(%player.getTransform());
        MissionCleanup.add(%emitter);
        SpeedyKart_PowerupFollowPlayer(%player, %emitter);
        SpeedyKart_SuperBoost(%player);
    }
    %player.numOfPowerups--;
    if(%player.numOfPowerups <= 0)
        %player.powerup = "";
}
function SpeedyKart_SuperBoost(%player) {
    if(isEventPending(%player.superBoostSchedule))
        cancel(%player.superBoostSchedule);
    if(!isObject(%mount=%player.getObjectMount()) || %player.superBoost >= 15)
        return;
    %player.superBoost++;
    %mount.setVelocity(vectorScale(%player.getObjectMount().getVelocity(), 1.2));
    %player.superBoostSchedule = schedule(250, 0, SpeedyKart_SuperBoost, %player);
}
function SpeedyKart_PowerupFollowPlayer(%player, %emitter) {
    cancel(%emitter.follow);
    if(!isObject(%emitter) || !isObject(%player) || !isObject(%mount=%player.getObjectMount())) {
        if(isObject(%emitter))
            %emitter.delete();
        return;
    }
    if(%emitter.name $= "Star") %emitter.position = vectorAdd(%mount.position, "0 0 4");
    else %emitter.position = %mount.position;
    %emitter.inspectPostApply();
    %emitter.follow = schedule(100, 0, SpeedyKart_PowerupFollowPlayer, %player, %emitter);
}
function SpeedyKart_ChangePowerupLoc(%powerup, %time) {
    if(isEventPending(%powerup.schedule))
        cancel(%powerup.schedule);
    if(!isObject(%powerup) || %powerup.dataBlock !$= "GenericEmitterNode")
        return;
    if(trim(%powerup.forward) $= "" || %time >= 250) {
        %powerup.delete();
        return;
    }
    %pos = %powerup.position = (vectorAdd(%powerup.position, vectorScale(%powerup.forward, (%time/2500) + %powerup.speed)));
    %powerup.inspectPostApply();
    initContainerBoxSearch(%pos, "2 2 2", $TypeMasks::VehicleObjectType);
    while(isObject(%hit = containerSearchNext())) {
        if(%powerup.vehicle.getId() == %hit.getId() || !isObject(%hit.getMountedObject(0)) || vectorLen(%hit.getVelocity()) <= 0)
            continue;
        %player = %hit.getMountedObject(0);
        if(%player.isShielded) {
            SpeedyKart_GiveAchievement(%player.client, 6);
            %powerup.shooter.chatMessage("\c6You hit \c3" @ %player.client.getPlayerName() @ " \c6but they were shielded!");
            %powerup.delete();
            return;
        }
        %player.client.centerPrint("You were hit with a \c3" @ %powerup.name @ "\c0 powerup!", 2);
        %hit.lastPosition = %hit.position;
        if(isEventPending(%hit.PutOnGround))
            cancel(%hit.PutOnGround);
        %hit.PutOnGround = schedule(1000, 0, SpeedyKart_PutOnGround, %hit);
        %hit.setVelocity("0 0 18");
        %powerup.shooter.AddExp(2);
        %powerup.shooter.chatMessage("\c6You hit \c3" @ %player.client.getPlayerName() @ " \c6and gained \c52 EXP\c6!");
        SpeedyKart_Giveachievement(%powerup.shooter, 1);
        %powerup.delete();
        return;
    }
    %time += 1;
    %powerup.schedule = schedule(10, 0, SpeedyKart_ChangePowerupLoc, %powerup, %time);
}
function SpeedyKart_PutOnGround(%vehicle) {
    if(isEventPending(%vehicle.PutOnGround))
        cancel(%vehicle.PutOnGround);
    //talk(vectorDist(%vehicle.position, %vehicle.lastPosition));
    %vehicle.position = %vehicle.lastPosition;
}
function SpeedyKart_ProjectileSchedule(%projectile) {
    if(!isObject(%projectile)) {
        return;
    }
    if(!%projectile.canAutoAim)
        return schedule(35, 0, SpeedyKart_ProjectileSchedule, %projectile);
    initContainerBoxSearch(%projectile.position, "20 20 20", $TypeMasks::PlayerObjectType);
    if(isObject(%hit = containerSearchNext()) && %hit != %projectile.shooter) {
        %vector = vectorScale(vectorNormalize(vectorSub(%hit.position, %projectile.position)), (20+(%projectile.velocity/2)));
        %p = new Projectile() {
            dataBlock = RocketLauncherProjectile;
            initialVelocity = %vector;
            initialPosition = %projectile.position;
            sourceObject = %projectile.player;
            sourceSlot = 1;
            client = %projectile.client;
            minigame = %projectile.client.minigame;
        };
        MissionCleanup.add(%p);
        %projectile.delete();
        SpeedyKart_ProjectileSchedule(%p);
    }
    schedule(35, 0, SpeedyKart_ProjectileSchedule, %projectile);
}
