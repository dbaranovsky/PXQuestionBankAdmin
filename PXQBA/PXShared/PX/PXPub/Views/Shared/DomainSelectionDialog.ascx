<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.DomainSelection>"%>
<div id="content" style="height: 300px;">
    
    <span>Your account is associated with more than one university.</span></br>
    <span>Select a university to associate with this new course.</span>

    <% using (Html.BeginForm(Model.CallbackAction, Model.CallbackController, FormMethod.Post, new { id = "frmSelectDomain" }))
       { %>

        <%=Html.DropDownListFor(m => Model.SelectedDomainId, new SelectList(Model.Domains, "Id", "Name")) %>
       
    
    <% } %>
           <br /><br /> <input type="submit" name="behavior" value="Submit" id="select-domain" class="confirm-buttons"  />
           <button id="unblock-action" class="divPopupClose">Cancel</button> 

</div>

<script type="text/javascript" language="javascript">
    (function ($) {
        PxPage.OnReady(function () {
            PxPage.SetFneTitle("Choose your university");
            $('#fne-window').addClass('choose-university');
            $('domain-selection-dialog').dialog({
                autoOpen: false,
                title: "Choose your university",
                modal: true,
                height: 300,
                buttons: {
                    "Submit": function () { $(this).dialog("close"); }

                }
            });



            $('#unblock-action').click(function () {
                PxPage.UnBlock();
                //$('domain-selection-dialog').dialog("close");
            });

            $('#select-domain').click(function () {
                $('#frmSelectDomain').submit();

            });

            var cssObj = {'top':'200px', 'position':'relative' ,'max-height': '260px',
                'width': '500px'
            };



           // $('#content').css(cssObj);
            $('#fne-content').css({ 'height': '275px' });
            $('#fne-window').css(cssObj);
            //$('#SelectedDomainId').css({ 'width': '90%' });
            $('#frmSelectDomain').css({ 'width': '90%' });
            //launch the dialog on load
            // $('domain-selection-dialog').dialog("open");

        });
    } (jQuery))

</script>