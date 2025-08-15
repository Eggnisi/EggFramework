#region

//文件创建者：Egg
//创建时间：02-21 12:38

#endregion

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EggFramework.Util;
using QFramework;
using UnityEngine.AddressableAssets;

namespace EggFramework
{
    public sealed class BuffDataModel : AbstractModel, IBuffDataModel
    {
        private          BuffDataRef                  _buffDataRef;
        private readonly Dictionary<string, BuffData> _buffDatas = new();
        private readonly Dictionary<string, BuffData> _buffDataNameDic = new();
        private          List<string>                 _customTriggers;
        private          BuffConfig                   _config;

        private async UniTask LoadBuffData()
        {
            _config         = StorageUtil.LoadFromSettingFile(nameof(BuffConfig), new BuffConfig());
            _customTriggers = _config.CustomTriggers;
            _buffDataRef    = await Addressables.LoadAssetAsync<BuffDataRef>(nameof(BuffDataRef)).ToUniTask();
            foreach (var buffData in _buffDataRef.BuffDatas)
            {
                _buffDatas[buffData.Id]             = buffData;
                _buffDataNameDic[buffData.BuffName] = buffData;
            }
        }

        protected override void OnInit()
        {
           
        }

        public override async UniTask AsyncInit()
        {
            await LoadBuffData();
        }

        public BuffData GetBuffDataByName(string buffName)
        {
            return _buffDataNameDic.GetValueOrDefault(buffName, null);
        }

        public BuffData GetBuffDataById(string buffId)
        {
            return _buffDatas.GetValueOrDefault(buffId, null);
        }

        public IReadOnlyList<string> GetCustomTriggers()
        {
            return _customTriggers;
        }
    }
}