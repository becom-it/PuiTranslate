using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuiTranslate.Client.Extensions
{
    public static class TranslationTableJsExtension
    {
        public static async Task InitializeTranslationList(this IJSRuntime jsRuntime)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync("translationListJsHelper.Initialize");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while initializing the translation list: {ex.Message}", ex);
            }
        }
    }
}
