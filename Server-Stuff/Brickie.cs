// 2015

if(!isObject(Shape_Col1))
	datablock StaticShapeData(Shape_Col1)
	{
	        shapeFile = "config/Cube_Col1.dts";
	};
if(isObject($Brickie))
	$Brickie.delete();
if($brickie::rotater !$= "")
	$brickie::rotater = "";

$brickie::firstPos = "";

function createBrickie()
{
	%rad = getRandom(10, 70);
	%color = getRandom() SPC getRandom() SPC getRandom() SPC 1;
	//if(%rad == $brickie::last)
	//	return createBrickie();

	$brickie::location["x1"] = $brickie::rotater + 3;
	$brickie::location["x2"] = $brickie::rotator - 3;

	$brickie::last = %rad;

	$Brickie = new StaticShape()
	{
			dataBlock = Shape_Col1;
			position = "0 0 355";
			scale = %rad SPC %rad SPC %rad;
	};
	$Brickie.setTransform("0 0 355" SPC eulerToAxis(getRandom(0,360) SPC getRandom(0,360) SPC getRandom(0,360)));
	$Brickie.setNodeColor("ALL",%color);
	if($brickie::rotater $= "")
		$brickie::rotater = $Brickie.getPosition();
}

createBrickie();

function randomize(%var)
{
	if(!isObject($Brickie))
		return cancel($brickieRandomize);
	if(%var == 1)
		return cancel($brickieRandomize);


	if(isObject($Brickie))
		$Brickie.delete();
	createBrickie();
	//_brickie.setColor(getRandom(0, 10));
	//_brickie.setColorFX(getRandom(0,8));
	//_brickie.setShapeFX(getRandom(0, 2));


	cancel($brickieRandomize);
	$brickieRandomize = schedule(250, 0, randomize);
}

randomize();
