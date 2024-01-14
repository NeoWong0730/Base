using Logic.Core;
using Table;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Packet;

namespace Logic
{
    public class UI_UndergroundArena : UIBase,UI_UndergroundArena_Layout.IListener
    {
        private UI_UndergroundArena_Layout m_Layout = new UI_UndergroundArena_Layout();

        private uint m_SelectInstance = 0;
        private List<CSVInstanceDaily.Data> LevelDatas;

        private int MaxStageIndex = 0;

        private Timer m_ScorllMoveTimer = null;

        private int m_FocusIndex = 0;

        private bool m_IsCross = false;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance_UGA.Instance.eventEmitter.Handle(Sys_Instance_UGA.EEvents.InfoRefresh, OnInfoRefresh, toRegister);
            Sys_Instance_UGA.Instance.eventEmitter.Handle(Sys_Instance_UGA.EEvents.InstanceRefresh, OnInstanceInfoRefresh, toRegister);
        }
        protected override void OnShow()
        {
            Refresh(true);
        }


        protected override void OnClose()
        {
            m_ScorllMoveTimer?.Cancel();
        }

        private void OnInfoRefresh()
        {
            Refresh(false);
        }

        private void OnInstanceInfoRefresh()
        {
            Refresh(true);
        }
        private void Refresh(bool facus)
        {
            m_SelectInstance = Sys_Instance_UGA.Instance.CurInstance;

            LevelDatas = Sys_Instance.Instance.getDailyByInstanceID(m_SelectInstance);

            if (Sys_Instance_UGA.Instance.HadShowTips(m_SelectInstance) == false)
            {

                UIManager.OpenUI(EUIID.UI_UndergroundArena_Reward);

                Sys_Instance_UGA.Instance.SendSetFristTips(m_SelectInstance);
            }
            OnRefresh(facus);
        }
        private void OnRefresh(bool focus)
        {
            var data = CSVInstance.Instance.GetConfData(m_SelectInstance);

            m_Layout.m_TexName.text = LanguageHelper.GetTextContent(data.Name,Sys_Instance_UGA.Instance.Num.ToString());

            m_Layout.m_TexTime.text =  GetTimeString();

            var instancecommondata = Sys_Instance_UGA.Instance.serverInstanceData.instanceCommonData;

            var entriesdata = instancecommondata.Entries.Find(o => o.InstanceId == Sys_Instance_UGA.Instance.CurInstance);

            int realcount = 1;

            string recodestr = string.Empty;
            if (entriesdata != null )
            {
               var index = LevelDatas.FindIndex(o => o.id == entriesdata.PerMaxStageId);

                MaxStageIndex = index;

                if (index < 0)
                    index = -1;

                m_FocusIndex =  index + 1;

                realcount = index + 2;

                recodestr = (index + 1) >= (LevelDatas.Count) ? (LanguageHelper.GetTextContent(14002)) : 
                    LanguageHelper.GetTextContent(14001, LanguageHelper.GetTextContent(LevelDatas[index + 1].Name));
                
				m_IsCross = (index + 1) >= (LevelDatas.Count);
			}

            m_Layout.m_TexMaxStage.text = recodestr;

            m_Layout.m_CardInfinity.CellCount = Mathf.Min( realcount, LevelDatas.Count);
            m_Layout.m_CardInfinity.ForceRefreshActiveCell();
            m_Layout.m_CardInfinity.MoveToIndex(0);

            m_Layout.m_TransScoreTips.gameObject.SetActive(!m_IsCross);

            if (!m_IsCross)
            {
                var leveldata = LevelDatas[m_FocusIndex];
                uint score = leveldata.Score;
                uint rolepower = (uint)Sys_Attr.Instance.rolePower;
                uint styleid = rolepower >= score ? 74u : 156u;

                var styledata = CSVWordStyle.Instance.GetConfData(styleid);
                TextHelper.SetText(m_Layout.m_TexScore, score.ToString(), styledata);
            }


            if (focus)
                MoveToFoucs();
        }


        

