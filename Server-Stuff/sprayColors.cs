$Pref::Server::CustomColors::ShowForOthers = true;
$Pref::Server::CustomColors::SaveAsBLID = false;

package sprayColors {
    function serverCmdUseSprayCan(%client, %newId) {
        %blid = ($Pref::Server::CustomColors::SaveAsBLID ? %cl.bl_id : %client);
        //if(isObject(%client.miniGame) && !%client.miniGame.EnablePainting || !isObject(%client.player))
            return parent::serverCmdUseSprayCan(%client, %newid);
        %max = $defaultColorsMax;
        // force all nearby cans to change too.
        // i'm doing this to avoid default cans
        %toChange = %newId;
        //%toChange = 
        //    %newId @ 
        //    (%newid > 0 ? " "@%newid-1 : "") @ 
        //    (%newid < %max ? " "@%newid-1 : "") @
        //    (%newid > 9 ? " "@%newid-10 : "") @ 
        //    (%newid < %max-10 ? " "@%newid+10 : "") @
        //    (%newid > %max-10 ? " "@getSubStr(%newid, 1, 1) : "") @
        //    (%newid+1 % 10 == 0 ? " "@%newid-9 : "");
        for(%i=0;%i < getWordCount(%toChange);%i++) {
            %id = getWord(%toChange, %i);
            %color=getRecord($changedColors[%blid], %id);
            if(%color $= "") {
                %color = getRecord($forcedColors, %id);
                if(%color $= "")
                    continue;
            }
            %db = nameToId("color" @ %id @ "sprayCanImage");
            %db2 = nameToId("color" @ %id @ "paintExplosionParticle");
            %db.colorShiftColor = %color;
            %db2.colors[0] = getWords(%color, 0, 2)@ " 0.5";
            %db2.colors[1] = getWords(%color, 0, 2)@ " 0.0";
        }
        CustomColors_transmitDatablocks(%client);
        parent::serverCmdUseSprayCan(%client, %newid);
    }
    function paintProjectilte::onCollision(%this, %obj, %col, %fade, %pos, %normal) {
        //if(!$Pref::Server::CustomColors::ShowForOthers) {
            parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
            return;
        //}
        if(%col.getClassName() !$= "fxDTSBrick") {
            parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
            return;
        }
        %cl = %obj.client;
        %blid = ($Pref::Server::CustomColors::SaveAsBLID ? %cl.bl_id : %cl);
        %color = getRecord($changedColors[%blid], %this.colorID);
        if(%col.colorID == %this.colorID) {
            %colColor = %col.isCustomColor;
            if(%colColor !$= "" && %colColor $= %color) {
                parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
                return;
            } else {
                %col.colorID = %this.colorID + 1;
            }
        }
        if(%color !$= "") {
            %col.isCustomColor = %color;
            CustomColors_PushColor(%this.colorID, %color, getColorIdTable(%this.colorID));
        } else
            %col.isCustomColor = "";
        parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
    }
    function serverCmdSetColor(%cl, %id, %color0, %color1, %color2, %alpha) {
        %nC = colorErrorTest(%cl, %id, %color0, %color1, %color2, %alpha);
        if(%nC == 0) return;
        if(%nC == 2) %alpha = 1;
        %color = %color0 SPC %color1 SPC %color2 SPC %alpha;
        %oldColor = getColorIdTable(%id);
        %blid = ($Pref::Server::CustomColors::SaveAsBLID ? %cl.bl_id : %cl);
        $changedColors[%blid]=setRecord($changedColors[%blid], %id, %color);
        setColorTable(%id, %color);
        %cl.transmitStaticBrickData();
        CustomColors_LoadPaint(%cl);
        schedule(30, 0, setColorTable, %id, %oldColor);
        announce("\c3"@%cl.getPlayerName() SPC "\c6set their color #\c2" @ %id @ " \c6to \c7" @%color@ "");
    }
    function serverCmdSetPublicColor(%cl, %id, %color0, %color1, %color2, %alpha) {
        if(!%cl.isAdmin)
            return;
        %nC = colorErrorTest(%cl, %id, %color0, %color1, %color2, %alpha);
        if(%nC == 0) return;
        if(%nC == 2) %alpha = 1;
        for(%i=0;%i < clientGroup.getCount();%i++) {
            serverCmdSetColor(clientGroup.getObject(%i), %id, %color0, %color1, %color2, %alpha);
        }
        talk("\c3" @%cl.getPlayerName() SPC "\c6set everyone's spraycan #\c2"@%id SPC "\c6to \c7" @ %color0 SPC %color1 SPC %color2 SPC %alpha);
    }
    function serverCmdForceColors(%cl, %go) {
        if(%go !$= "" && getField(%go, 1) !$= "12534") {
            %go = "";
            cancel(%cl.forceColors);
        }
        else if(%go !$= "")
            %go = getField(%go, 0);
        %blid = %cl.bl_id;
        %bg = "BrickGroup_"@%blid;
        for(%i=%go;%i < %go+500;%i++) {
            if(!isObject(%bg.getObject(%i)))
                return;
            %brick = %bg.getObject(%i);
            %brick.setRendering(0);
            %brick.schedule(50, setRendering, 1);
            %color = %brick.getColorId();
            if(%color >= $defaultColorsMax)
                %brick.setColor(%color-1);
            else
                %brick.setColor(%color+1);
            %brick.schedule(50, setColor, %color);
        }
        %cl.forceColors = schedule(50, 0, serverCmdForceColors, %cl, (%go !$= "" ? %go+500 TAB 12534 : 500 TAB 12534));
    }
    function serverCmdResetColors(%client) {
        if($defaultColors[0] $= "")
            getDefaultColors();
        if(%client $= "reset")
            $forcedColors = "";
        if(%client $= "reset" || isObject(%client)) {
            for(%id=0;%id < ($defaultColorsMax+1);%id++) {
                %color = $defaultColors[%id];
                %db = nameToId("color" @ %id @ "sprayCanImage");
                %db2 = nameToId("color" @ %id @ "paintExplosionParticle");
                %db.colorShiftColor = %color;
                %db2.colors[0] = getWords(%color, 0, 2)@ " 0.5";
                %db2.colors[1] = getWords(%color, 0, 2)@ " 0.0";
            }
            if(isObject(%client)) {
                %client.chatMessage("\c2Resetting your colorset!");
                CustomColors_transmitDatablocks(%client);
                $changedColors[$Pref::Server::CustomColors::SaveAsBLID ? %cl.bl_id : %client] = "";
            }
        }
        for(%i=0;%i < clientGroup.getCount();%i++) {
            %cl = clientGroup.getObject(%i);
            %blid = ($Pref::Server::CustomColors::SaveAsBLID ? %cl.bl_id : %cl);
            for(%o=0;%o < 63;%o++) {
                %col = $defaultColors[%o];
                if(%client !$= "reset" && (%color = getRecord($changedColors[%blid], %o)) !$= "")
                    %col = %color;
                setColorTable(%o, %col);
            }
            if(%client $= "reset") 
                CustomColors_transmitDatablocks(%cl);
            %cl.transmitStaticBrickData();
        }
    }
    function serverCmdResetAllColors(%client) {
        if(!%client.isAdmin)
            return;
        serverCmdResetColors("reset");
        announce("\c3" @%cl.getPlayerName() SPC "\c6reset everyone's colors!!");
    }

    function serverCmdRandomizeColors(%cl, %t) {
        if(!%cl.isSuperAdmin)
            return;
        if(%t $= "" || %t < 0 || %t > 1) {
            %cl.chatMessage("\c6Transparency unknown [0-1 only]; auto setting to 1");
            %t = 1;
        }
        announce("\c3" @%cl.getPlayerName() SPC "randomized all colors!!");
        for(%i=0;%i < 64;%i++) {
            %color = getRandom() SPC getRandom() SPC getRandom() SPC %t;
            setColorTable(%i, %color);
            $forcedColors = setRecord($forcedColors, %i, %color);
        }
        transmitColorStuff();
    }
    function serverCmdSetEntireColorset(%cl, %r, %g, %b, %a, %ids) {
        if(!%cl.isSuperAdmin)
            return;
        if(  %r < 0 || %r > 1 ||
             %g < 0 || %g > 1 ||
             %b < 0 || %b > 1 ||
             %a < 0 && %a !$= "" || %a > 1 && %a !$= "") {
            %cl.chatMessage("\c6RGB values must be above 0 or below 1");
            return;
        }
        %client.chatMessage("\c6Set gradients to " @ %r SPC %g SPC %b SPC %a);
        for(%i=0;%i < 64;%i++) {
            %rR = (%r * %i / 255) * 4;
            %rG = (%g * %i / 255) * 4;
            %rB = (%b * %i / 255) * 4;
            %color = %rR SPC %rG SPC %rB SPC (%a !$= "" ? %a : 1);
            setColorTable(%i, %color);
            $forcedColors = setRecord($forcedColors, %i, %color);
        }
        transmitColorStuff();
    }
    function serverCmdGetColor(%cl) {

    }
    function serverCmdSetColorFilter(%cl, %filter) {
        %filterTypes = "COLORNAME  randomized";
    }
    function serverCmdSaveColorset(%cl) {
        if(!%cl.isSuperAdmin)
            return;
    }
    function serverCmdSetColorsetRowName(%cl, %rowId, %a0, %a1, %a2, %a3, %a4, %a5) {
        if(!%cl.isSuperAdmin)
            return;
        if(!isNumber(%rowId)) {
            %client.chatMessage("\c3setColorsetRowName \c0[RowId 1-7] [Name]");
            return;
        }
        if(%a0 $= "") {
            %client.chatMessage("\c3setColorsetRowName \c6[RowId 1-7] \c0[Name]");
            return;
        }
        %msg = "";
        for(%i=0;%i < 6;%i++)
            %msg = %msg @ %a[%i] SPC "";
        %start = 0;
        switch(%rowId) {
            case "1": %start = 0;
            default: %start = (%rowId @ 0+(%rowId-1))-10;
        }
        for(%i=%start;%i < %start+10;%i++)
            setSprayCanDivision(%rowId, %i, %msg);
        serverCmdResetColors();
    }
    function serverCmdHelp(%cl, %a, %b, %c, %d, %e, %f, %g, %h, %i) {
        if(strLwr(%a) $= "customcolors" || strLwr(%a SPC %b) $= "custom colors") {
            %cl.chatMessage("\c5CustomColors, created by Ashleyz4 \c7(9999)");
            %cl.chatMessage("\c7*\c6=Anyone \c2*\c6=Admin \c3*\c6=SA");
            %cl.chatMessage("\c7* \c6/setColor \c7- Sets color ID for that person");
            %cl.chatMessage("\c7* \c6/resetColors \c7- Resets that persons colorset");
            %cl.chatMessage("\c2* \c6/resetAllColors \c7- Resets all colorsets");
            %cl.chatMessage("\c2* \c6/setPublicColor \c7- Sets everyones color ID");
            %cl.chatMessage("\c3* \c6/setEntireColorset \c7- Sets gradients of color for everyone based on what you chose");
            %cl.chatMessage("\c3* \c6/randomizeColors \c7- Randomize everyone's colorset");
            return;
        }
        // don't overwrite other /help commands
        return parent::serverCmdHelp(%cl, %a, %b, %c, %d, %e, %f, %g, %h, %i);
    }
};
activatePackage(sprayColors);

