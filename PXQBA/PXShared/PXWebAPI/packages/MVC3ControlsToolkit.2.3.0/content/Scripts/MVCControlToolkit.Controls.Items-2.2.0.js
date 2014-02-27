/* ****************************************************************************
*
* Copyright (c) Francesco Abbruzzese. All rights reserved.
* francesco@dotnet-programming.com
* http://www.dotnet-programming.com/
* 
* This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
* and included in the license.txt file of this distribution.
* 
* You must not remove this notice, or any other, from this software.
*
* ***************************************************************************/

////////////////////////////////////SORTABLE and PERMUTATIONS////////////////////////////////
var SortableList_PermutationUpdateRootPrefix = '_PermutationUpdateRoot';
var SortableList_CanSortPrefix = '_CanSort';
var SortableList_ElementsNumberPrefix = '_ElementsNumber';
var SortableList_TemplateSymbolPrefix = '_TemplateSymbol';
var SortableList_TemplateSriptPrefix = '_TemplateSript';
var SortableList_TemplateHtmlPrefix = '_TemplateHtml';
var SortableList_PermutationPrefix = '_Permutation';
var SortableList_PermutationNamePrefix = '.Permutation';
var SortableList_UpdateModelPrefix = '___';
var SortableList_UpdateModelNamePrefix = '.$$';
var SortableList_UpdateModelFieldsPrefix = '_f_ields';
var SortableList_UpdateModelFieldsNamePrefix = '.f$ields';
var SortableList_ItemsContainerPrefix = '_ItemsContainer';
var SortableList_OriginalElementsNumber = '_OriginalElementsNumber';
var SortableList_TemplateHiddenPrefix = '_TemplateHidden';
var SortableList_TemplateHiddenHtmlPrefix = '_TemplateHiddenHtml';
var SortableList_NamePrefixPrefix = '_NamePrefix';
var SortableList_CssPostFix = "_Css";
var SortableList_AltCssPostFix = "_AltCss";

function MvcControlsToolkit_SortableList_ItemName(item) {
     return item.id;
}

