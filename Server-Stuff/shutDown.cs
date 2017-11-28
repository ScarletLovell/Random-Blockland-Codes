// 2015

function shutDownCountDown(%number) {
    cancel($shutDownCountDown);
    announce("\c6Shutting down in\c5" SPC %number);
    announce("\c6If you want to rejoin after the restart you can");
    if(%number == 0) {
        announce("\c0Goodbye.");
        schedule(500, 0, quit);
    }
    $shutDownCountDown = schedule(1000, 0, shutDownCountDown, %number - 1);
}
