#region

//文件创建者：Egg
//创建时间：02-21 12:37

#endregion

using System.Collections.Generic;
using QFramework;

namespace EggFramework
{
    public interface IBuffDataModel : IModel
    {
        BuffData GetBuffDataByName(string buffName);
        BuffData GetBuffDataById(string buffId);

        IReadOnlyList<string> GetCustomTriggers();
    }
}