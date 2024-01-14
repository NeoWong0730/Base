using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_Multi_Info : UIBase, UI_Multi_Info_Layout.IListener
    {
        private UI_Multi_Info_Layout m_Layout = new UI_Multi_Info_Layout();


        private uint CopyID = 0;//副本ID

        private uint ChapterID = 0;//章节ID 

        private List<CSBiographyChapter.Data> m_ChaptersList;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.TeamInstanceProgress, OnTeamInstaceProcess, toRegister);
        }
        protected override void OnOpen(object arg)
        {            
            CopyID = (uint)arg;
  
        }
        protected override void OnShow()
        {            
            RefreshChapterInfo();

            //if (m_Layout.getFocusInfo(0))
            //{
            //    OnClickInfo(0);
            //}
            //else
            //    m_Layout.FocusInfo(0);

            m_Layout.ShowGoOn(!Sys_Instance.Instance.IsInInstance);

          //  m_Layout.SetFastTeamActive(!Sys_Team.Instance.HaveTeam);

            RefreshTitle();


        }

        private void RefreshTitle()
        {
            var seriesData = CSVBiographySeries.Instance.GetConfData(Sys_Instance.Instance.CurSeriesID);
            if (seriesData != null)
            {
                string serisename = LanguageHelper.GetTextContent(seriesData.Name);

                var instanceData = CSVInstance.Instance.GetConfData(CopyID);

                string instancename = LanguageHelper.GetTextContent(instanceData.Name);

                m_Layout.SetTitle(serisename + " · " + instancename);
            }
        }
        //刷新章节列表
        private void RefreshChapterInfo()
        {
            if (m_ChaptersList != null)
                m_ChaptersList.Clear();

            var data = Sys_Instance.Instance.getChapter(CopyID);

            m_ChaptersList = data;

            int infoCount = data.Count;

            m_Layout.SetInfoCount(infoCount);

           var bestdilay = getPlayerBestHistoryDily(CopyID);
            var todayBestdilay = getPlayerTodayBestDily(CopyID);

            int fouceIndex = 0;

            for (int i = 0; i < infoCount; i++)
            {
                bool lockcharpter = data[i].Sort > bestdilay.LayerStage;

                if (data[i].Sort == todayBestdilay.LayerStage)
                    fouceIndex = i;

                string num = LanguageHelper.GetTextContent(data[i].Name);
                string text = lockcharpter ? LanguageHelper.GetTextContent(2009638) : LanguageHelper.GetTextContent(data[i].Des);

                m_Layout.SetInfo(i, num, text);
                m_Layout.SetInfoIndex(i, i);
                m_Layout.SetInteractableInfo(i, !lockcharpter);
            }

            if (m_Layout.getFocusInfo(fouceIndex) == false)
                m_Layout.FocusInfo(fouceIndex);
            else
                OnClickInfo(fouceIndex);


            Sys_Instance.ServerInstanceData sdata = Sys_Instance.Instance.getMultiInstance();

            bool isSelectedReward = sdata == null ? false : (CopyID == sdata.instanceCommonData.SelectedInstanceId);

            m_Layout.SetProcessState(isSelectedReward);

            if (isSelectedReward)
            {
                int cur = 0;
                int total = 0;
                Sys_Instance.Instance.getRewardProcess(CopyID,out cur,out total);
                m_Layout.SetProcess(cur,total);
            }
               
        }

        /// <summary>
        /// 获取玩家自己副本最好的成绩  关卡 记录
        /// </summary>
        /// <param name="instanceID"></param>
        private CSVInstanceDaily.Data getPlayerBestHistoryDily(uint instanceID)
        {
            var insdata = Sys_Instance.Instance.getMultiInstance();

            if (insdata != null)
            {
                var listdata = insdata.instanceCommonData.Entries;

                // 当前关卡在当前章节关卡列表的下标
                for (int i = 0; i < listdata.Count; i++)
                {
                    if (instanceID == listdata[i].InstanceId)
                    {
                        var id = listdata[i].HistoryStageId;

                        var daillist = Sys_Instance.Instance.getDailyByInstanceID(instanceID);

                        if (daillist.Count == 0)
                            return null;

                        int index = daillist.FindIndex(o => o.id == id);

                        index = Mathf.Max(0, index+1);

                        index = Mathf.Min(index, daillist.Count - 1);

                        var StateData = daillist[index];

                        return StateData;
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// 获取玩家自己副本当天最好的成绩 关卡 记录  周期会被置0 周期目前为 1 天
        /// </summary>
        /// <param name="instanceID"></param>
        private CSVInstanceDaily.Data getPlayerTodayBestDily(uint instanceID)
        {

            var bestdata = Sys_Instance.Instance.getMultiBestStateID(instanceID);

            var daillist = Sys_Instance.Instance.getDailyByInstanceID(instanceID);

            if (daillist.Count == 0)
                return null;

            int index = daillist.FindIndex(o => o.id == bestdata);

            index = Mathf.Max(0, index + 1);

            index = Mathf.Min(index, daillist.Count - 1);

            var StateData = daillist[index];

            return StateData;

        }

        /// <summary>
        /// 获取玩家当前所在副本关卡,在副本中使用
        /// </summary>
        private CSVInstanceDaily.Data getCurDaily()
        {
            if (Sys_Instance.Instance.isManyDungeons == false)
                return null;

            uint id = Sys_Instance.Instance.curInstance.StageID;

            var StateData = CSVInstanceDaily.Instance.GetConfData(id);

            return StateData;
        }

        Color32 mInfoNormalColor = new Color32(95,62,33,255);
        Color32 mInfoLockedColor = new Color32(253,10,10,255);

        //刷新关卡列表
        private void RefreshDailyInfo()
        {

            var data = Sys_Instance.Instance.getDaily(ChapterID);//副本下的所有关卡
            var datachapter = CSBiographyChapter.Instance.GetConfData(ChapterID);//章节

            int infoCount = data.Count;

            m_Layout.SetInfoChildCount(infoCount);


            var bestDilay = getPlayerBestHistoryDily(datachapter.InstanceID);
            var todayDilay = getPlayerTodayBestDily(datachapter.InstanceID);

            var allDilay = Sys_Instance.Instance.getDailyByInstanceID(datachapter.InstanceID);

            var maxDilay = allDilay.Count > 0 ? allDilay[allDilay.Count - 1] : null;//最高关卡

            var curDilay = getCurDaily();

            for (int i = 0; i < infoCount; i++)
            {
                string num = string.Format("{0}.{1}",data[i].LayerStage,data[i].Layerlevel);
                string text = LanguageHelper.GetTextContent(data[i].Name);

                int state =  ChildInfoStateInstance(data[i],todayDilay,bestDilay, maxDilay,curDilay);

                m_Layout.SetChildInfoState(i,state);//-2 隐藏信息条 -1 隐藏标记 0 锁 1 完成 2 现在

                Color color = state == 0 ? mInfoLockedColor : mInfoNormalColor;

                
                m_Layout.SetInfoChild(i, num, state == 0 ? LanguageHelper.GetTextContent(2009654):text ,color);

                m_Layout.SetInfoChildActive(i, state != -2);
            }


        }

        /// <summary>
        /// 标记关卡状态
        /// </summary>
        /// <param name="data">关卡</param>
        ///  <param name="todayData">今天过关关卡</param>
        /// <param name="bestData">达到最高关卡</param>
        /// <param name="maxData">最高关卡</param>
        /// <param name="curData">当前进入的关卡</param>
        /// 
        /// <returns></returns>
        private int  ChildInfoStateInstance( CSVInstanceDaily.Data data, CSVInstanceDaily.Data todayData, CSVInstanceDaily.Data bestData, CSVInstanceDaily.Data maxData, CSVInstanceDaily.Data curData)
        {
            uint layerStage = bestData == null ? 0 : bestData.LayerStage;
            uint layerlevel = bestData == null ? 0 : bestData.Layerlevel;

            int disStage = (int)layerStage - (int)data.LayerStage;
            int dislevel = (int)layerlevel - (int)data.Layerlevel;

            uint todaylayerStage = todayData == null ? 0 : todayData.LayerStage;
            uint todaylayerlevel = todayData == null ? 0 : todayData.Layerlevel;

            int todaydisStage = (int)todaylayerStage - (int)data.LayerStage;
            int todaydislevel = (int)todaylayerlevel - (int)data.Layerlevel;


            uint maxlayerStage = maxData == null ? 0 : maxData.LayerStage;
            uint maxlayerlevel = maxData == null ? 0 : maxData.Layerlevel;

            int maxdisStage = (int)maxlayerStage - (int)todaylayerStage;
            int maxdislevel = (int)maxlayerlevel - (int)todaylayerlevel;


            if (todaydisStage > 0)
                return 1;

            if (todaydisStage == 0 && todaydislevel > 0)
                return 1;

            if (curData == null && maxdislevel == 0 && maxdisStage == 0)
                return 1;

            if (curData == null && todaydisStage == 0 && todaydislevel == 0)
                return 2;

            if (curData != null && data.Layerlevel == curData.Layerlevel && data.LayerStage == curData.LayerStage)
                return 2;
          

            if (disStage > 0 || (disStage == 0 && dislevel >= 0)) // 小于未解锁 隐藏标记
                return -1;

            if (disStage < 0 || (disStage == 0 && dislevel < -1)) // 大于未解锁 隐藏信息条
                return -2;

            return 0;
        }

        public void Agine()
        {

        }
        public void GoOn()
        {
           bool result = Sys_Instance.Instance.CheckTeamCondition(CopyID);

            if (result == false)
                return;

            Sys_Instance.Instance.OnSendTeamMemsInstance(CopyID);

        }

        public void OnClickInfo(int index)
        {
            if (m_ChaptersList.Count <= index)
                return;

            var bestdilay = getPlayerBestHistoryDily(CopyID);

            var item = m_ChaptersList[index];

            if (item.Sort > bestdilay.LayerStage)
                return;

            ChapterID = item.id;

            RefreshDailyInfo();
        }

        public void Close()
        {
            CloseSelf();
        }

        public void OnClickFastTeam()
        {
            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI((uint)50);
        }

        private void OnTeamInstaceProcess()
        {
           bool resutl = Sys_Instance.Instance.DecisionToReady();

            if (resutl == false)
                return;


            Close();

            UIManager.OpenUI(EUIID.UI_Multi_Ready,false,1);
        }

    }
}
