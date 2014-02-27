/**
* @projectDescription   Wrapper functions for using the ARGA Framework.
*
* Defines some wrapper functions to make it easy for content developers to use
* this interface without knowing anything about SCORM.
*
* @author   Pepper Williams
* @version  3.2.1
* @date     20 Feb 2014
*
* This version implements ARGA functions for the Platform X, 
* using Agilix's "Data Exchange Javascript API" (DEJS)
* See: http://solitude.agilix.com/docs/DEJS_Overview
* Note, though, that we have extensively altered the DEJS code for use in Platform X

Change log:

2013/06/24: Updated DEJS code to check in the "Url" parameter for enrollmentid, itemid, and approot, to deal with PX Proxy page issues
2013/07/02: Updated Get_Data_From_LMS function so that if section data is being retreived, it honors the cancel_initialization_alert argumment
2013/07/09: Fixed bug where grade of 0 was triggering argacomplete call
2013/09/27: Updated ajax call timeout from 15000ms to 30000ms
2013/10/10: Fixed grace period issue
2013/12/10: Numerous fixes and enhancements, including:
              - Use POST instead of JSONP/GET for sending data (DEJS putData) when possible
              - Improve progress indicator so that it works even when jquery is not available, and blocks the screen
                while save is occurring
              - When process fails, reliably notify the user, block the screen while a retry is attempted,
                and then warn the user again if the retry also fails
              - Add onbeforeunload event to attempt to stop users from leaving the page while save is occurring 
                (but see comments for limitations)
              - Remove code, which was never used, for using BH SCORM_API
2014/02/12: Support infinite grace period and Full credit on completion
2014/02/20: Record students minutes spent on the activity
**/

// minimize with:
// http://developer.yahoo.com/yui/compressor/#work

var ARGA_API = null;

var Url_Query = null;

var Local_Start_Time = null;
// object to hold ARGA constants and variables
var ARGA_VARS = function() {
	// extract get variables
	var GET_OBJ = new Object();
	var getA = window.location.search.substr(1).split('&');
	for (var i = 0; i < getA.length; ++i) {
		var getI = getA[i].split('=');
		if (getI.length == 2) {
			GET_OBJ[getI[0].toLowerCase()] = getI[1];
		} else {
			GET_OBJ[('un' + i)] = getA[i];
		}
	}


	return {

	/**
	* Parse the url search string into a PHP-like GET array id=value pairs get parsed to GET_FN('id')=value
	* single values get parsed to GET_FN('un1')=value.  e.g. 'home.html?blah&foo=bar' will be parsed to:
	*	GET_FN('un0') = 'blah'
	*	GET_FN('foo') = 'bar'
	* The function converts all property names to lower case, so that everything is case-insensitive
	* @param {String} property GET property whose value you want to retrieve
	* @param {Object} win Current window object
	*/
	GET_FN: function(property) {
		// Return the specified property's value
		return GET_OBJ[property.toLowerCase()];
	},

	ARGA_version: "3.2.0",
	ARGA_debug: true,
	ARGA_initialized: false

		// Remember: no comma after last item
	};	// end return for public vars and functions
}();


/**
* Initializes an ARGA session for the SCO
*
* Initialize_ARGA_Session_Callback() will be called when the initialization
* process is complete; the caller can define this function if it so chooses
*/
function Initialize_ARGA_Session(args) {
	ARGA_API = new Object();

	// establish the data array
	ARGA_API.data = new Array();

	// Call the AJAX function to get data for this activity/student; return its value
	return ARGA_Private_Fns.Get_Data_From_LMS(ARGA_API, args);
}

/**
* Sets data
* The SCO can get and save absolutely anything here. Or the SCO can use the more
* specific calls below for question responses and grades
* *NOTE* that data is not actually stored to the server until Save_ARGA_Data()
* is called.
* @param  {String}  key The name of the data value
* @param  {String}  value The data string itself
* @return {Boolean} true if successful
*/
function Set_ARGA_Data(key, value) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
		return false;
	}
	// See if the arbitrary data key already has an entry
	var arb_data_index = ARGA_API.arbitraryDataIndex[key];

	// if not, set arbitrary data key index to the next value, stored in ARGA_API
	if (arb_data_index == null) {
		arb_data_index = ARGA_API.next_arb_data_index;

		// add to key/index mapping
		ARGA_API.arbitraryDataIndex[key] = arb_data_index;

		// and update arbitrary data key index
		++ARGA_API.next_arb_data_index;
	}
	ARGA_Private_Fns.SetValue("cmi.comments_from_learner." + arb_data_index + ".location", key);
	ARGA_Private_Fns.SetValue("cmi.comments_from_learner." + arb_data_index + ".comment", value);

	// return success
	ARGA_Private_Fns.Report("Set_ARGA_Data successful: key=" + key + " / value=" + value);
	return true;
}

/**
* Retrieve previously-stored data
* @param  {String}  key The name of the data value
* @return {Boolean, NULL} true if successful, NULL if not
*/
function Get_ARGA_Data(key, learner_id) {
	if (!ARGA_VARS.ARGA_initialized) {
		return false;
	}

	// get designated learner's response if specified; otherwise the current user
	var api;
	if (learner_id != null) {
		api = ARGA_Private_Fns.Get_API_For_Learner(learner_id);
	} else {
		api = ARGA_API;
	}

	if (api == null) {
		return "";
	}

	// Some things are predefined
	// These values are retrieved from the server
	switch (key) {
		case "learner_name": return api.learner_name;
		case "learner_id": return api.learner_id;
		case "course_id": return api.course_id;
		case "user_rights": return api.user_rights;
		case "user_due_date": return api.user_due_date;
		case "due_date_has_passed": return api.due_date_has_passed;
	}

	// If we make it to here, it's not a predefined thing, so use GetValue
	var key_index = api.arbitraryDataIndex[key];
	return ARGA_Private_Fns.GetValue(api, "cmi.comments_from_learner." + key_index + ".comment");
}

/**
* Retrieve an array with data for all class members for the given key
*
* For this function to work, "retrieve_class_data:1" must have been sent in
* to Initialize_ARGA_Session()
*/
function Get_ARGA_Data_Class(key) {
	if (!ARGA_VARS.ARGA_initialized) {
		return [];
	}

	// if we didn't get class_info, return empty array
	if (ARGA_API.class_info == null) {
		return [];
	}

	var arr = new Array();

	for (var j = 0; j < ARGA_API.class_info.length; ++j) {
		// by default, fill in empty string for the learner
		arr[j] = "";

		// Some things are predefined
		// These values are retrieved from the server
		switch (key) {
			case "learner_name": arr[j] = ARGA_API.class_info[j].learner_name; continue;
			case "learner_id": arr[j] = ARGA_API.class_info[j].learner_id; continue;
			case "course_id": arr[j] = ARGA_API.course_id; continue;	// this is always the same
			// case "entry_id": arr[j] = ARGA_API.entry_id; continue;		// this is always the same
			case "user_rights": arr[j] = ARGA_API.class_info[j].user_rights; continue;
			case "user_due_date": arr[j] = ARGA_API.class_info[j].user_due_date; continue;
			case "due_date_has_passed": arr[j] = ARGA_API.class_info[j].due_date_has_passed; continue;
		}

		var id = "cmi.comments_from_learner." + ARGA_API.class_info[j].arbitraryDataIndex[key] + ".comment"
		arr[j] = ARGA_API.class_info[j].data[id];
	}

	// return the array
	return arr;
}


/**
* Submit a question response
* If an SCO uses this function to store info, BFW's Angel implementation will format it nicely
* in the report
*
* @param  {String}  questionNum	 Question number. There is no format specified for this; it can be, e.g. "1", "1.1", "Essay 1", etc.
* @param  {String}  questionType	Up to SCO to define; it will be displayed to the instructor, so make it readable (e.g. "multiple choice")
* @param  {String}  questionText	Text of question as presented to student (to be displayed to instructor when viewing results)
* @param  {String}  correctAnswer   Text of correct answer (if a MC question, write out the correct response; e.g. "Venus")
* @param  {String}  learnerResponse Student's response (if a MC question, write out the response given, e.g. "Mars")
* @param  {int}	 questionGrade   0-100 percentage, or -1 if can't be scored
* @param  {int}	 questionWeight  Percentage of the total grade for the activity. (Percentages have to be hard-coded into the SCO; not configurable by profs at this time.)
* @param  {String}  questionData		Optional parameter to store info needed to recreate student's experience, e.g. a random number seed
* @return {Boolean}				 true/false
*/
function Set_ARGA_Question_Response(
		questionNum,
		questionType,
		questionText,
		correctAnswer,
		learnerResponse,
		questionGrade,
		questionWeight,
		questionData
		) {

	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
		return false;
	}

	// If first argument is an object, get vars that way
	if (typeof arguments[0] == 'object') {
		var a = arguments[0];
		questionType = a.questionType;
		questionText = a.questionText;
		correctAnswer = a.correctAnswer;
		learnerResponse = a.learnerResponse;
		questionGrade = a.questionGrade;
		questionWeight = a.questionWeight;
		questionData = a.questionData;
		questionNum = a.questionNum;	// have to do this last
	}

	// if no correct answer given, make it an empty string
	if (correctAnswer == null) {
		correctAnswer = "";
	}

	// See if the questionNum already has an entry
	var scorm_index = ARGA_API.questionIndex[questionNum];

	// if not, set scorm_index to the next value, stored in ARGA_API
	if (scorm_index == null) {
		scorm_index = ARGA_API.next_scorm_index;

		// add to key/index mapping
		ARGA_API.questionIndex[questionNum] = scorm_index;

		// and update next_scorm_index
		++ARGA_API.next_scorm_index;
	}

	// now store the values in the appropriate places in the scorm data structure
	// question data goes in cmi.interactions.n.xxx
	var name_start = "cmi.interactions." + scorm_index + ".";

	// questionNum goes in id
	ARGA_Private_Fns.SetValue(name_start + "id", questionNum);

	// questionType goes in type="other" and displaytype=questionType
	ARGA_Private_Fns.SetValue(name_start + "type", "other");
	ARGA_Private_Fns.SetValue(name_start + "displaytype", questionType);

	// questionText goes in description
	ARGA_Private_Fns.SetValue(name_start + "description", questionText);

	// correctAnswer goes in "correct_responses.0.pattern"
	ARGA_Private_Fns.SetValue(name_start + "correct_responses.0.pattern", correctAnswer);

	// learnerResponse goes in learner_response
	ARGA_Private_Fns.SetValue(name_start + "learner_response", learnerResponse);

	// questionGrade goes in result, but has to be scaled to a decimal
	// BUT if grade is -1 (meaning not set; e.g. essay question), don't set this at all
	questionGrade = questionGrade * 1;
	if (isNaN(questionGrade)) {
		questionGrade = 0;
	} else if (questionGrade == -1) {
		questionGrade = "";
	} else {
		questionGrade = questionGrade / 100;
	}
	ARGA_Private_Fns.SetValue(name_start + "result", questionGrade);

	// questionWeight goes in weighting
	ARGA_Private_Fns.SetValue(name_start + "weighting", questionWeight);

	// questionData goes in tag
	ARGA_Private_Fns.SetValue(name_start + "tag", questionData);

	// return success
	return true;
}

/**
* Retrieve a previously-stored learnerResponse, indexed to questionNum
* @param  {String}  questionNum Index number
* @return {Boolean, NULL}	   true if success, otherwise NULL
*/
function Get_ARGA_LearnerResponse(questionNum, learner_id) {
	return ARGA_Private_Fns.GetQuestionData("learnerResponse", questionNum, learner_id);
}

/**
* Retrieve an array with learnerResponse's for all class members for the questionNum
*
* For this function to work, "retrieve_class_data:1" must have been sent in
* to Initialize_ARGA_Session()

PW 12/11/2012: Pulled in code from the LC implementation of arga_wrapper
*/
function Get_ARGA_LearnerResponse_Class(questionNum) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
		return [];
	}

	var arr = new Array();

	// if we didn't get class_info, return empty array
	if (ARGA_API.class_info == null) {
		return "";
	}

	for (var j = 0; j < ARGA_API.class_info.length; ++j) {
		// by default, fill in empty string for the learner
		arr[j] = "";
		// construct the scorm value id we want
		var id = 'cmi.interactions.' + ARGA_API.class_info[j].questionIndex[questionNum] + '.learner_response';
		arr[j] = ARGA_API.class_info[j].data[id];
	}

	// return the array
	return arr;
}

/**
* Retrieve previously-stored questionData, indexed to questionNum
* @param  {String}  questionNum Index number
* @return {String}  questionData, or empty string if it doesn't exist
*/
function Get_ARGA_QuestionData(questionNum, learner_id) {
	return ARGA_Private_Fns.GetQuestionData("questionData", questionNum, learner_id);
}

/**
* Retrieve previously-stored question grade, indexed to questionNum
* @param  {String}  questionNum Index number
* @return {Number}  the current grade, or -1 if the question can't be found
*/
function Get_ARGA_QuestionGrade(questionNum, learner_id) {
	// get the stored grade if there
	var g = ARGA_Private_Fns.GetQuestionData("questionGrade", questionNum, learner_id);

	// return -1 if not found or the grade if found
	if (g == "" || g == null) {
		return -1;
	} else {
		// convert to percentage
		return g * 100;
	}
}

