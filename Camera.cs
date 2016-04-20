//Created by Anthonyrules144
// You can use this for whatever you want, just try to give me credit.

$Camera::Custom = false; //Turn off the servercmd

function serverCmdSetCameraCustom(%client, %dir,%dir1,%dir2, %dist,%dist1,%dist2, %angle, %rot)
{
    if(!%client.isSuperAdmin || $Camera::Custom == false)
        return;
    if(%dir $= "reset")
        return %client.setControlObject(%player);
    if(%dir $= "" || %dist $= "" || %angle $= "")
    {
        messageClient(%client, '', "\c6You're missing something... \c4/setCustomCamera [direction] [direction2] [direction3] [distance] [distance2] [distance3] [angle]");
        return messageClient(%client, '', "\c6If you want, you can also have custom rotation! just add a \c4[rot] \c6at the end of the cmd!");
    }
    %dir = %dir SPC %dir1 SPC %dir2;
    %dist = %dist SPC %dist1 SPC %dist2;
    talk(%client SPC %dir SPC %dist SPC %angle);
    messageClient(%client, '', "\c6Success");

    %client.setCameraCustom(%dir, %dist, %angle, %rot);
}

function GameConnection::setCameraCustom(%client, %dir, %dist, %angle, %rot)
{
    %angles = "overhead below north south east west";
    for(%i=0;%i<getWordCount(%angles);%i++)
    {
        if(%angle $= getWord(%angles, %i))
            %angles = 1;
        else
            continue;
    }
    if(%rot > 90)
        %rot = 90;
    if(%angles == 1)
        switch$(%angle)
        {
            case "overhead":
                %angle = -90;
            case "below":
                %angle = 90;
            case "north":
                %angle = 360;
            case "south":
                %angle = 180;
            case "east":
                %angle = 90;
                if(!%rot)
                    %rot = 90;
            case "west":
                %angle = -90;
                if(!%rot)
                    %rot = 90;
            default:
                return;
        }
    %player = %client.player;
    %camera = %client.camera;
    if(!isObject(%player) || !isObject(%camera))
		return;

    %camera.setFlyMode();
	%camera.mode = "Observer";
    %camera.setTransform(%player.getTransform());
    %client.setControlObject(%camera); %camera.setControlObject(%player);
    %camera.setOrbitMode(%player, %camera.getPosition() SPC eulerToAxis(%direction + %angle SPC %rot), getWord(%dist, 0), getWord(%dist, 1), getWord(%dist, 2));
}

function eulerToAxis(%euler)
{ // created by Trader.
	%euler = vectorScale(%euler, $pi / 180);
	%matrix = matrixCreateFromEuler(%euler);
	return getWords(%matrix, 3, 6);
}
