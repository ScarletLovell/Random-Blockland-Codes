// 2015

function serverCmdSad(%client, %text)
{
    if(%text $= "kitty" || %text $= "kitten")
        return messageClient(%client, '', "\c6Noone likes sad kitties ;( ");
    %rand = getRandom(1,5);
    if(%rand == 1) return messageClient(%client, '', "\c6Have you not found out this is useless to you?");
    if(%rand == 2) return messageClient(%client, '', "\c6You realize there's no point in doing this right?");
    if(%rand == 3) return messageClient(%client, '', "\c6You must be sad\c3" SPC %client.name @ "\c6!");
    if(%rand == 4) return messageClient(%client, '', "\c6Everyone gets sad once in a while.");
    if(%rand == 5) return messageClient(%client, '', "\c6Atleast you tried\c3" SPC %client.name @ "\c6.");
}
