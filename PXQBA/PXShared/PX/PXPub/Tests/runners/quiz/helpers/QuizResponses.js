var QuizResponses = {
    questions: {
        success: {
            status: 200,
            contentType: "application/html",
            responseText: '<ul><li class="question" id="68C8CC49582AB2AAA2DE51DC170402C8"></li></ul>'
        }
    },
    savequestions: {
        success: {
            status: 200,
            contentType : "application/html",
            responseText: '<ul><li class="question" id="68C8CC49582AB2AAA2DE51DC170402C8"></li><li></li><li></li><li></li><li></li></ul>'
        }
    },
    points: {
        success: {
            status: 200,
            contentType: "application/html",
            responseText: '<ul><li class="question" id="68C8CC49582AB2AAA2DE51DC170402C8"></li></ul>'
        }
    },
    questionSettings: {
        success: {
            status: 200,
            contentType: "application/html",
            responseText: '<div id="edit-quiz-settings-form">' +
                '<input type="hidden" value="68C8CC49582AB2AAA2DE51DC170402C8" name="Id"></input>' +
                '<input type="hidden" value="0afa3ca4e1ea4259bd30d00170879711" name="QuizId"></input>' +
                '<div class="question-settings-form">' +
                'Points:<input type="textbox" id="Points" text="3" data-val-number="" value="3" name="Points"></input></div></div>'
        }
    },
    
    useElseWhere: {
        success: {
            status: 200,
            contentType: "application/html"
            
        }
    },
    
    addQuestionToQuiz: {
        success: {
            status: 200,
            contentType: "application/json",
            responseText:'{"message" : "Question was added to quiz", "success":"true"}'
        }

    },
    
    addQuestionToQuiz_fail: {
        success: {
            status: 200,
            contentType: "application/json",
            responseText: '{"message" : "Question not added to quiz", "success":""}'
        }

    },

    homeworkQuestionSettings: {
        success: {
            status: 200,
            contentType: "application/html",
            responseText: '<div id="edit-settings-form" >' +
               '<input type="hidden" value="68C8CC49582AB2AAA2DE51DC170402C8" name="Id"></input>' +
               '<label for="AttemptLimit">Number of attempts</label>:' +
               '<select name="NumberOfAttempts.Attempts"> <option value="0">Unlimited</option><option selected="selected" value="3">3</option></select>' +
               '<label for="Points">Points possible</label>: <input type="text" value="2" name="Points" maxlength="3" id="Points" data-val-required="The Points possible field is required." data-val-number="The field Points possible must be a number." data-val="true">'

        }
    }
};

