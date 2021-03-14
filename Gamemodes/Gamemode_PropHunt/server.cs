// 2021

function getPlayerPos(%pl, %brickSize) {
    %pos = %pl.position;
    %pos = getWords(%pos,0,1) SPC (getWord(%pos,2)+%brickSize);
    return %pos;
}

function ServerCmdBecomeProp(%cl) {
    %ctrl = %cl.getControlObject();
    %hit = containerRayCast(%ctrl.getEyePoint(), vectorAdd(%ctrl.getEyePoint(), VectorScale(%ctrl.getEyeVector(), 500)),
				$TypeMasks::ALL & ~$TypeMasks::PhysicalZoneObjectType, %ctrl);
    
    if(!isObject(%cl.player)) {
        PropHunt_Msg(%cl, "\c0You must be spawned to do this!");
        return;
    }
    if(%hit == 0 || %hit.dataBlock $= "") {
        PropHunt_Msg(%cl, "\c0You must aim at a brick you want to become!");
        return;
    }
    %db = %hit.dataBlock;
    if(%db.brickFile $= "") {
        PropHunt_Msg(%cl, "\c0The object you are aiming at is not a brick!");
        return;
    }
    if(!%hit.isRendering()) {
        propHunt_Msg(%cl, "\c0You cannot become an un-rendered brick!");
        return;
    }
    %cl.printID = %hit.printID;
    %cl.brickColor = %hit.colorID;
    %cl.fxColor = %hit.colorFxID;
    %cl.brickCollision = %hit.isColliding();
    %cl.brickRayCast = %hit.isRayCasting();

    %pl = %cl.player;
    %pl.oldName = %cl.player.getShapeName();
    %pl.setShapeName("", 8564862);
    %pl.hideNode("ALL");
    PropHunt_BecomeProp(%cl, %db);
}
function PropHunt_BecomeProp(%cl, %dataBlock) {
    if(%cl.isBrick) {
        propHunt_Msg(%cl, "\c6Your current prop was removed");
        propHunt_Msg(%cl, "\c6Use \c3/BecomeProp \c6again to become a prop");
        PropHunt_ReleaseProp(%cl);
        return;
    }
    if(isEventPending(%cl.createBrickEvent))
        cancel(%cl.createBrickEvent);
    if(%cl.lastBrick !$= "" && %cl.lastBrick != 0)
        %cl.lastBrick.delete();
    createBrick(%cl, %dataBlock);
}

function PropHunt_AngToRotation(%pos, %angId) {
    %trans = %pos;
    if(%angId == 0)
        %trans = %trans SPC " 1 0 0 0";
    else if(%angId == 1)
        %trans = %trans SPC " 0 0 1" SPC $piOver2;
    else if(%angId == 2)
        %trans = %trans SPC " 0 0 1" SPC $pi;
    else if(%angId == 3)
        %trans = %trans SPC " 0 0 -1" SPC $piOver2;
    return %trans;
}

function PropHunt_BottomPrint(%cl) {
    %b = %cl.lastBrick;
    %bN = %b.dataBlock.uiName;
    %angId = %b.angleID;
    %plant = %b.isPlanted;
    %plantT = (%plant ? "\c2Yes" : "\c0No");
    %line = " \c7| ";
    commandToClient(%cl, 'BottomPrint', 
        "<just:center>"@%line SPC
            "\c6Current Brick: \c3"@%bN @" \c7| " NL %line SPC
            "\c6Rotation: \c3"@%angID@" \c7| " NL %line SPC
            "\c6Planted: "@%plantT SPC
        %line,
        1, 1);
}

function PropHunt_ReleaseProp(%cl, %old) {
    if(isEventPending(%cl.createBrickEvent))
        cancel(%cl.createBrickEvent);
    if(%cl.isBrick) {
        %b = %cl.lastBrick;
        if(%old)
            %b = %old;
        %b.isPlanted = false;
        if(isObject(%b))
            %b.delete();
        %cl.lastBrick = 0;
        %cl.isBrick = false;
        %cl.camera.setFlyMode();
        if(isObject(%cl.player)) {
            %pl = %cl.player;
            %pl.position = %b.position;
            %pl.unHideNode("HeadSkin");
            %cl.setControlObject(%pl);
            %pl.setShapeName(%pl.oldName, 8564862);
            %cl.applyBodyParts("ALL");
        }
    }
}

function PropHunt_PlaceSelf(%cl, %optionalTrig) {
    %b = %cl.lastBrick;
    %pl = %cl.player;
    if(!isObject(%b))
        return;
    if(%b.isPlanted) {
        %cl.isBrick = false;
        %pl.position = %pl.oldPosition;
        %cl.setControlObject(%pl);
        createBrick(%cl, %b.dataBlock);
        propHunt_Msg(%cl, "\c1You deleted your own placed brick");
    } else {
        %code = %b.plant();
        if(%code == 0) {
            %b.isPlanted = true;
            %b.setColliding(true);
            %b.setRayCasting(%cl.brickRayCast);
            %b.setColliding(%cl.brickCollision);
            //%b.setRendering(%cl.brickRender);
            %temp = new StaticShape("") {
                position = vectorAdd(%b.position, "0 0 1");
                scale = "0.001 0.001 0.001";
                dataBlock = dummyPlayer;
            }; missioncleanup.add(%temp);
            %db = %b.dataBlock;

            %bSX = %db.brickSizeX;
            %bSY = %db.brickSizeY;
            %bSN = 0;
            if(%bSX < 6 && %bSY < 6)
                %bSN = (%bSX > %bSY) ? (%bSX * 1.25) : (%bSY * 1.25);
            else
                %bSN = (%bSX > %bSY) ? (%bSX / 1.50) : (%bSY / 1.50);
            
            %cl.camera.setOrbitMode(%temp, %b.getTransform(), 1, %bSN, %bSN);
            %cl.setControlObject(%cl.camera);
            %pl.oldPosition = %pl.position;
            %pl.position = "-5 -5 -50";
            propHunt_Msg(%cl, "\c2Your brick was planted where you are standing");
            %temp.delete();
        } else {
            %err = PropHunt_GetPlantError(%code);
            propHunt_Msg(%cl, "\c0A brick cannot be placed here: " @ %err);
        }
    }
}

