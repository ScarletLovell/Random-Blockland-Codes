if(!isPackage(Slayer)) {
    warn("[BumperKart] PACKAGE REQUIRED: Slayer");
    return;
}
if(!$BumperKart_Exec) {
    if(isFile("./BumperKart_Commands.cs"))
        exec("./BumperKart_Commands.cs");
    if(isFile("./BumperKart_UI.cs"))
        exec("./BumperKart_UI.cs");
    if(isFile("./BumperKart_Bots.cs"))
        exec("./BumperKart_Bots.cs");
}

$BumperKart["version"] = "v0.8.2";
$defaultMinigame["maxRounds"] = 4;

deleteVariables("$DeathMessage_*");


function readTransformFile(%path) {
    %transform = filePath(%path) @ "/Transform.txt";
    if(isFile(%transform)) {
        %file = new FileObject();
        %file.openForRead(%transform);
        %trans = %file.readLine();
        %file.close();
        %file.delete();
    }
    return %trans;
}
// --- Taken From GameMode_SpeedKart --- \\
function SK_LoadTrack_Phase1(%filename)
{
    //suspend minigame resets
    $SK::MapChange = 1;

    //put everyone in observer mode
    %mg = $defaultMiniGame;
    if(!isObject(%mg))
    {
        error("ERROR: SK_LoadTrack( " @ %filename  @ " ) - default minigame does not exist");
        return;
    }
    %transform = filePath(getMapList($defaultMinigame["currentMap"])) @ "/Transform.txt";
    if(isFile(%transform)) {
        %file = new FileObject();
        %file.openForRead(%transform);
        %trans = %file.readLine();
        %file.close();
        %file.delete();
    }
    for(%i = 0; %i < %mg.numMembers; %i++) {
        %client = %mg.member[%i];
        %player = %client.player;
        if(isObject(%player)) {
            %player.delete();
            %client.createPlayer("0 0 " @ getRandom(999, 999999));
        }
        %camera = %client.camera;
        %camera.setFlyMode();
        %camera.mode = "Observer";
        %client.setControlObject(%camera);
        if(%trans !$= "")
            %client.camera.setOrbitPointMode(%trans, 0);
    }

   $loadMap = %fileName;
   $mapLoading = true;
   clearBricks2(0, 0);
}

function SK_LoadTrack_Phase2(%filename)
{
    talk("Loading BumperKart track" SPC %filename);

    //load environment if it exists
    %envFile = filePath(%fileName) @ "/environment.txt";
    if(isFile(%envFile)) {
        //echo("parsing env file " @ %envFile);
        //usage: GameModeGuiServer::ParseGameModeFile(%filename, %append);
        //if %append == 0, all minigame variables will be cleared
        %res = GameModeGuiServer::ParseGameModeFile(%envFile, 1);

        EnvGuiServer::getIdxFromFilenames();
        EnvGuiServer::SetSimpleMode();

        if(!$EnvGuiServer::SimpleMode) {
            EnvGuiServer::fillAdvancedVarsFromSimple();
            EnvGuiServer::SetAdvancedMode();
        }
    }

    //load save file
    schedule(10, 0, serverDirectSaveFileLoad, %fileName, 3, "", 2, 1);
    $defaultMinigame.schedule(650, reset);
    $defaultMinigame.schedule(700, respawnAll);
    for(%i=0;%i < clientGroup.getCount();%i++)
        clientGroup.getObject(%i).instantRespawn();
    schedule(200, 0,  eval, "$mapLoading = false;");
    schedule(200, 0, eval, "BK_PrepareBotSpawn(mFloor(($Pref::Server::MaxPlayers - $Server::PlayerCount) / 2));");
}

// --- Our Stuff and edited SpeedKart stuff --- \\
function changeMap(%number) {
    if(!%number)
        %number = $defaultMinigame["currentMap"]++;
    if(%number >= $BumperKart["maxMaps"] || $defaultMinigame["currentMap"] >= $BumperKart["maxMaps"]) {
        %number = 0;
        $defaultMinigame["currentMap"] = $BumperKart["map" @ %number];
    }
    %map = $BumperKart["map" @ %number];
    talk("Attempting to load\c3" SPC %map);
    SK_LoadTrack_Phase1(%map);
}

function serverCmdNextTrack(%client) {
    if(%client.isSuperAdmin)
        changeMap();
    else
        return %client.chatMessage("\c6You must be \c2Super Admin \c6to do this!");
}


