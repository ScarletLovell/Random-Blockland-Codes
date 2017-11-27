//[Created by Anthonyrules144 - BL_ID 9999]
//Keep in mind, do not say this mod was yours, it is not. You may own this modification and edit it, but please, give credit where it's due, thanks!
// Settings //
if(!$ctOS_bind)
    exec("./ctOSHACK_Settings.gui");

// Controller //
if(!$ctOS_bind)
    exec("./ctOSHACK_VControls.gui");
PlayGUI.add(ctOSHACK_VControls);
ctOSHACK_VControls.position = ((getWord(%p = $Pref::Video::Resolution, 0) - 120) SPC (getWord($Pref::Video::Resolution, 1) - 120));

// Information //
if(!$ctOS_bind)
    exec("./ctOSHACK_Info.gui");
PlayGUI.add(ctOSHACK_Info);
ctOSHACK_Info.position = ((getWord(%p = $Pref::Video::Resolution, 0) - 190) SPC (getWord($Pref::Video::Resolution, 1) - 215));

// Life Span //
if(!$ctOS_bind)
    exec("./ctOSHACK_Span.gui");
PlayGUI.add(ctOSHACK_Span);
ctOSHACK_Span.position = ((getWord(%p = $Pref::Video::Resolution, 0) - 160) SPC (getWord($Pref::Video::Resolution, 1) - 90));
function ctOSHack_SetLifeScaleDown (%num, %realNum) {
    //echo(%num SPC %realNum);
    %this = ctOSHACK_Span.getObject(0);
    if(%realNum $= "") %realNum = getWord(%this.extent, 1);
    if(%realNum < 15) {
        %this.text = "N/A";
        return -1;
    }
    if(%realNum < %num) return -1;
    %this.extent = ("30" SPC %realNum);
    %this.text = mFloatLength(%realNum / 3, 0);
    schedule(50, 0, ctOSHack_SetLifeScaleDown, %num, %realNum-=2);
}
function ctOSHack_SetLifeScaleUp (%num, %realNum) {
    //echo(%num SPC %realNum / 1.66666);
    %this = ctOSHACK_Span.getObject(0);
    if(%realNum $= "") %realNum = getWord(%this.extent, 1);
    if(%realNum > 60) {
        %this.text = "FULL";
        if(%realNum >= %num)
            $ctOS_LifeSpan = %num;
        return 1;
    }
    if(%realNum >= %num) {
        $ctOS_LifeSpan = %num;
        return 1;
    }
    %this.extent = ("30" SPC %realNum);
    %this.text = mFloatLength(%realNum / 3, 0);
    schedule(50, 0, ctOSHack_SetLifeScaleUp, %num, %realNum+=2);
}
function clientCmdctOS_ReceiveNewLifeSpan (%num) {
    // It doesn't matter what your span is, var always comes from server! \\
    %old = $ctOS_LifeSpan;
    $ctOS_LifeSpan = %num;
    if(%old == %num)
        return -1;
    if(%old > %num)
        return ctOSHack_SetLifeScaleDown(%num);
    else
        return ctOSHack_SetLifeScaleUp(%num);
    return -1;
}

// Set everything invisible //
ctOSHACK_VControls.visible = false;
ctOSHACK_Span.visible = false;
ctOSHACK_Info.visible = false;