function MvcControlsToolkit_SortableList_PrepareTemplate(root, templateId) {
    MvcControlsToolkit_SortableList_PrepareTemplates(root, [templateId]);
}
function MvcControlsToolkit_SortableList_PrepareTemplates(root, templatesId) {
    eval(root + SortableList_TemplateSriptPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHtmlPrefix + ' = new Array();');
    eval(root + SortableList_TemplateHiddenHtmlPrefix + ' = new Array();');
    for (var i = 0; i < templatesId.length; i++) {
        var templateId = templatesId[i];
        var templateElement = $('#' + templateId);
        
        var allJavascript = CollectAllScriptsInelement(templateId);
        eval(root + SortableList_TemplateSriptPrefix + '[i] = allJavascript;');

        $('#' + templateId).find('script').remove();
        var temp=null;
        if (templateElement.hasClass("MVCCT_EncodedTemplate")) 
        {
            temp = templateElement.text();
        }
        else{
            temp = $('<div>').append(templateElement.clone()).remove().html();
        }
        eval(root + SortableList_TemplateHtmlPrefix + '[i] = temp;');

        var hidden = eval(root + SortableList_TemplateHiddenPrefix);
        if (hidden.constructor == Array) {
            hidden = eval(root + SortableList_TemplateHiddenPrefix + '[i]');
        }
        
        temp = $('<div>').append($('#' + hidden).clone()).remove().html();
        eval(root + SortableList_TemplateHiddenHtmlPrefix + '[i] = temp;');

        $('#' + templateId).remove();
    }

    var canSort = eval(root + SortableList_CanSortPrefix);
    if (canSort){
        var elementNumber = eval(root + SortableList_ElementsNumberPrefix);

        var updateModel = document.getElementById(root + SortableList_UpdateModelPrefix + elementNumber );
        updateModel.setAttribute('id', updateModel.id+'_');


        var updateModelFields = document.getElementById(root + SortableList_UpdateModelPrefix + elementNumber  + SortableList_UpdateModelFieldsPrefix);
        updateModelFields.setAttribute('id', updateModelFields.id+'_'); 
    }
   
}
function MvcControlsToolkit_SortableList_AddNew(root, item, after, replace) {
    if (typeof (root) != 'string') {
        root = root.id;
        var end_prefix = root.lastIndexOf("_");
        root = root.substring(0, end_prefix);
    }
    MvcControlsToolkit_SortableList_AddNewChoice(root, 0, item, after, replace);
}
function MvcControlsToolkit_SortableList_AddNewChoice(root, choice, item, after, replace) {
    if (eval("typeof  " + root + SortableList_ElementsNumberPrefix + " === 'undefined'")) return;
    var rootElement = $('#' + root + SortableList_ItemsContainerPrefix);
    var changeData = new MvcControlsToolkit_changeData(null, 'ItemCreating', choice);
    rootElement.trigger('itemChange', changeData);
    if (changeData.Cancel == true) return;
    var elementNumber = eval(root + SortableList_ElementsNumberPrefix);
    var originalElementNumber = eval(root + SortableList_OriginalElementsNumber);
    var templateSymbol = eval(root + SortableList_TemplateSymbolPrefix);
    if (templateSymbol.constructor == Array) {
        templateSymbol = eval(root + SortableList_TemplateSymbolPrefix + '[choice]');
    }
    var hidden = eval(root + SortableList_TemplateHiddenPrefix);
    if (hidden.constructor == Array) {
        hidden = eval(root + SortableList_TemplateHiddenPrefix + '[choice]');
    }
    var allJavascript = eval(root + SortableList_TemplateSriptPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var allHtml = eval(root + SortableList_TemplateHtmlPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var hiddenElementFather = $('#' + hidden).parent();
    var hiddenElement = eval(root + SortableList_TemplateHiddenHtmlPrefix + '[choice]').replace(templateSymbol, elementNumber + '');
    var canSort = eval(root + SortableList_CanSortPrefix);
    var namePrefix = eval(root + SortableList_NamePrefixPrefix);

    elementNumber++;
    eval(root + SortableList_ElementsNumberPrefix + ' = elementNumber;');

    if (canSort) {
        var permutation = document.getElementById(root + SortableList_PermutationPrefix);
        permutation.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber + ".$" + SortableList_PermutationNamePrefix);

        var updateModel = document.getElementById(root + SortableList_UpdateModelPrefix + originalElementNumber + '_');
        updateModel.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber);


        var updateModelFields = document.getElementById(root + SortableList_UpdateModelPrefix + originalElementNumber + SortableList_UpdateModelFieldsPrefix + '_');
        updateModelFields.setAttribute('name', namePrefix + SortableList_UpdateModelNamePrefix + elementNumber + SortableList_UpdateModelFieldsNamePrefix);
    }

    hiddenElementFather.append(hiddenElement);
    var result;
    if (item == null) {
        var hasFooter = false;
        var lastChild = $('#' + root + "_Footer");
        if (lastChild.length > 0) {
            hasFooter = true;
        }
        if (hasFooter) {
            result = $(allHtml).insertBefore(lastChild);
        }
        else {
            result = $(allHtml).appendTo(rootElement);
        }
    }
    else {
        if (after != true) result = $(allHtml).insertBefore(item);
        else result = $(allHtml).insertAfter(item);
        if (replace) {
            MvcControlsToolkit_Bind(item, result[0]);
            $(item).remove();
        }
    }

    if (typeof $ !== 'undefined' && $ !== null && typeof $.validator !== 'undefined' && $.validator !== null && typeof $.validator.unobtrusive !== 'undefined' && $.validator.unobtrusive !== null) {
        jQuery.validator.unobtrusive.parseExt('#' + result[0].id)
    }

    jQuery.globalEval(allJavascript);
    result.data("ScriptsRemoved", true);
    Update_Permutations_Root(root);
    if (canSort) {

        $('#' + root + SortableList_ItemsContainerPrefix).sortable("refresh");
    }
    changeData = new MvcControlsToolkit_changeData(result, 'ItemCreated', choice);
    rootElement.trigger('itemChange', changeData);
    return result;
}
function MvcControlsToolkit_SortableList_ComputeRoot(itemName) {
    var place = itemName.lastIndexOf("___");
    if (place < 0) return null;
    var rootName = itemName.substring(0, place);
    place = rootName.lastIndexOf("___");
    rootName = rootName.substring(0, place);
    if (place < 0) return null;
    return rootName;
}
function MvcControlsToolkit_SortableListUpdate(item, senderId) {
    if (senderId != item.parent().attr('id')) return;
    var nodeName = item.attr('id');
    if (nodeName == null) return;
    var rootName = MvcControlsToolkit_SortableList_ComputeRoot(nodeName);
    Update_Permutations_Root(rootName);
    var rootElement = $('#' + rootName + '_ItemsContainer');
    var changeData = new MvcControlsToolkit_changeData(item, 'ItemMoved', 0);
    rootElement.trigger('itemChange', changeData);
}
function Update_Permutations(itemName) {

    return Update_Permutations_Root(MvcControlsToolkit_SortableList_ComputeRoot(itemName));
}

