function BK_AddBot() {
    if($BK_Bot["max"] < 0)
        $BK_Bot["max"] = 0;
    $BK_Bot[$BK_Bot["max"]] = %bot = new AIPlayer(Bob) {
        number = $BK_Bot["max"];
        name = "Bob";
        playerName = "bob";
        datablock = "PlayerStandardArmor";
        position = getRandom(50) SPC getRandom(50) SPC "1";
        scale = "1 1 1";
        team = "";
        isBot = true;
    };
    %bot.position = (%spawn = BK_FindSpawn(%bot)).position;
    %veh = findVehicleSpawn(%spawn.position);
    if(!isObject(%veh)) {
        %bot.delete();
        warn("[Func, BK_AddBot] Bot could not be made due to unfound spawn");
        return;
    }
    setVehicleToSpawn(%bot, "SpeedKartVehicle");
    %bot.setName("BK_Bot" @ %bot.number);
    %bot.setShapeName("BK_Bot" @ %bot.number, "8564862");
    $BK_Bot["max"]++;

    %veh.vehicle.mountObject(%bot, 0);
    %bot.setMoveY(1);

    %bot.sched = schedule(200, 0, BK_BotSchedule, %bot);
}

function BK_PrepareBotSpawn(%number) {
    $BK_SpawnedBots = %number;
    BK_RemoveBot(500);
    $BK_Bot["max"] = 0;
    BK_DoBotSpawn(%number);
}
function BK_DoBotSpawn(%number) {
    if($BK_Bot["max"] >= %number)
        return;
    BK_AddBot();
    schedule(500, 0, BK_DoBotSpawn, %number);
}

function BK_RemoveBot(%number) {
    if(%number > 0 || !isObject(%number)) {
        for(%i=0;%i <= %number;%i++) {
            if($BK_Bot["max"] < 1) {
                warn("No more bots can be removed!");
                return;
            }
            if(isObject($BK_Bot[$BK_Bot["max"]-1]))
                $BK_Bot[$BK_Bot["max"]-1].delete();
            warn("Removed " @ %number @ " bot(s)!");
            $BK_Bot["max"]--;
        }
    } else if(isObject(%number)) {
        %number.delete();
        $BK_Bot["max"]--;
        return;
    }
}

function BK_FindSpawn(%bot)
{ // Created by Buddy.
    if(!isObject(%bot))
        return;
    %db = $uiNameTable["Spawn Point"];
    %mbc = mainBrickGroup.getCount();
    for(%i=0;%i < %mbc;%i++) {
        %brickgroup = mainBrickGroup.getObject(%i);
        %bgc = %brickgroup.getCount();
        for(%o=0;%o < %bgc;%o++) {
            %brick = %brickgroup.getObject(%o);
            if(!isObject(%brick))
                continue;
            if(%brick.getDatablock() == %db) {
                %spawn = findVehicleSpawn(%brick.position);
                if(isObject(%spawn.vehicle) && %spawn.vehicle.getMountedObject(0))
                    continue;
                initContainerBoxSearch(%brick.getPosition(), "5 5 5", $TypeMasks::PlayerObjectType);
                if(isObject(%player = containerSearchNext())) {
                    continue;
                }
                return %brick;
            }
        }
    }
}

function BK_BotSchedule(%bot) {
    if(!isObject(%bot)) {
        if(isEventPending(%bot.sched))
            cancel(%bot.sched);
        return;
    }
    if(!isObject(%mount = %bot.getObjectMount())) {
        if(isEventPending(%bot.sched))
            cancel(%bot.sched);
        %bot.delete();
        return;
    }
    %speed = mFloor(vectorLen(%bot.getObjectMount().getVelocity()));
    if(%speed < 2 && !isEventPending($countDown["active"])) {
        %bot.prepareImpulse++;
        if(%bot.prepareImpulse > 5)
            %mount.applyImpulse(%mount.getWorldBoxCenter(), vectorScale(getRandom(-15, 15) SPC getRandom(-15, 15) SPC getRandom(3, 35), 3));
        if(%bot.prepareImpulse > 25) { // lets assume they're just stuck forever.
            //talk("bot delete");
            %bot.delete();
        }
    }
    else
        %bot.prepareImpulse = 0;
    if(vectorLen(%pos=%bot.position) != 0) {
        initContainerBoxSearch(%pos, "100 100 100", $TypeMasks::PlayerObjectType);
        if(isObject(%player = containerSearchNext())) {
            %bot.destination = %player.getPosition();
            %bot.setMoveDestination(%player.getPosition());
        }
    }
    %bot.sched = schedule(500, 0, BK_BotSchedule, %bot);
}
