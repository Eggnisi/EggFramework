#region

//文件创建者：Egg
//创建时间：10-23 01:46

#endregion

using System;
using System.Collections.Generic;
using EggFramework.Util.Excel;
using QFramework;

namespace EggFramework
{
    public interface IExcelDataModel : IModel
    {
        bool Inited { get; }
        List<T> GetEntityData<T>() where T : IExcelEntity;
        T GetEntityDataById<T>(int id) where T : IExcelEntity;
        T GetEntityDataById<T>(string id) where T : IExcelEntity;
        void AssertEntityExist<T>(string id) where T : IExcelEntity;
        
        
        void TriggerOnFinishInitOrLater(Action action);
    }
}