function clearBricks2(%bgI,%initBC)
{
    cancel($clearBricks2);
    if(%initBC $= "" || %initBC < 10)
        %initBC = getBrickCount();
    if(%bgI >= mainBrickGroup.getCount())
    {
        cancel($clearBricks3);
        cancel($clearBricks4);
        $clearBricks3 = schedule(%GD + 2500,0,SK_LoadTrack_Phase2,$loadMap);
        $clearBricks4 = schedule(%GD,0,"announce","\c6Cleared bricks, loading: \c3"@ $loadMap);
        return centerprintAll("\c6100% Bricks: 0/"@ %initBC,10);
    }
    centerprintAll("\c6"@ 100 - ((%bc=getBrickCount()) / %initBC)*100 @"% Bricks: "@ %bc @"/"@ %initBC,10);
    %bg = mainBrickGroup.getObject(%bgI);
    for(%I=0;%I<25;%I++)
    {
        if(%bg.getCount() <= 0)
        {
            %bgI++;
            break;
        }
        %o = %bg.getObject(0);
        if(%o.getDatablock().brickSizeX > 16)
        {
            %o.respawn();
            %o.disappear(-1);
            %o.delete();
            return $clearBricks2 = schedule(15, 0, clearBricks2, %bgI,%initBC);
        }
        %o.delete();
    }
    $clearBricks2 = schedule(1, 0, clearBricks2, %bgI,%initBC);
}



function findNewMaps() {
    // We tend to use the same format because people are so used to SpeedKart by now.
    $BumperKart["maxMaps"] = 0;
    %id = 0;
    %path = "Add-Ons/BumperKart_*/save.bls";
    %file = findFirstFile(%path);
    while(%file !$= "") {
        $BumperKart["maxMaps"]++;
        $BumperKart["map" @ %id] = %file;
        %id++;
        %file = findNextFile(%path);
    }
} findNewMaps();

function getMapList(%id) {
    if(%id !$= "")
        return $bumperKart["map" @ %id];
    for(%i=0;%i < $BumperKart["maxMaps"];%i++)
        %maps = %maps SPC "\n\c3" @ %i @ "\c6:" SPC $BumperKart["map" @ %i];
    return %maps;
}
function serverCmdMapList(%client) {
    for(%i=0;%i < $BumperKart["maxMaps"];%i++) {
        %map = $BumperKart["map" @ %i];
        %map = strReplace(%map, "Add-Ons/", "");
        %map = strReplace(%map, "BumperKart_", "");
        %map = strReplace(%map, "/save.bls", "");
        %map = strReplace(%map, "_", " ");
        messageClient(%client, '', "\c3" @ %i @ "\c6:" SPC %map);
    }
    messageClient(%client, '', "\c6Current Map: \c3" @ $BumperKart["map" @ $defaultMinigame["currentMap"]]);
}

