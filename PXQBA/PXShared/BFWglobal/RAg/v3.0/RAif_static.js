(function($) {
//$(document).ready(function () {
    if (!$) 
    {
        throw new Error('jQuery required for RAif');
//        return;
    }
    if (!RA_CtrlWin) 
    {
//        throw new Error('RA_CtrlWin not set, defaulting');
        RA_CtrlWin = window.self;
    }
    if (!RA_CtrlWin.RA) 
    {
        throw new Error('RA_CtrlWin.RA required for RAif.app');
//        return;
    }
    if (!RA_CtrlWin.RA.RAif) 
    {
        throw new Error('RA_CtrlWin.RA.RAif required for RAif.app');
//        return;
    }

RA_CtrlWin.RA.RAif.app = {};

RA_CtrlWin.RA.RAif.DestroyApp = function () {
    RA_CtrlWin.RA.RAif.app = {};
}

RA_CtrlWin.RA.RAif.CreateApp = function () {
RA_CtrlWin.RA.RAif.app = {};

RA_CtrlWin.RA.RAif.app.useRAWS3 = true;
RA_CtrlWin.RA.RAif.app.ChangingEmail = false;
RA_CtrlWin.RA.RAif.app.ChangingPw = false;
RA_CtrlWin.RA.RAif.app.CheckOut_win = null;
RA_CtrlWin.RA.RAif.app.RAXSReInit_holdNeedsRAXS = false

//  ********************************************************************************
// Resize
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.Resize = function () {
//alert(1);
	RA_CtrlWin.RA.RAif.Resize();
//alert(3);
}

//  ********************************************************************************
// Init
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.Init = function () {
//if (RA_CtrlWin.RA.dev_check()) alert('go: '+ RA_CtrlWin.RA.RAif.state.current() );
	if (RA_CtrlWin.RA.CurrentSite == null) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('nosite');
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else if (BFW_QStr['clear']=='true') {
//alert('clear = true');
//alert( 'uid = '+ RAreturn_uid );
//		RA_CtrlWin.RA.InitCurrentUser( RA_CtrlWin.RA.RAif.app.Init_2 );
		RA_CtrlWin.RA.RAif.app.Init_2();
//alert('user inited');
	} else {
		RA_CtrlWin.RA.RAif.app.Init_2();
	}
}

RA_CtrlWin.RA.RAif.app.Init_2 = function () {
//alert('RA_CtrlWin.RA.RAif.app.Init_2');

	if (RA_CtrlWin.RA.RAif.state.current() == 'quizclassprompt') {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('quizprompt');
	}

	switch (RA_CtrlWin.RA.RAif.state.current()) {
		case 'quizprompt' :
//INSTRUCTOR EMAIL VERSION
			if (RA_CtrlWin.RA.CurrentUser != null) {
				RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] = RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail;
			} else {
				RA_CtrlWin.RA.RAif.state.push('checkemail');
			}
			RA_CtrlWin.RA.RAif.app.DrawPage();
			if (RA_CtrlWin.RA.RAif.updated && RA_CtrlWin.RA.CurrentUser.ClassPrompt==0) {
				setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
			}
		break;
		case 'classprompt' :
//CLASS PROMPT VERSION
			if (RA_CtrlWin.RA.CurrentUser != null) {
				var tmpOneClass = null;
				var UserClassesCt = 0;
				for (var i in RA_CtrlWin.RA.CurrentUser.ClassLogins) {if (RA_CtrlWin.RA.CurrentUser.ClassLogins.hasOwnProperty(i)) {
					UserClassesCt ++;
					if (UserClassesCt==1) tmpOneClass = RA_CtrlWin.RA.CurrentUser.ClassLogins[i].Class;
				}}
				if (RA_CtrlWin.RA.RAif.updated && RA_CtrlWin.RA.CurrentUser.ClassPrompt==0) {
					RA_CtrlWin.RA.RAif.app.DrawPage();
					$('#RA_quizclassprompt_1').hide('','');
					$('#RA_quizclassprompt_2').hide('','');
					$('#RA_quizclassprompt_3').hide('','');
					$('#RA_quizclassprompt_4').hide('','');
					$('#RA_quizclassprompt_5').show('','');
					setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
					RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
				} else if (UserClassesCt>1) {
					RA_CtrlWin.RA.RAif.app.DrawPage();
					$('#RA_quizclassprompt_1').hide('','');
					$('#RA_quizclassprompt_2').hide('','');
					$('#RA_quizclassprompt_3').show('','');
					$('#RA_quizclassprompt_4').hide('','');
				} else if (UserClassesCt==1) {
					if (RA_CtrlWin.RA.CurrentUser.ClassUsing == null) {
						RA_CtrlWin.RA.CurrentUser.ClassUsing = tmpOneClass;
					}
					RA_CtrlWin.RA.RAif.app.DrawPage();
					$('#RA_quizclassprompt_1').hide('','');
					$('#RA_quizclassprompt_2').hide('','');
					$('#RA_quizclassprompt_3').show('','');
					$('#RA_quizclassprompt_4').hide('','');
				} else {
					RA_CtrlWin.RA.RAif.app.DrawPage();
					$('#RA_quizclassprompt_1').show('','');
					$('#RA_quizclassprompt_2').hide('','');
					$('#RA_quizclassprompt_3').hide('','');
					$('#RA_quizclassprompt_4').hide('','');
				}
				RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();

			} else {
				RA_CtrlWin.RA.RAif.state.push('checkemail');
				RA_CtrlWin.RA.RAif.app.DrawPage();
			}
		break;
		case 'quizjoinedclass' :
			for (var i in RA_CtrlWin.RA.CurrentUser.ClassLogins) {if (RA_CtrlWin.RA.CurrentUser.ClassLogins.hasOwnProperty(i)) {
				if (RA_CtrlWin.RA.RAif.vars['UsingClassID']==RA_CtrlWin.RA.CurrentUser.ClassLogins[i].Class.ID) RA_CtrlWin.RA.CurrentUser.ClassUsing = RA_CtrlWin.RA.CurrentUser.ClassLogins[i].Class;
			}}
			RA_CtrlWin.RA.RAif.updated = true;
			RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_quizclassprompt_5').show('','');
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
			setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
		break;
		case 'dologin' :
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('checkemail');
//			RA_CtrlWin.RA.RAif.app.DrawPage();
			RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin()', 5);

		break;
		case 'dologout' :
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('logout');
			RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.RAXSLogout()', 5);
		break;
		case 'docheckemail' :
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('checkemail');
//			RA_CtrlWin.RA.RAif.app.DrawPage();
			RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.CheckEmail()', 5);
		break;
		case 'updateprofile' :
			RA_CtrlWin.RA.RAif.vars['sUserEmail'] = RA_CtrlWin.RA.CurrentUser.Email;
			RA_CtrlWin.RA.RAif.vars['sUserFirstName'] = RA_CtrlWin.RA.CurrentUser.FName;
			RA_CtrlWin.RA.RAif.vars['sUserLastName'] = RA_CtrlWin.RA.CurrentUser.LName;
			RA_CtrlWin.RA.RAif.app.DrawPage();
		break;
		case 'docheckout' :
			RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.CartCheckItems()', 5);
		break;
		case 'doassignusercodes' :
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('assignusercodes');
			RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.AssignUserCodes()', 5);
		break;
		default :
//alert(1);
			RA_CtrlWin.RA.RAif.app.DrawPage();
//alert(2);
	}
//	RA_hidden_if = $('#RA_hidden')[0];
}

//  ********************************************************************************
// Login
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goLogin = function () {
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Email')[0].value);
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.vars['Email'].toLowerCase();
	RA_CtrlWin.RA.RAif.vars['Password'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Password')[0].value);
	RA_CtrlWin.RA.RAif.vars['RememberMe'] = 0;
	if (!RA_CtrlWin.RA.RAif.app.IsValidEmail(RA_CtrlWin.RA.RAif.vars['Email'])) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_login').show('','');
		$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
		return;
	}
/*
	if ($('#RA_RememberMe')[0]) {
		if ($('#RA_RememberMe')[0].checked) {
			RA_CtrlWin.RA.RAif.vars['RememberMe'] = 1;
		}
	}
*/
	RA_CtrlWin.RA.RAif.app.Login();
}

RA_CtrlWin.RA.RAif.app.Login = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('loggingin');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	if (RA_CtrlWin.RA.RavellAuthentication) {
                //Pasha commented 
		//RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Login_0_go, Ravell_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
		RA_CtrlWin.RA.RAif.app.RavellLogin_continue(RA_CtrlWin.RA.RAif.vars['Email'], RA_CtrlWin.RA.RAif.vars['Password']);
	} else if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
//		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Login_0_go, RAWS3_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Login_0_go, RAWS1_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Login_0_go, RAWS1_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	}
}
RA_CtrlWin.RA.RAif.app.Login_0_go = function () {
	RA_CtrlWin.RA.RAif.app.Login_0();
}
RA_CtrlWin.RA.RAif.app.Login_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RA.RavellAuthentication && RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.app.RavellLogin_continue(RA_CtrlWin.RA.RAif.vars['Email'], RA_CtrlWin.RA.RAif.vars['Password']);
	}
	else if ( RA_CtrlWin.RAWS_error=='' && RA_CtrlWin.RAWS_iUserID>0 ) {
		RA_CtrlWin.RA.RAif.app.RAXSLogin_continue(RA_CtrlWin.RAWS_iUserID, 'loggedin');
	} else if (RA_CtrlWin.RA.RavellAuthentication) {
		if ( RA_CtrlWin.RAWS_error.indexOf('Login failed, please try again.') > -1 ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error + ' Your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else {
	//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	//prompt( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- ', RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		}
	} else {
		if ( RA_CtrlWin.RAWS_error.indexOf('Invalid email address format.')>-1 ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else if ( RA_CtrlWin.RAWS_error=='That e-mail address is registered with a different password. Please enter valid password.' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The password you entered is incorrect. Please try again. If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
		} else if ( RA_CtrlWin.RAWS_error=='That password is incorrect.' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The password you entered is incorrect. Please try again. If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
		} else if ( RA_CtrlWin.RAWS_error=='E-mail address not found. Check spelling and make sure you are using the e-mail address with which you registered. ' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('register');
			RA_CtrlWin.RA.RAif.vars['Error'] = '';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
	//		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
	//		RA_CtrlWin.RA.RAif.app.DrawPage();
	//		$('#RA_formError_login').show('','');
	//		$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
	//		RA_CtrlWin.RA.RAif.app.Resize();
		} else {
	//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	//prompt( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- ', RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
		}
	}
}

//  ********************************************************************************
// Logout
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goLogout = function () {
	RA_CtrlWin.RA.RAif.app.RAXSLogout();
}

//  ********************************************************************************
// InstEmail
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goEditInstEmail = function () {
	RA_CtrlWin.RA.RAif.state.push('instemailprompt');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.goInstEmailPrompt = function () {
	var tmpE = $('#RA_sInstructorEmail')[0].value;
	if (!RA_CtrlWin.RA.RAif.app.IsValidEmail(tmpE)) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, an e-mail address cannot contain spaces or special characters such as apostrophes. Please check your instructor\'s e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_instemailprompt').show('','');
		$('#RA_formError_instemailprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
		return;
	}
	RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sInstructorEmail')[0].value);
	RA_CtrlWin.RA.RAif.app.CheckInstEmail();
}

RA_CtrlWin.RA.RAif.app.CheckInstEmail = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('instemailprompted');
	RA_CtrlWin.RA.RAif.state.push('checkinginstemail');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckInstEmail_0_go, RAWS1_GetUsernamePasswordHint, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['sInstructorEmail'], 'sBaseUrl=' ) );
}

RA_CtrlWin.RA.RAif.app.CheckInstEmail_0_go = function () {
	RA_CtrlWin.RA.RAif.app.CheckInstEmail_0();
}

