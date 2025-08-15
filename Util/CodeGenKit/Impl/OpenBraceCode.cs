#region

//文件创建者：Egg
//创建时间：09-23 02:20

#endregion

namespace EggFramework.CodeGenKit
{
    public sealed class OpenBraceCode:ICode
    {
        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine("{");
        }
    }
}