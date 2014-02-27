<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MetaDataElement>" %>
	
<div id="divDropDownFor_<% = Model.Name%>" >
	<dl id="target_<% = Model.Name%>"  class='cssDropDown'>
		<dt >
			<a href="#" >
				<% 				
					Response.Write(Model.ElasticData.InternalValue != null && Model.ElasticData.InternalValue.ToString().Length != 0 ? Model.ElasticData.InternalValue.ToString() : "None"); 				
				%>
				<span class="value">
				   <% 
					   Response.Write(Model.ElasticData.InternalValue != null && Model.ElasticData.InternalValue.ToString().Length!=0 ? Model.ElasticData.InternalValue.ToString() : "None"); 
				   %>
				</span>
			</a>
		</dt>
	</dl>
</div>

<input type ="hidden" id="<% =Model.Name%>" value="<% =Model.ElasticData.InternalValue %>"/>  
  
<div id="divEdit_<% = Model.Name%>" 
	 style="display:none;width:100%;border:1px solid gray;padding:10px;position:relative;top:-19px;background:white;">
		
	<b>Edit Resource Sub-type</b>
	<br/>
	By updating the resource sub-type, all resources using this sub-type label will also be updated.
	<p>
		<b>Old Value:  </b><span id="oldResourceSubtype"></span>
	</p>	
	<b>New Value:</b>
	<input type="text" id="txtEditNew_<% = Model.Name%>" value=""  style="width:90%"/>
	<br/>
	<div id="msgNewValue_<% = Model.Name%>" style="display: none;color:red;">Please enter a new value before you click Save.</div>
	<p></p>
	<div class="form-footer">
		<input id="btnSaveResouceSubType"  type="button" value="Save"  />
		<a id="lnkCancelEdit" href="#" >Cancel</a>
	</div>
</div>

<div id="divDelete_<% = Model.Name%>" 
	 style="display:none;width:100%;border:1px solid gray;padding:10px;position:relative;top:-19px;background:white;" >
	
	<b>Delete Resource Sub-type:  </b><span id="ResouceSubTypeDelete"></span>
	<br/>
	Deleting this record will remove it from all resources that currently use the classification. <br />
    Are you sure you want to delete this record?
	<input type="hidden" id="ResouceSubTypeToDelete"/>	
	<div class="form-footer">
		<input id="btnDeleteResouceSubType"  type="button" value="Delete" />
		<a id="lnkCancelDelete" href="#">Cancel</a>
	</div>

</div>


<style type="text/css"  >
	
			body { font-family:Arial, Helvetica, Sans-Serif; font-size:0.75em; color:black;}
			.cssDropDown dd, .cssDropDown dt, .cssDropDown ul { margin:0px; padding-left:0px; }
			.cssDropDown dd { position:relative; }
			.cssDropDown a, .cssDropDown a:visited { color:black; text-decoration:none; outline:none;}
			.cssDropDown a:hover { color:black;}
			.cssDropDown dt a:hover { color:black; border:1px solid lightgray;}
			.cssDropDown dt a {background:white url("/content/images/combo_dropdown.png") no-repeat scroll content-box right; 
			                         display:block;padding-left:5px;padding-right:0px;
				                     border:1px solid lightgray; width:auto;line-height: 17px;}
			.cssDropDown dt a span {cursor:pointer; display:block;}
			.cssDropDown dd ul { background:white none repeat scroll 0 0; border:1px solid lightgray; color:black; display:none;
				left:0px; padding:5px 0px; position:absolute; top:0px; width:100%; list-style:none;max-height: 205px; overflow-y: scroll;}
			.cssDropDown span.value { display:none;}
			.cssDropDown dd ul li { clear: both;}
			.cssDropDown dd ul li a { padding-left: 5px; padding:5px; display:block;float: right;line-height: 16px;}
			.cssDropDown dd ul li span { float: left;padding-left: 5px; line-height: 16px;cursor:pointer;  }
			.cssDropDown dd ul li a:hover { background-color:lightgray;}
        
			.cssDropDown img.flag { border:none; vertical-align:middle; margin-left:10px; }
			.flagvisibility { display:none;}
			
