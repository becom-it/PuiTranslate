using PuiTranslate.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;

namespace PuiTranslate.Common.Models
{
    public class EditViewModel
    {
        public int GrpId { get; set; }

        [Required(ErrorMessage = "Der deutsche Lang-Text ist erforderlich!")]
        public string DeLong { get; set; }

        [MaxLength(20, ErrorMessage = "Der Mittel-Text darf nicht länger als 20 Zeichen sein!")]
        public string DeMiddle { get; set; }

        [MaxLength(10, ErrorMessage = "Der Kurz-Text darf nicht länger als 10 Zeichen sein!")]
        public string DeShort { get; set; }


        [Required(ErrorMessage = "Der englische Lang-Text ist erforderlich!")]
        public string EnLong { get; set; }

        [MaxLength(20, ErrorMessage = "Der Mittel-Text darf nicht länger als 20 Zeichen sein!")]
        public string EnMiddle { get; set; }

        [MaxLength(10, ErrorMessage = "Der Kurz-Text darf nicht länger als 10 Zeichen sein!")]
        public string EnShort { get; set; }

    }

    public static class EditViewModelExtensions
    {
        public static EditViewModel Map2ViewModel(this TranslationListEntry entry)
        {
            var ret = new EditViewModel();
            ret.GrpId = entry.Id;

            var de = entry.LangItems.Where(x => x.LangCode == "de_DE").First();
            var en = entry.LangItems.Where(x => x.LangCode == "en_EN").First();

            ret.DeLong = de.Long;
            ret.DeMiddle = de.Middle;
            ret.DeShort = de.Short;

            ret.EnLong = en.Long;
            ret.EnMiddle = en.Middle;
            ret.EnShort = en.Short;

            return ret;
        }
    }
}
