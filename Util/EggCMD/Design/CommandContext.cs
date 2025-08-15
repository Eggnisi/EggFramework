#region

//文件创建者：Egg
//创建时间：03-25 10:32

#endregion

using System.Collections.Generic;
using System.Linq;

namespace EggFramework.Util.EggCMD
{
    public sealed class CommandContext : ICanExecuteCommand, ICanHaveRawCommand
    {
        public string     CommandId   => RawCommand?.CommandId ?? string.Empty;
        public RawCommand RawCommand  { get; private set; }
        public int        ParamCount  => RawCommand?.Params.Count ?? 0;
        public int        OptionCount => RawCommand?.OptionDic.Count ?? 0;

        void ICanHaveRawCommand.SetRawCommand(RawCommand rawCommand)
        {
            RawCommand = rawCommand;
        }

        void ICanExecuteCommand.ExecuteCommand()
        {
            var handle = CommandMatcher.Match(CommandId, RawCommand.Params);
            if (handle == null) return;
            handle.SetContext(this);
            CommandActuator.Execute(handle, RawCommand.Params);
        }

        public bool OptionExist(string optionName)
        {
            return RawCommand?.OptionDic.ContainsKey(optionName) ?? false;
        }
        
        public bool OptionExist(params string[] optionNames)
        {
            return RawCommand != null && optionNames.Aggregate(false, (current, optionName) => current | RawCommand.OptionDic.ContainsKey(optionName));
        }

        public List<string> GetOptionValue(string optionName)
        {
            if (RawCommand == null) return new List<string>();
            return RawCommand.OptionDic.GetValueOrDefault(optionName, new List<string>());
        }

        public string GetFirstOptionValue(string optionName)
        {
            if (RawCommand == null) return string.Empty;
            var list = RawCommand.OptionDic.GetValueOrDefault(optionName, new List<string> { string.Empty });
            return list.Count > 0 ? list[0] : string.Empty;
        }
    }
}