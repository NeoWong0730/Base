using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lib.Core;
using Logic;

public class UI_Menu_Layout
{
    public Transform mTrans;
    public Button Btn_Copy;
    public Button Btn_ClueTask;
    public Button Btn_Partner;
    public Button Btn_Role;
    public Button Btn_Change;
    public Button Btn_CookMain;
    public Button Btn_Bag;
    public Button Btn_TempBag;
    public Button Btn_SkillLearn;
    public Button Btn_Equip;
    public Button Btn_Treasure;
    public Button Btn_HundreadPeopelBuff;
    public GameObject hundredBufferRed;
    public GameObject hundredBufferBlue;
    public Button mapMailGo;
    public Text mapMailCountText;

    public Button Btn_Hangup;
    public GameObject go_hangup1;
    public GameObject go_hangup2;

    public Image roleIcon;
    public Text roleLv;
    public Text roleName;
    public Slider roleLife;
    public Slider roleMagic;
    public GameObject changeHeadRedPoint;

    public Image petIcon;
    public Image nopetGo;
    public Text petLv;
    public GameObject petLvGo;
    public Text petName;
    public Slider petLife;
    public Slider petMagic;
    public GameObject petLifeGo;
    public GameObject petMagicGo;
    public GameObject petlevelupfx;
    public GameObject petAddPointRedPoint;

    public Button Btn_Back;
    public GameObject copyout;
    public GameObject copyin;
    public Transform petShow;
    public Button Btn_Header;
    public GameObject view_FPS;
    public Text txt_FPS;

    public Button Btn_Task;
    public Transform TaskOrTeam;
    public GameObject view_nearbyNpc;
    public GameObject nearbyNpcContent;
    public GameObject nearbyNpcPrefab;
    public GameObject view_Collect;
    public Image collectIcon;
    public Image collectIconBg;
    public Text collectName;
    public GameObject view_Probe;
    public Image probeProgress;
    public Image exp;
    public GameObject Grid01Go;
    public GameObject buttonGo;
    public GameObject probebuttonGo;
    public Button Btn_NewPet;
    public Button Btn_LifeSkill;
    public Button Btn_Family;
    public Button Btn_Close_Normal;
    public Button Btn_Close_Probe;
    public Button Btn_Open;
    public GameObject open_RedPoint;

    public Button Btn_Repair;
    public Button Btn_ExplorationTip;
    public Image explorationIcon;
    public Button Btn_Weather;
    public Image Image_Weather_DayOrNight;
    public Image Image_Circle;
    public Button Btn_Crystal;

    public Button Btn_Old;
    public Button Btn_Eye;
    public Button Btn_Look;
    public GameObject Btn_Look_Effect;
    public Button Btn_TempPetBag;
    public GameObject BlockImage;

    public Button Btn_Uplifted;
    public GameObject Go_Uplifted_Fx;
    public GameObject Go_NormalMenu;
    public GameObject Go_ProbeMenu;

    public Button m_BtnDaily;
    public Text m_TexDailyName;
    public Text m_TexDailyTime;
    public Text m_TexDailyNum;

    public GameObject[] wifiObj = new GameObject[3];
    public Text timeText;
    public Slider power;

    public Transform m_TransTeamRed;
    public GameObject m_View_Map;
    public GameObject m_ServerActivityMenu;
    public GameObject m_DailyInterface;

    public Transform m_SurvivalPvp;
    public Text m_TexNameServivalPvp;
    public Text m_TexTimeServivalPvp;
    public Button m_BtnServivalPvp;
    public Animator m_AnServivalPvp;

    public Button m_SpecialBtn;
    public Transform m_SpecialBtn_Arrow;
    public GameObject m_Special_Grid;
    public Button m_Privilege_Button;

    //消息包
    public Button Btn_MessageBag;
    public Text m_TextMessageBag;
    public GameObject messageBagRedPoint;
    public Text countText;


    public Button m_BtnBattlePass;
    public Transform m_TransBattlePassRedDot;
    public GameObject View_RedPacket;
    public Button Button_RedpPacket;

    public Button Button_FamilyEesBattleSignupTip;
    public CP_TransformContainer familyResBattleHider;

    public Button m_Cooking;
    public GameObject m_CookingRed;

    public GameObject m_ViewWarning;
    public GameObject m_ViewTask;
    public GameObject m_ViewIcon;

    public Button m_TrialGateSkillBtn;

    public Button m_BossTowerBtn;

