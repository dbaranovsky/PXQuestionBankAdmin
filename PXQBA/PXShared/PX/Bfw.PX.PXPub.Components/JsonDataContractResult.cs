using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Components
{
    public class JsonDataContractResult : JsonResult
    {
        public JsonDataContractResult(object data)
        {
            Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var serializer = new DataContractJsonSerializer(this.Data.GetType());
            String output = String.Empty;
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this.Data);
                output = Encoding.UTF8.GetString(ms.ToArray());
            }
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(output);
        }
    }
}
