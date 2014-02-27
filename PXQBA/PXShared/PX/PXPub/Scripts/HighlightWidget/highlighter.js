/*
* HTML Parser By John Resig (ejohn.org)
* Original code by Erik Arvidsson, Mozilla Public License
* http://erik.eae.net/simplehtmlparser/simplehtmlparser.js
*
* // Use like so:
* HTMLParser(htmlString, {
*     start: function(tag, attrs, unary) {},
*     end: function(tag) {},
*     chars: function(text) {},
*     comment: function(text) {}
* });
*
* // or to get an XML string:
* HTMLtoXML(htmlString);
*
* // or to get an XML DOM Document
* HTMLtoDOM(htmlString);
*
* // or to inject into an existing document/DOM node
* HTMLtoDOM(htmlString, document);
* HTMLtoDOM(htmlString, document.body);
*
*/

(function () {
    var startTag = /^<(\w+)((?:\s*[\w:-]+(?:\s*=\s*(?:(?:"[^"]*")|(?:'[^']*')|[^>\s]+))?)*)\s*(\/?)>/,
      endTag = /^<\/(\w+)[^>]*>/,
      attr = /([\w:-]+)(?:\s*=\s*(?:(?:"((?:\\.|[^"])*)")|(?:'((?:\\.|[^'])*)')|([^>\s]+)))?/g,
      doctype = /^<!DOCTYPE [^>]+>/i;

    // Empty Elements - HTML 4.01
    var empty = makeMap("area,base,basefont,br,col,frame,hr,img,input,isindex,link,meta,param,embed");

    // Block Elements - HTML 4.01
    var block = makeMap("address,applet,blockquote,button,center,dd,del,dir,div,dl,dt,fieldset,form,frameset,hr,iframe,ins,isindex,li,map,menu,noframes,noscript,object,ol,p,pre,script,table,tbody,td,tfoot,th,thead,tr,ul");

    // Inline Elements - HTML 4.01
    var inline = makeMap("a,abbr,acronym,applet,b,basefont,bdo,big,br,button,cite,code,del,dfn,em,font,i,iframe,img,input,ins,kbd,label,map,object,q,s,samp,script,select,small,span,strike,strong,sub,sup,textarea,tt,u,var");

    // Elements that you can, intentionally, leave open
    // (and which close themselves)
    var closeSelf = makeMap("colgroup,dd,dt,li,options,p,td,tfoot,th,thead,tr");

    // Attributes that have their values filled in disabled="disabled"
    var fillAttrs = makeMap("checked,compact,declare,defer,disabled,ismap,multiple,nohref,noresize,noshade,nowrap,readonly,selected");

    // Special Elements (can contain anything)
    var special = makeMap("script,style");

    var reCache = {}, stackedTag, re;

    var HTMLParser = this.HTMLParser = function (html, handler) {
        var index, chars, match, stack = [], last = html;
        stack.last = function () {
            return this[this.length - 1];
        };

        while (html) {
            chars = true;

            // Make sure we're not in a script or style element
            if (!stack.last() || !special[stack.last()]) {

                // Comment
                if (html.indexOf("<!--") === 0) {
                    index = html.indexOf("-->");

                    if (index >= 0) {
                        if (handler.comment)
                            handler.comment(html.substring(4, index));
                        html = html.substring(index + 3);
                        chars = false;
                    }
                }
                else if (match = doctype.exec(html)) {
                    if (handler.doctype)
                        handler.doctype(match[0]);
                    html = html.substring(match[0].length);
                    chars = false;

                    // end tag
                } else if (html.indexOf("</") === 0) {
                    match = html.match(endTag);

                    if (match) {
                        html = html.substring(match[0].length);
                        match[0].replace(endTag, parseEndTag);
                        chars = false;
                    }

                    // start tag
                } else if (html.indexOf("<") === 0) {
                    match = html.match(startTag);

                    if (match) {
                        html = html.substring(match[0].length);
                        match[0].replace(startTag, parseStartTag);
                        chars = false;
                    }
                }

                if (chars) {
                    index = html.indexOf("<");

                    var text = index < 0 ? html : html.substring(0, index);
                    html = index < 0 ? "" : html.substring(index);

                    if (handler.chars)
                        handler.chars(text);
                }

            } else {
                //                html = html.replace(new RegExp("(.*)<\/" + stack.last() + "[^>]*>"), function(all, text) {
                //                    text = text.replace(/<!--(.*?)-->/g, "$1")
                //						.replace(/<!\[CDATA\[(.*?)]]>/g, "$1");

                //                    if (handler.chars)
                //                        handler.chars(text);

                //                    return "";
                //                });
                var end = "</" + stack.last() + ">";
                index = html.indexOf(end);

                if (index < 0) {
                    end = "/>";
                    index = html.indexOf(end);
                }

                if (index < 0) {
                    end = ">";
                    index = html.indexOf(end);
                }

                var text = index < 0 ? html : html.substring(0, index);
                html = index < 0 ? "" : html.substring(index + end.length);

                if (handler.chars)
                    handler.chars(text);

                parseEndTag("", stack.last());
            }

            if (html == last)
                throw "Parse Error: " + html;
            last = html;
        }

        // Clean up any remaining tags
        parseEndTag();

        function parseStartTag(tag, tagName, rest, unary) {
            if (block[tagName]) {
                while (stack.last() && inline[stack.last()]) {
                    parseEndTag("", stack.last());
                }
            }

            if (closeSelf[tagName] && stack.last() == tagName) {
                parseEndTag("", tagName);
            }

            unary = empty[tagName] || !!unary;

            if (!unary)
                stack.push(tagName);

            if (handler.start) {
                var attrs = [];

                rest.replace(attr, function (match, name) {
                    var value = arguments[2] ? arguments[2] :
            arguments[3] ? arguments[3] :
            arguments[4] ? arguments[4] :
            fillAttrs[name] ? name : "";
                    attrs.push({
                        name: name,
                        value: value,
                        escaped: value.replace(/(^|[^\\])"/g, '$1\\\"') //"
                    });
                });

                if (handler.start)
                    handler.start(tagName, attrs, unary);
            }
        }

        function parseEndTag(tag, tagName) {
            // If no tag name is provided, clean shop
            if (!tagName)
                var pos = 0;

            // Find the closest opened tag of the same type
            else
                for (var pos = stack.length - 1; pos >= 0; pos--)
                    if (stack[pos] == tagName)
                        break;

            if (pos >= 0) {
                // Close all the open elements, up the stack
                for (var i = stack.length - 1; i >= pos; i--)
                    if (handler.end)
                        handler.end(stack[i]);

                // Remove the open elements from the stack
                stack.length = pos;
            }
        }
    };

    var HTMLtoXML = function (html) {
        var results = "";

        HTMLParser(html, {
            start: function (tag, attrs, unary) {
                results += "<" + tag;

                for (var i = 0; i < attrs.length; i++)
                    results += " " + attrs[i].name + '="' + attrs[i].escaped + '"';

                results += (unary ? "/" : "") + ">";
            },
            end: function (tag) {
                results += "</" + tag + ">";
            },
            chars: function (text) {
                results += text;
            },
            comment: function (text) {
                results += "<!--" + text + "-->";
            }
        });

        return results;
    };

    var HTMLtoDOM = function (html, doc) {
        // There can be only one of these elements
        var one = makeMap("html,head,body,title");

        // Enforce a structure for the document
        var structure = {
            link: "head",
            base: "head"
        };

        if (!doc) {
            if (typeof DOMDocument != "undefined")
                doc = new DOMDocument();
            else if (typeof document != "undefined" && document.implementation && document.implementation.createDocument)
                doc = document.implementation.createDocument("", "", null);
            else if (typeof ActiveX != "undefined")
                doc = new ActiveXObject("Msxml.DOMDocument");

        } else
            doc = doc.ownerDocument ||
        doc.getOwnerDocument && doc.getOwnerDocument() ||
        doc;

        var elems = [],
      documentElement = doc.documentElement ||
        doc.getDocumentElement && doc.getDocumentElement();

        // If we're dealing with an empty document then we
        // need to pre-populate it with the HTML document structure
        if (!documentElement && doc.createElement) (function () {
            var html = doc.createElement("html");
            var head = doc.createElement("head");
            head.appendChild(doc.createElement("title"));
            html.appendChild(head);
            html.appendChild(doc.createElement("body"));
            doc.appendChild(html);
        })();

        // Find all the unique elements
        if (doc.getElementsByTagName)
            for (var i in one)
                one[i] = doc.getElementsByTagName(i)[0];

        // If we're working with a document, inject contents into
        // the body element
        var curParentNode = one.body;

        HTMLParser(html, {
            start: function (tagName, attrs, unary) {
                // If it's a pre-built element, then we can ignore
                // its construction
                if (one[tagName]) {
                    curParentNode = one[tagName];
                    return;
                }

                var elem = doc.createElement(tagName);

                for (var attr in attrs)
                    elem.setAttribute(attrs[attr].name, attrs[attr].value);

                if (structure[tagName] && typeof one[structure[tagName]] != "boolean")
                    one[structure[tagName]].appendChild(elem);

                else if (curParentNode && curParentNode.appendChild)
                    curParentNode.appendChild(elem);

                if (!unary) {
                    elems.push(elem);
                    curParentNode = elem;
                }
            },
            end: function (tag) {
                elems.length -= 1;

                // Init the new parentNode
                curParentNode = elems[elems.length - 1];
            },
            chars: function (text) {
                curParentNode.appendChild(doc.createTextNode(text));
            },
            comment: function (text) {
                // create comment node
            }
        });

        return doc;
    };

    function makeMap(str) {
        var obj = {}, items = str.split(",");
        for (var i = 0; i < items.length; i++) {
            obj[items[i]] = true;
            obj[items[i].toUpperCase()] = true;
        }
        return obj;
    }
})();



(function($) {

    //This function takes in the range object and wrap all the text within the range with openTag/closeTag
    function wrapHighlightTag(range, openTag, closeTag) {
        //inner recursive function to traverse the node and wrap the tags.
        var wrapNode = function (node) {
            var markup = "";
            if (node.hasChildNodes()) {
                var child = node.firstChild;
                while (child) {
                    if (child.nodeType === 1) {  //If element node
                        //If self-closing element, skip it.
                        if ($(child).html().length === 0) {
                            if (child.outerHTML.match(/\/>$/)) { //tag is self closing (ends with '/>') - BRs, etc
                                markup += reconstructTag(child, true);
                            } else {
                                markup += reconstructTag(child, false); //tag is empty
                                markup += "</" + child.nodeName + ">";
                            }

                        }
                        else {  //Otherwise, reconstruct the tag with attributes and recursively process the node.
                            markup += reconstructTag(child);
                            if (child.tagName.toLowerCase() != "textarea") //do not wrap content inside textareas, instead use value
                            {
                                markup += wrapNode(child);
                            }
                            markup += "</" + child.nodeName + ">";
                        }
                    }                  
                    else if (child.nodeType === 3) { //Process text node. 
                        if (child === range.startContainer && child === range.endContainer) {  //Range falls in same text node.
                            markup += $(child).text().substr(0, range.startOffset) + openTag + $(child).text().substr(range.startOffset, range.endOffset - range.startOffset) + closeTag + $(child).text().substr(range.endOffset);
                        } else if (child === range.startContainer) { 
                            markup += $(child).text().substr(0, range.startOffset) + openTag + $(child).text().substr(range.startOffset) + closeTag;
                        } else if (child === range.endContainer) {
                            markup += openTag + $(child).text().substr(0, range.endOffset) + closeTag + $(child).text().substr(range.endOffset);
                        } else {
                            if (range.isPointInRange(child, 1))
                                markup += openTag + $(child).text() + closeTag;
                            else
                                markup += $(child).text();
                        }
                    }

                    child = child.nextSibling;
                }
            }
            return markup;
        }

        //inner utility function to reconstruct the tag with attributes.
        var reconstructTag = function (node, isSelfClosing) {
            var attributes = node.attributes;
            var tag = "<" + node.nodeName;
            if (node.value && node.tagName.toLowerCase() != "textarea") { //textareas dont support a "value" attribute
                tag += ' value="' + node.value + '"';
                if (attributes["value"])
                    attributes.removeNamedItem("value");
            }
            
            if (node.checked) {
                tag += ' checked="' + node.checked + '"';
                if (attributes["checked"])
                    attributes.removeNamedItem("checked");
            }
            if (node.selectedIndex) {
                tag += ' selectedIndex="' + node.selectedIndex + '"';
                if (attributes["selectedIndex"])
                    attributes.removeNamedItem("selectedIndex");
            }
            for(var i=0; i<attributes.length; i++)
            {
                var nodeValue = attributes[i].nodeValue.replace(/"/g,"'"); 
                tag += ' ' + attributes[i].nodeName + '="' + nodeValue + '"';
            }
           
            if (isSelfClosing) {
                tag += "/>";
            } else {
                tag += ">";
            }
            
            if (node.tagName.toLowerCase() == "textarea") { //textareas don't have a value attribute, instead we need to set the inner html
                tag += node.value;
            }
            return tag;
        }

        var parent = range.commonAncestorContainer;
        //Need to process from a element node because text node's text cannot be replaced by innerHTML. this happens when the range falls in same text node.
        while (parent.nodeType === 3)
            parent = parent.parentNode;

        //https://macmillanhighered.atlassian.net/browse/PX-5196
        //TODO: Read current form values and set them explicity so that they are preserved 
        parent.innerHTML = wrapNode(parent);
       
    }

    //This function adds an entire list of highlights.
    //content - array of objects like { rangeDetail, searchText, id, css }
    //          rangeDetail - start, startOffset, end, endOffset
    //          searchText - text to highlight
    //          id - id for the highlight. will be added as a class of the form cid_{id}
    //          css - map conforming to the jQuery CSS map. Used to style the comments
    //          className - class name that will be appended to the standard classname of highlight
    //eachComplete - called after each content item is processed function(highlights)
    //complete - called after all content items have been processed function()
    function _highlightList(content, targetFrame, eachComplete, complete) {
        $.each(content, function(i, item) {
            //If any of the following is null, it's for the old highlighting records, ignore it.
            if(item.rangeDetail.start == null || item.rangeDetail.startOffset == null || item.rangeDetail.end == null || item.rangeDetail.endOffset == null)
                return;

            var style = $("<span>").css(item.css).attr("style");
            var id = "highlight-" + item.id;
            var cls = "highlight";
            if (item.className) {
                cls = cls + " " + item.className;
            }

            var close = '</span>';
            var open;

            //If status == 3, it means the highlight was deleted alreay, so hide it by not including the highlighing attributes.
            if(item.rangeDetail.status === 3)
                open = "<span class='removedHL'>";
            else
                open = '<span id="' + id + '" class="' + cls + '" style="' + style + '">';
            
            var targetDocument = targetFrame.contentWindow.document;
            var targetContext = targetFrame.contentWindow.document.body;

            //recreate the range object based on rangeDetail retrived from database
            var rangeDetail = item.rangeDetail;
            var startNode = targetDocument.evaluate("/" + rangeDetail.start, targetContext, null, 9, null).singleNodeValue; //9=XPathResult.FIRST_ORDERED_NODE_TYPE
            var endNode = targetDocument.evaluate("/" + rangeDetail.end, targetContext, null, 9, null).singleNodeValue; //9=XPathResult.FIRST_ORDERED_NODE_TYPE
            
            if(startNode == null || endNode == null)
                return;

            // if the content changes and the range is no longer available it should just skip that comment it can not find
            if (startNode.length < rangeDetail.startOffset || 
                endNode.length   < rangeDetail.endOffset)
                return;

            //Updated to use rangy create range so that it works with IE8
            var range = rangy.createRange(targetDocument);
            range.setStart(startNode, rangeDetail.startOffset);
            range.setEnd(endNode, rangeDetail.endOffset);

            //Surround the range text with highlighting open/close tags.
            wrapHighlightTag(range, open, close)

            if (eachComplete != null && typeof (eachComplete) === "function") {
                eachComplete($(cls));
            }
        });

        if (complete != null && typeof (complete) === "function") {
            complete();
        }

        return;
    };

    window.Highlighter = {
        Highlight: _highlightList
    };

})(jQuery);