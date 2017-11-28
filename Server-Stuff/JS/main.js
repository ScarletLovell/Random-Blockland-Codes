// 2016

// I don't remember why I made this, but it's here now.4

var clientGroup = { ids: 0, talk: false } || function() { return true; };
var sClientGroup = { };
function findClientByName(name) {
    return sClientGroup[ts_eval("return findClientByName(\"" + name + "\");")];
}
function fcbn(name) {
    return findClientByName(name);
}
function JS_makePlayer(player) {
    var ids = clientGroup["ids"];
    player = player.split(" ");
    clientGroup[ids] = {
        name: player[0],
        client: { id: player[1], player: player[2],
            setScore: function(score) { ts_eval("return " + player[1] + ".setScore(\"" + score + "\");"); },
            getPing: function() { ts_eval("return " + player[1] + ".getPing();"); }
        },
    };
    for(i=0;i < 250;i++) {
        var field = ts_eval("return getField(" + player[1] + ".getTaggedField(" + i + "), 0) SPC getField(" + player[1] + ".getTaggedField(" + i + "), 1);");
        if(field !== undefined && field !== " ") {
            field = field.split(" ");
            clientGroup[ids]["client"][field[0]] = field[1];
        }
        else
            break;
    }
    if(player[2] != -1) {
        clientGroup[ids]["player"] = { id: player[2], client: player[1],
            position: ts_eval("return " + player[2] + ".position;"),
            dataBlock: {
                getID: function() { ts_eval("return " + player[2] + ".getDatablock().getID();"); },
            },
        }
        var regularFunctions = "setTransform addVelocity setVelocity getVelocity";
        for(i=0;i < (regularFunctions.split(" ").length);i++) {
            clientGroup[ids]["player"][regularFunctions[i]] = 1; //function(vector3) {
                //ts_eval(player[2] + "." + regularFunctions[i] + "(\"" + vector3 + "\");");
            //}
        }
        for(i=0;i < 250;i++) {
            var field = ts_eval("return getField(" + player[2] + ".getDatablock().getTaggedField(" + i + "), 0) SPC getField(" + player[2] + ".getDatablock().getTaggedField(" + i + "), 1);");
            if(field !== undefined && field !== " ") {
                field = field.split(" ");
                clientGroup[ids]["player"]["dataBlock"][field[0]] = field[1];
            }
            else
                break;
        }
    }
    else
        clientGroup[ids]["player"] = null;
    sClientGroup[player[0]] = clientGroup[ids];
    sClientGroup[player[1]] = clientGroup[ids];
    clientGroup["ids"] = clientGroup["ids"] + 1;
    return true;
}


var console = console || function() {};
console.log = function (msg) {
    return print(msg);
};
console.talk = function (msg) {
    return ts_eval("talk(\"" + msg + "\");");
};
console.error = function (msg) {
    return print("Error received: " + msg);
}
