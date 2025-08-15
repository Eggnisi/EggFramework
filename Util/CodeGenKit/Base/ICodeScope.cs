#region

//文件创建者：Egg
//创建时间：09-23 02:19

#endregion

using System.Collections.Generic;

namespace EggFramework.CodeGenKit
{
    public interface ICodeScope : ICode
    {
        List<ICode> Codes { get; }
    }
}