function Update_Permutations_Root(rootName) {
    if (rootName == null) return;
    var root = document.getElementById(rootName + '_ItemsContainer');
    if (root == null) return;
    if (root.childNodes.length > 0 &&
        root.childNodes[root.childNodes.length - 1].nodeType == 1 &&
        root.childNodes[root.childNodes.length - 1].outerHTML != "" &&
        root.childNodes[root.childNodes.length - 1].id == "") {
        root = root.childNodes[root.childNodes.length - 1];
    }
    var field = document.getElementById(rootName + '_Permutation');
    var alt = false;
    var css = eval(rootName + SortableList_CssPostFix);
    var altCss = eval(rootName + SortableList_AltCssPostFix);
    for (i = 0; i < root.childNodes.length; i++) {
        var currNode = root.childNodes[i];
        var jCurrNode = $(currNode);
        if (jCurrNode.data("ScriptsRemoved") !== true) {
            CollectScriptAndDestroy(currNode);
            jCurrNode.data("ScriptsRemoved", true);
        }
        var nodeId = root.childNodes[i].getAttribute('id');
        if (nodeId == null) continue;
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var ending = nodeId.substring(end_prefix + 1);
        if (ending == 'Header' || ending == 'Footer') continue;
        if (alt) {
            if (css != '') $(root.childNodes[i]).removeClass(css);
            if (altCss != '') $(root.childNodes[i]).addClass(altCss);
        }
        else {
            if (altCss != '') $(root.childNodes[i]).removeClass(altCss);
            if (css != '') $(root.childNodes[i]).addClass(css);
        }
        alt = !alt;
    }
    if (field == null) {

        return;
    }

    var res = '';
    for (i = 0; i < root.childNodes.length; i++) {

        var nodeId = root.childNodes[i].getAttribute('id');
        var end_prefix = nodeId.lastIndexOf("_");
        if (end_prefix < 0) continue;
        var deleteName = nodeId.substring(0, end_prefix + 1) + "Deleted";
        var deletedHidden = document.getElementById(deleteName);
        if (deletedHidden != null && deletedHidden.value == "True") continue;
        var end = nodeId.lastIndexOf("___");
        var order = nodeId.substring(0, end);
        var start = order.lastIndexOf("_") + 1;
        order = order.substring(start);
        if (i > 0) res = res + ',';
        res = res + order;
    }
    field.value = res;
}

function MvcControlsToolkit_SortableList_Click(target, dataButtonType, noEvents) {
    if (typeof (target) != 'string') target = target.id;
    if (dataButtonType == ManipulationButtonCustom) {
        eval(target);
        return;
    }
    var end_prefix = target.lastIndexOf("_");
    var deleteName = target.substring(0, end_prefix);
    end_prefix = deleteName.lastIndexOf("_");
    deleteName = deleteName.substring(0, end_prefix + 1) + "Deleted";
    var deletedHidden = document.getElementById(deleteName);
    var rootName = MvcControlsToolkit_SortableList_ComputeRoot(target);
    var rootElement = $('#' + rootName + SortableList_ItemsContainerPrefix);
    if (dataButtonType == ManipulationButtonRemove) {
        var item = $('#' + target);
        var changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleting', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        item.remove();
        Update_Permutations_Root(rootName);
        changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleted', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
    }
    else if (dataButtonType == ManipulationButtonHide) {
        var item = $('#' + target);
        var changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleting', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        var placeHolder = "<span style='display:none' id = '" + target + "_ph' />";
        $(placeHolder).insertBefore('#' + target);
        jQuery.globalEval("var " + deleteName + " = $('#' + '" + target + "').detach();");
        if (deletedHidden != null) deletedHidden.value = "True";
        Update_Permutations_Root(rootName);
        changeData = new MvcControlsToolkit_changeData(item, 'ItemDeleted', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
    }
    else if (dataButtonType == ManipulationButtonShow) {
        var changeData = new MvcControlsToolkit_changeData(null, 'ItemCreating', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
        if (changeData.Cancel == true) return;
        var toAdd = eval(deleteName);
        toAdd.find('script').remove();
        $(toAdd).insertBefore('#' + target + '_ph');
        $('#' + target + '_ph').remove();
        if (deletedHidden != null) deletedHidden.value = "False";
        Update_Permutations_Root(rootName);
        changeData = new MvcControlsToolkit_changeData(toAdd, 'ItemCreated', 0);
        if (noEvents == null) rootElement.trigger('itemChange', changeData);
    }


}

function MvcControlsToolkit_SortableList_Move(item, target, after) {
    if (after != true) $(item).insertBefore(target);
    else $(item).insertAfter(target);
    var rootName = MvcControlsToolkit_SortableList_ComputeRoot(item.id);
    Update_Permutations_Root(rootName);
    var rootElement = $('#' + rootName + '_ItemsContainer');
    var changeData = new MvcControlsToolkit_changeData(item, 'ItemMoved', 0);
    rootElement.trigger('itemChange', changeData);
}
