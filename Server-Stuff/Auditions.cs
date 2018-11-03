package Auditions
{
	if($GameModeArg $= "Add-Ons/GameMode_Auditions/gamemode.txt")
	{
		deactivatePackage(Auditions);
		return;
	}
	function Auditions::RotatePlayers()
	{
		announce("\c6Selecting a player to audition...");
		%count = ClientGroup.getCount();

		for(%cl = 0; %cl < %count; %cl++)
		{
			%clientB = ClientGroup.getObject(%cl);
			if(%clientB.hasAuditiond)
			{
				messageClient(%client,'',"You have already Audition'd, moving on.");
				return;
			}
			else
			{
				cancel($AuditionLoop);
				talk(%clientB.name);
				return;
			}
			announce("\c6Couldn't find a player... Continuing...");
		}
		cancel($AuditionLoop);
		announce("\c6Couldn't find a player to audition... Continuing...");
	}
	function Auditions::RotatePlayers::LOOP()
	{
		$AuditionLoop = schedule(2000, 0, Auditions::RotatePlayers);
	}
};
activatePackage(Auditions);