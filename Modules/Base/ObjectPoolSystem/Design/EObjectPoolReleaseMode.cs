#region

//文件创建者：Egg
//创建时间：04-21 09:48

#endregion

using Sirenix.OdinInspector;

namespace EggFramework.ObjectPool
{
    public enum EObjectPoolReleaseMode
    {
        [LabelText("使用池计时器")]  UsePoolTimeout,    //对象池维护计时器，长期不使用，释放对象至数量为低水位
        [LabelText("使用对象计时器")] UseObjectTimeout,  //每个对象维护计时器，长期不使用将释放（直到池内数量下降至低水位）
        [LabelText("预测释放")]    PredictiveRelease, //预测回收，在未降至低水位的前提下保证池内对象数量小于等于池外对象的30%，避免内存浪费
        [LabelText("从不释放")]    DontRelease,       //不进行释放
    }
}