package BumperKart_Vehicles
{
    function fxDTSBrick::setVehicle(%obj, %data, %client)
    { // Created by Buddy
        if(%obj.vehicleDataBlock == %data)
            return;
        if(isObject(%obj.Vehicle))
            %obj.Vehicle.delete();
        %obj.Vehicle = "0";
        if(isObject(%obj.VehicleSpawnMarker))
            %obj.VehicleSpawnMarker.delete();
        %obj.VehicleSpawnMarker = "0";
        if((!(%obj.getDataBlock().specialBrickType $= "VehicleSpawn")))
            return;
        if((!isObject(%data)))
        {
            %obj.vehicleDataBlock = "0";
            return;
        }
        if((!(%data.getClassName() $= "PlayerData")))
        if((!(%data.getClassName() $= "WheeledVehicleData")))
        if((!(%data.getClassName() $= "FlyingVehicleData")))
        if((!(%data.getClassName() $= "HoverVehicleData")))
            return;

        if((!%data.rideAble))
            return;
        if((%data.uiName $= ""))
            return;
        %brickGroup = %obj.getGroup();
        %obj.vehicleDataBlock = %data;
        %obj.spawnVehicle();
        if((%obj.reColorVehicle $= ""))
            %obj.reColorVehicle = "1";
        %obj.VehicleSpawnMarker = new VehicleSpawnMarker()
        {
            dataBlock = "VehicleSpawnMarkerData";
            uiName = %data.uiName;
            reColorVehicle = %obj.reColorVehicle;
            vehicleDataBlock = %data;
            brick = %obj;
        };
        "MissionCleanup".add(%obj.VehicleSpawnMarker);
        %obj.VehicleSpawnMarker.setTransform(%obj.getTransform());
    }
    function fxDTSBrick::spawnVehicle(%obj, %delay)
    { // Created by Buddy
        if(%delay > 0)
        {
            if(isEventPending(%obj.spawnVehicleSchedule))
                cancel(%obj.spawnVehicleSchedule);
            %obj.spawnVehicleSchedule = %obj.schedule(%delay, "spawnVehicle", "0");
            return;
        }
        if(isEventPending(%obj.spawnVehicleSchedule))
            cancel(%obj.spawnVehicleSchedule);
        if((!(%obj.getDataBlock().specialBrickType $= "VehicleSpawn")))
            return;
        if((!isObject(%obj.vehicleDataBlock)))
            return;
        if(isObject(%obj.Vehicle))
            if(%obj.Vehicle.getDamagePercent() < 1)
                %obj.Vehicle.delete();
        %trans = %obj.getTransform();
        %x = getWord(%trans, "0");
        %y = getWord(%trans, "1");
        %z = getWord(%trans, "2");
        %z = (%z + ((getWord(%obj.getWorldBox(), "5") - getWord(%obj.getWorldBox(), "2")) / 2));
        %rot = getWords(%trans, "3", "6");
        if((%obj.vehicleDataBlock.getClassName() $= "PlayerData"))
        {
            %v = new AIPlayer()
            {
                dataBlock = %obj.vehicleDataBlock;
            };
        }
        if((%obj.vehicleDataBlock.getClassName() $= "WheeledVehicleData"))
        {
            %v = new WheeledVehicle()
            {
                dataBlock = %obj.vehicleDataBlock;
            };
        }
        if((%obj.vehicleDataBlock.getClassName() $= "FlyingVehicleData"))
        {
            %z = (%z + %obj.vehicleDataBlock.createHoverHeight);
            %v = new FlyingVehicle()
            {
                dataBlock = %obj.vehicleDataBlock;
            };
        }
        if((%obj.vehicleDataBlock.getClassName() $= "HoverVehicleData"))
        {
            %z = (%z + %obj.vehicleDataBlock.createHoverHeight);
            %v = new HoverVehicle()
            {
                dataBlock = %obj.vehicleDataBlock;
            };
        }
        if((!%v))
            return;
        MissionCleanup.add(%v);
        %v.spawnBrick = %obj;
        %v.brickGroup = %obj.getGroup();
        %obj.Vehicle = %v;
        %obj.colorVehicle();
        if((!(%v.getType() & $TypeMasks::PlayerObjectType)))
        {
            %worldBoxZ = mAbs(getWord(%v.getWorldBox(), "2") - getWord(%v.getWorldBox(),2));
            %worldBoxZ = mAbs(getWord(%v.getWorldBox(), "2") - getWord(%v.getPosition(),2));
            %z = (%z + (%worldBoxZ + 0.1));
        }
        %trans = %x SPC %y SPC %z SPC %rot;
        %v.setTransform(%trans);
        if((%v.getType() & $TypeMasks::PlayerObjectType))
            %pos = %v.getHackPosition();
        %pos = %v.getWorldBoxCenter();

        if($BumperKart["SpecialRound"] == 1) // BUMPERKART SPECIAL ROUND
            %v.setScale(getRandom() SPC getRandom() SPC getRandom());
        if(getWord(%v.getScale(), 2) < 0.2)
            %v.setScale(getRandom() SPC getRandom() SPC 1);

        %p = new Projectile()
        {
            dataBlock = spawnProjectile;
            initialVelocity = "0 0 0";
            initialPosition = %pos;
            sourceObject = %v;
            sourceSlot = "0";
            client = %this;
        };
        if((!%p))
            return;
        MissionCleanup.add(%p);
    }
    function VehicleData::onEnterLiquid(%data, %obj, %coverage, %type)
    { // Created by Buddy
        Parent::onEnterLiquid(%data, %obj, %coverage, %type);
        if(!isObject(%obj))
            return;
        %obj.spawnBrick.setVehicle(0);
        //%obj.finalExplosion();
    }
    //players should die in water
    function Armor::onEnterLiquid(%data, %obj, %coverage, %type)
    { // Created by Buddy
        Parent::onEnterLiquid(%data, %obj, %coverage, %type);
        if(%obj.isBot) { // Anthonyrules144's stuff
            if(isObject(%obj)) {
                %obj.delete();
                BK_AddBot();
            }
        } // End of Anthonyrules144's stuff
        if(!isObject(%obj.client.minigame) || !isObject(%obj))
            return;
        %obj.hasShotOnce = true;
        %obj.invulnerable = false;
        %obj.damage(%obj, %obj.getPosition(), 10000, $DamageType::Lava);
    }
    function Vehicle::onDriverLeave(%obj, %player)
    { // Created by Buddy
        if(isObject(%player.client.minigame) && isObject(%obj))
        {
            %player.count = 10;
            %player.countDown(10, %obj);
        }
        parent::onDriverLeave(%obj,%player);
    }
}; activatepackage(BumperKart_Vehicles);


