using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PuiTranslate.Client.Extensions;
using PuiTranslate.Common.Models;
using PuiTranslate.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PuiTranslate.Components.Component
{
    public partial class TranslationList
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public ITranslationDataService TranslationDataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public int CurrentPage { get; set; } = 1;

        public List<TranslationListEntry> Entries { get; set; } = new List<TranslationListEntry>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await reload();
            }
        }

        public async Task Prev()
        {
            if (CurrentPage > 1) CurrentPage--;
            await reload();
        }

        public async Task Next()
        {
            CurrentPage++;
            await reload();
        }

        private async Task reload()
        {
            Entries = await TranslationDataService.LoadData(CurrentPage);
            StateHasChanged();
            await JSRuntime.InitializeTranslationList();
        }

        public async Task FilterChanged(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                await reload();
                return;
            }

            Entries = await TranslationDataService.FilterData(text);
            StateHasChanged();
            await JSRuntime.InitializeTranslationList();
        }

        public void Edit(TranslationListEntry entry)
        {
            NavigationManager.NavigateTo($"Edit/{entry.Id}");
        }

        public void Create()
        {
            NavigationManager.NavigateTo("Edit");
        }
    }
}
