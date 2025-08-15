#region

//文件创建者：Egg
//创建时间：02-22 02:09

#endregion

namespace EggFramework
{
    public interface IBuffCallbackParam<T, Tp> where T : IBuffCallbackHandle<T, Tp> where Tp : IBuffCallbackParam<T, Tp>
    {
    }
}