        public void SetRewardFocusIndex(int index)
        {
            if (index < 0)
                index = 0;

            var rect = m_Layout.m_CardInfinity.Content;

            var cellsize = m_Layout.m_CardInfinity.CellSize;

            var space = m_Layout.m_CardInfinity.Spacing;

            var size = cellsize.x * (index + 1) + space.x * index;

            if (size < m_Layout.m_CardInfinity.ScrollView.viewport.rect.width)
                return;

            float offsetx = cellsize.x * (index) + space.x * index;


            float xpos = m_Layout.m_CardInfinity.ScrollView.viewport.sizeDelta.x * rect.anchorMin.x - offsetx;

            float maxpos = m_Layout.m_CardInfinity.ScrollView.viewport.sizeDelta.x * rect.anchorMin.x -
                (rect.sizeDelta.x - m_Layout.m_CardInfinity.ScrollView.viewport.sizeDelta.x);

            float minpos = m_Layout.m_CardInfinity.ScrollView.viewport.sizeDelta.x * (-rect.anchorMin.x);

            if (Mathf.Abs(minpos) > Mathf.Abs(xpos))
                xpos = minpos;
            else if (Mathf.Abs(xpos) > Mathf.Abs(maxpos))
                xpos = maxpos;

            m_Layout.m_CardInfinity.Content.anchoredPosition = new Vector2(xpos, rect.anchoredPosition.y);
        }

        private void MoveToFoucs()
        {
            m_ScorllMoveTimer?.Cancel();
            m_ScorllMoveTimer = Timer.Register(0.2f, () => {

               
                SetRewardFocusIndex(m_FocusIndex);

            });

        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            m_Layout.m_TexTime.text = GetTimeString();
        }

        private string GetTimeString()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            string strtime = string.Empty;

            if (nowtime >= Sys_Instance_UGA.Instance.EndTime)
            {
                return LanguageHelper.GetTextContent(14060u);
            }
                

            var offsettime = Sys_Instance_UGA.Instance.EndTime - nowtime;

            var day = offsettime / 86400;
            var dayofsset = offsettime % 86400;

            var hour = dayofsset / 3600;
            var houroffset = dayofsset % 3600;

            var minu = houroffset / 60;
            var sec = houroffset % 60;

           
            if (day > 0)
                strtime += (day.ToString() + LanguageHelper.GetTextContent(4604));

            if (day > 0 || hour > 0)
            {
                string strhour = hour.ToString();

                if (hour < 10)
                    strhour = "0" + strhour;

                strtime += (strhour + LanguageHelper.GetTextContent(4605));
            }

            if (day > 0 || hour > 0 || minu > 0)
            {
                string strmin = minu.ToString();

                if (minu < 10)
                    strmin = "0" + strmin;

                strtime += (strmin + LanguageHelper.GetTextContent(4606));
            }

            if (day == 0 && hour == 0 && minu == 0 && sec > 0)
            {
                string strsec = sec.ToString();

                if (sec < 10)
                    strsec = "0" + strsec;

                strtime += (strsec + LanguageHelper.GetTextContent(11994));
            }

