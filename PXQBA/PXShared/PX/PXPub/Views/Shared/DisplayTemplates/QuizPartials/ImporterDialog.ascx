<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div>
    <div class='importer-desc'>
        Using the Respondus format, replace the sample text in the text box with the multiple choice or fill-in-blank questions you want to import.
    </div>

    <a href='#' class='importer-help-link'>
        <span class='importer-help-sel'>+</span>&nbsp;Sample questions
    </a>
    
 
    <div class='importer-help-area' style='display: none'>
        <div class="sample-questions-nav">
            <a href='#' class='importer-sample-multiple-choice active'>Multiple choice</a>
            <a href='#' class='importer-sample-fill-in-blank'>Fill in the blank</a>
        </div>

        <div class="importer-sample-multiple-choice-placeholder">
            <ul>
                <li class="importer-li" style="width: 100%;" tooltip='<b>Question title:</b> The rest of the line is the title of the question. Question titles are optional and when used, must be placed at the beginning of a question.'>Title: The colors of everyday things</li>
                <li class="importer-li" style="width: 100%;" tooltip='<b>Point value:</b> The rest of the line is an integer or decimal point value. This point value will be applied to all subsequent questions until another Points: line is encountered. This line is optional.'>Points: 3</li>
                <li class="importer-li" style="width: 100%;" tooltip='<b>Question stem:</b> Number is the question number, rest of line is the text of the question stem (at least one space should be between the question number and the text).'>1. Which of these statements is true?</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>General feedback:</b> The rest of the line is a feedback string to be given regardless of the student's answer. This line is optional, but if it is used, it must appear directly after a <b>question stem</b> line, before any <b>answer choice</b> lines.">@ General feedback appears here</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">A. Grass is orange</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">B. Sand is green</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Correct answer choice:</b> Same as <b>answer choice</b>, above, but this choice is considered correct. Note: If no correct choice is present for a question, the first <b>answer choice</b> is assumed to be correct. This line is required for multiple choice questions and optional for fill-in-the-blank questions.">*C. The sky is blue</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">D. Firetrucks are purple</li>
            </ul>
        </div>

        <div class="importer-sample-fill-in-blank-placeholder" style="display:none;">
            <ul>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Question type:</b> identificator for Fill in the blank question.">Type F:</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Question stem:</b> Number is the question number, rest of line is the text of the question stem (at least one space should be between the question number and the text).">2. What are the primary colors?</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>General feedback:</b> The rest of the line is a feedback string to be given regardless of the student's answer. This line is optional, but if it is used, it must appear directly after a <b>question stem</b> line, before any <b>answer choice</b> lines.">@ General feedback appears here</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">A. Red</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">B. Green</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">C. Blue</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer choice:</b> Letter is the letter of the answer choice, rest of line is the text of the answer choice. One or more <b>answer choice</b> lines follow a single <b>question stem</b> line.">D. Yellow</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Answer key header:</b> If this line is present in the file, the rest of the file is considered to be an answer key. Each line following this line should be of the form 1. A, where 1 is the number of a question in the file, and A is the letter of the correct answer choice.">Answers:</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Correct answer choice:</b> Same as <b>answer choice</b>, above, but this choice is considered correct. Note: If no correct choice is present for a question, the first <b>answer choice</b> is assumed to be correct. This line is required for multiple choice questions and optional for fill-in-the-blank questions.">2. A</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Correct answer choice:</b> Same as <b>answer choice</b>, above, but this choice is considered correct. Note: If no correct choice is present for a question, the first <b>answer choice</b> is assumed to be correct. This line is required for multiple choice questions and optional for fill-in-the-blank questions.">2. C</li>
                <li class="importer-li" style="width: 100%;" tooltip="<b>Correct answer choice:</b> Same as <b>answer choice</b>, above, but this choice is considered correct. Note: If no correct choice is present for a question, the first <b>answer choice</b> is assumed to be correct. This line is required for multiple choice questions and optional for fill-in-the-blank questions.">2. D</li>
            </ul>
        </div>

        <div class='importer-sample-text'>
        </div>
    </div>

    <br/>
    
<textarea id='importer-text'>
1. Each Question should start with "1" or be successively numbered.
A. If there are choices, the first should start with "A."
*B. Each successive choice should start with the next letter and period.
C. Each choice should be on a single line, but it's OK if it is longer than will fit in the text area and it wraps on it's own.
</textarea>

<div class='importer-response'></div>

</div>