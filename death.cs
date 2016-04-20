//Created by Anthonyrules144
// You can use this for whatever you want, just try to give me credit.
//  LOTS OF SPAMMY CODE INCOMING!

function sendMsgClientKilled_Impact( %msgType, %client, %sourceClient, %damageType )
{ //Falling to the ground
	%mg = %client.minigame;
    %rad = getRandom(1,8);
    if(%rad == 1) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3The ground \c6killed \c3%1.', %client.name );
    if(%rad == 2) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3%1 \c6tried to be an anvil!', %client.name );
    if(%rad == 3) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3%1 \c6got squished.', %client.name );
    if(%rad == 4) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3%1 \c6was changing the laws of gravity.', %client.name );
    if(%rad == 5) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3The ground \c6smacked \c3%1\c6.', %client.name );
    if(%rad == 6) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3%1 \c6tossed themself.', %client.name );
    if(%rad == 7) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3The ground \c6gave up on \c3%1\c6.', %client.name );
    if(%rad == 8) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3%1 \c6tried to be lucky.', %client.name );
    if(%rad == 9) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/crater> \c3%1 \c6got rejected by gravity.', %client.name );
}
function sendMsgClientKilled_Hammer( %msgType, %client, %sourceClient, %damageType )
{
    %rad = getRandom(1,3);
    if(%rad == 1) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6thought it was hammer time with \c3%2\c6.', %sourceClient, %client.name );
    if(%rad == 2) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6brought the law with \c3%2\c6.', %sourceClient, %client.name );
    if(%rad == 3) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6was just being blunt with \c3%2\c6.', %sourceClient, %client.name );
}
function sendMsgClientKilled_Suicide( %msgType, %client, %sourceClient, %damageType )
{ //Killing yourself.
	%mg = %client.minigame;
    %rad = getRandom(1,20);
    if(%rad == 1) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6tried their best.', %client.name );
    if(%rad == 2) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6killed themself.', %client.name );
    if(%rad == 3) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6\'probably\' will be remembered.', %client.name );
    if(%rad == 4) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6had an accident.', %client.name );
    if(%rad == 5) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6pointed a gun the wrong way.', %client.name );
    if(%rad == 6) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6was too hasty.', %client.name );
    if(%rad == 7) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6forgot how to live again.', %client.name );
    if(%rad == 8) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6just realised they cant respawn.', %client.name );
    if(%rad == 9) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c601000100 01101001 01100101 01100100.', %client.name );
    if(%rad == 10) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6got greedy with power.', %client.name );
    if(%rad == 11) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1\c6\'s timeline is over.', %client.name );
    if(%rad == 12) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c6Another one bites the dust, ain\'t that right \c3%1\c6?', %client.name );
    if(%rad == 13) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6found an alternative to CTRL K.', %client.name );
    if(%rad == 14) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6blinked.', %client.name );
    if(%rad == 15) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6will probably complain about this death.', %client.name );
    if(%rad == 16) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6wasn\'t ready!', %client.name );
    if(%rad == 17) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6is Dead! Dead! Dead!', %client.name );
    if(%rad == 18) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6can\'t see themself anymore.', %client.name );
    if(%rad == 19) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c3%1 \c6is out!', %client.name );
    if(%rad == 20) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/death> \c6And \c3%1 \c6was never heard from again...', %client.name );
}
function sendMsgClientKilled_Default( %msgType, %client, %sourceClient, %sourceObject, %damageType )
{ //Check for type of kill or person -> person killing
	%mg = %client.minigame;
    if(isObject(%sourceObject))
        %name = %sourceObject.name;
    else
        %name = "AIPlayer";

    //echo("Dead MSG;" SPC %msgType SPC %client SPC %client.name SPC "victim;" SPC %sourceClient SPC "victim name;" SPC %name SPC "victim object;" SPC %sourceObject SPC %damageType);

    if(%damageType == 2 || %name $= %client.name)
		return sendMsgClientKilled_Suicide( %msgType, %client, %sourceClient, %damageType );
    if(%damageType == 10)
        return sendMsgClientKilled_Hammer( %msgType, %client, %name, %damageType );
    if(%damageType == 5 || %damageType == 6)
        return sendMsgClientKilled_Impact( %msgType, %client, %sourceClient, %damageType );

   else if ( %sourceClient.team !$= "" && %sourceClient.team $= %client.team )
   {
        %rad = getRandom(1,6);
        if(%rad == 1) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6tried their luck too hard at \c3%2 \c6- \c0Friendly fire', %name, %client.name );
        if(%rad == 2) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6thought \c3%2 \c6was a target dummy - \c0Friendly fire', %name, %client.name );
        if(%rad == 3) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6had a virus on \c3%2\c6 - \c0Friendly fire', %name, %client.name );
        if(%rad == 4) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6just really wanted it back at \c3%2\c6 - \c0Friendly fire', %name, %client.name );
        if(%rad == 5) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%2 \c6was so confused to see \c3%1\c6 shoot them in the face - \c0Friendly fire', %name, %client.name );
        if(%rad == 6) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6used \c3%2\c6 - \c0Friendly fire', %name, %client.name );
        if(%rad == 6) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%2 \c6was punished by \c3%1\c6 - \c0Friendly fire', %name, %client.name );
    }
   else
   {
        %rad = getRandom(1,13);
        if(%rad == 1) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6roasted \c3%2\c6!', %name, %client.name );
        if(%rad == 2) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6tried to translate \c3%2.',%name, %client.name );
        if(%rad == 3) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6gave \c3%2 \c6their problems.', %name, %client.name );
        if(%rad == 4) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c6Once upon a time, \c3%1 \c6killed \c3%2\c6; the end!', %name, %client.name );
        if(%rad == 5) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6gave it their all at \c3%2\c6!', %name, %client.name );
        if(%rad == 6) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6had a lucky \'shot\' with \c3%2\c6.', %name, %client.name );
        if(%rad == 7) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6stopped \c3%2\c6\'s doom!.', %name, %client.name );
        if(%rad == 8) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6went down in fury because of \c3%2\c6.', %name, %client.name );
        if(%rad == 9) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6is getting better at shooting \c3%2\c6.', %name, %client.name );
        if(%rad == 10) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6got \c3%2\c6.', %name, %client.name );
        if(%rad == 11) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c6in a galaxy far far away, \c3%1 \c6killed \c3%2\c6.', %name, %client.name );
        if(%rad == 12) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6shot first at \c3%2\c6.', %name, %client.name );
        if(%rad == 13) %mg.messageAll( %msgType, '<bitmap:base/client/ui/ci/generic> \c3%1 \c6made \c3%2 \c6a meme.', %name, %client.name );
   }
}

package newDeath
{
	function MiniGameSO::messageAll(%obj, %exception, %msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10)
	{
		parent::messageAll(%obj, %exception, %msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10);
	}
    function GameConnection::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc)
    {
        parent::onDeath(%client, %sourceClient, %sourceObject, %damageType, %damLoc);
        %msgType = "";
        sendMsgClientKilled_Default(%msgType, %client, %sourceClient, %sourceObject, %damageType );
        //do nothing
    }
};
activatePackage(newDeath);
