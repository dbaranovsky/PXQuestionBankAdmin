/**
* @projectDescription   Wrapper functions for using the ARGA Framework.
*
* Defines some wrapper functions to make it easy for content developers to use 
* this interface without knowing anything about SCORM.
*
* @author   Pepper
* @version  3.0
*
* This version makes the ARGA functions wrappers for the Platform X (Agilix OLS)
* SCORM functions, which have versions that work whether or not
* the activity files exist on the same server as the OLS
**/

// minimize with:
// http://developer.yahoo.com/yui/compressor/#work

// make sure we have the necessary agilix javascript libraries
//document.write('<script src="http://xxx/DataExchange.js" type="text/javascript" language="Javascript"></scr' + 'ipt>');
document.write('<script src="/BFWglobal/js/DEJS/DataExchange.js" type="text/javascript" language="Javascript"></scr' + 'ipt>');
var ARGA_API = null;

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
	*    GET_FN('un0') = 'blah'
	*    GET_FN('foo') = 'bar'
	* The function converts all property names to lower case, so that everything is case-insensitive
	* @param {String} property GET property whose value you want to retrieve
	* @param {Object} win Current window object
	*/
	GET_FN: function(property) {
		// Return the specified property's value
		return GET_OBJ[property.toLowerCase()];
	},
		
	ARGA_version: "3.0",
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
		ARGA_Private_Fns.Report("not initialized");
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
	// XXX CHECK
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
		// case "entry_id": return api.entry_id;	// xxx this was angel-specific; do any activities actually use it?
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

		// If we make it to here, it's not a predefined thing
		arr[j] = ARGA_Private_Fns.GetValue(api, "cmi.comments_from_learner." + scorm_index + ".comment");
	}
	
	// return the array
	return arr;
}


/**
* Submit a question response
* If an SCO uses this function to store info, BFW's Angel implementation will format it nicely
* in the report
*
* @param  {String}  questionNum     Question number. There is no format specified for this; it can be, e.g. "1", "1.1", "Essay 1", etc.
* @param  {String}  questionType    Up to SCO to define; it will be displayed to the instructor, so make it readable (e.g. "multiple choice")
* @param  {String}  questionText    Text of question as presented to student (to be displayed to instructor when viewing results)
* @param  {String}  correctAnswer   Text of correct answer (if a MC question, write out the correct response; e.g. "Venus")
* @param  {String}  learnerResponse Student's response (if a MC question, write out the response given, e.g. "Mars")
* @param  {int}     questionGrade   0-100 percentage, or -1 if can't be scored
* @param  {int}     questionWeight  Percentage of the total grade for the activity. (Percentages have to be hard-coded into the SCO; not configurable by profs at this time.)
* @param  {String}  questionData	    Optional parameter to store info needed to recreate student's experience, e.g. a random number seed
* @return {Boolean}                 true/false
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
		ARGA_Private_Fns.Report("not initialized");
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
	
	// questionGrade goes in result
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
* @return {Boolean, NULL}       true if success, otherwise NULL
*/
function Get_ARGA_LearnerResponse(questionNum, learner_id) {
	return ARGA_Private_Fns.GetQuestionData("learnerResponse", questionNum, learner_id);
}

/**
* Retrieve an array with learnerResponse's for all class members for the questionNum
*
* For this function to work, "retrieve_class_data:1" must have been sent in
* to Initialize_ARGA_Session()

xxx TODO: implement this
*/
function Get_ARGA_LearnerResponse_Class(questionNum) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.Report("not initialized");
		return [];
	}
	
	// if we didn't get class_info, return empty array
	if (ARGA_API.class_info == null) {
		return "";
	}
	
	var arr = new Array();

	for (var j = 0; j < ARGA_API.class_info.length; ++j) {
		// by default, fill in empty string for the learner
		arr[j] = "";
		
		// Look for the question; if found, fill in the response
		for (var i = 0; i < ARGA_API.class_info[j].data.length; ++i) {
			var d = ARGA_API.class_info[j].data[i];
			if (d.type == "question" && d.questionNum == questionNum) {
				arr[j] = d.learnerResponse;
			}
		}
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
	if (g == "") {
		return -1;
	} else {
		return g;
	}
}

