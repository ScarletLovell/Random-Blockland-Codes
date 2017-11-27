function cleanupEmitters() {
    for(%i=0;%i < missionCleanup.getCount();%i++) {
        %m = missionCleanup.getObject(%i);
        if(%m.getClassName() $= "ParticleEmitterNode")
            %m.delete();
    }
}
function cleanupLights() {
    for(%i=0;%i < missionCleanup.getCount();%i++) {
        %m = missionCleanup.getObject(%i);
        if(%m.getClassName() $= "fxLight")
            %m.delete();
    }
}
function cleanupAIs() {
    for(%i=0;%i < missionCleanup.getCount();%i++) {
        %m = missionCleanup.getObject(%i);
        if(%m.getClassName() $= "AIPlayer")
            %m.delete();
    }
}
function cleanupStaticShapes() {
    for(%i=0;%i < missionCleanup.getCount();%i++) {
        %m = missionCleanup.getObject(%i);
        if(%m.getClassName() $= "StaticShape")
            %m.delete();
    }
}
function cleanupZones() {
    for(%i=0;%i < missionCleanup.getCount();%i++) {
        %m = missionCleanup.getObject(%i);
        if(%m.getClassName() $= "PhysicalZone")
            %m.delete();
    }
}
