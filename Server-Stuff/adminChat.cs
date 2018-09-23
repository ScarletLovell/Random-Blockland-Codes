function serverCmdac(%cl, 
        %a0, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, 
        %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19) {
    if(!%cl.isAdmin)
        return;
    %a = "";
    for(%i=0;%i < 20;%i++) {
        %word = %a[%i];
        %http = "http://";
        if(strPos(%word, %http) == 0) {
            %word = strReplace(%word, %http, "");
        }
        %a = %a @ %word @ " ";
    }
    %a = stripMLControlChars(%a);
    for(%i=0;%i < clientGroup.getCount();%i++) {
        %c = clientGroup.getObject(%i);
        if(%c.isAdmin) {
            %c.chatMessage("\c7[\c2ADMINCHAT \c3" @%cl.getPlayerName()@ "\c7]\c6: " @ %a);
        }
    }
}