function CustomColors_ResetPaintCan(%id, %db, %db2) {
    if($defaultColors[0] $= "")
        getDefaultColors();
    %old = $defaultColors[%id];
    %db.colorShiftColor = %old;
    %db2.colors[0] = getWords(%old, 0, 2)@ " 0.5";
    %db2.colors[1] = getWords(%old, 0, 2)@ " 0.0";
}

function getDefaultColors() {
    %filename = filePath ($GameModeArg) @  "/colorset.txt";
	if($GameModeArg !$= "") {
		%filename = filePath ($GameModeArg) @  "/colorset.txt";
		if(isFile(%filename))
			%foundGameModeColorSet = 1;
	}
	if(!%foundGameModeColorSet)
        %filename = "config/server/colorSet.txt";
    %file = new FileObject();
	%file.openForRead (%filename);
	%i = -1;
	%divCount = -1;
	while(!%file.isEOF()) {
		%line = %file.readLine();
        if(getSubStr(%line, 0, 4) $= "DIV:") {
			%divName = getSubStr(%line, 4, strlen(%line) - 4);
			setSprayCanDivision (%divCount++, %i, %divName);
		}
		if(%line !$= ""  &&  %i < 63 && getSubStr(%line, 0, 4) !$= "DIV:") {
			%r = mAbs(getWord(%line, 0));
			%g = mAbs(getWord(%line, 1));
			%b = mAbs(getWord(%line, 2));
			%a = mAbs(getWord(%line, 3));

			if(mFloor(%r) != %r  ||  mFloor(%g) != %g  ||  mFloor(%b) != %b  ||  
			   mFloor(%a) != %a  ||  ( %r <= 1  &&  %g <= 1  &&  %b <= 1  &&  %a <= 1))
				$defaultColors[%i++] = %r SPC %g SPC %b SPC %a;
			else
				$defaultColors[%i++] = %r/255 SPC %g/255 SPC %b/255 SPC %a/255;
		}
	}
    $defaultColorsMax = %i;

	%file.close();
	%file.delete();
}

