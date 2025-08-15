#region

//文件创建者：Egg
//创建时间：09-23 02:17

#endregion

namespace EggFramework.CodeGenKit
{
    public interface ICodeWriter
    {
        int IndentCount { get; set; }
        void WriteLine(string code = null);
    }
}