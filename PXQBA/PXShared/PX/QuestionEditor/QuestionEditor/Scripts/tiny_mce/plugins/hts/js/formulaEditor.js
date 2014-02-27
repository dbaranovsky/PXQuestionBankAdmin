tinyMCEPopup.requireLangPack();

var FormulaEditorDialog = {
    init: function () {
        //        var f = document.forms[0];

        //        // Get the selected contents as text and place it in the input
        //        f.someval.value = tinyMCEPopup.editor.selection.getContent({ format: 'text' });
        //        f.somearg.value = tinyMCEPopup.getWindowArg('some_custom_arg');
    },

    insert: function (formulaText) {
        var formulaImgNode = tinyMCEPopup.getWindowArg('equation_image');
        var formulaImgBaseUrl = tinyMCEPopup.getWindowArg('equation_image_path');
        var questionEditorHelper = window.parent.QuestionEditorHelper;
        var bookmark = tinyMCEPopup.getWindowArg('current_bookmark');
        if (questionEditorHelper == undefined) {
            questionEditorHelper = window.parent.parent.QuestionEditorHelper;
        }

        //clean up the text returned back from flash player
        formulaText = formulaText.replace(/&emptyop;/g, ' '); /* '&nbsp;'); */
        formulaText = formulaText.replace(/</g, '&lt;');
        formulaText = formulaText.replace(/>/g, '&gt;');

        //formulaText = escapeHTML(formulaText);
        formulaText = encodeURIComponent(formulaText);
        formulaText = formulaText.replace(/'/g, "%27");

        var formulaImgSrc = formulaImgBaseUrl + "?eqtext=" + formulaText;

        if (formulaImgNode) {
            var srcEqOrExpr = formulaImgNode.getAttribute('src');
            if (srcEqOrExpr.match(/\?exprtext=/)) {
                formulaImgSrc = formulaImgBaseUrl + "?exprtext=" + formulaText;
            }
            formulaImgNode.setAttribute('src', formulaImgSrc);
            formulaImgNode.setAttribute('data-mce-src', formulaImgSrc);

            formulaImgNode.setAttribute('alt', formulaImgSrc);
            formulaImgNode.setAttribute('hts-data-equation', formulaText);
            removeClass(formulaImgNode, 'hts-formula-input');
        }
        else {
            questionEditorHelper.restoreBookMark(bookmark);
            // Insert the contents from the input into the document
            var strFormula = 'formula';
            var img = "<img src='" + formulaImgSrc + "' hts-data-equation='" + formulaText + "' hts-data-type='" + strFormula + "' ></img>";
            tinyMCEPopup.editor.execCommand('mceInsertContent', false, img);
        }
    },

    close: function () {
        var element = document.getElementById("formula-editor-wrapper");
        element.parentNode.removeChild(element);
        tinyMCEPopup.close();
    },

    getFormulaSelection: function () {
        var formulaImgNode = tinyMCEPopup.getWindowArg('equation_image');
        var formulaText = "";

        if (formulaImgNode) {
            var attr = formulaImgNode.getAttribute('hts-data-equation');
            if (attr && typeof attr !== 'undefined' && attr !== false) {
                attr = attr.replace(/\+/g, '');
                formulaText = decodeURIComponent(attr);
                //formulaText = encodeURIComponent(formulaText)
            }
        }

        return formulaText;
    }
};

tinyMCEPopup.onInit.add(FormulaEditorDialog.init, FormulaEditorDialog);

// call from eqEditor to initialize eqEditor text
function eqEditorInitText() {
    return FormulaEditorDialog.getFormulaSelection();
};

// callback from eqEditor flash movie for submit button
function eqSubmit(value) {
    FormulaEditorDialog.insert(value);
    FormulaEditorDialog.close();
};

// callback from eqEditor flash movie for cancel button
function eqCancel() {
    FormulaEditorDialog.close();
};

function setVariables() {
    //FormulaEditorDialog.close();
    return '$num1';
};

function isPageReady() {
    return true;
};

function htmlEncode(str) {
    var div = document.createElement('div');
    var text = document.createTextNode(str);
    div.appendChild(text);
    return div.innerHTML;
};

function htmlDecode(str) {    
    var div = document.createElement('div');
    div.innerHTML = str;
    return div.firstChild.data;
}; 

function hasClass(ele, cls) {
    return ele.className.match(new RegExp('(\\s|^)' + cls + '(\\s|$)'));
}
function addClass(ele, cls) {
    if (!this.hasClass(ele, cls)) ele.className += " " + cls;
}
function removeClass(ele, cls) {
    if (hasClass(ele, cls)) {
        var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
        ele.className = ele.className.replace(reg, ' ');
    }
}
