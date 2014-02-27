<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Web.UI;
public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        BLTI.BLTI bObj = new BLTI.BLTI { BltiHttpRequest = context.Request };
        string strSignatureBase = "";
        string strValidationMessage = "";
        string strSignature = "";
        bObj.ValidateRequestUsingBltiHttpRequest("secret", out strValidationMessage, out strSignatureBase, out strSignature,400);
        context.Response.Write("Signature:" + strSignature + "<br/>     Base Signature: "
            + strSignatureBase + "<br/> Validation Message: " + strValidationMessage);
       
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}