RA_CtrlWin.RA.RAif.app.CheckInstEmail_0 = function () {
//prompt( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.app.QuizPrompt_1();
	} else if (RA_CtrlWin.RAWS_error=='User not found.') {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('quizpromptBAD');
		RA_CtrlWin.RA.RAif.vars['Error'] = '';
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_instemailprompt').show('','');
		$('#RA_formError_instemailprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}

}

//  ********************************************************************************
// UpdateProfile
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goUpdateProfile = function () {

	$('#RA_formError_updateprofile').hide('','');
	$('#RA_sUserFirstName_error').hide('','');
	$('#RA_sUserLastName_error').hide('','');
	$('#RA_sUserEmail_error').hide('','');
	$('#RA_sUserEmail_confirm_error').hide('','');
	$('#RA_sOldPwd_error').hide('','');
	$('#RA_sNewPwd_error').hide('','');
	$('#RA_sVPwd_error').hide('','');
	if ($('#RA_sUserFirstName')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please enter your first name.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sUserFirstName_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if ($('#RA_sUserLastName')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please enter your last name.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sUserLastName_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.app.ChangingEmail && $('#RA_sUserEmail')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please choose your new e-mail address.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sUserEmail_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.app.ChangingEmail && $('#RA_sUserEmail')[0].value != $('#RA_sUserEmail_confirm')[0].value) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please confirm your new e-mail address.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sUserEmail_confirm_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.app.ChangingPw && $('#RA_sOldPwd')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please confirm your current password.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sOldPwd_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.app.ChangingPw && $('#RA_sNewPwd')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please choose your new password.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sNewPwd_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.app.ChangingPw && $('#RA_sNewPwd')[0].value != $('#RA_sVPwd')[0].value) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please confirm your new password.';
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_updateprofile').show('','');
		$('#RA_sVPwd_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}

	RA_CtrlWin.RA.RAif.vars['iUserID'] = RA_CtrlWin.RA.CurrentUser.ID;
	RA_CtrlWin.RA.RAif.vars['sUserEmail'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sUserEmail')[0].value);
	if (RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID] != null)
	{
		RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] = RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail;
	}
	else
	{
		RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] = '';
	}
	RA_CtrlWin.RA.RAif.vars['sUserFirstName'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sUserFirstName')[0].value);
	RA_CtrlWin.RA.RAif.vars['sUserLastName'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sUserLastName')[0].value);
//	RA_CtrlWin.RA.RAif.vars['sMailPreferences'] = 'TEXT';
//	RA_CtrlWin.RA.RAif.vars['sOptInEmail'] = 0;
	RA_CtrlWin.RA.RAif.vars['sBaseURL'] = RA_CtrlWin.RA.CurrentSite.BaseURL;
	RA_CtrlWin.RA.RAif.vars['sUserName'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sUserEmail')[0].value);
	RA_CtrlWin.RA.RAif.vars['sOldPwd'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sOldPwd')[0].value);
	RA_CtrlWin.RA.RAif.vars['sNewPwd'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sNewPwd')[0].value);
	RA_CtrlWin.RA.RAif.vars['sVPwd'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sVPwd')[0].value);
	var xHint = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sNewPwd')[0].value);
	xHint = xHint.substring(0,1) +' '+ xHint.substring(1,xHint.length-2) +' '+  xHint.substring(xHint.length-2);
	RA_CtrlWin.RA.RAif.vars['sHintPwd'] = xHint;
	try {
		var w = $('#RA_OptInEmail')[0];
		if (w.checked) {
			RA_CtrlWin.RA.RAif.vars['OptInEmail'] = 1;
		} else {
			RA_CtrlWin.RA.RAif.vars['OptInEmail'] = 0;
		}
	} catch(e) {
		RA_CtrlWin.RA.RAif.vars['OptInEmail'] = 0;
	}

	RA_CtrlWin.RA.RAif.app.UpdateProfile();
}

RA_CtrlWin.RA.RAif.app.UpdateProfile = function () {
	RA_CtrlWin.RA.RAif.state.push('updatingprofile');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.UpdateProfile_0_go, RAWS3_UpdateProfile, new Array( 'iUserID='+RA_CtrlWin.RA.RAif.vars['iUserID'], 'sUserEmail='+RA_CtrlWin.RA.RAif.vars['sUserEmail'], 'sInstructorEmail='+RA_CtrlWin.RA.RAif.vars['sInstructorEmail'], 'sUserFirstName='+RA_CtrlWin.RA.RAif.vars['sUserFirstName'], 'sUserLastName='+RA_CtrlWin.RA.RAif.vars['sUserLastName'], 'sBaseURL='+RA_CtrlWin.RA.RAif.vars['sBaseURL'], 'sOptInEmail='+RA_CtrlWin.RA.RAif.vars['OptInEmail'] ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.UpdateProfile_0_go, RAWS1_UpdateProfile, new Array( 'iUserID='+RA_CtrlWin.RA.RAif.vars['iUserID'], 'sUserEmail='+RA_CtrlWin.RA.RAif.vars['sUserEmail'], 'sInstructorEmail='+RA_CtrlWin.RA.RAif.vars['sInstructorEmail'], 'sUserFirstName='+RA_CtrlWin.RA.RAif.vars['sUserFirstName'], 'sUserLastName='+RA_CtrlWin.RA.RAif.vars['sUserLastName'], 'sBaseURL='+RA_CtrlWin.RA.RAif.vars['sBaseURL'] ) );
	}
}
RA_CtrlWin.RA.RAif.app.UpdateProfile_0_go = function () {
	RA_CtrlWin.RA.RAif.app.UpdateProfile_0();
}
RA_CtrlWin.RA.RAif.app.UpdateProfile_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RAWS_error=='' ) {
		if (RA_CtrlWin.RA.RAif.app.ChangingPw) {
//alert(1);
			RA_CtrlWin.RA.RAif.state.push('updatingprofile');
			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.UpdateProfile_1_go, RAWS1_UpdatePassword, new Array( 'iUserID='+RA_CtrlWin.RA.RAif.vars['iUserID'], 'sUserName='+RA_CtrlWin.RA.RAif.vars['sUserName'], 'sOldPwd='+RA_CtrlWin.RA.RAif.vars['sOldPwd'], 'sNewPwd='+RA_CtrlWin.RA.RAif.vars['sNewPwd'], 'sVPwd='+RA_CtrlWin.RA.RAif.vars['sVPwd'], 'sHintPwd='+RA_CtrlWin.RA.RAif.vars['sHintPwd'] ) );
		} else {
//alert(2);
			RA_CtrlWin.RA.RAif.app.RAXSReInit('updatedprofile');
		}
	} else {
//alert(3);
		if ( RA_CtrlWin.RAWS_error == 'This User Email address already exists.') {
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The e-mail address <b>'+ RA_CtrlWin.RA.RAif.vars['sUserEmail'] +'</b> is already registered.';
			RA_CtrlWin.RA.RAif.vars['sUserEmail'] = RA_CtrlWin.RA.CurrentUser.Email;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_updateprofile').show('','');
			$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		} else {
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_updateprofile').show();
			$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
if (RA_CtrlWin.RA.dev_check()) alert($('#RA_formError_updateprofile').html() +' - '+ RA_CtrlWin.RA.RAif.vars['Error']);
//			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		}
	}
}
RA_CtrlWin.RA.RAif.app.UpdateProfile_1_go = function () {
	RA_CtrlWin.RA.RAif.app.UpdateProfile_1();
}
RA_CtrlWin.RA.RAif.app.UpdateProfile_1 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RAWS_error=='' ) {
//alert(4);
		RA_CtrlWin.RA.RAif.app.RAXSReInit('updatedprofile');
	} else {
//alert(5);
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		$('#RA_formError_updateprofile').show('','');
		$('#RA_formError_updateprofile').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.doChangeEmail = function () {
	$('#RA_editLogin').show('','');
	$('#RA_editEmail').show('','');
	$('#RA_showEmail').hide('','');
	RA_CtrlWin.RA.RAif.app.ChangingEmail=true;
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

RA_CtrlWin.RA.RAif.app.doChangePw = function () {
	$('#RA_editLogin').show('','');
	$('#RA_changePw').show('','');
	$('#RA_showPw').hide('','');
	RA_CtrlWin.RA.RAif.app.ChangingPw=true;
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}



//  ********************************************************************************
// Register
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.RegYesLogin = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('checkemaillogin');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.RegNoReg = function () {
	RA_CtrlWin.RA.RAif.vars['RegAlreadyAsked'] = true;
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('checkemail');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.LoginYesReg = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('register');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.LoginNoCheck = function () {
	RA_CtrlWin.RA.RAif.vars['RegAlreadyAsked'] = false;
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('checkemail');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goRegister = function () {
//	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Email')[0].value);
	RA_CtrlWin.RA.RAif.vars['Email_confirm'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Email_confirm')[0].value);
	RA_CtrlWin.RA.RAif.vars['Email_confirm'] = RA_CtrlWin.RA.RAif.vars['Email_confirm'].toLowerCase();
	RA_CtrlWin.RA.RAif.vars['FName'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_FName')[0].value);
	RA_CtrlWin.RA.RAif.vars['LName'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_LName')[0].value);
	RA_CtrlWin.RA.RAif.vars['Password'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Password')[0].value);
	RA_CtrlWin.RA.RAif.vars['Password_confirm'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Password_confirm')[0].value);
	try {
		var w = $('#RA_OptInEmail')[0];
		if (w.checked) {
			RA_CtrlWin.RA.RAif.vars['OptInEmail'] = 1;
		} else {
			RA_CtrlWin.RA.RAif.vars['OptInEmail'] = 0;
		}
	} catch(e) {
		RA_CtrlWin.RA.RAif.vars['OptInEmail'] = 0;
	}

	var xHint = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Password')[0].value);
	xHint = xHint.substring(0,1) +' '+ xHint.substring(1,xHint.length-2) +' '+  xHint.substring(xHint.length-2);
	RA_CtrlWin.RA.RAif.vars['sHintPwd'] = xHint;

	$('#RA_formError_register').hide('','');
	RA_CtrlWin.RA.RAif.vars['Error'] = '';
	$('#RA_FName_error').hide('','');
	$('#RA_LName_error').hide('','');
	$('#RA_Password_error').hide('','');
	$('#RA_Email_confirm_error').hide('','');
	$('#RA_Password_confirm_error').hide('','');
	if (RA_CtrlWin.RA.RAif.vars['FName']=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'All fields are required. Please fill them all in.';
		$('#RA_FName_error').show('','');
	}
	if (RA_CtrlWin.RA.RAif.vars['LName']=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'All fields are required. Please fill them all in.';
		$('#RA_LName_error').show('','');
	}
	if (RA_CtrlWin.RA.RAif.vars['Email_confirm']=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'All fields are required. Please fill them all in.';
		$('#RA_Email_confirm_error').show('','');
	}
	if (RA_CtrlWin.RA.RAif.vars['Password']=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'All fields are required. Please fill them all in.';
		$('#RA_Password_error').show('','');
	}
	if (RA_CtrlWin.RA.RAif.vars['Password_confirm']=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'All fields are required. Please fill them all in.';
		$('#RA_Password_confirm_error').show('','');
	}
	if (RA_CtrlWin.RA.RAif.vars['Error']!='') {
		$('#RA_formError_register').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_register').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.vars['Email']!=RA_CtrlWin.RA.RAif.vars['Email_confirm']) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'The e-mail addresses you entered do not match. Make sure you have spelled it correctly and try again.';
		$('#RA_formError_register').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_register').show('','');
		$('#RA_Email_confirm_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.vars['Password']==RA_CtrlWin.RA.RAif.vars['Email'].substring(0,RA_CtrlWin.RA.RAif.vars['Email'].indexOf('@'))) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your password cannot match your e-mail address. Please choose a more secure password.';
		$('#RA_formError_register').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_register').show('','');
		$('#RA_Password_confirm_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if (RA_CtrlWin.RA.RAif.vars['Password']!=RA_CtrlWin.RA.RAif.vars['Password_confirm']) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'The passwords you entered do not match. Please try again.';
		$('#RA_formError_register').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_register').show('','');
		$('#RA_Password_confirm_error').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	RA_CtrlWin.RA.RAif.app.Register();
}

RA_CtrlWin.RA.RAif.app.Register = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('registering');
	RA_CtrlWin.RA.RAif.app.DrawPage();

	if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Register_0_go, RAWS3_StudentRegistration, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sUserFirstName='+RA_CtrlWin.RA.RAif.vars['FName'], 'sUserLastName='+RA_CtrlWin.RA.RAif.vars['LName'], 'sUserPwd='+RA_CtrlWin.RA.RAif.vars['Password'], 'sVerifyPwd='+RA_CtrlWin.RA.RAif.vars['Password_confirm'], 'sUserPwdHint='+RA_CtrlWin.RA.RAif.vars['sHintPwd'], 'sRemoteIPAddr=chad faking', 'sBaseURL='+RA_CtrlWin.RA.CurrentSite.BaseURL, 'sOptInEmail='+RA_CtrlWin.RA.RAif.vars['OptInEmail'], 'sInstructorEmail='  ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Register_0_go, RAWS1_StudentRegistration, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sUserFirstName='+RA_CtrlWin.RA.RAif.vars['FName'], 'sUserLastName='+RA_CtrlWin.RA.RAif.vars['LName'], 'sUserPwd='+RA_CtrlWin.RA.RAif.vars['Password'], 'sVerifyPwd='+RA_CtrlWin.RA.RAif.vars['Password_confirm'], 'sUserPwdHint='+RA_CtrlWin.RA.RAif.vars['sHintPwd'], 'sRemoteIPAddr=chad faking', 'sBaseURL='+RA_CtrlWin.RA.CurrentSite.BaseURL, 'sInstructorEmail='  ) );
	}
}
RA_CtrlWin.RA.RAif.app.Register_0_go = function () {
	RA_CtrlWin.RA.RAif.app.Register_0();
}
RA_CtrlWin.RA.RAif.app.Register_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RAWS_error=='' && RA_CtrlWin.RAWS_iUserID>0 ) {
//alert( 'RA_CtrlWin.RAWS_iUserID --- '+ RA_CtrlWin.RAWS_iUserID );
		RA_CtrlWin.RA.RAif.app.RAXSLogin_continue(RA_CtrlWin.RAWS_iUserID, 'registered');
//		if ($('#RA_CtrlWin.RA.RAif.app.Register_rememberMe')[0].checked) {
//			RA_ControlWindow.RARememberMe = 1;
//		}
	} else {
if (RA_CtrlWin.RA.dev_check()) alert( 'error iUserID('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		$('#RA_formError_register').show('','');
		$('#RA_formError_register').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}


//  ********************************************************************************
// CheckEmail
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goCheckEmail = function () {
	if ($('#RA_Email_error')[0]) {
		$('#RA_Email_error').css('color','#000');
	} else {
		$('#RA_formError_checkemail').hide('','');
	}
	if ($('#RA_Email')[0].value=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Enter your e-mail address and we\'ll check it for you.';
		if ($('#RA_Email_error')[0]) {
			$('#RA_Email_error').css('color','#f00');
		} else {
			$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
//			$('#RA_formError_checkemail').show('','');
			$('#RA_formError_checkemail').show('','');
		}
		return;
	}
	var tmpE = $('#RA_Email')[0].value;
	if (!RA_CtrlWin.RA.RAif.app.IsValidEmail(tmpE)) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkemail').show('','');
		$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
		return;
	}
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput(tmpE);
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.vars['Email'].toLowerCase();
	RA_CtrlWin.RA.RAif.app.CheckEmail();
}

RA_CtrlWin.RA.RAif.app.CheckEmail = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('checkingemail');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmail_0_goRAWS3, RAWS3_CheckUsername, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sBaseUrl=' ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmail_0_go, RAWS1_GetUsernamePasswordHint, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sBaseUrl=' ) );
	}
}
RA_CtrlWin.RA.RAif.app.CheckEmail_0_goRAWS3 = function () {
	RA_CtrlWin.RA.RAif.app.CheckEmail_0RAWS3();
}
RA_CtrlWin.RA.RAif.app.CheckEmail_0RAWS3 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('checkedemail');
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else if ( RA_CtrlWin.RAWS_error=='User not found.' || RA_CtrlWin.RAWS_error=='User Not found.' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('register');
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	} else if ( RA_CtrlWin.RAWS_error.indexOf('Invalid email address format.')>-1 ) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkemail').show('','');
		$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	} else {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkemail').show('','');
		$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

RA_CtrlWin.RA.RAif.app.CheckEmail_0_go = function () {
	RA_CtrlWin.RA.RAif.app.CheckEmail_0();
}
RA_CtrlWin.RA.RAif.app.CheckEmail_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('checkedemail');
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else if ( RA_CtrlWin.RAWS_error=='User not found.' || RA_CtrlWin.RAWS_error=='User Not found.' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('register');
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	} else if ( RA_CtrlWin.RAWS_error.indexOf('Invalid email address format.')>-1 ) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkemail').show('','');
		$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	} else {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkemail').show('','');
		$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

//  ********************************************************************************
// CheckEmailANDLogin
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goCheckEmailANDLogin = function () {
	if ($('#RA_Email_error')[0]) {
		$('#RA_Email_error').css('color','#000');
	} else {
		$('#RA_formError_checkandemail').hide('','');
	}
	$('#RA_formError_checkemaillogin').hide('','');
	$('#RA_formError_checkemaillogin').css('color','#000');
	if ($('#RA_EmailLOGIN')[0].value=='' || $('#RA_EmailLOGIN')[0].value=='e-mail address') {
		$('#RA_Email_error').css('color','#f00');
		return;
	}
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_EmailLOGIN')[0].value);
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.vars['Email'].toLowerCase();
	if (!RA_CtrlWin.RA.RAif.app.IsValidEmail(RA_CtrlWin.RA.RAif.vars['Email'])) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkemaillogin').show('','');
		$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
		return;
	}
	if ($('#RA_Password')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please enter your password to log in.';
		$('#RA_formError_checkemaillogin').css('color','#f00');
		$('#RA_formError_checkemaillogin').show('','');
		return;
	}
	RA_CtrlWin.RA.RAif.vars['Password'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Password')[0].value);
	RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin();
}


RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	if (!RA_CtrlWin.RA.RAif.app.IsValidEmail(RA_CtrlWin.RA.RAif.vars['Email'])) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		if ($('#RA_formError_checkemail')[0]) {
			$('#RA_formError_checkemail').css('color','#f00');
			$('#RA_formError_checkemail').show('','');
			$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
		} else {
			$('#RA_formError_login').show('','');
			$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
		}
		RA_CtrlWin.RA.RAif.app.Resize();
		return;
	}
	RA_CtrlWin.RA.RAif.state.push('checkemailloggingin');
	RA_CtrlWin.RA.RAif.app.DrawPage();
//alert('SEND auth');
	if (RA_CtrlWin.RA.RavellAuthentication) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Login_0_go, Ravell_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	} else if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
//		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin_0_go, RAWS3_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin_0_go, RAWS1_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin_0_go, RAWS1_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	}
}
RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin_0_go = function () {
	RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin_0();
}
RA_CtrlWin.RA.RAif.app.CheckEmailANDLogin_0 = function () {
//alert('DONE auth');
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RA.RavellAuthentication && RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.app.RavellLogin_continue(RA_CtrlWin.RA.RAif.vars['Email'], RA_CtrlWin.RA.RAif.vars['Password']);
	}
	else if ( RA_CtrlWin.RAWS_error=='' && RA_CtrlWin.RAWS_iUserID>0 ) {
		RA_CtrlWin.RA.RAif.app.RAXSLogin_continue(RA_CtrlWin.RAWS_iUserID, 'checkemailloggedin');
//		if ($('#RA_CtrlWin.RA.RAif.app.Login_rememberMe')[0].checked) {
//			RA_ControlWindow.RARememberMe = 1;
//		}
	} else if (RA_CtrlWin.RA.RavellAuthentication) {
		if ( RA_CtrlWin.RAWS_error.indexOf('Login failed, please try again.') > -1 ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error + ' Your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemail').show('','');
			$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else {
	//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	//prompt( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- ', RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemail').show('','');
			$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		}
	} else {
		if ( RA_CtrlWin.RAWS_error=='E-mail address not found. Check spelling and make sure you are using the e-mail address with which you registered. ' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('register');
			RA_CtrlWin.RA.RAif.vars['Error'] = '';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
	//		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
	//		RA_CtrlWin.RA.RAif.app.DrawPage();
	//		$('#RA_formError_login').show('','');
	//		$('#RA_formError_login').html(RA_CtrlWin.RA.RAif.vars['Error']);
	//		RA_CtrlWin.RA.RAif.app.Resize();
		} else if ( RA_CtrlWin.RAWS_error.indexOf('Invalid email address format.')>-1 ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = '!!! Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			if (RA_CtrlWin.RA.RAif.state.current() == 'checkemail') {
				$('#RA_formError_checkemail').show('','');
				$('#RA_formError_checkemail').html(RA_CtrlWin.RA.RAif.vars['Error']);
			} else {
				$('#RA_formError_checkemaillogin').show('','');
				$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			}
			RA_CtrlWin.RA.RAif.app.Resize();
		} else if ( RA_CtrlWin.RAWS_error=='That e-mail address is registered with a different password. Please enter valid password.' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The password you entered is incorrect. Please try again. If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('checkedemail');
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else if ( RA_CtrlWin.RAWS_error=='That password is incorrect.' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The password you entered is incorrect. Please try again. If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else {
	//prompt('error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- ', RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		}
	}
}

//  ********************************************************************************
// CheckEmailLogin
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goCheckEmailLogin = function () {
	$('#RA_formError_checkemaillogin').hide('','');
	$('#RA_formError_checkemaillogin').css('color','#000');
	if ($('#RA_Password')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please enter your password to log in.';
		$('#RA_formError_checkemaillogin').css('color','#f00');
		$('#RA_formError_checkemaillogin').show('','');
		return;
	}
	RA_CtrlWin.RA.RAif.vars['Password'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Password')[0].value);
	RA_CtrlWin.RA.RAif.app.CheckEmailLogin();
}

RA_CtrlWin.RA.RAif.app.CheckEmailLogin = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('checkemailloggingin');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	if (RA_CtrlWin.RA.RavellAuthentication) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.Login_0_go, Ravell_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	} else if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
//		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmailLogin_0_go, RAWS3_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmailLogin_0_go, RAWS1_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckEmailLogin_0_go, RAWS1_UserAuthentication, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['Email'], 'sPassword='+RA_CtrlWin.RA.RAif.vars['Password'] ) );
	}
}
RA_CtrlWin.RA.RAif.app.CheckEmailLogin_0_go = function () {
	RA_CtrlWin.RA.RAif.app.CheckEmailLogin_0();
}
RA_CtrlWin.RA.RAif.app.CheckEmailLogin_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RA.RavellAuthentication && RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.app.RavellLogin_continue(RA_CtrlWin.RA.RAif.vars['Email'], RA_CtrlWin.RA.RAif.vars['Password']);
	}
	else if ( RA_CtrlWin.RAWS_error=='' && RA_CtrlWin.RAWS_iUserID>0 ) {
		RA_CtrlWin.RA.RAif.app.RAXSLogin_continue(RA_CtrlWin.RAWS_iUserID, 'checkemailloggedin');
//		if ($('#RA_CtrlWin.RA.RAif.app.Login_rememberMe')[0].checked) {
//			RA_ControlWindow.RARememberMe = 1;
//		}
	} else if (RA_CtrlWin.RA.RavellAuthentication) {
		if ( RA_CtrlWin.RAWS_error.indexOf('Login failed, please try again.') > -1 ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error + ' Your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else {
	//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	//prompt( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- ', RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		}
	} else {
		if ( RA_CtrlWin.RAWS_error.indexOf('Invalid email address format.')>-1 ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else if ( RA_CtrlWin.RAWS_error=='That e-mail address is registered with a different password. Please enter valid password.' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The password you entered is incorrect. Please try again. If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else if ( RA_CtrlWin.RAWS_error=='That password is incorrect.' ) {
	//alert( 'error 1 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = 'The password you entered is incorrect. Please try again. If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		} else {
	//prompt('error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- ', RA_CtrlWin.RAWS_error );
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkemaillogin').show('','');
			$('#RA_formError_checkemaillogin').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
		}
	}
}


//  ********************************************************************************
// EmailPassword
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.doEmailPassword = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('emailpassword');
	if (RA_CtrlWin.RA.RAif.vars['Email']=='') {
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Enter your e-mail address.';
		if ($('#RA_Email_error')[0]) {
			$('#RA_Email_error').css('color','#f00');
		} else {
			$('#RA_formError_emailpassword').html(RA_CtrlWin.RA.RAif.vars['Error']);
			$('#RA_formError_emailpassword').show('','');
		}
		return;
	}
	RA_CtrlWin.RA.RAif.app.EmailPassword();
}

RA_CtrlWin.RA.RAif.app.goEmailPassword = function () {
	if ($('#RA_Email_error')[0]) {
		$('#RA_Email_error').css('color','#000');
	} else {
		$('#RA_formError_emailpassword').hide('','');
	}
	if ($('#RA_Email')[0].value=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Enter your e-mail address.';
		if ($('#RA_Email_error')[0]) {
			$('#RA_Email_error').css('color','#f00');
		} else {
			$('#RA_formError_emailpassword').html(RA_CtrlWin.RA.RAif.vars['Error']);
			$('#RA_formError_emailpassword').show('','');
		}
		return;
	}
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Email')[0].value);
	RA_CtrlWin.RA.RAif.vars['Email'] = RA_CtrlWin.RA.RAif.vars['Email'].toLowerCase();
if (!RA_CtrlWin.RA.RAif.app.IsValidEmail(RA_CtrlWin.RA.RAif.vars['Email'])) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_emailpassword').show('','');
		$('#RA_formError_emailpassword').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
		return;
	}
	RA_CtrlWin.RA.RAif.app.EmailPassword();
}

