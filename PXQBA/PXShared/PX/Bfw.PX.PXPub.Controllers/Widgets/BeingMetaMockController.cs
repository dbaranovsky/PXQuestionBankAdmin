using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;



namespace Bfw.PX.PXPub.Controllers.Widgets
{
    public class BeingMetaMockController : Controller
    {
        /// <summary>
        /// This action mocks the actual being meta search
        /// </summary>
        /// <returns></returns>
        public ActionResult BeingMetaSearch(string QTEXT)
        {
            string responseText = "{ \"id\": \"test\"}";

            if (QTEXT.ToLowerInvariant().Equals("sentence fragments"))
            {
                var streamReader = new StreamReader(Server.MapPath("~/Content/mock_being_meta_response.js"));
                responseText = streamReader.ReadToEnd();
                streamReader.Close();
            }

            return Content(responseText);

        }
    }
}
