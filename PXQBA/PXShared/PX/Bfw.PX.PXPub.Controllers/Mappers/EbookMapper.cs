using System;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers {
    public static class EbookMapper {
        /// <summary>
        /// Convert to an Ebook.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        public static void ToEbook( this Ebook model, BizDC.Ebook biz )
        {
            model.Id = biz.Id;
            model.Title = biz.Title;
            model.ParentId = biz.ParentId;
            model.Sequence = biz.Sequence;
            model.Authors = biz.Authors;
            model.Subtitle = biz.Subtitle;
            model.CoverImage = biz.CoverImage;
            model.RootId = biz.RootId;
            model.CatagoryId = biz.CatagoryId;
        }

        /// <summary>
        /// Convert to an Ebook DataContract to Ebook Model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static Ebook ToEbook( this BizDC.ContentItem biz ) {
            var hiddenFromStudents = false;
            if(biz.Properties.ContainsKey("bfw_hiddenfromstudents"))
            {
                hiddenFromStudents = biz.Properties["bfw_hiddenfromstudents"] == null ? false : Convert.ToBoolean(biz.Properties["bfw_hiddenfromstudents"].Value);
            }
            var ebook = new Ebook()
            {
                Id = biz.Id,
                Title = biz.Title,
                ParentId = biz.ParentId,
                Sequence = biz.Sequence,
                Authors =  biz.Properties [ "bfw_authors"] == null ? "" : biz.Properties [ "bfw_authors"].Value.ToString(),
                Subtitle = biz.Properties["bfw_subtitle"] == null ? "" : biz.Properties["bfw_subtitle"].Value.ToString(),
                CoverImage = biz.Properties["bfw_coverimage"] == null ? "" : biz.Properties["bfw_coverimage"].Value.ToString(),
                RootId = biz.Properties["bfw_rootid"] == null ? "" : biz.Properties["bfw_rootid"].Value.ToString(),
                CatagoryId = biz.Properties["bfw_catagoryid"] == null ? "" : biz.Properties["bfw_catagoryid"].Value.ToString(),
                HiddenFromStudents = hiddenFromStudents
            };

            ebook.Sequence = biz.Sequence;
            ebook.Description = biz.Description;
            return ebook;
        }

        /// <summary>
        /// Converts to a Ebook Model to Ebook DataContract.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.Ebook ToEbook( this Ebook model ) {
            var biz = new BizDC.Ebook
            {
                Id = model.Id,
                Title = model.Title,
                ParentId = model.ParentId,
                Sequence = model.Sequence,
                Authors = model.Authors,
                Subtitle = model.Subtitle,
                CoverImage = model.CoverImage,
                RootId = model.RootId,
                CatagoryId = model.CatagoryId
            };

            return biz;
        }
    }
}
