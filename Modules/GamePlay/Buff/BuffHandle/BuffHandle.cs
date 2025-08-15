#region

//文件创建者：Egg
//创建时间：07-26 03:40

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework
{
    public sealed partial class BuffHandle : MonoBehaviour
    {
        [ShowInInspector] private List<BuffRunTimeInfo>  _buffRunTimeInfos = new();
        private                   IBuffDataModel         _buffDataModel;
        private readonly          Queue<BuffRunTimeInfo> _buff2Add = new();
        private void Awake()
        {
            _buffDataModel = TypeUtil.GetFirstActiveArchitecture().GetModel<IBuffDataModel>();
        }

        public List<BuffRunTimeInfo> GetBuffs()
        {
            return _buffRunTimeInfos.ToList();
        }

        public void ClearBuff()
        {
            _buffRunTimeInfos.Clear();
        }

        public void TriggerCustom(string callBackName, params object[] paramList)
        {
            foreach (var buffRunTimeInfo in _buffRunTimeInfos)
            {
                buffRunTimeInfo.BuffData.TriggerBuffModule(callBackName, buffRunTimeInfo, paramList);
            }
        }

        public bool BuffExist(string buffId)
        {
            return _buffRunTimeInfos.Any(buff => buff.BuffData.Id == buffId);
        }

        public bool BuffExistByName(string buffName)
        {
            return _buffRunTimeInfos.Any(buff => buff.BuffData.BuffName == buffName);
        }

        private void Update()
        {
            var deleteList = new List<BuffRunTimeInfo>();
            foreach (var buff in _buffRunTimeInfos)
            {
                var tick = (IBuffTicker)buff;
                if (!buff.BuffData.IsForever)
                {
                    tick.DurationTimer -= Time.deltaTime;
                    if (tick.DurationTimer < 0)
                    {
                        deleteList.Add(buff);
                    }
                }

                if (buff.BuffData.ExistBuffModule(InnerBuffConstant.ON_TICK))
                {
                    tick.TickTimer -= Time.deltaTime;
                    if (!(tick.TickTimer < 0)) continue;
                    buff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_TICK, buff);
                    tick.TickTimer = buff.BuffData.TickTime;
                }
            }

            foreach (var buff in deleteList)
            {
                RemoveBuff(buff);
            }

            while (_buff2Add.Count > 0)
            {
                var buff = _buff2Add.Dequeue();
                _buffRunTimeInfos.Add(buff);
            }
        }

        [Button]
        public bool AddBuffByName(string buffName, GameObject creator)
        {
            return AddBuff(new BuffRunTimeInfo
            {
                BuffData = _buffDataModel.GetBuffDataByName(buffName),
                Creator  = creator,
                Target   = gameObject
            });
        }

        [Button]
        public bool AddBuff(string buffId, GameObject creator)
        {
            return AddBuff(new BuffRunTimeInfo
            {
                BuffData = _buffDataModel.GetBuffDataById(buffId),
                Creator  = creator,
                Target   = gameObject
            });
        }

        public bool AddBuff(BuffRunTimeInfo buff)
        {
            var findBuff = _buffRunTimeInfos.Find(runtimeBuff => runtimeBuff.BuffData.Id == buff.BuffData.Id);
            if (findBuff == null)
            {
                _buff2Add.Enqueue(buff);
                var tick = (IBuffTicker)buff;
                tick.DurationTimer = buff.BuffData.Duration;
                tick.TickTimer     = buff.BuffData.TickTime;
                buff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_CREATE, buff);
                if (buff.BuffData.TriggerTickOnCreate)
                {
                    buff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_TICK, buff);
                }
            }
            else
            {
                var ticker = (IBuffTicker)findBuff;
                switch (findBuff.BuffData.BuffUpdateEnum)
                {
                    case BuffUpdateEnum.AddTime:
                        ticker.DurationTimer += findBuff.BuffData.Duration;
                        break;
                    case BuffUpdateEnum.ReplaceAndAddStack:
                        ticker.DurationTimer = findBuff.BuffData.Duration;
                        if (ticker.CurStack < findBuff.BuffData.MaxStack)
                        {
                            ticker.CurStack++;
                            findBuff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_ADD_STACK, findBuff);
                        }

                        break;
                    case BuffUpdateEnum.KeepAndAddStack:
                        if (ticker.CurStack < findBuff.BuffData.MaxStack)
                        {
                            ticker.CurStack++;
                            findBuff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_ADD_STACK, findBuff);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return true;
        }

        public bool RemoveBuffById(string buffId)
        {
            var findBuff = _buffRunTimeInfos.Find(runtimeBuff => runtimeBuff.BuffData.Id == buffId);
            if (findBuff == null) return false;
            RemoveBuff(findBuff);
            return true;
        }

        public bool RemoveBuff(BuffRunTimeInfo buff)
        {
            var findBuff = _buffRunTimeInfos.Find(runtimeBuff => runtimeBuff.BuffData.Id == buff.BuffData.Id);
            if (findBuff == null)
            {
                return false;
            }

            var tick = (IBuffTicker)findBuff;
            switch (findBuff.BuffData.BuffRemoveStackEnum)
            {
                case BuffRemoveStackEnum.Clear:
                    findBuff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_REMOVE, findBuff);
                    _buffRunTimeInfos.Remove(findBuff);
                    break;
                case BuffRemoveStackEnum.Reduce:
                    tick.CurStack--;
                    findBuff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_REDUCE_STACK, findBuff);
                    if (tick.CurStack <= 0)
                    {
                        _buffRunTimeInfos.Remove(findBuff);
                        findBuff.BuffData.TriggerBuffModule(InnerBuffConstant.ON_REMOVE, findBuff);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }
    }
}