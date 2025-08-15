#region

//文件创建者：Egg
//创建时间：12-03 08:40

#endregion

using EggFramework.Util;

namespace EggFramework.UIUtil
{
    public sealed class BlackScreenManager : MonoSingleton<BlackScreenManager>
    {
        private BlackScreenController _controller => _inst ??= BlackScreenFactory.Create();

        private BlackScreenController _inst;

        public void EnterBlack(float time)
        {
            _controller.OnBlackScreen(new BlackScreenEvent
            {
                EnterBlack = true,
                Duration   = time
            });
        }
        
        public void ExitBlack(float time)
        {
            _controller.OnBlackScreen(new BlackScreenEvent
            {
                EnterBlack = false,
                Duration   = time
            });
        }
    }
}