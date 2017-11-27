function Player::ForceThirdPerson(%player, %value) {
    %client = %player.client;
    %player.getDatablock().thirdPersonOnly = %value;
    %client.transmitDatablocks(1);
}