</style>

<script language="javascript" type="text/javascript" >

	var ddUrl_<%=Model.Name %> = "<%= Url.Action( "PopulateMetaDataDropDownFor_" + Model.Name , "AdminMetaData" )%>";
	
	var actionUpdate_<%=Model.Name %> = "<%= Url.Action( "Update_" + Model.Name , "AdminMetaData" )%>";
	var actionDelete_<%=Model.Name %> = "<%= Url.Action( "Delete_" + Model.Name , "AdminMetaData" )%>";

	var ddResult_<%=Model.Name %>;
	
	PopulateMetaDataDropDownFor_<% = Model.Name %>();

	////////////////////////////////////////

	$("#lnkCancelDelete").unbind('click').bind("click", function() {
		$("#divDelete_<% = Model.Name%>").hide();
	});

	$("#lnkCancelEdit").unbind('click').bind("click", function() {
		$("#divEdit_<% = Model.Name%>").hide();

	});

	$("#btnDeleteResouceSubType").unbind('click').bind("click", function(e) {
		var resourceSubType=$("#ResouceSubTypeToDelete").val();
		deleteResourceSubType(resourceSubType);	
		$("#divDelete_<% = Model.Name%>").hide();
		
		StopEventPropogation(e);
			
		alert("Please, expect possible system delay to reflect the changes.");		
	});
	
	$("#btnSaveResouceSubType").unbind('click').bind("click", function(e) {
		var newResourceSubType=$("#txtEditNew_<% = Model.Name%>").val();
		var oldResourceSubType=$("#oldResourceSubtype").text();	
	
		if (newResourceSubType.length==0) {
			$("#msgNewValue_<% = Model.Name%>").show();
		}
		else {			
			$("#msgNewValue_<% = Model.Name%>").hide();
			editResourceSubType(oldResourceSubType, newResourceSubType);	
			$("#divEdit_<% = Model.Name%>").hide();

			StopEventPropogation(e);
			
		    alert("Please, expect possible system delay to reflect the changes.");			
		}
	});

	$(document).on("click", function (e) {
		var $clicked = $(e.target);		
		if (!$clicked.parents().hasClass("cssDropDown")) {
			$(".cssDropDown dd ul").hide();
		}
	});

	///Handle Edit and Delete Resource Sub Types:
	$(document).off('click',".cssDropDown dd ul li a").on("click", ".cssDropDown dd ul li a", function (e) {
		$("#divDelete_<% = Model.Name%>").hide();
		$("#divEdit_<% = Model.Name%>").hide();
		
		e.cancelBubble = true;
		e.stoppropogation = true;
		e.stopImmediatePropagation();
		
		var $linkAction = $(e.target);
		var resourceSubType = $linkAction.closest('li').text();
		var action = $linkAction.text();
		resourceSubType = resourceSubType.replace("DeleteEdit", "");

		if (action == "Edit") {
			showEditResourceSubType(resourceSubType);
		}
		if (action == "Delete") {
			showDeleteResourceSubType(resourceSubType);
		}						
		$(".cssDropDown dd ul").hide();
	});


    $(document).on('click', ".cssDropDown dd ul li span").on('click', ".cssDropDown dd ul li span", function () {
		$("#divDelete_<% = Model.Name%>").hide();
		$("#divEdit_<% = Model.Name%>").hide();

		var text = $(this).html();
		$(".cssDropDown dt a").html(text);
		$(".cssDropDown dd ul").hide();	
		
		var val = jQuery.trim(text);
		$("#<%=Model.Name %>").attr("value", val );	
		
		if (jQuery($('#<%=Model.Name %>')).attr("id") == "ResourceSubType" ) {		
			changeResourceUserGroup();								 	
		} 		
	});

    $(document).off('click', ".cssDropDown dt a").on('click', ".cssDropDown dt a", function () {
		$("#divDelete_<% = Model.Name%>").hide();
		$("#divEdit_<% = Model.Name%>").hide();
		$(".cssDropDown dd ul").toggle();
	});


	function StopEventPropogation(e) {
			e.cancelBubble = true;
			e.stoppropogation = true;
			e.stopImmediatePropagation();
	}

	//***********************************
	// PopulateMetaDataDropDown
	//***********************************
	function PopulateMetaDataDropDownFor_<% = Model.Name %>() {
		$.ajax(
				{   type: "GET",
					url: ddUrl_<%=Model.Name %>,
					data: { data: "<% = Model.DefaultValue %>" },
					success: 
						function(result) {
							if (result != null) {
								ddResult_<%=Model.Name %> = result; //cache full result													
								 if (jQuery($('#<%=Model.Name %>')).attr("id") == "ResourceSubType" ) {		
								 	populateResourceSubType();								 	
								 } else {
								 	populateDropdown($('#select_<%=Model.Name %>'), result);	
								 }								
							}
						}
				});		
	 };
	
	///////////////////////////////////////
	function populateResourceSubType() {
		var result = ddResult_<%=Model.Name %>;
		if (result != null) 
		{
			createDropDown(result);
			ddPopulated_<% =Model.Name %> = "true";
		}
	};
	
   function changeResourceUserGroup() {
	    $("input:radio[name='ResourceUserGroup']").each(function () {
            var UserGroupVal = $(this).val();
            var array = UserGroupVal.split('_');
            var firstValue = array[0];
            var newVal = firstValue + '_' + $("#<%=Model.Name %>").val();
            $(this).val(newVal);
        });
	};

	function showEditResourceSubType( resourceSubType) {
		$("#txtEditNew_<% = Model.Name%>").attr("value", "");
		$("#oldResourceSubtype").html(resourceSubType);
		$('#divEdit_<% = Model.Name%>').show();
	};

	function showDeleteResourceSubType( resourceSubType) {		
		$("#ResouceSubTypeToDelete").attr("value", resourceSubType);
		$("#ResouceSubTypeDelete").html(resourceSubType);
		$('#divDelete_<% = Model.Name%>').show();		
	};

	function deleteResourceSubType(resourceSubType) {
				{$.ajax(
				{   type: "POST",
					url: actionDelete_<% = Model.Name %>,
					data: { deleteValue: resourceSubType },
					success: function(result) {
							if (result != null) {
								ddResult_<% = Model.Name %> = result;
								populateResourceSubType();	
							}
						}
				});
		}		
		$(".cssDropDown dt a").html("None");
		$("#<%=Model.Name %>").attr("value", '' );	
		changeResourceUserGroup();
		PopulateMetaDataDropDownFor_<% = Model.Name %>();
		
	};

    function editResourceSubType(oldResourceSubType,newResourceSubType ) {  	
    	var userGroup ='student_' ;
    	if ($('#ResourceUserGroup').val().indexOf('instructor_') >-1) userGroup = 'instructor_';
				{$.ajax(
				{   type: "POST",
					url: actionUpdate_<% = Model.Name %>,
					data: { oldValue: (oldResourceSubType) , newValue: (userGroup + newResourceSubType) },
					success: function(result) {
							if (result != null) {
								ddResult_<% = Model.Name %> = result;
								populateResourceSubType(newResourceSubType);	
							}
						}
				});
		}    	
		$(".cssDropDown dt a").html(newResourceSubType);
		$("#<%=Model.Name %>").attr("value", newResourceSubType );
    	changeResourceUserGroup();
    	PopulateMetaDataDropDownFor_<% = Model.Name %>();
	};

	function createDropDown(data) {			
			$("#target_<% = Model.Name%>").append('<dd><ul></ul></dd>');
			 
			$.each(data, function(key,value)  {
				var liValue = "<li><span>" + value + "</span>";
				liValue = liValue + "<a href='#' >Delete</a>"; 
				liValue = liValue + "<a href='#' >Edit</a>";     
				liValue = liValue + "</li>";				
				$("#target_<% = Model.Name%> dd ul").append(liValue);				
			});
	}

	function populateDropdown(select, data) { 
				$.each(data, function(key, value) {
					select.append($("<option/>", {value: key, text: value}));   								
			});          
     };   
	

</script>