function servercmdVoteGravity(%client, %vote) {
    if($gravityVote_voting) {
        if(strPos($gravityVote_Users, %client) != -1) {
            %client.cM("\c6You already voted");
            return;
        }
        $gravityVote_Users = $gravityVote_Users SPC %client;
        announce("<font:impact:18>\c3" @ %client.getPlayerName() SPC "\c6accepted to the gravity vote of " @ $gravityVote SPC "\c7["@getWordCount($gravityVote_Users) SPC "/" SPC $gravityVote_Required@"]");
    } else {
        if(%vote < 0.4 || %vote > 1) {
            %client.cM("\c5/voteGravity \c7[0.4 - 1]");
            return;
        }
        %pCT = ((%pC=$Server::playerCount) > 1 ? %pC : %pC+1);
        $gravityVote_Required = mFloor(%pCT / 2)+1;
        announce("<font:impact:18>\c3" @ %client.getPlayerName() SPC "\c6voted for the gravity to be \c5" @ %vote @ "\c6, use \c5/voteGravity \c6to accept it!");
        announce("<font:impact:18>\c6Required \c3" @ $gravityVote_Required SPC "\c6users to vote! \c7[30 seconds left]");
        $gravityVote = %vote;
        $gravityVote_Users = %client;
        $gravityVote_voting = true;
        $gravityVote_Pending = schedule(30000, 0, gravityVote_End);
    }
}

function gravityVote_End() {
    if($gravityVote_Required >= getWordCount($gravityVote_Users)) {
        $Gravity_GlobalZone.gravityMod = $gravityVote;
        $gravity = $gravityVote;
        announce("<font:impact:18>\c6The gravity vote was successful, gravity is set to " @ $gravityVote);
    } else {
        announce("<font:impact:18>\c6The gravity vote was unsucessful, gravity was not changed");
    }
    $gravityVote = "";
    $gravityVote_Users = "";
    $gravityVote_voting = false;
    $gravityVote_Pending = "";
    $gravityVote_Required = "";
}
