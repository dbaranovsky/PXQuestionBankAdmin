<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.MetaDataElement>" %>
<%@ Import Namespace="Bfw.Common.DynamicExtention" %>
<div id="divMain_<% = Model.DefaultValue %>Associations">
	
<% 
  dynamic meta_topics = Model.ElasticData;

  List<ElasticObject> meta_topic_All = meta_topics.meta_topic_All;

  bool noAssociations = true;
	
%>
	  <ol  id="associationsList_<% = Model.DefaultValue %>" > 	
	  <%	 		
	   foreach (dynamic meta_topic in meta_topic_All)
	   {		   
			if(meta_topic.Attribute("toc")==null) continue;
			if (meta_topic.toc != Model.DefaultValue) continue;
			if (meta_topic.toc == Model.DefaultValue) noAssociations = false;
			%>	
			<li>
	   			<%= meta_topic.InternalValue %> 
   				<a id="removeAssociation_<% = Model.DefaultValue %>" href="#" ><img src="<%= Url.Content("~/Content/images/delete.png") %>" alt="Remove association" />Remove association</a> 
				<input type="hidden" name="<%=  Model.Name %>" id="<%=  Model.Name %>"  value="<%= meta_topic.InternalValue + '|' %>"/>
			</li>		
	<%		
	   }	
	%>
	</ol>								
	<%	
		if (noAssociations)
		{
		%>
			<div id="divNoAssociations_<% = Model.DefaultValue %>">No associations</div>
			<br/>
		<%
		}
	%>


		

	</div> 
	<div id="divAssosiations_<% = Model.DefaultValue %>"  >	
		<div id="divAssociatedChapters_<% = Model.DefaultValue %>" style="display:none;">
    		<div id="divChaptersDropDown_<% = Model.DefaultValue %>"></div>
			<input type="button" id="Save_<% = Model.DefaultValue %>" onclick="" value="Save" />   
			<a id="Cancel_<% = Model.DefaultValue %>" href="#" onclick="">Cancel</a>                    			
	   </div>		
	   <div id="divAddAssociationButtons_<% = Model.DefaultValue %>">
		   <input type="button" value="Add Association" id="addAssociation_<% = Model.DefaultValue %>" /> <a href="#" id="associateToAll_<% = Model.DefaultValue %>" >Associate to All Chapters</a>
	   </div>

