<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<%--   <form  id="bltiLaunchForm" name="bltiLaunchForm" action="http://dev-lmslink.bfwpub.com/basiclti/php-simple/tool.php" method="POST">
<input type="hidden" value="1.0" name="oauth_version">
<input type="hidden" value="6862033" name="oauth_nonce">
<input type="hidden" value="1348683530" name="oauth_timestamp">
<input type="hidden" value="12345" name="oauth_consumer_key">
<input type="hidden" value="120988f929-274612" name="resource_link_id">
<input type="hidden" value="Weekly Blog" name="resource_link_title">
<input type="hidden" value="A weekly blog." name="resource_link_description">
<input type="hidden" value="292832126" name="user_id">
<input type="hidden" value="Instructor" name="roles">
<input type="hidden" value="Jane Q. Public" name="lis_person_name_full">
<input type="hidden" value="user@school.edu" name="lis_person_contact_email_primary">
<input type="hidden" value="school.edu:user" name="lis_person_sourcedid">
<input type="hidden" value="456434513" name="context_id">
<input type="hidden" value="Design of Personal Environments" name="context_title">
<input type="hidden" value="SI182" name="context_label">
<input type="hidden" value="lmsng.school.edu" name="tool_consumer_instance_guid">
<input type="hidden" value="University of School (LMSng)" name="tool_consumer_instance_description">
<input type="hidden" value="about:blank" name="oauth_callback">
<input type="hidden" value="LTI-1p0" name="lti_version">
<input type="hidden" value="basic-lti-launch-request" name="lti_message_type">
<input type="submit" value="Press to Launch" name="ext_submit">
<input type="hidden" value="HMAC-SHA1" name="oauth_signature_method">
<input type="hidden" value="Y0vIf/KMg8Se0K9/4VnfPZaLfks=" name="oauth_signature">
<input type="hidden" name="ext_submit" value="Press to Launch">
</form>--%>
<%--<form  id="bltiLaunchForm" name="bltiLaunchForm" action="http://dev-ebooks.bfwpub.com/ims-blti/blti_logon.php?uid=6668608" method="POST">
<input type="hidden" name="context_id" value="A_16"><br/>
<input type="hidden" name="context_label" value="lms middleware"><br/>
<input type="hidden" name="context_title" value="lms middleware"><br/>
<input type="hidden" name="launch_presentation_return_url" value="http://dev-lmslink.bfwpub.com"><br/>
<input type="hidden" name="lti_message_type" value="basic-lti-launch-request"><br/>
<input type="hidden" name="lti_version" value="LTI-1p0"><br/>
<input type="hidden" name="oauth_callback" value="about:blank"><br/>
<input type="hidden" name="oauth_consumer_key" value="key"><br/>
<input type="hidden" name="oauth_signature_method" value="HMAC-SHA1"><br/>
<input type="hidden" name="oauth_version" value="1.0"><br/>
<input type="hidden" name="resource_link_id" value="None"><br/>
<input type="hidden" name="tool_consumer_instance_contact_email" value="techsupport@macmillan.com"><br/>
<input type="hidden" name="tool_consumer_instance_guid" value="ebooks"><br/>
<input type="hidden" name="tool_consumer_instance_name" value="ebooks"><br/>
<input type="hidden" name="bookId" value="henretta7e"><br/>
<input type="hidden" name="baseURL" value="dev-ebooks.bfwpub.com/henretta7e"><br/>
<input type="hidden" name="uid" value="6668608"><br/>
<input type="hidden" name="oauth_nonce" value="5139002"><br/>
<input type="hidden" name="oauth_timestamp" value="1349278518"><br/>
<input type="hidden" name="oauth_signature" value="OvmTaEt1FeR4Q+YhYgGoLRZSav8="><br/>
<input type="submit" value="Launch" onClick="blti.launch(); return false;">
</form>--%>

<form  id="bltiLaunchForm" name="bltiLaunchForm" action="http://localhost:50713/api/users/checkandcreateuserandenrollment?rauserid=8889&courseid=25027&domainid=8&firstname=mike&lastname=dikan&email=mdikan@nps.com&isinstructor=false&startdate=11/01/2012" method="POST">
    <input type="hidden" name="context_id" value="A_16" /><br/>
    <input type="hidden" name="context_label" value="lms middleware"/><br/>
    <input type="hidden" name="context_title" value="lms middleware"/><br/>
    <input type="hidden" name="launch_presentation_return_url" value="http://dev-lmslink.bfwpub.com"/><br/>
    <input type="hidden" name="lti_message_type" value="basic-lti-launch-request"/><br/>
    <input type="hidden" name="lti_version" value="LTI-1p0"/><br/>
    <input type="hidden" name="oauth_callback" value="about:blank"/><br/>
    <input type="hidden" name="oauth_consumer_key" value="middlewarekey2"><br/>
    <input type="hidden" name="oauth_signature_method" value="HMAC-SHA1"/><br/>
    <input type="hidden" name="oauth_version" value="1.0"/><br/>
    <input type="hidden" name="resource_link_id" value="None"/><br/>
    <input type="hidden" name="tool_consumer_instance_contact_email" value="techsupport@macmillan.com"/><br/>
    <input type="hidden" name="tool_consumer_instance_guid" value="ebooks"/><br/>
    <input type="hidden" name="tool_consumer_instance_name" value="ebooks"/><br/>
    <input type="hidden" name="bookId" value="henretta7e"/><br/>
    <input type="hidden" name="baseURL" value="dev-ebooks.bfwpub.com/henretta7e"/><br/>
    <input type="hidden" name="uid" value="6668608"/><br/>
    <input type="hidden" name="oauth_nonce" value="6329531"/><br/>
    <input type="hidden" name="oauth_timestamp" value="1349814010"/><br/>
    <input type="hidden" name="oauth_signature" value="BD8r8ncsr74EGD6JwyY284q8tGQ="/><br/>
    <input type="submit" value="Launch" onclick="blti.launch(); return false;"/>
</form>

</asp:Content>
