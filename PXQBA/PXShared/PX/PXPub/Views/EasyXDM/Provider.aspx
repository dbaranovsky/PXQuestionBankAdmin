<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<EasyXDM>" %>

<html>
<head>
    <script type="text/javascript" language="javascript" src="<%= Url.ContentCache("~/Scripts/jquery/jquery-1.10.2.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%= Url.ContentCache("~/Scripts/easyXDM/easyXDM.js") %>"></script>
    <script type="text/javascript" language="javascript">
        easyXDM.DomHelper.requiresJSON("<%= Url.ContentCache("~/Scripts/easyXDM/json2.js") %>");
    </script>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            var rpc = new easyXDM.Rpc({}, {
                remote: {
                    ResponseCallBack: {},
                    FinishedLoading: {}
                },
                local: {
                    getDocument: function (requestId) {
                        var doc = GetFrameDocument();
                        if (doc != null) {
                            var response = doc.documentElement;

                            Respond(requestId, response.innerHTML, doc);
                        } else {
                            Respond(requestId, "Could not access DOM", document);
                        }
                    },
                    getElementById: function (requestId, id) {
                        var doc = GetFrameDocument();
                        if (doc != null) {
                            var response = doc.getElementById(id).innerHTML; /* ie compatible */
                            Respond(requestId, response, doc);
                        } else {
                            Respond(requestId, "Could not access DOM", document);
                        }
                    },
                    getElementByName: function (requestId, name) { // ie will not like this
                        var doc = GetFrameDocument();
                        if (doc != null) {
                            var response = doc.getElementsByName(name);
                            for (index = 0; index < response.length; index++)
                                Respond(requestId, response[index].innerHTML, doc);
                        } else {
                            Respond(requestId, "Could not access DOM", document);
                        }
                    },
                    getElementByTagName: function (requestId, tagName) { // ie will not like this
                        var doc = GetFrameDocument();
                        if (doc != null) {
                            var response = doc.getElementsByTagName(tagName);
                            for (index = 0; index < response.length; index++)
                                Respond(requestId, response[index].innerHTML, doc);
                        } else {
                            Respond(requestId, "Could not access DOM", document);
                        }
                    }
                }
            });

            $("#BFW_EasyXDM_Container").load(function () {
                rpc.FinishedLoading();
            });

            function Respond(requestId, response, doc) {
                rpc.ResponseCallBack(requestId, doc.domain, doc.location.href, response);
            }

            function GetFrameDocument() {
                var frame = document.getElementsByTagName('iframe');
                if (frame.length != 0) {
                    var doc = frame[0].contentDocument;

                    if ((doc == null || typeof (doc) == 'undefined') && frame[0].contentWindow.document)
                        doc = frame[0].contentWindow.document;
                    else if ((doc == null || typeof (doc) == 'undefined') && frame[0].document)
                        doc = frame[0].document;
                }
                return doc;
            }
        });

    </script>
</head>
<body>
    <iframe id="BFW_EasyXDM_Container" name="BFW_EasyXDM_Container" src="about:blank">
    </iframe>
</body>
</html>