</div>

	<script type="text/javascript" >
	
		var listItem_<% = Model.DefaultValue %> = '<a href="#" id="removeAssociation_<% = Model.DefaultValue %>"><img src="<%= Url.Content("~/Content/images/delete.png") %>" alt="Remove association">Remove association</a>' +
			'<input type="hidden" name="<%= Model.Name%>" id="<%=Model.Name%>"  value="';

		var show_chapters_route ="<%= Url.Action("AssociatedChaptersDropDown", "AdminMetaData" )%>";

		var done_ChaptersDropDown_<% = Model.DefaultValue %> = "false";
		
		//***********************************
		//Add All Chapters to Associations:
		//***********************************
	    $(document).off('click', '#associateToAll_<% = Model.DefaultValue %>').on('click', '#associateToAll_<% = Model.DefaultValue %>', function() {

			//1 Load DropDown if needed:
			if (done_ChaptersDropDown_<% = Model.DefaultValue %> == "false") {
				PopulateChaptersDropDown_<% = Model.DefaultValue %>("false");
			}

			AssociateToAllChapters_<% = Model.DefaultValue %>();
						
		});

		//***********************************
		//Remove Association
		//***********************************
	    $(document).off('click', '#removeAssociation_<% = Model.DefaultValue %>').on('click', '#removeAssociation_<% = Model.DefaultValue %>', function () {
			$(this).closest('li').remove();
			
			if ($('#associationsList_<% = Model.DefaultValue %>').children().length!= 0) {
				$('#divNoAssociations_<% = Model.DefaultValue %>').empty();
			}

			if ($('#associationsList_<% = Model.DefaultValue %>').children().length== 0) {
				$('#divNoAssociations_<% = Model.DefaultValue %>').append('No associations');
			}
		});

		//***********************************
		//Hide chapters dropdown:
		//***********************************
	    $(document).off('click', '#Cancel_<% = Model.DefaultValue %>').on('click', '#Cancel_<% = Model.DefaultValue %>', function () {
			$('#divAssociatedChapters_<% = Model.DefaultValue %>').hide();
			$('#divAddAssociationButtons_<% = Model.DefaultValue %>').show();
		});
			
		//***********************************
		//Add Association:
		//***********************************
		$('#Save_<% = Model.DefaultValue %>').live ('click', function() {
			var association = $('#dropDownChapters_<% = Model.DefaultValue%>').children("option:selected").text();
			var hiddenValue = $('#dropDownChapters_<% = Model.DefaultValue%>').children("option:selected").val();
						
			var isExists="false";
			
			if (association=="Select") {
						isExists = "true";	
					}
			
			if (association=="") {
						isExists = "true";	
					}
			//Check if such association is already in the list:
			$('#associationsList_<% = Model.DefaultValue %> li').each(function() {

				if ($(this).text().indexOf(association) >= 0) {
						isExists = "true";	
					}								
			});			
			
			if (isExists=="false") {
			   $('<li>' + association + listItem_<% = Model.DefaultValue %> + hiddenValue + '"/></li>').appendTo('#associationsList_<% = Model.DefaultValue %>');
				
			   $('#divNoAssociations_<% = Model.DefaultValue %>').empty();
			}

			$('#Cancel_<% = Model.DefaultValue %>').click();
		});

		//***********************************
		//Load dropDownChapters:
		//***********************************
	    $(document).off('click', '#addAssociation_<% = Model.DefaultValue %>').on('click', '#addAssociation_<% = Model.DefaultValue %>', function () {			
			//Load DropDown if needed:
			if ($('#dropDownChapters_<% = Model.DefaultValue %>').val() == undefined) {
				PopulateChaptersDropDown_<% = Model.DefaultValue %>("true");
			}

			$('#divAssociatedChapters_<% = Model.DefaultValue %>').show();
		    $('#divAddAssociationButtons_<% = Model.DefaultValue %>').hide();
		});
		
		//***********************************
		// PopulateChaptersDropDown
		//***********************************
	    function PopulateChaptersDropDown_<% = Model.DefaultValue %>(showAssociatedChapters) {
	    	$.ajax(
	    		{
	    			type: "GET",
	    			url: show_chapters_route,
	    			data: { data: "<% = Model.DefaultValue %>" },
	    			success: function(result) {
	    				if (result != null) {
	    					$('#divChaptersDropDown_<% = Model.DefaultValue %>').html(result);
	    					
							done_ChaptersDropDown_<% = Model.DefaultValue %> = "true";	
	    					
	    					if (showAssociatedChapters == "true") {
	    						$('#divAssociatedChapters_<% = Model.DefaultValue %>').show();
	    						$('#divAddAssociationButtons_<% = Model.DefaultValue %>').hide();
	    					}
	    					if (showAssociatedChapters == "false") {
	    						$('#divAssociatedChapters_<% = Model.DefaultValue %>').hide();
	    						$('#divAddAssociationButtons_<% = Model.DefaultValue %>').show();
	    						AssociateToAllChapters_<% = Model.DefaultValue %>();	    						
	    					}
	    				}
	    			}
	    		});
	    }

        //***********************************
		// Associate to All Chapters
		//***********************************
	    function AssociateToAllChapters_<% = Model.DefaultValue %>() {
	    	//1 Remove all chapters from #associationsList
			$('#associationsList_<% = Model.DefaultValue %> li').remove();

			//2. Add All chapters to #associationsList
			$('#dropDownChapters_<% = Model.DefaultValue %> option').each(function() {				
				if($(this).text()!="Select") {
					$('<li>' + $(this).text() + listItem_<% = Model.DefaultValue %> + $(this).val() + '"/></li>').appendTo('#associationsList_<% = Model.DefaultValue %>');				
				}
			});	
	    	
			$('#divNoAssociations_<% = Model.DefaultValue %>').empty();
	    	$('#divNoAssociations_<% = Model.DefaultValue %>').append('This resource is associated to all chapters');
	    }
	    	

	    
//})
</script>