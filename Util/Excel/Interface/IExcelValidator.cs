#region

//文件创建者：Egg
//创建时间：04-10 10:56

#endregion

namespace EggFramework.Util.Excel
{
    public interface IExcelValidator<in T> where T : IExcelEntity
    {
        bool Validate(T data);
    }
}