//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2020/1/13 11:33:56
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Logic
{

    public class UI_ChatSimplify_Layout
    {
        #region UI Variable Statement 
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public RectTransform rtBG_RectTransform { get; private set; }
        public Button btn_Friend_Button { get; private set; }
        public InfinityIrregularGrid sv_content_InfinityIrregularGrid { get; private set; }
        public Button btn_arrow_Button { get; private set; }
        public Button btn_setting_Button { get; private set; }
        public Button btn_tip_Button { get; private set; }
        public Button btn_videoStation { get; private set; }

        public Text txt_tip_Text { get; private set; }
        public Image rt_Setting_Image { get; private set; }
        public Button btn_Close_Button { get; private set; }
        public Toggle tog_System_Toggle { get; private set; }
        public Toggle tog_World_Toggle { get; private set; }
        public Toggle tog_Local_Toggle { get; private set; }
        public Toggle tog_Guild_Toggle { get; private set; }
        public Toggle tog_Team_Toggle { get; private set; }
        public Toggle tog_LookForTeam_Toggle { get; private set; }
        public Toggle tog_ChannelCareer_Toggle { get; private set; }
        public Toggle tog_ChannelBraveTeam_Toggle { get; private set; }
        public RectTransform tempContent_RectTransform { get; private set; }
        public EmojiText txtTemplate_EmojiText { get; private set; }
        public EventTrigger sv_content_EventTrigger { get; private set; }
        public GameObject mail_Red { get; private set; }

        public RectTransform rt_voiceChannel_RectTransform { get; private set; }

        public Button btn_world_RectTransform { get; private set; }
        public Button btn_home_RectTransform { get; private set; }
        public Button btn_team_RectTransform { get; private set; }
        public RectTransform btn_voice_RectTransform { get; private set; }

        public Text txt_World { get; private set; }
        public Text txt_Guild { get; private set; }
        public Text txt_Team { get; private set; }

        /// <summary>
        /// ÏûÏ¢°ü
        /// </summary>
        public Button btn_MessageBag { get; private set; }
        public Text txt_MessageType { get; private set; }
        public GameObject go_MessageRedPoint { get; private set; }
        public Text txt_MessageCount { get; private set; }
        public Button btn_money { get; private set; }
        public Button btn_Plan { get; private set; }
        public RectTransform rt_RedTips { get; private set; }

        #endregion
        public void Parse(GameObject root)
        {
            mRoot = root;
            mTrans = root.transform;
            rtBG_RectTransform = mTrans.Find("Animator/_rtBG").GetComponent<RectTransform>();
            btn_Friend_Button = mTrans.Find("Animator/_rtBG/rtTop/_btn_Friend").GetComponent<Button>();

            rt_voiceChannel_RectTransform = mTrans.Find("Animator/_rtBG/rtTop/Grid") as RectTransform;

            btn_world_RectTransform = mTrans.Find("Animator/_rtBG/rtTop/Grid/_btn_world").GetComponent<Button>();
            btn_home_RectTransform = mTrans.Find("Animator/_rtBG/rtTop/Grid/_btn_home").GetComponent<Button>();
            btn_team_RectTransform = mTrans.Find("Animator/_rtBG/rtTop/Grid/_btn_team").GetComponent<Button>();

            btn_voice_RectTransform = mTrans.Find("Animator/_rtBG/rtTop/_btn_world") as RectTransform;

            txt_World = mTrans.Find("Animator/_rtBG/rtTop/_btn_world/Image/Text_world").GetComponent<Text>();
            txt_Guild = mTrans.Find("Animator/_rtBG/rtTop/_btn_world/Image/Text_home").GetComponent<Text>();
            txt_Team = mTrans.Find("Animator/_rtBG/rtTop/_btn_world/Image/Text_team").GetComponent<Text>();

            sv_content_InfinityIrregularGrid = mTrans.Find("Animator/_rtBG/rtBottom/_sv_content").GetComponent<InfinityIrregularGrid>();
            btn_arrow_Button = mTrans.Find("Animator/_rtBG/rtBottom/_btn_arrow").GetComponent<Button>();
            btn_setting_Button = mTrans.Find("Animator/_rtBG/rtBottom/_btn_setting").GetComponent<Button>();
            btn_tip_Button = mTrans.Find("Animator/_rtBG/rtBottom/_btn_tip").GetComponent<Button>();
            txt_tip_Text = mTrans.Find("Animator/_rtBG/rtBottom/_btn_tip/_txt_tip").GetComponent<Text>();
            rt_Setting_Image = mTrans.Find("Animator/_rt_Setting").GetComponent<Image>();
            btn_Close_Button = mTrans.Find("Animator/_rt_Setting/_btn_Close").GetComponent<Button>();
            tog_System_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_System/_tog_System").GetComponent<Toggle>();
            tog_World_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_World/_tog_World").GetComponent<Toggle>();
            tog_Local_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_Local/_tog_Local").GetComponent<Toggle>();
            tog_Guild_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_Guild/_tog_Guild").GetComponent<Toggle>();
            tog_Team_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_Team/_tog_Team").GetComponent<Toggle>();
            tog_LookForTeam_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_LookForTeam/_tog_LookForTeam").GetComponent<Toggle>();
            tog_ChannelCareer_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_ChannelCareer/_tog_World").GetComponent<Toggle>();
            tog_ChannelBraveTeam_Toggle = mTrans.Find("Animator/_rt_Setting/Panel/tog_BraveTeam/_tog_Team").GetComponent<Toggle>();
            tempContent_RectTransform = mTrans.Find("@tempContent").GetComponent<RectTransform>();
            txtTemplate_EmojiText = mTrans.Find("_txtTemplate").GetComponent<EmojiText>();
            sv_content_EventTrigger = mTrans.Find("Animator/_rtBG/rtBottom/_sv_content").GetComponent<EventTrigger>();
            mail_Red = mTrans.Find("Animator/_rtBG/rtTop/_btn_mail").gameObject;

            btn_MessageBag = mTrans.Find("Animator/_rtBG/rtTop/_btn_Message").GetComponent<Button>();
            txt_MessageType = mTrans.Find("Animator/_rtBG/rtTop/_btn_Message/Text").GetComponent<Text>();
            go_MessageRedPoint = mTrans.Find("Animator/_rtBG/rtTop/_btn_Message/Image_Red").gameObject;
            txt_MessageCount = mTrans.Find("Animator/_rtBG/rtTop/_btn_Message/Image_Red/Text").GetComponent<Text>();

            btn_money = mTrans.Find("Animator/_rtBG/rtBottom/_btn_money").GetComponent<Button>();
            btn_Plan = mTrans.Find("Animator/_rtBG/rtTop/_btn_Plan").GetComponent<Button>();
            rt_RedTips = btn_money.transform.Find("Image_RedTips") as RectTransform;

            btn_videoStation = mTrans.Find("Animator/_rtBG/rtTop/_btn_Video").GetComponent<Button>();

        }

        public void RegisterEvents(IListener listener)
        {
            btn_Friend_Button.onClick.AddListener(listener.OnFriend_ButtonClicked);
            btn_arrow_Button.onClick.AddListener(listener.Onarrow_ButtonClicked);
            btn_setting_Button.onClick.AddListener(listener.Onsetting_ButtonClicked);
            btn_tip_Button.onClick.AddListener(listener.Ontip_ButtonClicked);
            btn_Close_Button.onClick.AddListener(listener.OnClose_ButtonClicked);
            tog_System_Toggle.onValueChanged.AddListener(listener.OnSystem_ToggleValueChanged);
            tog_World_Toggle.onValueChanged.AddListener(listener.OnWorld_ToggleValueChanged);
            tog_Local_Toggle.onValueChanged.AddListener(listener.OnLocal_ToggleValueChanged);
            tog_Guild_Toggle.onValueChanged.AddListener(listener.OnGuild_ToggleValueChanged);
            tog_Team_Toggle.onValueChanged.AddListener(listener.OnTeam_ToggleValueChanged);
            tog_LookForTeam_Toggle.onValueChanged.AddListener(listener.OnLookForTeam_ToggleValueChanged);
            tog_ChannelCareer_Toggle.onValueChanged.AddListener(listener.OnChannelCareer_ToggleValueChanged);
            tog_ChannelBraveTeam_Toggle.onValueChanged.AddListener(listener.OnChannelBraveTeam_ToggleValueChanged);

            btn_world_RectTransform.onClick.AddListener(listener.OnWorld_ButtonClicked);
            btn_home_RectTransform.onClick.AddListener(listener.OnGuild_ButtonClicked);
            btn_team_RectTransform.onClick.AddListener(listener.OnTeam_ButtonClicked);
            btn_MessageBag.onClick.AddListener(listener.OnMessageBag_ButtonClicked);
            btn_money.onClick.AddListener(listener.OnMoney_ButtonClicked);
            btn_Plan.onClick.AddListener(listener.OnMonePlan_ButtonClicked);
            btn_videoStation.onClick.AddListener(listener.OnVideoStation_ButtonClicked);
        }

        public interface IListener
        {
            void OnFriend_ButtonClicked();
            void Onarrow_ButtonClicked();
            void Onsetting_ButtonClicked();
            void Ontip_ButtonClicked();
            void OnClose_ButtonClicked();
            void OnSystem_ToggleValueChanged(bool arg);
            void OnWorld_ToggleValueChanged(bool arg);
            void OnLocal_ToggleValueChanged(bool arg);
            void OnGuild_ToggleValueChanged(bool arg);
            void OnTeam_ToggleValueChanged(bool arg);
            void OnLookForTeam_ToggleValueChanged(bool arg);
            void OnChannelCareer_ToggleValueChanged(bool arg);
            void OnChannelBraveTeam_ToggleValueChanged(bool arg);
            void OnWorld_ButtonClicked();
            void OnGuild_ButtonClicked();
            void OnTeam_ButtonClicked();
            void OnMessageBag_ButtonClicked();
            void OnMoney_ButtonClicked();

            void OnMonePlan_ButtonClicked();
            void OnVideoStation_ButtonClicked();
        }
    }

}
