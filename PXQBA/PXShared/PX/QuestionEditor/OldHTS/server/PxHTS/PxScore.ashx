<%@ WebHandler Language="C#" Class="PxScore" %>

using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using BFW;
using HTS;


public class PxScore : IHttpHandler
{
    bool debugMode = false; //on/off saving requests/responses
    string mathJS = string.Empty; // path to maths.js file. We need use the javascript functions from this file to calculate values of variables
    string encryptKey = BFW.ItemServer.EncryptionKey; // EncryptionKey from web.config
    Guid problemXMLguid = Guid.NewGuid(); //for generate unique file name in debug mode


    public void ProcessRequest (HttpContext context) 
    {
        context.Response.ContentType = "text/plain";
        context.Response.Write(Score(context));
        context.Response.End();
    }

    public string Score(HttpContext context)
    {
        bool bDebug = false;
        bool err = false;
        System.IO.Stream body = context.Request.InputStream;
        string len = body.Length.ToString();
        System.Text.Encoding encoding = context.Request.ContentEncoding;
        System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
        string s = reader.ReadToEnd();
        mathJS = context.Server.MapPath("~\\" + "htsplayer\\maths.js");
        if (bDebug)
        { // save request for score
            string fname1 = context.Server.MapPath("~\\" + "htsplayer\\session") + "\\" + "score-" + problemXMLguid + ".xml";
            using (StreamWriter f = File.CreateText(fname1))
            {
                f.Write(s);
                f.Close();
            }
        }

        string partId = "";
        double possible = 0;
        double computed = 0;
        XDocument request = null;
        
        
        try
        {
            request = XDocument.Load(new System.IO.StringReader(s));

            partId = request.Root.Element("submission").Attribute("partid").Value;
            possible = XmlConvert.ToDouble(request.XPathSelectElement("info/response").Attribute("pointspossible").Value);

        }
        catch 
        { 
            partId = "Bad format of request";
            err = true;
        }
        
        if (!err)
        {
            try
            {
                computed = GetScore(request.Root, possible);
            }
            catch
            {
                partId = "Error while processing request";
                err = true;
            }
        }

        XDocument custom =
            new XDocument(
                new XElement("custom",
                    new XElement("response",
                        new XAttribute("type", "submission"),
                        new XAttribute("foreignid", partId),
                        new XAttribute("pointspossible", XmlConvert.ToString(possible)),
                        new XAttribute("pointscomputed", XmlConvert.ToString(computed))
                    )
                )
            );
        s = custom.ToString();

        if (bDebug)
        { //save score XML response
            string fname1 = context.Server.MapPath("~\\" + "htsplayer\\session") + "\\" + "scoreRES-" + problemXMLguid + ".xml";
            using (StreamWriter f = File.CreateText(fname1))
            {
                f.Write(s);
                f.Close();
            }
        }
        
        return s;
    }

    private double GetScore(XElement info, double possible)
    {
        double rawScore = .0;
        try
        {
            string problemXML = info.XPathSelectElement("question/interaction/data").Value;
            string answer = info.Element("submission").Element("answer").Value;

            CHTSProblem hts = new CHTSProblem(mathJS, "");
            rawScore = hts.doScore(problemXML, answer, encryptKey, getSignKey(info));
            rawScore /= 100;

        }
        catch (Exception ex)
        {
        }
        // Always scale the points to possible
        return rawScore * possible;
    }
    private string getSignKey(XElement info)
    {
        string signKey = "";
        try
        {
            XElement node = info.XPathSelectElement("submission/attemptquestion/customprivatedata");
            if (node != null)
            {
                signKey = node.Value;
            }
        }
        catch { }
        if (signKey == "")
        {
            signKey = "agilixSign";
        }
        return signKey;

    }

    public bool IsReusable 
    {
        get {
            return false;
        }
    }

}