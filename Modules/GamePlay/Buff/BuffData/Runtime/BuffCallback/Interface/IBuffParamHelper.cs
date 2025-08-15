#region

//文件创建者：Egg
//创建时间：02-22 03:58

#endregion

using System.Collections.Generic;

namespace EggFramework
{
    public interface IBuffParamHelper
    {
        
    }
    
    public interface IBuffParamValueGetter : IBuffParamHelper
    {
        List<string> ValueGetter();
    }
    
    public interface IBuffParamValidator : IBuffParamHelper
    {
        bool Validate(string stringValue, ref string errMsg);
    }

    public interface IBuffParamParser : IBuffParamHelper
    {
        object Parse(string stringValue);
    }
}