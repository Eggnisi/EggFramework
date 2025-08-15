#region

//文件创建者：Egg
//创建时间：11-19 02:53

#endregion

#if UNITY_EDITOR


using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed class ExcelStructModifyWindow : OdinEditorWindow
    {
        [LabelText("结构体定义"), ValidateInput("Validate")]
        public ExcelStructConfig Config;

        [HideInInspector] public ExcelStructColumn Host;

        private bool Validate(ExcelStructConfig config, ref string errString)
        {
            if (string.IsNullOrEmpty(config.TypeName))
            {
                errString = "必须定义类型名称";
                return false;
            }

            if (config.Columns.Count <= 0)
            {
                errString = "必须至少有一个成员";
                return false;
            }

            return true;
        }

        [Button("生成新结构体", ButtonSizes.Large)]
        public void GenerateNewStruct()
        {
            string e = "";
            if (Validate(Config, ref e))
            {
                for (var index = 0; index < Config.Columns.Count; index++)
                {
                    var excelColConfig = Config.Columns[index];
                    Config.Columns[index] = new ExcelColConfig
                    {
                        ExcelColType = excelColConfig.ExcelColType,
                        IsList       = excelColConfig.IsList,
                        Name         = excelColConfig.Name.Trim(),
                        ChineseName  = excelColConfig.ChineseName
                    };
                }
                Host.Save(Config);
            }
        }
    }
}
#endif