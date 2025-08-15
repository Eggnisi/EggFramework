#region

//文件创建者：Egg
//创建时间：04-21 09:24

#endregion

using Sirenix.OdinInspector;

namespace EggFramework.ObjectPool
{
    public enum EObjectPoolExpandMode
    {
        [LabelText("自动扩容")]      AutoExpand,                 //无可用对象自动扩容（直到到达高水位，变为不允许扩容）
        [LabelText("预测扩容")]      PredictiveExpand,           //预测扩容，在未达到高水位的前提下保证池内对象数量大于等于池外对象的20%，避免临时大量扩容
        [LabelText("从不扩容")]      DontExpand,                 //不允许扩容(对象数量始终为低水位数量)
    }
}