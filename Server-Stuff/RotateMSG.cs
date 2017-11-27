if($Pref::Server::RotateMSGPath $= "")
	$Pref::Server::RotateMSGPath = "config/server/RotateMSG.txt";
function RotateMSGCreate(%path) {
	%file = new FileObject();
	%file.openForWrite(%path);
	%file.writeLine("\c6You can store messages in \c5config/server/RotateMSG.txt");
	%file.writeLine("\c6Be sure to check out my other add-ons @ \c5<a:www.github.com/Anthonyrules144>my github</a>\c6!");
	%file.writeLine("TIME = 55000 // 60 seconds");
	%file.close();
	%file.delete();
}

function RotateMSGLoad() {
	if($RotateMSG::Num[0] !$= "")
		for(%i=0;%i < 15;%i++)
			$RotateMSG::Num[%i] = "";
	%path = $Pref::Server::RotateMSGPath;
	if(!isFile(%path))
		return RotateMSGCreate(%path);
	%file = new FileObject();
	%file.openforRead(%path);
	while(!%file.isEOF()) {
		for(%i=0;%i < 16;%i++) { // 16 messages max.
			%a = %file.readLine();
			if(%a $= "" && getWord(%a, 0) !$= "TIME")
				continue;
			if(getWord(%a, 0) $= "TIME") {
				$RotateMSG::Time = getWord(%a, 2);
				continue;
			}
			$RotateMSG::Num[%i] = %a;
		}
	}
	%file.close();
	%file.delete();
	if($RotateMSG::Time $= "")
		$RotateMSG::Time = 55000;
}

function RotateMSG(%toggle) {
	RotateMSGLoad();
	cancel($RotateMSG);
	if(%toggle == 1)
		return;
	if($RotateMSG::Num[$RotateMSG::Number] $= "" || $RotateMSG::Number < 0)
		$RotateMSG::Number = 0;
	announce($RotateMSG::Num[$RotateMSG::Number]);
	$RotateMSG::Number++;
	if($RotateMSG::Time !$= "")
		$RotateMSG = schedule($RotateMSG::Time, 0, RotateMSG);
	else
		$RotateMSG = schedule(55000 * 3, 0, RotateMSG);
}
if(!isEventPending($RotateMSG))
	RotateMSG();