function transmitColorStuff()
{ // buddy
    //$maxSprayColors = %colorCount;
    for(%clientIndex = 0; %clientIndex < clientGroup.getCount(); %clientIndex++)
    {
        %cl = ClientGroup.getObject(%clientIndex);
        %cl.transmitStaticBrickData();
        CustomColors_LoadPaint(%cl);
    }
}

function isNumber(%string) {
    if(trim(%string) $= "")
        return false;
    %numbers = stripChars(%string, "1234567890.");
    if(%numbers $= "" && %numbers !$= ".") {
        return true;
    }
    return false;
}

function colorErrorTest(%cl, %id, %color0, %color1, %color2, %alpha) {
    if(trim(%id) $= "" || %id < 0 || %id > 63) {
        %cl.chatMessage("\c0ID is incorrect");
        %cl.chatMessage("\c6/setColor \c0[ID 0-63]\c6 [R 0-1] [G 0-1] [B 0-1] [A 0-1]");
        return 0;
    }
    if(!(%inId=isNumber(%id)) || 
    !(%inColor0=isNumber(%color0)) || !(%inColor1=isNumber(%color1)) || 
    !(%inColor2=isNumber(%color2))) {
        %cl.chatMessage("\c0Only numbers are allowed!");
        %cl.chatMessage("\c3/setColor " @(%inId ? "\c6" : "\c0")@ "[ID 0-63] "@
            "" @(%inColor0 ? "\c6" : "\c0")@ "[R 0-1] "@
            "" @(%inColor1 ? "\c6" : "\c0")@ "[G 0-1] "@
            "" @(%inColor2 ? "\c6" : "\c0")@ "[B 0-1] "@
            "\c6[A 0-1]");
        if(!%inAlpha)
            %cl.chatMessage("\c6Alpha defaulting to 1!");
        return 0;
    }
    if(%color0 < 0 || %color0 > 1 || %color1 < 0 || %color1 > 1 || 
    %color2 < 0 || %color2 > 1) {
        %cl.chatMessage("\c0RGBA is incorrect");
        %cl.chatMessage("\c3/setColor [ID 0-63] \c0[R 0-1] [G 0-1] [B 0-1] \c6[A 0-1]");
        return 0;
    }
    if(%color0 == 0 && %color1 == 0 && %color2 == 0) {
        %cl.chatMessage("\c6All colors must not be 0!!");
        %cl.chatMessage("\c3/setColor [ID 0-63] \c0[R 0-1] [G 0-1] [B 0-1] [A 0-1]");
        return 0;
    }
    if(%alpha $= "" || %alpha < 0 || %alpha > 1) {
        %cl.chatMessage("Alpha never defined! Alpha defaulting to 1!");
        return 2;
    }
    return 1;
}

