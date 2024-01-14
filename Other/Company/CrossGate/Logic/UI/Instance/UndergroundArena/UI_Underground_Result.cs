using Logic.Core;
using Packet;
using Table;
namespace Logic
{
    public class UI_Underground_Result_Parma
    {
        public uint InstanceID { get; set; } = 0;

        public uint StageID { get; set; } = 0;
        public CmdUnderGroundStageResultNty info { get; set; }
    }

    public class UI_Underground_Result : UIBase, UI_Underground_Result_Layout.IListener
    {
        private UI_Underground_Result_Layout m_Layout = new UI_Underground_Result_Layout();

        private uint m_SelectInstance = 0;
        private uint m_SelectStage = 0;

        private uint m_MonsterID = 0;

        UI_Underground_Result_Parma m_Parma = null;

        UnderGroundFormationStage m_StageInfo;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            m_Parma = arg as UI_Underground_Result_Parma;
            if (m_Parma != null)
            {
                m_SelectInstance = m_Parma.InstanceID;
                m_SelectStage = m_Parma.StageID;


                var formation = Sys_Instance_UGA.Instance.GetMonsterFormation();

                m_StageInfo = formation.Stages.Find(o => o.Stageid == m_SelectStage);

                if (m_StageInfo != null)
                {
                   var result = GetMonsterCaption(m_StageInfo);

                    m_MonsterID = result.Monsterid;
                }
                    
            }
        }
        protected override void OnShow()
        {
            OnRefresh();
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

        private void OnRefresh()
        {
            var instancedata = CSVInstance.Instance.GetConfData(m_SelectInstance);

            TextHelper.SetText(m_Layout.m_TexTitle, instancedata.Name, Sys_Instance_UGA.Instance.Num.ToString());

            var stagedata = CSVInstanceDaily.Instance.GetConfData(m_SelectStage);
            TextHelper.SetText(m_Layout.m_TexStage, stagedata.Name);


            var hero = GameCenter.mainHero;
            var headid = CharacterHelper.getHeadID(hero.heroBaseComponent.HeroID, hero.heroBaseComponent.HeadId);

            ImageHelper.SetIcon(m_Layout.m_ImgOwnIcon, headid);
            TextHelper.SetText(m_Layout.m_TexOwnName, 14003, Sys_Role.Instance.Role.Name.ToStringUtf8());

            var monsterdata = CSVMonster.Instance.GetConfData(m_MonsterID);
            var monsterpvpdata = CSVMonsterPvp.Instance.GetConfData(m_MonsterID);

            ImageHelper.SetIcon(m_Layout.m_ImgOtherIcon, monsterpvpdata.icon);
            TextHelper.SetText(m_Layout.m_TexOtherName, LanguageHelper.GetTextContent(14003, LanguageHelper.GetTextContent(monsterdata.monster_name)));

            m_Layout.m_BtnOwn.interactable = m_Parma.info.Win;
            m_Layout.m_BtnOther.interactable = !m_Parma.info.Win;

            int normacount = m_Parma.info.Normal.Count;
            int fristcount = m_Parma.info.First.Count;

            if (m_Parma.info.Win == false)
            {
                m_Layout.m_TransFristAward.gameObject.SetActive(false);
                m_Layout.m_TexNoRewardInfo.gameObject.SetActive(true);
                m_Layout.m_TransNormalAward.gameObject.SetActive(false);
                TextHelper.SetText(m_Layout.m_TexNoRewardInfo, 14034);
            }

            else if (normacount == 0 && fristcount == 0)
            {
                m_Layout.m_TransFristAward.gameObject.SetActive(false);
                m_Layout.m_TexNoRewardInfo.gameObject.SetActive(true);
                m_Layout.m_TransNormalAward.gameObject.SetActive(false);
                TextHelper.SetText(m_Layout.m_TexNoRewardInfo, 14033);
            }
               
            else if (fristcount > 0)
            {
                m_Layout.m_TransFristAward.gameObject.SetActive(true);
                m_Layout.m_TexNoRewardInfo.gameObject.SetActive(false);
                m_Layout.m_TransNormalAward.gameObject.SetActive(false);

                m_Layout.m_FristRewareGroup.SetChildSize(fristcount);
                m_Layout.m_FristNormalRewareGroup.SetChildSize(normacount);

                for (int i = 0; i < fristcount; i++)
                {
                   var item =  m_Layout.m_FristRewareGroup.getAt(i);

                    if (item != null)
                    {
                        item.SetItem(m_Parma.info.First[i].Itemtid, m_Parma.info.First[i].Num);
                    }
                }

                for (int i = 0; i < normacount; i++)
                {
                    var item = m_Layout.m_FristNormalRewareGroup.getAt(i);

                    if (item != null)
                    {
                        item.SetItem(m_Parma.info.Normal[i].Itemtid, m_Parma.info.Normal[i].Num);
                    }
                }
            }
            else if (normacount > 0)
            {
                m_Layout.m_TransFristAward.gameObject.SetActive(false);
                m_Layout.m_TexNoRewardInfo.gameObject.SetActive(false);
                m_Layout.m_TransNormalAward.gameObject.SetActive(true);

                m_Layout.m_NormalRewareGroup.SetChildSize(normacount);
               
                for (int i = 0; i < normacount; i++)
                {
                    var item = m_Layout.m_NormalRewareGroup.getAt(i);

                    if (item != null)
                    {
                        item.SetItem(m_Parma.info.Normal[i].Itemtid, m_Parma.info.Normal[i].Num);
                    }
                }
            }


        }
       

        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}