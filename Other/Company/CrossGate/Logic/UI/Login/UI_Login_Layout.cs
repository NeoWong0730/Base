//this file is auto created by QuickCode,you can edit it
//do not need to care initialization of ui widget any more
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2019/4/18 17:18:11
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_Login_Layout
    {
        #region UI Variable Statement
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public RectTransform FirstButton { get; private set; }
        public Animator Login_Animator { get; private set; }
        public Button Btn_Login_Button { get; private set; }
        public InputField Input_Account_InputField { get; private set; }
        public InputField Input_Password_InputField { get; private set; }
        public RectTransform SecondButton { get; private set; }
        public Button Btn_Start_Button { get; private set; }
        public Button Btn_SelectServer_Button { get; private set; }
        public Text ServerName_Text { get; private set; }
        public Image State_Icon_Image { get; private set; }
        public Text State_Text { get; private set; }
        public Button Button_Logout_Button { get; private set; }
        public Button Button_Board_Button { get; private set; }
        public Button Button_FixHotUpdate_Button1 { get; private set; }
        public Button Button_FixHotUpdate_Button2 { get; private set; }
        public Button Button_ScanQRCode_Button { get; private set; }
        public Button Button_CustomService_Button { get; private set; }
        //public Button Button_CommunityService_Button { get; private set; }
        public Button Button_XieYi_Button { get; private set; }
        public Button Button_RightAge_Button { get; private set;}

        public Text Text_Version_Text { get; private set; }
        public Text Text_Protocal_Text { get; private set; }
        public Text Text_Resources_Text { get; private set; }
        public Button Btn_PreHeat_Button { get; private set; }
        public Button Btn_Geren_Button { get; private set; }
        public Button Btn_huiguifuli_Button { get; private set; }
        //public Text readtip { get; private set; }
        //public Toggle toggleRead { get; private set; }
        //public EmojiText emojiText { get; private set; }
        #endregion
        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;
            FirstButton = mTrans.Find("Animator/_Login").GetComponent<RectTransform>();
            Login_Animator = mTrans.Find("Animator/_Login").GetComponent<Animator>();
            Btn_Login_Button = mTrans.Find("Animator/_Login/_Btn_Login").GetComponent<Button>();
            Input_Account_InputField = mTrans.Find("Animator/_Login/Account/_Input_Account").GetComponent<InputField>();
            Input_Password_InputField = mTrans.Find("Animator/_Login/Password/_Input_Password").GetComponent<InputField>();
            SecondButton = mTrans.Find("Animator/_Start").GetComponent<RectTransform>();
            Btn_Start_Button = mTrans.Find("Animator/_Start/_Btn_Start").GetComponent<Button>();
            Btn_SelectServer_Button = mTrans.Find("Animator/_Start/_Btn_SelectServer").GetComponent<Button>();
            ServerName_Text = mTrans.Find("Animator/_Start/_Btn_SelectServer/_ServerName").GetComponent<Text>();
            State_Icon_Image = mTrans.Find("Animator/_Start/_Btn_SelectServer/_State_Icon").GetComponent<Image>();
            State_Text = mTrans.Find("Animator/_Start/_Btn_SelectServer/_State_Icon/_State").GetComponent<Text>();
      
            Button_Board_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_Board").GetComponent<Button>();
            Button_CustomService_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_Service").GetComponent<Button>();
            Button_XieYi_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_xieyi").GetComponent<Button>();
            Btn_Geren_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_geren").GetComponent<Button>();
            Btn_huiguifuli_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_huiguifuli").GetComponent<Button>();
            Button_FixHotUpdate_Button1 = mTrans.Find("Animator/_Login/BtnNode/_Button_repair").GetComponent<Button>();
            Button_FixHotUpdate_Button2 = mTrans.Find("Animator/_Start/BtnNode/_Button_repair").GetComponent<Button>();
            Button_ScanQRCode_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_saoma").GetComponent<Button>();
            Button_Logout_Button = mTrans.Find("Animator/_Start/BtnNode/_Button_zhuxiao").GetComponent<Button>();
            Button_RightAge_Button = mTrans.Find("Animator/Btn_16").GetComponent<Button>();

            Text_Version_Text = mTrans.Find("Animator/_Text_Version").GetComponent<Text>();
            Text_Resources_Text = mTrans.Find("Animator/_Text_Resources").GetComponent<Text>();
            Text_Protocal_Text = mTrans.Find("Animator/_Text_Protocol").GetComponent<Text>();

            Btn_PreHeat_Button = mTrans.Find("Animator/_Start/_Btn_Preheat").GetComponent<Button>();
            Btn_PreHeat_Button.gameObject.SetActive(false);

            //toggleRead = mTrans.Find("Animator/Toggle_Read").GetComponent<Toggle>();
            //readtip = mTrans.Find("Animator/LabelIRead").GetComponent<Text>();
            //emojiText = mTrans.Find("Animator/Label").GetComponent<EmojiText>();
        }

        public void RegisterEvents(IListener listener)
        {
            Btn_Login_Button.onClick.AddListener(listener.OnLogin_ButtonClicked);
            Input_Account_InputField.onValueChanged.AddListener(listener.OnAccount_InputFieldValueChanged);
            Input_Password_InputField.onValueChanged.AddListener(listener.OnPassword_InputFieldValueChanged);
            Btn_Start_Button.onClick.AddListener(listener.OnStart_ButtonClicked);
            Btn_SelectServer_Button.onClick.AddListener(listener.OnSelectServer_ButtonClicked);
     
            Button_Board_Button.onClick.AddListener(listener.OnBoard_ButtonClicked);
            Button_CustomService_Button.onClick.AddListener(listener.OnCustomService_ButtonClicked);
            Button_XieYi_Button.onClick.AddListener(listener.OnXieYi_ButtonClicked);
            Btn_Geren_Button.onClick.AddListener(listener.OnGeRen_ButtonClicked);
            Btn_huiguifuli_Button.onClick.AddListener(listener.OnBtnHuiguifuliClick);
            Button_FixHotUpdate_Button1.onClick.AddListener(listener.OnFixHotUpdate_ButtonClicked);
            Button_FixHotUpdate_Button2.onClick.AddListener(listener.OnFixHotUpdate_ButtonClicked);
            Button_ScanQRCode_Button.onClick.AddListener(listener.OnScanQRCode_ButtonClicked);
            Button_Logout_Button.onClick.AddListener(listener.OnLogout_ButtonClicked);
            Button_RightAge_Button.onClick.AddListener(listener.OnRightAge_ButtonClicked);

            Btn_PreHeat_Button.onClick.AddListener(listener.OnBtnPreHeatClick);
            //toggleRead.onValueChanged.AddListener(listener.OnValueChanged);
            //emojiText.onHrefClick += listener.OnHrefClick;

            
        }

        public interface IListener
        {
            void OnLogin_ButtonClicked();
            void OnAccount_InputFieldValueChanged(string arg);
            void OnPassword_InputFieldValueChanged(string arg);
            void OnStart_ButtonClicked();
            void OnSelectServer_ButtonClicked();
            void OnLogout_ButtonClicked();
            void OnBoard_ButtonClicked();
   
            void OnFixHotUpdate_ButtonClicked();
            void OnScanQRCode_ButtonClicked();
            void OnCustomService_ButtonClicked();
            void OnXieYi_ButtonClicked();
            void OnGeRen_ButtonClicked();
            void OnRightAge_ButtonClicked();
            void OnHrefClick(string arg);
            void OnValueChanged(bool status);
            void OnBtnPreHeatClick();
            void OnBtnHuiguifuliClick();
        }
    }
}