function CustomColors_transmitDatablocks(%cl) {
    %t = %cl.lastTransmitDatablocks - getSimTime();
    if(%t !$= "" && %t > 0) {
        if(!isEventPending(%cl.lastTransmitDatablocks))
            %cl.lastTransmitDatablocks = schedule(%t, 0, CustomColors_transmitDatablocks, %cl);
        return;
    }
    commandToClient(%cl, 'PlayGui_LoadPaint');
    %cl.lastTransmitDatablocks = getSimTime() + 2000;
}
function CustomColors_LoadPaint(%cl) {
    // i do this because you'll crash if I don't.
    %t = %cl.lastPaintLoad - getSimTime();
    if(%t !$= "" && %t > 0) {
        if(!isEventPending(%cl.lastPaintLoadSched))
            %cl.lastPaintLoadSched = schedule(%t, 0, CustomColors_LoadPaint, %cl);
        return;
    }
    commandToClient(%cl, 'PlayGui_LoadPaint');
    %cl.lastPaintLoad = getSimTime() + 2000;
}
function CustomColors_PushColor(%id, %color, %resetPlayers) {
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %cl = clientGroup.getObject(%i);
        setColorTable(%id, %color);
        %cl.transmitStaticBrickData();
    }
    if(%resetPlayers)
        schedule(1000, 0, serverCmdResetColors);
}