using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class SearchResultDocMapper
    {
        /// <summary>
        /// Converts to a Result Doc from a Biz Result Doc.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static SearchResultDoc ToSearchResultDoc(this BizDC.SearchResultDoc biz)
        {
            var model = new SearchResultDoc();

            model.itemid = biz.itemid.Replace("Shortcut__1__","");
            model.dlap_class = biz.dlap_class;
            model.dlap_hiddenfromstudent = biz.dlap_hiddenfromstudent;
            model.dlap_id = biz.dlap_id;
            model.dlap_itemtype = biz.dlap_itemtype;
            model.dlap_contenttype = biz.dlap_contenttype;
            model.dlap_subtitle = biz.dlap_subtitle;
            model.dlap_title = biz.dlap_title;
            model.doc_class = biz.doc_class;
            model.entityid = biz.entityid;
            model.score = biz.score;
            model.Url = biz.url;
            model.dlap_html_text = biz.dlap_html_text;
            model.dlap_text = biz.dlap_text;
            model.Metadata = biz.Metadata;

            return model;
        }

    }
}