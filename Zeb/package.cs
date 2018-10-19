package Zeb {
    function clientCmdTrustInvite(%name, %bl_id, %level) {
		parent::clientCmdTrustInvite(%name, %bl_id, %level);
		trustInviteGui.ClickAccept();
	}
    function clientCmdMessageBoxOK(%title, %message) {
		parent::clientCmdMessageBoxOK(%title, %message);
		messageCallback(MessageBoxOKDlg, MessageBoxOKDlg.callback);
	}
    function clientCmdMessageBoxYesNo(%title, %message, %okServerCmd, %cancelServerCmd) {
		parent::clientCmdMessageBoxYesNo(%title, %message, %okServerCmd, %cancelServerCmd);
		messageCallback(MessageBoxYesNoDlg, MessageBoxYesNoDlg.yesCallback);
	}
	function clientCmdMessageBoxOKCancel(%title, %message, %okServerCmd, %cancelServerCmd) {
		parent::clientCmdMessageBoxOKCancel(%title, %message, %okServerCmd, %cancelServerCmd);
		messageCallback(MessageBoxOKCancelDlg, MessageBoxOKCancelDlg.callback);
	}
    function GlassNotification::onAdd(%this, %a, %b) {
        return;
        parent::onAdd(%this, %a, %b);
	}
};
activatePackage(zeb);