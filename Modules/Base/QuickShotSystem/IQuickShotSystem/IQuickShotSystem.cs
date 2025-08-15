#region

//文件创建者：Egg
//创建时间：09-08 09:13

#endregion

using QFramework;

namespace EggFramework
{
    public interface IQuickShotSystem : ISystem
    {
        //当前入栈的快照数量
        int CurrentQuickShotCount { get; }
        //历史快照数量
        int HistoryQuickShotCount { get; }
        void RegisterQuickShotClass(ICanQuickShot canQuickShot);
        void UnregisterQuickShotClass(ICanQuickShot canQuickShot);

        void Clear();

        //移除关卡数据
        void DeleteQuickShot(int quickShotId);
        //记录一个快照数据，但不会存入快照栈中
        int TakeRandomQuickShot();

        //收集快照数据并记录为一个快照
        int PushQuickShot();

        //收集快照数据
        void CollectQuickShotData();

        //记录为一个快照，（需要先调用CollectionQuickShotData），并且将快照入栈
        void PushQuickShotDataInStack();

        //回退到栈顶快照，同时移除栈顶
        void PopQuickShot();
        //回退到某个快照，移除对应数据
        void PopQuickShot(int id);
        //回退到栈顶快照，不移除数据
        void PeekQuickShot();
        //回退到某个快照，不移除数据
        void PeekQuickShot(int id);
    }
}