/**
* Set a 0-100 percentage grade (e.g. 80 for 80%) for the entire activity
* @param  {String}  grade
* @return {Boolean}
*/
function Set_ARGA_Grade(grade) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
		return false;
	}

	// just set ARGA_API.grade here; we send it to the server when
	// Save_ARGA_Data() is called

	// If 'complete' has been set to 'no', the activity is not yet complete,
	if (Get_ARGA_Data('complete') == 'no') {
		// so stop now with grade set to -2
		ARGA_API.grade = -2;

	// If an actual grade was sent in, just set it
	} else if (grade != null) {
		ARGA_API.grade = grade;

	// Otherwise, calculate the grades from the question weights/scores
	} else {
		var totalPoints = 0;
		var totalWeight = 0;

		// Look for questions
		for (var i = 0; i < 1000; ++i) {
			// try to get the id for this "cmi.interactions"
			var id = ARGA_Private_Fns.GetValue(ARGA_API, "cmi.interactions." + i + ".id");
			if (id == "") {
				break;
			}

			// Only do anything if weight is not null
			// Note: questionWeight 0 could be used for extra credit
			var w = parseInt(ARGA_Private_Fns.GetValue(ARGA_API, "cmi.interactions." + i + ".weighting"));		// parseInt(null or string) = NaN
			if (!isNaN(w)) {
				// if questionWeight is -1, use 10 (so that we would treat everything equally)
				if (w == -1) {
					w = 10;
				}

				// get grade entered
				var questionGrade = ARGA_Private_Fns.GetValue(ARGA_API, "cmi.interactions." + i + ".result");

				// If there is no grade entered for the question, we assume it can't be graded
				if (questionGrade == null || questionGrade === "" || questionGrade == -1) {
					// if 'grade_partial' is set to 'yes', grade based on the other questions
					// otherwise, return an overall grade of -1, meaning "complete but no grade"
					if (Get_ARGA_Data('grade_partial') != 'yes') {
						ARGA_API.grade = -1;
						return true;
					}

				// Otherwise add questionGrade, weighted by weight, to totalPoints
				} else {
					// scale grade to percent
					questionGrade *= 100;
					
					totalPoints += (questionGrade * w);
					// also add weight in to total in this case
					totalWeight += w;
				}
			}
		}

		// Now we should have totalPoints and totalWeight; calculate the grade
		if (totalWeight == 0) {
			ARGA_API.grade = 0;
		} else {
			// it will already be a percentage
			ARGA_API.grade = Math.round(totalPoints / totalWeight);
		}
	}

	// return success
	return true;
}

/**
* Retrieve a previously-stored grade
* @return {int} grade
* PW 12/11/2012: I'm not sure if returning -1 is really appropriate here, but it shouldn't really hurt anything, and it's how it's always been
*/
function Get_ARGA_Grade(learner_id) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
		return -1;
	}

	// get designated learner's grade if specified; otherwise the current user
	var api;
	if (learner_id != null) {
		api = ARGA_Private_Fns.Get_API_For_Learner(learner_id);
	} else {
		api = ARGA_API;
	}

	// PW: Had "|| !api.grade before, but that can't be right, because "!0" returns true too
	// PW 2013/12/09: checking for empty string is appopriate though
	if (api == null || api.grade == "" || api.grade == null) {
		return -1;
	} else {
		return api.grade;
	}
}

/**
* Retrieve an array with grade for all class members for the activity
*
* For this function to work, "retrieve_class_data:1" must have been sent in
* to Initialize_ARGA_Session()
*/
function Get_ARGA_Grade_Class() {
	if (!ARGA_VARS.ARGA_initialized) {
		return [];
	}

	// if we didn't get class_info, return empty array
	if (ARGA_API.class_info == null) {
		return [];
	}

	var arr = new Array();

	for (var j = 0; j < ARGA_API.class_info.length; ++j) {
		arr[j] = ARGA_API.class_info[j].grade;
	}

	return arr;
}

/**
* Commit all data that has been stored using the Set_ functions defined here.
* @return {Boolean}
*/
function Save_ARGA_Data(args) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
		return false;
	}

	return ARGA_Private_Fns.Save_Data_To_LMS(args);
}

/**
* Get question data from another page, indexed by a "pageId".
* This is used in Portal for, e.g., retrieving data entered on a different activity
* for use in this activity.
* THIS FUNCTION IS NOT CURRENTLY IMPLEMENTED IN PX
* @return {String} Question data
*/
function Get_ARGA_QuestionData_For_PageId(pageId) {
	// initialize an API to hold the data from this other page
	var tempAPI = new Object();
	tempAPI.learner_id = ARGA_API.learner_id;
	tempAPI.course_id = ARGA_API.course_id;
	tempAPI.page_id = pageId;
	tempAPI.data = new Array();
	ARGA_Private_Fns.Get_Data_From_LMS(tempAPI);

	// The caller's Get_ARGA_QuestionData_For_PageId_Callback should be called when Get_Data_From_LMS is done
}

