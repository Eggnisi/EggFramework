#region

//文件创建者：Egg
//创建时间：08-13 09:13

#endregion

using System.Collections.Generic;
using System.Linq;

namespace EggFramework.Util.EggCMD
{
    public sealed class DefineTokenSource : ICommandTokenSource
    {
        private static readonly List<string> _innerDefines = new()
        {
            "ENABLE_STORAGE_ENCRYPTION",
            "UNITASK_DOTWEEN_SUPPORT",
            "EGG_PROCEDURE_LOG_LEVEL_NODE",
            "EGG_PROCEDURE_LOG_LEVEL_NONE",
            "EGG_PROCEDURE_LOG_LEVEL_PROCEDURE",
            "EGG_PROCEDURE_LOG_LEVEL_SYSTEM",
        };
        public List<string> GetTokenSource(List<string> tokens) => _innerDefines.ToList();
    }
}