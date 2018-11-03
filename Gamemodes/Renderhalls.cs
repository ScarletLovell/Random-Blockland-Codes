// 2014

if($GameModeArg !$= "Add-Ons/GameMode_Renderhalls/gamemode.txt")
	return;

package Renderhalls
{
	function serverCmdMakeTeam(%client,%teamName,%option)
	{
		if(%teamName $= "")
		{
			messsageClient(%client,'',"\c6You need a \c4Team Name, \c6/MakeTeam \c4[Team Name] \c6[Private or Open]");
			return;
		}
		if(%option $= "")
		{
			messsageClient(%client,'',"\c6You need a \c4Team Type, \c6/MakeTeam [Team Name] \c4[Private or Open]");
			return;
		}
		if(%option $= "Open")
		{
			return;
		}
		if(%client.isVIP && %option $= "Private")
		{
			announce("\c4" @%client.name SPC "\c6has created the\c4" SPC %teamName SPC "\c6Team!");
			announce("\c6This team is \c4" SPC %option SPC "\c6Join it by saying /joinTeam" SPC %teamName);
			saveTeam(%client,%teamName,%option);
		}
		else
		{
			messageClient(%client,'',"\c6You need to be \c4VIP \c6to have a Private team!");
			messageClient(%client,'',"\c6You can buy VIP at <a:www.ajsserverhosting.weebly.com/Donations.html>AjsServerHosting.weebly.com");
			return;
		}
	}
	function serverCmdMyTeam(%client)
	{
		messageClient(%client,'',"\c6Your team is:\c4" SPC %client.teamName);
		messageClient(%client,'',"\c6Your team level is: \c4Unknown");
		messageClient(%client,'',"\c6Players in your team are:");
		messageClient(%client,'',"\c4Your team is: \c6");
	}
	function saveTeam(%client,%teamName,%option)
	{
		%path = "config/server/Renderhalls/" @%teamName@ ".txt";
		if(isFile(%path))
		{
			messageClient(%client,'',"\c6That team already exists.");
			return;
		}
		%teamOwner = %client.name;
		%file = new FileObject();
		%file.openForAppend(%path);
		%file.writeLine(%teamOwner);
		%file.writeLine("\n"@%teamName);
		%file.writeLine("\n"@%option);
		%file.close();
		%file.delete();
	}
	function serverCmdJoinTeam(%client,%teamName)
	{
		%path = "config/server/Renderhalls/" @%teamName@ ".txt";
		if(isFile(%path))
		{
  			%file = new FileObject();
			%file.openforRead(%path);

	  		%line = %file.readLine();
	  		%line = stripMLControlChars(%line);

	  		%teamName = getField(%line,0);
	  		%option = getField(%line,1);

	  		%file.close();
	  		%file.delete();
		}
		else
		{
			messageClient(%client,'',"\c6That team doesn't exist!");
			return;
		}

		if(%option $= "Private")
		{
			messageClient(%client,'',"\c6This team is Private, you can't join it!");
			messageClient(%client,'',"\c6If you want to join the team" SPC %teamName SPC "\c6, you must be invited.");
			return;
		}
		if(%client.team $= %teamName)
		{
			messageClient(%client,'',"\c6You are already on that team!");
			return;
		}
		jointeam(%client,%teamName);
	}
};
activatePackage(Renderhalls);