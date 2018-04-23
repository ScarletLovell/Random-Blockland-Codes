if(!isObject(musicData_Wait_afterDawn))
    datablock AudioProfile(musicData_Wait_afterDawn)
    {
        filename = "config/Wait_afterDawn.ogg";
        description = AudioMusicLooping3d;
        preload = false;
        uiName = "afterDawn";
    };
//    };
function BK_PushAudioData() {
    $music__Wait_afterDawn = 1;
    $music__Wait_nothingNew = 1;
    transmitNewFiles();
}
BK_PushAudioData();

//transmitNewFiles() code made by Zeblote
if(!isObject(ActiveDownloadSet))
    new SimSet(ActiveDownloadSet);
function transmitNewFiles()
{
    if(ActiveDownloadSet.getCount() != 0)
    {
        messageAll('', "\c6Downloads are already in progress! Cannot start again until finished.");
        return;
    }
    messageAll('', "\c6Starting download of new files...");
    setManifestDirty();
    %hash = snapshotGameAssets();
    for(%i = 0; %i < ClientGroup.getCount(); %i++)
    {
        %client = ClientGroup.getObject(%i);
        if(!%client.hasSpawnedOnce)
        {
            commandToClient(%client, 'GameModeChange');
            %client.schedule(10, delete, "Please rejoin!");
        }
        else
        {
            %client.sendManifest(%hash);
            ActiveDownloadSet.add(%client);
        }
    }
}
function onAllFinishDownload()
{
    messageAll('', "\c6All clients finished downloading the new files!");
    %cg = clientGroup.getID();
    for(%I = %cg.getCount()-1; %I >= 0; %I--)
        commandToClient(%cg.getObject(%I),'missionStartPhase3');
}
package ReDownload
{
    function serverCmdBlobDownloadFinished(%client)
    {
        parent::serverCmdBlobDownloadFinished(%client);
        if(ActiveDownloadSet.isMember(%client))
        {
            ActiveDownloadSet.remove(%client);
            messageAll('', '\c6%1 finished downloading, %2',%client.getPlayerName(), ActiveDownloadSet.getCount());
            if(ActiveDownloadSet.getCount() == 0)
                schedule(1000,0,onAllFinishDownload);
        }
    }
    function GameConnection::onClientLeaveGame(%client)
    {
        if(ActiveDownloadSet.isMember(%client))
        {
            ActiveDownloadSet.remove(%client);
            if(ActiveDownloadSet.getCount() == 0)
                schedule(1000,0,onAllFinishDownload);
        }
        parent::onClientLeaveGame(%client);
    }
};
activatePackage(ReDownload);
