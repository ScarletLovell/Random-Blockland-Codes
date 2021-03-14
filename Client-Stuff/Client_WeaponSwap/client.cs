// 2021

$IsInWeaponMode = false;
$IsUsingSlot1 = false;
$WeaponSwap::CurrentSlot = -1;

package WeaponSwap {
    function useBricks(%a) {
        $IsUsingSlot1 = (%a == 1);
        if($IsInWeaponMode) {
            swapToolSpotFor(0, %a);
        } else
            Parent::useBricks(%a);
    }
    function swapToolSpotFor(%a, %clickType) {
        if(%clickType == 1) {
            if($WeaponSwap::CurrentSlot == %a) {
                useTools(1);
                return;
            }
            $WeaponSwap::CurrentSlot = %a;
            clientCmdSetActiveTool(%a);
            setActiveTool(%a);
        }
    }
    function useFirstSlot(%a) {
        $IsUsingSlot1 = (%a == 1);
        if($IsInWeaponMode) {
            swapToolSpotFor(0, %a);
        } else
            Parent::useFirstSlot(%a);
    }
    function useSecondSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(1, %a);
        } else
            Parent::useSecondSlot(%a);
    }
    function useThirdSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(2, %a);
        } else
            Parent::useThirdSlot(%a);
    }
    function useFourthSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(3, %a);
        } else
            Parent::useFourthSlot(%a);
    }
    function useFifthSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(4, %a);
        } else
            Parent::useFifthSlot(%a);
    }
    function useSixthSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(5, %a);
        } else
            Parent::useSixthSlot(%a);
    }
    function useSeventhSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(6, %a);
        } else
            Parent::useSevenSlot(%a);
    }
    function useEighthSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(7, %a);
        } else
            Parent::useEighthSlot(%a);
    }
    function useNinthSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(8, %a);
        } else
            Parent::useNinthSlot(%a);
    }
    function useTenthSlot(%a) {
        if($IsInWeaponMode) {
            swapToolSpotFor(9, %a);
        } else
            Parent::useTenthSlot(%a);
    }
    function useLight(%a) {
        if(%a == 1 && $IsUsingSlot1) {
            $IsInWeaponMode = !$IsInWeaponMode;
            if($IsInWeaponMode)
                WeaponSwap::SendMessage("Enabled Weapons Mode");
            else
                WeaponSwap::SendMessage("Disabled Weapons Mode");
            Parent::useBricks(0);
            Parent::useFirstBricks(0);
        } else
            Parent::useLight(%a);
    }
    function WeaponSwap::SendMessage(%a) {
        clientCmdChatMessage(0, 0, 0, "<color:89gad3>" @ %a);
    }
};activatePackage(WeaponSwap);