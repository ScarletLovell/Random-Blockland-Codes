//Created by Anthonyrules144
// You can use this for whatever you want, just try to give me credit.

function returnVar(%var, %t)
{
    %numbers = "1 2 3 4 5 6 7 8 9";
    %words = "a b c d e f g h i j k l m n o p q r s t u v w y x z";
    if(strLen(%var) < 2 || %var $= "true" || %var $= "false")
        %this = "Boolean:" SPC %var;
    else
        for(%i=0;%i < strLen(%var);%i++)
        {
            for(%o=0;%o <= getWordCount(%words);%o++)
                if(getSubStr(%var, %o - 1, %o) $= getWord(%words, %o))
                {
                    %this = "String:" SPC %var;
                    break;
                }
                else
                    continue;
            if(getWord(%this, 0) !$= "String:")
                for(%o=0;%o < 10;%o++)
                    if(getSubStr(%var, %o - 1, %o) $= getWord(%numbers, %o))
                    {
                        if(getSubStr(%var, %o - 1, %o) $= ".")
                            %decimal = " decimal";
                        %this = "Number" @ %decimal @ ":" SPC %var;
                    }
        }
    if(%t == 1)
        return talk(%this);
    else
        return %this;
}
