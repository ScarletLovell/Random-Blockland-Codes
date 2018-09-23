function makeAdmin(%cl, %level) {
    if(%level >= 1)
        %cl.isAdmin = 1;
    if(%level == 2)
        %cl.isSuperAdmin = 1;
    commandToClient(%cl, 'setAdminLevel', %level);
}