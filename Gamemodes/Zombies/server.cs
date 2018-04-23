
function turnIntoZombie(%client) {
    if(!isObject(%player = %client.player))
        return;
}

function testFall(%client) {
    if(isObject(%player = %client.player)) {
        %player.isSuperDeathing = 1;
        %player.setDamageFlash(0.75);
        %player.playDeathCry();
        %player.setArmThread("death1");
        superFall(%player, 0);
    }
}

if(isObject(SwordImage) && SwordImage.shapeFile !$= "base/data/shapes/empty.dts") {
    SwordImage.shapeFile = "base/data/shapes/empty.dts";
}

$Zombies::HideableNodes = "helmet copHat knitHat scoutHat bicorn pointyHelmet flareHelmet plume triplume septplume visor" @
    "epaulets epauletsRankA epauletsRankB epauletsRankC epauletsRankD ShoulderPads" @
    "armor, pack, quiver, tank, bucket, cape RShoe LShoe SkirtTrimRight SkirtTrimLeft skirtHip";
$Zombies::ActiveNodes = "Pants lShoe rShoe";
$Zombies::NodeColors = "rArm:1|0|0 lArm:1|0|0 lHand:0.3|0.6|0.3 rHand:0.3|0.6|0.3 headSkin:0.3|0.6|0.3 pants:0|0|1 rShoe:0|0|1 lShoe:0|0|1 " @
    "femChest:1|1|1 chest:1|1|1";
package superDeath {
    function GameConnection::SpawnPlayer(%client) {
        parent::SpawnPlayer(%client);
        %client.zombie = false;
        %client.player.health = %client.player.getDatablock().maxDamage;
    }
    function Armor::damage(%this, %obj, %sourceObject, %position, %damage, %damageType) {
        if(isObject(%client=%obj.client) && isObject(%client.minigame)) {
            if(%obj.isSuperDeathing || %damageType == 2 || !isObject(%cHit = %sourceObject.client))
                return;
            if(%cHit.zombie && %client.zombie || strLwr(%cHit.getTeam().name) $= "zombies" && %client.zombie || strLwr(%client.getTeam().name) $= "zombies" && %cHit.zombie)
                return;
            %health=%obj.health -= %damage;
            if(%health <= 0) {
                if(strLwr(%client.getTeam().name) !$= "zombies" && !%client.zombie) {
                    for(%i=0;%i < %client.minigame.teams.getCount();%i++) {
                        if(strLwr((%team = %client.minigame.teams.getObject(%i)).name) !$= "humans") {
                            %winTeam = %team;
                            continue;
                        }
                        %teamCount = %team.numMembers;
                        for(%o=0;%o < %teamCount;%o++) {
                            %c = %team.member[%o];
                            if(%c.zombie)
                                %p++;
                        }
                        if(%p >= %teamCount)
                            %mg.win(%winTeam);
                    }
                    //for(%i=0;%i < getWordCount($Zombies::HideableNodes);%i++) {
                    //    %obj.hideNode(getWord($Zombies::HideableNodes, %i));
                    //}
                    //for(%i=0;%i < getWordCount($Zombies::ActiveNodes);%i++) {
                    //    %obj.unHideNode(getWord($Zombies::ActiveNodes, %i));
                    //}
                    //for(%i=0;%i < getWordCount($Zombies::NodeColors);%i++) {
                    //    %node = getWord($Zombies::NodeColors, %i);
                    //    %colors = strReplace(getSubStr(%node, strPos(%node, ":")+1, 200), "|", " ");
                    //    talk(%colors);
                    //    %obj.setNodeColor(getSubStr(%node, 0, strPos(%node, ":")), %colors SPC 1);
                    //}
                    %client.zombie = true;
                    announce("\c3" @ %client.getPlayerName() SPC "\c6is now a zombie!");
                    %client.chatMessage("\c51 life remaining");
                    testFall(%client);
                    return;
                }
            }
        }
        return parent::damage(%this, %obj, %sourceObject, %position, %damage, %damageType);
    }
};
activatePackage(superDeath);

function superFall(%player, %lookLimits) {
    if(%lookLimits >= 1) {
        superDeath(%player, 1);
        return;
    }
    turnIntoZombie(%player);
    %player.setLookLimits(%lookLimits, 1);
    schedule(25, 0, superFall, %player, %lookLimits+=0.05);
}
function zombieJoin(%player) {
    %player.position = %player.client.lastPos;
}
function superDeath(%player, %lookLimits) {
    if(%lookLimits <= 0) {
        %player.isSuperDeathing = 0;
        %player.setHealth(%player.health = %player.getDatablock().maxDamage);
        %player.setLookLimits(1, 0);
        %player.setArmThread("look");
        %player.client.lastPos = %player.position;
        %player.client.joinTeam("Zombies");
        schedule(250, 0, zombieJoin, %player);
        return;
    }
    %player.setLookLimits(%lookLimits, 1);
    schedule(55, 0, superDeath, %player, %lookLimits-=0.05);
}
