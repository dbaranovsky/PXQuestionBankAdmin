<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MetaDataElement>" %>

<select name="<% =Model.Name%>" id="<% =Model.Name%>" >
	<option value="<% =Model.ElasticData.InternalValue %>" selected="selected"><% =Model.ElasticData.InternalValue %></option>
</select>


<script language="javascript" type="text/javascript" >

	var ddPopulated_<%=Model.Name %> = "false";
	
	var ddUrl_<%=Model.Name %> = "<%= Url.Action( "PopulateMetaDataDropDownFor_" + Model.Name , "AdminMetaData" )%>";

	var ddResult_<%=Model.Name %>;
	
	PopulateMetaDataDropDownFor_<% = Model.Name %>();

	//***********************************
	// PopulateMetaDataDropDown
	//***********************************
	function PopulateMetaDataDropDownFor_<% = Model.Name %>() {
		if (ddPopulated_<%=Model.Name %> == "false") 
		{$.ajax(
				{   type: "GET",
					url: ddUrl_<%=Model.Name %>,
					data: { data: "<% = Model.DefaultValue %>" },
					success: 
						function(result) {
							if (result != null) {
								ddResult_<%=Model.Name %> = result; //cache full result													
								
								 populateDropdown($('#<%=Model.Name %>'), result);	
								 								
								ddPopulated_<% =Model.Name %> = "true";
							}
						}
				});
		}
	 };


	function populateDropdown(select, data) { 
				$.each(data, function(key, value) {
					select.append($("<option/>", {value: key, text: value}));   								
			});          
     };   
	

</script>