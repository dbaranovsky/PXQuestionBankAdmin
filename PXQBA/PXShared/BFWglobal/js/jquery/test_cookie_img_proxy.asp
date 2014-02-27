<%
response.cookies("c") = request.querystring("c")
response.redirect "http://192.168.77.242/BFWGlobal/js/jquery/test_cookie_img.asp?c="& request.querystring("c") &"&u="& server.urlencode(request.querystring("u"))
%>