    public Button m_BtnBless;
    public Transform m_TransBlessRedDot;
    public void Parse(GameObject root)
    {
        mTrans = root.transform;
        m_ViewWarning = mTrans.Find("Animator/View_Warning").gameObject;
        m_ViewTask = mTrans.Find("Animator/View_Task").gameObject;
        m_ViewIcon = mTrans.Find("Animator/View_Icon").gameObject;
        familyResBattleHider = mTrans.GetComponent<CP_TransformContainer>();
        Button_FamilyEesBattleSignupTip = mTrans.Find("Animator/View_Warning/Grid01/Button_FamilyBattle").GetComponent<Button>();

        Btn_Partner = mTrans.Find("Animator/View_Menu/Grid01/Button_Partner").GetComponent<Button>();
        Btn_Bag = mTrans.Find("Animator/View_Menu/Grid01/Button_Bag").GetComponent<Button>();
        Btn_Role = mTrans.Find("Animator/View_Menu/Grid01/Button_Role").GetComponent<Button>();
        Btn_Change = mTrans.Find("Animator/Button_Change").GetComponent<Button>();
        Btn_CookMain = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Cooking").GetComponent<Button>();
        Btn_TempBag = mTrans.Find("Animator/View_Warning/Grid01/Button_ExtraBag").GetComponent<Button>();
        Btn_SkillLearn = mTrans.Find("Animator/View_Menu/Grid01/Button_Skill").GetComponent<Button>();
        Btn_Equip = mTrans.Find("Animator/View_Menu/Grid01/Button_Equip").GetComponent<Button>();
        //Btn_Treasure = mTrans.Find("Animator/View_Menu/Grid01/Button_Treasure").GetComponent<Button>();
        Btn_Hangup = mTrans.Find("Animator/View_SpecialMenu/Grid01/Button_Hangup").GetComponent<Button>();
        Btn_HundreadPeopelBuff = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Hundred").GetComponent<Button>();
        hundredBufferRed = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Hundred/Red").gameObject;
        hundredBufferBlue = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Hundred/Blue").gameObject;
        mapMailGo = mTrans.Find("Animator/View_Warning/Grid01/Button_MapMail").GetComponent<Button>();
        mapMailCountText = mTrans.Find("Animator/View_Warning/Grid01/Button_MapMail/Num").GetComponent<Text>();

        go_hangup1 = mTrans.Find("Animator/View_SpecialMenu/Grid01/Button_Hangup/Text").gameObject;
        go_hangup2 = mTrans.Find("Animator/View_SpecialMenu/Grid01/Button_Hangup/Text2").gameObject;
        changeHeadRedPoint = mTrans.Find("Animator/View_Icon/Image_Role/Image_Dot").gameObject;
        roleIcon = mTrans.Find("Animator/View_Icon/Image_Role/Head").GetComponent<Image>();
        roleLv = mTrans.Find("Animator/View_Icon/Image_Role/Text_Level").GetComponent<Text>();
        roleName = mTrans.Find("Animator/View_Icon/Image_Role/Text_Name").GetComponent<Text>();
        roleLife = mTrans.Find("Animator/View_Icon/Image_Role/Text_Life/Image_Life").GetComponent<Slider>();
        roleMagic = mTrans.Find("Animator/View_Icon/Image_Role/Text_Magic/Image_Magic").GetComponent<Slider>();
        petIcon = mTrans.Find("Animator/View_Icon/Image_Icon_Pet").GetComponent<Image>();
        nopetGo = mTrans.Find("Animator/View_Icon/Image_Pet/Image_Empty").GetComponent<Image>();
        petLvGo = mTrans.Find("Animator/View_Icon/Image_Pet/Image_Level_BG").gameObject;
        petlevelupfx = mTrans.Find("Animator/View_Icon/Image_Icon_Pet/Fx_ui_Petup").gameObject;
        petLv = mTrans.Find("Animator/View_Icon/Image_Pet/Text_Level").GetComponent<Text>();
        petName = mTrans.Find("Animator/View_Icon/Image_Pet/Text_Name").GetComponent<Text>();
        petLife = mTrans.Find("Animator/View_Icon/Image_Pet/Text_Life/Image_Life").GetComponent<Slider>();
        petMagic = mTrans.Find("Animator/View_Icon/Image_Pet/Text_Magic/Image_Magic").GetComponent<Slider>();
        petLifeGo = mTrans.Find("Animator/View_Icon/Image_Pet/Text_Life/Image_Life").gameObject;
        petMagicGo = mTrans.Find("Animator/View_Icon/Image_Pet/Text_Magic/Image_Magic").gameObject;
        petAddPointRedPoint = mTrans.Find("Animator/View_Icon/Image_Pet/Image_Dot").gameObject;

        petShow = mTrans.Find("Animator/View_Icon/Image_Pet");
        Btn_Header = mTrans.Find("Animator/View_Icon/Image_Role").GetComponent<Button>();
        view_FPS = mTrans.Find("Animator/View_FPS").gameObject;
        txt_FPS = mTrans.Find("Animator/View_FPS/Text").GetComponent<Text>();
        Btn_Task = mTrans.Find("Animator/View_Task/Button_Open").GetComponent<Button>();
        TaskOrTeam = mTrans.Find("Animator/View_Task/Node/UI_TaskOrTeam");
        view_Collect = mTrans.Find("Animator/View_Collect/NPC_Collect").gameObject;
        collectIcon = mTrans.Find("Animator/View_Collect/NPC_Collect/Root/Image1").GetComponent<Image>();
        collectIconBg = mTrans.Find("Animator/View_Collect/NPC_Collect/Root/Image_Blank/Image3").GetComponent<Image>();
        collectName = mTrans.Find("Animator/View_Collect/NPC_Collect/Root/Text").GetComponent<Text>();
        view_Probe = mTrans.Find("Animator/View_Collect/Probe_Collect").gameObject;
        probeProgress = mTrans.Find("Animator/View_Collect/Probe_Collect/Animator/Image_Mask/Image_Slider").GetComponent<Image>();
        exp = mTrans.Find("Animator/View_ExpProgress/Image_Exp").GetComponent<Image>();
        Grid01Go = mTrans.Find("Animator/View_Menu/Grid01").gameObject;
        buttonGo = mTrans.Find("Animator/View_Menu").gameObject;

        Btn_NewPet = mTrans.Find("Animator/View_Icon/Image_Pet").GetComponent<Button>();
        Btn_LifeSkill = mTrans.Find("Animator/View_Menu/Grid01/Button_LifeSkill").GetComponent<Button>();
        Btn_Family = mTrans.Find("Animator/View_Menu/Grid01/Button_Family").GetComponent<Button>();
        m_Cooking = mTrans.Find("Animator/View_Menu/Grid01/Button_Cooing").GetComponent<Button>();
        m_CookingRed = mTrans.Find("Animator/View_Menu/Grid01/Button_Cooing/Image_Dot").gameObject;

        Btn_Repair = mTrans.Find("Animator/View_Warning/Grid01/Button_Repair").GetComponent<Button>();
        Btn_Crystal = mTrans.Find("Animator/View_Warning/Grid01/Button_Crystal").GetComponent<Button>();
        Btn_ExplorationTip = mTrans.Find("Animator/View_Warning/Grid01/Button_Map").GetComponent<Button>();
        explorationIcon = mTrans.Find("Animator/View_Warning/Grid01/Button_Map/Image_Icon").GetComponent<Image>();
        Btn_Weather = mTrans.Find("Animator/View_Map/MapBG/Btn_Weather01").GetComponent<Button>();
        Image_Weather_DayOrNight = mTrans.Find("Animator/View_Map/MapBG/Image_Weather02").GetComponent<Image>();
        Image_Circle = mTrans.Find("Animator/View_Map/MapBG/Image_Circle").GetComponent<Image>();
        Btn_Close_Normal = mTrans.Find("Animator/View_Menu/Grid01/View_Close_Btn/Button_Close").GetComponent<Button>();
        Btn_Close_Probe = mTrans.Find("Animator/View_Menu/Grid_Probe/View_Close_Btn/Button_Close").GetComponent<Button>();
        Btn_Open = mTrans.Find("Animator/Button_Open").GetComponent<Button>();
        open_RedPoint = mTrans.Find("Animator/Button_Open/Image_Dot").gameObject;
        probebuttonGo = mTrans.Find("Animator/View_Menu/Grid_Probe").gameObject;

        Btn_Old = mTrans.Find("Animator/View_Menu/Grid_Probe/Button_Old").GetComponent<Button>();
        Btn_Eye = mTrans.Find("Animator/View_Menu/Grid_Probe/Button_Eye").GetComponent<Button>();
        Btn_Look = mTrans.Find("Animator/View_Menu/Grid_Probe/Button_Look").GetComponent<Button>();
        Btn_Look_Effect = Btn_Look.gameObject.FindChildByName("Fx_ui_yindao06");
        Btn_TempPetBag = mTrans.Find("Animator/View_Warning/Grid01/Button_PetBag").GetComponent<Button>();
        BlockImage = mTrans.Find("Animator/BlockImage").gameObject;

        Btn_Uplifted = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Uplifted").GetComponent<Button>();
        Go_Uplifted_Fx = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Uplifted/Image_Icon/Image (1)/Fx_ui_Uplifted02").gameObject;
        Go_NormalMenu = mTrans.Find("Animator/Button_Change/Image_Icon1").gameObject;
        Go_ProbeMenu = mTrans.Find("Animator/Button_Change/Image_Icon2").gameObject;

        view_nearbyNpc = mTrans.gameObject.FindChildByName("View_NPCTrigger");
        nearbyNpcContent = view_nearbyNpc.FindChildByName("Content");
        nearbyNpcPrefab = view_nearbyNpc.FindChildByName("Image_Button");
        Btn_MessageBag = mTrans.Find("Animator/View_Warning/Grid01/Button_Message").GetComponent<Button>();
        m_TextMessageBag = mTrans.Find("Animator/View_Warning/Grid01/Button_Message/Text").GetComponent<Text>();
        messageBagRedPoint = mTrans.Find("Animator/View_Warning/Grid01/Button_Message/Image_Red").gameObject;
        countText = mTrans.Find("Animator/View_Warning/Grid01/Button_Message/Image_Red/Text").GetComponent<Text>();
        m_View_Map = mTrans.Find("Animator/View_Map").gameObject;
        m_ServerActivityMenu = mTrans.Find("Animator/View_ServerActivity").gameObject;
        m_DailyInterface = mTrans.Find("Animator/View_Activity").gameObject;

        m_TransTeamRed = mTrans.Find("Animator/View_Task/Node/UI_TaskOrTeam/Toggle_Team/UI_RedTips_Small");

        m_BtnDaily = mTrans.Find("Animator/View_Warning/Grid01/Button_Activity").GetComponent<Button>();
        m_TexDailyName = m_BtnDaily.transform.Find("Text").GetComponent<Text>();
        m_TexDailyTime = m_BtnDaily.transform.Find("Text1").GetComponent<Text>();
        m_TexDailyNum = m_BtnDaily.transform.Find("Text2").GetComponent<Text>();

        for (int i = 0; i < 3; ++i)
        {
            wifiObj[i] = mTrans.Find("Animator/View_Map/MapBG/Image_Wifi/Image_Wifi0" + (i + 1).ToString()).gameObject;
        }
        timeText = mTrans.Find("Animator/View_Map/MapBG/Text_Time").GetComponent<Text>();
        power = mTrans.Find("Animator/View_Map/MapBG/Image_Power").GetComponent<Slider>();

        m_SurvivalPvp = mTrans.Find("Animator/View_SurvivalPVP");

        m_TexNameServivalPvp = m_SurvivalPvp.Find("Btn_PVP/Text_Name").GetComponent<Text>();
        m_TexTimeServivalPvp = m_SurvivalPvp.Find("Btn_PVP/Text_Time").GetComponent<Text>();
        m_BtnServivalPvp = m_SurvivalPvp.Find("Btn_PVP").GetComponent<Button>();
        m_AnServivalPvp = m_SurvivalPvp.Find("Btn_PVP").GetComponent<Animator>();

        m_SpecialBtn = mTrans.Find("Animator/View_SpecialMenu/Button01").GetComponent<Button>();
        m_SpecialBtn_Arrow = mTrans.Find("Animator/View_SpecialMenu/Button01/Image_Icon");
        m_Special_Grid = mTrans.Find("Animator/View_SpecialMenu/Grid01").gameObject;

        m_Privilege_Button = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_Tequan").GetComponent<Button>();


        m_BtnBattlePass = mTrans.Find("Animator/View_ServerActivity/Grid01/Button_BattlePass").GetComponent<Button>();
        m_TransBattlePassRedDot = m_BtnBattlePass.transform.Find("Image_Dot");

        View_RedPacket = mTrans.Find("Animator/View_RedPacket").gameObject;
        Button_RedpPacket = mTrans.Find("Animator/View_RedPacket/Button_RedpPacket").GetComponent<Button>();

        m_TrialGateSkillBtn= mTrans.Find("Animator/View_Icon/Grid_Buff/Button_TrialGateSkill").GetComponent<Button>();
        m_BossTowerBtn = mTrans.Find("Animator/View_Icon/Grid_Buff/Button_BossGame").GetComponent<Button>();

        m_BtnBless = mTrans.Find("Animator/View_ServerActivity/Grid01/Button_Blessing").GetComponent<Button>();
        m_TransBlessRedDot = m_BtnBless.transform.Find("Image_Dot");
    }

