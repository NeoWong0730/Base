using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_Multi_PlayTypeNew : UIBase, UI_Multi_PlayTypeNew_Layout.IListener
    {
        private UI_Multi_PlayTypeNew_Layout m_Layout = new UI_Multi_PlayTypeNew_Layout();

        private int mSeriesIndex = 0;

        List<CSVNewBiographySeries.Data>  mSortedSeriesList = new List<CSVNewBiographySeries.Data>();
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetCharpterInfoCount(mSortedSeriesList[0].instance_id.Count);

            m_Layout.SetCharpterFocus(0);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceDataUpdate, RefreshSeriesInfo,toRegister);
            Sys_Instance.Instance.eventEmitter.Handle<uint>(Sys_Instance.EEvents.RewardRefresh, OnRewardSelect, toRegister);
        }
        protected override void OnOpen(object arg)
        {
            Sys_Instance.Instance.InstanceDataReq(Sys_Instance.BioInstanceID);

            ReadSeriesInfo();

        }
        protected override void OnShow()
        {            
            RefreshSeriesInfo();
            //m_Layout.FocusInfo(0);
        }

        protected override void OnHide()
        {            
            m_Layout.Close();
        }


        //刷新章节列表
        private void RefreshChapterInfo()
        {
            var seriesItem = mSortedSeriesList[mSeriesIndex];

            int copyCount = seriesItem.instance_id.Count;

            m_Layout.SetCharpterInfoCount(copyCount);

            Sys_Instance.ServerInstanceData data = Sys_Instance.Instance.GetServerInstanceData(130);

            if (data == null || data.instanceCommonData == null)
                return;

            for (int i = 0; i < copyCount; i++)
            {
                var copyItem = CSVInstance.Instance.GetConfData(seriesItem.instance_id[i]);

                var Netdata = data == null ? null : data.GetInsEntry(copyItem.id);

                bool isLock = (Netdata == null || Netdata.Unlock == false);

                m_Layout.setCharpterIndex(i, i);

                m_Layout.setCharpterName(i, LanguageHelper.GetTextContent(copyItem.Name));

                m_Layout.SetCharpterLock(i, isLock);

                m_Layout.SetCharpterMinLevel(i, (int)copyItem.lv[0]);

                m_Layout.SetCharpterScore(i, (uint)copyItem.RecommondPoint, Sys_Attr.Instance.rolePower >= (ulong)copyItem.RecommondPoint);
                if (isLock)
                {
                    uint level = copyItem.lv[0];

                    m_Layout.SetCharpterLockLevel(i, (int)level, (uint)(Sys_Role.Instance.Role.Level >= level ? 2009606 : 2009605) );

                    var preitem = CSVInstance.Instance.GetConfData(copyItem.pre_instance);

                    bool islastfini = false;

                    if (preitem != null)
                    {
                        var NetPredata = data == null ? null : data.GetInsEntry(copyItem.pre_instance);

                        if (NetPredata != null)
                        {
                            islastfini = NetPredata.PassRecord;
                        }
                    }


                    m_Layout.SetCharpterLockLastLeve(i,preitem == null ? 0:preitem.Name, (uint)(islastfini ? 2009604 : 2009603));
                }

                m_Layout.SetCharpterBg(i,copyItem.bg);
            }


            uint selectID = data.instanceCommonData.SelectedInstanceId;

            var instanceData = CSVInstance.Instance.GetConfData(selectID);


            bool bHideReward = data.instanceCommonData.Entries.Count <= 1;

            m_Layout.SetBtnRewardActive(!bHideReward);
        }

        private void ReadSeriesInfo()
        {
            var data = CSVNewBiographySeries.Instance.GetAll();

            mSortedSeriesList.Clear();

            foreach (var item in data)
            {
                mSortedSeriesList.Add(item);
            }

            mSortedSeriesList.Sort((x, y) => {

                if (x.sort_id > y.sort_id)
                    return 1;
                if (x.sort_id < y.sort_id)
                    return -1;
                return 0;
            });
        }
        //刷新传记列表
        private void RefreshSeriesInfo()
        {
            int count = mSortedSeriesList.Count;

            m_Layout.SetCardInfoCount(count);

            for (int i = 0; i < count; i++)
            {
                m_Layout.setCardIcon(i,mSortedSeriesList[i].icon);
                m_Layout.setCardIndex(i, i);
               
            }

            OnClickChardInfo(mSeriesIndex);
            m_Layout.setCardSelect(mSeriesIndex, true);

            var dailydata = CSVDailyActivity.Instance.GetConfData(130);

            uint usedTimes = Sys_Instance_Bio.Instance.insData == null ? 0 : Sys_Instance_Bio.Instance.insData.StageRewardLimit.UsedTimes;

            m_Layout.SetHaveRewardTimes(dailydata.Times - usedTimes);

        }

        private void OnRewardSelect(uint id)
        {
            RefreshChapterInfo();

        }


        public void Close()
        {
            CloseSelf();
        }

        public void OnClickReward()
        {
            
            var item = mSortedSeriesList[mSeriesIndex];

            UIManager.OpenUI(EUIID.UI_Multi_Reward,false, item.id);
        }

        public void OnClickChapterInfo(int index)
        {
            Sys_Instance.ServerInstanceData data = Sys_Instance.Instance.GetServerInstanceData(130);

            var item = mSortedSeriesList[mSeriesIndex];

            uint charpterID = item.instance_id[index];

            var Netdata = data == null ? null:data.GetInsEntry(charpterID);

            bool isLock = (Netdata == null || Netdata.Unlock == false);

            if (isLock)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009637));
                return;
            }
               

            Sys_Instance.Instance.CurInstancID = charpterID;

            UI_Multi_InfoNew_Parmas parmas = new UI_Multi_InfoNew_Parmas() {

                SeriesID = item.id,
                InstanceID = charpterID
            };

            UIManager.OpenUI(EUIID.UI_Bio_Info, false, parmas);
        }

        public void OnClickChardInfo(int index)
        {
           var item =  mSortedSeriesList[index];

            m_Layout.SetModelActor(0,item.model);

            mSeriesIndex = index;

            Sys_Instance.Instance.CurSeriesID = item.id;

            m_Layout.SetCurCardName(LanguageHelper.GetTextContent(item.Name));

            RefreshChapterInfo();

            m_Layout.setFocusIndex(index, mSortedSeriesList.Count);
        }

        private List<uint> animatorList = new List<uint>();
        public void ShowModleFinish(ShowSceneControl control, DisplayControl<EHeroModelParts> displayControl)
        {
            var item = mSortedSeriesList[mSeriesIndex];

            animatorList.Clear();

          //  animatorList.Add(item.action_id);

            displayControl?.mAnimation.UpdateHoldingAnimations(item.action_id);


            control.mRoot.transform.position = new Vector3(item.positionx / 10000f, item.positiony / 10000f, item.positionz / 10000f);
            control.mRoot.transform.localScale = new Vector3(item.scale / 10000f, item.scale / 10000f, item.scale / 10000f);
        }

        public void OnClickHaveRewardTimes()
        {
            m_Layout.SetRuleActive(true);
        }

        public void OnClickCloseRule()
        {
            m_Layout.SetRuleActive(false);
        }
    }
}