RA_CtrlWin.RA.RAif.app.EmailPassword = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('emailingpassword');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.EmailPassword_0_go, RAWS3_EmailLogins, new Array( 'sUserEmail='+RA_CtrlWin.RA.RAif.vars['Email'] ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.EmailPassword_0_go, RAWS1HCL_EmailLogins, new Array( 'sUserEmail='+RA_CtrlWin.RA.RAif.vars['Email'] ) );
	}
}
RA_CtrlWin.RA.RAif.app.EmailPassword_0_go = function () {
	RA_CtrlWin.RA.RAif.app.EmailPassword_0();
}
RA_CtrlWin.RA.RAif.app.EmailPassword_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('emailedpassword');
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else if ( RA_CtrlWin.RAWS_error=='User not found.' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('register');
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	} else if ( RA_CtrlWin.RAWS_error.indexOf('Invalid UserEmail format.') > -1 ) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Sorry, your e-mail address cannot contain spaces or special characters such as apostrophes. Please check your spelling or register with a different e-mail address.';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_emailpassword').show('','');
		$('#RA_formError_emailpassword').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	} else {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('emailedpassword');
		RA_CtrlWin.RA.RAif.app.DrawPage();
return false;
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_emailpassword').show('','');
		$('#RA_formError_emailpassword').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

//  ********************************************************************************
// CheckCode
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.CodeYesLogin = function () {
	RA_CtrlWin.RA.RAif.state.push('login');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.CodeNoReg = function () {
	RA_CtrlWin.RA.RAif.state.push('checkemail');
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.CodeContinue = function () {
	if (RA_CtrlWin.RA.CurrentUser!=null) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('assignusercodes');
		RA_CtrlWin.RA.RAif.app.goAssignUserCodes();
	} else {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('doassignusercodes');
		RA_CtrlWin.RA.RAif.state.push('checkemail');
	}
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.goCheckCode = function () {
	if (!RA_CtrlWin.RA.RAif.vars['UnlockProducts']) {
		RA_CtrlWin.RA.RAif.vars['UnlockProducts'] = new Object();
		RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'] = new Object();
		RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] = 0;
	}
	RA_CtrlWin.RA.RAif.vars['Code'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_Code')[0].value);
	if (RA_CtrlWin.RA.RAif.vars['Code']=='') {
		RA_CtrlWin.RA.RAif.vars['Error'] = '<p>Enter the activation code printed on your access card and we\'ll check it for you.</p>';
		$('#RA_formError_checkcode').show('','');
		$('#RA_formError_checkcode').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	RA_CtrlWin.RA.RAif.app.CheckCode();
}

RA_CtrlWin.RA.RAif.app.CheckCode = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('checkingcode');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	var tmpSiteIDs = '';
	var tmpSiteIDs_ct = 0;
	for (var iprod in RA_CtrlWin.RA.Products) {if (RA_CtrlWin.RA.Products.hasOwnProperty(iprod)) {
		switch (RA_CtrlWin.RA.Products[iprod].Type) {
		case 'RA CONTENT' :
			tmpSiteIDs_ct ++;
			if (tmpSiteIDs_ct > 1) tmpSiteIDs += ',';
			tmpSiteIDs += RA_CtrlWin.RA.Products[iprod].TypeObj.ID;
			break;
		case 'RA SITE' :
			tmpSiteIDs_ct ++;
			if (tmpSiteIDs_ct > 1) tmpSiteIDs += ',';
			tmpSiteIDs += RA_CtrlWin.RA.Products[iprod].TypeObj.ID;
			break;
		case 'RA PACKAGE SITE' :
			tmpSiteIDs_ct ++;
			if (tmpSiteIDs_ct > 1) tmpSiteIDs += ',';
			tmpSiteIDs += RA_CtrlWin.RA.Products[iprod].TypeObj.ID;
			break;
/*
		case 'RA PACKAGE' :
			for (var isite in RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments) {if (RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments.hasOwnProperty(isite)) {
				tmpSiteIDs_ct ++;
				var tmpID = RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments[isite].Site.ID;
				if (tmpSiteIDs_ct > 1) tmpSiteIDs += ',';
				tmpSiteIDs += tmpID;
			}}
			break;
*/
		}
	}}
/*CCC 201005118: NEED TO UPDATE TO HANDLE UNLOCKING SITE ITSELF WITH NO CART PRODUCTS
   HERE AND WITH ASSIGNUSERCODES
	if (tmpSiteIDs_ct==0)
	{
		tmpSiteIDs += RA_CtrlWin.RA.CurrentSite.ID;
	}
*/
	if (RA_CtrlWin.RA.CurrentUser!=null) {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckCode_0_go, RAWS3_CheckActivationCode, new Array( 'sActivationCode='+RA_CtrlWin.RA.RAif.vars['Code'], 'sSiteIDs='+tmpSiteIDs, 'iUserID='+RA_CtrlWin.RA.CurrentUser.ID ) );
	} else {
		RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckCode_0_go, RAWS3_CheckActivationCode, new Array( 'sActivationCode='+RA_CtrlWin.RA.RAif.vars['Code'], 'sSiteIDs='+tmpSiteIDs, 'iUserID=' ) );
	}
}
RA_CtrlWin.RA.RAif.app.CheckCode_0_go = function () {
	RA_CtrlWin.RA.RAif.app.CheckCode_0();
}
RA_CtrlWin.RA.RAif.app.CheckCode_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//alert( 'CheckCode_0 ('+ RA_CtrlWin.RAWS_iPackageID +') --- '+ RA_CtrlWin.RAWS_error );
	if ( RA_CtrlWin.RAWS_error=='' ) {
//BCSv3_ebook_EW4e_PS: r6-3aj-48c4dz6a
//bcs_bsm_rewritingplus_PS: f4-y6-48c4emyc
//both: wi-3fk-4ege7y8c
//DEV both: mx-32w-3tjukja9
//DEV BCSv3_ebook_EW4e_PS: my-32x-3tjunqb2
//DEV bcs_bsm_rewritingplus_PS: f4-y6-3tjumm9x
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('checkedcode');

		for (var iprod in RA_CtrlWin.RA.Products) {if (RA_CtrlWin.RA.Products.hasOwnProperty(iprod)) {

		switch (RA_CtrlWin.RA.Products[iprod].Type) {
		case 'RA CONTENT' :
			for (var isite in RA_CtrlWin.RAWS_arrSiteIDs) {if (RA_CtrlWin.RAWS_arrSiteIDs.hasOwnProperty(isite)) {
if (false && RA_CtrlWin.RA.dev_check()) {
var mstr = ''
mstr += '\n prod id:';
mstr += RA_CtrlWin.RA.Products[iprod].ID;
mstr += '\n access:';
mstr += RA_CtrlWin.RA.Products[iprod].CurrentUserAccess()
mstr += '\n isite:';
mstr += isite;
mstr += '\n RAWS site arr:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs;
mstr += '\n RAWS site id:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs[isite];
alert( mstr );
}
			if (RA_CtrlWin.RA.Sites[RA_CtrlWin.RAWS_arrSiteIDs[isite]] && RA_CtrlWin.RA.Products[iprod].TypeObj.ID == RA_CtrlWin.RAWS_arrSiteIDs[isite]) {
				if (RA_CtrlWin.RA.Products[iprod].CurrentUserAccess() <= 20) {
					RA_CtrlWin.RA.RAif.vars['UnlockProducts'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.Products[iprod];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.RAif.vars['Code'];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] += 1;
				}
			}
			}}
			break;
		case 'RA SITE' :
			for (var isite in RA_CtrlWin.RAWS_arrSiteIDs) {if (RA_CtrlWin.RAWS_arrSiteIDs.hasOwnProperty(isite)) {
if (true && RA_CtrlWin.RA.dev_check('c')) {
var mstr = ''
mstr += '\n prod id:';
mstr += RA_CtrlWin.RA.Products[iprod].ID;
mstr += '\n access:';
mstr += RA_CtrlWin.RA.Products[iprod].CurrentUserAccess()
mstr += '\n isite:';
mstr += isite;
mstr += '\n RAWS site arr:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs;
mstr += '\n RAWS site id:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs[isite];
alert( mstr );
}
			if (RA_CtrlWin.RA.Sites[RA_CtrlWin.RAWS_arrSiteIDs[isite]] && RA_CtrlWin.RA.Products[iprod].TypeObj.ID == RA_CtrlWin.RAWS_arrSiteIDs[isite]) {
//CCC 201005118: DOES COMMENTING THIS BREAK LOGIC???
//				if (RA_CtrlWin.RA.Products[iprod].CurrentUserAccess() <= 20) {
					RA_CtrlWin.RA.RAif.vars['UnlockProducts'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.Products[iprod];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.RAif.vars['Code'];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] += 1;
//				}
			}
			}}
			break;
		case 'RA PACKAGE' :
//DEV henretta: 33f-3qh-62gcct6b
var mstr = ''
mstr += '\n prod id:';
mstr += RA_CtrlWin.RA.Products[iprod].ID;
mstr += '\n access:';
mstr += RA_CtrlWin.RA.Products[iprod].CurrentUserAccess()
			var yesAll = true;
			var tmpIDsStr = ',';
			for (var isite in RA_CtrlWin.RAWS_arrSiteIDs) {if (RA_CtrlWin.RAWS_arrSiteIDs.hasOwnProperty(isite)) {
				tmpIDsStr += RA_CtrlWin.RAWS_arrSiteIDs[isite] +',';
mstr += '\n isite:';
mstr += isite;
mstr += '\n RAWS site arr:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs;
mstr += '\n RAWS site id:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs[isite];
				if (!RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments[RA_CtrlWin.RAWS_arrSiteIDs[isite]]) {
					yesAll = false;
				}
			}}
mstr += '\n yes all 1:';
mstr += yesAll;
mstr += '\n tmpIDsStr:';
mstr += tmpIDsStr;
			for (var isite2 in RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments) {if (RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments.hasOwnProperty(isite2)) {
				var tmpID = RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments[isite2].Site.ID;
mstr += '\n isite2:';
mstr += isite2;
mstr += '\n tmpID:';
mstr += tmpID;
mstr += '\n tmpIDsStr indexOf tmpID:';
mstr += tmpIDsStr.indexOf(','+tmpID+',');
				if (tmpIDsStr.indexOf(','+tmpID+',')==-1) {
					yesAll = false;
				}
			}}
mstr += '\n yes all 2:';
mstr += yesAll;
if (false && RA_CtrlWin.RA.dev_check()) {
alert( mstr );
}
			if (yesAll && RA_CtrlWin.RA.Products[iprod].CurrentUserAccess() <= 20) {
				RA_CtrlWin.RA.RAif.vars['UnlockProducts'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.Products[iprod];
				RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.RAif.vars['Code'];
				RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] += 1;
			}
/*
			for (var isite2 in RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments) {if (RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments.hasOwnProperty(isite2)) {
				var tmpID = RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments[isite2].Site.ID;
if (RA_CtrlWin.RA.dev_check()) {
var mstr = ''
mstr += '\n prod id:';
mstr += RA_CtrlWin.RA.Products[iprod].ID;
mstr += '\n access:';
mstr += RA_CtrlWin.RA.Products[iprod].CurrentUserAccess()
mstr += '\n isite:';
mstr += isite;
mstr += '\n RAWS site arr:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs;
mstr += '\n RAWS site id:';
mstr += RA_CtrlWin.RAWS_arrSiteIDs[isite];
mstr += '\n isite2:';
mstr += isite2;
mstr += '\n prod site id:';
mstr += RA_CtrlWin.RA.Products[iprod].TypeObj.SiteAssignments[isite2].Site.ID;
alert( mstr );
}
				if (RA_CtrlWin.RA.Sites[RA_CtrlWin.RAWS_arrSiteIDs[isite]] && tmpID == RA_CtrlWin.RAWS_arrSiteIDs[isite]) {
					if (RA_CtrlWin.RA.Products[iprod].CurrentUserAccess() <= 20) {
						RA_CtrlWin.RA.RAif.vars['UnlockProducts'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.Products[iprod];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.RAif.vars['Code'];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] += 1;
					}
				}
			}}
*/
			break;
		}
/*
				if (RA_CtrlWin.RA.Products[iprod].CurrentUserAccess() <= 20 && RA_CtrlWin.RA.Products[iprod].Type == 'RASite') {
					if (RA_CtrlWin.RA.Sites[RA_CtrlWin.RAWS_arrSiteIDs[isite]] && RA_CtrlWin.RA.Products[iprod].TypeObj.ID == RA_CtrlWin.RAWS_arrSiteIDs[isite]) {
						RA_CtrlWin.RA.RAif.vars['UnlockProducts'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.Products[iprod];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'][RA_CtrlWin.RA.Products[iprod].ID] = RA_CtrlWin.RA.RAif.vars['Code'];
						RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] += 1;
					}
				}
*/
		}}
		delete RA_CtrlWin.RA.RAif.vars['Error'];
		delete RA_CtrlWin.RA.RAif.vars['Code'];
//alert( 'pct --- '+ RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] );
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );

		if (RA_CtrlWin.RAWS_error=='Invalid input: no sActivationCode') {

			RA_CtrlWin.RA.RAif.vars['Error'] = '<p>Enter the activation code printed on your access card and we\'ll check it for you.</p>';

		} else if (RA_CtrlWin.RAWS_error=='CodeInvalidFormat' || RA_CtrlWin.RAWS_error=='CodeNotFound' || RA_CtrlWin.RAWS_error=='CodeFailsSumCheck') {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'There\'s a problem with the code you entered. Check that you entered the code exactly as it appears on your access card and try again.';

		} else if (RA_CtrlWin.RAWS_error=='CodeNoUsesLeft') {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'That code is all used up! Check that you entered the same code on your access card and try again. If you get this message again, contact tech support.';

		} else if (RA_CtrlWin.RAWS_error=='CodeUseByDateExpired' || RA_CtrlWin.RAWS_error=='CodeExpiredAbsolute' || RA_CtrlWin.RAWS_error=='CodeExpiredRelative' || RA_CtrlWin.RAWS_error=='BatchSuspended') {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'The code you entered has expired. Contact tech support.';

		} else if (RA_CtrlWin.RAWS_error=='PackageAlreadyAssignedToUser') {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'You already have access to the resources unlocked by this code. If you are experiencing problems with your access, contact tech support.';
//alert(RA_CtrlWin.RAWS_iPackageID +' - '+ RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_iPackageID]);
			if (RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_iPackageID]) {
				var pstr = RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_iPackageID].DisplayAll('form');
				RA_CtrlWin.RA.RAif.vars['Error'] += pstr;
			}

		} else if (RA_CtrlWin.RAWS_error=='Invalid input: no CheckActivationCode element' || RA_CtrlWin.RAWS_error=='Invalid input: no sPackageIDs' || RA_CtrlWin.RAWS_error=='Invalid input: no iSiteID' || RA_CtrlWin.RAWS_error=='Invalid input: no iUserID' || RA_CtrlWin.RAWS_error=='BatchNotFound' || RA_CtrlWin.RAWS_error=='CodeNotValidForPackages' || RA_CtrlWin.RAWS_error=='CodeNotValidForSite' || RA_CtrlWin.RAWS_error=='CodeNotValidForAnySites') {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'An error has occurred. Please contact tech support.  (error code: '+ RA_CtrlWin.RAWS_error +')';

		} else {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'An error has occurred. Please contact tech support.  (error code: '+ RA_CtrlWin.RAWS_error +')';

		}

		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_checkcode').show('','');
		$('#RA_formError_checkcode').html('');
		$('#RA_formError_checkcode').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

//  ********************************************************************************
// AssignUserCodes
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goAssignUserCodes = function () {
	if (!RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes']) {
alert('no codes!');
	}
	RA_CtrlWin.RA.RAif.app.AssignUserCodes();
}

RA_CtrlWin.RA.RAif.app.AssignUserCodes = function () {
//alert(RA_CtrlWin.RA.RAif.vars['Email'] +' / '+ RA_CtrlWin.RA.RAif.vars['Password'])
	RA_CtrlWin.RA.RAif.state.push('assigningusercodes');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	var strcodes = '';
	for (var i in RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes']) {if (RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'].hasOwnProperty(i)) {
		if (strcodes!='') {
			strcodes += ',';
		}
		strcodes += RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'][i];
	}}
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.AssignUserCodes_0_go, RAWS3_AssignUserCodes, new Array( 'sActivationCodes='+strcodes, 'iUserID='+RA_CtrlWin.RA.CurrentUser.ID ) );
}
RA_CtrlWin.RA.RAif.app.AssignUserCodes_0_go = function () {
	RA_CtrlWin.RA.RAif.app.AssignUserCodes_0();
}
RA_CtrlWin.RA.RAif.app.AssignUserCodes_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
//BCSv3_ebook_EW4e_PS: r6-3aj-48c4dz6a
//bcs_bsm_rewritingplus_PS: f4-y6-48c4emyc
//both: wi-3fk-4ege7y8c
//DEV both: mx-32w-3tjukja9
//DEV BCSv3_ebook_EW4e_PS: my-32x-3tjunqb2
//DEV bcs_bsm_rewritingplus_PS: f4-y6-3tjumm9x
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.app.RAXSReInit('assignedusercodes');
	} else {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );

		if (RA_CtrlWin.RAWS_error=='PackageAlreadyAssignedToUser') {

			RA_CtrlWin.RA.RAif.vars['Error'] = 'You already have access to the resources unlocked by this code. Click "continue" to access your resources. If you are experiencing problems with your access, contact tech support.';
			for (var i in RA_CtrlWin.RAWS_udtPackages) {if (RA_CtrlWin.RAWS_udtPackages.hasOwnProperty(i)) {
				var pstr = RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_udtPackages[i].PackageID].DisplayAll('form');
				RA_CtrlWin.RA.RAif.vars['Error'] += pstr;
			}}
//alert(RA_CtrlWin.RAWS_iPackageID +' - '+ RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_iPackageID]);
			if (RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_iPackageID]) {
				var pstr = RA_CtrlWin.RA.Packages[RA_CtrlWin.RAWS_iPackageID].DisplayAll('form');
				RA_CtrlWin.RA.RAif.vars['Error'] += pstr;
			}
		} else {
			RA_CtrlWin.RA.RAif.vars['Error'] = 'An error has occurred. Please contact tech support.  (error code: '+ RA_CtrlWin.RAWS_error +')';
		}

		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_assignusercodes').show('','');
		$('#RA_formError_assignusercodes').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

//  ********************************************************************************
// CodeOrCard
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.CodeOrCardPromptYes = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('checkcode');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

RA_CtrlWin.RA.RAif.app.CodeOrCardPromptNo = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('cart');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

//  ********************************************************************************
// Cart
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.AddToCart = function (pid,cartItemId) {
	for (var jitem in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(jitem)) {
		var item2 = RA_CtrlWin.RA.Cart.Items[jitem];
		if (pid == item2.Product.ID && item2.InCart) {
			var x = RA_CtrlWin.RA.Cart.RemoveItem(item2.Product.ID,item2.ItemID);
		}
	}}
	var x = RA_CtrlWin.RA.Cart.AddItem(pid,cartItemId);
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.RemoveFromCart = function (pid,cartItemId) {
	var x = RA_CtrlWin.RA.Cart.RemoveItem(pid,cartItemId);
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.BackToCart = function (clearError) {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('cart');
	if (clearError) {
		RA_CtrlWin.RA.RAif.vars['Error'] = '';
	}
	RA_CtrlWin.RA.RAif.app.DrawPage();
	if (RA_CtrlWin.RA.RAif.vars['Error'] && RA_CtrlWin.RA.RAif.vars['Error'] != '') {
		$('#RA_formError_cart').show('','');		
	}
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

RA_CtrlWin.RA.RAif.app.goCartCheckOut = function () {
	if (RA_CtrlWin.RA.Cart.Items_InCart_ct>0) {
		if (RA_CtrlWin.RA.CurrentUser!=null) {
//alert('check out!');
			if (!RA_CtrlWin.RA.CurrentUser.School) {
				RA_CtrlWin.RA.RAif.state.pop();
				RA_CtrlWin.RA.RAif.state.push('checkoutschoolprompt');
				RA_CtrlWin.RA.RAif.app.DrawPage();
				RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
			} else {
				RA_CtrlWin.RA.RAif.state.pop();
				RA_CtrlWin.RA.RAif.state.push('checkoutprompt');
				RA_CtrlWin.RA.RAif.app.DrawPage();
				RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
			}
		} else {
			RA_CtrlWin.RA.RAif.state.pop();
			RA_CtrlWin.RA.RAif.state.push('docheckout');
			RA_CtrlWin.RA.RAif.state.push('checkemail');
			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		}
	} else {
alert('Your cart is empty')
	}
}

RA_CtrlWin.RA.RAif.app.CartCheckItems = function () {
	var foundInCart = false;
	for (var item in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(item)) {
		if (RA_CtrlWin.RA.Cart.Items[item].InCart && RA_CtrlWin.RA.Cart.Items[item].Product.CurrentUserAccess() >= RA_CtrlWin.RA.Cart.Items[item].LevelOfAccess) {
			var x = RA_CtrlWin.RA.Cart.RemoveItem(RA_CtrlWin.RA.Cart.Items[item].Product.ID, RA_CtrlWin.RA.Cart.Items[item].ItemID);
			foundInCart = true;
		}
	}}
	if (foundInCart) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'You already have access to one or more items in your cart. We\'ve removed these items. Please review your updated selections below.'; //'
		RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.BackToCart()', 5);
	} else {
		RAif_timeout = setTimeout('RA_CtrlWin.RA.RAif.app.goCartCheckOut()', 5);
	}
}

//  ********************************************************************************
// CheckOut
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.CheckOut = function () {
	RA_CtrlWin.RA.RAif.vars['Error'] = '';
	RA_CtrlWin.RA.RAif.state.push('checkingout');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	RA_CtrlWin.RA.RAif.app.CheckOut_win = window.open( 'cart_start.html', 'RA_checkout_win','location=no,menubar=no,scrollbars=yes,titlebar=no,toolbar=no' );
}