// ------------------------------------------------------------------
// Private functions
var ARGA_Private_Fns = function () {
	//default ajax timouts -- PW: moved into ARGA_Private_Fns namespace and separated for get, getsection, and put
	var ajax_timeout_getdata = 15000;
	var ajax_timeout_getsectiondata = 60000;
	var ajax_timeout_putdata = 15000;
	
	var save_in_progress = false;

	// PW: translate enrollment_rights into arga versions: 1_student, 2_course_assistant, 3_instructor
	// from http://dev.dlap.bfwpub.com/Docs/Enum/RightsFlags,
	// relevant flags to check are as follows:
	// if 0x1000000 ("GradeAssignment") is set, "User can grade assignments in a course or section"
	// so we assume the user is an instructor
	// if 0x01 ("Participate") is set, "The "student" bit: a user can submit assignments and exam attempts in a course or section."
	// (but we just assume it's a student if GradeAssignment isn't set)
	// for now we don't know how to define course assistant rights
	// Note: sample values I've seen are 552155348992 for instructor and 131073 for student
	function translate_user_rights(bhrights) {
		if ((bhrights & 0x1000000) > 0) {
			return "3_instructor";
		} else {
			return "1_student";
		}
	}
	
	function show_progress_indicator() {
		// if the ARGA_ajax_save_div is already there, just adjust its properties
		var d = document.getElementById("ARGA_ajax_save_div");
		if (d != null) {
			d.style.display = "block";

		// else insert it into the html
		} else {
			var div = document.createElement("div");
			div.setAttribute("id", "ARGA_ajax_save_div");
			div.innerHTML = "<div style='position:fixed; left:0px; top:0px; width:100%; height:100%; z-index:99999;'><div style='position:fixed; left:0px; top:0px; width:100%; height:100%; background-color:#fff; opacity: .7; filter:Alpha(Opacity=70);'></div><div style='position:fixed; left:0px; top:0px; width:100%; height:100%'><div style='margin-top:150px; margin-left:auto; margin-right:auto; width:160px; text-align:center; background-color:#000; opacity: 1; filter:Alpha(Opacity=100); border:1px solid #000; border-radius:8px; padding:10px; color:#fff; font-weight:bold; font-family:Verdana, sans-serif; font-size:14px;'><img border='0' src='http://ajax.aspnetcdn.com/ajax/jquery.mobile/1.1.0/images/ajax-loader.gif' width='20' height='20' align='absbottom'> Saving data...</div></div>";

			document.body.appendChild(div);
		}
	}
	
	// PUBLIC VARS AND FUNCTIONS
	return {
	
	/**
	* Calculate due date after grace period.
	* If grace period is not set, original due date will be returned.
	* @param  {String} dueDate, should be in this format: yyyy-mm-dd hh:mm:ss\
	* @param {String} gracePeriod, grace period in minute
	* @return {Object} due date with grace period.
	*/
	CalculateDueDateGrace: function(dueDate, gracePeriod){
		//If due date is empty, return nothing
		if (!dueDate)
			return null;
			
		if (Url_Query && Url_Query['dueDate'] > 0){
			dueDate = new Date((Url_Query['dueDate']));
		}
		//If Date.parse() fails to parse string to date object, do manual parse
		else if (isNaN(Date.parse(dueDate)))
			dueDate = ARGA_Private_Fns.ParseDate(dueDate);
		else 
			dueDate = new Date(dueDate);
			
		//If grace period is 0 or empty, return original due date
		if (!gracePeriod)
			return dueDate;
		//If grace period is -1(infinite), set due date to 1/1/2999
		else if (gracePeriod == -1){
			var dueDateGrace = new Date('1/1/2999');
			return dueDateGrace;
		}
		else {
		//Else, calculate due date after grace period
			var dueDateGrace;
			try{
				dueDateGrace = new Date(dueDate.getTime() + (gracePeriod * 60000));
			} catch(e) {
				ARGA_Private_Fns.ReportErrorToPx('CalculateDueDateGrace() error : dueDate = ' + dueDate + ', gracePeriod = ' + gracePeriod);
			}
			return dueDateGrace;
		}
	},
	//Parse due date from string and return Date object
	ParseDate: function(dueDateTxt) {
		var parts = dueDateTxt.match(/(\d+)-(\d+)-(\d+) (\d+):(\d+):(\d+)/);
		return new Date(parts[1], parts[2] -1, parts[3], parts[4], parts[5], parts[6]);
	},
	Get_API_For_Learner: function(learner_id) {
		for (var j = 0; j < ARGA_API.class_info.length; ++j) {
			if (ARGA_API.class_info[j].learner_id == learner_id) {
				return ARGA_API.class_info[j];
			}
		}
		return null;
	},

	// get a value out of the API, using the appropriate method
	GetValue: function(API, name, obj) {
		// use dejs_data if no explicit object is supplied
		if (obj == null && API.dejs_data != null) {
			for (var i = 0; i < API.dejs_data.length; ++i) {
				var d = API.dejs_data[i];
				if (d.name == name) {
					return d.value;
				}
			}
			// if we make it through the loop we couldn't find the data, so return "" below
		} else if (obj != null){
			for (key in obj) {
				if (key == name) {
					return obj[key];
				}
			}
		}

		// if we make it to here return empty string
		return "";
	},

	GetQuestionData: function(which, questionNum, learner_id) {
		if (!ARGA_VARS.ARGA_initialized) {
			ARGA_Private_Fns.ReportErrorToPx("Arga not initialized");
			return "";
		}

		// get designated learner's response if specified; otherwise the current user
		var api;
		if (learner_id != null) {
			api = ARGA_Private_Fns.Get_API_For_Learner(learner_id);
		} else {
			api = ARGA_API;
		}

		if (api == null) {
			return "";
		}

		// See if the questionNum has an entry
		var scorm_index = api.questionIndex[questionNum];

		// if so, return the response
		if (scorm_index != null) {
			// construct the scorm value id we want
			var id = "cmi.interactions." + scorm_index;
			if (which == 'learnerResponse') {
				id += ".learner_response";
			} else if (which == 'questionData') {
				id += ".tag";
			} else if (which == 'questionGrade') {
				id += ".result";
			}

			return ARGA_Private_Fns.GetValue(api, id);

		// otherwise return empty string
		} else {
			return "";
		}
	},

	// note that you can only set values on the "main" api (ARGA_API)
	SetValue: function(name, value) {
		// make sure we only set each name once, so go through all existing items
		for (var i = 0; i < ARGA_API.dejs_data.length; ++i) {
			var d = ARGA_API.dejs_data[i];

			// if this item has the given name,
			if (d.name == name) {

				// set to the new value
				d.value = value;

				// PW 2013/12/04: mark this item as "dirty" so that we send it to the server when Save_ARGA_Data is next called
				d.dirty = true;

				return;
			}
		}
		// If we get to here, the key isn't already there, and i = ARGA_API.data.length
		// set to the new key/value

		var ind = ARGA_API.dejs_data.length;

		d = ARGA_API.dejs_data[ind] = new Object();
		d.name = name;
		d.value = value;
		// mark this item as "dirty" so that we know we shouldn't send it to the server when Save_ARGA_Data is next called
		d.dirty = true;
	},

	/**
	* Get data using LMS-specific method
	* @param  {Object} ARGA API object
	*/
	// Receives an API object (ARGA_API for the main page, but could be a
	// different one from Get_ARGA_QuestionData_For_PageId)
	Get_Data_From_LMS: function(tempAPI, args) {
		// push tempAPI onto args so it'll get passed through to the callback fn one way or the other below
		if (args == null) {
			args = new Object();
		}
		args.tempAPI = tempAPI;

		// initialize DEJS
		var result = DEJS_API.initialize();
		if (!result) {
			ARGA_Private_Fns.ReportErrorToPx("DEJS_API failed to initialize");
			return false;
		}

		// if flags are set, handle section data
		if (args.retrieve_class_data == true || args.retrieve_class_data == '1') {
			if (args.retrieve_class_data_rights != null) {
				var sectionDataArgs = new Object() ;
				sectionDataArgs.tempAPI = tempAPI;
				sectionDataArgs.timeout = ajax_timeout_getsectiondata;
				sectionDataArgs.callback = ARGA_Private_Fns.Get_Section_Data_AJAX_Callback;
				// PX-1685: Allow activity to cancel initialization alert for section data
				sectionDataArgs.cancel_initialization_alert = args.cancel_initialization_alert;
				// Added time track functionality
				sectionDataArgs.dueDateTimeTrackConfig = args.dueDateTimeTrackConfig;

				var sectionDataResult = DEJS_API.getSectionData(sectionDataArgs);
				if (!sectionDataResult) {
					ARGA_Private_Fns.ReportErrorToPx("DEJS_API getSectionData failed to run");
					return false;
				}
			} else {
				// push additional parameters into args, then use it for the options in DEJS_API.getData
				args.callback = ARGA_Private_Fns.Get_Data_From_LMS_Callback;
				args.timeout = ajax_timeout_getdata;
				result = DEJS_API.getData(args);
				if (!result) {
					ARGA_Private_Fns.ReportErrorToPx("DEJS_API getData failed to run");
					return false;
				}
			}
		} else {
			// push additional parameters into args, then use it for the options in DEJS_API.getData
			args.callback = ARGA_Private_Fns.Get_Data_From_LMS_Callback;
			args.timeout = ajax_timeout_getdata;
			result = DEJS_API.getData(args);
			if (!result) {
				ARGA_Private_Fns.ReportErrorToPx("DEJS_API getData failed to run");
				return false;
			}
		}

		// if we make to here, initialization appears to have been successful,
		// so return true
		return true;
	},

	/**
	* Callback for Get_Data_From_LMS.  Format is specified by DEJS
	*/
	Get_Data_From_LMS_Callback: function(args, success, data) {
		if (!success) {
			ARGA_Private_Fns.ReportErrorToPx("DEJS_API getData's ajax call failed");
			return false;
		}

		// if we make it to here we've got the SCORM data one way or the other

		var tempAPI = args.tempAPI;

		// receive data if it came in (it might be null, which is fine)
		tempAPI.dejs_data = data;

		
		// record user info
		tempAPI.learner_id = ARGA_Private_Fns.GetValue(tempAPI, "bh.user_id");
		tempAPI.learner_name = ARGA_Private_Fns.GetValue(tempAPI, "bh.user_display");
		tempAPI.course_id = ARGA_Private_Fns.GetValue(tempAPI, "bh.course_id");
		tempAPI.user_rights = translate_user_rights(ARGA_Private_Fns.GetValue(tempAPI, "bh.enrollment_rights"));

		// record timezone-adjusted user due date and whether due date has passed or not
		tempAPI.user_due_date = ARGA_Private_Fns.GetValue(tempAPI, "bh.item_due_date");
		// if due date is passed from url query, parse it
		if (Url_Query && Url_Query['dueDate']!= null) {
			Url_Query['dueDate'] = Url_Query['dueDate']*1;
		} else {
			Url_Query['dueDate'] = null;
		}
		// record grace period and calculate due date after grace period
		tempAPI.user_grace = ARGA_Private_Fns.GetValue(tempAPI, "bh.custom.duedategrace");
		tempAPI.user_due_date_grace = ARGA_Private_Fns.CalculateDueDateGrace(tempAPI.user_due_date, tempAPI.user_grace);
		// record submission grade action
		tempAPI.submission_grade_action = ARGA_Private_Fns.GetValue(tempAPI, "bh.custom.submissiongradeaction");
		
		// User's current grade for the activity
		var scorm_grade = ARGA_Private_Fns.GetValue(tempAPI, "cmi.score.scaled");
		// if this is an empty string...
		if (scorm_grade == "") {
			// if cmi.exit is empty, then the user hasn't started the activity, and we set grade to ""
			var exit = ARGA_Private_Fns.GetValue(tempAPI, "cmi.exit");
			if (exit == "") {
				tempAPI.grade = "";
			
			// else ...
			} else {
				// value depends on cmi.completion_status...
				var completion_status = ARGA_Private_Fns.GetValue(tempAPI, "cmi.completion_status");
				
				// if it's "complete", grade should be coded as -1
				if (completion_status == "completed") {
					tempAPI.grade = "-1";
				
				// if it's "incomplete", grade should be coded as -2
				} else if (completion_status == "incomplete") {
					tempAPI.grade = "-2";
				
				// otherwise leave it as an empty string (in theory this shouldn't happen)
				} else {
					tempAPI.grade = "";
				}
			}
		
		// else we have a grade
		} else {
			tempAPI.grade = scorm_grade * 100;	// SCORM is 0-1; ARGA uses 0-100
		}
		
		ARGA_Private_Fns.SetDueDateInfo(args, tempAPI);
		/*
		Question responses are stored in cmi.interactions.n.x, and other data is stored in arga.x

		If the "traditional" SCORM method is used, we have no idea ahead of time what will be stored in arga.x,
		but it's easy enough to look them up directly here.

		For question responses, we find the mappings between n (in cmi.interactions.n) and the activity-defined
		questionNum when the SCORM data is first retrieved, then look up the rest of the question data when requested.
		*/

		// we shouldn't ever have more than 1000 questions in an activity; limiting the loop like this
		// guarantees we don't fall into an infinite loop
		tempAPI.questionIndex = new Object();
		for (var i = 0; i < 1000; ++i) {
			// try to get the id for this "cmi.interactions"
			var id = ARGA_Private_Fns.GetValue(tempAPI, "cmi.interactions." + i + ".id");
			if (id == "") {
				break;
			}
			// if we get to here we have an id, so record it
			// we use an associative array mapping the id onto the "n" in "cmi.intearctions.n.id"
			tempAPI.questionIndex[id] = i;
		}

		// store the next scorm_index to use in tempAPI
		tempAPI.next_scorm_index = i;

		// using cmi.comments_from_learner.n.comment binding to store arbitrary data
		// key/value pairs, e.g. "foo":"bar"

		tempAPI.arbitraryDataIndex = new Object()
		var cs_from_l = ARGA_Private_Fns.GetValue(tempAPI, "cmi.comments_from_learner._count");
		if (cs_from_l == "" || parseFloat(cs_from_l) > 0) {
			for (var x = 0; x < 1000; ++x) {
				// try to get the key value
				var data_key = ARGA_Private_Fns.GetValue(tempAPI, "cmi.comments_from_learner." + x + ".location");
				if (data_key == "") {
					break;
				}

				// if we get to here we have an id, so record it
				// we use an associative array mapping the id onto the "n" in "cmi.intearctions.n.id"
				tempAPI.arbitraryDataIndex[data_key] = x;
			}
			// store the next arb_data_index to use in tempAPI
			tempAPI.next_arb_data_index = x;
		} else {
			// store the next arb_data_index as 0
			tempAPI.next_arb_data_index = 0;
		}

		ARGA_VARS.ARGA_initialized = true;

		ARGA_Private_Fns.Report("Initialize_ARGA_Session successful");

		// show an initialization alert, unless the activity has specifically cancelled it
		if (args.cancel_initialization_alert != true && args.cancel_initialization_alert != 1) {
			var msg = "";
			// PW: if grade is "", there *is* no grade, so we have to check for this
			if (tempAPI.due_date_has_passed == 0 && (tempAPI.grade != "" && parseFloat(tempAPI.grade) >= 0)) {
				msg += 'You have completed this activity. You may change your answers and re-submit if you like.';
			} else if (tempAPI.due_date_has_passed == 1 && (tempAPI.grade == "" || parseFloat(tempAPI.grade) < 0)) {
				msg += 'The due date for this assignment (' + tempAPI.user_due_date + ') has now passed.  You may review the materials in the activity, but you will not receive a grade for submitting answers.';
			} else if (tempAPI.due_date_has_passed == 1 && parseFloat(tempAPI.grade) >= 0) {
				msg += 'You have completed this activity. The due date for this assignment (' + tempAPI.user_due_date + ') has now passed.  You may review the materials in the activity, but further submissions will not be recorded.';
			}
			if (msg != "") {
				alert(msg);
			}
		}
		
		// register an onbeforeunload event to try to prevent users from leaving the page while a save is occurring
		// This will be effective if the user closes the entire LaunchPad window, but won't have any effect if the user clicks "Home" or "Next"
		window.onbeforeunload = ARGA_Private_Fns.OnBeforeUnload;

		// call application-defined callback function if there
		if (window.Initialize_ARGA_Session_Callback != null) {
			Initialize_ARGA_Session_Callback(true);
		}
		// Date.now() is not supported in IE7 & IE8
		Local_Start_Time = Date && Date.now? Date.now() : new Date().getTime();
		return true;
	},

	/*
	 * Callback function from getSectionData
	 * @param args object containing temp api
	 * @param success boolean true if getSectionData succeeds
	 * @param data array of objects
	 */
	Get_Section_Data_AJAX_Callback: function(args, success, data) {
		if (!success) {
			ARGA_Private_Fns.ReportErrorToPx("DEJS_API getData's ajax call failed");
			return;
		}
		// if we make it to here we've got the SCORM data one way or the other

		var tempAPI = args.tempAPI;

		// receive data if it came in (it might be null, which is fine)
		tempAPI.dejs_class_data = data;

		// create class_data array if necessary
		if (tempAPI.class_info == null) {
			tempAPI.class_info = new Array();
		}


		for(var dataIndex = 0; dataIndex < data.length; dataIndex++){
			// only students who have completed the activity are in the array
			// this represents one student record
			var studentResponseObject = data[dataIndex];

			// object to push on to ARGA_API.class_info array
			var info = new Object();


			/*
			 * id {String} Student's enrollment ID. If the requester is a student, this property is omitted.
			 * first {String} Student's first name. If the requester is a student, this property is omitted.
			 * last {String} Student's last name. If the requester is a student, this property is omitted.
			 * score {String} A real number (typically between 0 and 1) that is the student's score.
			 * scorm {Array} Array of objects containing name and value properties for all other scorm variables set
			 * 	by the student. The objects are in the form [{'name':'value'},…].
			 * 	For example: [{'cmi.interactions.0.id':'q1'},{'cmi.interactions.0.type','other'}]
			 */
			// record user info
			try {
				info.learner_id = studentResponseObject.id;
				info.learner_name = studentResponseObject.first + " " + studentResponseObject.last;
				info.course_id = ARGA_Private_Fns.GetValue(tempAPI, "bh.course_id");
				info.user_rights = translate_user_rights(ARGA_Private_Fns.GetValue(tempAPI, "bh.enrollment_rights"));

				// record timezone-adjusted user due date and whether due date has passed or not
				info.user_due_date = ARGA_Private_Fns.GetValue(info, "bh.item_due_date");
				// due_date_has_passed doesn't really matter for class data
				info.due_date_has_passed = 0;

				// User's current grade for the activity
				var scorm_grade = studentResponseObject.score;
				// if this is an empty string, grade should be an empty string
				if (scorm_grade == "") {
					info.grade = "";
				} else {
					info.grade = scorm_grade * 100;	// SCORM is 0-1; ARGA uses 0-100
				}

				info.data = studentResponseObject.scorm;

				/*
				Question responses are stored in cmi.interactions.n.x, and other data is stored in arga.x

				If the "traditional" SCORM method is used, we have no idea ahead of time what will be stored in arga.x,
				but it's easy enough to look them up directly here.

				For question responses, we find the mappings between n (in cmi.interactions.n) and the activity-defined
				questionNum when the SCORM data is first retrieved, then look up the rest of the question data when requested.
				*/

				// we shouldn't ever have more than 1000 questions in an activity; limiting the loop like this
				// guarantees we don't fall into an infinite loop
				info.questionIndex = new Object();
				for (var i = 0; i < 1000; ++i) {
					// try to get the id for this "cmi.interactions"
				   // var id = studentResponseObject.scorm['cmi.interactions.' + i + '.id'];
					var id = ARGA_Private_Fns.GetValue(null, 'cmi.interactions.' + i + '.id', studentResponseObject.scorm);
					if (id == "") {
						break;
					}
					// if we get to here we have an id, so record it
					// we use an associative array mapping the id onto the "n" in "cmi.intearctions.n.id"
					info.questionIndex[id] = i;
				}

				// store the next scorm_index to use in info
				info.next_scorm_index = i;

				// using cmi.comments_from_learner.n.comment binding to store arbitrary data
				// key/value pairs, e.g. "foo":"bar"

				info.arbitraryDataIndex = new Object()
				//var cs_from_l = studentResponseObject.scorm['cmi.comments_from_learner._count'];
				var cs_from_l = ARGA_Private_Fns.GetValue(null, 'cmi.comments_from_learner._count', studentResponseObject.scorm);
				if (cs_from_l == "" || parseFloat(cs_from_l) > 0){
					for (var x = 0; x < 1000; ++x) {
						// try to get the key value
						//var data_key = studentResponseObject.scorm['cmi.comments_from_learner.' + x + '.location'];
						var data_key = ARGA_Private_Fns.GetValue(null, 'cmi.comments_from_learner.' + x + '.location', studentResponseObject.scorm);
						if (data_key == "") {
							break;
						}

						// if we get to here we have an id, so record it
						// we use an associative array mapping the id onto the "n" in "cmi.intearctions.n.id"
						info.arbitraryDataIndex[data_key] = x;
					}
					// store the next arb_data_index to use in info
					info.next_arb_data_index = x;
				} else {
					// store the next arb_data_index as 0
					info.next_arb_data_index = 0;
				}
				tempAPI.class_info.push(info);
			} catch(e){
				console.log('error Get_Section_Data_AJAX_Callback(): ' + e.message? e.message : e);
				ARGA_Private_Fns.Report("Get section data callback error:" +  e.message);
			};
		}

		// push additional parameters into args, then use it for the options in DEJS_API.getData
		args.callback = ARGA_Private_Fns.Get_Data_From_LMS_Callback;
		args.timeout = ajax_timeout_getdata;
		result = DEJS_API.getData(args);
		if (!result) {
			ARGA_Private_Fns.Report("DEJS_API getData failed to run");
			return;
		}
	},

	/**
	* Saves data using whichever method we have at our disposal
	*/
	Save_Data_To_LMS: function (args) {
		// all data will already have been stored to scorm or dejs by SetValue, except the final grade

		// also set cmi.completion_status appropriately
		// http://scorm.com/scorm-explained/technical-scorm/run-time/run-time-reference/
		// cmi.completion_status ("completed", "incomplete", "not attempted", "unknown", RW) Indicates whether the learner has completed the SCO
		
		// grade will be -2 iff Set_ARGA_Data('complete') is explicitly set to "no"
		// also assume that if grade isn't set at all (e.g., responses were saved but not submitted)
		// , this is what we want
		if (ARGA_API.grade == -2 || ARGA_API.grade === "" || ARGA_API.grade == null) {
			ARGA_Private_Fns.SetValue("cmi.completion_status", "incomplete");
		
		// grade will be -1 if Set_ARGA_Grade has been called (meaning the activity tried to set a grade)
		// but something was not-auto-gradeable.  So in this case the item *is* complete, but we don't have an actual grade to set
		} else if (ARGA_API.grade == -1) {
			ARGA_Private_Fns.SetValue("cmi.completion_status", "completed");
		
		// else we have a grade
		} else {
			// if the due date has passed, *don't* store an updated grade
			// (note: the server wouldn't record the updated grade anyway)
			if (ARGA_API.due_date_has_passed != 1) {
				// SCORM is 0-1; ARGA uses 0-100
				if (ARGA_API.submission_grade_action == 'Full_Credit') {
					ARGA_Private_Fns.SetValue("cmi.score.scaled", 1);
				} else {
					ARGA_Private_Fns.SetValue("cmi.score.scaled", ARGA_API.grade / 100);
				}
			}
			
			// For ARGA activities we assume that if a grade has been calculated and Set_ARGA_Data('complete') is not "no"
			// then the activity should be considered complete.
			ARGA_Private_Fns.SetValue("cmi.completion_status", "completed");
		}

		// Store session duration, this is neccessary to get minutes spent show up in the gradebook
		var trackMinutesSpent = Url_Query && Url_Query['track'] == 'true';
		if (Local_Start_Time != null && trackMinutesSpent) {
			var currentTime = Date && Date.now? Date.now() : new Date().getTime();
			var timeSpentInSeconds = Math.round((currentTime - Local_Start_Time) / 1000);
			ARGA_Private_Fns.SetValue('cmi.session_time', 'PT' + timeSpentInSeconds + 'S');
		}
		// DC we for most LMS SCORM implementations, we need to set cmi.exit to "suspend"
		// possible values for cmi.exit are timeout, suspend, logout, normal, and empty string
		// If the cmi.exit is set to "normal", "logout","time-out" or "" (empty characterstring)
		// then the SCOs learner attempt ends. The SCOs Run-Time
		// Environment data model element values of the current learner session will NOT
		// be available if the SCO is relaunched
		ARGA_Private_Fns.SetValue("cmi.exit", "suspend");

		// if we got a "show_progress:true" argument, block the screen
		if (args != null && args.show_progress == true) {
			show_progress_indicator();
		}

		// call the DEJS putData function
		// PW 2013/12/04: we would ideally pull out the specific set of data we want to save -- any "dirty" data
		// but unfortunately "The putData call overwrites any previously existing variables, so the activity should include in the putData call all data retrieved during getData." (http://solitude.agilix.com/docs/DEJS_Overview)
		var data_to_store = new Array();
		for (var i = 0; i < ARGA_API.dejs_data.length; i++) {
			var d = ARGA_API.dejs_data[i];
			// if (d.dirty == true) {
				data_to_store.push(d);
			//}
		}

		DEJS_API.putData({
			callback: ARGA_Private_Fns.Save_Data_To_LMS_Callback
			, timeout: ajax_timeout_putdata
			, data: data_to_store
			, retry: false
		});
		
		// note that we're trying to save, so that if user tries to close window during save they'll be warned
		// (this will be reset in the callback function)
		save_in_progress = true;

		// Assume things will go OK from here...
		return (true);
	},

	Save_Data_To_LMS_Callback: function(options, success, result_message) {
		// options should be the object that was originally sent to putData in the previous function
		
		// hide the ARGA_ajax_save_div if it was there
		var d = document.getElementById('ARGA_ajax_save_div');
		if (d != null) {
			d.style.display = 'none';
		}
		
		// note that save has finished (whether successful or not)
		save_in_progress = false;
		
		// if ajax call was unsuccessful...
		if (!success) {
			ARGA_Private_Fns.Report("Save_Data_To_LMS error:");
			ARGA_Private_Fns.Report(result_message);

			// if options.retry if false, we just tried for the first time and failed, so try again
			if (options.retry == false) {
				// LOG RESULT???
				
				// try to show progress indicator now whether the app asked for it or not
				show_progress_indicator();
				
				// re-set save_in_progress to true
				save_in_progress = true;
				
				// call putData again
				options.retry = true;
				// If this activity is in the same host as its parent
				if (JSONP.sameHost() && parent.sessionKeepAlive) {
					parent.sessionKeepAlive();
					setTimeout(function(){
						DEJS_API.putData(options);
					}, 1000);
				} else {
					DEJS_API.putData(options);
				}

				// note that we show the alert after initiating the retry, so that we start trying to resave
				// while the user is reading the alert. Also note that we show an alert here, not a confirm --
				// we don't give the user the option of *not* trying again (that would be too confusing)
				//alert("We encountered an error when trying to save your activity data. Please be patient while we attempt to re-save the data.");	
				
				return;

			// else we've already retried, so tell the user to try again later
			} else {
				// LOG RESULT???
				alert("We were again unable to save your activity data. This may be due to a poor internet connection. Try refreshing your browser window and attempting the activity again, or try again later.\n\nIf you encounter this message consistently and report the incident to technical support, please pass on the following information:\n\nError message: " + result_message);
				return;
			}
		}

		// else call was successful
		// PW 2013/12/04: mark everything as not dirty anymore
		for (var i = 0; i < ARGA_API.dejs_data.length; i++) {
			ARGA_API.dejs_data[i].dirty = null;
		}

		// if activity is complete (see Save_Data_To_LMS)...
		// PW 2013/07/09: fix bug where grade of 0 was triggering argacomplete call
		if (ARGA_API.grade !== "" && ARGA_API.grade != null && ARGA_API.grade*1 >= -1) {
			// ... then call the easyXDM argacomplete function, to tell the platform that the item is complete
			try {
				// PX-9524, call argacomplete only when the grade is different from the last call
				if (ARGA_API.previousGrade == null || ARGA_API.previousGrade !== ARGA_API.grade) {
					arga_rpc.argacomplete(ARGA_API.grade);
					ARGA_Private_Fns.Report("called rpc.argacomplete; grade=" + ARGA_API.grade);
					ARGA_API.previousGrade = ARGA_API.grade;
				}
			} catch(e) {console.log('error Save_Data_To_LMS(): ' + e.message? e.message : e);}
		}

		// call application-defined callback if defined
		if (window.Save_ARGA_Data_Callback) {
			Save_ARGA_Data_Callback(true);
		}
	},

	/**
	* Reports AGRA activity if debugging is on
	* @param {String} s Report message.
	*/
	Report: function(s) {
		if (ARGA_VARS.ARGA_debug) {
			try {
				console.log(s);
			} catch(e){console.log('error Report(): ' + e.message? e.message : e);}
		}

		// if there is a div on the page with id "ARGA_debug_div", put the string there too
		var elm = document.getElementById("ARGA_debug_div");
		if (elm) {
			elm.innerHTML += "<div style='border-top:1px solid #666; padding-top:3px; margin-top:3px'>" + s + "</div>";
		}
	},
	
		/**
	* Reports error to platform x
	* @param {String} s Report message.
	*/
	ReportErrorToPx: function(s) {
		ARGA_Private_Fns.Report(s);
		if (JSONP.sameHost() && parent.PxPage){
			$.post(parent.PxPage.Routes.log_javascript_errors, {errorName: 'Arga Activity Error', errorMessage: s});
		}
	},
	/**
	* Set due date related information
	* 
	*/
	SetDueDateInfo: function(args, api) {
		var timeTrackConfig = args.dueDateTimeTrackConfig;
		var currentTime;
		if (timeTrackConfig != null && !!timeTrackConfig.startTime){
			currentTime = timeTrackConfig.startTime;
		} else if (Url_Query && Url_Query['startTime']){
			currentTime = Url_Query['startTime'] * 1;
		
		} else{
			currentTime = new Date().getTime();
		}
		var arr;
		var month, day, hour, minute, year;
		// if due date is passed from url query in unix time format
		if (Url_Query && Url_Query['dueDate'] > 0) {
			var dueDate = new Date(Url_Query['dueDate']);
			month = dueDate.getMonth();
			day = dueDate.getDate();
			hour = dueDate.getHours();
			minute = dueDate.getMinutes();
			year = dueDate.getFullYear();
			
			if (Url_Query['dueDate'] < currentTime && (api.user_due_date_grace == null || isNaN(api.user_due_date_grace) || (api.user_due_date_grace < currentTime))) {
				api.due_date_has_passed = 1;
			} else {
				api.due_date_has_passed = 0;
			}
		} else if ((arr = api.user_due_date.match(/(\d\d\d\d)-(\d\d)-(\d\d) (\d\d):(\d\d):(\d\d)/)) != null) {
			// calculate due_date_has_passed
			// due date will come in as 2013-01-06 23:59:00
			// unfortunately, we don't know what time zone that's in, so we hope it's the same as the student's
			// the server will not accept a submission after the due date in any case, so worst case is that
			// the student thinks he's submitting but it doesn't work

			year = arr[1]*1;
			month = arr[2]*1-1;	// transform 01-12 to 0-11
			day = arr[3]*1;
			hour = arr[4]*1;
			minute = arr[5]*1;
			var seconds = arr[6]*1;
			var milliseconds = 0;

			var d = new Date(year, month, day, hour, minute, seconds, milliseconds);
			// if due date is earlier than now, then due date has passed
			if (d.getTime() < currentTime && (api.user_due_date_grace == null || isNaN(api.user_due_date_grace) || (api.user_due_date_grace < currentTime))) {
				api.due_date_has_passed = 1;
			
			// otherwise due date has not passed.
			} else {
				api.due_date_has_passed = 0;
			}

		// if we can't parse the date string, assume the due date hasn't passed.
		} else {
			api.due_date_has_passed = 0;
		}
		if (year == 9999) {
			api.user_due_date = '';
		} else if (month != null){
			// also reformat the due date so it'll be prettier to display to the user
			var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
			var nd = months[month] + " " + day + " at ";
			if (hour < 12) {
				if (hour == 0) hour = 12;
				nd += hour + ":" + (minute < 10? '0': '') + minute + " AM";
			} else {
				nd += hour + ":" + (minute < 10? '0': '') + minute + " PM";
			}
			api.user_due_date = nd;
		}
		var dueDateTime = api.user_due_date_grace == null? NaN : api.user_due_date_grace.getTime();
		// If 1) time track configuation is not null
		//    2) due date has not passed
		//    3) activity is assigned
		//    4) student hasn't completed the activity yet
		// then we initialize time track
		if (timeTrackConfig && !api.due_date_has_passed && !isNaN(dueDateTime) && api.grade !== 100) {
			if (timeTrackConfig.dueTimeExpired) {
				var alertTimer = dueDateTime - currentTime;
				var alertCallback = function () {
					ARGA_Private_Fns.AlertUserDueDateHasPassed(timeTrackConfig.dueTimeExpired.showAlert, api);
					if(timeTrackConfig.alertCallback && typeof(timeTrackConfig.alertCallback) === 'function')
						timeTrackConfig.alertCallback();
				}
				ARGA_Private_Fns.SetUpTimeTrack(alertTimer, alertCallback) ;
			}
			
			if (timeTrackConfig.dueTimeReminder) {
				var remainingTime = dueDateTime - currentTime;
				var remainderTimer = remainingTime - 600000;
				var remainderCallback = function () {
					ARGA_Private_Fns.AlertUserDueDateSoonPass(timeTrackConfig.dueTimeReminder.showAlert, remainingTime < 600000? remainingTime: 600000);
					if(timeTrackConfig.reminderCallback && typeof(timeTrackConfig.reminderCallback) === 'function')
						timeTrackConfig.reminderCallback();
				}
				ARGA_Private_Fns.SetUpTimeTrack((remainingTime < 600000? 100: remainderTimer), remainderCallback) ;
			}
		
		}
	},
	
	SetUpTimeTrack: function(timer, callback){
		//In fact, 4ms is specified by the HTML5 spec and is consistent across browsers released in 2010 and onward. Prior to (Firefox 5.0 / Thunderbird 5.0 / SeaMonkey 2.2), 
		//the minimum timeout value for nested timeouts was 10 ms
		
		//Browsers including Internet Explorer, Chrome, Safari, and Firefox store the delay as a 32-bit signed Integer internally. 
		//This causes an Integer overflow when using delays larger than 2147483647, resulting in the timeout being executed immediately.
		if (timer < 10 || timer > 2147483647)
			return;
		setTimeout(callback, timer);
	},
	
	AlertUserDueDateHasPassed: function(showAlert, api) {
		api.due_date_has_passed = 1;
		if (showAlert)
			alert('The due time has expired. You can continue to work on the activity, but your grade will no longer be updated.');
	},
	
	AlertUserDueDateSoonPass: function(showAlert, remainingTime) {
		var minutes = Math.ceil(remainingTime / 60000)
		if (showAlert && !isNaN(minutes))
			alert('The activity is going to be due in less than ' + minutes + (minutes === 1? ' minute.' : ' minutes.'));
	},
	
	OnBeforeUnload: function(e) {
		if (save_in_progress == true) {
			var msg = "Your activity data is currently in the process of being saved.";
		
			var e = e || window.event;
			// For IE and Firefox prior to version 4
			if (e) {
				e.returnValue = msg;
			}
			// For Safari
			return msg;
		}
		// no return value means don't show an alert (https://developer.mozilla.org/en-US/docs/Web/Reference/Events/beforeunload)
	}
	
		// Remember: no comma after last item
	};	// end return for public vars and functions
}();

