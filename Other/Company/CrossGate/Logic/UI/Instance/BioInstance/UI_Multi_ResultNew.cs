using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Multi_ResultNew_Parma
    {
        public uint InstanceID { get; set; } = 0;

        // 传入参数 0 通关 1 胜利 2 失败
        public int Mode { get; set; } = 0;

        public bool isFristCorss { get; set; } = false;
    }
    public class UI_Multi_ResultNew : UIBase, UI_Multi_ResultNew_Layout.IListener
    {
        private UI_Multi_ResultNew_Layout m_Layout = new UI_Multi_ResultNew_Layout();

        private UI_Multi_ResultNew_Parma m_Parmas = new UI_Multi_ResultNew_Parma();

        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_Instance.Instance.eventEmitter.Handle<uint>(Sys_Instance.EEvents.RewardRefresh, RewardNotify,toRegister);
        }
        protected override void OnOpen(object arg)
        {
            var parma = arg as UI_Multi_ResultNew_Parma;

            if (parma != null)
                m_Parmas = parma;
        }
        protected override void OnShow()
        {            
            m_Layout.ShowMode(m_Parmas.Mode);

            m_Layout.SetActive(true);

            RefreshInfo();
        }

        private void RefreshInfo()
        {
            var data = CSVInstance.Instance.GetConfData(m_Parmas.InstanceID);
            if (data == null)
                return;

            m_Layout.SetDesc(LanguageHelper.GetTextContent(data.Des));
            m_Layout.SetName(LanguageHelper.GetTextContent(data.Name));


            RefreshInstancePass();


        }

        private void RefreshInstancePass()
        {
            if (m_Parmas.Mode != 0)
                return;

            m_Layout.SetFristCross(m_Parmas.isFristCorss);

            if (m_Parmas.isFristCorss)
            {
                int count = Sys_Instance_Bio.Instance.EndReward.Items.Count;

                m_Layout.SetFristRewardCount(count);


                for (int i = 0; i < count; i++)
                {
                    m_Layout.SetFristReward(i, Sys_Instance_Bio.Instance.EndReward.Items[i].InfoId, Sys_Instance_Bio.Instance.EndReward.Items[i].Count,Sys_Instance_Bio.Instance.EndReward.Items[i].Item_);
                }
            }
        }
        protected override void OnHide()
        {            
            m_Layout.SetActive(false);

            CloseSelf();
        
        }

        public void Close()
        {
            CloseSelf();

            if (Sys_Team.Instance.isCaptain() && Sys_Instance.Instance.IsInInstance)
                Sys_Instance.Instance.InstanceExitReq();
        }

    }
}
