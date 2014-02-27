// **********************************************************************
/*
BFW Error handling objects
Author: Chad Crume
Requires:
	<script src="/BFWglobal/js/BFW_LogError.js" type="text/javascript" language="Javascript"></script>
*/


BFW_Errors = new BFW_ErrorsObj();

function BFW_ErrorsObj () {
	this.defaultTo = 'chad';
	this.errs = new Array();
	this.lastLoggedWorstSeverity = 0;
	this.lastLoggedWorstSeverityName = '';
	this.lastLoggedMsg = '';
	this.add = function ( e, severity ) {
		if (!severity || isNaN(severity) ) severity = 0;
		this.errs.push( new BFW_ErrorObj( e.name, e.message, severity ) );
	}
	this.LogErrors = function () {
		var errorsToLog = false;
		var msg = '';
		var worstSeverity = 0;
		var worstSeverityName = '';
		var ix = 0;
		for (i=0; i<BFW_Errors.errs.length; i++) {
			if (!BFW_Errors.errs[i].logged) {
				ix++;
				if (ix>1) msg += '\n\n';
				msg +=  'Error '+ ix +' :: '+ BFW_Errors.errs[i].name +' --- '+ BFW_Errors.errs[i].message
				if (BFW_Errors.errs[i].severity) msg += ' --- severity: '+ BFW_Errors.errs[i].severity;
				if (BFW_Errors.errs[i].lineNumber) msg += ' --- line: '+ BFW_Errors.errs[i].lineNumber;
				if (BFW_Errors.errs[i].stack) msg += ' --- stack: '+ BFW_Errors.errs[i].stack;
				BFW_Errors.errs[i].logged = true;
				errorsToLog = true;
				if (BFW_Errors.errs[i].severity > worstSeverity) {
					worstSeverity = BFW_Errors.errs[i].severity;
					worstSeverityName = BFW_Errors.errs[i].name
				}
			}
		}
		if (errorsToLog) {
			BFW_LogError (this.defaultTo,window.location.href,worstSeverity+'-'+worstSeverityName,msg)
//			alert(msg);
			this.lastLoggedWorstSeverity = worstSeverity;
			this.lastLoggedWorstSeverityName = worstSeverityName;
			this.lastLoggedMsg = msg;
		}
		return errorsToLog;
	}
}

function BFW_ErrorObj (name, message, severity) {
	this.name = name;
	this.message = message;
	this.severity = severity;
	this.logged = false;
}


