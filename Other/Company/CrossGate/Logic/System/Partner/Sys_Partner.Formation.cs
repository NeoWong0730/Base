using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;

namespace Logic
{
    public partial class Sys_Partner : SystemModuleBase<Sys_Partner>
    {
        public bool isSelectOp = false;
        public PartnerFormOperation formSelectOp;

        public class PartnerFormOperation
        {
            public uint partnerId;
            public int formIndex;
            public int posIndex;
        }

        #region Logic
        public PartnerFormation GetFormationByIndex(int _formIndex)
        {
            if (_formIndex < partnerInfo.FmList.Count)
            {
                return partnerInfo.FmList[_formIndex];
            }

            return null;
        }

        /// <summary>
        /// 获取阵容配置
        /// </summary>
        /// <returns></returns>
        public IList<PartnerFormation> GetFmList()
        {
            return partnerInfo.FmList;
        }

        /// <summary>
        /// 获取当前选中阵容ID
        /// </summary>
        /// <returns></returns>
        public uint GetCurFmList()
        {
            return  partnerInfo.CurrentIndex;
        }
        public void SetSelectFormation(int _formIndex)
        {
            if (partnerInfo.CurrentIndex != (uint)_formIndex)
            {
                partnerInfo.CurrentIndex = (uint)_formIndex;
                eventEmitter.Trigger(EEvents.OnFormationSelectedNtf);
                ClearSelectState();
                //eventEmitter.Trigger(EEvents.OnFormRefreshNotification);
            }
        }

        public bool IsSelectedFormation(int _formIndex)
        {
            return partnerInfo.CurrentIndex == (uint)_formIndex;
        }

        //TODO:
        public bool IsPosCanOp(int _formIndex, int _index)
        {
            if (_index == 0)
                return true;

            return partnerInfo.FmList[_formIndex].Pa[_index - 1] != 0;
        }

        public bool IsSelectedFormPos(int _formIndex, int _index)
        {
            if (formSelectOp != null)
            {
                if (formSelectOp.formIndex == _formIndex && formSelectOp.posIndex == _index)
                    return true;
            }

            return false;
        }

        public void OnSelectFormPos(PartnerFormOperation _formOp)
        {
            if (!isSelectOp)
            {
                isSelectOp = true;
                formSelectOp = _formOp;
                eventEmitter.Trigger(EEvents.OnFormSelectNotification, _formOp);
            }
            else
            {
                if (formSelectOp.formIndex == _formOp.formIndex)
                {
                    if (formSelectOp.posIndex != _formOp.posIndex)
                    {
                        formSelectOp = _formOp;
                        eventEmitter.Trigger(EEvents.OnFormSelectNotification, _formOp);
                    }
                }
                else
                {
                    formSelectOp = _formOp;
                    eventEmitter.Trigger(EEvents.OnFormSelectNotification, _formOp);
                }
            }
        }

        public void OnDownFormOrReplace(PartnerFormOperation _formOp)
        {
            if (_formOp.formIndex == formSelectOp.formIndex
                    && _formOp.posIndex == formSelectOp.posIndex)
            {
                //卸下
                partnerInfo.FmList[formSelectOp.formIndex].Pa[formSelectOp.posIndex] = 0;
                //所有后面数据,向前移一位
                for (int i = formSelectOp.posIndex; i < partnerInfo.FmList[formSelectOp.formIndex].Pa.Count; ++i)
                {
                    int nextIndex = i + 1;
                    if (nextIndex < partnerInfo.FmList[formSelectOp.formIndex].Pa.Count)
                    {
                        partnerInfo.FmList[formSelectOp.formIndex].Pa[i] = partnerInfo.FmList[formSelectOp.formIndex].Pa[nextIndex];
                    }
                    else
                    {
                        partnerInfo.FmList[formSelectOp.formIndex].Pa[nextIndex - 1] = 0;
                        break;
                    }
                }

                isSelectOp = false;
                formSelectOp = null;
                //refresh
                eventEmitter.Trigger(EEvents.OnFormRefreshNotification);
            }
            else if (_formOp.formIndex == formSelectOp.formIndex)
            {
                //替换
                partnerInfo.FmList[formSelectOp.formIndex].Pa[formSelectOp.posIndex] = _formOp.partnerId;
                partnerInfo.FmList[_formOp.formIndex].Pa[_formOp.posIndex] = formSelectOp.partnerId;

                isSelectOp = false;
                formSelectOp = null;
                //refresh
                eventEmitter.Trigger(EEvents.OnFormRefreshNotification);
            }

            Sys_Hint.Instance.PushEffectInNextFight();
        }

        public void OnUpForm(uint infoId)
        {
            if (isSelectOp)
            {
                //set data
                partnerInfo.FmList[formSelectOp.formIndex].Pa[formSelectOp.posIndex] = infoId;
            }

            isSelectOp = false;
            formSelectOp = null;
            //refresh
            eventEmitter.Trigger(EEvents.OnFormRefreshNotification);

            Sys_Hint.Instance.PushEffectInNextFight();

            uint count = 0;
            for (int i = 0; i < partnerInfo.FmList[(int)partnerInfo.CurrentIndex].Pa.Count; ++i)
            {
                if (partnerInfo.FmList[(int)partnerInfo.CurrentIndex].Pa[i] != 0u)
                    count++;
            }

            //魔力宝典
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent2, EMagicAchievement.Event19, count);
        }
        

        public bool IsCanReplace(uint _infoId, PartnerFormOperation _formData)
        {
            if (_infoId == _formData.partnerId)
                return false;

            return !partnerInfo.FmList[_formData.formIndex].Pa.Contains(_infoId);
        }

        public bool IsInForm(uint infoId)
        {
            return partnerInfo.FmList[(int)partnerInfo.CurrentIndex].Pa.Contains(infoId);
        }

        public void ClearSelectState()
        {
            isSelectOp = false;
            formSelectOp = null;
            //refresh
            eventEmitter.Trigger(EEvents.OnFormRefreshNotification);
        }

        public List<int> BelongFormations(uint partnerId)
        {
            List<int> list = new List<int>();
            for(int i = 0; i < partnerInfo.FmList.Count; ++i)
            {
                if (partnerInfo.FmList[i].Pa.IndexOf(partnerId) >= 0
                    && !list.Contains(i))
                {
                    list.Add(i);
                }
            }

            return list;
        }
        #endregion
    }
}