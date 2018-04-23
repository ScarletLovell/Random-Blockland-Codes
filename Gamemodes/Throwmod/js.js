function FP_doSplitCheck(str) {
    var split = str.split("|");
    return split[0] + "^" + split[1];
}

function checkStringOccurences(str, oc) {
    var talk = ts.func("talk");
    var count = "this is foo bar".split("o").length - 1;
    talk(count)
    return count;
}
