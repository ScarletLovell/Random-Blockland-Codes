function clientCmdChatMessage(%sender, %voice, %pitch, %msgString, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10) {
	onChatMessage(detag(%msgString), %voice, %pitch);
    %name = %a2;
    if(%name $= "Zeb")
        return;
    %msg = %a4;
    if(%name $= "Ashleyz4") {
        if(%msg $= "zeb turn off") {
            zebSchedule(0);
            commandToServer('messageSent', "Shutting dow...");
            Zeb_shuttingDown(1);
        } else if(%msg $= "zeb turn on") {
            zebSchedule(1);
            commandToServer('messageSent', "Reloading...");
            commandToServer('messageSent', "Ready!");
            Zeb_turnOn(1);
        }
    }
    %trim = trim(%msg);
    %atZeb = strPos(strReplace(getWord(strLwr(%trim), 0), ",", ""), "zeb");
    if(%atZeb == -1)
        return;
    if(%atZeb > 4)
        return;
    //if(getSubStr(getWord(strLwr(%trim), 0), 0, 3) !$= "zeb")
    //    return;
	%waz = strLwr(getWords(%trim, 1, getWordCount(%trim)-1));
    if(getWords(%waz, 0, 1) $= "who is") {
        
    }
	if(%waz $= "follow me" || %waz $= "follow") {
		%player = ConfirmPlayer(%name);
		if(%player != 0) {
			ZebFollowPlayer(1, %player);
		}
	}
	if(%waz $= "stop") {
		if(isEventPending($ZebFollowPlayer)) {
            ZebFollowPlayer(0);
            $ZebFollowPlayer = 0;
            commandToServer('messageSent', "Stopped following you...");
		} else {
            if($zebIsActive)
                schedule(30000, 0, zebSchedule, 1);
            $zebIsActive = false;
            commandToServer('messageSent', "Stopping for 30 seconds...");
            moveForward(0);
        }
	}
    if(%waz $= "sit") {
        commandToServer('sit');
    } else if(%waz $= "alarm") {
        commandToServer('alarm');
    } else if(%waz $= "hate") {
        commandToServer('hate');
    } else if(%waz $= "love") {
        commandToServer('love');
    } else if(%waz $= "confusion" || %waz $= "confuse") {
        commandToServer('confusion');
    }
    if(getWordCount(%trim) < 2) {
        %record = getRecord($defaultSayings["hi"], getRandom(0, getRecordCount($defaultSayings["hi"])));
        %record = strReplace(%record, "%1", %name);
        //%record = strReplace(%record, "%2", clientGroup.getObject(getRandom(0, clientGroup.getCount())));
        stopAndSaySomething(%record);
        return;
    }
    %mathCheck=getMath(%math = getWords(%trim, 1, getWordCount(%trim)-1));
    if(%mathCheck) {
        %return = eval("return " @ %math @ ";");
        echo(%math SPC %return);
        if(%return !$= "") {
            commandToServer('messageSent', %name @ ": " @ %math @ " = " @ %return);
        } else
            stopAndSaySomething("That equation has an error, " @ %name @ ".");
        return;
    }
    %force = "";
    for(%i=1;%i < getWordCount(%trim);%i++) {
        %words = getWords(%trim, 1, %i);
        %words = stripChars(%words, "!@#$%^&*()_-=+[];\'\"<>/.,{}:");
        if($defaultSayings[%words] !$= "" && %force $= "") {
            %force = %words;
        } else if(%force !$= "") {
            %rest = %rest @ getWord(%trim, %i) SPC "";
        }
        %argv[%i] = getWord(%trim, %i);
    }
    if(%force !$= "") {
        %record = getRecord($defaultSayings[%force], getRandom(0, getRecordCount($defaultSayings[%force])));
        %record = strReplace(%record, "%1", %name);
        //%record = strReplace(%record, "%2", clientGroup.getObject(getRandom(0, clientGroup.getCount())));
        %record = strReplace(%record, "%3", strReplace(trim(%rest), " me ", " you "));
        echo(%record);
        stopAndSaySomething(%record);
    }
}

function addUserToSqlite(%name, %blid) {
    sqlite_open("config/codes/db/players.db");
    sqlite_query("CREATE TABLE Players(BLID INT PRIMARY KEY NOT NULL, Name TEXT NOT NULL);");
    sqlite_query("INSERT INTO Players (BLID, Name) VALUES ("@%blid@", '"@%name@"');");
    sqlite_close();
}
function getPlayerFromSqlite(%name) {
    sqlite_open("config/codes/db/players.db");
    %result = sqlite_query("SELECT * FROM Players WHERE Name LIKE \'%"@%name@"%\'");
    sqlite_close();
    return %result;
}

function stopAndSaySomething(%say) {
    %time = strLen(%say)*100;
    if($zebIsActive) {
        zebSchedule(0);
        schedule(%time + getRandom(25, 400), 0, zebSchedule, 1);
    }
    commandToServer('startTalking');
    schedule(%time, 0, commandToServer, 'stopTalking');
    schedule(%time, 0, commandToServer, 'messageSent', %say);
}

function getDefaultSaying(%f) {
    %fullName = strReplace(fileName(%f), ".txt", "");
    %fullName = strReplace(%fullName, "_", " ");
    %fullName = strReplace(%fullName, "-", "\t");
    %file = new FileObject();
    %file.openForRead(%f);
    %o = -1;
    while(!%file.isEoF()) {
        %line = %file.readLine();
        for(%i=0;%i < getFieldCount(%fullName);%i++) {
            %name = getField(%fullName, %i);
            echo(%name SPC %i);
            $defaultSayings[%name] = setRecord($defaultSayings[%name], %o++, %line);
        }
    }
    %file.close();
    %file.delete();
}
%path = "config/codes/defaultSayings/*.txt";
for(%file = findFirstFile(%path);%file !$= "";%file = findNextFile(%path)) {
    getDefaultSaying(%file);
}

function getMath(%msg) {
    %math = stripChars(%msg, "1234567890*()%+-<>/.^ ");
    if(%math !$= "")
        return false;
    return true;
}