/**
* Set a 0-100 percentage grade (e.g. 80 for 80%) for the entire activity
* @param  {String}  grade 
* @return {Boolean}
*/
function Set_ARGA_Grade(grade) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.Report("not initialized");
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
				// if questionWeight is -1, use 100 (so that we would treat everything equally)
				if (w == -1) {
					w = 100;
				}

				// get grade entered
				var questionGrade = ARGA_Private_Fns.GetValue(ARGA_API, "cmi.interactions." + i + ".result");
				
				// If there is no grade entered, nothing will be added, so it'll be counted as 0
				if (questionGrade == null || questionGrade == "") {
					// Add weight in to total in this case, because we want it to count against the student
					totalWeight += w;
				} else {
					// If questionGrade for this question is set to -1, it can't be graded here
					if (questionGrade == -1) {
						// if 'grade_partial' is set to 'yes', grade based on the other questions
						// otherwise, return an overall grade of -1, meaning "complete but no grade"
						if (Get_ARGA_Data('grade_partial') != 'yes') {
							ARGA_API.grade = -1;
							return true;
						}

					// Otherwise add questionGrade, weighted by weight, to totalPoints
					} else {
						totalPoints += (questionGrade * w);
						// also Add weight in to total in this case
						totalWeight += w;
					}
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
*/
function Get_ARGA_Grade(learner_id) {
	if (!ARGA_VARS.ARGA_initialized) {
		ARGA_Private_Fns.Report("not initialized");
		return -1;
	}
	
	// get designated learner's grade if specified; otherwise the current user
	var api;
	if (learner_id != null) {
		api = ARGA_Private_Fns.Get_API_For_Learner(learner_id);
	} else {
		api = ARGA_API;
	}
	
	if (api == null || !api.grade) {
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
		ARGA_Private_Fns.Report("not initialized");
		return false;
	}
	
	return ARGA_Private_Fns.Save_Data_To_LMS(args);
}

/**
* Get question data from another page, indexed by a "pageId".
* This is used in Portal for, e.g., retrieving data entered on a different activity
* for use in this activity.
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
	
	// The caller's Get_ARGA_QuestionData_For_PageId_Callback will be called when Get_Data_From_LMS is done
}

// ------------------------------------------------------------------
// Private functions
var ARGA_Private_Fns = function () {
    // PUBLIC VARS AND FUNCTIONS
    return {

        Get_API_For_Learner: function (learner_id) {
            for (var j = 0; j < ARGA_API.class_info.length; ++j) {
                if (ARGA_API.class_info[j].learner_id == learner_id) {
                    return ARGA_API.class_info[j];
                }
            }
            return null;
        },

        // get a value out of the API, using the appropriate method
        GetValue: function (API, name) {
            // use scorm_api if we have it
            if (API.scorm_api != null) {
                // ARGA_Private_Fns.Report("GetValue: we have scorm api");
                var result = API.scorm_api.GetValue(name);
                ARGA_Private_Fns.Report("GetValue(" + name + ") returned '" + result + "'");

                return result;

                // otherwise use dejs_data if we have it
            } else if (API.dejs_data != null) {
                // xxx CHECK: we have to go through each data field in a loop here, which seems inefficient; maybe
                // we should at least be storing data more efficiently as we go...
                for (var i = 0; i < API.dejs_data.length; ++i) {
                    var d = API.dejs_data[i];
                    if (d.name == name) {
                        return d.value;
                    }
                }
                // if we make it through the loop we couldn't find the data, so return "" below
            }

            // if we make it to here return empty string
            return "";
        },

        GetQuestionData: function (which, questionNum, learner_id) {
            if (!ARGA_VARS.ARGA_initialized) {
                ARGA_Private_Fns.Report("not initialized");
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
        SetValue: function (name, value) {

            // use scorm_api if we have it
            if (ARGA_API.scorm_api != null) {
                ARGA_API.scorm_api.SetValue(name, value);

                try {
                    var code = ARGA_API.scorm_api.GetLastError("");
                    if (code != '0') {
                        ARGA_Private_Fns.Report("SetValue(" + name + ", " + value + ") error: " + ARGA_API.scorm_api.GetDiagnostic());
                    }
                } catch (e) { }

                // otherwise use dejs_data if we have it
            } else if (ARGA_API.dejs_data != null) {
                // make sure we only set each name once, so go through all existing items
                for (var i = 0; i < ARGA_API.dejs_data.length; ++i) {
                    var d = ARGA_API.dejs_data[i];

                    // if this item has the given name,
                    if (d.name == name) {

                        // set to the new value
                        d.value = value;

                        // mark this item as something we have to send to the server when Save_ARGA_Data is called
                        // store the deja_data index there to make it easy to pull it out in Save_Data_To_LMS
                        ARGA_API.items_to_save[name] = i;
                        return;
                    }
                }
                // If we get to here, the key isn't already there, and i = ARGA_API.data.length			
                // set to the new key/value

                var ind = ARGA_API.dejs_data.length;

                d = ARGA_API.dejs_data[ind] = new Object();
                d.name = name;
                d.value = value;
                // mark this item as something we have to send to the server when Save_ARGA_Data is called
                // store the deja_data index there to make it easy to pull it out in Save_Data_To_LMS
                ARGA_API.items_to_save[name] = ind;

                return;
            }
        },

        /**
        * Get data using LMS-specific method
        * @param  {Object} ARGA API object
        */
        // Receives an API object (ARGA_API for the main page, but could be a
        // different one from Get_ARGA_QuestionData_For_PageId)
        Get_Data_From_LMS: function (tempAPI, args) {
            var nFindAPITries = 0;
            var maxTries = 500;

            // The ScanForAPI() function searches for an object named API_1484_11
            // in the window that is passed into the function.  If the object is
            // found a reference to the object is returned to the calling function.
            // If the instance is found the SCO now has a handle to the LMS
            // provided API Instance.  The function searches a maximum number
            // of parents of the current window.  If no object is found the
            // function returns a null reference.  This function also reassigns a
            // value to the win parameter passed in, based on the number of
            // parents.  At the end of the function call, the win variable will be
            // set to the upper most parent in the chain of parents.
            function ScanForAPI(win) {
                while ((win.API_1484_11 == null) && (win.parent != null) && (win.parent != win)) {
                    nFindAPITries++;
                    if (nFindAPITries > maxTries) {
                        return null;
                    }
                    win = win.parent;
                }
                return win.API_1484_11;
            }

            // The findApiHost() function begins the process of searching for the LMS
            // provided API Instance.  The function takes in a parameter that
            // represents the current window.  The function is built to search in a
            // specific order and stop when the LMS provided API Instance is found.
            // The function begins by searching the current window’s parent, if the
            // current window has a parent.  If the API Instance is not found, the
            // function then checks to see if there are any opener windows.  If
            // the window has an opener, the function begins to look for the
            // API Instance in the opener window.
            function findApiHost(win) {
                try {
                    if ((win.parent != null) && (win.parent != win)) {
                        return ScanForAPI(win.parent);
                    }
                    if ((API == null) && (win.opener != null)) {
                        return ScanForAPI(win.opener);
                    }
                } catch (e) { }
            }

            function findApiHost_tmp(win) {
                try {
                    // Look for the API first on this window
                    if (win.API != null) {
                        return win.API;
                    } else if (win.API_1484_11 != null) {
                        return win.API;
                    }
                    // also try the top, if it's in a content player it might be there
                    if (top.window.API != null) {
                        return top.window.API;
                    } else if (top.window.API_1484_11 != null) {
                        return top.window.API_1484_11;
                    }
                    // Try looking on frameset kin
                    if (top.window.frames.length > 0) {
                        for (var i = 0; i < top.window.frames.length; i++) {
                            if (top.window.frames[i].API != null) {
                                return top.window.frames[i].API;
                            } else if (top.window.frames[i].API_1484_11 != null) {
                                return top.window.frames[i].API_1484_11;
                            }
                        }
                    } 			/*
				// Try looking on frameset kin
				if (win.frames.length > 0) {
					for (var i=0; i<win.frames.length; i++) {
						if (win.frames[i].API != null)  {
							return win.frames[i].API;
						}
					}
				}
				*/
                    // Try parent
                    if (win.parent != win) {
                        return findApiHost(win.parent);
                    }
                    // else give up
                    return null;
                } catch (e) {
                    return null;
                }
            }

            // XXX TODO: deal with alt. page_ids, which would be in tempAPI.page_id

            // Try to get access to the SCORM API directly via js
            // this should work if the activity is running on the PX server
            var scorm_api = findApiHost(window);

            // xxx TODO: deal with class data
            // if (args.retrieve_class_data == true || args.retrieve_class_data == '1') {
            //		if (args.retrieve_class_data_rights != null) {

            // push tempAPI onto args so it'll get passed through to the callback fn one way or the other below
            if (args == null) {
                args = new Object();
            }
            args.tempAPI = tempAPI;

            // If we found the API this way, we can get the data we need right here
            if (scorm_api != null) {
                // first we need to initialize SCORM
                var result = scorm_api.Initialize("");

                if (!result) {
                    ARGA_Private_Fns.Report("SCORM failed to initialize");
                    return false;
                }

                // store the reference to the scorm_api in tempAPI
                tempAPI.scorm_api = scorm_api;
                ARGA_Private_Fns.Get_Data_From_LMS_Callback(args, true, null);

                // else we need to use the "Data Exchange Javascript API"
            } else {
                // initialize DEJS
                var result = DEJS_API.initialize();
                if (!result) {
                    ARGA_Private_Fns.Report("DEJS_API failed to initialize");
                    return false;
                }
                // initialize the items_to_save array, used in SetValue and Save_Data_To_LMS
                tempAPI.items_to_save = new Array();
                
                // push additional paramters into args, then use it for the options in DEJS_API.getData
                args.callback = ARGA_Private_Fns.Get_Data_From_LMS_Callback;
                args.timeout = 15000;
                result = DEJS_API.getData(args);
                if (!result) {
                    ARGA_Private_Fns.Report("DEJS_API getData failed to run");
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
        Get_Data_From_LMS_Callback: function (args, success, data) {
            if (!success) {
                ARGA_Private_Fns.Report("DEJS_API getData's ajax call failed");
                return;
            }

            // if we make it to here we've got the SCORM data one way or the other

            var tempAPI = args.tempAPI;

            // receive data if it came in (it might be null, which is fine)
            tempAPI.dejs_data = data;

            // record user info
            tempAPI.learner_id = ARGA_Private_Fns.GetValue(tempAPI, "bh.user_id");
            tempAPI.learner_name = ARGA_Private_Fns.GetValue(tempAPI, "bh.user_display");
            tempAPI.course_id = ARGA_Private_Fns.GetValue(tempAPI, "bh.course_id");
            tempAPI.user_rights = ARGA_Private_Fns.GetValue(tempAPI, "bh.enrollment_rights");
            // xxx TODO: translate enrollment_rights into arga versions: 1_student, 2_course_assistant, 3_instructor

            // record timezone-adjusted user due date and whether due date has passed or not
            tempAPI.user_due_date = ARGA_Private_Fns.GetValue(tempAPI, "bh.item_due_date");
            // xxx TODO: calculate due_date_has_passed (which means we're doing it client side, which is not good...)
            tempAPI.due_date_has_passed = 0;

            // User's current grade for the activity
            var scorm_grade = ARGA_Private_Fns.GetValue(tempAPI, "cmi.score.scaled");
            // if this is an empty string, grade should be an empty string
            if (scorm_grade == "") {
                tempAPI.grade = "";
            } else {
                tempAPI.grade = scorm_grade * 100; // SCORM is 0-1; ARGA uses 0-100
            }

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
                if (tempAPI.due_date_has_passed == 0 && parseFloat(tempAPI.grade) >= 0) {
                    msg += 'You have completed this activity. You may change your answers and re-submit if you like.';
                } else if (tempAPI.due_date_has_passed == 1 && parseFloat(tempAPI.grade) < 0) {
                    msg += 'The due date for this assignment (' + LF(activity_due_date) + ') has now passed.  You may review the materials in the activity, but your submission will not be recorded and you will not receive a grade.';
                } else if (tempAPI.due_date_has_passed == 1 && parseFloat(tempAPI.grade) >= 0) {
                    msg += 'You have completed this activity. The due date for this assignment (' + LF(activity_due_date) + ') has now passed.  You may review the materials in the activity, but further submissions will not be recorded.';
                }
                if (msg != "") {
                    alert(msg);
                }
            }

//            ARGA_Private_Fns.Report("Get_Data_From_LMS_Callback feedback: " + tempAPI.toSource());

            // call application-defined callback function if there
            if (window.Initialize_ARGA_Session_Callback != null) {
                Initialize_ARGA_Session_Callback(true);
            }
        },


        /**
        * Saves data using whichever method we have at our disposal
        */
        Save_Data_To_LMS: function (args) {
            // all data will already have been stored to scorm or dejs by SetValue, except the final grade
            if (ARGA_API.grade == -2) {
                // have to find out what to do here
            } else if (ARGA_API.grade == -1) {
                // have to find out what to do here
            } else {
                ARGA_Private_Fns.SetValue("cmi.score.scaled", ARGA_API.grade / 100); // SCORM is 0-1; ARGA uses 0-100
            }

            // DC we for most LMS SCORM implementations, we need to set cmi.exit to "suspend"
            // possible values for cmi.exit are timeout, suspend, logout, normal, and empty string
            // If the cmi.exit is set to “normal”, “logout”,“time-out” or “” (empty characterstring)
            // then the SCOs learner attempt ends. The SCOs Run-Time
            // Environment data model element values of the current learner session will NOT
            // be available if the SCO is relaunched
            ARGA_Private_Fns.SetValue("cmi.exit", "suspend");

            // if we got a "show_progress:true" argument, block the screen
            if (args != null && args.show_progress == true) {
                var progress_width = 200;
                var progress_left = Math.round((window.innerWidth - progress_width) / 2);
                // get scroll position, which IE makes annoying (and we're making do without jquery here)
                var scrollTop = 0;
                if (document.body && document.body.scrollTop > 0) {
                    scrollTop = document.body.scrollTop;
                } else if (document.documentElement && document.documentElement.scrollTop > 0) {
                    scrollTop = document.documentElement.scrollTop;
                }

                var progress_top = scrollTop + 100;

                // if the ARGA_ajax_save_div is already there, just adjust its properties
                var d = document.getElementById('ARGA_ajax_save_div');
                if (d != null) {
                    d.style.left = progress_left + "px";
                    d.style.top = progress_top + "px";
                    d.style.display = "block";

                    // else insert it into the html
                } else {
                    var progress_html = "<div id='ARGA_ajax_save_div' style='position:absolute; z-index:99999; left:" + progress_left + "px; top:" + progress_top + "px; width:" + progress_width + "px; text-align:center; background-color:#fff; border:5px solid #006; padding:10px; color:#000; font-weight:normal; font-family:Verdana, sans-serif; font-size:14px;'><img border='0' src='http://angel.bfwpub.com/AngelThemes/icons/map/loading.gif'> Saving data...</div>";
                    
                    // PW: simply appending the progress_html to document.body breaks events!
                    // so instead, let's use jquery, first testing to make sure it's enabled
                    // (if jquery isn't here, the worst that happens is that the progress indicator isn't shown)
                    // document.body.innerHTML += progress_html;
                    if ($ != null && $('body') != null) {
	                    $('body').append(progress_html);
	                }
                }
            }

            // call the commit or putData function
            // use scorm_api if we have it
            if (ARGA_API.scorm_api != null) {
                // scorm returns "true" or "false" as a string
                // apparently synchronously
                // so, if it succeeds, we call the callback fn
                var commit_success = ARGA_API.scorm_api.Commit("");
                if (commit_success == "true") {
                    ARGA_Private_Fns.Save_Data_To_LMS_Callback(null, true)
                } else {
                    ARGA_Private_Fns.Save_Data_To_LMS_Callback(null, false)
                }
                try {
                    var code = ARGA_API.scorm_api.GetLastError("");
                    if (code == '0') {
                        ARGA_Private_Fns.Report("Commit: No error!");
                    } else {
                        ARGA_Private_Fns.Report("Commit error: " + ARGA_API.scorm_api.GetErrorString(code));
                    }
                } catch (e) { }

                // otherwise use dejs_data if we have it
            } else if (ARGA_API.dejs_data != null) {
                // in this case we have to pull out the specific set of data we want to save
                // the items_to_save associative array was formed in SetValue
                var data_to_store = new Array();
                for (var i = 0; i < ARGA_API.dejs_data.length; i++) {
                    if (ARGA_API.dejs_data[i].name.substring(0, 3) != "bh.") {
                        data_to_store.push(ARGA_API.dejs_data[i]);
                    }
                }
                // DC TEST
                //data_to_store = ARGA_API.dejs_data;
                DEJS_API.putData({
                    callback: ARGA_Private_Fns.Save_Data_To_LMS_Callback
				, timeout: 15000
				, data: data_to_store
                });

            }

            // Assume things will go OK from here...
            return (true);
        },

        Save_Data_To_LMS_Callback: function (options, success) {
            // hide the ARGA_ajax_save_div if it was there
            var d = document.getElementById('ARGA_ajax_save_div');
            if (d != null) {
                d.style.display = 'none';
            }

            if (!success) {
                alert("There was a problem saving your data!");
            } else {
                // reset ARGA_API.items_to_save
                ARGA_API.items_to_save = new Array();

                // call application-defined callback if defined
                if (window.Save_ARGA_Data_Callback) {
                    Save_ARGA_Data_Callback(true);
                }
            }
        },

        /**
        * Reports AGRA activity if debugging is on
        * @param {String} s Report message.
        */
        Report: function (s) {
            if (ARGA_VARS.ARGA_debug) {
                try {
                    top.console.log(s);
                } catch (e) { }
            }

            // if there is a div on the page with id "ARGA_debug_div", put the string there too
            var elm = document.getElementById("ARGA_debug_div");
            if (elm) {
                elm.innerHTML += "<div style='border-top:1px solid #666; padding-top:3px; margin-top:3px'>" + s + "</div>";
            }
        },

        // CALLBACK FUNCTIONS FOR AJAX CALLS
        // xxx TODO: this probably isn't necessary, but something like it might be
        Get_All_Data_AJAX_Callback: function (api_index, learner_id, learner_name, course_id, user_rights, grade, resp_data, user_due_date, due_date_has_passed) {
            var tempAPI = gda_apis[api_index];

            // create class_data array if necessary
            if (tempAPI.class_info == null) {
                tempAPI.class_info = new Array();
            }

            var info = new Object();

            info.learner_id = learner_id;
            info.learner_name = learner_name;
            info.user_rights = user_rights;
            info.grade = grade;
            info.data = UnChunkData(resp_data);
            info.user_due_date = user_due_date;
            info.due_date_has_passed = due_date_has_passed;
            tempAPI.class_info.push(info);
        }

        // Remember: no comma after last item
    }; // end return for public vars and functions
} ();
	
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
