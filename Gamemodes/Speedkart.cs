// 2014

package SpeedkartStuff
{
	function speedkartLoadRecords()
	{
		return; //not done yet

		%Record = "config/server/Speedkart/" @ $AjsCommands::Speedkart::TrackName @ ".txt";
		if(isFile(%Record))
		{
  			%file = new FileObject();
			%file.openforRead(%creditsFilename);

	  		%line = %file.readLine();
	  		%line = stripMLControlChars(%line);
	  		%loadMsg = "Unavalible";

	  		%file.close();
	  		%file.delete();
		}
		else
		{
			announce("\c6"@$AjsCommands::Speedkart::TrackName SPC "\c6Record does not exist, creating it...");

			%file = new FileObject();
			%file.openForAppend("config/server/Speedkart/" @ $AjsCommands::Speedkart::TrackName @ ".txt");
			%file.writeLine("3000");
			%file.close();
			%file.delete();
			return;
		}
	}
	function SpeedartCheckRecord(%record)
	{
		//enter code here
		return;
	}
	function SpeedkartStartCountRecord(%client)
	{
		return; //not done yet

		schedule(19000, 0, SpeedkartCountRecord, %client);
	}
	function SpeedkartCountRecord(%client)
	{
		return; //not done yet

		%client.Record++;
		cancel($CountRecord[%client.BL_ID]);
		$CountRecord[%client.BL_ID] = schedule(1000, 0, SpeedkartCountRecord, %client);
	}

	function giveEXP(%client)
	{
		if($Levelmod::isEnabled)
		{
			%EXP = getRandom(6,12);
			%client.EXP += %EXP;
			if(%client.EXP >= %client.maxEXP)
			{
				%extra = %client.EXP - %client.maxEXP;
				%client.EXP = %extra;
				%client.maxEXP += 12;
				%client.level += 1;
				schedule(1000, 0, saveEverything, %client);
				announce("\c4" @ %client.name @ " \c6has gained a level! \c6[\c4" @ %client.level @ "\c6]");
				quickPrefix(%client);
				return;
			}
			messageClient(%client,'',"\c6You have gained \c4" @ %EXP @ "\c6 EXP. \c6[\c4" @ %client.EXP @ "\c6/\c4" @ %client.maxEXP @ "\c6]");
			quickPrefix(%client);
		}
	}
};
activatePackage(SpeedkartStuff);