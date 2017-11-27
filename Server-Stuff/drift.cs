package VehicleTest { // Package created by Buddy
    function WheeledVehicleData::onAdd (%Datablock,%Object,%c,%d,%e) {
        parent::onAdd(%Datablock,%Object,%c,%d,%e);
        if(!isObject($VehicleGroup))
            $VehicleGroup = new SimSet(vehicleGroup);
        $VehicleGroup.add(%object);
    }
    function GameConnection::unMount(%client) {
        parent::unMount(%client);
    }
};
activatePackage(VehicleTest);

// Created by Anthonyrules144
function GrabDrifters () {
    if(isEventPending($GrabDrifters))
        cancel($GrabDrifters);
    for(%i=0;%i < vehicleGroup.getCount();%i++) {
        %vehicle = vehicleGroup.getObject(%i);
        GiveDrift(%vehicle);
    }
    $GrabDrifters = schedule(500, 0, GrabDrifters);
} GrabDrifters();

function GiveDrift (%vehicle) {
    %x = vectorSub(vectorNormalize(%vehicle.getForwardVector()), %vehicle.getVelocity());
    %velocity = %vehicle.getVelocity();
    %maxSpeed = %vehicle.getDatablock().maxWheelSpeed;
    if(%vehicle.lastDrift !$= "") {
        %S = vectorDist(%x, %vehicle.lastDrift);
    }
    for(%i=0;%i < 9;%i++) {
        if(isObject(%client=%vehicle.getMountedObject(%i).client)) {
            if(((%S=mFloatLength(%S, 0))> (10 + %maxSpeed / 3))) {
                %t=%TIMES = mFloatLength((%vehicle.drifting > 45 ? 45 : %vehicle.drifting+=1) / 10, 0)*2;
                %color = ("<color:" @ ((%_sT=%TIMES)@(%_sT@%_sT@%_sT@%_sT@%_sT)) @ ">");
                %client.bottomPrint("\c6Drift Score: \c3" @ %S SPC "<just:right>\c6Driver's score:\c5 " @ %client.getScore() @ " x" @ %color @ (%TIMES == 0 ? 1 : %TIMES), 1);
                %client.setScore(%client.getScore() + (%S * (%TIMES == 0 ? 1 : %TIMES)));
            }
            else {
                %vehicle.drifting = 0;
                %client.setScore(0);
                %client.bottomPrint("\c6Drift Score: \c3" @ %S SPC "<just:right> \c6Driver's score: \c5" @ %client.getScore(), 1);
            }
        }

    }
    %vehicle.lastDrift = %x;
}
