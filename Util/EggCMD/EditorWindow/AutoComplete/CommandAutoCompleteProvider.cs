#region

//文件创建者：Egg
//创建时间：03-27 09:24

#endregion

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EggFramework.Util.EggCMD
{
    public sealed class CommandAutoCompleteProvider : AbstractCommandAutoCompleteProvider
    {
        private TextField _textField;

        public CommandAutoCompleteProvider(CommandInputContext context, TextField textField,
            Dictionary<string, Helper> metadata) : base(context, metadata)
        {
            _textField = textField;
        }

        protected override void AfterInsertSuggestion()
        {
            _textField.SetValueWithoutNotify(string.Join(" ", _context.Tokens));
            _textField.cursorIndex = _textField.text.Length - 1;
            _textField.Focus();
        }
    }
}