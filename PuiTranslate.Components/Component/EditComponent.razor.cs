using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PuiTranslate.Common.Models;
using PuiTranslate.Services;
using System;
using System.Threading.Tasks;

namespace PuiTranslate.Components.Component
{
    public partial class EditComponent
    {
        public EditViewModel ViewModel { get; set; } = new EditViewModel();

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public ITranslationDataService TranslationDataService { get; set; }

        [Parameter]
        public int? TranslationId { get; set; }

        public TranslationListEntry Entity { get; set; } = null;


        protected override async Task OnInitializedAsync()
        {
            if (TranslationId.HasValue)
            {
                Entity = await TranslationDataService.GetDetail(TranslationId.Value);
                ViewModel = Entity.Map2ViewModel();
            } else
            {
                var newid = await TranslationDataService.GetNextId();
                Entity = new TranslationListEntry();
                Entity.Id = newid;
                Entity.LangItems.Add(new LanguageItem { LangCode = "de_DE", LangText = "Deutsch" });
                Entity.LangItems.Add(new LanguageItem { LangCode = "en_EN", LangText = "Englisch" });

                ViewModel = Entity.Map2ViewModel();
            }
        }

        public async Task Save()
        {
            await TranslationDataService.UpdateTranslations(ViewModel);
            await JSRuntime.InvokeVoidAsync("history.back");
        }

        public async Task Cancel()
        {
            await JSRuntime.InvokeVoidAsync("history.back");
        }
    }
}