function Player::Countdown(%player, %count, %vehicle) {
    %client = %client.player;
    if(%count > 15)
        %count = 15;
    if(%player.count $= "")
        %player.count = %count;
    if(isEventPending(%player.countDown["sched"]))
        cancel(%player.countDown["sched"]);
    if(!isObject(%player) || !isObject(%player.client))
        return;
    if(%player.count < 1) {
        if(isObject(%vehicle) && %vehicle.getObjectMount() < 1) {
            %obj.schedule(100,damage,%obj,%obj.position,100000,$DamageType::Lava);
            %vehicle.spawnBrick.setVehicle(0);
        }
        return %player.kill();
    }
    %player.client.bottomPrint("<font:impact:25>\c6You have\c0 " @ %player.count @ " \c6seconds to get in a vehicle", 1.2);
    %player.count -=1;
    %player.countDown["sched"] = %player.schedule(1000, Countdown, %count, %vehicle);
}
package BumperKart_Main {
    function GameConnection::AutoAdminCheck(%client) {
        parent::AutoAdminCheck(%client);
    }
    function GameConnection::onClientLeaveGame(%client) {
        //if(%client.hasSpawned)
        //    BK_AddBot();
        parent::onClientLeaveGame(%client);
    }
    function serverCmdSuicide(%client) {
        if(isObject(%client.player))
            if(isObject(%client.player.getObjectMount()))
                %client.player.getObjectMount().spawnBrick.setVehicle(0);
        parent::serverCmdSuicide(%client);
    }
    function Armor::onMount(%this, %obj, %mount, %node) {
        parent::onMount(%this, %obj, %mount, %node);
        if(isObject(%obj.client)) {
            %player = %obj.client.player;
            if(isEventPending(%player.countDown["sched"]))
                cancel(%player.countDown["sched"]);
            if(%obj.client.hasCustomTires !$= "" || %obj.client.hasCustomTires != -1) {
                if(isObject(%obj.client.hasCustomTires))
                    for(%i=0;%i < 4;%i++)
                        %mount.setWheelTire(%i, %obj.client.hasCustomTires);
            }
            if(%obj.client.hasCustomSpring !$= "" || %obj.client.hasCustomSpring != 0) {
                if(isObject(%obj.client.hasCustomSpring))
                    for(%i=0;%i < 4;%i++)
                        %mount.setWheelSpring(%i, %obj.client.hasCustomSpring);
            }
            %obj.client.sendUI();
        }
    }
    function GameConnection::SpawnPlayer(%client, %a, %b) {
        parent::SpawnPlayer(%client, %a, %b);
        if($mapLoading == true && %client.minigame > 0 && %client.hasSpanwed == true) {
            return messageClient(%client, '', "\c6Map is currently loading");
        }
        //if(!%client.hasSpawned)
            //BK_RemoveBot(1);
        %client.hasSpawned = true;
        if($mapLoading == true && %client.minigame > 0)
            return %client.player.delete();
        %spawn = findVehicleSpawn(%client.player.position);
        if(isObject(%spawn.vehicle) && %spawn.vehicle.getMountedObject(0)) {
            %client.schedule(50, instantRespawn);
            return;
        }
        if(!isEventPending(%client.player.vehicleCountDown) && %client.minigame > 0) {
            if($BumperKart["SpecialRound"] == 3) {
                %client.player.tool0 = RocketLauncherItem.getID();
                %client.player.weaponCount++;
                messageClient(%client, 'MsgItemPickup', '', 0, %client.player.tool[0]);
            }
            //%client.centerPrint(%client, '', "\c6You have 15 seconds to get in a vehicle");
            %client.player.count = 15;
            %client.player.countdown(15);
        }
    }
    function WheeledVehicleData::onCollision(%this, %vehicle, %victim, %d, %e, %f) {
        parent::onCollision(%this, %vehicle, %victim, %d, %e, %f);
        if(isObject(%victim) && %victim.getClassName() $= "WheeledVehicle") {
            if((getSimTime() - %vehicle.lastHit) < 2000 || (getSimTime() - %victim.lastHit) < 2000)
                return;
            %vLen = vectorLen(%vehicle.getVelocity());
            %viLen = vectorLen(%victim.getVelocity());
            if(%vLen > %viLen) {
                %vehicle.schedule(100, setVelocity, "0 0 0");
                %victim.lastHit = getSimTime();
                %vehicle.lastHit = getSimTime();
                %victim.applyImpulse("0 0 0", getRandom(-30, 30) SPC getRandom(-30, 30) SPC getRandom(-30, 30));
            } else if(%viLen > %vLen) {
                %victim.schedule(100, setVelocity, "0 0 0");
                %vehicle.lastHit = getSimTime();
                %victim.lastHit = getSimTime();
                %vehicle.applyImpulse("0 0 0", getRandom(-30, 30) SPC getRandom(-30, 30) SPC getRandom(-30, 30));
            }
        }
    }
    function fxDTSBrick::onActivate(%brick, %player, %client, %pos, %vec) {
        parent::onActivate(%brick, %player, %client, %pos, %vec);
        if(%brick.getName() $= "_color") {
            %spawn = findVehicleSpawn(%player.getPosition());
            if(%spawn != 0) {
                %spawn.setColor(%brick.getColorID());
                %brick.setColorFX(1);
                %brick.schedule(150, setColorFX, 0);
            }
        }
        else if(getSubStr(%brick.getName(), 0, 8) $= "_vehicle") {
            %brick.setColorFX(1);
            %brick.schedule(150, setColorFX, 0);
            if(%player.client.isUsingSpecial != -1 && %player.client.isUsingSpecial !$= "")
                %vehicle = $Special[%player.client.isUsingSpecial];
            else {
                %vehicle = getSubStr(%brick.getName(), 8, strLen(%brick.getName()));
                if(%vehicle $= "SpeedKartBuggy")
                    %vehicle = "SpeedKartBuggyVehicle";
            }
            %name = %vehicle;
            if(strPos(%vehicle, "SpeedKart") >= 0)
                %name = strReplace(%name, "SpeedKart", "");
            if(strPos(%vehicle, "Vehicle") >= 0)
                %name = strReplace(%name, "Vehicle", "");
            if(%vehicle !$= "")
                messageClient(%player.client, '', "\c6Attempted to spawn a\c3" SPC (%name) SPC " \c6Success:\c2" SPC (setVehicleToSpawn(%player, %vehicle) <= 0 ? "False" : "True"));
        }
    }
    function GameConnection::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc) {
        %hitter = %client.hitter;
        if(isObject(%hitter)) {
            %hitter.points += 1;
            messageClient(%hitter, '', "\c6You now have\c2 " @ %hitter.points @ " \c6total points");
            %hitter.setScore(%hitter.score += 1);
        }
        if(%max !$= "")
            for(%i=0;%i < clientGroup.getCount();%i++) {
                %cl = clientGroup.getObject(%i);
                if(%client.minigame != 0) {
                    %r = %m[getRandom(0, %max)];
                    messageClient(%cl, '', %icon SPC %r);
                    %cl.bottomprint("<font:arial:15>\c6Last Death: " @ %icon SPC "<font:arial:25>" @ %r @ "\n<just:center><font:arial:15>\c6Score of\c2" SPC %hitter.score, 0, 1);
                } else continue;
            }
        parent::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc);
    }
    function miniGameSO::reset(%minigame, %a, %b, %c) {
        parent::reset(%minigame, %a, %b, %c);
        //for(%i=0;%i < $BK_Bot["max"];%i++) {
        //    if(isObject(%bot = $BK_Bot[%i]))
        //        %bot.delete();
        //}
        BK_PrepareBotSpawn(mFloor(($Pref::Server::MaxPlayers - $Server::PlayerCount) / 2));
        $BumperKart["SpecialRound"] = false;
        if(getTimescale() > 1)
            setTimescale(1);
        SpecialRound();
        if($BumperKart["SpecialRound"] != false) {
            if($BumperKart["SpecialRound"] == 1)
                announce("<font:cambria:25>\c2SPECIAL \c5ROUND\c6: Vehicles now resize randomly!");
            //if($BumperKart["SpecialRound"] == 2) {
            //    %timescale = 2;
            //    talk("\c2SPECIAL \c5ROUND\c6: Timescale " @ %timescale @ "!");
            //    setTimescale(%timescale);
            //}
            if($BumperKart["SpecialRound"] == 3)
                announce("<font:cambria:25>\c2SPECIAL \c5ROUND\c6: Everyone gets Rocket Launchers on spawn!");
        }
        if(!$BK_DebugMode)
            $defaultMinigame["round"]++;
        else
            $defaultMinigame["round"] = "0 (DEBUG MODE IS ACTIVE)";
        if($defaultMinigame["round"] > $defaultMinigame["maxRounds"]) {
            announce("<font:cambria:25>\c5BumperKart\c6: Reached the max round... changing maps!");
            $defaultMinigame["round"] = 0;
            return changeMap();
        }
        %minigame.respawnAll();
        announce("<font:cambria:25>\c5BumperKart\c6: Round \c3" @ $defaultMinigame["round"] SPC "\c6/\c3" SPC $defaultMinigame["maxRounds"]);
        %id = nametoID(musicData_Wait_afterDawn);
        if(isObject(%ID))
            SM_PlaySong(%id, "", 0, 1, 1);
        countDown(25);
        schedule(250, 0, clearVehicles);
     }
    function fxDTSBrick::respawnVehicle(%brick, %vehicle, %a, %b) {
        %brick.setVehicle(0);
        parent::respawnVehicle(%brick, %vehicle, %a, %b);
    }
    function serverCmdActivateStuff(%client) {
        parent::serverCmdActivateStuff(%client);
        if(!isObject(%player = %client.player) || !isObject(%mount = %player.getControlObject()) || isEventPending($countDown["active"]))
            return;
        initContainerBoxSearch(%mount.getWorldBoxCenter(), "0 0 5", $TypeMasks::FxBrickAlwaysObjectType);
        if(isObject(containerSearchNext())) {
            return %mount.applyImpulse(%mount.getWorldBoxCenter(), vectorScale("0 0 10", 300));
        }
    }
    function Armor::onTrigger(%armor, %player, %trigger, %active) {
        parent::onTrigger(%armor, %player, %trigger, %active);
        if(%trigger == 0 && %active == 1) {
            %player.isDrifting = true;
            %player.drifting = %player.schedule(150, drift);
        }
        if(%trigger == 0 && %active == 0) {
            %player.stopDrifting();
            %player.isDrifting = false;
        }
    }
}; activatePackage(BumperKart_Main);