// ========================================================================
// Wrappers for old BFW_SCORM functions, so that activities that used these
// functions can work while including ARGA_wrapper.js instead of bfw_scorm_wrapper.js
// ========================================================================

// Start the scorm session for the SCO
function Initialize_SCORM_Session() {
	return Initialize_ARGA_Session();
}

// Set data.
// The SCO can get and save absolutely anything here
// Or the SCO can use the more specific calls below
// for question responses and grades
// *NOTE* that data is not actually stored to the server until Save_SCORM_Data() is called.
function Set_SCORM_Data(key, value) {
	return Set_ARGA_Data(key, value);
}

// Retrieve previously-stored data
function Get_SCORM_Data(key) {
	return Get_ARGA_Data(key);
}

// Submit a question response
// If an SCO uses this function to store info, BFW's Angel will format it nicely in the report
function Set_SCORM_Question_Response(
		questionNum, 		// should start at 1 for first question, 2 for second question, etc.
		questionType, 		// up to SCO to define; it will be displayed to the instructor, so make it readable (e.g. "multiple choice")
		questionText, 		// text of question as presented to student, to be displayed to instructor when viewing results
		correctAnswer, 		// text of correct answer (if a MC question, write out the correct response; e.g. "Venus")
		learnerResponse, 	// student's response (if a MC question, write out the response given, e.g. "Mars")
		questionGrade, 		// 0-100 percentage, or -1 if can't be scored
		questionWeight,		// percentage of the total grade for the activity.  Note that these percentages have to be hard-coded into the SCO; not configurable by profs at this time.
		questionData		// optional parameter to store info needed to recreate student's experience, e.g. a random number seed
		) {
	return Set_ARGA_Question_Response(
		questionNum, 		// should start at 1 for first question, 2 for second question, etc.
		questionType, 		// up to SCO to define; it will be displayed to the instructor, so make it readable (e.g. "multiple choice")
		questionText, 		// text of question as presented to student, to be displayed to instructor when viewing results
		correctAnswer, 		// text of correct answer (if a MC question, write out the correct response; e.g. "Venus")
		learnerResponse, 	// student's response (if a MC question, write out the response given, e.g. "Mars")
		questionGrade, 		// 0-100 percentage, or -1 if can't be scored
		questionWeight,		// percentage of the total grade for the activity.  Note that these percentages have to be hard-coded into the SCO; not configurable by profs at this time.
		questionData		// optional parameter to store info needed to recreate student's experience, e.g. a random number seed
		);
}

// Retrieve a previously-stored learnerResponse, indexed to questionNum
function Get_SCORM_LearnerResponse(questionNum) {
	return Get_ARGA_LearnerResponse(questionNum);
}

