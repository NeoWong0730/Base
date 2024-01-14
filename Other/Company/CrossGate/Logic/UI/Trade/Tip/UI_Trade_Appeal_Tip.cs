using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Appeal_Tip : UIBase
    {
        private Button _btnClose;

        private Image _imgHead;
        private Text _textName;
        private Text _textID;
        private Text _textType;

        private InputField _inputField;
        private Button _btnAppeal;

        private TradeSaleRecord _recordData;
        private string _strContent;

        protected override void OnOpen(object arg)
        {
            _recordData = null;
            if (arg != null)
                _recordData = (TradeSaleRecord)arg;
        }

        protected override void OnLoaded()
        {
            _btnClose = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            _btnClose.onClick.AddListener(OnClickClose);

            _imgHead = transform.Find("Animator/Detail/ImageBG/Head").GetComponent<Image>();
            _textName = transform.Find("Animator/Detail/ImageBG/Text_Name").GetComponent<Text>();
            _textID = transform.Find("Animator/Detail/ImageBG/Text_ID").GetComponent<Text>();
            _textType = transform.Find("Animator/Detail/ImageBG/Text_Type").GetComponent<Text>();

            _inputField = transform.Find("Animator/Detail/InputField").GetComponent<InputField>();
            _inputField.onEndEdit.AddListener(OnEditEnd);

            _btnAppeal = transform.Find("Animator/Detail/Btn_01").GetComponent<Button>();
            _btnAppeal.onClick.AddListener(OnClickAppeal);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnEditEnd(string str)
        {
            _strContent = str;
        }

        private void OnClickAppeal()
        {
            if (_strContent.Length < 10)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011252));
                return;
            }

            if (_strContent.Length > 200)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011253));
                return;
            }

            Sys_Trade.Instance.OnAppeal(_recordData.DealId, _strContent, _recordData.CheckTime);
            this.CloseSelf();
        }
        
        private void UpdateInfo()
        {
            //head
            Sys_Head.Instance.SetHeadAndFrameData(_imgHead);
            //name
            _textName.text = Sys_Role.Instance.sRoleName;
            //Id
            _textID.text = string.Format("ID:{0}", Sys_Role.Instance.RoleId);

            //type
            uint lanId = 0; //未通过
            switch ((TradeCheckStatus)_recordData.CheckStatus)
            {
                case TradeCheckStatus.TradeCheckFreeze:
                case TradeCheckStatus.TradeCheckKdipFreeze:    
                    lanId = 2011248u;
                    break;
                case TradeCheckStatus.TradeCheckPunish:
                case TradeCheckStatus.TradeCheckKdipPunish:
                    lanId = 2011247u;
                    break;
                default:
                    break;
            }
            _textType.text = LanguageHelper.GetTextContent(lanId);

            _strContent = string.Empty;
        }
    }
}