// time
function BK_TimeStampNext() {
    $BK_Time--;
    if($BK_Time <= 0) {
        if(isEventPending($BK_TimeStamp))
            cancel($BK_TimeStamp);
        $defaultMinigame.reset();
        return;
    }
    announce("<font:cambria:25>\c5" @ $BK_Time @ " \c6minutes remaining!");
    $BK_TimeStamp = schedule(60000, 0, BK_TimeStampNext);
}

// drifting
function DefineDriftTires()
{ // Created by Buddy
    if($BumperKart["hasDefinedDriftTires"])
        warn("[FUNC, DefineDriftTires] Function was already used, using this again \"might\" result in un-used datablocks");
    %gs = getDataBlockGroupSize();
    for(%I = 0; %I < %gs; %I++)
    {
        if((%db = getDatablock(%I)).getClassName() $= "WheeledVehicleTire" && !isObject(%db.drifting))
        {
            eval("datablock WheeledVehicleTire(" @ (%db.getName() @ "Drifting") @" : "@ %db.getName() @") { lateralForce = 2000; lateralDamping = 100; lateralRelaxation = 0.5; longitudinalForce = 14000; longitudinalDamping = 2000; longitudinalRelaxation = 0.01; };");
            %dat = nametoID(%db.getName() @ "Drifting");
            //talk("make "@ %dat);
            %db.drifting = %dat;
            %dat.drifting = %db;
        }
    }
}