function PropHunt_GetPlantError(%code) {
    switch(%code) {
        case 1:
            return "Overlap";
        case 2:
            return "Float";
        case 3:
            return "Stuck";
        case 4:
            return "Unstable";
        case 5:
            return "Buried";
        case 6:
            return "Forbidden";
    }
}

function PropHunt_Msg(%cl, %msg) {
    %pre = "\c7[\c1PropHunt\c7]\c6: ";
    %msg = %pre @ %msg;
    messageClient(%cl, '', %msg);
}

function deleteBrick(%cl, %b) {
    if(isEventPending(%cl.createBrickEvent))
        cancel(%cl.createBrickEvent);
    %pl = %cl.player;
    if(isObject(%b) && %b.isPlanted) {
        PropHunt_BottomPrint(%cl);
        %pl.position = "-5 -5 -50";
        %pl.setVelocity("0 0 0");
        %cl.createBrickEvent = schedule(200, 0, deleteBrick, %cl, %b);
        return;
    }
    %db = %b.dataBlock;
    if(isObject(%b) && %pl.lastPosition $= getPlayerPos(%pl, %db.brickSizeZ/10)) {
        PropHunt_BottomPrint(%cl);
        %cl.createBrickEvent = schedule(200, 0, deleteBrick, %cl, %b);
        return;
    }
    %cl.createBrickEvent = schedule(100, 0, createBrick, %cl, %b.dataBlock);
    if(isObject(%b))
        %b.delete();
}

function createBrick(%cl, %db) {
    if(%cl.isBrick && %cl.lastBrick.isPlanted)
        return;
    if(isEventPending(%cl.createBrickEvent))
        cancel(%cl.createBrickEvent);
    if(%db.brickFile $= "")
        return;
    if(%cl.brickAngle < 0 || %cl.brickAngle $= "")
        %cl.brickAngle = 0;
    %pl = %cl.player;
    %brickSize = %db.brickSizeZ / 10;
    %pos = getPlayerPos(%pl, %brickSize);
    %brick = new fxDTSBrick("") {
        angleID = %cl.brickAngle;
        colorId = %cl.brickColor;
        colorFxID = %cl.fxColor;
        printID = %cl.printID;
        dataBlock = %db;
        position = %pos;
        isPlanted = false;
        isBasePlate = false;
    };
    %trans = PropHunt_AngToRotation(%pos, %cl.brickAngle);
    %brick.setTransform(%trans);


    if(isObject(%cl.lastBrick))
        %cl.lastBrick.delete();
    %cl.isBrick = true;
    %cl.lastBrick = %brick;
    %cl.plantTimer = 0;

    %newScale = %brickSize SPC %brickSize SPC %brickSize;
    //if(%pl.getScale() !$= %newScale) {
        //%pl.setScale(%brickSize SPC %brickSize SPC %brickSize);
        //propHunt_Msg(%cl, "\c6You were scaled down to \c1x"@%brickSize);
    //}
    %pl.lastPosition = %pos;
    PropHunt_BottomPrint(%cl);
    %cl.createBrickEvent = schedule(200, 0, deleteBrick, %cl, %brick);
}

for(%i=0;%i < clientGroup.getCount();%i++) {
    %cl = clientGroup.getObject(%i);
    if(%cl.isBrick)
        PropHunt_ReleaseProp(%cl);
}

package PropHunt {
    function ServerCmdDropCameraAtPlayer(%cl) {
        if(!%cl.isAdmin)
            return;
        if(%cl.isBrick && %cl.lastBrick.isPlanted) {
            PropHunt_Msg(%cl, "\c0You were forced out of being a prop for orbing");
            PropHunt_ReleaseProp(%cl);
        }
        Parent::ServerCmdDropCameraAtPlayer(%cl);
    }
    function ServerCmdPlantBrick(%cl) {
        if(%cl.isBrick) {
            PropHunt_PlaceSelf(%cl);
            return;
        }
        Parent::ServerCmdPlantBrick(%cl);
    }
    function ServerCmdRotateBrick(%cl, %rot) {
        if(%cl.isBrick) {
            %cl.brickAngle += %rot;
            if(%cl.brickAngle < 0)
                %cl.brickAngle = 3;
            if(%cl.brickAngle > 3)
                %cl.brickAngle = 0;
            //if(!%cl.lastBrick.isPlanted)
            //    createBrick(%cl, %cl.lastBrick.dataBlock);
            return;
        }
        parent::ServerCmdRotateBrick(%cl, %rot);
    }
    function ServerCmdSuicide(%cl) {
        PropHunt_ReleaseProp(%cl);
        parent::ServerCmdSuicide(%cl);
    }
    function ServerCmdLight(%cl) {
        if(%cl.isBrick) {
            PropHunt_PlaceSelf(%cl);
            return;
        }
        parent::ServerCmdLight(%cl);
    }
}; activatePackage(PropHunt);