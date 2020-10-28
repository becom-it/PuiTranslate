using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PuiTranslate.Common.Models
{
    public class TranslationListEntry
    {
        public int Id { get; set; } = default;
        public string DescriptionDE { get; set; } = default;
        public string DescriptionEN { get; set; } = default;

        public List<LanguageItem> LangItems { get; set; } = new List<LanguageItem>();
    }

    public class LanguageItem
    {
        public string LangCode { get; set; } = string.Empty;
        public string LangText { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Der Kurztext darf nicht lägner als 10 Zeichen lang sein!")]
        public string Short { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Der Mitteltext darf nicht lägner als 20 Zeichen lang sein!")]
        public string Middle { get; set; } = string.Empty;

        [Required]
        public string Long { get; set; } = string.Empty;
    }

}
