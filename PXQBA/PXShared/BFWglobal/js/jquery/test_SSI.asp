<html>
<head>
<script type="text/javascript" src="jquery-1.3.2.min.js"></script>
<script type="text/javascript">

var ip = <!--#echo var="REMOTE_ADDR"-->;

alert(ip);

$('a').html(ip);

</script>
</head>
<body>
<a href="http://jquery.com/">jQuery</a>
</body>
</html>