function Player::Drift(%player)
{ // Created by Buddy
    if(isObject(%mount = %player.getObjectMount()))
    {
        %count = %mount.getWheelCount();
        %tire = (isObject(%player.client.hasCustomTires) ? %player.client.hasCustomTires : %mount.getDatablock().defaultTire);
        if(isObject(%tire.drifting))
            for(%I=0;%i<%count;%I++)
                %mount.setWheelTire(%I,%tire.drifting);
    }
}
function Player::StopDrifting(%player)
{ // Created by Buddy
    if(isObject(%mount = %player.getObjectMount()))
    {
        %count = %mount.getWheelCount();
        %tire = (isObject(%player.client.hasCustomTires) ? %player.client.hasCustomTires : %mount.getDatablock().defaultTire);
        for(%I=0;%i<%count;%I++)
            %mount.setWheelTire(%I,%tire);
    }
}

// --- SPECIAL  ROUNDS --- //
function SpecialRound() {
    if(getRandom(3) == getRandom(3)) {
        %rad = getRandom(1, 3);
        return $BumperKart["SpecialRound"] = %rad;
    }
    else
        $BumperKart["SpecialRound"] = false;
}


// --- OTHER VEHICLE STUFF --- //

function getBumperKartVersion() {
    return $BumperKart["version"];
}

// --- ALL VEHICLE STUFF --- \\
function findVehicleSpawn(%pos) {
    initContainerBoxSearch(%pos, "5 5 5", $TypeMasks::FxBrickAlwaysObjectType);
    while(isObject(%find = containerSearchNext()))
        if(%find.getDatablock().uiName $= "Vehicle Spawn")
            return %find;
}

function setVehicleToSpawn(%player, %vehicle) {
    %spawn = findVehicleSpawn(%player.getPosition());
    if(%spawn > 0 && %vehicle !$= "") {
        if(isObject(%spawn.vehicle) && %spawn.vehicle.getMountedObject(0) != 0) {
            %user = %spawn.vehicle.getMountedObject(0).client.name;
            return messageClient(%player.client, '', "\c3" @ %user SPC "\c6is using this garage.");
        }
        %spawn.spawnVehicle(%vehicle);
        %vehicle.spawnTime = getSimTime();
        %spawn.setVehicle(%vehicle.getID());
        if(isEventPending($countDown["active"]))
            %spawn.setVehiclePowered(0);
        else
            %spawn.setVehiclePowered(1);
    }
}

