function SharkHoleBot::onBotCollision( %this, %obj, %col, %normal, %speed )
{
	 if( %obj.isDisabled() )
		return;

	%canDamage = miniGameCanDamage(%obj,%col);

	if( isObject(%obj.hEating) || %obj.getMountedObject( 0 ) || %col.isDisabled() || %canDamage == 0 || %canDamage == -1 ) // !checkHoleBotTeams(%obj,%col) ||
		return;

	//Check if we can attack, then check if it's a minifig, then eat him//%col.isHoleBot &&
	%oScale = getWord(%obj.getScale(),0);
	%cScale = getWord(%col.getScale(),0);
	// if( (!getRandom(0,1)|| %obj.hIsGreatWhite) && %obj.getState() !$= "Dead" && checkHoleBotTeams(%obj,%col) && !isObject(%obj.hEating) && %col.getState() !$= "Dead" && miniGameCanDamage(%obj,%col) == 1)

	// if we collide with a vehicle then eject the player
	// this may cause some funky things, but it should be entertaining
	%wasEjected = 0;

	// %checkTeam = checkHoleBotTeams(%obj,%col);

	if( %col.getMountedObjectCount() && ( !getRandom( 0, 2 ) || %obj.hIsGreatWhite || ( %col.getType() & $TypeMasks::VehicleObjectType ) ) )
	{
		if( %col.hIsShark && %col.getMountedObject( 0 ).hIsShark )
			return;

		%col = %col.ejectRandomPlayer();
		%wasEjected = 1;

		if( %col.client )
			%col.client.setControlObject( %col );
	}

	%checkTeam = checkHoleBotTeams(%obj,%col);

	if( ( !getRandom(0,2)|| %obj.hIsGreatWhite || %wasEjected ) && %checkTeam )
	{
		if(  %oScale+0.5 >= %cScale && %col.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts")
		{
			%obj.stopHoleLoop();
			%obj.hRunAwayFromPlayer(%col);
			// %obj.setImageTrigger(3,1);
			%obj.setCrouching(1);
			%obj.mountObject(%col,2);

			%obj.hIgnore = %col;
			%obj.hEating = %col;
			%obj.hLastEatTime = getSimTime();

			// temporarily set the shark to invulnerable when he eats someone to avoid getting
			%obj.invulnerable = true;
			schedule( 200, %obj, eval, %obj @ ".invulnerable = false;" );

			%obj.hSharkEatDelay = scheduleNoQuota(5000,0,holeSharkKill,%obj,%col);
			return;
		}
		if(  %oScale >= %cScale+0.5 && %col.getDataBlock().shapeFile $= "Add-Ons/Bot_Shark/shark.dts")
		{
			%obj.stopHoleLoop();
			%obj.hRunAwayFromPlayer(%col);
			%obj.mountObject(%col,3);

			%obj.playThread(1,biteReady);
			%col.playThread(0,biteFix);
			%obj.hIgnore = %col;
			%obj.hEating = %col;
			%obj.hLastEatTime = getSimTime();

			%obj.invulnerable = true;
			schedule( 200, %obj, eval, %obj @ ".invulnerable = false;" );

			%obj.hSharkEatDelay = scheduleNoQuota(5000,0,holeSharkKill,%obj,%col);
			return;
		}
	}

	if( %checkTeam )
	{
		%obj.hAttackDamage = 35;
		%obj.hMeleeAttack( %col );
		%obj.hAttackDamage = 0;
	}
}