             return LanguageHelper.GetTextContent(14038, strtime);
        }
        public void OnClickClose()
        {
            CloseSelf();
        }
        public void OnClickFastTeam()
        {
            var data = CSVInstance.Instance.GetConfData(m_SelectInstance);

            if (data == null)
                return;

            Sys_Team.Instance.OpenFastUI(data.TeamID);
        }

        public void OnClickGoTo()
        {
            if (Sys_Instance.Instance.CheckTeamCondition(m_SelectInstance) == false)
                return;

            Sys_Instance_UGA.Instance.SendStartVote();

            //UI_Underground_Vote_Parmas parmas = new UI_Underground_Vote_Parmas();
            //parmas.InstanceID = m_SelectInstance;
            //UIManager.OpenUI(EUIID.UI_UndergroundArena_Vote,false,parmas);
        }

        public void OnClickRank()
        {
            UIManager.OpenUI(EUIID.UI_UndergroundArena_Rank);
        }

        public void OnClickReward()
        {
            UIManager.OpenUI(EUIID.UI_UndergroundArena_Reward);
        }

        public void OnInfinityChange(InfinityGridCell cell, int index)
        {
            var info = cell.mUserData as UI_UndergroundArena_Layout.VsInfoCard;

            TextHelper.SetText(info.m_TexCardName, LevelDatas[index].Name);

            var hero = GameCenter.mainHero;
            var headid = CharacterHelper.getHeadID(hero.heroBaseComponent.HeroID, hero.heroBaseComponent.HeadId);
            var headframeid =CharacterHelper.getHeadFrameID(hero.heroBaseComponent.HeadFrameId);



            var monsterinfo = Sys_Instance_UGA.Instance.GetMonsterFormation();
            var monsterunit = monsterinfo.Stages[index];

            var monsterfrist = GetMonsterCaption(monsterunit);

            var monsterdata = CSVMonster.Instance.GetConfData(monsterfrist.Monsterid);
            var monsterpvpdata = CSVMonsterPvp.Instance.GetConfData(monsterfrist.Monsterid);


            ImageHelper.SetIcon(monsterunit.Side ? info.m_ImgOwnHead : info.m_ImgOtherHead, headid);
            TextHelper.SetText(monsterunit.Side ? info.m_TexOwnName : info.m_TexOtherName, LanguageHelper.GetTextContent(14003, hero.heroBaseComponent.Name));

            ImageHelper.SetIcon(monsterunit.Side == false ? info.m_ImgOwnHead : info.m_ImgOtherHead, monsterpvpdata.icon);
            TextHelper.SetText(monsterunit.Side == false ? info.m_TexOwnName : info.m_TexOtherName, LanguageHelper.GetTextContent(14003, LanguageHelper.GetTextContent(monsterdata.monster_name)));

            info.m_BtnOwnIcon.enabled = true;
            info.m_BtnOtherIcon.enabled = true;

            if (index <= MaxStageIndex)
            {
                info.m_BtnOwnIcon.interactable = monsterunit.Side == false ? false : true;
                info.m_BtnOtherIcon.interactable = monsterunit.Side ? false : true;
            }
            else
            {
                info.m_BtnOwnIcon.interactable = true;
                info.m_BtnOtherIcon.interactable = true;
            }
           

            info.ID = monsterunit.Stageid;


            info.m_TransVS.gameObject.SetActive(index > MaxStageIndex );
            info.m_TransKO.gameObject.SetActive(index <= MaxStageIndex);
        }

        private UnderGroundFormationUnit GetMonsterCaption(UnderGroundFormationStage stage)
        {
            int count = stage.Units.Count;

            for (int i = 0; i < count; i++)
            {
                var monsterpvpdata = CSVMonsterPvp.Instance.GetConfData(stage.Units[i].Monsterid);

                if (monsterpvpdata.team_sign_type)
                {
                    return stage.Units[i];
                }
            }

            return null;
        }

        public void OnInfinityCreate(InfinityGridCell cell)
        {
            UI_UndergroundArena_Layout.VsInfoCard info = new UI_UndergroundArena_Layout.VsInfoCard();

            
            info.Load(cell.mRootTransform);

            info.ClickOtherAc = OnClickLowSideIcon;
            info.ClickOwnAc = OnClickUpSideIcon;

            cell.BindUserData(info);
        }

        public void OnClickLowSideIcon(uint id)
        {
            var monsterinfo = Sys_Instance_UGA.Instance.GetMonsterFormation();

            var result =  monsterinfo.Stages.Find(o => o.Stageid == id);

            if (result == null /*|| result.Side ==false*/)
                return;



            UI_Underground_Opponent_Parma parma = new UI_Underground_Opponent_Parma() { stageID = id ,EnterWar = false};

            UIManager.OpenUI(EUIID.UI_UndergroundArena_Opponent,false,parma);
        }

        public void OnClickUpSideIcon(uint id)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(14061u));
        }
    }
}