    public void RegisterEvents(IListener listener)
    {
        Btn_Close_Normal.onClick.AddListener(listener.OnClose_ButtonClicked);
        Btn_Close_Probe.onClick.AddListener(listener.OnProbeClose_ButtonClicked);
        Btn_Open.onClick.AddListener(listener.OnOpen_ButtonClicked);
        Btn_Change.onClick.AddListener(listener.OnChange_ButtonClicked);
        Btn_CookMain.onClick.AddListener(listener.OnCook_ButtonClicked);
        mapMailGo.onClick.AddListener(listener.OnBtnClickedReadMapMail);
        Button_FamilyEesBattleSignupTip.onClick.AddListener(listener.OnButtonClicked_FamilyEesBattleSignupTip);
        Btn_Partner.onClick.AddListener(listener.OnPartner_ButtonClicked);
        Btn_Role.onClick.AddListener(listener.OnRole_ButtonClicked);
        Btn_Header.onClick.AddListener(listener.OnHeader_ButtonClicked);
        Btn_Task.onClick.AddListener(listener.OnTaskOrTeam_ButtonClicked);
        Btn_Bag.onClick.AddListener(listener.OnBag_ButtonClicked);
        Btn_TempBag.onClick.AddListener(listener.OnTempBag_ButtonClicked);
        Btn_LifeSkill.onClick.AddListener(listener.OnBtn_LifeSkill_ButtonClicked);
        Btn_SkillLearn.onClick.AddListener(listener.OnSkill_ButtonClicked);
        Btn_Equip.onClick.AddListener(listener.OnEquip_ButtonClicked);
        //Btn_Treasure.onClick.AddListener(listener.OnTreasure_ButtonCliked);
        //Btn_ClueTask.onClick.AddListener(listener.OnBtn_ClueTask_ButtonClicked);

        Btn_NewPet.onClick.AddListener(listener.OnBtn_NewPet_ButtonClicked);

        //Btn_Single.onClick.AddListener(listener.OnBtn_Single_ButtonClicked); //-------删除
        //Btn_Multi.onClick.AddListener(listener.OnBtn_Multi_ButtonClicked); //-------删除
        //Btn_AreaProtection.onClick.AddListener(listener.OnBtn_AreaProtection_ButtonClicked); //-------删除
        //Btn_Favorability.onClick.AddListener(listener.OnBtn_Favorability_ButtonClicked);
        Btn_Family.onClick.AddListener(listener.OnBtn_Family_ButtonClicked);
        m_Cooking.onClick.AddListener(listener.OnBtn_Cook_ButtonClicked);

        Btn_Repair.onClick.AddListener(listener.OnBtn_Repair_ButtonClicked);
        //Btn_KnowledgeTip.onClick.AddListener(listener.OnBtn_KnowledgeTip_ButtonClicked);
        Btn_ExplorationTip.onClick.AddListener(listener.OnBtn_ExplorationTip_ButtonClicked);
        Btn_Weather.onClick.AddListener(listener.OnBtn_Weather_ButtonClicked);
        //Btn_Eraser.onClick.AddListener(listener.OnBtn_EraserClicked);

        Btn_Old.onClick.AddListener(listener.OnBtn_OldClicked);
        Btn_Eye.onClick.AddListener(listener.OnBtn_EyeClicked);
        Btn_Look.onClick.AddListener(listener.OnBtn_LookClicked);

        Btn_TempPetBag.onClick.AddListener(listener.OnBtn_TempPetBagClicked);
        //Btn_Energyspar.onClick.AddListener(listener.OnBtn_EnergysparClicked); //-------删除
        Btn_Uplifted.onClick.AddListener(listener.OnBtn_UpliftedClicked);
        Btn_Crystal.onClick.AddListener(listener.OnBtn_CrystalClicked);

        m_BtnDaily.onClick.AddListener(listener.OnBtn_DailyClicked);
        Btn_Hangup.onClick.AddListener(listener.OnBtn_HangupClicked);
        Btn_HundreadPeopelBuff.onClick.AddListener(listener.OnBtn_HundreadPeopelBuffClicked);
        Btn_MessageBag.onClick.AddListener(listener.OnBtn_MessageBag);
        m_BtnServivalPvp.onClick.AddListener(listener.OnBtn_ServivalPvpClicked);
        m_SpecialBtn.onClick.AddListener(listener.OnBtn_SpecialBtnClicked);
        m_Privilege_Button.onClick.AddListener(listener.OnBtn_PrivilegeBtnClicked);

        //Btn_TravellerAwakening.onClick.AddListener(listener.OnBtn_TravellerAwakeningBtnClicked); //-------删除


        m_BtnBattlePass.onClick.AddListener(listener.OnBtn_BattlePassBtnClicked);


        Button_RedpPacket.onClick.AddListener(listener.OnBtn_OpenFamilyRedPacket);

        m_TrialGateSkillBtn.onClick.AddListener(listener.OnBtn_OpenTrialGateSkill);
        m_BossTowerBtn.onClick.AddListener(listener.OnBtn_OpenBossTower);

        m_BtnBless.onClick.AddListener(listener.OnBtn_Bless);
    }

