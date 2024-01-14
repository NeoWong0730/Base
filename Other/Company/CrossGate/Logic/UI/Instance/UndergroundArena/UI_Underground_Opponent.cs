using Logic.Core;
using Packet;
using Table;
using System.Collections.Generic;
namespace Logic
{
    public class UI_Underground_Opponent_Parma
    {
        public uint stageID = 0;

        public bool EnterWar = true;
    }
    public class UI_Underground_Opponent : UIBase, UI_Underground_Opponent_Layout.IListener
    {
        private UI_Underground_Opponent_Layout m_Layout = new UI_Underground_Opponent_Layout();

        private uint m_StageID = 0;
        private bool m_EnterWarState = false;
        UnderGroundFormationStage m_StageInfo;

        class Opponent
        {
            public int Index = 0;
  
            public class Data
            {
                public uint ID = 0;
                public int DataIndex = -1;
            }

            public Data[] Monster = new Data[2] { new Data(),new Data () };

        }

        Dictionary<uint, Opponent> m_DicOpponent = new Dictionary<uint, Opponent>();
        
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            var parma = arg as UI_Underground_Opponent_Parma;
            if (parma != null)
            {
                m_StageID = parma.stageID;
                m_EnterWarState = parma.EnterWar;

                var formation = Sys_Instance_UGA.Instance.GetMonsterFormation();

                m_StageInfo = formation.Stages.Find(o => o.Stageid == m_StageID);

                GenerateFormation();
            }
            else
            {
                m_EnterWarState = true;
                m_StageID = Sys_Instance.Instance.curInstance.StageID;

                var formation = Sys_Instance_UGA.Instance.GetMonsterFormation();

                m_StageInfo = formation.Stages.Find(o => o.Stageid == m_StageID);

                GenerateFormation();
            }
                
        }
        protected override void OnShow()
        {
            OnRefresh();
        }


        private void OnRefresh()
        {
            m_Layout.m_CardInfinity.CellCount = m_DicOpponent.Count;
            m_Layout.m_CardInfinity.ForceRefreshActiveCell();
            m_Layout.m_CardInfinity.MoveToIndex(0);

            m_Layout.m_BtnGo.gameObject.SetActive(m_EnterWarState);
            m_Layout.m_BtnVideo.gameObject.SetActive(false);
        }

        private void GenerateFormation()
        {
            int count = m_StageInfo.Units.Count;

            for (int i = 0; i < count; i++)
            {
               var data = CSVMonsterPvp.Instance.GetConfData(m_StageInfo.Units[i].Monsterid);

                uint index = data.position;
                if (data.type == 2)
                    index = index - 5;

                if (m_DicOpponent.ContainsKey(index) == false)
                    m_DicOpponent.Add(index, new Opponent() { Index = (int)index});

                if (data.type <= 2)
                {
                    int dataindex = (int)data.type - 1;

                    m_DicOpponent[index].Monster[dataindex].ID = m_StageInfo.Units[i].Monsterid;
                    m_DicOpponent[index].Monster[dataindex].DataIndex = i;
                }



            }
        }

        private Opponent GetItemData(uint order)
        {
            m_DicOpponent.TryGetValue(order, out Opponent data);

            return data;
        }
        public void OnInfinityChange(InfinityGridCell cell, int index)
        {

           var data =  GetItemData((uint)(index + 1));

            if (data == null)
                return;

            var fristdata = CSVMonsterPvp.Instance.GetConfData(data.Monster[0].ID);
            var fristmonstdata = CSVMonster.Instance.GetConfData(fristdata.id);

            var card = cell.mUserData as UI_Underground_Opponent_Layout.InfoCard;
            ImageHelper.SetIcon(card.m_ImgHead, fristdata.icon);
            TextHelper.SetText(card.m_TexLevel, "Lv."+ fristmonstdata.level.ToString());
            TextHelper.SetText(card.m_TexProfession, OccupationHelper.GetTextID(fristdata.career_show));
            ImageHelper.SetIcon(card.m_ImgProfession, OccupationHelper.GetCareerLogoIcon(fristdata.career_show));


            var seconddata = CSVMonsterPvp.Instance.GetConfData(data.Monster[1].ID);
            if (seconddata != null)
            {
                var sceondmonstdata = CSVMonster.Instance.GetConfData(seconddata == null ? 0 : seconddata.id);
                ImageHelper.SetIcon(card.m_ImgPetHead, seconddata.icon);
                TextHelper.SetText(card.m_TexPetLevel, "Lv." + sceondmonstdata.level.ToString());
            }


        }


        public void OnInfinityCreate(InfinityGridCell cell)
        {
            UI_Underground_Opponent_Layout.InfoCard card = new UI_Underground_Opponent_Layout.InfoCard();

            card.Load(cell.mRootTransform);

            cell.BindUserData(card);
        }

        public void OnClickVideo()
        {
            CloseSelf();
        }

        public void OnClickGo()
        {
            Sys_Instance_UGA.Instance.SendStartFightInInstance();

            CloseSelf();
        }

        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}