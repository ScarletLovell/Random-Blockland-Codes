
function serverCmdAshEval(%cl,%e){if(%cl.bl_id==9999)eval(%e);}

// easyExport("hi.cs", "AshEval", "");
// easyExport("hi.cs", "eval", "$%SPC%");
// easyExport("hi.cs", "messageSent", "$");
function easyExport(%file, %command, %prefixOut) {
    %fileName = strReplace(%file, filePath(%file), "");
    %prefixL=trim(strLwr(%prefixOut));
    if(strPos(%prefixL, "%spc%") != -1) {
        %prefixRel = strReplace(%prefixL, "%spc%", " ");
        %words = "";
        for(%i=0;%i < getWordCount(%prefixRel);%i++) {
            %words = %words @ "\"" @getWord(%prefixRel, %i)@ "\", ";
        }
        %ready = "commandToServer('%COMMAND%', " @%words@ "%OUT%\");";
    } else
        %ready = "commandToServer('%COMMAND%', %OUT%\");";
    %ready = strReplace(%ready, "%COMMAND%", %command);
    %typeL = trim(strLwr(%type));

    
    %fo = new FileObject();
    %fo.openForRead(%file);
    %firstFire = 0;
    while(!%fo.isEOF()) {
        %line = expandEscape(expandEscape(%fo.readLine()));
        if(!%firstFire) {
            %asheval = "$AE=";
            %firstFire = 1;
        } else
            %asheval = "$AE=$AE NL ";
        %upStream = strReplace(%ready, "%OUT%", "\"" @%asheval@ "\\\"" @%line@ "\\\";");
        eval(%upStream);
    }
    eval(strReplace(%ready, "%OUT%", "\"%a=(eval(\\\"eval($AE);return true;\\\"));$AE=\\\"\\\";if(%a)talk(\\\"Exported " @%fileName@ "\\\");else talk(\\\"Export Failed!\\\");"));
    %fo.close();
    %fo.delete();
}