RA_CtrlWin.RA.RAif.app.CartFinishCheckOut = function () {
	if (RA_CtrlWin.RA.Cart.Items_InCart_ct>0) {
//alert('check out!');
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.app.RAXSReInit('checkedout');
	} else {
alert('Your cart is empty')
	}
}

RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool = function () {
	if (RA_CtrlWin.RA.RAif.vars['Schools'].length == 0) {
		RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool1();
	} else {
		var x = document.getElementsByName('RA_SchoolType');
		var y = RA_CtrlWin.RA.RAif.vars['SchoolType'];
		for (var i=0; i<x.length; i++) {
			if (x[i].checked) {
				y = x[i].value;
			}
		}
		if (y != RA_CtrlWin.RA.RAif.vars['SchoolType']) {
			RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool1();
			return;
		}
		if ($('#RA_iZip')[0].value.replace('-','') != RA_CtrlWin.RA.RAif.vars['iZip']) {
			RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool1();
			return;
		}
		var w = $('#RA_School')[0];
		if (w.selectedIndex == -1) {
			RA_CtrlWin.RA.RAif.vars['Error'] = 'Please select your institution from the list below.';
			$('#RA_formError_checkoutschoolprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
			$('#RA_formError_checkoutschoolprompt').show('','');
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
			return;
		}
		RA_CtrlWin.RA.CurrentUser.School = new Object();
		RA_CtrlWin.RA.CurrentUser.School.ID = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].ID;
		RA_CtrlWin.RA.CurrentUser.School.Name = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].Name;
		RA_CtrlWin.RA.CurrentUser.School.Country = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].Country;
		RA_CtrlWin.RA.CurrentUser.School.StateAbbr = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].StateAbbr;
		RA_CtrlWin.RA.CurrentUser.School.City = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].City;
		RA_CtrlWin.RA.CurrentUser.School.Zip = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].Zip;
		RA_CtrlWin.RA.CurrentUser.School.Type = RA_CtrlWin.RA.RAif.vars['Schools'][w.options[w.selectedIndex].value].Type;
		RA_CtrlWin.RA.RAif.app.goCartCheckOut();
	}
}
RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool1 = function () {
	var x = document.getElementsByName('RA_SchoolType');
	for (var i=0; i<x.length; i++) {
		if (x[i].checked) {
			RA_CtrlWin.RA.RAif.vars['SchoolType'] = x[i].value;
		}
	}
	if (RA_CtrlWin.RA.RAif.vars['SchoolType'] == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Please select whether your institution is a "college" or a "high school".';
		$('#RA_formError_checkoutschoolprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_checkoutschoolprompt').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	if ($('#RA_iZip')[0].value == '') {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'Enter the zip code of your college, university, or high school.';
		$('#RA_formError_checkoutschoolprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_checkoutschoolprompt').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	var z = new Number( RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_iZip')[0].value.replace('-','')) );
	if (isNaN(z)) {
		RA_CtrlWin.RA.RAif.vars['Error'] = 'You entered an invalid zip code. Please Enter the zip code of your college, university, or high school.';
		$('#RA_formError_checkoutschoolprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		$('#RA_formError_checkoutschoolprompt').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		return;
	}
	RA_CtrlWin.RA.RAif.vars['iZip'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_iZip')[0].value.replace('-',''));
	RA_CtrlWin.RA.RAif.app.CheckOutLookupSchool();
}

RA_CtrlWin.RA.RAif.app.CheckOutLookupSchool = function () {
	RA_CtrlWin.RA.RAif.state.push('checkoutlookingupschool');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.CheckOutLookupSchool_0_go, RAWS3_GetOnyxSchoolsByZip, new Array( 'iZipPrefix='+RA_CtrlWin.RA.RAif.vars['iZip'], 'sSchoolType='+RA_CtrlWin.RA.RAif.vars['SchoolType'] ) );
}
RA_CtrlWin.RA.RAif.app.CheckOutLookupSchool_0_go = function () {
	RA_CtrlWin.RA.RAif.app.CheckOutLookupSchool_0();
}
RA_CtrlWin.RA.RAif.app.CheckOutLookupSchool_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('checkoutschoolprompt');
		RA_CtrlWin.RA.RAif.vars['Schools'] = new Array();
		for (var i in RA_CtrlWin.RAWS_udtSchools) {if (RA_CtrlWin.RAWS_udtSchools.hasOwnProperty(i)) {
			RA_CtrlWin.RA.RAif.vars['Schools'][RA_CtrlWin.RA.RAif.vars['Schools'].length] = RA_CtrlWin.RAWS_udtSchools[i];
		}}
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
		if ( RA_CtrlWin.RA.RAif.vars['Error'] ) {
			$('#RA_formError_checkoutschoolprompt').show();
		}
//window.self.focus();
	} else {
		if (RA_CtrlWin.RAWS_error == 'SchoolNotFound') {
			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.vars['Error'] = 'We couldn\'t find any schools in that zip code.';
			$('#RA_formError_checkoutschoolprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
			$('#RA_formError_checkoutschoolprompt').show();
//			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		} else {
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_formError_checkoutschoolprompt').show('','');
			$('#RA_formError_checkoutschoolprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.DrawPage();
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		}
	}
}

RA_CtrlWin.RA.RAif.app.goCheckOutSelectSchool = function () {
//		RA_CtrlWin.RA.RAif.app.goCartCheckOut();
}

//  ********************************************************************************
// QuizPrompt
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.reGoQuizPrompt = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('quizprompt');
//	RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] = RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail = RA_CtrlWin.RA.RAif.vars['sInstructorEmail'];
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.goQuizPrompt = function () {
//alert(RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] +' --- '+ RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sInstructorEmail')[0].value));
	RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sInstructorEmail')[0].value);
	if (RA_CtrlWin.RA.RAif.vars['sInstructorEmailSKIP'] != true && RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] == '') {
		$('#RA_form_quizpromptInstrux').css('color','#ff0000');
	} else if (RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] == RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail) {
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('quizprompted');
		RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
		RA_CtrlWin.RA.RAif.updated = true;
		RA_CtrlWin.RA.RAif.vars['Error'] = '';
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		RA_CtrlWin.RA.RAif.state.pop();
	} else if (RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] == '') {
		RA_CtrlWin.RA.RAif.app.QuizPrompt_1();
	} else {
		RA_CtrlWin.RA.RAif.app.QuizPrompt();
	}
}

RA_CtrlWin.RA.RAif.app.QuizPrompt = function () {
//if (RA_CtrlWin.RA.dev_check()) alert(RA_CtrlWin.RA.RAif.vars['sInstructorEmail'])
	RA_CtrlWin.RA.RAif.state.push('quizprompting');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.QuizPrompt_0_go, RAWS1_GetUsernamePasswordHint, new Array( 'sUserName='+RA_CtrlWin.RA.RAif.vars['sInstructorEmail'], 'sBaseUrl=' ) );
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_0_go = function () {
	RA_CtrlWin.RA.RAif.app.QuizPrompt_0();
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_0 = function () {
//prompt( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.RAif.app.QuizPrompt_1();
	} else if (RA_CtrlWin.RAWS_error=='User not found.') {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.state.push('quizpromptBAD');
		RA_CtrlWin.RA.RAif.vars['Error'] = '';
		RA_CtrlWin.RA.RAif.app.DrawPage();
	} else {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
		RA_CtrlWin.RA.RAif.state.pop();
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_quizprompt').show('','');
		$('#RA_formError_quizprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}

RA_CtrlWin.RA.RAif.app.QuizPromptBad1Yes = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('quizprompting');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAif.app.QuizPrompt_1();
}

RA_CtrlWin.RA.RAif.app.QuizPromptBad1No = function () {
//alert( 'error 0 ('+ RA_CtrlWin.RAWS_iUserID +') --- '+ RA_CtrlWin.RAWS_error );
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.app.DrawPage();
}

RA_CtrlWin.RA.RAif.app.QuizPrompt_1 = function () {
//alert(RA_CtrlWin.RA.RAif.vars['sInstructorEmail'])
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.QuizPrompt_1_go, RAWS1_UpdateSiteLogin, new Array( 'iUserID='+RA_CtrlWin.RA.CurrentUser.ID, 'iSiteID='+RA_CtrlWin.RA.CurrentSite.ID, 'sInstructorEmail='+RA_CtrlWin.RA.RAif.vars['sInstructorEmail'], 'sIPAddr='+RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].LoggedOn ) );
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_1_go = function () {
	RA_CtrlWin.RA.RAif.app.QuizPrompt_2();
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_2 = function () {
	if (RA_CtrlWin.RA.RAif.app.useRAWS3) {
		RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail = RA_CtrlWin.RA.RAif.vars['sInstructorEmail'];
		RA_CtrlWin.RA.RAif.app.QuizPrompt_3();
	} else {
		RA_CtrlWin.RA.RAif.state.pop();
//prompt('error',RA_CtrlWin.RAWS_error);
		if ( RA_CtrlWin.RAWS_error=='' ) {
			RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
//			RA_CtrlWin.RA.RAif.app.RAXSReInit('quizprompted');
			RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail = RA_CtrlWin.RA.RAif.vars['sInstructorEmail'];
			RA_CtrlWin.RA.RAif.updated = true;
			RA_CtrlWin.RA.RAif.app.FormContinue();
		} else {
			if ( RA_CtrlWin.RAWS_error == 'Please provide all required information.' || RA_CtrlWin.RAWS_error == 'Please enter email address.' ) {
				RA_CtrlWin.RA.RAif.state.push('quizprompted');
				RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
				RA_CtrlWin.RA.RAif.updated = true;
				RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail = RA_CtrlWin.RA.RAif.vars['sInstructorEmail'];
				RA_CtrlWin.RA.RAif.vars['Error'] = '';
				RA_CtrlWin.RA.RAif.app.DrawPage();
				RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
				RA_CtrlWin.RA.RAif.state.pop();
			} else {
				RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
				RA_CtrlWin.RA.RAif.app.DrawPage();
				$('#RA_formError_quizprompt').show('','');
				$('#RA_formError_quizprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
				RA_CtrlWin.RA.RAif.app.DrawPage();
				RA_CtrlWin.RA.RAif.app.Resize();
	//window.self.focus();
			}
		}
	}
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_3 = function () {
//alert(RA_CtrlWin.RA.RAif.vars['sInstructorEmail'])
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.QuizPrompt_4_go, RAWS3_GetDefaultClass, new Array( 'sUserEmail='+RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail ) );
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_4_go = function () {
	RA_CtrlWin.RA.RAif.app.QuizPrompt_4();
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_4 = function () {
//prompt('error',RA_CtrlWin.RAWS_error);
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].DefaultClassID = RA_CtrlWin.RAWS_iClassUsingClassID;
		RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
		RA_CtrlWin.RA.RAif.updated = true;
		RA_CtrlWin.RA.RAif.app.QuizPrompt_5();
	} else {
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_quizprompt').show('','');
		$('#RA_formError_quizprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
	}
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_5 = function () {
//alert(RA_CtrlWin.RA.RAif.vars['sInstructorEmail'])
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.QuizPrompt_6_go, RAWS3_JoinUsertoClass, new Array( 'iUserID='+RA_CtrlWin.RA.CurrentUser.ID, 'iClassID='+RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].DefaultClassID ) );
//	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.QuizPrompt_6_go, RAWS3_JoinClass, new Array( 'iUserID='+RA_CtrlWin.RA.CurrentUser.ID, 'sClassCode='+RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail ) );
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_6_go = function () {
	RA_CtrlWin.RA.RAif.app.QuizPrompt_6();
}
RA_CtrlWin.RA.RAif.app.QuizPrompt_6 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//prompt('error',RA_CtrlWin.RAWS_error);
	if ( RA_CtrlWin.RAWS_error=='' || RA_CtrlWin.RAWS_error=='User already member of class.' ) {
//alert('done!!!');
//alert(RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].DefaultClassID);
		RA_CtrlWin.RA.RAif.app.FormContinue();
	} else {
		RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
		RA_CtrlWin.RA.RAif.app.DrawPage();
		$('#RA_formError_quizprompt').show('','');
		$('#RA_formError_quizprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
		RA_CtrlWin.RA.RAif.app.DrawPage();
		RA_CtrlWin.RA.RAif.app.Resize();
	}
}

//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.QuizClassPrompt1Yes = function () {
	var UserClassesCt = 0;
	for (var i in RA_CtrlWin.RA.CurrentUser.ClassLogins) {if (RA_CtrlWin.RA.CurrentUser.ClassLogins.hasOwnProperty(i)) {
		UserClassesCt ++;
	}}
console.log(UserClassesCt +' - '+ RA_CtrlWin.RA.CurrentUser.ClassLogins.length);
	switch (UserClassesCt) {
	case 0 :
		$('#RA_quizclassprompt_1').hide('','');
		$('#RA_quizclassprompt_2').show('','');
		$('#RA_quizclassprompt_3').hide('','');
		$('#RA_quizclassprompt_4').hide('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		break;
	case 1 :
		$('#RA_quizclassprompt_1').hide('','');
		$('#RA_quizclassprompt_2').hide('','');
		$('#RA_quizclassprompt_3').hide('','');
		$('#RA_quizclassprompt_4').show('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		break;
	default :
		$('#RA_quizclassprompt_1').hide('','');
		$('#RA_quizclassprompt_2').hide('','');
		$('#RA_quizclassprompt_3').show('','');
		$('#RA_quizclassprompt_4').hide('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}
RA_CtrlWin.RA.RAif.app.QuizClassPrompt1No = function () {
	RA_CtrlWin.RA.CurrentUser.ClassUsing = null;
	$('#RA_quizclassprompt_1').hide('','');
	$('#RA_quizclassprompt_2').hide('','');
	$('#RA_quizclassprompt_3').hide('','');
	$('#RA_quizclassprompt_4').hide('','');
	$('#RA_quizclassprompt_5').show('','');
	setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

RA_CtrlWin.RA.RAif.app.QuizClassPrompt2Yes = function () {
	$('#RA_quizclassprompt_2').show('','');
	$('#RA_quizclassprompt_4').hide('','');
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}
RA_CtrlWin.RA.RAif.app.QuizClassPrompt2No = function () {
	$('#RA_quizclassprompt_2').show('','');
	$('#RA_quizclassprompt_4').hide('','');
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes = function (UseClass) {
	if (UseClass!=null) {
		RA_CtrlWin.RA.CurrentUser.ClassUsing = RA_CtrlWin.RA.CurrentUser.ClassLogins[UseClass].Class;
	} else {
		RA_CtrlWin.RA.CurrentUser.ClassUsing = null;
	}
	RA_CtrlWin.RA.RAif.app.DrawPage();
	$('#RA_quizclassprompt_1').hide('','');
	$('#RA_quizclassprompt_2').hide('','');
	$('#RA_quizclassprompt_3').hide('','');
	$('#RA_quizclassprompt_4').hide('','');
	$('#RA_quizclassprompt_5').show('','');
	setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}
RA_CtrlWin.RA.RAif.app.QuizClassPrompt3No = function () {
	RA_CtrlWin.RA.RAif.app.DrawPage();
	$('#RA_quizclassprompt_1').hide('','');
	$('#RA_quizclassprompt_2').show('','');
	$('#RA_quizclassprompt_3').hide('','');
	$('#RA_quizclassprompt_4').hide('','');
	RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
}

RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes = function () {
	RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
	RA_CtrlWin.RA.RAif.updated = true;
	RA_CtrlWin.RA.RAif.app.FormContinue();
}
RA_CtrlWin.RA.RAif.app.QuizClassPromptedNo = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('classprompt');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	var x = document.getElementsByName('RA_QuizClassPrompt1')
	for (var i=0;i<x.length;i++) if (x[i].value=='Yes') x[i].checked = '';
	switch (UserClassesCt) {
	case 0 :
		$('#RA_quizclassprompt_1').show('','');
		$('#RA_quizclassprompt_2').hide('','');
		$('#RA_quizclassprompt_3').hide('','');
		$('#RA_quizclassprompt_4').hide('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		break;
	case 1 :
		$('#RA_quizclassprompt_1').hide('','');
		$('#RA_quizclassprompt_2').hide('','');
		$('#RA_quizclassprompt_3').show('','');
		$('#RA_quizclassprompt_4').hide('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		break;
	default :
		$('#RA_quizclassprompt_1').hide('','');
		$('#RA_quizclassprompt_2').hide('','');
		$('#RA_quizclassprompt_3').show('','');
		$('#RA_quizclassprompt_4').hide('','');
		RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
	}
}


//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.goQuizJoinClass = function () {
	RA_CtrlWin.RA.RAif.vars['sClassCode'] = RA_CtrlWin.RA.RAif.app.RAWSCleanInput($('#RA_sClassCode')[0].value);
	RA_CtrlWin.RA.RAif.app.QuizJoinClass();
}
RA_CtrlWin.RA.RAif.app.QuizJoinClass = function () {
	RA_CtrlWin.RA.RAif.state.push('quizjoiningclass');
	RA_CtrlWin.RA.RAif.app.DrawPage();
/*
setTimeout('RA_CtrlWin.RA.RAif.app.QuizJoinClass_0_go()', 5);
return;
*/
	RA_CtrlWin.RA.RAWS.Send( RA_CtrlWin.RA.RAif.app.QuizJoinClass_0_go, RAWS3_JoinClass, new Array( 'iUserID='+RA_CtrlWin.RA.CurrentUser.ID, 'sClassCode='+RA_CtrlWin.RA.RAif.vars['sClassCode'] ) );
}
RA_CtrlWin.RA.RAif.app.QuizJoinClass_0_go = function () {
	RA_CtrlWin.RA.RAif.app.QuizJoinClass_0();
}
RA_CtrlWin.RA.RAif.app.QuizJoinClass_0 = function () {
	RA_CtrlWin.RA.RAif.state.pop();
//prompt('',RA_CtrlWin.RAWS_error);
	if ( RA_CtrlWin.RAWS_error=='' ) {
		RA_CtrlWin.RA.CurrentUser.ClassPrompt = 0;
		RA_CtrlWin.RA.RAif.vars['UsingClassID'] = RA_CtrlWin.RAWS_iClassID
		RA_CtrlWin.RA.RAif.app.RAXSReInit('quizjoinedclass');
	} else {
		if ( RA_CtrlWin.RAWS_error == 'SOME ERROR TO CATCH.') {
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_quizclassprompt_1').hide('','');
			$('#RA_quizclassprompt_2').show('','');
			$('#RA_quizclassprompt_3').hide('','');
			$('#RA_quizclassprompt_4').hide('','');
			$('#RA_formError_quizclassprompt').show('','');
			$('#RA_formError_quizclassprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		} else {
			RA_CtrlWin.RA.RAif.vars['Error'] = RA_CtrlWin.RAWS_error;
			RA_CtrlWin.RA.RAif.app.DrawPage();
			$('#RA_quizclassprompt_1').hide('','');
			$('#RA_quizclassprompt_2').show('','');
			$('#RA_quizclassprompt_3').hide('','');
			$('#RA_quizclassprompt_4').hide('','');
			$('#RA_formError_quizclassprompt').show('','');
			$('#RA_formError_quizclassprompt').html(RA_CtrlWin.RA.RAif.vars['Error']);
			RA_CtrlWin.RA.RAif.app.Resize();
//window.self.focus();
		}
	}
}

//  ********************************************************************************
// State Continue/Cancel
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.FormContinue = function () {
	RA_CtrlWin.RA.RAif.state.pop();
	if (RA_CtrlWin.RA.RAif.state.current() == '') {
		RA_CtrlWin.RA.RAif.app.FormCancel();
	} else {
		if (RA_CtrlWin.RA.RAif.UseIFrame == true) 
		{
		    window.location.reload();
		}
		else
		{
		    RA_CtrlWin.RA.RAif.DestroyApp();
		    RA_CtrlWin.RA.RAif.CreateApp();
	        setTimeout('RA_CtrlWin.RA.RAif.app.Init()',20);
		}
	}
}

RA_CtrlWin.RA.RAif.app.FormCartContinue = function () {
	for (var item in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(item)) {
		if (RA_CtrlWin.RA.Cart.Items[item].InCart) {
			var x = RA_CtrlWin.RA.Cart.RemoveItem(RA_CtrlWin.RA.Cart.Items[item].Product.ID, RA_CtrlWin.RA.Cart.Items[item].ItemID);
		}
	}}
	RA_CtrlWin.RA.RAif.app.FormContinue();
}

RA_CtrlWin.RA.RAif.app.FormAssignedCodesContinue = function () {
	delete RA_CtrlWin.RA.RAif.vars['UnlockProducts'];
	delete RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'];
	delete RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'];
	RA_CtrlWin.RA.RAif.app.FormContinue();
}

RA_CtrlWin.RA.RAif.app.FormCancel = function () {
	delete RA_CtrlWin.RA.RAif.vars['UnlockProducts'];
	delete RA_CtrlWin.RA.RAif.vars['UnlockProductsCodes'];
	delete RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'];
	delete RA_CtrlWin.RA.RAif.vars['Code'];
	delete RA_CtrlWin.RA.RAif.vars['Email'];
	delete RA_CtrlWin.RA.RAif.vars['Password'];
	delete RA_CtrlWin.RA.RAif.vars['Error'];
	delete RA_CtrlWin.RA.RAif.vars['RegAlreadyAsked'];
//alert( RA_CtrlWin.RA.RAif.updated +' --- '+ RA_CtrlWin.RA.CurrentUser.ClassPrompt );
	RA_CtrlWin.RA.RAif.close();
	RA_CtrlWin.RA.RAif.state.last = 'formcancel';
}

//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.RAXSLogout = function (nopop) {
	if (!nopop) RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push('loggingout');
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAif.state.pop();
	if (RA_CtrlWin.RA.RavellAuthentication) {
		RA_CtrlWin.RA.RAif.app.RavellLogout ( 'loggedout', nopop, 0, true );
	} else {
		RA_CtrlWin.RA.RAif.app.RAXSReInit ( 'loggedout', nopop, 0, true );
	}
}

RA_CtrlWin.RA.RAif.app.Reload = function (formstate, nopop) {
	if (!nopop) RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push(formstate);
	if (RA_CtrlWin.RA.RAif.UseIFrame == true) 
	{
	    window.location.reload();
	}
	else
	{
	    RA_CtrlWin.RA.RAif.DestroyApp();
	    RA_CtrlWin.RA.RAif.CreateApp();
        setTimeout('RA_CtrlWin.RA.RAif.app.Init()',20);
	}
}

RA_CtrlWin.RA.RAif.app.ShowLogin = function (formstate, nopop) {
	if (!nopop) RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push(formstate);
//	window.location.reload();
	RA_CtrlWin.RA.RAif.app.DrawPage();
	RA_CtrlWin.RA.RAif.app.Resize();
}

RA_CtrlWin.RA.RAif.app.RavellLogin_continue = function ( uname, upw ) {
//alert( 'RA_CtrlWin.RA.RAif.app.RAXSLogin_continue: '+ uname +' - '+ upw );
	var target = BFW_QStr['target'];
	if (RA_CtrlWin.RA.TargetSecureUrl)
	{
		target = RA_CtrlWin.RA.TargetSecureUrl;
	}
	else if (RA_CtrlWin.RA.SecureUrl)
	{
		target = RA_CtrlWin.RA.SecureUrl;
	}
	else if (!target)
	{
		target = 'http://no.target';
	}
	$('#IDPLogin').remove();
	//
	// /account/login?id=emlogin
	var action = 'http://idp.bfwpub.com/nidp/app/login?id=emlogin';
	if (window.location.host.indexOf('dev.') == 0)
	{
		action = 'http://dev.idp.bfwpub.com:8080/nidp/app/login?id=emlogin';
	}
	else if (window.location.host.indexOf('qa.') == 0)
	{
		action = 'http://qa.idp.bfwpub.com/nidp/app/login?id=emlogin';
	}
	else if (window.location.host.indexOf('stg.') == 0)
	{
		action = 'http://stg.idp.bfwpub.com/nidp/app/login?id=emlogin';
	}
	if (RA_CtrlWin.RA.dev_check('any')) {
		alert('idp action url: '+ action);
	}
	var htmlForm = '<form style="display:none;" id="IDPLogin" name="IDPLogin" method="POST" action="'+ action +'" enctype="application/x-www-form-urlencoded">'+
		'<input id="option" name="option" type="hidden" value="credential">'+
		'<input id="target" name="target" type="hidden" value="'+ target +'"></input>'+
		'<input type="hidden" style="display:block;" id="Ecom_User_ID" name="Ecom_User_ID" size="30" maxlength="150" value="'+uname+'"/>'+
		'<input type="password" style="display:block;" id="Ecom_Password" name="Ecom_Password" size="30" maxlength="150" value="'+upw+'"/>'+
		'<input type="submit" value="go"/>'+
		'</form>';

	$('#RA_Page').append(htmlForm);
	if (RA_CtrlWin.RA.dev_check('any')) {
		//alert( $('#IDPLogin').attr('action') +' -- '+ $('#target').val() );
		$('#IDPLogin').append('<input type="submit" value="go"/>');
		$('#IDPLogin').show();
	} else {
		$('#IDPLogin').submit();
	}
}

RA_CtrlWin.RA.RAif.app.RAXSLogin_continue = function ( uid, nextstate, nopop ) {
//alert( 'RA_CtrlWin.RA.RAif.app.RAXSLogin_continue: '+ uid +' - '+ nextstate );
	RA_CtrlWin.RA.RAif.app.RAXSReInit ( nextstate, nopop, uid, true );
}

//  ********************************************************************************
RA_CtrlWin.RA.RAif.app.RavellLogout = function () {
	RA_CtrlWin.RA.ClearUser();
	RA_CtrlWin.jQuery.cookie('RaUData','', { path: '/' });
	var target = BFW_QStr['target'];
	if (RA_CtrlWin.RA.TargetPublicUrl)
	{
		target = RA_CtrlWin.RA.TargetPublicUrl;
	}
	else if (RA_CtrlWin.RA.PublicUrl)
	{
		target = RA_CtrlWin.RA.PublicUrl;
	}
	else if (!target)
	{
		target = 'http://no.target';
	}
	$('#IDPLogin').remove();
	var action = 'http://sb.bfwpub.com/nesp/jsp/AGLogoutRedirect.jsp?target='+ encodeURIComponent(target);
	//var action = 'http://sb.bfwpub.com/nesp/app/plogout?target='+ encodeURIComponent(target);
	if (window.location.host.indexOf('dev.') == 0)
	{
		action = 'http://dev.sb.bfwpub.com/nesp/jsp/AGLogoutRedirect.jsp?target='+ encodeURIComponent(target);
	}
	else if (window.location.host.indexOf('qa.') == 0)
	{
		action = 'http://qa.sb.bfwpub.com/nesp/jsp/AGLogoutRedirect.jsp?target='+ encodeURIComponent(target);
	}
	else if (window.location.host.indexOf('stg.') == 0)
	{
		action = 'http://stg.sb.bfwpub.com/nesp/jsp/AGLogoutRedirect.jsp?target='+ encodeURIComponent(target);
	}
	var winhost = window.location.host;
//	if ( winhost.indexOf('localhost:')==0 )
//	{
//		action = 'http://dev.bfwpub.com/AGLogout';
//	}
	var htmlForm = '<form style="display:none;" id="IDPLogin" name="IDPLogin" method="POST" action="'+ action +'" enctype="application/x-www-form-urlencoded">'+
		'<input id="target" name="target" type="hidden" value="'+target+'"></input>'+
		'</form>';
	$('#RA_Page').append(htmlForm);
	if (RA_CtrlWin.RA.dev_check('any')) {
		//alert( $('#IDPLogin').attr('action') +' -- '+ $('#target').val() );
		$('#IDPLogin').append('<input type="submit" value="go"/>');
		$('#IDPLogin').show();
	} else {
		$('#IDPLogin').submit();
	}
}

//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.RAXSReInit = function ( nextstate, nopop, uid, needsRAXS ) {
//alert( 'RA_CtrlWin.RA.RAif.app.RAXSReInit: '+ uid +' - '+ nextstate );
	RA_CtrlWin.RA.RAif.app.RAXSReInit_holdNeedsRAXS = (needsRAXS==true) ? true : false;
	if (!nopop) RA_CtrlWin.RA.RAif.state.pop();
	RA_CtrlWin.RA.RAif.state.push( nextstate );
	if (!isNaN(uid) && uid>=0) {
		RA_CtrlWin.RA.RAif.app.RAXSInit(uid)
	} else if (RA_CtrlWin.RA.CurrentUser) {
		RA_CtrlWin.RA.RAif.app.RAXSInit(RA_CtrlWin.RA.CurrentUser.ID)
	} else {
		RA_CtrlWin.RA.RAif.app.RAXSInited();
	}
}

RA_CtrlWin.RA.RAif.app.RAXSInit = function ( uid ) {
//alert('RA_CtrlWin.RA.RAif.app.RAXSInit');
	RA_CtrlWin.jQuery.cookie('RAXSsd',uid, { path: '/' });
	RA_CtrlWin.RA.RAXS.InitSession( RA_CtrlWin.RA.RAif.app.RAXSInited );
}
RA_CtrlWin.RA.RAif.app.RAXSInited = function () {
//alert('RA_CtrlWin.RA.RAif.app.RAXSInited');
//alert('RA_CtrlWin.RA.RAif.app.RAXSInited: '+ RA_CtrlWin.RA.CurrentUser);
	RA_CtrlWin.RA.RAif.updated = true;
	RA_CtrlWin.RA.RAif.updated_needsRAXS = RA_CtrlWin.RA.RAif.app.RAXSReInit_holdNeedsRAXS;
	RA_CtrlWin.RA.RAif.app.RAXSReInit_holdNeedsRAXS = false;
//	if (RA_CtrlWin.RA.ProxyType=='ASP.NET')
	var winhost = window.location.host;
	if (winhost.indexOf('localhost:')==0 && RA_CtrlWin.RA.ProxyType=='ASP.NET' && RA_CtrlWin.RA.LocalRAifURL!='')
	{
		if (RA_CtrlWin.RA.RAif.UseIFrame == true) 
		{
		    RA_CtrlWin.RA.RAif._if.src = RA_CtrlWin.RA.LocalRAifURL +'?clear=true';
		}
		else
		{
		    RA_CtrlWin.RA.RAif.DestroyApp();
		    RA_CtrlWin.RA.RAif.CreateApp();
	        setTimeout('RA_CtrlWin.RA.RAif.app.Init()',20);
		}
	}
	else
	{
		if (RA_CtrlWin.RA.RAif.UseIFrame == true) 
		{
		    RA_CtrlWin.RA.RAif._if.src = '/BFWglobal/RAg/v2.0/RAif.html?clear=true';
		}
		else
		{
		    RA_CtrlWin.RA.RAif.DestroyApp();
		    RA_CtrlWin.RA.RAif.CreateApp();
	        setTimeout('RA_CtrlWin.RA.RAif.app.Init()',20);
		}
	}
}


//  ********************************************************************************
// DrawPage
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.DrawPage = function () {
//alert('RA_CtrlWin.RA.RAif.app.DrawPage');
	var str = '';
	str = '';
	switch (RA_CtrlWin.RA.RAif.state.current()) {
	//  ********************************************************************************
		case 'login' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {
				switch (RA_CtrlWin.RA.RAif.state.next()) {
					case 'quizprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						str += '<p class="RA_formText">You need to be logged in to take this quiz.</p>';
						break;
					case 'classprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						str += '<p class="RA_formText">You need to be logged in to take this quiz.</p>';
						break;
					default :
						str += '<p class="RA_formTitle">Log in</p>';
				}
				str += '<p id="RA_formError_login" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_Login" action="JavaScript:RA_CtrlWin.RA.RAif.app.goLogin();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>E-mail address:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += '';
				}
				str += '"/></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>Password:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_Password" name="pw" value=""/></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td></td><td>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goLogin();">LOG IN</a></td>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '</tr></table>';
				str += '</td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
//				str += '<p class="RA_formText">';
//				str += 'If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
//				str += '</p>';
			}
		break;
		case 'loggingin' :
			str += '<p class="RA_formText">logging in...</p>';
		break;
		case 'loggedin' :
//			str += '<p class="RA_formTitle">Log in</p>';
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">logging in...</p>';
				setTimeout('RA_CtrlWin.RA.RAif.app.FormContinue()', 5);
/*
				str += '<p class="RA_formText">';
				str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CONTINUE</a></td>';
				str += '</tr></table>';
*/
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
	//  ********************************************************************************
		case 'logout' :
//			str += '<p class="RA_formTitle">Log out</p>';
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += 'Are you sure you want to log out?';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
		case 'loggingout' :
			str += '<p class="RA_formText">logging out...</p>';
		break;
		case 'loggedout' :
//			str += '<p class="RA_formText">logging out...</p>';
			setTimeout('RA_CtrlWin.RA.RAif.app.FormContinue()', 5);
/*
			str += '<p class="RA_formText">You are now logged out</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CONTINUE</a></td>';
				str += '</tr></table>';
*/
		break;
	//  ********************************************************************************
		case 'checkedemailbad' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {
				str += '<p class="RA_formTitle">Log in</p>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formText"><nobr>You entered:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formValueDisplay">';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += '';
				}
				str += '</td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<p class="RA_formText">We don\'t have an account with this e-mail address. Would you like to create a new account?</p>';
				str += '<p class="RA_formText">';
				str += '<input type="radio" onclick="RA_CtrlWin.RA.RAif.app.LoginYesReg()"/>Yes&nbsp;&nbsp;&nbsp;<input type="radio" onclick="RA_CtrlWin.RA.RAif.app.LoginNoCheck()"/>No, try a different e-mail address.';
				str += '</p>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			}
		break;
	//  ********************************************************************************
		case 'checkemail' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {
				switch (RA_CtrlWin.RA.RAif.state.next()) {
					case 'docheckout' :
						str += '<p class="RA_formTitle">Check out: Step 1 of 2: Register</p>';
						break;
					case 'doassignusercodes' :
						str += '<p class="RA_formTitle">Step 2 of 2: Log in or Register</p>';
						break;
					case 'quizprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						break;
					case 'classprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						break;
					default :
//						str += '<p class="RA_formTitle">Registration</p>';
				}

/*VERSION 2, LOGIN/CHECKEMAIL
			str += '<table class="RA_box_tbl" width="100%" border="0" cellpadding="0" cellspacing="0">';
			str += '<tr>';
			str += '<td class="RA_box_tbl_tl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_ml">&nbsp;</td>';
			str += '<td class="RA_box_tbl_mc">';

			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px">';
				str += '<p class="RA_formTitle">Returning Student?</p>';
				str += '<p id="RA_formError_checkemaillogin" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<p class="RA_formText" id="RA_Email_error"><nobr>Enter your e-mail address and password to log in.&nbsp;</nobr></p>';
				str += '<form name="RA_CheckEmailANDLogin" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmailANDLogin();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formLabelSmall">';
				str += 'E-mail address';
				str += '</td>';
				str += '<td width="10" class="RA_formLabelSmall">';
				str += 'Password';
				str += '</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td width="10" class="RA_formField">';
				str += '<input maxlength="150" type="email" id="RA_EmailLOGIN" name="emaillogin" value="" style="width:130px;"/></td>';
				str += '<td width="10" class="RA_formField">';
				str += '<input type="password" maxlength="16" id="RA_Password" name="password" value="" style="width:90px;"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmailANDLogin();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += 'If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
				str += '</p>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a>/td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

			str += '</td>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px; border-left:1px solid #000;">';
				str += '<p class="RA_formTitle">New Student?</p>';
				str += '<p id="RA_formError_checkemail" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<p class="RA_formText" id="RA_Email_error"><nobr>Enter the e-mail address you would like to register:&nbsp;</nobr></p>';
				str += '<form name="RA_CheckEmail" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
//				if (RA_CtrlWin.RA.RAif.vars['Email']) {
//				str += RA_CtrlWin.RA.RAif.vars['Email'];
//				} else {
//				}
				str += '"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '<br/>';
			str += '</td>';
			str += '</tr></table>';

			str += '</td>';
			str += '<td class="RA_box_tbl_mr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_bl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_bc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_br">&nbsp;</td>';
			str += '</tr>';
			str += '</table>';
*/

/*VERSION 1, BASIC EMAIL CHECK ONLY
*/
				str += '<p id="RA_formError_checkemail" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_CheckEmail" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">';
				if (RA_CtrlWin.RA.RAif.vars['RegAlreadyAsked']) {
					str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
					str += '<td width="10" class="RA_formText" id="RA_Email_error"><nobr>Enter the e-mail address you would like to register:&nbsp;</nobr></td>';
				} else {
					str += '<p class="RA_formText">Do you already have an account with us? If you have ever used a textbook or online resources published by Bedford/St. Martin&#x0027;s, W.H. Freeman, or Worth Publishers, you may already have an account with us.</p>';
					str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
					str += '<td width="10" class="RA_formText" id="RA_Email_error"><nobr>Enter your e-mail address and we\'ll check for you:&nbsp;</nobr></td>';
				}
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
//				if (RA_CtrlWin.RA.RAif.vars['Email']) {
//				str += RA_CtrlWin.RA.RAif.vars['Email'];
//				} else {
//				}
				str += '"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '<br/>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			}
		break;
		case 'checkingemail' :
			str += '<p class="RA_formText">checking...</p>';
		break;
		case 'checkedemail' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {

				switch (RA_CtrlWin.RA.RAif.state.next()) {
					case 'docheckout' :
						str += '<p class="RA_formTitle">Check out: Step 1 of 2: Register</p>';
						break;
					case 'doassignusercodes' :
						str += '<p class="RA_formTitle">Step 2 of 2: Log in Register</p>';
						break;
					case 'quizprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						break;
					case 'classprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						break;
					default :
//						str += '<p class="RA_formTitle">Registration</p>';
				}

				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formText"><nobr>You entered:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formValueDisplay">';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += '';
				}
				str += '</td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

				str += '<p class="RA_formText">';
				str += 'We have an account with this e-mail address. ';
					switch (RA_CtrlWin.RA.RAif.state.next()) {
						case 'docheckout' :
				str += 'Would you like to use this account to purchase your premium content?';
							break;
						case 'doassignusercodes' :
				str += 'Would you like to use this account to unlock your premium content?';
							break;
						case 'quiz' :
				str += 'Would you like to use this account to access your quiz content?';
							break;
						default :
				str += 'Would you like to log in with this account?';
					}
				str += '</p>';

			str += '<table class="RA_box_tbl" width="100%" border="0" cellpadding="0" cellspacing="0">';
			str += '<tr>';
			str += '<td class="RA_box_tbl_tl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_ml">&nbsp;</td>';
			str += '<td class="RA_box_tbl_mc">';

			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px">';
				str += '<p class="RA_formTitle">Yes, log in</p>';
				str += '<p id="RA_formError_checkemaillogin" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_CheckEmailLogin" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmailLogin();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formText"><nobr>Enter your password:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_Password" name="password" value=""/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmailLogin();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '</p>';
//				str += '<p class="RA_formText">';
//				str += 'If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
//				str += '</p>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

			str += '</td>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px; border-left:1px solid #000;">';
				str += '<p class="RA_formTitle">No, create a new account</p>';
				str += '<p id="RA_formError_checkemail" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<p class="RA_formText" id="RA_Email_error"><nobr>Enter the e-mail address you would like to register:&nbsp;</nobr></p>';
				str += '<form name="RA_CheckEmail" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
//				if (RA_CtrlWin.RA.RAif.vars['Email']) {
//				str += RA_CtrlWin.RA.RAif.vars['Email'];
//				} else {
//				}
				str += '"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '<br/>';
			str += '</td>';
			str += '</tr></table>';

			str += '</td>';
			str += '<td class="RA_box_tbl_mr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_bl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_bc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_br">&nbsp;</td>';
			str += '</tr>';
			str += '</table>';


			}
		break;
		case 'checkemaillogin' :
			switch (RA_CtrlWin.RA.RAif.state.next()) {
				case 'docheckout' :
					str += '<p class="RA_formTitle">Check out: Step 1 of 2: Register</p>';
					break;
				case 'doassignusercodes' :
					str += '<p class="RA_formTitle">Step 2 of 2: Log in Register</p>';
					break;
				case 'quiz' :
					str += '<p class="RA_formTitle">Registration</p>';
					break;
				default :
//					str += '<p class="RA_formTitle">Registration</p>';
			}
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {

				switch (RA_CtrlWin.RA.RAif.state.next()) {
					case 'docheckout' :
						str += '<p class="RA_formTitle">Check out: Step 1 of 2: Register</p>';
						break;
					case 'doassignusercodes' :
						str += '<p class="RA_formTitle">Step 2 of 2: Log in or Register</p>';
						break;
					case 'quizprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						break;
					case 'classprompt' :
						str += '<p class="RA_formTitle">Quiz log in</p>';
						break;
					default :
//						str += '<p class="RA_formTitle">Registration</p>';
				}

				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formText"><nobr>You entered:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formValueDisplay">';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += '';
				}
				str += '</td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

				str += '<p class="RA_formText">';
				str += 'We have an account with this e-mail address. ';
					switch (RA_CtrlWin.RA.RAif.state.next()) {
						case 'docheckout' :
				str += 'Would you like to use this account to purchase your premium content?';
							break;
						case 'doassignusercodes' :
				str += 'Would you like to use this account to unlock your premium content?';
							break;
						case 'quiz' :
				str += 'Would you like to use this account to access your quiz content?';
							break;
						default :
				str += 'Would you like to log in with this account?';
					}
				str += '</p>';

			str += '<table class="RA_box_tbl" width="100%" border="0" cellpadding="0" cellspacing="0">';
			str += '<tr>';
			str += '<td class="RA_box_tbl_tl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_ml">&nbsp;</td>';
			str += '<td class="RA_box_tbl_mc">';

			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px">';
				str += '<p class="RA_formTitle">Yes, log in</p>';
				str += '<p id="RA_formError_checkemaillogin" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_CheckEmailLogin" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmailLogin();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formText"><nobr>Enter your password:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_Password" name="password" value=""/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmailLogin();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
//				str += '<p class="RA_formText">';
//				str += 'If you have forgotten your password, <a href="JavaScript:RA_CtrlWin.RA.RAif.app.doEmailPassword();">click here</a> and we\'ll e-mail it to you.';
//				str += '</p>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

			str += '</td>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px; border-left:1px solid #000;">';
				str += '<p class="RA_formTitle">No, create a new account</p>';
				str += '<p id="RA_formError_checkemail" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_CheckEmail" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">';
				if (RA_CtrlWin.RA.RAif.vars['RegAlreadyAsked']) {
					str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
					str += '<td width="10" class="RA_formText" id="RA_Email_error"><nobr>Enter the e-mail address you would like to register:&nbsp;</nobr></td>';
				} else {
					str += '<p class="RA_formText">Do you already have an account with us? If you have ever used a textbook or online resources published by Bedford/St. Martin&#x0027;s, W.H. Freeman, or Worth Publishers, you may already have an account with us.</p>';
					str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
					str += '<td width="10" class="RA_formText" id="RA_Email_error"><nobr>Enter your e-mail address and we\'ll check for you:&nbsp;</nobr></td>';
				}
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
//				if (RA_CtrlWin.RA.RAif.vars['Email']) {
//				str += RA_CtrlWin.RA.RAif.vars['Email'];
//				} else {
//				}
				str += '"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';

			str += '</td>';
			str += '</tr></table>';
			str += '<br/>';

			str += '</td>';
			str += '<td class="RA_box_tbl_mr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_bl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_bc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_br">&nbsp;</td>';
			str += '</tr>';
			str += '</table>';

			}
		break;
		case 'checkemailloggingin' :
			str += '<p class="RA_formText">logging in...</p>';
		break;
		case 'checkemailloggedin' :
//			str += '<p class="RA_formTitle">Log in</p>';
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">logging in...</p>';
				setTimeout('RA_CtrlWin.RA.RAif.app.FormContinue()', 5);
/*
				str += '<p class="RA_formText">';
				str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button">';
				var fnstr = 'JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue()';
				str += '<a class="BFWj_PepperButton" href="'+fnstr+'">CONTINUE</a>';
				str += '</td>';
				str += '</tr></table>';
*/
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
	//  ********************************************************************************
		case 'emailpassword' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {
//				str += '<p class="RA_formTitle">E-mail Password</p>';
				str += '<p id="RA_formError_emailpassword" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<p class="RA_formText" id="RA_Email_error">Enter your e-mail address and we\'ll send your password to you.</p>';
				str += '<form name="RA_EmailPassword" action="JavaScript:RA_CtrlWin.RA.RAif.app.goEmailPassword();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += 'e-mail address';
				}
				str += '" onfocus="if (this.value==\'e-mail address\') this.value=\'\'" onblur="if (this.value==\'\') this.value=\'e-mail address\';"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goEmailPassword();">GO</a>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '<br/>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			}
		break;
		case 'emailingpassword' :
			str += '<p class="RA_formText">emailing...</p>';
		break;
		case 'emailedpassword' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {
				str += '<p class="RA_formText">';
				str += 'Your password has been e-mailed to ';
				str += '<span class="RA_formValueDisplay">';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += 'the address you entered';
				}
				str += '</span>';
				str += '</p>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\');">LOG IN</a></td>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

			}
		break;
		case 'register' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">log out</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			} else {

				str += '<p class="RA_formTitle">';
				switch (RA_CtrlWin.RA.RAif.state.next()) {
					case 'docheckout' :
					str += 'Check out: Step 1 of 2: Register';
						break;
					case 'doassignusercodes' :
					str += 'Step 2 of 2: Register';
						break;
					case 'quizprompt' :
					str += 'Quiz log in';
						break;
					case 'classprompt' :
					str += 'Quiz log in';
						break;
					default :
					str += 'Register';
				}
				str += '</p>';

				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formText"><nobr>You entered:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formValueDisplay">';
				if (RA_CtrlWin.RA.RAif.vars['Email']) {
				str += RA_CtrlWin.RA.RAif.vars['Email'];
				} else {
				str += '';
				}
				str += '</td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';

				str += '<p class="RA_formText">';
				str += 'We don\'t have an account with this e-mail address. Would you like to create one?';
				str += '</p>';

			str += '<table class="RA_box_tbl" width="100%" border="0" cellpadding="0" cellspacing="0">';
			str += '<tr>';
			str += '<td class="RA_box_tbl_tl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_ml">&nbsp;</td>';
			str += '<td class="RA_box_tbl_mc">';

			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 0px">';
				str += '<p class="RA_formTitle">Yes, create an account with this e-mail address</p>';
				str += '<p class="RA_formText">';
				str += 'You can register an account with this e-mail address by completing the form below.';
				str += '</p>';
				str += '<p id="RA_formError_register" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_Register" action="JavaScript:RA_CtrlWin.RA.RAif.app.goRegister();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>Enter your first name:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input maxlength="50" type="text" id="RA_FName" name="email" value="';
				if (RA_CtrlWin.RA.RAif.vars['FName']) {
				str += RA_CtrlWin.RA.RAif.vars['FName'];
				} else {
				str += '';
				}
				str += '"/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_FName_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>Enter your last name:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input maxlength="50" type="text" id="RA_LName" name="email" value="';
				if (RA_CtrlWin.RA.RAif.vars['LName']) {
				str += RA_CtrlWin.RA.RAif.vars['LName'];
				} else {
				str += '';
				}
				str += '"/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_LName_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>Choose your password:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_Password" name="password" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_Password_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<br/>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0" style="border:1px solid #f00;"><tr>';
				str += '<td colspan="4" style="padding:4px;"><p class="RA_formText">Please confirm your e-mail address and password. You will need to remember these for the next time you log in.</p></td>';
				str += '</tr><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>Re-enter your e-mail address:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email_confirm" name="email_confirm" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_Email_confirm_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td width="10" class="RA_formLabel"><nobr>Re-enter your password:&nbsp;</nobr></td>';
				str += '<td width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_Password_confirm" name="password_confirm" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_Password_confirm_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '</tr></table>';
				str += '<br/>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td class="RA_formLabel">Make it easier for you to hear about updates and special offers about this product and new products from Bedford, Freeman, & Worth Publishing Group that can benefit you in your studies and preparation for exams.</td>';
				str += '<td width="10" class="RA_formField"><input type="checkbox" id="RA_OptInEmail" name="RA_OptInEmail" CHECKED="CHECKED"></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '</tr></table>';
				str += '<br/>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goRegister();">REGISTER</a></td>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';

			str += '</td>';
			str += '<td width="50%" valign="top" style="padding:0px 10px 10px 30px; border-left:1px solid #000;">';
				str += '<p class="RA_formTitle">No, use a different e-mail address</p>';
				str += '<p class="RA_formText">';
				str += 'If you are certain you have an account with us, please try again. Make sure you are entering your e-mail address correctly.';
				str += '</p>';
				str += '<p id="RA_formError_checkemail" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<form name="RA_CheckEmail" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_Email" name="email" value="';
//				if (RA_CtrlWin.RA.RAif.vars['Email']) {
//				str += RA_CtrlWin.RA.RAif.vars['Email'];
//				} else {
//				}
				str += '"/></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckEmail();">GO</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
				str += '<br/>';
			str += '</td>';
			str += '</tr></table>';

			str += '</td>';
			str += '<td class="RA_box_tbl_mr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_bl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_bc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_br">&nbsp;</td>';
			str += '</tr>';
			str += '</table>';

			}
		break;
		case 'registering' :
			str += '<p class="RA_formText">registering...</p>';
		break;
		case 'registered' :
			str += '<p class="RA_formTitle">';
			switch (RA_CtrlWin.RA.RAif.state.next()) {
				case 'docheckout' :
				str += 'Check out: Step 1 of 2: Register';
					break;
				case 'doassignusercodes' :
				str += 'Step 2 of 2: Register';
					break;
				case 'quiz' :
				str += 'Register';
					break;
				default :
				str += 'Register';
			}
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are now registered and logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CONTINUE</a></td>';
				str += '</tr></table>';
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'checkemail\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
	//  ********************************************************************************
		case 'codeorcart' :
			str += '<p class="RA_formText">';
			str += 'Did you receive an access card packaged with your text?';
			str += '</p>';
			str += '<p class="RA_formText">';
			str += '<input type="radio" name="RA_CodeOrCardPrompt" value="Yes" onclick="RA_CtrlWin.RA.RAif.app.CodeOrCardPromptYes()"/>Yes&nbsp;&nbsp;&nbsp;';
			str += '<input type="radio" name="RA_CodeOrCardPrompt" value="No" onclick="RA_CtrlWin.RA.RAif.app.CodeOrCardPromptNo()"/>No.';
			str += '</p>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
	//  ********************************************************************************
		case 'checkcode' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formTitle">Unlock your resources</p>';
			} else {
				str += '<p class="RA_formTitle">Step 1 of 2: Enter Your Activation Code</p>';
			}
			str += '<div id="RA_formError_checkcode" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';

			str += '<form name="RA_CheckCode" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckCode();">';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Enter the activation code printed on your access card:&nbsp;</nobr></td>';
			str += '<td valign="top" width="10" class="RA_formField"><nobr><input maxlength="50" type="text" id="RA_Code" name="code" ';
//			if (RA_CtrlWin.RA.RAif.vars['Code']) {
//			str += ' value="'+ RA_CtrlWin.RA.RAif.vars['Code'] +'" onfocus="if (this.value==\'\') this.value=\'r6-3aj-48c4dz6a\';"';
//			} else {
//			str += ' value="" onfocus="if (this.value==\'\') this.value=\'r6-3aj-48c4dz6a\';"';
//			}
			str += ' value=""';
			str += '/>&nbsp;&nbsp;</nobr></td>';
			str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckCode();">GO</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
		case 'checkingcode' :
			str += '<p class="RA_formText">checking code ...</p>';
		break;
		case 'checkedcode' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formTitle">Unlock your resources</p>';
			} else {
				str += '<p class="RA_formTitle">Step 1 of 2</p>';
			}
			str += '<p class="RA_formText">The code(s) you entered unlock';
//			if (RA_CtrlWin.RA.RAif.vars['CodesEntered'] > 1) {
//				str += 's you entered unlock';
//			} else {
//				str += ' you entered unlocks';
//			}
			str += ' the following premium content. Is this what you want to unlock? If so, click continue.</p>';

			for (var i in RA_CtrlWin.RA.RAif.vars['UnlockProducts']) {if (RA_CtrlWin.RA.RAif.vars['UnlockProducts'].hasOwnProperty(i)) {
				str += '<p class="RA_formText">'+ RA_CtrlWin.RA.Products[RA_CtrlWin.RA.RAif.vars['UnlockProducts'][i].ID].DisplayAll('form') +'</p>';
			}}
			str += '<div id="RA_formError_checkcode" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';
			str += '<form name="RA_CheckCode" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckCode();">';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td valign="top" width="10" class="RA_formText"><nobr>Do you have another activation code?&nbsp;</nobr></td>';
			str += '<td valign="top" width="10" class="RA_formText"><nobr><div id="RA_checkedcode_another_prompt"><a href="JavaScript:void(0);" onclick="document.getElementById(\'RA_checkedcode_another\').style.display=\'block\';document.getElementById(\'RA_checkedcode_another_prompt\').style.display=\'none\';">Click here to enter it.</a></div></nobr></td>';
			str += '<td width="10" class="RA_button"></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr><tr id="RA_checkedcode_another" style="display:none">';
			str += '<td valign="top" width="10" class="RA_formText"><nobr>Enter additional activation code:&nbsp;</nobr></td>';
			str += '<td valign="top" width="10" class="RA_formField"><nobr><input maxlength="50" type="text" id="RA_Code" name="code" value="';
//			if (RA_CtrlWin.RA.RAif.vars['Code']) {
//			str += RA_CtrlWin.RA.RAif.vars['Code'];
//			} else {
//			str += 'f4-y6-48c4emyc';
//			}
			str += '"/>&nbsp;&nbsp;</nobr></td>';
			str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckCode();">GO</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.CodeContinue();">CONTINUE</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
	//  ********************************************************************************
		case 'assignusercodes' :
			str += '<p class="RA_formTitle">Unlock your resources (assign)</p>';
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p id="RA_formError_assignusercodes" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CONTINUE</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
		case 'assigningusercodes' :
			str += '<p class="RA_formText">unlocking resources ...</p>';
		break;
		case 'assignedusercodes' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				if (RA_CtrlWin.RA.RAif.vars['justloggedin']) {
					str += '<p class="RA_formText">';
					str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+'), ';
					str += 'and you have unlocked the following premium resource';
					if (RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] > 1) {
						str += 's';
					} else {
						str += '';
					}
					str += '.</p>';
					delete RA_CtrlWin.RA.RAif.vars['justloggedin'];
				} else {
					str += '<p class="RA_formTitle">You have unlocked the following premium resource';
					if (RA_CtrlWin.RA.RAif.vars['UnlockProductsCt'] > 1) {
						str += 's';
					} else {
						str += '';
					}
					str += '.</p>';
				}
				for (var i in RA_CtrlWin.RA.RAif.vars['UnlockProducts']) {if (RA_CtrlWin.RA.RAif.vars['UnlockProducts'].hasOwnProperty(i)) {
					str += '<p class="RA_formText">'+ RA_CtrlWin.RA.Products[RA_CtrlWin.RA.RAif.vars['UnlockProducts'][i].ID].DisplayAll('form') +'</p>';
				}}
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormAssignedCodesContinue();">CONTINUE</a></td>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
	//  ********************************************************************************
		case 'cart' :

			if (RA_CtrlWin.RA.RAif.vars['justloggedin']) {
				str += '<p class="RA_formText">';
				str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				delete RA_CtrlWin.RA.RAif.vars['justloggedin'];
			}
			str += '<p class="RA_formTitle">Shopping Cart</p>';

			str += '<div id="RA_formError_cart" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';
			str += '<br/>';

			str += '<table class="RA_box_tbl" width="100%" border="0" cellpadding="0" cellspacing="0">';
			str += '<tr>';
			str += '<td class="RA_box_tbl_tl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_ml">&nbsp;</td>';
			str += '<td class="RA_box_tbl_mc">';

			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';

			str += '<td align="center" valign="top" width="100%" style="padding:0px 10px 30px 0px">';
				str += '<table width="660" border="0" cellpadding="0" cellspacing="0">';
				str += '<tr>';
				str += '<td width="25" style="width:25px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td style="width:150px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="5" style="width:5px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="140" style="width:140px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="10" style="width:10px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="10" style="width:10px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="140" style="width:140px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="5" style="width:5px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td width="25" style="width:25px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '<td style="width:150px;margin:0px;padding:0px;font-size:1px;">&nbsp;</td>';
				str += '</tr>';

				str += '<tr><td colspan="4" style="margin-top:0px;vertical-align:top;">';
				str += '<p class="RA_formTitle" style="margin-top:0px;">Available premium resources</p><hr style="background-color:#aaa;height:1px;"/>';
				str += '</td>';
//				str += '<td rowspan="10" width="10" style="border-right:1px solid #000;">&nbsp;</td>';
//				str += '<td rowspan="10" width="10">&nbsp;</td>';
				str += '<td width="10" style="border-right:1px solid #000;">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td colspan="4" style="margin-top:0px;vertical-align:top;">';
				str += '<p class="RA_formTitle" style="margin-top:0px;">Your cart</p><hr style="background-color:#aaa;height:1px;"/>';
				str += '</td></tr>';
			var displayedItems = new Object();
			for (var iitem in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(iitem)) {
				var item = RA_CtrlWin.RA.Cart.Items[iitem];
				if (!displayedItems[item.Product.ID]) {

				str += '<tr>';
				str += '<td width="25" valign="top" align="center" class="RA_formText"><nobr>';
					if ( item.Product.CurrentUserAccess() >= item.LevelOfAccess ) {
				str += '<img src="images/RA_premium_unlocked.gif" width="18" height="13" border="0" alt="unlocked"/>&nbsp;&nbsp;';
					} else {
				str += '<img src="images/RA_premium_locked.gif" width="18" height="13" border="0" alt="locked"/>&nbsp;&nbsp;';
					}
				str += '&nbsp;&nbsp;</nobr></td>';
				str += '<td valign="top" class="RA_formText"><div style="margin:0px;padding:0px;">';
				str += item.Product.Title;
				str += '&nbsp;&nbsp;<br/>';
				str += '</div></td>';
				str += '<td width="5"><br/><br/></td>';
				str += '<td width="140" class="RA_formLabel" align="right" valign="top" style="width:140px">';

				if ( item.Product.CurrentUserAccess() >= item.LevelOfAccess ) {
				str += '<p style="margin-top:0;margin-bottom:10px"><nobr><span style="color:#c00">you have access</span></nobr></p>';
				} else {
			for (var jitem in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(jitem)) {
				var item2 = RA_CtrlWin.RA.Cart.Items[jitem];
				if (item.Product.ID == item2.Product.ID) {
				str += '<p style="margin-top:0;margin-bottom:5px;">';
				str += '<nobr><i>'
					if (item2.ExpType == 'Relative') {
						var tmpExp = item2.ExpValue;
						if (tmpExp%365==0) {
				str += ''+ tmpExp/365 +' year access';
						} else {
				str += ''+ tmpExp +' day access';
						}
					} else if (item2.ExpType == 'Absolute') {
				str += 'access until '+ item2.ExpValue +'';
					}
				str += '&nbsp;&nbsp;</i></nobr>';
				str += '$'+ item2.Price +'</p>';
					if ( item2.InCart ) {
				str += '<p style="margin-bottom:10px"><nobr>in cart &gt;</nobr></p>';
					} else {
				str += '<table border="0" cellpadding="0" cellspacing="0" style="margin-bottom:10px"><tr>';
				str += '<td>&nbsp;</td>';
				str += '<td width="98" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.AddToCart(\''+ item2.Product.ID +'\',\''+ item2.ItemID +'\');">ADD TO CART &gt;</a></td>';
				str += '</tr></table>';
					}
				}
			}}
				}
				str += '</td>';
				str += '<td width="10" style="border-right:1px solid #000;">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
					//-----------------------------------------------------
//					if (item.InCart) {
			var item2InCart = false;
			for (var jitem in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(jitem)) {
				var item2 = RA_CtrlWin.RA.Cart.Items[jitem];
				if (item.Product.ID == item2.Product.ID && item2.InCart) {
					item2InCart = true;
				}
			}}
					if (item2InCart) {
				str += '<td width="140" class="RA_formLabel" align="left" valign="top" style="width:140px;">';
			for (var jitem in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(jitem)) {
				var item2 = RA_CtrlWin.RA.Cart.Items[jitem];
					if (item2.InCart) {
				if (item.Product.ID == item2.Product.ID) {
				str += '<p style="width:135px;margin-top:0;margin-bottom:5px;">';
				str += '<nobr><i>'
					if (item2.ExpType == 'Relative') {
						var tmpExp = item2.ExpValue;
						if (tmpExp%365==0) {
				str += ''+ tmpExp/365 +' year access';
						} else {
				str += ''+ tmpExp +' day access';
						}
					} else if (item2.ExpType == 'Absolute') {
				str += 'access until '+ item2.ExpValue +'';
					}
				str += '&nbsp;&nbsp;</i></nobr>';
				str += '$'+ item2.Price +'</p>';
				str += '<table border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="68" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.RemoveFromCart(\''+ item2.Product.ID +'\',\''+ item2.ItemID +'\');">&lt; REMOVE</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
				}
					}
			}}
				str += '</td>';
				str += '<td width="5"><br/><br/></td>';
				str += '<td width="25" valign="top" align="center" class="RA_formText"><nobr>';
				str += '<img src="images/RA_premium_locked.gif" width="18" height="13" border="0" alt="locked"/>&nbsp;&nbsp;';
				str += '&nbsp;&nbsp;</nobr></td>';
				str += '<td valign="top" class="RA_formText"><div style="margin:0px;padding:0px;">';
				str += item.Product.Title;
				str += '&nbsp;&nbsp;<br/>';
				str += '</div></td>';
					} else {
				str += '<td width="140" style="width:140px">&nbsp;</td>';
				str += '<td width="5"><br/><br/></td>';
				str += '<td width="25">&nbsp;</td>';
				str += '<td>&nbsp;</td>';
					}
				str += '</tr>';
				displayedItems[item.Product.ID] = true;
				}//displayedItems[item.Product.ID]
			}}
				str += '<tr>';
				str += '<td>&nbsp;</td>';
				str += '<td>&nbsp;</td>';
				str += '<td>&nbsp;</td>';
				str += '<td>&nbsp;</td>';
				str += '<td width="10" style="border-right:1px solid #000;">&nbsp;</td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td colspan="4" style="padding:20px 10px 10px 10px;">';
					str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
					str += '<td width="110" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCartCheckOut();">CHECK OUT NOW</a></td>';
					str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
					str += '<td>&nbsp;</td>';
					str += '</tr></table>';
				str += '</td>';
				str += '</tr>';
				str += '</table>';
			str += '</td>';
			str += '</tr></table>';

			str += '</td>';
			str += '<td class="RA_box_tbl_mr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_bl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_bc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_br">&nbsp;</td>';
			str += '</tr>';
			str += '</table>';

			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
		case 'checkoutschoolprompt' :
			if (!RA_CtrlWin.RA.RAif.vars['Schools']) {
				RA_CtrlWin.RA.RAif.vars['Schools'] = new Array();
			}
			if (!RA_CtrlWin.RA.RAif.vars['SchoolType']) {
				RA_CtrlWin.RA.RAif.vars['SchoolType'] = 'C';
			}
			if (!RA_CtrlWin.RA.RAif.vars['iZip']) {
				RA_CtrlWin.RA.RAif.vars['iZip'] = NaN;
			}
			if (RA_CtrlWin.RA.RAif.vars['justloggedin']) {
				str += '<p class="RA_formText">';
				str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				delete RA_CtrlWin.RA.RAif.vars['justloggedin'];
			}
			str += '<p class="RA_formTitle">Checking out...</p>';
			str += '<p id="RA_formError_checkoutschoolprompt" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';
			str += '<p class="RA_formText">Is the course for which you are using this resource being offered by a...<br/>';
			str += '<input type="radio" name="RA_SchoolType" value="C"';
			if (RA_CtrlWin.RA.RAif.vars['SchoolType'] == 'C') {
				str += ' CHECKED';
			}
			str += '/>college or university&nbsp;&nbsp;&nbsp;or&nbsp;';
			str += '<input type="radio" name="RA_SchoolType" value="H"';
			if (RA_CtrlWin.RA.RAif.vars['SchoolType'] == 'H') {
				str += ' CHECKED';
			}
			str += '/>high school?.';
			str += '</p>';
			str += '<form name="RA_CheckOutLookupSchool" action="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool();">';
			str += '<p class="RA_formText">Please enter the zip code or postal code <br/>of your college, university, or high school: ';
			str += '<input maxlength="10" type="text" id="RA_iZip" name="zip" value="';
			if (! isNaN(RA_CtrlWin.RA.RAif.vars['iZip']) ) {
				str += RA_CtrlWin.RA.RAif.vars['iZip'];
			}
			str += '" size="10" maxlength="10"/>';
			str += '<br/><span class="RA_formText" style="font-size:8pt;">';
			str += '(If you do not know the zip code or postal code of your school, enter your home zip code or postal code.)'
			str += '</span></p>';
			if (RA_CtrlWin.RA.RAif.vars['Schools'].length > 0) {
			str += '<p class="RA_formText" style="font-size:8pt;">';
			str += '<select id="RA_School" name="RA_School" size="';
				if (RA_CtrlWin.RA.RAif.vars['Schools'].length == 1) {
			str += '2';
				} else if (RA_CtrlWin.RA.RAif.vars['Schools'].length > 9) {
			str += '9';
				} else {
			str += Number(RA_CtrlWin.RA.RAif.vars['Schools'].length+1);
				}
			str += '">';
				for ( var i=0; i<RA_CtrlWin.RA.RAif.vars['Schools'].length; i++ ) {
			str += '<option value="'+ i +'">';
			str += RA_CtrlWin.RA.RAif.vars['Schools'][i].Name;
			str += '</option>'
				}
			str += '</select>';
			str += '</p>';
			}
			str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="40" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goCheckOutLookupSchool();">NEXT</a></td>';
			str += '<td width="170" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.BackToCart(true);">BACK TO MY SHOPPING CART</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
		case 'checkoutlookingupschool' :
			str += '<p class="RA_formText">searching for zip code...</p>';
		break;
		case 'checkoutprompt' :
			str += '<p class="RA_formTitle">Checking out...</p>';
