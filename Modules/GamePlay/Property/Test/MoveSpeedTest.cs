#region

//文件创建者：Egg
//创建时间：08-20 06:58

#endregion

namespace EggFramework
{
    public static class MoveSpeedTest
    {
        public static void RunTest()
        {
            var propertyManager = new PropertyHandle();
            var moveSpeedValueConfig = new BaseProperty(5f, "MoveSpeed-Value-Config").Register(propertyManager)
                .NotifyParentDirty("MoveSpeed-Value");
            var moveSpeedValueBuff = new BaseProperty(0f, "MoveSpeed-Value-Buff").Register(propertyManager)
                .NotifyParentDirty("MoveSpeed-Value");
            var moveSpeedValueOther = new BaseProperty(0f, "MoveSpeed-Value-Other").Register(propertyManager)
                .NotifyParentDirty("MoveSpeed-Value");

            var moveSpeedValue = new ComputeProperty(() =>
            {
                var configValue = moveSpeedValueConfig.GetValue();
                var bufferValue = moveSpeedValueBuff.GetValue();
                var otherValue  = moveSpeedValueOther.GetValue();
                return configValue + bufferValue + otherValue;
            }, "MoveSpeed-Value").Register(propertyManager);
        }
    }
}