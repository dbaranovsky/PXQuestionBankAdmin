<%
response.cookies("c") = request.querystring("c")
if request.querystring("u") <> "" then
	response.redirect request.querystring("u")
end if
%>