//			str += '<p class="RA_formText">A new window will open and take you through the check out and payment process using PayPal, a safe and widely trusted internet payment website. You will need to register an account with PayPal, or log in if you already have one. Your PayPal account is different from the account you log in with here.</p>';
//			str += '<p class="RA_formText">You must complete the payment process in the PayPal pop-up window before you can finish unlocking your selected premium resources. At the end of the payment process, you will see an orange button labelled "CLICK HERE TO FINISH UNLOCKING YOUR PREMIUM RESOURCES". You must click this button to confirm your purchase and return to this website with your resources unlocked. You can cancel your purchase at any time in the PayPal pop-up window.</p>';
			str += '<p class="RA_formText">In order to collect your payment securely, we\'re going to link you PayPal. At the end of the payment process, you will see an orange button labelled "CLICK HERE TO FINISH UNLOCKING YOUR PREMIUM RESOURCES". Click that button to confirm your purchase and return to this website with your resources unlocked. You can cancel your purchase at any time in the PayPal pop-up window and return to this website.</p>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="105" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.CheckOut();">CHECK OUT NOW</a></td>';
			str += '<td width="170" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.BackToCart(true);">BACK TO MY SHOPPING CART</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
		case 'checkingout' :
			str += '<p class="RA_formText">checking out...</p>';
			str += '<p class="RA_formText">You must complete the payment process in the check out pop-up window before you can finish unlocking your selected premium resources.</p>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="170" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.BackToCart(true);">BACK TO MY SHOPPING CART</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
		case 'checkedout' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formTitle">Your resource';
				if (RA_CtrlWin.RA.Cart.Items_InCart_ct > 1) {
					str += 's have';
				} else {
					str += ' has';
				}
				str += ' been unlocked</p>';
				for (var iitem in RA_CtrlWin.RA.Cart.Items) {if (RA_CtrlWin.RA.Cart.Items.hasOwnProperty(iitem)) {
					if (RA_CtrlWin.RA.Cart.Items[iitem].InCart) {
				str += '<p class="RA_formText">'+ RA_CtrlWin.RA.Cart.Items[iitem].Product.DisplayAll('form') +'</p>';
					}
				}}
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCartContinue();">CONTINUE</a></td>';
				str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>, or <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
	//  ********************************************************************************
		case 'updateprofile' :
			str += '<p class="RA_formTitle">Update your profile information</p>';
			if (RA_CtrlWin.RA.CurrentUser==null) {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>';
				str += '</p>';
			} else {
				str += '<p id="RA_formError_updateprofile" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</p>';

				str += '<form name="RA_UpdateProfile" action="JavaScript:RA_CtrlWin.RA.RAif.app.goUpdateProfile();">';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>First and last name:&nbsp;</nobr></td>';
				str += '<td valign="top" width="150" class="RA_formField">';
				str += '<div id="RA_editName" style="display:none">';

				str += '<table border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td valign="top" width="10" class="RA_formField"><input maxlength="50" type="text" id="RA_sUserFirstName" name="fname" value="'+ RA_CtrlWin.RA.RAif.vars['sUserFirstName'] +'"/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sUserFirstName_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField"><nobr>&nbsp;<input maxlength="50" type="text" id="RA_sUserLastName" name="lname" value="'+ RA_CtrlWin.RA.RAif.vars['sUserLastName'] +'"/></nobr></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sUserLastName_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr></table>';

				str += '</div>';
				str += '<div id="RA_showName" class="RA_formShow" style="display:block"><nobr>'+ RA_CtrlWin.RA.RAif.vars['sUserFirstName'] +'&nbsp;'+ RA_CtrlWin.RA.RAif.vars['sUserLastName'] +'&nbsp;&nbsp;&nbsp;&nbsp;<a class="RA_editlink" href="JavaScript:void(0);" onclick="document.getElementById(\'RA_editName\').style.display=\'block\';document.getElementById(\'RA_showName\').style.display=\'none\';">edit</a></nobr></div>';
				str += '</td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';

				str += '<td valign="top">&nbsp;</td><td valign="top">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';

				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>E-mail address:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField">';
				str += '<div id="RA_showEmail" class="RA_formShow" style="display:block"><nobr>'+ RA_CtrlWin.RA.RAif.vars['sUserEmail'] +'&nbsp;&nbsp;&nbsp;&nbsp;<a class="RA_editlink" href="JavaScript:void(0);" onclick="RA_CtrlWin.RA.RAif.app.doChangeEmail()">edit</a></nobr></div>';
				str += '<div id="RA_editEmail" style="display:none">';
				str += '<table border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #ff0000;"><tr>';
				str += '<td valign="top" width="10">&nbsp;</td><td valign="top" width="10">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Edit your e-mail address:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_sUserEmail" name="email" value="'+ RA_CtrlWin.RA.RAif.vars['sUserEmail'] +'"/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sUserEmail_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Confirm your new e-mail address:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField"><input maxlength="150" type="text" id="RA_sUserEmail_confirm" name="email_confirm" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sUserEmail_confirm_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top">&nbsp;</td><td valign="top">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr></table>';
				str += '</div>';
				str += '</td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';

				str += '<td valign="top">&nbsp;</td><td valign="top">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';

				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Password:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField">';
				str += '<div id="RA_showPw" class="RA_formShow" style="display:block"><nobr>**********&nbsp;&nbsp;&nbsp;&nbsp;<a class="RA_editlink" href="JavaScript:void(0);" onclick="RA_CtrlWin.RA.RAif.app.doChangePw()">change password</a></nobr></div>';
				str += '<div id="RA_changePw" style="display:none;">';
				str += '<table border="0" cellpadding="0" cellspacing="0" style="border: 1px solid #ff0000;"><tr>';
				str += '<td valign="top" width="10">&nbsp;</td><td valign="top" width="10">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Current password:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_sOldPwd" name="pw" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sOldPwd_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>New password:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_sNewPwd" name="newpw" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sNewPwd_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Confirm new password:&nbsp;</nobr></td>';
				str += '<td valign="top" width="10" class="RA_formField"><input type="password" maxlength="16" id="RA_sVPwd" name="vpw" value=""/></td>';
				str += '<td width="10" valign="middle"><nobr><div id="RA_sVPwd_error" class="RA_formFieldError">&lt;&lt;</div></nobr></td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top">&nbsp;</td><td valign="top">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr></table>';
				str += '</div>';

				str += '</td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top">&nbsp;</td><td valign="top">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';

				str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Instructor e-mail:&nbsp;</nobr></td>';
				str += '<td valign="top" width="150" class="RA_formField"><nobr>';
				if(RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail == ''){
					str += '<i>none</i>';
				} else {
					str += RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail;
				}
				str += '&nbsp;&nbsp;&nbsp;&nbsp;<a class="RA_editlink" href="Javascript:RA_CtrlWin.RA.RAif.app.goEditInstEmail();">edit</a>';
				str += '</td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top">&nbsp;</td><td valign="top">&nbsp;</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';

				str += '<td valign="top" colspan="3" style="padding-right:200px">';
//				str += '<br/>';
				str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td class="RA_formLabel">Make it easier for you to hear about updates and special offers about this product and new products from Bedford, Freeman, & Worth Publishing Group that can benefit you in your studies and preparation for exams.</td>';
				str += '<td width="10" class="RA_formField"><input type="checkbox" id="RA_OptInEmail" name="RA_OptInEmail"';
			if (RA_CtrlWin.RA.CurrentUser.OptInEmail=='True') {
				str += ' CHECKED="CHECKED"';
			}
				str += '></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '</tr></table>';
				str += '</td><td valign="top">&nbsp;</td>';
				str += '</tr><tr>';
				str += '<td valign="top"></td><td class="RA_formField" valign="top">';
				str += '<div id="RA_editLogin" style="display:none;width:550px;color:#ee0000;">You have chosen to change your e-mail address and/or password. You must use these to log in again. Please confirm your the changes above before saving.</div>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td valign="top" width="105" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goUpdateProfile();">SAVE CHANGES</a></td>';
				str += '<td valign="top" width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
				str += '</tr></table>';
				str += '</td>';
				str += '<td valign="top">&nbsp;</td>';
				str += '</tr></table>';
				str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			}
		break;
		case 'updatingprofile' :
			str += '<p class="RA_formText">saving changes to your profile...</p>';
		break;
		case 'updatedprofile' :
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'Your changes have been saved. You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CONTINUE</a></td>';
				str += '</tr></table>';
			} else {
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">log in</a> - <a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">cancel</a>';
				str += '</p>';
			}
		break;
	//  ********************************************************************************
		case 'quizprompt' :