// Retrieve previously-stored questionData, indexed to questionNum
function Get_SCORM_QuestionData(questionNum) {
	return Get_ARGA_QuestionData(questionNum);
}

// Set a 0-100 percentage grade (e.g. 80 for 80%) for the entire activity
function Set_SCORM_Grade(grade) {
	return Set_ARGA_Grade(grade);
}

// Retrieve a previously-stored grade
function Get_SCORM_Grade() {
	return Get_ARGA_Grade();
}

// Commit all data that has been stored using the Set_ functions defined here.
function Save_SCORM_Data() {
	return Save_ARGA_Data();
}


//////////////////////////////////////////////////////////////////////////////
// Including code from DataExchange.js here
// The latest version of this code should be available from
// http://support.agilix.com/downloads/Utilities/DataExchange.zip
//
// MACMILLAN SHOULD NOT UPDATE OR ADD TO THE BELOW CODE
//////////////////////////////////////////////////////////////////////////////

// Although not used directly by any BrainHoney pages, this file implements the
// Data Exchange Javascript API (DEJS), and can be
// included by activities hosted on external servers being played within an iFrame in the BrainHoney player.
// This API enables activities to save SCORM data variables, including those that update the BrainHoney gradebook
// for the current activity/current user.
// Additional documentation for the API can be found in BrainHoney Content Developer Guide.

// The DEJS_API class implements the following public methods. Detailed documentation for each
// method is adjacent to the method implementation:
//	  initialize
//	  ping
//	  getData
//	  setData
//	  getSectionData

// The JSONP class is a supporting class that facilitates cross-site AJAX-type requests without actually
// using AJAX. It uses the JSON with Padding (JSONP) approach described here: http://en.wikipedia.org/wiki/JSON#JSONP
// It also provides some utility functions used by DEJS_API.
window.undefined = window.undefined;
var JSONP = function() {
	var queue = [];
	var queueIndex = 0;
	var defaultTimeout = 30000;
	var useHasOwn=!!{}.hasOwnProperty;

	function clearQueueItem(requestId) {
		var result = queue[requestId];
		if (result != null && result.timeout != -1) {
			clearTimeout(result.timeout);
			result.timeout = -1;
		}
		queue[requestId] = null;
		return result;
	}

	function onTimeout(requestId) {
		JSONP.callback(requestId, null);
	}

	// Generic Utility functions
	function isDate(v) {
		return v && typeof v.getFullYear=='function';
	}

	var m = {
		"\b": '\\b',
		"\t": '\\t',
		"\n": '\\n',
		"\f": '\\f',
		"\r": '\\r',
		'"' : '\\"',
		"\\": '\\\\'
	};

	function encodeString(s){
		if(/["\\\x00-\x1f]/.test(s)){
			return'"'+s.replace(/([\x00-\x1f\\"])/g, function(a,b){
				var c=m[b];
				if(c){
					return c;
				}
				c=b.charCodeAt();
				return "\\u00" + Math.floor(c/16).toString(16) + (c%16).toString(16);
			}) + '"';
		}
		return '"' + s + '"';
	}

	function encodeArray(o) {
		var a=["["],b,i,l=o.length,v;
		for(i=0;i<l;i+=1){
			v=o[i];
			switch(typeof v){
				case"undefined":
				case"function":
				case"unknown":
					break;
				default:
					if(b){
						a.push(',');
					}
					a.push(v===null?"null":JSONP.encode(v));
					b=true;
			}
		}
		a.push("]");
		return a.join("");
	}

	function pad(n){
		return n<10?"0"+n:n;
	}
	function encodeDate(o){
		return'"'+o.getFullYear()+"-"+
			pad(o.getMonth()+1)+"-"+
			pad(o.getDate())+"T"+
			pad(o.getHours())+":"+
			pad(o.getMinutes())+":"+
			pad(o.getSeconds())+'"';
	}


	return {
		request: function (o) {
			if(JSONP.isEmpty(o) || JSONP.isEmpty(o.url)) {
				return;
			}
			var thisId = queue.length;
			o.params = o.params || {};
			o.params.i = thisId.toString(10);   // index added to each call...to align responses in callback

			var script = document.createElement('script');
			script.type = 'text/javascript';

			var current = {
				script: script
				, options: o
				, timeout: -1
			};
			queue.push(current);

			// execute the request
			script.src = o.url + '?' + JSONP.urlEncode(o.params);
			current.timeout = setTimeout(function() {onTimeout(thisId); }, o.timeout || defaultTimeout);
			document.getElementsByTagName('head')[0].appendChild(script);
		}

		,callback: function(requestId, json) {
			if (requestId == -1 || queue[requestId] == null) {
				// -1 means caller never specified the id (should never happen)
				// null means this request has already been cancelled due to timeout. In both cases...do nothing
				return;
			}
			// removes it from the queue, stops any timeouts, etc.
			var request = clearQueueItem(requestId);
			if (request != null && !JSONP.isEmpty(request.options.callback)) {
				try {
					request.options.callback.apply(request.options.scope || window, [request.options, json]);
				} catch (e) {
					console.log('error callback(): ' + e.message? e.message : e);
				}
				document.getElementsByTagName('head')[0].removeChild(request.script);
			}
		}
		, getUrlLength: function(url, params) {
			params.apiIndex = '99999';   // sufficient padding for lots of requests
			var temp = url + '?' + JSONP.urlEncode(params);
			return temp.length;
		}

		// Generic Utility methods
		, apply: function(o, c, defaults){
			if(defaults){
				apply(o, defaults);
			}
			if(o && c && typeof c == 'object'){
				for(var p in c){
					o[p] = c[p];
				}
			}
			return o;
		}

		, isObject : function(v){
			return v && typeof v == "object";
		}
		, isArray : function(v){
			return v && typeof v.length=='number' && typeof v.splice=='function';
		}
		, isEmpty : function(v) {
			return v===null || v==='' || typeof v == 'undefined';
		}
		, encode : function(o) {
			if(typeof o=="undefined"||o===null){
				return "null";
			} else if (JSONP.isArray(o)){
				return encodeArray(o);
			} else if (isDate(o)){
				return encodeDate(o);
			} else if (typeof o=="string"){
				return encodeString(o);
			} else if (typeof o=="number"){
				return isFinite(o) ? String(o) : "null";
			} else if (typeof o=="boolean"){
				return String(o);
			} else{
				var a=["{"],b,i,v;
				for(i in o){
					if(!useHasOwn||o.hasOwnProperty(i)){
						v=o[i];
						switch(typeof v){
							case"undefined":
							case"function":
							case"unknown":
								break;
							default:
								if(b){
									a.push(',');
								}
								a.push(JSONP.encode(i),":",v===null?"null":JSONP.encode(v));
								b=true;
						}
					}
				}
				a.push("}");
				return a.join("");
			}
		}
		, urlEncode : function(o){
			if(!o){return"";}
			var buf=[];
			for(var key in o){
				var ov=o[key],k=encodeURIComponent(key);
				var type=typeof ov;
				if(type=='undefined'){
					buf.push(k,"=&");
				}else if(type!="function" && type!="object"){
					buf.push(k,"=",encodeURIComponent(ov),"&");
				}else if(isDate(ov)){
					var s=encode(ov).replace(/"/g,'');
					buf.push(k,"=",s,"&");
				}else if(JSONP.isArray(ov)){
					if(ov.length){
						for(var i=0,len=ov.length;i<len;i++){
							buf.push(k,"=",encodeURIComponent(ov[i]===undefined?'':ov[i]),"&");
						}
					}else{
						buf.push(k,"=&");
					}
				}
			}
			buf.pop();
			return buf.join("");
		}
		, urlDecode : function(string,overwrite) {
			if (!string||!string.length){
				return{};
			}
			var obj={};
			var pairs=string.split('&');
			var pair,name,value;
			for(var i=0,len=pairs.length;i<len;i++){
				pair=pairs[i].split('=');
				name=decodeURIComponent(pair[0]);
				value=decodeURIComponent(pair[1]);
				if(overwrite!==true){
					if(typeof obj[name]=="undefined"){
						obj[name]=value;
					} else if(typeof obj[name]=="string"){
						obj[name]=[obj[name]];
						obj[name].push(value);
					} else {
						obj[name].push(value);
					}
				} else {
					obj[name]=value;
				}
			}
			return obj;
		}
		, htmlEncode : function(value){
			return !value ? value : String(value).replace(/&(?!#)/g,"&amp;").replace(/>/g,"&gt;").replace(/</g,"&lt;").replace(/"/g,'&quot;');
		}
		,
		// PW 2013/12/06: function to determine if this page is in the same domain as scormdata.ashx
		sameHost: function() {
			var urlQuery = Url_Query? Url_Query : JSONP.urlDecode(window.location.search.substr(1));
			var appRoot = urlQuery['approot'];
			return appRoot && (appRoot.indexOf(window.location.host) > -1);
		}
	};
}();

JSONP.apply(Function.prototype, {
	createDelegate : function(obj,args){
		var method=this;
		return function(){
			var callArgs=args||arguments;
			return method.apply(obj||window,callArgs);
		};
	},
	defer : function(millis,obj,args){
		var fn=this.createDelegate(obj,args);
		if(millis){
			return setTimeout(fn,millis);
		}
		fn();
		return 0;
	}
});

var DEJS_API = function(){
	// private variables
	var enrollmentId = null;
	var itemId = null;
	var appRoot = null;
	var debug = false;
	var pingTimer = null;
	var timeToLive = 15;	// DLAP expiration timeout in minutes

	// private methods

	// returns true if initialize was successfully called; otherwise false
	function isInitialized() {
		return !JSONP.isEmpty(enrollmentId) && !JSONP.isEmpty(appRoot) && !JSONP.isEmpty(itemId);
	}
	
	function pingScorm(options) {
		if (!isInitialized()) {
			return false;
		}
		JSONP.request({
			url: appRoot + '/Learn/ScormData.ashx'
			, params: {
				action: 'ping'
			}
			, callback: pingCallback
			, scope: this
			, timeout: options == null ? null : options.timeout
			, agxOptions: options
		});
		return true;
	}

	function checkForJSONPError(data) {
		var result = data || {};
		if (JSONP.isEmpty(result.success)) {
			result.success = false;
		}
		if (!result.success && JSONP.isEmpty(result.message)) {
			result.message = 'no response';
		}
		return result;
	}
	
	// PW 2013/12/06: version of putDataWorker that sends data via POST instead of get
	function putDataWorker_post(options, data) {
		jQuery.ajax({
			type: "POST",
			url: appRoot + '/Learn/ScormData.ashx',
			cache: false,
			data: {
				action: 'putscormdata'
				, enrollmentid: enrollmentId
				, itemid: itemId
				, data: data
				, last: 1		// have to include this; otherwise ScormData.ashx won't save the data
			},
			dataType: "text",
			timeout: (options == null || options.timeout == null) ? 15000 : options.timeout,
			success: function(result) {
				// ScormData.ashx should return "{success: true}" if it worked
				// add "POST" to the start so we know we used this method (and make sure this is a string)
				result = "POST: " + result;
				var really_successful;
				if (result.search(/success:\s*true/i) > -1) {
					console.log("DEJS POST AJAX success: " + result);
					really_successful = true;
				} else {
					console.log("DEJS POST AJAX returned, but with error: " + result);
					really_successful = false;
				}
				if (options != null && options.callback != null) {
					options.callback(options, really_successful, result);
				}
			},
			error: function(jqXHR, textStatus, errorThrown) {
				console.log("DEJS POST AJAX error: " + result);
				console.log(jqXHR);
				console.log(textStatus);
				console.log(errorThrown);
				// send back errorThrown as third parameter; add "POST" so we know we used this method
				options.callback(options, false, "POST: " + errorThrown);
			}
		});
	}

	var putCount = 0;		   // global counter to keep track of last request made
	var putRemainder = null;	// remaining data to put after the current request completes
	function putDataWorker(options, data, add) {
		// Max URL length in IE is 2083 characters (http://support.microsoft.com/kb/208427)
		// Calculate the largest allowed buffer size given the current param set.
		var sizeTest = {
			url: appRoot + '/Learn/ScormData.ashx'
			, action: 'putscormdata'
			, enrollmentid: enrollmentId
			, itemid: itemId
			, data: ''
			, add: '1'
			, last: '1'
		};
		var maxPutSize = 2083 - (JSONP.getUrlLength(appRoot + '/Learn/ScormData.ashx', sizeTest) + 20); // 20 = extra padding
		var last = false;
		// If the data is larger than allowed, split it into multiple chunks and make multiple JSONP requests
		if (data.length > maxPutSize) {
			putRemainder = data.substr(maxPutSize);
			data = data.substr(0, maxPutSize);

			// Avoid escaped-character boundaries...back up a few characters to get in front of the last escaped character
			if (data.substr(data.length-10).indexOf('%') != -1) {
				var splitAt = data.lastIndexOf('%');
				putRemainder = data.substr(splitAt) + putRemainder;
				data = data.substr(0, splitAt);
			}
		} else {
			putRemainder = null;
			last = true;
		}

		// Build parameter list and make the JSONP request
		putCount++;
		var params = {
			action: 'putscormdata'
			, enrollmentid: enrollmentId
			, itemid: itemId
			, data: data
		};
		if (add) {
			params.add = '1';
		}
		if (last) {
			params.last = '1';
		}
		JSONP.request({
			url: appRoot + '/Learn/ScormData.ashx'
			, params: params
			, callback: putDataCallback
			, scope: this
			, timeout: options == null ? null : options.timeout
			, agxOptions: options
			, agxPutCount: putCount
		});
	}

	function putDataCallback(options, data) {
		if (options.agxPutCount != putCount) {
			// this response is for a cancelled/superceeded request. Ignore the result and just return. No callback
			// to the caller since they know that they cancelled one before the previous callback was called.
			console.log("putDataCallback: cancelled/superceded request; agxPutCount = " + options.agxPutCount + " / putCount = " + putCount + " (this shouldn't be a problem.)");
			return;
		}
		var result = checkForJSONPError(data);
		if (result.success) {
			if (!JSONP.isEmpty(putRemainder)) {
				// there's more data to put. Immediately call putDataWorker after this callback has returned
				putDataWorker.defer(1, this, [options.agxOptions, putRemainder, true /*add*/]);
				return;
			}
		} else {
			console.log("putDataCallback (JSONP) error:");
			console.log(result);
			
			// call failed. No need to keep this around
			putRemainder = null;
		}
		// if we got here, then the put has completed.
		if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
			// PW 2013/12/05: add a return message that includes "JSONP" so we know that's the method we used
			options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success, "JSONP: " + result.message);
		}
	}

	function pingCallback(options, data) {
		var result = checkForJSONPError(data);
		if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
			options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success);
		}
	}

	function getDataCallback(options, data) {
		var result = checkForJSONPError(data);
		var data = [];
		if (result.success) {
			//log("got data: " + JSONP.encode(result));
			// push activity user data into array first
			if (!JSONP.isEmpty(result.scormData)) {
				for (var i = 0, len = result.scormData.length; i < len; i++) {
					var name = result.scormData[i].name;
					var value = result.scormData[i].value;
					if (!JSONP.isEmpty(name)) {
						data.push({name:name, value:value});
					}
				}
			}
			// readonly custom fields next
			if (!JSONP.isEmpty(result.customFields)) {
				for (var i = 0, len = result.customFields.length; i < len; i++) {
					var name = result.customFields[i][0];
					var value = result.customFields[i][1];
					if (!JSONP.isEmpty(name)) {
						name = 'bh.custom.' + name;
						data.push({name:name, value:value});
					}
				}
			}
			// readonly system vars last
			if (!JSONP.isEmpty(result.bhVars)) {
				for (var i = 0, len = result.bhVars.length; i < len; i++) {
					var name = result.bhVars[i][0];
					var value = result.bhVars[i][1];
					if (!JSONP.isEmpty(name)) {
						data.push({name:name, value:value});
					}
				}
			}
		}
		if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
			options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success, data);
		}
	}

	function getSectionDataCallback(options, data) {
		var result = checkForJSONPError(data);
		if (result.success) {
			//log("got class data: " + JSONP.encode(result));
		}
		if (!JSONP.isEmpty(options.agxOptions) && !JSONP.isEmpty(options.agxOptions.callback)) {
			options.agxOptions.callback.call(options.agxOptions.scope || window, options.agxOptions, result.success, result.data);
		}
	}

	// public methods
	return {

		/**
		* Initializes the DEJS_API object. You must call this method immediately upon page load
		* to ensure the page remains authenticated and that all other calls succeed. This method
		* also starts a timer that regularly pings the server to keep this object authenticated
		* @return {Boolean} Returns true on success; otherwise false.
		*/
		initialize: function() {
			if (isInitialized()) {
				return true;
			}
			var queryParams = Url_Query? Url_Query : JSONP.urlDecode(window.location.search.substr(1));
			if(queryParams['enrollmentid']) {
				enrollmentId = queryParams['enrollmentid'];
			}
			if (queryParams['itemid']) {
				itemId = queryParams['itemid']
			}
			if (queryParams['approot']) {
				appRoot = queryParams['approot'];
			}
			// Look for BLTI names
			if(queryParams['ext_enrollmentid']) {
				enrollmentId = queryParams['ext_enrollmentid'];
			}
			if (queryParams['ext_itemid']) {
				itemId = queryParams['ext_itemid']
			}
			if (queryParams['ext_approot']) {
				appRoot = queryParams['ext_approot'];
			}
			
			// PW 2013/06/24: if we haven't gotten enrollmentid yet, look in an unescaped version of the "Url" parameter
			// This is necessary because the PX proxy escapes the original URL
			if (enrollmentId == null && queryParams['Url'] != null) {
				var qp2 = JSONP.urlDecode(queryParams['Url'].replace(/.*?\?/, ""));
				if(qp2['enrollmentid']) {
					enrollmentId = qp2['enrollmentid'];
				}
				if (qp2['itemid']) {
					itemId = qp2['itemid']
				}
				if (qp2['approot']) {
					appRoot = qp2['approot'];
				}
			}
			
			if (!isInitialized()) {
				return false;
			}
			// Set the timer to tick just shy of 50% of the timeout, which means we'll get 2 pings in
			// every timeout interval (including the first one), and sometimes 3 (in later ones)
			// Currently timeToLive is 15, so this will ping at 7, 14, 21, 28, etc.
			pingTimer = setInterval(pingScorm, (timeToLive * 1000 * 28));
			return true;
		}

		/**
		* Launches an asynchronous ping request. This is typically not necessary to call because
		* the initialize method starts a timer that regularly calls ping.
		* @param {Object} options You can specify an options object containing these members:
		*	  * callback {Function} The callback to call upon completion of the request.
		*		  The callback is called with these parameters:
		*			  options {Object} - original options passed to ping
		*			  success {Boolean} - true if successful; otherwise false
		*	  * scope {Object} (optional) scope to execute the callback request. The this pointer for the callback. The default is window.
		*	  * timeout {Number} (optional) milliseconds to wait before timing out. The default is 30000 (30 seconds)
		*/
		, ping: function(options) {
			return pingScorm(options);
		}

		/**
		* Launches an asynchronous request to get the user's data for this activity
		* @param {Object} options Specify an options object containing these members:
		*	  * callback {Function} The callback to call upon completion of the request
		*		  The callback is called with these parameters:
		*			  options {Object} The original options passed to getData
		*			  success {Boolean} True if successful; otherwise false
		*			  data {Array} An array of objects with 'name' and 'value' members. For example: [{name:'a', value:'foo'},{name:'b',value:'bar'}]
		*	  * scope {Object} (optional) Scope to execute the callback request. The this pointer for the callback. The default is window.
		*	  * timeout {Number} (optional) Milliseconds to wait before timing out. The default is 30000 (30 seconds)
		*  @return {Boolean} Returns true on successful launch; otherwise false. Check success status of the request in your callback.
		*/
		, getData: function(options) {
			if (!isInitialized()) {
				return false;   // initialize never called
			}
			JSONP.request({
				url: appRoot + '/Learn/ScormData.ashx'
				, params: {
					action: 'getscormdata'
					, enrollmentid: enrollmentId
					, itemid: itemId
				}
				, callback: getDataCallback
				, scope: this
				, timeout: options == null ? null : options.timeout
				, agxOptions: options
			});
			return true;
		}

		/**
		* Launches an asynchronous request to store the data specified in the options.data field.
		* @param {Object} options An options object containing these members:
		*	  * data {Array} An array of objects with 'name' and 'value' members. For example: [{name:'a', value:'foo'},{name:'b',value:'bar'}]
		*	  * callback {Function} (optional) The callback to call upon completion of the request
		*		  The callback is called with these parameters:
		*			  options {Object} The original options passed to putData
		*			  success {Boolean} True if successful; otherwise false
		*	  * scope {Object} (optional) Scope to execute the callback request. The this pointer for the callback. The default is window.
		*	  * timeout {Number} (optional) Milliseconds to wait before timing out. The default is 30000 (30 seconds)
		*  @return {Boolean} Returns true on successful launch; otherwise false. Check success status of the request in your callback.
		*/
		, putData: function(options) {
			if (!isInitialized()) {
				return false;
			}
			// Build the xml blob
			var dataXml = '<data>';
			if (JSONP.isArray(options.data)) {
				for (var i = 0, len = options.data.length; i < len; i++) {
					if (!JSONP.isEmpty(options.data[i]) && !JSONP.isEmpty(options.data[i].name) &&
						!JSONP.isObject(options.data[i].name) && !JSONP.isArray(options.data[i].name))
					{
						// PW 5 Apr 2013: previously this was setting value to "options.data[i].value || ''", which 
						// has the unfortunate effect of changing the value 0 to the empty string. Fixed that; I assume
						// the intention was actually to use the empty string for null or undefined values
						var val = options.data[i].value;
						if (val == null) val = "";
						dataXml = dataXml.concat('<entry name="' + JSONP.htmlEncode(options.data[i].name) + '" value="' +  JSONP.htmlEncode(val) + '"/>');
					}
				}
			}
			dataXml = dataXml.concat('</data>');
			
			// Send it to the server
			// PW 2013/12/06: if the activity is in the same domain as scormData.ashx
			// and we have jquery available, do the request via POST, which is faster and more reliable
			if (JSONP.sameHost() && (window.jQuery != null && jQuery('body') != null)) {
				putDataWorker_post(options, dataXml);
			// else use JSONP, as specified by agilix
			} else {
				putDataWorker(options, dataXml, false /*add*/);
			}
			return true;
		}

		/**
		* Launches an asynchronous request to get all section data for the current item.
		* @param {Object} options An options object containing these members:
		*	   * callback {Function} The callback to call upon completion of the request
		*		  The callback is called with these parameters:
		*			  options {Object} The original options passed to getData
		*			  success {Boolean} True if successful; otherwise false
		*			  data {Array} An array of objects. Each object corresponds to a single student's data and contains
		*				  these fields:
		*				  id {String} Student's enrollment ID. If the requester is a student, this property is omitted.
		*				  first {String} Student's first name. If the requester is a student, this property is omitted.
		*				  last {String} Student's last name. If the requester is a student, this property is omitted.
		*				  score {String} A real number (typically between 0 and 1) that is the student's score.
		*				  scorm {Array} Array of objects containing name and value properties for all other scorm variables set
		*					  by the student. The objects are in the form [{'name':'value'},•].
		*					  For example: [{'cmi.interactions.0.id':'q1'},{'cmi.interactions.0.type','other'}]
		*	  * scope {Object} (optional) Scope to execute the callback request. The this pointer for the callback. The default is window.
		*	  * timeout {Number} (optional) Milliseconds to wait before timing out. The default is 30000 (30 seconds)
		*  @ return {Boolean} True on successful launch; otherwise false. Check success status of the request and get the data in your callback.
		*/
		, getSectionData: function(options) {
			if (!isInitialized()) {
				return false;
			}
			JSONP.request({
				url: appRoot + '/Learn/ScormData.ashx'
				, params: {
					action: 'getsectionsummary'
					, enrollmentid: enrollmentId
					, itemid: itemId
					, allstatus: options == null ? false : (options.allstatus ? '1' : '0')
				}
				, callback: getSectionDataCallback
				, scope: this
				, timeout: options == null ? null : options.timeout
				, agxOptions: options
			});
			return true;
		}
	};
}();

