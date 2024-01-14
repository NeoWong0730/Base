using Logic.Core;
using Table;
namespace Logic
{
    public class UI_Underground_Reward : UIBase, UI_Underground_Reward_Layout.IListener
    {
        private UI_Underground_Reward_Layout m_Layout = new UI_Underground_Reward_Layout();

        private uint m_SelectInstance = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnShow()
        {
            m_SelectInstance = Sys_Instance_UGA.Instance.CurInstance;
            OnRefresh();
        }

        private void OnRefresh()
        {
            var data = CSVInstance.Instance.GetConfData(m_SelectInstance);

            var fristPassReward = CSVDrop.Instance.GetDropItem(data.FirstPassReward);
            var fristReward = CSVDrop.Instance.GetDropItem(data.FirstReward);

            int fprcount = fristPassReward.Count;
            m_Layout.m_FristGroup.SetChildSize(fprcount);
            for (int i = 0; i < fprcount; i++)
            {
                var item = m_Layout.m_FristGroup.getAt(i);
                item.m_ItemData.bShowCount = false;
                item.SetItem(fristPassReward[i].id, (uint)fristPassReward[i].count);
                item.m_SourceUiId = EUIID.UI_UndergroundArena_Opponent;
            }

            int frcount = fristReward.Count;
            m_Layout.m_SingleGroup.SetChildSize(frcount);

            for (int i = 0; i < frcount; i++)
            {
                var item = m_Layout.m_SingleGroup.getAt(i);
                item.m_ItemData.bShowCount = false;
                item.SetItem(fristReward[i].id, (uint)fristReward[i].count);
                item.m_SourceUiId = EUIID.UI_UndergroundArena_Opponent;
            }

            m_Layout.m_TexTitle.text = LanguageHelper.GetTextContent(14004, LanguageHelper.GetTextContent(data.DifficultyName));
            m_Layout.m_TexDes.text = LanguageHelper.GetTextContent(14010, data.lv[0].ToString(), data.lv[1].ToString(), (data.lv[1]+1).ToString());
            m_Layout.m_TexRewardName.text = LanguageHelper.GetTextContent(14005, LanguageHelper.GetTextContent(data.DifficultyName));



        }


        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}