function setVehiclePowered(%on) {
    %vehicleCount = 0;
    %count = MissionCleanup.getCount();
    for(%i = %count-1;%i > 0;%i--) {
        %obj = MissionCleanup.getObject(%i);
        if(%obj.getType() & $TypeMasks::VehicleObjectType && isObject(%obj.spawnBrick))
            %obj.spawnBrick.setVehiclePowered(%on);
    }
}

function getVehicleCount() {
    %vehicleCount = 0;
    %count = MissionCleanup.getCount();
    for(%i = %count-1;%i > 0;%i--) {
        %obj = MissionCleanup.getObject(%i);
        if(%obj.getType() & $TypeMasks::VehicleObjectType && isObject(%obj.spawnBrick)) {
            %vehicleCount++;
        }
    }
    return %vehicleCount;
}

function clearVehicles() {
    %vehicleCount = 0;
    %count = MissionCleanup.getCount();
    for(%i = %count-1;%i > 0;%i--) {
        %obj = MissionCleanup.getObject(%i);
        if(%obj.getType() & $TypeMasks::VehicleObjectType && isObject(%obj.spawnBrick)) {
            if(isObject(%obj.spawnBrick.vehicle))
                if(%obj.spawnBrick.Vehicle.getMountedObject(0) != 0)
                    continue;
            if((getSimTime() - %obj.spawnTime) < 5000)
                continue;
            %obj.spawnBrick.Vehicle = "";
            %obj.schedule("10", "delete");
            %vehicleCount++;
        }
    }
    $Server::numPhysVehicles = 0;
    announce("\c6Clearing\c3" SPC (%vehicleCount <= 0 ? "no" : %vehicleCount) SPC (%vehicleCount < 2 && %vehicleCount > 0 ? "\c6vehicle" : "\c6vehicles"));
}

function clearVehiclesSchedule() {
    if(isEventPending($BumperKart["clearVehicles"]))
        cancel($bumperKart["clearVehicles"]);
    clearVehicles();
    $bumperKart["clearVehicles"] = schedule(60000, 0, clearVehiclesSchedule);
}
if(!isEventPending($BumperKart["clearVehicles"]))
    clearVehiclesSchedule();

// --- COUNT-DOWN --- \\\
function CountDown(%number) {
    if(isEventPending($countDown["active"]))
        cancel($countDown["active"]);
    %font = "<font:verdana:55>";
    %time = 1000;
    $countDown["number"] = %number;
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        if(!isObject(%client.player)) {
            %client.centerPrint("");
            continue;
        }
        if(%number < 1) {
            %client.centerPrint(%font @ "\c6" @ "GO");
            %client.schedule(%time, centerPrint, "");
            continue;
        }
        %col = "<color:" @ getSubStr(getRandom(%number) @ getRandom(%number) @ %number @ %number @ getRandom(%number) @ %number, 0, 6) @ ">";
        %client.centerPrint(%font @ "\c6" @ %col @ %number);
    }
    for(%i=0;%i < 4;%i++) {
        %clock = "_clock" @ %i;
        if(!isObject(%clock))
            continue;
        if(%number > 3)
            %clock.setColor(0);
        else if(%number > 1 && %number < 3)
            %clock.setColor(2);
        else if(%number > 0 && %number < 2)
            %clock.setColor(4);
        %clock.setPrintCount(%number);
    }
    if(%number < 1) {
        SM_PlaySong("", "", 0, 1, 1);
        $BK_Time = 3; // 3 minutes
        $BK_TimeStamp = schedule(60000, 0, BK_TimeStampNext);
        announce("<font:cambria:25>\c5" @ $BK_Time @ " \c6minutes remaining!");
        setVehiclePowered(1);
        return;
    }
    else
        setVehiclePowered(0);
    $countDown["active"] = schedule(%time, 0, CountDown, %number -= 1);
}

// DEATH MESSAGES -- Anthonyrules144
deleteVariables("$DeathMessage_*");

