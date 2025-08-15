#region

// 作者： Egg
// 时间： 2025 07.19 08:58

#endregion

using System.Collections.Generic;

namespace EggFramework
{
    public interface IFuncHandle
    {
        void SetContext(FuncContext context);
        
        object Handle(params object[] paramsObjects);
        
        bool ParamValidCheck(List<string> rawParams);
    }
}