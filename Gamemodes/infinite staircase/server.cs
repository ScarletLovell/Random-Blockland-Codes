package infiniteStaircase {
    function GameConnection::SpawnPlayer(%client) {
        parent::SpawnPlayer(%client);
        if(!%client.iHasSpawned) {
            %client.iHasSpawned = true;
            %user = sqlite_query("config/sql/infiniteStaircaseSaves.db", "SELECT * FROM players WHERE blid="@%client.BL_ID);
            if(%user !$= "") {
                %client.lastSavedPosition = getField(%user, 1);
                schedule(250, 0, serverCmdRespawn, %client);
            }
        }
    }
    function GameConnection::onClientLeaveGame(%client) {
        SavePlayer(%client);
        parent::onClientLeaveGame(%client);
    }
};
activatePackage(infiniteStaircase);

function SavePlayer(%client) {
    sqlite_open("config/sql/infiniteStaircaseSaves.db");
    sqlite_query("BEGIN TRANSACTION");
    sqlite_query("CREATE TABLE IF NOT EXISTS players(blid INT NOT NULL PRIMARY KEY, position TEXT NOT NULL)");
    sqlite_query("INSERT OR REPLACE INTO players(blid, position) VALUES ("@%client.BL_ID@", \'"@%client.lastSavedPosition@"\')");
    sqlite_query("END TRANSACTION");
    sqlite_close();
}

function infiniteSchedule(%a, %b, %c) {
    if(isEventPending($infiniteSchedule))
        cancel($infiniteSchedule);
    %s = %saved = false;
    %a+=1;
    if(%a >= 10) {
        %a = 0;
        %b+=1;
        if(%b <= 2)
            %saved = true;
        if(%b >= 15) {
            %c+=1;
            %saved = true;
            %s = true;
            %b = 0;
        }
        if(%c >= 15) {
            %c = 0;
            %autoSave = true;
            announce("\c7[\c5Infinite Staircase\c7]\c6: Auto-saving everyone...");
        }
    }
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %client = clientGroup.getObject(%i);
        if(%client.timeSpent $= "") {
            %client.timeSpent = getSimTime();
        }
        if(!isObject(%player = %client.player))
            continue;
        %time = (%s=mCeil((getSimTime() - %client.timeSpent) / 1000)) < 60 ? %s@"\c6s" : (%m=mCeil(%s / 60)) < 60 ?  %m@"\c6m" : mCeil(%m / 60)@"\c6h";
        %bP = "\c6# Of Plates Up: \c5" @ mCeil(getWord(%player.position, 2)/0.2) @ "<just:right>" @ (%saved ? "\c2Saved!" : "") NL
                "<just:left>\c6Time spent in this session: \c5" @ %time@" ";
        %client.bottomPrint(%bp, 1, 1);
        if(getWord(%player.getvelocity(), 2) <= -35) {
            serverCmdRespawn(%client);
        }
        if(%s) {
            initContainerBoxSearch(vectorSub(%player.position, "0 0 1"), "1 1 1", $TypeMasks::FxBrickObjectType);
            if(isObject(%hit = containerSearchNext()))
                %client.lastSavedPosition = %player.position;
        }
        if(%autoSave)
            SavePlayer(%client);
        %client = %player = %hit = %bP = "";
    }
    $infiniteSchedule = schedule(100, 0, infiniteSchedule, %a, %b, %c);
}
infiniteSchedule();

function serverCmdRespawn(%client) {
    if(!isObject(%player = %client.player) || %client.lastSavedPosition $= "")
        return;
    %player.position = %client.lastSavedPosition;
}
