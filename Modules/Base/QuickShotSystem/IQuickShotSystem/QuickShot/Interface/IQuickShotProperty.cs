#region

//文件创建者：Egg
//创建时间：07-28 10:56

#endregion

using System;
using QFramework;

namespace EggFramework
{
    public interface IQuickShotProperty
    {
        Action Upload { get; set; }
        Action Download { get; set; }

        void BeforeUpload();
        void AfterDownload();
    }

    public class QuickShotProperty<T> : BindableProperty<T>, IQuickShotProperty
    {
        Action IQuickShotProperty.Upload { get; set; }

        private Action _beforeUploadAction;
        Action IQuickShotProperty.Download { get; set; }

        private Action _afterDownloadAction;

        public QuickShotProperty<T> RegisterShotProperty(ICanQuickShot host, string name)
        {
            host.RegisterQuickShotProperty(name, this);
            return this;
        }

        public void BeforeUpload()
        {
            _beforeUploadAction?.Invoke();
        }

        public void AfterDownload()
        {
            _afterDownloadAction?.Invoke();
        }

        public QuickShotProperty<T> AddUploadAction(Action action)
        {
            _beforeUploadAction += action;
            return this;
        }

        public QuickShotProperty<T> AddDownloadAction(Action action)
        {
            _afterDownloadAction += action;
            return this;
        }
    }
}