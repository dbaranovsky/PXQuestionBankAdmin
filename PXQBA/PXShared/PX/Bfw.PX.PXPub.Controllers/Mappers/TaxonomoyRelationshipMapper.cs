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
    public static class TaxonomoyRelationshipMapper
    {
        /// <summary>
        /// Converts to a Taxonomy Relationship from from a Biz Content Item Relationship.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static TaxonomyRelationship ToTaxonomyRelationship(this BizDC.TaxonomyRelationship biz, BizSC.IContentActions content)
        {
            var model = new TaxonomyRelationship()
            {
                ItemId = biz.ItemId,
                ItemTitle = biz.ItemTitle,
                RelatedItemId = biz.RelatedItemId,
                TaxonomyId = biz.TaxonomyId,
                TaxonomyTitle = biz.TaxonomyTitle,
                ScopeId = biz.ScopeId
            };

            return model;
        }
    }
}
