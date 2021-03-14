function Eval_IsApplicable(%cl, %msg) {
	if(!%cl.eval)
		return false;
	%first = getSubStr(%msg, 0, 1);
	if(%first $= "@" || %first $= "$")
		return true;
	return false;
}

function Eval_Do(%cl, %msg) {
	%time = getRealTime();
	%client = %cl;
	%mg = %minigame = %cl.minigame;
	%bg = %brickgroup = brickgroup @ "_" @ %cl.bl_id;
	if(isObject(%ctrl = %control = %cl.getControlObject()))
		%hit = %ray = %look = containerRayCast(%ctrl.getEyePoint(), vectorAdd(%ctrl.getEyePoint(), VectorScale(%ctrl.getEyeVector(), 500)),
			$TypeMasks::ALL & ~$TypeMasks::PhysicalZoneObjectType, %ctrl);
	if(isObject(%cl.camera))
		%cam = %camera = %cl.camera;
	if(isObject(%cl.player)) {
		%pl = %player = %cl.player;
		%vel = %velocity = %cl.player.getVelocity();
		%mount = %veh = %vehicle = %pl.getObjectMount();
		%pos = %pl.getPosition();
		%db = %datablock = %pl.getDatablock();
	}

	%trim = trim(%msg);
	%last = getSubStr(%trim, strlen(%trim) - 1, 1);
	%result = "";
	if(%last !$= ";" && %last !$= "}")
		%result = eval("return " @ %msg @ ";");
	else
		%result = eval(%msg);
	return %result;
}

function Eval_Query(%cl, %msg) {
	%public = %cl.evalPublic;
	%cLPath = "config/server/eval.txt";
    if(!isObject(EvalLog)) {
		new ConsoleLogger(EvalLog, %cLPath);
        $ConsoleLoggerCount++;
        EvalLog.level = 0;
	} else
    	EvalLog.attach();
	%time = getRealTime();
	%result = Eval_Do(%cl, %msg);
	%newTime = getRealTime() - %time;
	if(%newTime < 0)
		%newTime = 0;
	EvalLog.detach();

	%file = new FileObject();
	%file.openforRead(%cLPath);
	if(%result $= "") {
		%lastLine = "";
        while(!%file.isEoF()) {
            %line = %file.readLine();
			if(%line $= %lastLine || trim(%line) $= "")
				continue;
			%lastLine = %line;
			announce("\c8[\c7TS\c8] \c2"@%line);
		}
	} else {
		announce("\c8[\c7TS\c8] \c7RESULT: \c6"@%result);
	}
	%file.close();
	%file.delete();

	%name = %cl.name;
	if(%public) {
		announce("\c7"@%name@" (\c2"@%newTime@"\c7ms)\c6: "@%msg);
	}
}

package ScriptEval {
	function serverCmdMessageSent(%client, %msg) {
		if(Eval_IsApplicable(%client, %msg)) {
			Eval_Query(%client, getSubStr(%msg, 1, strLen(%msg)-1));
			return;
		}
        return parent::serverCmdMessageSent(%client, %msg);
    }
};
activatePackage(ScriptEval);