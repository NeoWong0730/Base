using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    /// <summary> 手机绑定 </summary>
    public class UI_PhoneBindGift : UI_OperationalActivityBase
    {
        private class MessageInfo
        {
            private Transform transform;

            private Text m_textRoleName;
            private Text m_textRoleId;
            private Text m_textServerName;

            public void Init(Transform trans)
            {
                transform = trans;

                m_textRoleName = transform.Find("Name").GetComponent<Text>();
                m_textRoleId = transform.Find("Server").GetComponent<Text>();
                m_textServerName = transform.Find("Id").GetComponent<Text>();
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo()
            {
                m_textRoleName.text = LanguageHelper.GetTextContent(2024201, Sys_Role.Instance.sRoleName);
                m_textServerName.text = LanguageHelper.GetTextContent(2024203, Sys_Login.Instance.mSelectedServer?.mServerInfo.ServerName);
                m_textRoleId.text = LanguageHelper.GetTextContent(2024202, Sys_Role.Instance.RoleId.ToString());
            }
        }

        private class BindingInfo
        {
            private Transform transform;

            //private InputField m_inputPhoneNumber;
            //private Button m_btnSendCerticate;
            //private InputField m_inputCerticate;
            private Button m_btnBind;
            private Text m_textBind;

            public void Init(Transform trans)
            {
                transform = trans;

                //m_inputPhoneNumber = transform.Find("Mobile/InputField").GetComponent<InputField>();
                //m_inputPhoneNumber.contentType = InputField.ContentType.IntegerNumber;

                //m_btnSendCerticate = transform.Find("Mobile/Btn_01").GetComponent<Button>();
                //m_btnSendCerticate.onClick.AddListener(OnClickSend);

                //m_inputCerticate = transform.Find("Test/InputField").GetComponent<InputField>();
                //m_inputCerticate.contentType = InputField.ContentType.IntegerNumber;

                m_btnBind = transform.Find("Btn_01").GetComponent<Button>();
                m_btnBind.onClick.AddListener(OnClickBind);
                m_textBind = transform.Find("Btn_01/Text_01").GetComponent<Text>();

                m_textBind.text = LanguageHelper.GetTextContent(2024219);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            //private void OnClickSend()
            //{

            //}

            private void OnClickBind()
            {
                SDKManager.SDKSetPhoneBind();
                UIManager.HitButton(EUIID.UI_OperationalActivity, "ClickBind", EOperationalActivity.PhoneBindGift.ToString());
            }
        }

        private class BindedInfo
        {
            private Transform transform;

            private Text m_textPhoneNumber;
            private Button m_btnGetReward;
            private Text m_textBtn;
            private bool isTakeReward;

            public void Init(Transform trans)
            {
                transform = trans;

                m_textPhoneNumber = transform.Find("ID").GetComponent<Text>();
                m_btnGetReward = transform.Find("Btn_01").GetComponent<Button>();
                m_textBtn = transform.Find("Btn_01/Text_01").GetComponent<Text>();
                m_btnGetReward.onClick.AddListener(OnClickGet);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
                UpdateState();
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void UpdateState()
            {
                isTakeReward = Sys_OperationalActivity.Instance.BindPhoneTakeWard;
                if(isTakeReward)
                {
                    m_textBtn.text = LanguageHelper.GetTextContent(2024220);
                    ImageHelper.SetImageGray(m_btnGetReward, true, true);
                }
                else
                {
                    m_textBtn.text = LanguageHelper.GetTextContent(2024217);
                    ImageHelper.SetImageGray(m_btnGetReward, false, true);
                }
            }

            private void OnClickGet()
            {
                if (!isTakeReward)
                    Sys_OperationalActivity.Instance.OnBindPhoneTakeWardReq();
                else
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024221));
                UIManager.HitButton(EUIID.UI_OperationalActivity, "ClickGetReward", EOperationalActivity.PhoneBindGift.ToString());
            }
        }

        private class GiftShowInfo
        {
            private Transform transform;

            private Transform transTemplate;

            public void Init(Transform trans)
            {
                transform = trans;

                transTemplate = transform.Find("Grid/Reward0");
                transTemplate.gameObject.SetActive(false);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo()
            {
                Lib.Core.FrameworkTool.DestroyChildren(transTemplate.parent.gameObject, transTemplate.name);

                uint dropId = 0;
                uint.TryParse(CSVParam.Instance.GetConfData(1278).str_value, out dropId);
                List<ItemIdCount> list = CSVDrop.Instance.GetDropItem(dropId);
                foreach (var data in list)
                {
                    CSVDrop.Data dropData = CSVDrop.Instance.GetDropItemData(dropId);

                    GameObject go = GameObject.Instantiate(transTemplate.gameObject, transTemplate.parent);
                    go.gameObject.SetActive(true);

                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(go.transform.Find("PropItem").gameObject);

                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(data.id, data.count, true, false, false, false, false, true, false, true);
                    itemData.equipPara = dropData.equip_para;
                    propItem.SetData(itemData, EUIID.UI_OperationalActivity);
                }
            }
        }

        private MessageInfo m_msgInfo;
        private BindingInfo m_bindingInfo;
        private BindedInfo m_bindedInfo;
        private GiftShowInfo m_giftShowInfo;

        protected override void Loaded()
        {
            m_msgInfo = new MessageInfo();
            m_msgInfo.Init(transform.Find("Message"));
            m_msgInfo.UpdateInfo();

            m_bindingInfo = new BindingInfo();
            m_bindingInfo.Init(transform.Find("Print1"));

            m_bindedInfo = new BindedInfo();
            m_bindedInfo.Init(transform.Find("Print2"));

            m_giftShowInfo = new GiftShowInfo();
            m_giftShowInfo.Init(transform.Find("Gift"));
            m_giftShowInfo.UpdateInfo();
        }

        public override void Show()
        {
            base.Show();
            OnUpdatePhoneBindStatus();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdatePhoneBindStatus, OnUpdatePhoneBindStatus, toRegister);
        }

        private void OnUpdatePhoneBindStatus()
        {
            if (Sys_OperationalActivity.Instance.BindPhoneBindingState())
            {
                m_bindingInfo.OnHide();
                m_bindedInfo.OnShow();
            }
            else
            {
                m_bindedInfo.OnHide();
                m_bindingInfo.OnShow();
            }
        }
    }
}