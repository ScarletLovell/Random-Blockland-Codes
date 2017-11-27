//[Created by Anthonyrules144 - BL_ID 9999]
//Keep in mind, do not say this mod was yours, it is not. You may own this modification and edit it, but please, give credit where it's due, thanks!
// Proper Exec //
exec("./resources/cGUI/cExec.cs");
function refreshGUIs () {

}

// Variables //
if(!isFile("config/client/ctOS.txt")) {
    $ctOS_disabled = false;
    $ctOS_showForce = true;
}

// Remap //
function easyRemap (%types) { if(!$ctOS_bind)for(%i=0;%i < getFieldCount(%types);%i++) {
        $remapName[$remapCount] = "Use " @ getField(%types, %i) @ " item";
        $remapCmd[$remapCount] = "ctOS_use" @ getField(%types, %i) @ "Item";
        $remapCount++;
    } }
if(!$ctOS_bind) {
	$remapDivision[$remapCount] = "ctOS";
	$remapName[$remapCount] = "Open/Close phone";
	$remapCmd[$remapCount] = "ctOS_phone";
	$remapCount++;
    $remapName[$remapCount] = "Show ctOS";
    $remapCmd[$remapCount] = "ctOS_grab";
    $remapCount++;
    easyRemap("top" TAB "left" TAB "right" TAB "bottom");
	$ctOS_bind = 1;
}

package ctOS {
// Main Functions //
// ~ ctOSHACK d3.E-d1s_eC
// En_tE-R  D3D_S-ec || J()1_n  uS
function ctOS_grab  (%key) {
    commandToServer('ctOS_Grab', %key);
}
function clientCmdctOS_ReceiveGrab (%active, %field) {
    if(%active != 1) {
        ctOS_VControlUp.visible = false;
        ctOS_VControlDown.visible = false;
        ctOS_VControlLeft.visible = false;
        ctOS_VControlRight.visible = false;
        ctOS_VControlCenter.visible = true;
        return ctOSHACK_Info.visible = false;
    }
    ctOS_InfoName.setValue(%name = getField(%field, 0));
    ctOS_InfoStatus.setValue(%status = getField(%field, 1));
    ctOS_InfoJob.setValue(%job = getField(%field, 2));
        ctOS_VControlUp.visible = true;
        ctOS_VControlDown.visible = true;
        ctOS_VControlLeft.visible = true;
        ctOS_VControlRight.visible = true;
        ctOS_VControlCenter.visible = false;
    ctOSHACK_Info.visible = true;
    echo(%active TAB %field);
}
function ctOS_phone (%key) {
    if(!%key != 1) return;
    $ctOS_phone = (!$ctOS_phone ? 1 : 0);
    commandToServer('ctOS_phone', $ctOS_phone);
    $ctOS_ServerActive = 0;
}
function clientCmdReceivePhoneUpdate (%active) {
    if(%active) {
        ctOSHACK_VControls.visible = $ctOS_phone;
        ctOSHACK_Span.visible = $ctOS_phone;
    }
}
function ctOS_useTopItem (%key) {
    if(!%key != 1 || !$ctOS_phone) return; commandToServer('ctOS_useTopItem'); }
function ctOS_useLeftItem (%key) {
    if(!%key != 1 || !$ctOS_phone) return; commandToServer('ctOS_useLeftItem'); }
function ctOS_useRightItem (%key) {
    if(!%key != 1 || !$ctOS_phone) return; commandToServer('ctOS_useRightItem'); }
function ctOS_useBottomItem (%key) {
    if(!%key != 1 || !$ctOS_phone) return; commandToServer('ctOS_useBottomItem'); }

};activatePackage(ctOS);
