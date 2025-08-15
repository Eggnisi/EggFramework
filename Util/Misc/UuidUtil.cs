#region

//文件创建者：Egg
//创建时间：09-19 08:53

#endregion

using System;

namespace EggFramework.Util
{
    public static class UuidUtil
    {
        public static string GetRandomUuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}