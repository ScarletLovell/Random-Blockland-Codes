// warning: you need sqlite
// you can find my version at: https://github.com/Ashleyz4/BLsql

package newChat {
    function newPlayerListGui::update(%this, %cl, %name, %BL_ID, %trust, %admin, %score) {
        if(isFunction(sqlite_query)) {
            sqlite_open("config/sql/players.db");
            sqlite_query("BEGIN TRANSACTION");
            if($newChat::newServer) {
                %ip = serverConnection.getAddress();
                %value = NPL_Window.getValue();
                %pos = strPos(%value, " Players - ")+11;
                sqlite_query("CREATE TABLE IF NOT EXISTS servers(name TEXT NOT NULL PRIMARY KEY, ip INT NOT NULL)");
                sqlite_query(formatString("INSERT OR REPLACE INTO servers(name, ip) VALUES (\'%s\', %s)", getSubStr(%value, %pos, strLen(%value)), %ip));
                $newChat::newServer = false;
            }
            $user[%name, "blid"] = %bl_id;
            sqlite_query("CREATE TABLE IF NOT EXISTS players(blid INT NOT NULL PRIMARY KEY, name TEXT NOT NULL, lastseen TEXT NOT NULL, previousnames TEXT NOT NULL)");
            %user = sqlite_query("SELECT * FROM players WHERE blid LIKE " @ %BL_ID);
            if(%user !$= "") {
                %names = getField(%user, 3);
                if(strPos(%names, %name) == -1) {
                    %names = %names SPC %name;
                    echo(%names);
                }
            }
            sqlite_query(formatString("INSERT OR REPLACE INTO players(blid, name, lastseen, previousnames) VALUES (%s, \'%s\', \'%s\', \'%s\')", $user[%name, "blid"], %name, getWord(getDateTime(), 0), %names));
            sqlite_query("END TRANSACTION");
            sqlite_close();
        }
        return parent::update(%this, %cl, %name, %BL_ID, %trust, %admin, %score);
    }
    function GameConnection::onConnectionAccepted(%this) {
        $newChat::newServer = true;
        return parent::onConnectionAccepted(%this);
    }
};
activatePackage(newChat);

function checkUser(%msg, %announce) {
    if(isFunction(sqlite_query)) {
        if(trim(strLwr(getWord(%msg, 0))) $= "^user") {
            %nB = trim(strLwr(getWord(%msg, 1)));
            if(%nB $= "name" || %nB $= "bl_id" || %nB $= "blid") {
                if(%nB $= "bl_id")
                    %nB = "blid";
                %userName = strReplace((%old=getWords(%msg, 2, 15)), "*", "%");
                %results = 0;
                if((trim(%userName) !$= "") && (strLen(trim(%userName)) >= 2)) {
                    %user = sqlite_query("config/sql/players.db", "SELECT * FROM players WHERE "@(%nB $= "blid" ? "blid" : "name")@" LIKE \'%"@%username@"%\' LIMIT 3");
                    for(%i=0;%i < getLineCount(%user);%i++) {
                        %result = getLine(%user, %i);
                        if(trim(%result) $= "")
                            continue;
                        %results += 1;
                        if($Users_lastResult $= %result && $Users_lastResult !$= "")
                            continue;
                        $Users_lastResult = %result;
                        %spc = "        ";
                        %m =
                            "["@%i@"]: " @ getField(%result, 1)@%spc@
                            "BL_ID: " @ getField(%result, 0)@%spc@
                            "Last Seen: " @ getField(%result, 2)@%spc@
                            (getField(%result, 3) !$= "" ? "Previous names: "@getField(%result, 3) : "");
                        if(%announce) commandToServer('messageSent', %m);
                        else echo(%m);
                    }
                    if(!%results && $Users_lastResult != -2) {
                        $Users_lastResult = -2;
                        if(%announce) commandToServer('messageSent', "Noone with the " @ %nB SPC %old @ " was found");
                        else echo("Noone with the " @ %nB SPC %old @ " was found");
                    }
                }
            } else {
                if($Users_lastResult != -1) {
                    $Users_lastResult = -1;
                    if(%announce) commandToServer('messageSent', "Try using BL_ID or NAME as the search");
                    else echo("Try using BL_ID or NAME as the search");
                }
            }
        }
    }
}

function clientCmdChatMessage(%cl, %voice, %pitch, %line, %pre, %name, %suf, %msg) {
    if($mute[%name] == true)
        return;
    checkUser(%msg, 1);
    if((%_sP0=strPos(%msg, "<a:")) != -1 && (%_sP1=strPos(%msg, ">", %_sP0)) != -1) {
        %_a=getSubStr(%msg, %_sP0+3, (%_sP1 - %_sP0 - 3));
        if((%_sP2=strPos(%_a, "/")) != -1) {
            %_b=getSubStr(%_a, %_sP2, strLen(%_a));
            %_a=getSubStr(%_a, 0, %_sP2);
        }
        if(%_b $= "")
            %_b = "/";
        if(!isObject(ServerReader))
             new HTTPObject(ServerReader);
        ServerReader.get(%_a @ ":80", %_b);
    }
    %f1 = "<font:georgia:"@16+0@">";
    %f2 = "<font:georgia:"@14+0@">";
    %words = getWordCount(%msg) + 1;
    for(%i=0;%i < %words;%i++) {
        if(getSubStr(getWord(%msg, %i), 0, 1) !$= "@" && (%a=strPos(getWord(%msg, %i), "@")) == -1)
            continue;
        if(%a != -1)
            %word = strReplace(getSubStr(getWord(%msg, %i), %a, strLen(getWord(%msg, %i))), "@", "");
        else
            %word = strReplace(getWord(%msg, %i), "@", "");
        if((%id = Chat_CheckIsClient(%word)) != -1)
             %msg = strReplace(%msg, "@" @ %word, "@" @ (%col=collapseEscape("\\c" @ getField(%id, 0))) @ %word @ ((%n=getField(%id, 1)) !$= "" ? " \c7[\c3" @ %col @ %n @ "\c7] " : "") @ "\c6");
    }
    echo(stripMlControlChars("-> \c7" @ (%p = (%pre !$= "" ? "\c7(" @ %pre @ "\c7)\c3 " : "\c3")) @ %name @ (%s = (%suf !$= "" ? "\c7(" @ %suf @ "\c7)" : "")) @ " \c6:" SPC %msg));
    %msg = "<font:georgia:20>\c7->" SPC %f2 @ "\c7" @ stripMlControlChars(%p) @ %f1 @ "\c3" @ %name @ "\c7" @ %f2 @ "\c7" SPC stripMlControlChars(%s) @ %f1 @ "\c6:" SPC "<font:georgia:"@14+$Pref::GUI::ChatSize@">\c6" @ %msg;
    NewChatHud_addLine(%msg);
}

// made by Buddy
function formatString(%string,%a0,%a1,%a2,%a3,%a4,%a5,%a6,%a7,%a8,%a9,%a10,%a11,%a12,%a13,%a14,%a15,%a16,%a17,%a18,%a19)
{
	%return = %string;
	for(%I=0; %I < 20; %I++)
	{
		%pos = stripos(%return, "%s");
		if(%pos != -1)
			%return = setSubStr(%return,%pos,2,%a[%i]);
	}
	return %return;
}
