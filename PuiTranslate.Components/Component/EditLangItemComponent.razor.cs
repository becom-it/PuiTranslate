using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq.Expressions;

namespace PuiTranslate.Components.Component
{
    public class EditLangItemComponentBase : InputBase<string>
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public Expression<Func<string>> ValidationFor { get; set; }

        protected override bool TryParseValueFromString(string value, out string result, out string validationErrorMessage)
        {
            result = value;
            validationErrorMessage = null;
            return true;
        }

        //private string _value;
        //[Parameter]
        //public string Value
        //{
        //    get => _value;
        //    set
        //    {
        //        if (_value == value) return;
        //        _value = value;
        //        ValueChanged.InvokeAsync(value);
        //    }
        //}

        //[Parameter]
        //public EventCallback<string> ValueChanged { get; set; }
    }
}
