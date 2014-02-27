<%@ WebHandler Language="C#" Class="EqGifHandler" %>

 using System;
 using System.Web;
using IPEQGif;
 
 public class EqGifHandler : IHttpHandler
 {

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public void ProcessRequest(HttpContext context)
    {
        string par;
        IPEqGifServerClass gif = new IPEqGifServerClass();
        
        par = context.Request.Params["gettext"];
        if (par != null && par != "")
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(gif.GetReduced(par));
            return;
        }

        par = context.Request.Params["exprtext"];
        if (par != null && par != "")
        {
            gif.ExprText = par;
        }
        else
        {
            par = context.Request.Params["eqtext"];
            if (par != null && par != "")
            {
                gif.EqText = par;
            }
        }
          
        par = context.Request.Params["top"];
        if (par != null && par != "")
        {
            gif.Top = System.Convert.ToInt32(par);
        }
   
        par = context.Request.Params["bottom"];
        if (par != null && par != "")
        {
            gif.Bottom = System.Convert.ToInt32(par);
        }
                   
        par = context.Request.Params["interpretfunctions"];
        if (par != null && par != "")
        {
            gif.InterpretFunctions = System.Convert.ToBoolean(par);
        }
   
        par = context.Request.Params["alignbyeq"];
        if (par != null && par != "")
        {
            gif.AlignByEq = System.Convert.ToBoolean(par);
        }

        par = context.Request.Params["donotreduce"];
        if (par != null && par != "")
        {
            gif.DoNotReduce = System.Convert.ToBoolean(par);
        }

        par = context.Request.Params["doborder"];
        if (par != null && par != "")
        {
            gif.DoBorder = System.Convert.ToBoolean(par);
        }
        
        par = context.Request.Params["path"];
        if (par != null && par != "")
        {
            gif.Path = par;
        }
   
        par = context.Request.Params["fontsize"];
        if (par != null && par != "")
        {
            gif.Fontsize = System.Convert.ToInt32(par);
        }

        // Phase 3 - B-8 get baseline information for the expression images
        par = context.Request.Params["exprtextinfo"];
        if (par != null && par != "")
        {
            gif.ExprText = par;
            context.Response.ContentType = "text/plain";
            context.Response.Write(gif.GetImageInfo());
            return;
            
        }
        else
        {
            par = context.Request.Params["eqtextinfo"];
            if (par != null && par != "")
            {
                gif.EqText = par;
                context.Response.ContentType = "text/plain";
                context.Response.Write(gif.GetImageInfo());
                return;
            }
        }
        
        string fname = System.IO.Path.GetTempFileName();
        gif.Path = System.IO.Path.GetTempPath();
        gif.Filename = System.IO.Path.GetFileName(fname); 
   
        gif.Save();

        context.Response.ContentType = "image/gif";
        context.Response.WriteFile(gif.Path+gif.Filename,true);
        
        System.IO.File.Delete(fname);
    }

}
