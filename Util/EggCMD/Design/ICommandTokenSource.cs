#region

//文件创建者：Egg
//创建时间：03-27 10:26

#endregion

using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public interface ICommandTokenSource
    {
        List<string> GetTokenSource(List<string> tokens);
    }
}