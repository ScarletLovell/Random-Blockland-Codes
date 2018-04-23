function MinigameSchedule() {
    //%count = js_eval("checkStringOccurences(\""@$Minigame["FP", "Players"]@"\", \"\t\");");
    for(%i=0;%i < %count;%i++) {
        %c = getWord($Minigame["FP", "Players"]);
        %client = getField(%client, 0);
        if(!isObject(%client)) {
            $Minigame["FP", "Players"] = strReplace($Minigame["FP", "Players"], %c SPC "", "");
        } else if(%client.player !$= getWord(%c, 1)) {
            $Minigame["FP", "Players"] = strReplace($Minigame["FP", "Players"], %c SPC "", "");
        }
    }
}
MinigameSchedule();

package Minigames {
    function GameConnection::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc) {
        %player = %client.player;
        if(%player.minigame !$= "") {
            if(%player.minigame $= "FP") {
                //announce("<font:arial:18>\c3" @ %client.getPlayerName() SPC "\c6"@%type@" the \c5Falling Platforms \c6minigame");
                %client.respawnAt = "_FP_banBrick";
            }
        }
		parent::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc);
    }
    function GameConnection::SpawnPlayer(%client) {
        parent::SpawnPlayer(%client);
        if(%client.respawnAt !$= "") {
            %client.player.setTransform(%client.respawnAt.getPosition());
            %client.respawnAt = "";
        }
    }
};
activatePackage(Minigames);
