using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_Info_Right
    {
        private Transform transform;

        private Text m_textType;
        private Text m_textLevel;
        private Text m_textTime;
        private Text m_textContent;
        private Text m_textOpenTime;
        private Text m_textCountDown;
        private Button m_btnGo;

        private GameObject goPropTemplate;

        private uint m_ActivityId;
        private CSVDailyActivity.Data m_ActivityData;

        public void Init(Transform trans)
        {
            transform = trans;

            m_textType = transform.Find("Image/Text_Type/Text").GetComponent<Text>();
            m_textLevel = transform.Find("Text_LV/Text").GetComponent<Text>();
            m_textTime = transform.Find("Text_Time/Text").GetComponent<Text>();
            m_textContent = transform.Find("Image2/Text_Content").GetComponent<Text>();
            m_textOpenTime = transform.Find("Image3/Text_Open (1)").GetComponent<Text>();
            m_textCountDown = transform.Find("Text_CountDown/Text").GetComponent<Text>();

            goPropTemplate = transform.Find("Award/Scroll_View/Viewport/Item").gameObject;
            goPropTemplate.SetActive(false);

            m_btnGo = transform.Find("Btn_01").GetComponent<Button>();
            m_btnGo.onClick.AddListener(OnClickGo);
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnClickGo()
        {
            UIManager.CloseUI(EUIID.UI_FamilyBoss_Info);
            UIManager.CloseUI(EUIID.UI_DailyActivites);
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(m_ActivityData.Npcid);
        }

        public void UpdateInfo(uint activityId)
        {
            m_ActivityId = activityId;
            m_ActivityData = CSVDailyActivity.Instance.GetConfData(m_ActivityId);

            m_textType.text = LanguageHelper.GetTextContent(m_ActivityData.WayDesc);
            m_textLevel.text = m_ActivityData.OpeningLevel.ToString();

            m_textTime.text = LanguageHelper.TimeToString(m_ActivityData.Duration, LanguageHelper.TimeFormat.Type_6);

            int hours = m_ActivityData.OpeningTime[0][0];
            int minute = m_ActivityData.OpeningTime[0][1];
            string str = ((hours == 0 ? "00" : hours.ToString()) + ":" + (minute == 0 ? "00" : minute.ToString()));
            m_textOpenTime.text = str;

            m_textContent.text = LanguageHelper.GetTextContent(m_ActivityData.ActiveDes);

            Lib.Core.FrameworkTool.DestroyChildren(goPropTemplate.transform.parent.gameObject, goPropTemplate.name);
            List<ItemIdCount> lists = CSVDrop.Instance.GetDropItem(m_ActivityData.Reward[0]);
            for (int i = 0; i < lists.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(goPropTemplate, goPropTemplate.transform.parent);
                go.SetActive(true);
                PropItem prop = new PropItem();
                prop.BindGameObject(go);

                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(lists[i].id, lists[i].count, true, false, false, false, false);
                prop.SetData(itemData, EUIID.UI_FamilyBoss_Info);
            }

        }
    }
}