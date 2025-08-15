#region

// 作者： Egg
// 时间： 2025 07.19 09:13

#endregion

namespace EggFramework
{
    public static class FuncManager
    {
        public static object DoFunc(string func)
        {
            var rawFunc = FuncParser.ParseRawFunc(func); 
            return InnerDoFunc(rawFunc);
        }

        private static object InnerDoFunc(RawFunc rawFunc)
        {
            var funcContext = new FuncContext();
            ((ICanHaveRawFunc)funcContext).SetRawFunc(rawFunc);
            return ((ICanExecuteFunc)funcContext).Execute();
        }
    }
}