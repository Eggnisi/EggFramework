#region

//文件创建者：Egg
//创建时间：03-25 11:44

#endregion

namespace EggFramework.Util.EggCMD
{
    public static class CommandManager
    {
        public static void DoCommand(string command)
        {
            var rawCommands = CommandParser.ParseRawCommands(command);
            foreach (var rawCommand in rawCommands)
            {
                DoCommand(rawCommand);
            }
        }

        private static void DoCommand(RawCommand command)
        {
            var commandContext = new CommandContext();
            ((ICanHaveRawCommand)commandContext).SetRawCommand(command);
            ((ICanExecuteCommand)commandContext).ExecuteCommand();
        }
    }
}