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
    public static class GroupSetMapper
    {

        /// <summary>
        /// Maps a Group Set from the business layer to a Group Set model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static GroupSet ToGroupSet(this BizDC.GroupSet biz)
        {
            var model = new GroupSet()
            {
                Name = biz.Name,
                Id = biz.Id
            };

            return model;
        }

        /// <summary>
        /// Convert to a Group set from a Biz GroupSet.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static IList<GroupSet> ToGroupSets(this IDictionary<BizDC.GroupSet, IList<BizDC.Group>> biz)
        {
            var model = new List<GroupSet>(biz.Keys.Count);
            foreach (var bizGroupSet in biz.Keys)
            {
                var modelGroupSet = bizGroupSet.ToGroupSet();
                modelGroupSet.Groups = biz[bizGroupSet].Map(g => g.ToGroup()).ToList();
                model.Add(modelGroupSet);
            }

            return model;
        }
    }
}