RA_CtrlWin.RA.RAif.vars['sInstructorEmailSKIP'] = false;
			str += '<div id="RA_formError_quizprompt" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';

//			str += '<p class="RA_formText">You do not have to enter your instructor\'s email address to do this exercise, but if your instructor wants to see your results, you will need to enter it. By entering your instructor\'s e-mail address, a copy of your quiz results will be sent to a report your instructor can access.</p>';
			str += '<p id="RA_form_quizpromptInstrux" class="RA_formText">Please enter your instructor\'s email address.</p>';
			str += '<form name="RA_QuizPrompt" action="JavaScript:RA_CtrlWin.RA.RAif.app.goQuizPrompt();">';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td valign="top" width="10" class="RA_formLabel"><nobr>Instructor e-mail address:&nbsp;</nobr></td>';
			str += '<td valign="top" width="10" class="RA_formField"><nobr><input maxlength="150" type="text" id="RA_sInstructorEmail" name="instructoremail" value="';
			if (RA_CtrlWin.RA.RAif.vars['sInstructorEmail']) {
			str += RA_CtrlWin.RA.RAif.vars['sInstructorEmail'];
			} else {
			str += RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail;
			}
			str += '"/>&nbsp;&nbsp;</nobr></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goQuizPrompt();">CONTINUE</a></td>';
			str += '<td width="340" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.vars[\'sInstructorEmailSKIP\'] = true;document.getElementById(\'RA_sInstructorEmail\').value=\'\';RA_CtrlWin.RA.RAif.app.goQuizPrompt();">SKIP THIS AND RECORD MY QUIZ RESULTS TO MY SCORECARD</a></td>';
			str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL AND DO NOT TAKE THE QUIZ</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
