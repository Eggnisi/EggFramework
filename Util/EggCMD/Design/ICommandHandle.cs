#region

//文件创建者：Egg
//创建时间：03-25 10:23

#endregion

using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public interface ICommandHandle
    {
        void SetContext(CommandContext context);
        void Handle(params object[] paramsObjects);
        bool ParamValidCheck(List<string> rawParams);
    }
}