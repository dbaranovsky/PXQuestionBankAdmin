<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="question-editor-step-templates">
    <div class="node step">
        <div class="title">
            <div class="toggle-button">
            </div>
            <div class="delete-button">
            </div>
            <span>Step 1</span>
            <div class="drag-message">
            </div>
        </div>
        <div class="node-body">
            <div class="body sub-nodes">
            </div>
            <div class="node-menu">
                <div class="left">
                </div>
                <div class="right">
                </div>
                <div class="center">
                    <input type="button" name="question" class="button add-question-button" value="Add Question" />
                    <input type="button" name="hint" class="button add-hint-button" value="Add Hint" />
                    <input type="button" name="correct" class="button add-correct-feedback-button" value="Add Correct Feedback" />
                    <input type="button" name="incorrect" class="button add-incorrect-feedback-button"
                        value="Add Incorrect Feedback" />
                </div>
                <div class="clearer">
                </div>
            </div>
        </div>
    </div>
    <div class="node sub-node">
        <div class="title">
            <div class="toggle-button">
            </div>
            <div class="delete-button">
            </div>
            <span></span>
        </div>
        <div class="node-body">
            <textarea class="body" rows="3" cols="3"></textarea>
        </div>
    </div>
    <div class="node solution">
        <div class="title">
            <div class="toggle-button">
            </div>
            <div class="delete-button">
            </div>
            <span>Solution</span>
        </div>
        <div class="node-body">
            <textarea class="body" rows="3" cols="3" id="solution-editor"></textarea>
        </div>
    </div>
    <div class="node preview">
        <div class="title">
            <span>Preview</span>
        </div>
        <div class="node-body">
            <textarea class="body" rows="3" cols="3" id="Textarea1"></textarea>
        </div>
    </div>
    
    <div id="dialog-confirm"></div>    
</div>