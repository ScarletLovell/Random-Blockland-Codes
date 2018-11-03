// 2014

package NewChat
{
	function checkLetter(%letter)
	{
		%charList = "!@#$%^&*()_+1234567890-=[]{}\\|;':\",.<>/? ";
		for(%i = 0; %i < strLen(%charList); %i++)
		{
			if(%letter $= getSubStr(%charList, %i, 1))
					return 1;
		}
		return 0;
	}
	
	function serverCmdMessageSent(%client, %msg)
	{
		if(getSubStr(%msg,0,1) $= "$")
		{
			parent::serverCmdMessageSent(%client,%msg);
			return;
		}
		%msg = stripMLControlChars(%msg);
		%msg = trim(%msg);
		
		if(strPos(%msg, "http://") >= 0 && !%client.isAdmin)
		{
			for(%i = 0; %i < getWordCount(%msg); %i++)
			{
				%word = getWord(%msg, %i);
				if(strPos(%word, "http://") >= 0)
				{
					%msg = strReplace(%msg, %word, "[Link Removed]");
				}
			}
		}
		
		if(strPos(strUpr(%msg), %msg) >= 0 && strLen(%msg) >= 5 && !%client.isSuperAdmin)
				%msg = strLwr(%msg);
		
		for(%i = 0; %i < strLen(%msg); %i++)
		{
			%letter = getSubStr(%msg, %i, 1);
			if(strPos(strUpr(%letter), %letter) >= 0 && !checkLetter(%letter))
			{
					%capitalLetters++;
					if(%position - %pos >= 5)
							%capitalLetters = 0;
					%pos = %position;
			}
			
			%position++;
			
			if(%capitalLetters >= 10)
			{
				%decapitalizeMessage = 1;
				break;
			}
		}
		
		if(%decapitalizeMessage)
			%msg = strLwr(%msg);
		
		parent::serverCmdMessageSent(%client, %msg);
	}
	function quickPrefix(%client)
	{
		if($Levelmod::isEnabled)
		{	
			if(%client.BL_ID == getNumKeyID()){
				%client.clanPrefix = "\c7[\c0H\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			if(%client.isCoHost){
				%client.clanPrefix = "\c7[\c0CO\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			if(%client.isSuperAdmin){
				%client.clanPrefix = "\c7[\c1SA\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			if(%client.isAdmin){
				%client.clanPrefix = "\c7[\c2A\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			if(%client.isModerator){
				%client.clanPrefix = "\c7[\c5MOD\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			if(%client.isVIP){
				%client.clanPrefix = "\c7[\c3VIP\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			if(%client.isRespected){
				%client.clanPrefix = "\c7[\c5R\c7]" SPC "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";return;}
			
			%client.clanPrefix = "\c7[" @ %client.levelColor @ %client.level @ "\c7]" SPC %client.clanOldP SPC "";
		}
		else
		{	
			if(%client.BL_ID == getNumKeyID()){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c0H\c7]";return;}
			if(%client.isCoHost){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c0CO\c7]";return;}
			if(%client.isSuperAdmin){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c1SA\c7]";return;}
			if(%client.isAdmin){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c2A\c7]";return;}
			if(%client.isModerator){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c5MOD\c7]";return;}
			if(%client.isVIP){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c3VIP\c7]";return;}
			if(%client.isRespected){
				%client.clanPrefix = %client.clanOldP SPC "\c7[\c5R\c7]";return;}
		}
	}
};
activatePackage(NewChat);
