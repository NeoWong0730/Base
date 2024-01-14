using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_ExpTips_Layout
    {
        public Transform transform;
        public Text worldLv;
        public Text expMultiple;
        public Text nextLvDays;
        public Text tips;
        public Text tipsLevelLimit;
        public Text tipsNextLvDays;
        public Text tipsAdvanceTime;
        public GameObject viewRoot;
        public GameObject nextLevelOpenDayRoot;
        public GameObject advanceRoot;
        public Button closeBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            worldLv = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Sever/Text_Num").GetComponent<Text>();
            expMultiple = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Exp/Text_Num").GetComponent<Text>();
            nextLvDays = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Level/Text_Title").GetComponent<Text>();
            viewRoot = transform.Find("Animator/Background_Root/Scroll_View/View").gameObject;
            nextLevelOpenDayRoot = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Level").gameObject;
            advanceRoot = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Promote").gameObject;
            tips = transform.Find("Animator/Background_Root/Scroll_View/View/Text_Tips").GetComponent<Text>();
            tipsLevelLimit= transform.Find("Animator/Background_Root/Scroll_View/View/Text_Tips_LevelLimit").GetComponent<Text>();
            tipsAdvanceTime = transform.Find("Animator/Background_Root/Scroll_View/View/Image_Title_Promote/Text_Title").GetComponent<Text>();
            closeBtn = transform.Find("Animator/Button_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }
    }

    public class UI_ExpTips : UIBase,UI_ExpTips_Layout.IListener
    {
        private UI_ExpTips_Layout layout = new UI_ExpTips_Layout();
        private  CSVWorldLevel.Data openServiceDay;
        private Timer timer;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            SetData();
            ForceRebuildLayout(layout.viewRoot.transform.gameObject);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {         
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateOpenServiceDay, OnUpdateOpenServiceDay, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }

        protected override void OnHide()
        {
            timer?.Cancel();
        }

        private void SetData()
        {
            layout.tipsLevelLimit.text = LanguageHelper.GetTextContent(4328);
            uint num = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).WelfareExp;
            layout.worldLv.text = Sys_Role.Instance.GetWorldLv().ToString();
            if (CSVWorldLevel.Instance.TryGetValue(Sys_Role.Instance.openServiceDay,out openServiceDay) && openServiceDay != null)
            {
                SetNetLvDays();
            }
            else
            {
                layout.nextLevelOpenDayRoot.SetActive(false);
                layout.tips.gameObject.SetActive(false);
            }
            SetexpMultiple();
            SetAdvanceTime();
        }

        private void SetAdvanceTime()
        {
            uint curPromoteCareerId = Sys_Advance.Instance.SetAdvanceRank();
            CSVPromoteCareer.Data csvData = CSVPromoteCareer.Instance.GetConfData(curPromoteCareerId + 1);
            if (csvData == null || csvData.serverConditions == 0)
            {
                layout.advanceRoot.SetActive(false);
            }
            else
            {
                int days = csvData.serverConditions / 86400;
                uint canAdvanceTime = Sys_Role.Instance.openServiceGameTime + (uint)csvData.serverConditions;
                uint nowTime = Sys_Time.Instance.GetServerTime();
                if (canAdvanceTime <= nowTime)
                {
                    layout.advanceRoot.SetActive(false);
                }
                else
                {
                    uint leftTime = canAdvanceTime - nowTime;
                    layout.advanceRoot.SetActive(true);
                    timer?.Cancel();
                    timer = Timer.Register(leftTime, () =>
                    {
                        timer.Cancel();
                    },
                    (time) =>
                    {
                        uint cutDown = leftTime - (uint)time;
                        uint day = cutDown / (3600 * 24);
                        uint hour = cutDown % (3600 * 24) / 3600;
                        uint min = cutDown % 3600 / 60;
                        uint sec = cutDown % 60;
                        TextHelper.SetText(layout.tipsAdvanceTime, 2029414, (Sys_Role.Instance.Role.CareerRank + 1).ToString(), days.ToString(), day.ToString(), hour.ToString(), min.ToString(), sec.ToString());
                    }, false, false);
                }
            }
        }

        private void SetNetLvDays()
        {
            DateTime dtOpen = TimeManager.GetDateTime( Sys_Role.Instance.openServiceGameTime);
            DateTime dtOpenReal = new DateTime(dtOpen.Year, dtOpen.Month, dtOpen.Day, 5, 0, 0);
            DateTime dtNow = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());     
            TimeSpan tp = dtNow - dtOpenReal;
            if (Sys_Role.Instance.openServiceDay <= tp.Days)
            {
                layout.nextLevelOpenDayRoot.SetActive(false);
            }
            else
            {
                int days =(int) Sys_Role.Instance.openServiceDay - (int)tp.Days;
                layout.nextLevelOpenDayRoot.SetActive(true);
                layout.nextLvDays.text = LanguageHelper.GetTextContent(2007509, days.ToString());
            }
        } 

        private void SetexpMultiple()
        {
            float num = 0;
            num= Sys_Attr.Instance.GetExpMultiple();
            layout.expMultiple.text = LanguageHelper.GetTextContent(2009361, num.ToString());
            layout.tips.gameObject.SetActive(false);
            if (num != 100)
            {
                SetTips();
            }
        }

        private void SetTips()
        {
            if (openServiceDay.world_level<Sys_Attr.Instance.GetRealLv())
            {
                layout.tips.gameObject.SetActive(true);
                TextHelper.SetText(layout.tips, 2007511);
            }
            else if(openServiceDay.world_level > Sys_Attr.Instance.GetRealLv())
            {            
                int lv = 0;
                int.TryParse(CSVParam.Instance.GetConfData(261).str_value, out lv);
                if (Sys_Role.Instance.Role.Level >= lv)
                {
                    layout.tips.gameObject.SetActive(true);
                    TextHelper.SetText(layout.tips, 2007510);
                }
            }
        }

        private void OnUpdateOpenServiceDay()
        {
            SetData();
            ForceRebuildLayout(layout.viewRoot.transform.gameObject);
        }

        private void OnTimeNtf(uint arg1, uint arg2)
        {
            SetAdvanceTime();
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_ExpTips);
        }
    }
}