/////////////////////////////////////////////////////////
// JSON2 minified (from http://yandex.st/json2/2011-10-19/json2.min.js)
var JSON;if(!JSON){JSON={}}(function(){function f(n){return n<10?"0"+n:n}if(typeof Date.prototype.toJSON!=="function"){Date.prototype.toJSON=function(key){return isFinite(this.valueOf())?this.getUTCFullYear()+"-"+f(this.getUTCMonth()+1)+"-"+f(this.getUTCDate())+"T"+f(this.getUTCHours())+":"+f(this.getUTCMinutes())+":"+f(this.getUTCSeconds())+"Z":null};String.prototype.toJSON=Number.prototype.toJSON=Boolean.prototype.toJSON=function(key){return this.valueOf()}}var cx=/[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,escapable=/[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,gap,indent,meta={"\b":"\\b","\t":"\\t","\n":"\\n","\f":"\\f","\r":"\\r",'"':'\\"',"\\":"\\\\"},rep;function quote(string){escapable.lastIndex=0;return escapable.test(string)?'"'+string.replace(escapable,function(a){var c=meta[a];return typeof c==="string"?c:"\\u"+("0000"+a.charCodeAt(0).toString(16)).slice(-4)})+'"':'"'+string+'"'}function str(key,holder){var i,k,v,length,mind=gap,partial,value=holder[key];if(value&&typeof value==="object"&&typeof value.toJSON==="function"){value=value.toJSON(key)}if(typeof rep==="function"){value=rep.call(holder,key,value)}switch(typeof value){case"string":return quote(value);case"number":return isFinite(value)?String(value):"null";case"boolean":case"null":return String(value);case"object":if(!value){return"null"}gap+=indent;partial=[];if(Object.prototype.toString.apply(value)==="[object Array]"){length=value.length;for(i=0;i<length;i+=1){partial[i]=str(i,value)||"null"}v=partial.length===0?"[]":gap?"[\n"+gap+partial.join(",\n"+gap)+"\n"+mind+"]":"["+partial.join(",")+"]";gap=mind;return v}if(rep&&typeof rep==="object"){length=rep.length;for(i=0;i<length;i+=1){if(typeof rep[i]==="string"){k=rep[i];v=str(k,value);if(v){partial.push(quote(k)+(gap?": ":":")+v)}}}}else{for(k in value){if(Object.prototype.hasOwnProperty.call(value,k)){v=str(k,value);if(v){partial.push(quote(k)+(gap?": ":":")+v)}}}}v=partial.length===0?"{}":gap?"{\n"+gap+partial.join(",\n"+gap)+"\n"+mind+"}":"{"+partial.join(",")+"}";gap=mind;return v}}if(typeof JSON.stringify!=="function"){JSON.stringify=function(value,replacer,space){var i;gap="";indent="";if(typeof space==="number"){for(i=0;i<space;i+=1){indent+=" "}}else{if(typeof space==="string"){indent=space}}rep=replacer;if(replacer&&typeof replacer!=="function"&&(typeof replacer!=="object"||typeof replacer.length!=="number")){throw new Error("JSON.stringify")}return str("",{"":value})}}if(typeof JSON.parse!=="function"){JSON.parse=function(text,reviver){var j;function walk(holder,key){var k,v,value=holder[key];if(value&&typeof value==="object"){for(k in value){if(Object.prototype.hasOwnProperty.call(value,k)){v=walk(value,k);if(v!==undefined){value[k]=v}else{delete value[k]}}}}return reviver.call(holder,key,value)}text=String(text);cx.lastIndex=0;if(cx.test(text)){text=text.replace(cx,function(a){return"\\u"+("0000"+a.charCodeAt(0).toString(16)).slice(-4)})}if(/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,"@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,"]").replace(/(?:^|:|,)(?:\s*\[)+/g,""))){j=eval("("+text+")");return typeof reviver==="function"?walk({"":j},""):j}throw new SyntaxError("JSON.parse")}}}());


/////////////////////////////////////////////////////////
/**
 * easyXDM
 * http://easyxdm.net/
 * Copyright(c) 2009-2011, Øyvind Sean Kinsey, oyvind@kinsey.no.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
// don't include if it's already defined or ARGA is in the same domain as host
if (window.easyXDM == null && !JSONP.sameHost()) {
	(function(N,d,p,K,k,H){var b=this;var n=Math.floor(Math.random()*10000);var q=Function.prototype;var Q=/^((http.?:)\/\/([^:\/\s]+)(:\d+)*)/;var R=/[\-\w]+\/\.\.\//;var F=/([^:])\/\//g;var I="";var o={};var M=N.easyXDM;var U="easyXDM_";var E;var y=false;var i;var h;function C(X,Z){var Y=typeof X[Z];return Y=="function"||(!!(Y=="object"&&X[Z]))||Y=="unknown"}function u(X,Y){return !!(typeof(X[Y])=="object"&&X[Y])}function r(X){return Object.prototype.toString.call(X)==="[object Array]"}function c(){var Z="Shockwave Flash",ad="application/x-shockwave-flash";if(!t(navigator.plugins)&&typeof navigator.plugins[Z]=="object"){var ab=navigator.plugins[Z].description;if(ab&&!t(navigator.mimeTypes)&&navigator.mimeTypes[ad]&&navigator.mimeTypes[ad].enabledPlugin){i=ab.match(/\d+/g)}}if(!i){var Y;try{Y=new ActiveXObject("ShockwaveFlash.ShockwaveFlash");i=Array.prototype.slice.call(Y.GetVariable("$version").match(/(\d+),(\d+),(\d+),(\d+)/),1);Y=null}catch(ac){}}if(!i){return false}var X=parseInt(i[0],10),aa=parseInt(i[1],10);h=X>9&&aa>0;return true}var v,x;if(C(N,"addEventListener")){v=function(Z,X,Y){Z.addEventListener(X,Y,false)};x=function(Z,X,Y){Z.removeEventListener(X,Y,false)}}else{if(C(N,"attachEvent")){v=function(X,Z,Y){X.attachEvent("on"+Z,Y)};x=function(X,Z,Y){X.detachEvent("on"+Z,Y)}}else{throw new Error("Browser not supported")}}var W=false,J=[],L;if("readyState" in d){L=d.readyState;W=L=="complete"||(~navigator.userAgent.indexOf("AppleWebKit/")&&(L=="loaded"||L=="interactive"))}else{W=!!d.body}function s(){if(W){return}W=true;for(var X=0;X<J.length;X++){J[X]()}J.length=0}if(!W){if(C(N,"addEventListener")){v(d,"DOMContentLoaded",s)}else{v(d,"readystatechange",function(){if(d.readyState=="complete"){s()}});if(d.documentElement.doScroll&&N===top){var g=function(){if(W){return}try{d.documentElement.doScroll("left")}catch(X){K(g,1);return}s()};g()}}v(N,"load",s)}function G(Y,X){if(W){Y.call(X);return}J.push(function(){Y.call(X)})}function m(){var Z=parent;if(I!==""){for(var X=0,Y=I.split(".");X<Y.length;X++){Z=Z[Y[X]]}}return Z.easyXDM}function e(X){N.easyXDM=M;I=X;if(I){U="easyXDM_"+I.replace(".","_")+"_"}return o}function z(X){return X.match(Q)[3]}function f(X){return X.match(Q)[4]||""}function j(Z){var X=Z.toLowerCase().match(Q);var aa=X[2],ab=X[3],Y=X[4]||"";if((aa=="http:"&&Y==":80")||(aa=="https:"&&Y==":443")){Y=""}return aa+"//"+ab+Y}function B(X){X=X.replace(F,"$1/");if(!X.match(/^(http||https):\/\//)){var Y=(X.substring(0,1)==="/")?"":p.pathname;if(Y.substring(Y.length-1)!=="/"){Y=Y.substring(0,Y.lastIndexOf("/")+1)}X=p.protocol+"//"+p.host+Y+X}while(R.test(X)){X=X.replace(R,"")}return X}function P(X,aa){var ac="",Z=X.indexOf("#");if(Z!==-1){ac=X.substring(Z);X=X.substring(0,Z)}var ab=[];for(var Y in aa){if(aa.hasOwnProperty(Y)){ab.push(Y+"="+H(aa[Y]))}}return X+(y?"#":(X.indexOf("?")==-1?"?":"&"))+ab.join("&")+ac}var S=(function(X){X=X.substring(1).split("&");var Z={},aa,Y=X.length;while(Y--){aa=X[Y].split("=");Z[aa[0]]=k(aa[1])}return Z}(/xdm_e=/.test(p.search)?p.search:p.hash));function t(X){return typeof X==="undefined"}var O=function(){var Y={};var Z={a:[1,2,3]},X='{"a":[1,2,3]}';if(typeof JSON!="undefined"&&typeof JSON.stringify==="function"&&JSON.stringify(Z).replace((/\s/g),"")===X){return JSON}if(Object.toJSON){if(Object.toJSON(Z).replace((/\s/g),"")===X){Y.stringify=Object.toJSON}}if(typeof String.prototype.evalJSON==="function"){Z=X.evalJSON();if(Z.a&&Z.a.length===3&&Z.a[2]===3){Y.parse=function(aa){return aa.evalJSON()}}}if(Y.stringify&&Y.parse){O=function(){return Y};return Y}return null};function T(X,Y,Z){var ab;for(var aa in Y){if(Y.hasOwnProperty(aa)){if(aa in X){ab=Y[aa];if(typeof ab==="object"){T(X[aa],ab,Z)}else{if(!Z){X[aa]=Y[aa]}}}else{X[aa]=Y[aa]}}}return X}function a(){var Y=d.body.appendChild(d.createElement("form")),X=Y.appendChild(d.createElement("input"));X.name=U+"TEST"+n;E=X!==Y.elements[X.name];d.body.removeChild(Y)}function A(Y){if(t(E)){a()}var ac;if(E){ac=d.createElement('<iframe name="'+Y.props.name+'"/>')}else{ac=d.createElement("IFRAME");ac.name=Y.props.name}ac.id=ac.name=Y.props.name;delete Y.props.name;if(typeof Y.container=="string"){Y.container=d.getElementById(Y.container)}if(!Y.container){T(ac.style,{position:"absolute",top:"-2000px",left:"0px"});Y.container=d.body}var ab=Y.props.src;Y.props.src="javascript:false";T(ac,Y.props);ac.border=ac.frameBorder=0;ac.allowTransparency=true;Y.container.appendChild(ac);if(Y.onLoad){v(ac,"load",Y.onLoad)}if(Y.usePost){var aa=Y.container.appendChild(d.createElement("form")),X;aa.target=ac.name;aa.action=ab;aa.method="POST";if(typeof(Y.usePost)==="object"){for(var Z in Y.usePost){if(Y.usePost.hasOwnProperty(Z)){if(E){X=d.createElement('<input name="'+Z+'"/>')}else{X=d.createElement("INPUT");X.name=Z}X.value=Y.usePost[Z];aa.appendChild(X)}}}aa.submit();aa.parentNode.removeChild(aa)}else{ac.src=ab}Y.props.src=ab;return ac}function V(aa,Z){if(typeof aa=="string"){aa=[aa]}var Y,X=aa.length;while(X--){Y=aa[X];Y=new RegExp(Y.substr(0,1)=="^"?Y:("^"+Y.replace(/(\*)/g,".$1").replace(/\?/g,".")+"$"));if(Y.test(Z)){return true}}return false}function l(Z){var ae=Z.protocol,Y;Z.isHost=Z.isHost||t(S.xdm_p);y=Z.hash||false;if(!Z.props){Z.props={}}if(!Z.isHost){Z.channel=S.xdm_c.replace(/["'<>\\]/g,"");Z.secret=S.xdm_s;Z.remote=S.xdm_e.replace(/["'<>\\]/g,"");ae=S.xdm_p;if(Z.acl&&!V(Z.acl,Z.remote)){throw new Error("Access denied for "+Z.remote)}}else{Z.remote=B(Z.remote);Z.channel=Z.channel||"default"+n++;Z.secret=Math.random().toString(16).substring(2);if(t(ae)){if(j(p.href)==j(Z.remote)){ae="4"}else{if(C(N,"postMessage")||C(d,"postMessage")){ae="1"}else{if(Z.swf&&C(N,"ActiveXObject")&&c()){ae="6"}else{if(navigator.product==="Gecko"&&"frameElement" in N&&navigator.userAgent.indexOf("WebKit")==-1){ae="5"}else{if(Z.remoteHelper){ae="2"}else{ae="0"}}}}}}}Z.protocol=ae;switch(ae){case"0":T(Z,{interval:100,delay:2000,useResize:true,useParent:false,usePolling:false},true);if(Z.isHost){if(!Z.local){var ac=p.protocol+"//"+p.host,X=d.body.getElementsByTagName("img"),ad;var aa=X.length;while(aa--){ad=X[aa];if(ad.src.substring(0,ac.length)===ac){Z.local=ad.src;break}}if(!Z.local){Z.local=N}}var ab={xdm_c:Z.channel,xdm_p:0};if(Z.local===N){Z.usePolling=true;Z.useParent=true;Z.local=p.protocol+"//"+p.host+p.pathname+p.search;ab.xdm_e=Z.local;ab.xdm_pa=1}else{ab.xdm_e=B(Z.local)}if(Z.container){Z.useResize=false;ab.xdm_po=1}Z.remote=P(Z.remote,ab)}else{T(Z,{channel:S.xdm_c,remote:S.xdm_e,useParent:!t(S.xdm_pa),usePolling:!t(S.xdm_po),useResize:Z.useParent?false:Z.useResize})}Y=[new o.stack.HashTransport(Z),new o.stack.ReliableBehavior({}),new o.stack.QueueBehavior({encode:true,maxLength:4000-Z.remote.length}),new o.stack.VerifyBehavior({initiate:Z.isHost})];break;case"1":Y=[new o.stack.PostMessageTransport(Z)];break;case"2":Z.remoteHelper=B(Z.remoteHelper);Y=[new o.stack.NameTransport(Z),new o.stack.QueueBehavior(),new o.stack.VerifyBehavior({initiate:Z.isHost})];break;case"3":Y=[new o.stack.NixTransport(Z)];break;case"4":Y=[new o.stack.SameOriginTransport(Z)];break;case"5":Y=[new o.stack.FrameElementTransport(Z)];break;case"6":if(!i){c()}Y=[new o.stack.FlashTransport(Z)];break}Y.push(new o.stack.QueueBehavior({lazy:Z.lazy,remove:true}));return Y}function D(aa){var ab,Z={incoming:function(ad,ac){this.up.incoming(ad,ac)},outgoing:function(ac,ad){this.down.outgoing(ac,ad)},callback:function(ac){this.up.callback(ac)},init:function(){this.down.init()},destroy:function(){this.down.destroy()}};for(var Y=0,X=aa.length;Y<X;Y++){ab=aa[Y];T(ab,Z,true);if(Y!==0){ab.down=aa[Y-1]}if(Y!==X-1){ab.up=aa[Y+1]}}return ab}function w(X){X.up.down=X.down;X.down.up=X.up;X.up=X.down=null}T(o,{version:"2.4.16.3",query:S,stack:{},apply:T,getJSONObject:O,whenReady:G,noConflict:e});o.DomHelper={on:v,un:x,requiresJSON:function(X){if(!u(N,"JSON")){d.write('<script type="text/javascript" src="'+X+'"><\/script>')}}};(function(){var X={};o.Fn={set:function(Y,Z){X[Y]=Z},get:function(Z,Y){var aa=X[Z];if(Y){delete X[Z]}return aa}}}());o.Socket=function(Y){var X=D(l(Y).concat([{incoming:function(ab,aa){Y.onMessage(ab,aa)},callback:function(aa){if(Y.onReady){Y.onReady(aa)}}}])),Z=j(Y.remote);this.origin=j(Y.remote);this.destroy=function(){X.destroy()};this.postMessage=function(aa){X.outgoing(aa,Z)};X.init()};o.Rpc=function(Z,Y){if(Y.local){for(var ab in Y.local){if(Y.local.hasOwnProperty(ab)){var aa=Y.local[ab];if(typeof aa==="function"){Y.local[ab]={method:aa}}}}}var X=D(l(Z).concat([new o.stack.RpcBehavior(this,Y),{callback:function(ac){if(Z.onReady){Z.onReady(ac)}}}]));this.origin=j(Z.remote);this.destroy=function(){X.destroy()};X.init()};o.stack.SameOriginTransport=function(Y){var Z,ab,aa,X;return(Z={outgoing:function(ad,ae,ac){aa(ad);if(ac){ac()}},destroy:function(){if(ab){ab.parentNode.removeChild(ab);ab=null}},onDOMReady:function(){X=j(Y.remote);if(Y.isHost){T(Y.props,{src:P(Y.remote,{xdm_e:p.protocol+"//"+p.host+p.pathname,xdm_c:Y.channel,xdm_p:4}),name:U+Y.channel+"_provider"});ab=A(Y);o.Fn.set(Y.channel,function(ac){aa=ac;K(function(){Z.up.callback(true)},0);return function(ad){Z.up.incoming(ad,X)}})}else{aa=m().Fn.get(Y.channel,true)(function(ac){Z.up.incoming(ac,X)});K(function(){Z.up.callback(true)},0)}},init:function(){G(Z.onDOMReady,Z)}})};o.stack.FlashTransport=function(aa){var ac,X,ab,ad,Y,ae;function af(ah,ag){K(function(){ac.up.incoming(ah,ad)},0)}function Z(ah){var ag=aa.swf+"?host="+aa.isHost;var aj="easyXDM_swf_"+Math.floor(Math.random()*10000);o.Fn.set("flash_loaded"+ah.replace(/[\-.]/g,"_"),function(){o.stack.FlashTransport[ah].swf=Y=ae.firstChild;var ak=o.stack.FlashTransport[ah].queue;for(var al=0;al<ak.length;al++){ak[al]()}ak.length=0});if(aa.swfContainer){ae=(typeof aa.swfContainer=="string")?d.getElementById(aa.swfContainer):aa.swfContainer}else{ae=d.createElement("div");T(ae.style,h&&aa.swfNoThrottle?{height:"20px",width:"20px",position:"fixed",right:0,top:0}:{height:"1px",width:"1px",position:"absolute",overflow:"hidden",right:0,top:0});d.body.appendChild(ae)}var ai="callback=flash_loaded"+ah.replace(/[\-.]/g,"_")+"&proto="+b.location.protocol+"&domain="+z(b.location.href)+"&port="+f(b.location.href)+"&ns="+I;ae.innerHTML="<object height='20' width='20' type='application/x-shockwave-flash' id='"+aj+"' data='"+ag+"'><param name='allowScriptAccess' value='always'></param><param name='wmode' value='transparent'><param name='movie' value='"+ag+"'></param><param name='flashvars' value='"+ai+"'></param><embed type='application/x-shockwave-flash' FlashVars='"+ai+"' allowScriptAccess='always' wmode='transparent' src='"+ag+"' height='1' width='1'></embed></object>"}return(ac={outgoing:function(ah,ai,ag){Y.postMessage(aa.channel,ah.toString());if(ag){ag()}},destroy:function(){try{Y.destroyChannel(aa.channel)}catch(ag){}Y=null;if(X){X.parentNode.removeChild(X);X=null}},onDOMReady:function(){ad=aa.remote;o.Fn.set("flash_"+aa.channel+"_init",function(){K(function(){ac.up.callback(true)})});o.Fn.set("flash_"+aa.channel+"_onMessage",af);aa.swf=B(aa.swf);var ah=z(aa.swf);var ag=function(){o.stack.FlashTransport[ah].init=true;Y=o.stack.FlashTransport[ah].swf;Y.createChannel(aa.channel,aa.secret,j(aa.remote),aa.isHost);if(aa.isHost){if(h&&aa.swfNoThrottle){T(aa.props,{position:"fixed",right:0,top:0,height:"20px",width:"20px"})}T(aa.props,{src:P(aa.remote,{xdm_e:j(p.href),xdm_c:aa.channel,xdm_p:6,xdm_s:aa.secret}),name:U+aa.channel+"_provider"});X=A(aa)}};if(o.stack.FlashTransport[ah]&&o.stack.FlashTransport[ah].init){ag()}else{if(!o.stack.FlashTransport[ah]){o.stack.FlashTransport[ah]={queue:[ag]};Z(ah)}else{o.stack.FlashTransport[ah].queue.push(ag)}}},init:function(){G(ac.onDOMReady,ac)}})};o.stack.PostMessageTransport=function(aa){var ac,ad,Y,Z;function X(ae){if(ae.origin){return j(ae.origin)}if(ae.uri){return j(ae.uri)}if(ae.domain){return p.protocol+"//"+ae.domain}throw"Unable to retrieve the origin of the event"}function ab(af){var ae=X(af);if(ae==Z&&af.data.substring(0,aa.channel.length+1)==aa.channel+" "){ac.up.incoming(af.data.substring(aa.channel.length+1),ae)}}return(ac={outgoing:function(af,ag,ae){Y.postMessage(aa.channel+" "+af,ag||Z);if(ae){ae()}},destroy:function(){x(N,"message",ab);if(ad){Y=null;ad.parentNode.removeChild(ad);ad=null}},onDOMReady:function(){Z=j(aa.remote);if(aa.isHost){var ae=function(af){if(af.data==aa.channel+"-ready"){Y=("postMessage" in ad.contentWindow)?ad.contentWindow:ad.contentWindow.document;x(N,"message",ae);v(N,"message",ab);K(function(){ac.up.callback(true)},0)}};v(N,"message",ae);T(aa.props,{src:P(aa.remote,{xdm_e:j(p.href),xdm_c:aa.channel,xdm_p:1}),name:U+aa.channel+"_provider"});ad=A(aa)}else{v(N,"message",ab);Y=("postMessage" in N.parent)?N.parent:N.parent.document;Y.postMessage(aa.channel+"-ready",Z);K(function(){ac.up.callback(true)},0)}},init:function(){G(ac.onDOMReady,ac)}})};o.stack.FrameElementTransport=function(Y){var Z,ab,aa,X;return(Z={outgoing:function(ad,ae,ac){aa.call(this,ad);if(ac){ac()}},destroy:function(){if(ab){ab.parentNode.removeChild(ab);ab=null}},onDOMReady:function(){X=j(Y.remote);if(Y.isHost){T(Y.props,{src:P(Y.remote,{xdm_e:j(p.href),xdm_c:Y.channel,xdm_p:5}),name:U+Y.channel+"_provider"});ab=A(Y);ab.fn=function(ac){delete ab.fn;aa=ac;K(function(){Z.up.callback(true)},0);return function(ad){Z.up.incoming(ad,X)}}}else{if(d.referrer&&j(d.referrer)!=S.xdm_e){N.top.location=S.xdm_e}aa=N.frameElement.fn(function(ac){Z.up.incoming(ac,X)});Z.up.callback(true)}},init:function(){G(Z.onDOMReady,Z)}})};o.stack.NameTransport=function(ab){var ac;var ae,ai,aa,ag,ah,Y,X;function af(al){var ak=ab.remoteHelper+(ae?"#_3":"#_2")+ab.channel;ai.contentWindow.sendMessage(al,ak)}function ad(){if(ae){if(++ag===2||!ae){ac.up.callback(true)}}else{af("ready");ac.up.callback(true)}}function aj(ak){ac.up.incoming(ak,Y)}function Z(){if(ah){K(function(){ah(true)},0)}}return(ac={outgoing:function(al,am,ak){ah=ak;af(al)},destroy:function(){ai.parentNode.removeChild(ai);ai=null;if(ae){aa.parentNode.removeChild(aa);aa=null}},onDOMReady:function(){ae=ab.isHost;ag=0;Y=j(ab.remote);ab.local=B(ab.local);if(ae){o.Fn.set(ab.channel,function(al){if(ae&&al==="ready"){o.Fn.set(ab.channel,aj);ad()}});X=P(ab.remote,{xdm_e:ab.local,xdm_c:ab.channel,xdm_p:2});T(ab.props,{src:X+"#"+ab.channel,name:U+ab.channel+"_provider"});aa=A(ab)}else{ab.remoteHelper=ab.remote;o.Fn.set(ab.channel,aj)}var ak=function(){var al=ai||this;x(al,"load",ak);o.Fn.set(ab.channel+"_load",Z);(function am(){if(typeof al.contentWindow.sendMessage=="function"){ad()}else{K(am,50)}}())};ai=A({props:{src:ab.local+"#_4"+ab.channel},onLoad:ak})},init:function(){G(ac.onDOMReady,ac)}})};o.stack.HashTransport=function(Z){var ac;var ah=this,af,aa,X,ad,am,ab,al;var ag,Y;function ak(ao){if(!al){return}var an=Z.remote+"#"+(am++)+"_"+ao;((af||!ag)?al.contentWindow:al).location=an}function ae(an){ad=an;ac.up.incoming(ad.substring(ad.indexOf("_")+1),Y)}function aj(){if(!ab){return}var an=ab.location.href,ap="",ao=an.indexOf("#");if(ao!=-1){ap=an.substring(ao)}if(ap&&ap!=ad){ae(ap)}}function ai(){aa=setInterval(aj,X)}return(ac={outgoing:function(an,ao){ak(an)},destroy:function(){N.clearInterval(aa);if(af||!ag){al.parentNode.removeChild(al)}al=null},onDOMReady:function(){af=Z.isHost;X=Z.interval;ad="#"+Z.channel;am=0;ag=Z.useParent;Y=j(Z.remote);if(af){T(Z.props,{src:Z.remote,name:U+Z.channel+"_provider"});if(ag){Z.onLoad=function(){ab=N;ai();ac.up.callback(true)}}else{var ap=0,an=Z.delay/50;(function ao(){if(++ap>an){throw new Error("Unable to reference listenerwindow")}try{ab=al.contentWindow.frames[U+Z.channel+"_consumer"]}catch(aq){}if(ab){ai();ac.up.callback(true)}else{K(ao,50)}}())}al=A(Z)}else{ab=N;ai();if(ag){al=parent;ac.up.callback(true)}else{T(Z,{props:{src:Z.remote+"#"+Z.channel+new Date(),name:U+Z.channel+"_consumer"},onLoad:function(){ac.up.callback(true)}});al=A(Z)}}},init:function(){G(ac.onDOMReady,ac)}})};o.stack.ReliableBehavior=function(Y){var aa,ac;var ab=0,X=0,Z="";return(aa={incoming:function(af,ad){var ae=af.indexOf("_"),ag=af.substring(0,ae).split(",");af=af.substring(ae+1);if(ag[0]==ab){Z="";if(ac){ac(true);ac=null}}if(af.length>0){aa.down.outgoing(ag[1]+","+ab+"_"+Z,ad);if(X!=ag[1]){X=ag[1];aa.up.incoming(af,ad)}}},outgoing:function(af,ad,ae){Z=af;ac=ae;aa.down.outgoing(X+","+(++ab)+"_"+af,ad)}})};o.stack.QueueBehavior=function(Z){var ac,ad=[],ag=true,aa="",af,X=0,Y=false,ab=false;function ae(){if(Z.remove&&ad.length===0){w(ac);return}if(ag||ad.length===0||af){return}ag=true;var ah=ad.shift();ac.down.outgoing(ah.data,ah.origin,function(ai){ag=false;if(ah.callback){K(function(){ah.callback(ai)},0)}ae()})}return(ac={init:function(){if(t(Z)){Z={}}if(Z.maxLength){X=Z.maxLength;ab=true}if(Z.lazy){Y=true}else{ac.down.init()}},callback:function(ai){ag=false;var ah=ac.up;ae();ah.callback(ai)},incoming:function(ak,ai){if(ab){var aj=ak.indexOf("_"),ah=parseInt(ak.substring(0,aj),10);aa+=ak.substring(aj+1);if(ah===0){if(Z.encode){aa=k(aa)}ac.up.incoming(aa,ai);aa=""}}else{ac.up.incoming(ak,ai)}},outgoing:function(al,ai,ak){if(Z.encode){al=H(al)}var ah=[],aj;if(ab){while(al.length!==0){aj=al.substring(0,X);al=al.substring(aj.length);ah.push(aj)}while((aj=ah.shift())){ad.push({data:ah.length+"_"+aj,origin:ai,callback:ah.length===0?ak:null})}}else{ad.push({data:al,origin:ai,callback:ak})}if(Y){ac.down.init()}else{ae()}},destroy:function(){af=true;ac.down.destroy()}})};o.stack.VerifyBehavior=function(ab){var ac,aa,Y,Z=false;function X(){aa=Math.random().toString(16).substring(2);ac.down.outgoing(aa)}return(ac={incoming:function(af,ad){var ae=af.indexOf("_");if(ae===-1){if(af===aa){ac.up.callback(true)}else{if(!Y){Y=af;if(!ab.initiate){X()}ac.down.outgoing(af)}}}else{if(af.substring(0,ae)===Y){ac.up.incoming(af.substring(ae+1),ad)}}},outgoing:function(af,ad,ae){ac.down.outgoing(aa+"_"+af,ad,ae)},callback:function(ad){if(ab.initiate){X()}}})};o.stack.RpcBehavior=function(ad,Y){var aa,af=Y.serializer||O();var ae=0,ac={};function X(ag){ag.jsonrpc="2.0";aa.down.outgoing(af.stringify(ag))}function ab(ag,ai){var ah=Array.prototype.slice;return function(){var aj=arguments.length,al,ak={method:ai};if(aj>0&&typeof arguments[aj-1]==="function"){if(aj>1&&typeof arguments[aj-2]==="function"){al={success:arguments[aj-2],error:arguments[aj-1]};ak.params=ah.call(arguments,0,aj-2)}else{al={success:arguments[aj-1]};ak.params=ah.call(arguments,0,aj-1)}ac[""+(++ae)]=al;ak.id=ae}else{ak.params=ah.call(arguments,0)}if(ag.namedParams&&ak.params.length===1){ak.params=ak.params[0]}X(ak)}}function Z(an,am,ai,al){if(!ai){if(am){X({id:am,error:{code:-32601,message:"Procedure not found."}})}return}var ak,ah;if(am){ak=function(ao){ak=q;X({id:am,result:ao})};ah=function(ao,ap){ah=q;var aq={id:am,error:{code:-32099,message:ao}};if(ap){aq.error.data=ap}X(aq)}}else{ak=ah=q}if(!r(al)){al=[al]}try{var ag=ai.method.apply(ai.scope,al.concat([ak,ah]));if(!t(ag)){ak(ag)}}catch(aj){ah(aj.message)}}return(aa={incoming:function(ah,ag){var ai=af.parse(ah);if(ai.method){if(Y.handle){Y.handle(ai,X)}else{Z(ai.method,ai.id,Y.local[ai.method],ai.params)}}else{var aj=ac[ai.id];if(ai.error){if(aj.error){aj.error(ai.error)}}else{if(aj.success){aj.success(ai.result)}}delete ac[ai.id]}},init:function(){if(Y.remote){for(var ag in Y.remote){if(Y.remote.hasOwnProperty(ag)){ad[ag]=ab(Y.remote[ag],ag)}}}aa.down.init()},destroy:function(){for(var ag in Y.remote){if(Y.remote.hasOwnProperty(ag)&&ad.hasOwnProperty(ag)){delete ad[ag]}}aa.down.destroy()}})};b.easyXDM=o})(window,document,location,window.setTimeout,decodeURIComponent,encodeURIComponent);
}


Url_Query = JSONP.urlDecode(window.location.search.substr(1));
if (JSONP.sameHost() && parent.ArgaApi) {
	window.arga_rpc = {
		argacomplete: function(){
			parent.ArgaApi.argacomplete();
		}
	};
} else {
	//////////////////////////////////////////////////////////////////////////////
	// easyXDM for communicating with Launch Pad frameset if it's in cross domain
	try {
		window.arga_rpc = new easyXDM.Rpc({
			//remote: "http://localhost:1192/myers10e/portal/94447/" // the path to the provider
			//remote: "*" // the path to the provider
		}, 
		{
			remote: {
				argacomplete:{
					
				}
			}
		});

		ARGA_Private_Fns.Report("RPC object successfully instantiated");

	} catch(e) {
		ARGA_Private_Fns.ReportErrorToPx("Error instantiating RPC object: " + e.message);
	}
}
// make sure console.log is defined
if (window.console == null) {
	console = new Object();
	console.log = new Function();
}