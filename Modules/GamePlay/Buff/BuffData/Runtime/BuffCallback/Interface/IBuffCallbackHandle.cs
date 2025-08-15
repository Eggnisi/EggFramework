#region

//文件创建者：Egg
//创建时间：02-22 02:08

#endregion

using System;

namespace EggFramework
{
    public interface IBuffCallbackHandle<T, in Tp> where T : IBuffCallbackHandle<T, Tp>
        where Tp : IBuffCallbackParam<T, Tp>
    {
        void Handle(BuffRunTimeInfo buffRunTimeInfo, Tp param, params object[] paramList);
    }
}