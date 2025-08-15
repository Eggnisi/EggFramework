#region

//文件创建者：Egg
//创建时间：07-28 10:31

#endregion

using System.Collections.Generic;

namespace EggFramework
{
    public interface ICanQuickShot
    {
        void RegisterQuickShotProperty<T>(string propName, QuickShotProperty<T> quickShotProperty);
        void UnRegisterQuickShotProperty<T>(QuickShotProperty<T> quickShotProperty);
        void DownloadQuickShot(Dictionary<string, object> quickShotData);

        Dictionary<string, object> UploadQuickShotData();

        void BeforeUpload();

        void AfterDownload();
    }
}