/*
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goQuizPrompt();">CONTINUE</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
*/
		break;
		case 'quizprompting' :
			str += '<p class="RA_formText">saving ...</p>';
		break;
		case 'quizpromptBAD' :
			str += '<p class="RA_formText"><b>'+ RA_CtrlWin.RA.RAif.vars['sInstructorEmail'] +'</b> does not yet exist in our system. Are you sure this is the correct e-mail address?</p>';
			str += '<p class="RA_formText">';
			str += '<input type="radio" name="RA_QuizPromptBad1" value="Yes" onclick="RA_CtrlWin.RA.RAif.app.QuizPromptBad1Yes()"/>Yes&nbsp;&nbsp;&nbsp;';
			str += '<input type="radio" name="RA_QuizPromptBad1" value="No" onclick="RA_CtrlWin.RA.RAif.app.QuizPromptBad1No()"/>No.';
			str += '</p>';
			str += '<br/>';
		break;
		case 'quizprompted' :
			setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
/*
			str += '<div id="RA_formError_quizprompt" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';
			str += '<p class="RA_formText">';
			if (RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail=='') {
					str += 'No instructor e-mail address has been saved with your profile. If your instructor wishes to view your quiz results, you will need to update your profile. You can do so now, or next time you visit.';
			} else {
					str += 'Congratulations. The e-mail address <b>'+ RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail +'</b> has been saved with your profile as your instructor\'s e-mail address, and now your instructor will be able to view results on your quizzes and exercises. If you need to change the e-mail address, or other elements of your profile, you can do so by choosing review profile now, or the next time you visit.';
			}
			str += '</p>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes();">CONTINUE</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.reGoQuizPrompt();">CHANGE</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
*/
		break;
	//  ********************************************************************************
		case 'classprompt' :
			str += '<div id="RA_formError_quizclassprompt" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';
//alert(RA_CtrlWin.RA.CurrentUser.ClassLogins.length);

			// --------------------------------------------------------------
			str += '<div id="RA_quizclassprompt_1">';
//			str += '<p class="RA_formText">';
//			str += '1. Quiz Class Prompt';
//			str += '</p>';
			if (RA_CtrlWin.RA.RAif.vars['justloggedin']) {
				str += '<p class="RA_formText">';
				str += 'You are now logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				delete RA_CtrlWin.RA.RAif.vars['justloggedin'];
			}
			str += '<p class="RA_formText">Do you want your quiz score to be recorded into your instructor\'s gradebook?</p>';
			str += '<p class="RA_formText">';
			str += '<input type="radio" name="RA_QuizClassPrompt1" value="Yes" onclick="RA_CtrlWin.RA.RAif.app.QuizClassPrompt1Yes()"/>Yes&nbsp;&nbsp;&nbsp;';
			str += '<input type="radio" name="RA_QuizClassPrompt1" value="No" onclick="RA_CtrlWin.RA.RAif.app.QuizClassPrompt1No()"/>No.';
			str += '</p>';
			str += '<br/>';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL AND DO NOT TAKE THE QUIZ</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '</div>';

			// --------------------------------------------------------------
			str += '<div id="RA_quizclassprompt_2">';
