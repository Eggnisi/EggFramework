#region

// 作者： Egg
// 时间： 2025 07.19 08:51

#endregion

namespace EggFramework
{
    public sealed class FuncContext : ICanHaveRawFunc, ICanExecuteFunc
    {
        public string     FuncId   => RawFunc?.FuncId ?? string.Empty;
        public RawFunc RawFunc  { get; private set; }
        
        public int        ParamCount  => RawFunc?.Params.Count ?? 0;
        
        void ICanHaveRawFunc.SetRawFunc(RawFunc rawFunc)
        {
            RawFunc = rawFunc;
        }

        object ICanExecuteFunc.Execute()
        {
            var handle = FuncMatcher.Match(FuncId, RawFunc.Params);
            if (handle == null) return null;
            handle.SetContext(this);
            return FuncActuator.Execute(handle, RawFunc.Params);
        }
    }
}