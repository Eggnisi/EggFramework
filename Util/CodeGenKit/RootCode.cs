#region

//文件创建者：Egg
//创建时间：09-23 02:37

#endregion

using System.Collections.Generic;

namespace EggFramework.CodeGenKit
{
    public sealed class RootCode : ICodeScope
    {
        public void Gen(ICodeWriter writer)
        {
            foreach (var code in Codes)
            {
                code.Gen(writer);
            }
        }

        public List<ICode> Codes { get; } = new();
    }
}