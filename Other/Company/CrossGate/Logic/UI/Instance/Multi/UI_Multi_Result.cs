using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Multi_Result : UIBase, UI_Multi_Result_Layout.IListener
    {
        private UI_Multi_Result_Layout m_Layout = new UI_Multi_Result_Layout();

        private int mMode;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_Instance.Instance.eventEmitter.Handle<uint>(Sys_Instance.EEvents.RewardRefresh, RewardNotify,toRegister);
        }
        protected override void OnOpen(object arg)// 传入参数 0 通关 1 胜利 2 失败
        {            
            mMode = (int)arg;
        }
        protected override void OnShow()
        {         
            m_Layout.ShowMode(mMode);

            m_Layout.SetActive(true);

            RefreshInfo();
        }

        private void RefreshInfo()
        {
            var data = CSVInstance.Instance.GetConfData(Sys_Instance.Instance.curInstance.InstanceId);
            if (data == null)
                return;

            m_Layout.SetDesc(LanguageHelper.GetTextContent(data.Des));
            m_Layout.SetName(LanguageHelper.GetTextContent(data.Name));
        }
        protected override void OnHide()
        {            
            m_Layout.SetActive(false);
        
        }

        public void Close()
        {
            CloseSelf();

            if (Sys_Team.Instance.isCaptain() && Sys_Instance.Instance.IsInInstance)
                Sys_Instance.Instance.InstanceExitReq();
        }

    }
}