    public interface IListener
    {
        void OnClose_ButtonClicked();
        void OnBag_ButtonClicked();
        void OnTempBag_ButtonClicked();
        void OnSafeBox_ButtonClicked();
        void OnPartner_ButtonClicked();
        void OnRole_ButtonClicked();
        void OnButtonClicked_FamilyEesBattleSignupTip();
        void OnHeader_ButtonClicked();
        void OnTaskOrTeam_ButtonClicked();
        void OnSkill_ButtonClicked();
        void OnEquip_ButtonClicked();
        void OnTreasure_ButtonCliked();
        void OnBtn_ClueTask_ButtonClicked();
        void OnBtn_NewPet_ButtonClicked();
        void OnBtn_Multi_ButtonClicked();
        void OnBtn_AreaProtection_ButtonClicked();
        void OnBtn_Favorability_ButtonClicked();
        void OnBtn_Family_ButtonClicked();
        void OnBtn_Single_ButtonClicked();
        void OnBtn_Repair_ButtonClicked();
        //void OnBtn_KnowledgeTip_ButtonClicked();
        void OnBtn_ExplorationTip_ButtonClicked();
        void OnBtn_Weather_ButtonClicked();
        void OnBtn_LifeSkill_ButtonClicked();
        void OnChange_ButtonClicked();
        void OnCook_ButtonClicked();
        void OnBtnClickedReadMapMail();
        void OnProbeClose_ButtonClicked();
        void OnOpen_ButtonClicked();
        void OnBtn_OldClicked();
        void OnBtn_EyeClicked();
        void OnBtn_LookClicked();
        void OnBtn_TempPetBagClicked();
        void OnBtn_EnergysparClicked();
        void OnBtn_UpliftedClicked();
        void OnBtn_CrystalClicked();
        void OnBtn_DailyClicked();
        void OnBtn_HangupClicked();
        void OnBtn_HundreadPeopelBuffClicked();
        void OnBtn_MessageBag();
        void OnBtn_ServivalPvpClicked();
        void OnBtn_SpecialBtnClicked();
        void OnBtn_PrivilegeBtnClicked();

        void OnBtn_TravellerAwakeningBtnClicked();


        void OnBtn_BattlePassBtnClicked();

        void OnBtn_OpenFamilyRedPacket();
        void OnBtn_Cook_ButtonClicked();
        void OnBtn_OpenTrialGateSkill();
        void OnBtn_OpenBossTower();

        void OnBtn_Bless();
    }

    public class NearByNpcItem
    {
        public Npc npcInfo;

        public GameObject root;
        public Text npcName;

        public NearByNpcItem(GameObject gameObject)
        {
            root = gameObject;

            npcName = root.FindChildByName("Text").GetComponent<Text>();
            root.GetComponent<Button>().onClick.AddListener(OnClickButton);
        }

        public void SetData(Npc npc)
        {
            npcInfo = npc;
            TextHelper.SetText(npcName, LanguageHelper.GetNpcTextContent(npcInfo.cSVNpcData.name));
        }

        public void Dispose()
        {
            npcInfo = null;
            npcName.text = string.Empty;
            root.SetActive(false);
        }

        void OnClickButton()
        {
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcInfo);
        }
    }
}
