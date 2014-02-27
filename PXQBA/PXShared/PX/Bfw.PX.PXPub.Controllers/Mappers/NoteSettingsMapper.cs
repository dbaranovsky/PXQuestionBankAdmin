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
    public static class NoteSettingsMapper
    {

        /// <summary>
        /// Converts to a Note Settings from a list of Biz Share Note Result.
        /// </summary>
        /// <param name="shares">The shares.</param>
        /// <returns></returns>
        public static NoteSettings ToNoteSettings(this IEnumerable<BizDC.ShareNoteResult> shares)
        {
            var notes = new List<DataShare>();
            var highlights = new List<DataShare>();

            var model = new NoteSettings()
            {
                Notes = notes,
                Highlights = highlights
            };

            foreach (var share in shares)
            {
                notes.Add(new DataShare()
                {
                    UserId = share.StudentId,
                    UserName = string.Format("{0} {1}", share.FirstNameSharer, share.LastNameSharer),
                    Active = share.NotesEnabled,
                    DataType = ShareType.Note,
                    ItemCount = share.ItemCount ?? 0
                });

                highlights.Add(new DataShare()
                {
                    UserId = share.StudentId,
                    UserName = string.Format("{0} {1}", share.FirstNameSharer, share.LastNameSharer),
                    Active = share.HighlightsEnabled,
                    DataType = ShareType.Highlight,
                    ItemCount = share.ItemCount ?? 0
                });
            }

            return model;
        }
    }
}
