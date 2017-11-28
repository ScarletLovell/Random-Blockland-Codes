// 2017

function pingScore() {
    cancel($pingScore);
    for(%i=0;%i < clientgroup.getcount();%i++)
        clientGroup.getObject(%i).setScore(clientGroup.getObject(%i).getPing());
    $pingScore = schedule(500, 0, pingScore);
}
