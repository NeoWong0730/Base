//this file is auto created by QuickCode,you can edit it
//do not need to care initialization of ui widget any more
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2019/4/18 20:59:21
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_Setting_Layout
    {
        #region UI Variable Statement        
        public RectTransform mRoot;
        public Button btn_Close;
        public CP_ToggleRegistry tg_Tables;

        public RectTransform rt_view0_Setting;
        public RectTransform rt_view1_Graphics;
        public CP_ToggleRegistry tg_SettingTable;
        public CP_ToggleRegistry tg_graphicsSetting;
        public RectTransform rt_graphicsSettingLayout0;
        //public RectTransform rt_graphicsSettingLayout2;
        public RectTransform rt_graphicsSettingLayout0_ToggleGroup_11;
        public Image img_headIcon;
        public Text txt_level;
        public Text txt_name;
        public Text txt_id;
        public Text txt_server;
        public Button btn_person;
        public Button btn_switchRole;
        public Button btn_bugReport;
        public Button btn_lockScreen;
        public Button btn_privacyPolicy;
        public Button btn_userAgreement;
        public Button btn_community;
        public Button btn_scanCode;

        public RectTransform rt_recommend_0;
        public RectTransform rt_recommend_1;
        public RectTransform rt_recommend_2;
        //public RectTransform rt_recommend_3;

        public RectTransform rt_view1_Audio;        

        public RectTransform rt_audioSettingLayout0;
        public RectTransform rt_audioSettingLayout1;
        public RectTransform rt_audioSettingLayout2;
        public RectTransform rt_audioSettingLayout3;
        public RectTransform rt_audioSettingLayout4;

        public RectTransform rt_view1_Game;
        public RectTransform rt_gameSettingLayout0;
        public RectTransform rt_pushSettingLayout0;

        public RectTransform rt_TabItem2_CDKey;
        public RectTransform rt_view0_CDKey;
        public InputField InputField_CDKey;
        public Button btn_CDKey;

        public RectTransform rt_sealSettingLayout0;
        public RectTransform rt_sealSettingLayout1;
        public RectTransform rt_sealSettingLayout2;
        public RectTransform rt_sealSettingLayout3;
        public Button btn_SealSetting;          

        public RectTransform rt_hangupSettingLayout0;
        public RectTransform rt_hangupSettingLayout1;
        public RectTransform rt_hangupSettingLayout2;
        public RectTransform rt_hangupSettingLayout3;

        public GameObject lockedHangup;
        public GameObject unlockedHangup;
        public Text autoHangupTip;
        public Text unlockedHangupTip;
        public Button btnRecharge;

        public Button btn_PetTips;

        public CP_Toggle tg_HotKeySetting;
        public RectTransform ScrollView_rejian;
        public Button btn_DefaultKey;

        //安全锁

        public RectTransform rt_view0_Safety;
        public Text title;
        public Text stateText;
        public GameObject openContent;
        public GameObject closeContent;
        public Button lockBtn;
        public Button unLockBtn;
        public Button forceUnLockBtn;
        public Button changePwdBtn;
        public Button cancelBtn;
        public GameObject imageClose;
        public GameObject timePanel;
        public Text dateCommit;
        public Text timeCommit;
        public Text dateRelieve;
        public Text timeRelieve;
        public GameObject leftPanel;
        public GameObject prohibitItem;
        public Text explainContent;
        public Text enforceContent;

        public GameObject goClosePowersaving;
        #endregion
        public void Parse(GameObject root)
        {
            mRoot = root.transform as RectTransform;

            btn_Close = mRoot.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            tg_Tables = mRoot.Find("Animator/Label_Scroll01/TabList").GetComponent<CP_ToggleRegistry>();

            //#设置页
            rt_view0_Setting = mRoot.Find("Animator/ViewSetting") as RectTransform;
            //##画面设置
            rt_view1_Graphics = mRoot.Find("Animator/ViewSetting/ScrollView_huamian") as RectTransform;
            tg_SettingTable = mRoot.Find("Animator/ViewSetting/SettingTable").GetComponent<CP_ToggleRegistry>();
            tg_HotKeySetting = mRoot.Find("Animator/ViewSetting/SettingTable/Toggle6").GetComponent<CP_Toggle>();
            tg_graphicsSetting = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/ToggleGroup_0").GetComponent<CP_ToggleRegistry>();
            rt_graphicsSettingLayout0 = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/SettingLayout1") as RectTransform;
            //rt_graphicsSettingLayout2 = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/SettingLayout2") as RectTransform;
            rt_graphicsSettingLayout0_ToggleGroup_11= rt_graphicsSettingLayout0.Find("ToggleGroup_11") as RectTransform;

            img_headIcon = mRoot.Find("Animator/ViewSetting/Account_info/Head/Head").GetComponent<Image>();
            txt_level = mRoot.Find("Animator/ViewSetting/Account_info/Head/Text_Level").GetComponent<Text>();
            txt_name = mRoot.Find("Animator/ViewSetting/Account_info/Account/Value").GetComponent<Text>();
            txt_id = mRoot.Find("Animator/ViewSetting/Account_info/ID/Value").GetComponent<Text>();
            txt_server = mRoot.Find("Animator/ViewSetting/Account_info/Server/Value").GetComponent<Text>();
            btn_person = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_01").GetComponent<Button>();
            btn_switchRole = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_06").GetComponent<Button>();
            btn_bugReport = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_03").GetComponent<Button>();
            btn_lockScreen = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_02").GetComponent<Button>();
            btn_privacyPolicy = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_04").GetComponent<Button>();
            btn_userAgreement = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_05").GetComponent<Button>();
            //btn_community = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_07").GetComponent<Button>();
            btn_scanCode = mRoot.Find("Animator/ViewSetting/Account_info/BtnGroup/Btn_08").GetComponent<Button>();

            btn_PetTips = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin/Viewport/DropdownItem01/Button_Message").GetComponent<Button>();

            rt_recommend_0 = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/ToggleGroup_0/Toggle1/Recommend") as RectTransform;
            rt_recommend_1 = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/ToggleGroup_0/Toggle2/Recommend") as RectTransform;
            rt_recommend_2 = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/ToggleGroup_0/Toggle3/Recommend") as RectTransform;
            //rt_recommend_3 = mRoot.Find("Animator/ViewSetting/ScrollView_huamian/Viewport/ToggleGroup_1/Toggle4/Recommend") as RectTransform;

            //##声音设置
            rt_TabItem2_CDKey = mRoot.Find("Animator/Label_Scroll01/TabList/TabItem (2)") as RectTransform;
            rt_view1_Graphics = mRoot.Find("Animator/ViewSetting/ScrollView_shengyin") as RectTransform;            
            rt_audioSettingLayout0 = mRoot.Find("Animator/ViewSetting/ScrollView_shengyin/Viewport/Slider0") as RectTransform;
            rt_audioSettingLayout1 = mRoot.Find("Animator/ViewSetting/ScrollView_shengyin/Viewport/Slider1") as RectTransform;
            rt_audioSettingLayout2 = mRoot.Find("Animator/ViewSetting/ScrollView_shengyin/Viewport/Slider2") as RectTransform;
            rt_audioSettingLayout3 = mRoot.Find("Animator/ViewSetting/ScrollView_shengyin/Viewport/Slider3") as RectTransform;
            rt_audioSettingLayout4 = mRoot.Find("Animator/ViewSetting/ScrollView_shengyin/Viewport/ToggleGrp") as RectTransform;

            //##游戏设置
            rt_view1_Graphics = mRoot.Find("Animator/ViewSetting/ScrollView_youxi") as RectTransform;
            rt_gameSettingLayout0 = mRoot.Find("Animator/ViewSetting/ScrollView_youxi/Viewport/ToggleGrp") as RectTransform;
            //省电模式
            goClosePowersaving = mRoot.Find("Animator/ViewSetting/ScrollView_youxi/Viewport/ToggleGrp/Toggle_2022").gameObject;
            //##游戏设置 -- 推送
            rt_pushSettingLayout0 = mRoot.Find("Animator/ViewSetting/ScrollView_youxi/Viewport/ToggleGrp2") as RectTransform;
            //成就弹窗屏蔽
            GameObject achieveBtn = mRoot.Find("Animator/ViewSetting/ScrollView_youxi/Viewport/ToggleGrp/Toggle_2031").gameObject;
            achieveBtn.SetActive(Sys_Achievement.Instance.CheckAchievementIsCanShow());
            //#兑换码
            rt_view0_CDKey = mRoot.Find("Animator/ViewCDkey") as RectTransform;
            InputField_CDKey = mRoot.Find("Animator/ViewCDkey/InputField").GetComponent<InputField>();
            btn_CDKey = mRoot.Find("Animator/ViewCDkey/Btn_01").GetComponent<Button>();

            //宠物封印
            rt_view1_Graphics = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin") as RectTransform;
            rt_sealSettingLayout0 = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin/Viewport/ToggleGrp01") as RectTransform;
            rt_sealSettingLayout1 = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin/Viewport/DropdownItem01") as RectTransform;
            rt_sealSettingLayout2 = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin/Viewport/DropdownItem02") as RectTransform;
            rt_sealSettingLayout3 = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin/Viewport/ToggleGrp02") as RectTransform;
            btn_SealSetting = mRoot.Find("Animator/ViewSetting/ScrollView_fengyin/Viewport/View_Setting/Btn_Set").GetComponent<Button>();

            // 挂机设置
            rt_hangupSettingLayout0 = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp01") as RectTransform;
            rt_hangupSettingLayout1 = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp02") as RectTransform;
            rt_hangupSettingLayout2 = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp03") as RectTransform;
            rt_hangupSettingLayout3 = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp04") as RectTransform;
            lockedHangup = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp03/Lock").gameObject;
            unlockedHangup = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp03/Unlock").gameObject;
            autoHangupTip = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp01/Text").GetComponent<Text>();
            unlockedHangupTip = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp03/Unlock/Text_Tip").GetComponent<Text>();
            btnRecharge = mRoot.Find("Animator/ViewSetting/ScrollView_guaji/Viewport/ToggleGrp03/Lock/Btn_01").GetComponent<Button>();

            //热键
            ScrollView_rejian = mRoot.Find("Animator/ViewSetting/ScrollView_rejian") as RectTransform;
            //安全锁
            rt_view0_Safety = mRoot.Find("Animator/ViewSafety") as RectTransform;
            title = mRoot.Find("Animator/ViewSafety/Left/Title").GetComponent<Text>();
            stateText = mRoot.Find("Animator/ViewSafety/Left/Title/State").GetComponent<Text>();
            openContent = mRoot.Find("Animator/ViewSafety/Left/Scroll View_1").gameObject;
            closeContent = mRoot.Find("Animator/ViewSafety/Left/Scroll View_2").gameObject;
            imageClose = mRoot.Find("Animator/ViewSafety/Lock_bg/Image_2").gameObject;
            lockBtn = mRoot.Find("Animator/ViewSafety/Btn_Lock").GetComponent<Button>();
            unLockBtn = mRoot.Find("Animator/ViewSafety/Btn_Unlock").GetComponent<Button>();
            forceUnLockBtn = mRoot.Find("Animator/ViewSafety/Btn_ForceUnlock").GetComponent<Button>();
            changePwdBtn = mRoot.Find("Animator/ViewSafety/Btn_ChangePassword").GetComponent<Button>();
            cancelBtn = mRoot.Find("Animator/ViewSafety/Btn_Cancel").GetComponent<Button>();
            timePanel = mRoot.Find("Animator/ViewSafety/Left/Time").gameObject;
            dateCommit = mRoot.Find("Animator/ViewSafety/Left/Time/Time_1_2").GetComponent<Text>();
            timeCommit = mRoot.Find("Animator/ViewSafety/Left/Time/Time_1_3").GetComponent<Text>();
            dateRelieve = mRoot.Find("Animator/ViewSafety/Left/Time/Time_2_2").GetComponent<Text>();
            timeRelieve = mRoot.Find("Animator/ViewSafety/Left/Time/Time_2_3").GetComponent<Text>();
            leftPanel= mRoot.Find("Animator/ViewSafety/Left/Scroll View_1/Viewport/Content").gameObject;
            prohibitItem = mRoot.Find("Animator/ViewSafety/Left/Scroll View_1/Viewport/Content/Content/Item").gameObject;
            explainContent = mRoot.Find("Animator/ViewSafety/Left/Scroll View_1/Viewport/Content/Explain").GetComponent<Text>();
            enforceContent = mRoot.Find("Animator/ViewSafety/Left/Scroll View_2/Viewport/Content/Explain").GetComponent<Text>();

            btn_DefaultKey = mRoot.Find("Animator/ViewSetting/ScrollView_rejian/Btn_01").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            btn_Close.onClick.AddListener(listener.OnBtnClose_ButtonClicked);
            btn_person.onClick.AddListener(listener.OnBtnPerson_ButtonClicked);
            btn_switchRole.onClick.AddListener(listener.OnBtnSwitchRole_ButtonClicked);
            btn_bugReport.onClick.AddListener(listener.OnBtnBugReport_ButtonClicked);
            btn_lockScreen.onClick.AddListener(listener.OnBtnLockScreen_ButtonClicked);
            //btn_community.onClick.AddListener(listener.OnBtnCommunity_ButtonClicked);
            btn_scanCode.onClick.AddListener(listener.OnBtnScanCode_ButtonClicked);

            btn_privacyPolicy.onClick.AddListener(listener.OnBtnPrivacyPolicy_ButtonClicked);
            btn_userAgreement.onClick.AddListener(listener.OnUserAgreement_ButtonClicked);
            btn_PetTips.onClick.AddListener(listener.OnPetTips_ButtonClicked);
            btnRecharge.onClick.AddListener(listener.OnRecharge_ButtonClicked);

            btn_CDKey.onClick.AddListener(listener.OnBtnExchangeCode_ButtonClicked);
            InputField_CDKey.onValueChanged.AddListener(listener.OnCDKey_InputFieldValueChanged);

            tg_Tables.onToggleChange += listener.OnTg_Tables_ToggleChange;
            tg_SettingTable.onToggleChange += listener.OnTg_SettingTable_ToggleChange;
            tg_HotKeySetting.onValueChanged.AddListener(listener.OnClickToggle);
            btn_DefaultKey.onClick.AddListener(listener.OnBtnDefaultKey_ButtonClicked);

            //安全锁
            lockBtn.onClick.AddListener(listener.OnLockBtnClicked);
            unLockBtn.onClick.AddListener(listener.OnUnLockBtnClicked);
            forceUnLockBtn.onClick.AddListener(listener.OnForceUnLockBtnClicked);
            changePwdBtn.onClick.AddListener(listener.OnChangePwdBtnClicked);
            cancelBtn.onClick.AddListener(listener.OnCancelBtnClicked);

            btn_SealSetting.onClick.AddListener(listener.OnBtnSealSetting_ButtonClicked);

        }

        public interface IListener
        {
            void OnBtnClose_ButtonClicked();
            void OnBtnPerson_ButtonClicked();
            void OnBtnSwitchRole_ButtonClicked();
            void OnBtnLockScreen_ButtonClicked();
            void OnBtnBugReport_ButtonClicked();
            void OnBtnCommunity_ButtonClicked();
            void OnBtnScanCode_ButtonClicked();
            void OnBtnPrivacyPolicy_ButtonClicked();
            void OnUserAgreement_ButtonClicked();
            void OnBtnExchangeCode_ButtonClicked();
            void OnTg_Tables_ToggleChange(int from, int to);
            void OnTg_SettingTable_ToggleChange(int from, int to);
            void OnCDKey_InputFieldValueChanged(string arg);
            void OnPetTips_ButtonClicked();
            void OnRecharge_ButtonClicked();
            void OnClickToggle(bool isOn);

            //安全锁
            void OnLockBtnClicked();
            void OnUnLockBtnClicked();
            void OnForceUnLockBtnClicked();
            void OnChangePwdBtnClicked();
            void OnCancelBtnClicked();
            void OnBtnDefaultKey_ButtonClicked();
            void OnBtnSealSetting_ButtonClicked();
        }
    }
}