function DeathMSG(%id, %client, %victim) {
	if(%client == %victim) %id = 2;
	%clN = strReplace(%client.name, %client.name, "\c3" @ %client.name @ "\c0");
	%viN = strReplace(%victim.name, %victim.name, "\c2" @ %victim.name @ "\c0");
	if(%id $= "hitter") { // BumperKart Minigame
		%msg[0] = "\c2" @ %viN SPC "\c0made\c3" SPC %clN SPC "\c0fly";
		%msg[1] = "\c2" @ %viN SPC "\c0flung\c3" SPC %clN;
		%msg[2] = "\c2" @ %viN SPC "\c0killed\c3" SPC %clN;
		%msg[3] = "\c2" @ %viN SPC "\c0hit\c3" SPC %clN SPC "\c0too hard";
		%msg[4] = "\c2" @ %viN SPC "\c0gently tapped\c3" SPC %clN;
		%msg[5] = "\c3" @ %clN SPC "\c0went zooming by\c2" SPC %viN;
		%msg[6] = "\c2" @ %viN SPC "\c0sent\c3" SPC %clN SPC "\c0into space";
        %msg[7] = "\c2" @ %viN SPC "\c0somehow killed\c3" SPC %clN;
        %msg[8] = "\c3" @ %clN SPC "\c0tried to play Bumper-Cars with\c2" SPC %viN;
		%victim.setScore(%victim.score++);
		%ci = "carExplosion";
	} else if(%id == 2) { // Suicide
		%msg[0] = %clN SPC "died";
		%msg[1] = %clN SPC "drank too much";
		%msg[2] = %clN SPC "tried too hard";
		%msg[3] = %clN SPC "suicided";
		%msg[4] = %clN SPC "probably pressed Ctrl-K";
		%msg[5] = %clN SPC "failed the matrix";
        %msg[6] = %clN SPC "didn\'t try hard enough";
        %msg[7] = %clN SPC "sent themselves to to shadow-realm";
        %msg[8] = %clN SPC "got themselves dunked on";
        %msg[9] = %clN SPC "tried to do the \"Roast-Me\" challenge";
        %msg[10] = %clN SPC "had to leave, their people needed them";
		%ci = "skull";
	} else if(%id == 6) { // Fall Damage
		%msg[0] = %clN SPC "was thrown back down";
		%msg[1] = %clN SPC "fell too far";
		%msg[2] = %clN SPC "hit the ground too hard";
		%msg[3] = %clN SPC "hit the ground running";
		%ci = "crater";
	} else if(%id == 7) { // Getting ran over
		%msg[0] = %clN SPC "got ran over by" SPC %viN;
		%msg[1] = %viN SPC "made" SPC %clN SPC "into dough";
		%ci = "car";
	} else if(%id == 8) { // Vehicle Explosion
		%msg[0] = %clN @ "\'s car exploded";
		%ci = "carExplosion";
	} else if(%id == 9) { // Drowning
		%msg[0] = %clN SPC "drowned";
		%msg[1] = %clN SPC "tried to swim";
		%msg[2] = %clN SPC "is swimming with the fishes";
        %msg[3] = %clN SPC "went swimming";
        %msg[4] = %clN SPC "made it into the show: \"Next best fish\"";
        %msg[5] = %cLN SPC "beep beep I'ma fish";
	} else if(%id == 10) { // Hammer
		%msg[0] = %clN SPC "was hammered by" SPC %viN;
		%ci = "hammer";
	} else if(%id == 14) {
		%msg[0] = %viN SPC "slayed" SPC %clN;
	} else if(%id == 15) { // Spear
		%msg[0] = %viN SPC "speared" SPC %clN;
		%ci = "generic";
	} else if(%id == 18) { // Rocket L
		%msg[0] = %viN SPC "exploded" SPC %clN;
		%ci = "generic";
	} else if(%id == 19 || %id == 20) { // Gun    // Gun Ambiko
		%msg[0] = %viN SPC "shot" SPC %clN;
		%ci = "generic";
	} else if(%id == 21) { // Bow
		%msg[0] = %clN SPC "was hit to the knee by" SPC %viN;
		%ci = "generic";
	} else return;
	for(%i=0;%i < 15;%i++)
		if(%msg[%i] $= "") {
			%max = %i - 1;
			break;
		}
	%r = getRandom(0, %max);
	for(%i=0;%i < clientGroup.getCount();%i++) {
		if(ClientGroup.getObject(%i).deathMessages < 1)
		messageClient(ClientGroup.getObject(%i), '', "<bitmap:base/client/ui/ci/" @ %ci @ ">" SPC %msg[%r]);
	}
}

package deathMessages {
	function GameConnection::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc) {
		parent::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc);
        if(isObject(%client.hitter)) { // BumperKart minigame
			DeathMSG("hitter", %client, %client.hitter);
			return %client.hitter = "";
		} else if(%sourceClient.getClassName() $= "AiPlayer") {
			%sourceClient.name = "Bot";
			return DeathMSG(%damageType, %client, %sourceClient);
		} else {
			if(isObject(%sourceClient.getMountedObject(0)))
				return DeathMSG(%damageType, %client, %sourceClient.getMountedObject(0).client);
			DeathMSG(%damageType, %client, %sourceClient.client);
		}
	}
};
activatePackage(deathMessages);

if(!$Bumperkart["started"]) {
    if(!$defaultMinigame) {
        warn("[BumperKart, $defaultMinigame] NO VALID MINIGAME TO START ON, RETURNING...");
        return;
    }
    changeMap();
    $BumperKart["started"] = true;
}