//			str += '<p class="RA_formText">';
//			str += '2. Join Class Prompt';
//			str += '</p>';
			str += '<p class="RA_formText">If your instructor gave you a gradebook ID, enter it here. Or enter your instructor\'s e-mail address.';
			str += '</p>';
			str += '<form name="RA_QuizJoinClass" action="JavaScript:RA_CtrlWin.RA.RAif.app.goQuizJoinClass();">';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td valign="top" width="10" class="RA_formField"><nobr><input maxlength="150" type="text" id="RA_sClassCode" name="RA_sClassCode" value=""/>&nbsp;&nbsp;</nobr></td>';
			str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goQuizJoinClass();">GO</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			str += '<br/>';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="340" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes(null);">SKIP THIS AND RECORD MY QUIZ RESULTS TO MY SCORECARD</a></td>';
			str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL AND DO NOT TAKE THE QUIZ</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '</div>';

			// --------------------------------------------------------------
			str += '<div id="RA_quizclassprompt_3">';
//			str += '<p class="RA_formText">';
//			str += '3. Class List';
//			str += '</p>';
//			str += '<p class="RA_formText">Do you want your quiz score to be recorded into your instructor\'s gradebook?</p>';
		    str += '<p class="RA_formText">';
            if (RA_CtrlWin.RA.CurrentUser.ClassUsing && RA_CtrlWin.RA.CurrentUser.ClassUsing!=null)
            {
			    str += 'You are currently associated with instructor '+ RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.Email +'. Choose your instructor from the list of your saved instructor\'s below, or click the "Enter a new instructor" button.';
            }
            else
            {
			    str += 'Choose your instructor from the list of your saved instructor\'s below, or click the "Enter a new instructor e-mail" button.';
            }
		    str += '</p>';

			str += '<table class="RA_box_tbl" width="100%" border="0" cellpadding="0" cellspacing="0">';
			str += '<tr>';
			str += '<td class="RA_box_tbl_tl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_tr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_ml">&nbsp;</td>';
			str += '<td class="RA_box_tbl_mc">';

			str += '<p class="RA_formText"><b>Your saved gradebooks:</b></p>';
			str += '<table width="100%" border="0" cellpadding="3" cellspacing="10">';
				str += '<tr><td valign="top" width="10" class="RA_formLabel"><nobr><b>Instructor</b></nobr></td>';
//				str += '<td valign="top" width="10" class="RA_formLabel"><nobr><b>Name</b></nobr></td>';
//				str += '<td valign="top" width="100" class="RA_formLabel"><nobr><b>Description</b></nobr></td>';
//				str += '<td valign="top" width="10" class="RA_formLabel"><nobr><b>Expiration</b></nobr></td>';
				str += '<td width="10">&nbsp;</td>';
				str += '<td>&nbsp;</td></tr>';
				str += '<tr><td valign="top" colspan="5" style="border-top:1px solid #000; font-size:1px">&nbsp;</td></tr>';
			for (var i in RA_CtrlWin.RA.CurrentUser.ClassLogins) {if (RA_CtrlWin.RA.CurrentUser.ClassLogins.hasOwnProperty(i)) {
				var x = RA_CtrlWin.RA.CurrentUser.ClassLogins[i];
//				str += '<tr><td valign="top" width="10" class="RA_formText"><nobr>'+x.Class.Creator.FName+'&nbsp;'+x.Class.Creator.LName+'</nobr></td>';
				str += '<tr><td valign="top" width="10" class="RA_formText"><nobr>'+x.Class.Creator.Email+'</nobr></td>';
//				str += '<td valign="top" width="10" class="RA_formText"><nobr>'+x.Class.Name+'</nobr></td>';
//				str += '<td valign="top" width="100" class="RA_formLabel" style="padding-top:4px"><nobr>'+x.Class.Desc+'</nobr></td>';
//				str += '<td valign="top" width="10" class="RA_formLabel" style="padding-top:4px"><nobr>'+x.Class.Expiration+'</nobr></td>';
                if (RA_CtrlWin.RA.CurrentUser.ClassUsing && RA_CtrlWin.RA.CurrentUser.ClassUsing!=null)
                {
				    if (x.Class.ID == RA_CtrlWin.RA.CurrentUser.ClassUsing.ID) 
				    {
				        str += '<td width="210" class="RA_formText"><i>currently chosen</i></td>';
				    }
				    else
				    {
				        str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes('+i+');">SELECT</a></td>';
				    }
                }
                else
                {
			        str += '<td width="80" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes('+i+');">SELECT</a></td>';
                }
//				    str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes('+i+');">RECORD QUIZ RESULTS INTO THIS GRADEBOOK</a></td>';
				str += '<td>&nbsp;</td></tr>';
				str += '<tr><td valign="top" colspan="5" style="border-top:1px solid #888; font-size:1px">&nbsp;</td></tr>';
			}}
			str += '</table>';

			str += '</td>';
			str += '<td class="RA_box_tbl_mr">&nbsp;</td>';
			str += '</tr>';
			str += '<tr>';
			str += '<td class="RA_box_tbl_bl">&nbsp;</td>';
			str += '<td class="RA_box_tbl_bc">&nbsp;</td>';
			str += '<td class="RA_box_tbl_br">&nbsp;</td>';
			str += '</tr>';
			str += '</table>';

			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
/*
			str += '<td width="200" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3No();">USE A DIFFERENT GRADEBOOK</a></td>';
			str += '<td width="340" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes(null);">SKIP THIS AND RECORD MY QUIZ RESULTS TO MY SCORECARD</a></td>';
			str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL AND DO NOT TAKE THE QUIZ</a></td>';
*/
			str += '<td width="200" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3No();">ENTER A NEW INSTRUCTOR E-MAIL</a></td>';
            if (RA_CtrlWin.RA.CurrentUser.ClassUsing && RA_CtrlWin.RA.CurrentUser.ClassUsing!=null)
            {
    			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes('+RA_CtrlWin.RA.CurrentUser.ClassUsing.ID+');">CONTINUE</a></td>';
    			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPrompt3Yes(null);">SKIP THIS</a></td>';
            }
            else
            {
            }
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '</div>';

			// --------------------------------------------------------------
			str += '<div id="RA_quizclassprompt_4">';
//			str += '<p class="RA_formText">';
//			str += '6. Do not report results';
//			str += '</p>';


			if (RA_CtrlWin.RA.CurrentUser.ClassUsing == null) {
				str += '<p class="RA_formText">';
				str += 'Your quiz results will not be recorded into your instructor\'s gradebook. Do you want to continue?';
				str += '</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes();">CONTINUE</a></td>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPromptedNo();">GO BACK</a></td>';
				str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL AND DO NOT TAKE THE QUIZ</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			} else {
				str += '<p class="RA_formText">';
//alert(RA_CtrlWin.RA.CurrentUser.ClassUsing);
				str += 'Your quiz results will be recorded into <nobr><b>'+RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.FName+'&nbsp;'+RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.LName+'\'s</b></nobr> <nobr>('+RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.Email+')</nobr> gradebook <nobr>"'+RA_CtrlWin.RA.CurrentUser.ClassUsing.Name+'"</nobr>.';
				str += '</p>';
				str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes();">CONTINUE</a></td>';
				str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPromptedNo();">GO BACK</a></td>';
				str += '<td width="210" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL AND DO NOT TAKE THE QUIZ</a></td>';
				str += '<td>&nbsp;</td>';
				str += '</tr></table>';
			}

			str += '</div>';

			// --------------------------------------------------------------
			str += '<div id="RA_quizclassprompt_5" style="display:none;">';
			str += '<p class="RA_formText">saving ...</p>';
			str += '</div>';
		break;
		case 'quizjoiningclass' :
			str += '<p class="RA_formText">saving ...</p>';
		break;
		case 'quizjoinedclass' :
			// --------------------------------------------------------------
			str += '<div id="RA_formError_quizclassprompt" class="RA_formError"></div>';
			// --------------------------------------------------------------
			str += '<div id="RA_quizclassprompt_5" style="display:block;">';
			str += '<p class="RA_formText">saving ...</p>';
			str += '</div>';
			str += '<div id="RA_quizclassprompt_4" style="display:none">';


			if (RA_CtrlWin.RA.CurrentUser.ClassUsing == null) {
				str += '<p class="RA_formText">';
				str += 'Your quiz results will not be recorded into your instructor\'s gradebook.';
				str += '</p>';
			} else {
				str += '<p class="RA_formText">';
//alert(RA_CtrlWin.RA.CurrentUser.ClassUsing);
				str += 'Your quiz results will be recorded into <nobr><b>'+RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.FName+'&nbsp;'+RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.LName+'\'s</b></nobr> <nobr>('+RA_CtrlWin.RA.CurrentUser.ClassUsing.Creator.Email+')</nobr> gradebook <nobr>"'+RA_CtrlWin.RA.CurrentUser.ClassUsing.Name+'"</nobr>.';
				str += '</p>';
			}
			str += '</div>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CONTINUE</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
	//  ********************************************************************************
		case 'nosite' :
			str += '<p style="text-align:right;font-size:10px;">';
			str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">close</a>';
			str += '</p>';
			str += '<p class="RA_formTitle">';
			str += '<b>Site error.</b>';
			str += '</p>';
			str += RA_CtrlWin.RA.DisplayAll();
		break;
	//  ********************************************************************************
		case 'instemailprompt' :
			str += '<div id="RA_formError_instemailprompt" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';
//alert(RA_CtrlWin.RA.CurrentUser.ClassLogins.length);

			str += '<div id="RA_instemail_1">';
			if(RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail == ''){
				str += '<p class="RA_formText">Please enter your instructor\'s e-mail address. Providing this information will allow you to access materials your instructor wants to share with you.</p>';
			} else {
				str += '<p class="RA_formText">Please update your Instructor\'s email address.</p>';
			}
			str += '<p></p>';
			str += '<form name="RA_InstEmailPrompt" action="JavaScript:RA_CtrlWin.RA.RAif.app.goInstEmailPrompt();">';
			str += '<table width="100%" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td valign="top" width="10" class="RA_formField"><nobr><input maxlength="150" type="text" id="RA_sInstructorEmail" name="RA_sInstructorEmail" value="';
			str += RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail;
			str += '"/>&nbsp;&nbsp;</nobr></td>';
			str += '<td width="30" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.goInstEmailPrompt();">GO</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
			str += '<input type="submit" name="submit" style="position:absolute;top:-500px;left:-500px"/></form>';
			str += '<br/>';
			str += '</div>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.FormContinue();">CANCEL</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;		
	//  ********************************************************************************
		case 'checkinginstemail' :
			str += '<p class="RA_formText">checking ...</p>';
		break;
	//  ********************************************************************************
		case 'instemailprompted' :
/*
			setTimeout('RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes()', 5);
*/
			str += '<div id="RA_formError_quizprompt" class="RA_formError">'+ RA_CtrlWin.RA.RAif.vars['Error'] +'</div>';
			str += '<p class="RA_formText">';
			if (RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail=='') {
					str += 'No instructor e-mail address has been saved with your profile. If your instructor wishes to view your quiz results or share content with you, you will need to update your profile. You can do so now, or next time you visit.';
			} else {
					str += 'Congratulations. The e-mail address <b>'+ RA_CtrlWin.RA.CurrentUser.SiteLogins[RA_CtrlWin.RA.CurrentSite.ID].InstructorEmail +'</b> has been saved with your profile as your instructor\'s e-mail address. Now you can view shared content and your instructor will be able to view results on your quizzes and exercises. If you need to change the e-mail address, or other elements of your profile, you can do so by choosing review profile now, or the next time you visit.';
			}
			str += '</p>';
			str += '<table class="RA_buttons" border="0" cellpadding="0" cellspacing="0"><tr>';
			str += '<td width="70" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.QuizClassPromptedYes();">CONTINUE</a></td>';
			str += '<td width="60" class="RA_button"><a class="BFWj_PepperButton" href="JavaScript:RA_CtrlWin.RA.RAif.app.reGoQuizPrompt();">CHANGE</a></td>';
			str += '<td>&nbsp;</td>';
			str += '</tr></table>';
		break;
	//  ********************************************************************************
		default :
			str += '<p style="text-align:right;font-size:10px;">';
			str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.FormCancel()">close</a>';
			str += '</p>';
			if (RA_CtrlWin.RA.dev_check('any')) {
				str += '<p class="RA_formText"><b>';
				str += 'RA info';
				str += '</b></p>';
			}
			if (RA_CtrlWin.RA.CurrentUser!=null) {
				str += '<p class="RA_formText">';
				str += 'You are logged in as '+RA_CtrlWin.RA.CurrentUser.FName+' '+RA_CtrlWin.RA.CurrentUser.LName+' ('+RA_CtrlWin.RA.CurrentUser.Email+') ';
				str += '</p>';
				str += '<p class="RA_formText">';
				str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.RAXSLogout(true);">Log out here</a>';
				str += '</p>';
				if (RA_CtrlWin.RA.dev_check('any')) {
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'checkemail\',true);">Register</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'quizprompt\',true);">Quiz prompt (instructor email)</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'quizclassprompt\',true);">Quiz prompt (class)</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'checkcode\',true);">Enter activation code</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'cart\',true);">Cart</a>';
					str += '</p>';
				}
			} else {
				str += '<p class="RA_formText">';
				str += 'You are not logged in.  <a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'login\',true);">Log in here</a>';
				str += '</p>';
				if (RA_CtrlWin.RA.dev_check('any')) {
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.ShowLogin(\'checkemail\',true);">Register</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'quizprompt\',true);">Quiz prompt (instructor email)</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'quizclassprompt\',true);">Quiz prompt (class)</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'checkcode\',true);">Enter activation code</a>';
					str += '</p>';
					str += '<p class="RA_formText">';
					str += '<a href="JavaScript:RA_CtrlWin.RA.RAif.app.Reload(\'cart\',true);">Cart</a>';
					str += '</p>';
				}
			}
			str += RA_CtrlWin.RA.DisplayAll();
	}
	if (RA_CtrlWin.RA.dev_check('any')) {
	str += '<p id="tester" style="color:#ccc;font-size:10px;padding:0;margin:0;">testing: '+ RA_CtrlWin.RA.RAif.state.current() +'</p>';
	}
	str += '<br/><br/><div style="font-size:9pt;"><hr/>Having trouble? <a target="blank" href="http://www.bfwpub.com/newcatalog.aspx?page=support\\techsupport.html">Click here for technical support</a></div>'

	$('#RA_Page').html( str );

/*
    text: "LOG IN" 
,   url: "JavaScript:RA_CtrlWin.RA.RAif.app.goLogin();"
,   id: "BFWj_PepperButton_LOGIN"
,
 */
    $('.BFWj_PepperButton').BFWj_PepperButton({
        extra: 'style="float:none; text-align:center"'
    });
//alert('RA_CtrlWin.RA.RAif.app.DrawPage 2');
	RA_CtrlWin.RA.RAif.app.Resize();

//alert('RA_CtrlWin.RA.RAif.app.DrawPage 3');
//window.self.focus();
}



//  ********************************************************************************
// Utility functions
//  ********************************************************************************

RA_CtrlWin.RA.RAif.app.RAWSCleanInput = function (val, type) {
	switch (type) {
	case '' :
		val = RA_CtrlWin.RA.RAif.app.trim(val);
		break;
	default :
		val = val.replace('<','').replace('>','');
		val = RA_CtrlWin.RA.RAif.app.trim(val);
	}
	return val;
}

RA_CtrlWin.RA.RAif.app.IsValidEmail = function (email) {
	try {
		email = RA_CtrlWin.RA.RAif.app.trim(email);
		var valid1 = email.indexOf('.') > -1;
		var valid2 = email.indexOf('..') == -1;
		var valid3 = email.indexOf('@') > -1;
		var valid4 = email.indexOf('@') == email.lastIndexOf('@');
		var rexpStr = '[\!#$%^\*< >\(\)\+\\\\\&\\s]';
		var rexp = new RegExp( rexpStr,'gim');
		var valid5 = !rexp.test(email);
		var valid6 = true;
		valid6 = (email.indexOf("\'")>-1) ? false : true;
		var valid = valid1 && valid2 && valid3 && valid4 && valid5 && valid6;
		return valid;
	} catch(e) {
		return false;
	}
}

RA_CtrlWin.RA.RAif.app.trim = function (str, chars) {
	return RA_CtrlWin.RA.RAif.app.ltrim(RA_CtrlWin.RA.RAif.app.rtrim(str, chars), chars);
}

RA_CtrlWin.RA.RAif.app.ltrim = function (str, chars) {
	chars = chars || "\\s";
	return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}

RA_CtrlWin.RA.RAif.app.rtrim = function (str, chars) {
	chars = chars || "\\s";
	return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}



}
//CreateApp

//})
})(jQuery);


// BFWj_PepperButton
/*
 REQUIRES:
    jQuery
*/ 
(function($) {
    if (!$) 
    {
        throw new Error('jQuery required for BFWj.PepperButton')
    }
    else if (typeof $.fn.position !== 'function') 
    {
        throw new Error('jQuery position required for BFWj.PepperButton')
    }
    else
    {
/*
$(...).BFWj_PepperButton({
    text: "LOG IN" 
,   url: "JavaScript:RA_CtrlWin.RA.RAif.app.goLogin();"
,   id: ""
,   type: "primary"
,   block: false
,   size: "medium"
,   extra: "style='float:none; text-align:center'"
})
 */
        $.fn.BFWj_PepperButton = function( options ) {
            var settings = {
                    text: 'CLICK'
                ,   url: 'Javascript:alert("button clicked");'
                ,   id: ''
                ,   type: 'primary'
                ,   size: 'medium'
                ,   block: false
                ,   extra: ''
            };
            if (options) {
                $.extend( settings, options);
            }
            return this.each(function(){
	            var html, typStyle;
            	
	            if (settings.size == "" || settings.size == null) {
		            settings.size = "medium";
	            }
            	
	            if (settings.type == "link" || settings.type == "primary" || settings.type == "settingBlue") {
		            typStyle = "link";
	            } else {
		            typStyle = "setting";
	            }
            	
	            html = "<a class='BFWj_PepperButton BFWj_PepperButton_" + settings.size + " BFWj_PepperButton_" + settings.size + "_" + typStyle + "'";
	            // html = "<a class='squarebutton'";

	            if (settings.extra != "" && settings.extra != null) {
		            html += " " + settings.extra;	// ' i.e. " style='float:right'"
	            }
            	
	            if ($(this).attr('id') != "") {	
		            html += " id='" + $(this).attr('id') + "'";
	            } else if (settings.id != "" && settings.id != null) {	
		            html += " id='" + settings.id + "'";
	            }
            	
	            // Note that for javascript urls, *double* quotes need to be escaped
	            // We do this because in vb script, it's easier to code single quotes without escaping
	            if ($(this).attr('href') != "") {	
	                html += ' href="' + $(this).attr('href') + '"><span>';
	            } else {
	                html += ' href="' + settings.url + '"><span>';
	            }
	            html += $(this).html();

	            if (settings.type == "setting" || settings.type == "settingBlue") {
		            html += "<img class='starr' src='/BFW/pics/launch-arrow.gif' alt='' />";
	            }
            	
	            html += "</span></a>";
            	
	            if (settings.block == true) {
		            html = "<div class='buttonwrapper'>" + html + "</div><div style='clear: both;'></div>";
	            }
            	
	            $(this).replaceWith(html);
            });
        };
    }
})(jQuery);
