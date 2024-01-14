using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_Multi_PlayType : UIBase, UI_Multi_PlayType_Layout.IListener
    {
        private UI_Multi_PlayType_Layout m_Layout = new UI_Multi_PlayType_Layout();


       // private uint CopyID = 0;//副本ID


        //private uint mSeriesID = 0;
        private int mSeriesIndex = 0;

        List<CSVBiographySeries.Data>  mSortedSeriesList = new List<CSVBiographySeries.Data>();
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetCharpterInfoCount(3);

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
            Sys_Instance.Instance.InstanceDataReq(Sys_Instance.ManyInstanceID);

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

            Sys_Instance.ServerInstanceData data = Sys_Instance.Instance.getMultiInstance();

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


                bool isSelectedReward = data == null ? false : (copyItem.id == data.instanceCommonData.SelectedInstanceId);

                if (i == 0 && isSelectedReward && data.instanceCommonData.Entries.Count <= 1)
                {
                    m_Layout.SetRewardTexActive(i,false);
                }
                else
                    m_Layout.SetRewardTexActive(i, true);

                if (isLock)
                {
                    uint level = copyItem.lv[0];

                    m_Layout.SetCharpterLockLevel(i, (int)level, (uint)(Sys_Role.Instance.Role.Level >= level ? 2009606 : 2009605) );

                    var preitem = CSVInstance.Instance.GetConfData(copyItem.pre_instance);

                    bool islastfini = false;

                    if (preitem != null)
                    {
                        var NetPredata = data == null ? null : data.GetInsEntry(copyItem.id);

                        if (NetPredata != null)
                        {
                            islastfini = !NetPredata.Unlock;
                        }
                    }


                    m_Layout.SetCharpterLockLastLeve(i,preitem == null ? 0:preitem.Name, (uint)(islastfini ? 2009604 : 2009603));
                }

                m_Layout.setCharpterRewardState(i, isSelectedReward);

                if (isSelectedReward)
                {
                    int cur = 0;
                    int total = 0;
                    Sys_Instance.Instance.getRewardProcess(copyItem.id, out cur, out total);
                    m_Layout.SetCharpterRewardProc(i, cur, total);
                }
                   

                m_Layout.SetCharpterBg(i,copyItem.bg);
            }


            uint selectID = data.instanceCommonData.SelectedInstanceId;

            var instanceData = CSVInstance.Instance.GetConfData(selectID);

            //uint selsectStageID =  Sys_Instance.Instance.getMultiStateID(selectID);

            //var stageData = CSVInstanceDaily.Instance.GetConfData(selsectStageID);

            bool bHideReward = data.instanceCommonData.Entries.Count <= 1;//|| instanceData == null || instanceData.pre_instance  == 0;

            m_Layout.SetBtnRewardActive(!bHideReward);
        }

        //刷新传记列表
        private void RefreshSeriesInfo()
        {
           var data=  CSVBiographySeries.Instance.GetAll();

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

            m_Layout.SetCardInfoCount(data.Count);

            int count = mSortedSeriesList.Count;

            for (int i = 0; i < count; i++)
            {
                m_Layout.setCardIcon(i,mSortedSeriesList[i].icon);
                m_Layout.setCardIndex(i, i);
               
            }

            OnClickChardInfo(0);
            m_Layout.setCardSelect(0, true);

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

            Sys_Instance.ServerInstanceData data = Sys_Instance.Instance.getMultiInstance();


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

            UIManager.OpenUI(EUIID.UI_Multi_Info,false, charpterID);
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
    }
}
