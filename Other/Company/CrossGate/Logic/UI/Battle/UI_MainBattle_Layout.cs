 using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_MainBattle_Layout
    {
        public Transform transform;
        public Text Text_Time;
        public Text Text_Time_Auto;
        public Text Text_Tips;
        public Text Text_Round;
        public Text Text_Skill_Name;
        public Text Text_Tips_01;
        public Text Text_BossName;
        public Text Text_SpeedUp;
        public Text Text_Auto_Round;

        public Image Image_Icon;
        public Image Image_NormalAttack_Icon;
        public Image Image_BossBuff;
        public Image exp;

        public Slider Slider_BossHp;
        public Slider Slider_BossHpMiddle;     

        public Button Button_Close;
        public Button Button_Exit;
        public Button Button_Team;
        public Button Button_Menu_Close;
        public Button Button_Revoke;
        public Button Button_SpeedUp;
        public Button Button_StopHangup;
        public Button Button_Hundred;
        public GameObject hundredBufferRed;
        public GameObject hundredBufferBlue;

        public GameObject View_Icon;
        public GameObject View_Right_Menu;
        public GameObject View_Time;
        public GameObject View_TimeRoot;
        public GameObject View_CloseShow;
        public GameObject View_Auto_Battle;
        public GameObject Fix_Round;
        public GameObject Fix_Tips;
        public GameObject Go_Mainbattle;
        public GameObject Go_BossHp;
        public GameObject Go_BossBuff;
        public GameObject Go_Revoke;
        public GameObject Go_RoundShow;
        public GameObject Go_RoundShowNum;

        public Animator Animator_Tips;
        public Button btnCamp;
        public GameObject btnCampRed;

        //战斗指挥
        public Button m_BtnCommand;
        public Button m_BtnCommandCancle;
        public Transform m_FastCommand;

        //观战录像站
        public GameObject Go_Video;
        public GameObject Go_Video_Speed;
        public GameObject Go_Video_Func;
        public GameObject Go_Watch_Tip;
        public GameObject Go_BulletChat;
        public GameObject Go_BulletView;
        public GameObject Go_Bullet_Open;
        public GameObject Go_Bullet_Close;
        public Button Button_Video_Close;
        public Button Button_Stop;
        public Button Button_Speed;
        public Button Button_Slow;
        public Button Button_Barrage;
        public Button Button_Barrage_Back;
        public Button Button_Barrage_Send;
        public Toggle Toggle_Barrage;
        public Toggle Toggle_Like;
        public Toggle Toggle_Mark;
        public InputField InputField;
        public Image Image_PlayOrStop;

        public Text Text_Prompt_Name;
        public Button Button_Prompt;
        public Button Button_Element;
        public Button Button_Sequence;

        public CP_TransformContainer container;
        public Canvas canvas;

        //试炼之门战斗阶段显示
        public Text Text_Satge;
        public Button Button_TrialGate;
        public Transform TrialGate_Root;
        public Button Button_TrialGateSkill;

        public void Init(Transform transform)
        {
            this.transform = transform;

            container = transform.GetComponent<CP_TransformContainer>();
            btnCamp = transform.Find("Animator/Button_Camp").GetComponent<Button>();
            btnCampRed = transform.Find("Animator/Button_Camp/Image_Red").gameObject;

            Text_Time = transform.Find("Animator/View_Time/Image_BG/Text_Time").GetComponent<Text>();
            Text_Time_Auto = transform.Find("Animator/View_Time/Image_BG/Text_Time_Auto").GetComponent<Text>();
            Text_Tips = transform.Find("Animator/View_Time/Image_BG01/Text_Tips").GetComponent<Text>();
            Text_Tips_01 = transform.Find("Animator/View_Time/Image_BG01/Text_Tips/Text_Tips_01").GetComponent<Text>();
            Text_Round = transform.Find("Animator/View_Left/Text_Round").GetComponent<Text>();
            Text_Skill_Name = transform.Find("Animator/View_CloseShow/Text_Skill_Name").GetComponent<Text>();
            Text_BossName = transform.Find("Animator/View_BossBlood/Text_Percent").GetComponent<Text>();
            Text_SpeedUp = transform.Find("Animator/Button_Speed/Text").GetComponent<Text>();
            Text_Auto_Round = transform.Find("Animator/View_Round/Image_BG/Text_Time").GetComponent<Text>();

            Image_BossBuff = transform.Find("Animator/View_BossBlood/BuffGrid/icon").GetComponent<Image>();
            Image_Icon = transform.Find("Animator/View_CloseShow/Image_Skill_Icon").GetComponent<Image>();
            Image_NormalAttack_Icon = transform.Find("Animator/View_Right_Menu/Grid01/Attack/Button_Attack").GetComponent<Image>();
            exp = transform.Find("Animator/View_ExpProgress/Image_Exp").GetComponent<Image>();
            Slider_BossHp = transform.Find("Animator/View_BossBlood/Slider02").GetComponent<Slider>();
            Slider_BossHpMiddle = transform.Find("Animator/View_BossBlood/Slider01").GetComponent<Slider>();

            Button_Close = transform.Find("Animator/View_CloseShow/Button_Close").GetComponent<Button>();
            Button_Exit = transform.Find("Animator/View_Left/Button_Exit").GetComponent<Button>();
            Button_Team = transform.Find("Animator/Button_Team").GetComponent<Button>();
            m_BtnCommand = transform.Find("Animator/Command/Button_Goods").GetComponent<Button>();
            m_BtnCommandCancle = transform.Find("Animator/Button_Cancel").GetComponent<Button>();
            Button_Menu_Close = transform.Find("Animator/Close/Btn_Close").GetComponent<Button>();
            Button_Revoke = transform.Find("Animator/View_Revoke/Btn_Revoke").GetComponent<Button>();
            Button_SpeedUp = transform.Find("Animator/Button_Speed").GetComponent<Button>();
            Button_StopHangup = transform.Find("Animator/Button_StopHangup").GetComponent<Button>();
            Button_Hundred = transform.Find("Animator/Button_Hundred").GetComponent<Button>();
            hundredBufferRed = transform.Find("Animator/Button_Hundred/Red").gameObject;
            hundredBufferBlue = transform.Find("Animator/Button_Hundred/Blue").gameObject;

            View_CloseShow = transform.Find("Animator/View_CloseShow").gameObject;
            Go_Mainbattle = transform.Find("Animator").gameObject;
            View_Icon = transform.Find("Animator/View_Icon").gameObject;
            View_Right_Menu = transform.Find("Animator/View_Right_Menu").gameObject;
            View_Right_Menu.SetActive(false);
            View_Time = transform.Find("Animator/View_Time/Image_BG").gameObject;
            View_TimeRoot = transform.Find("Animator/View_Time").gameObject;
            View_TimeRoot.SetActive(true);
            View_Auto_Battle = transform.Find("Animator/View_Auto_Battle").gameObject;
            Fix_Round = transform.Find("Animator/View_Left/Text_Round/Fx_ui_Round").gameObject;
            Fix_Tips = transform.Find("Animator/View_Time/Image_BG01/Text_Tips/Fx_ui_MainBattle").gameObject;
            Go_BossHp = transform.Find("Animator/View_BossBlood").gameObject;
            Go_BossBuff = transform.Find("Animator/View_BossBlood/BuffGrid").gameObject;
            Go_Revoke = transform.Find("Animator/View_Revoke").gameObject;
            Go_RoundShow = transform.Find("Animator/View_Round").gameObject;
            Go_RoundShowNum = transform.Find("Animator/View_Round/Image_BG").gameObject;
            Go_RoundShow.gameObject.SetActive(false);
            Go_Watch_Tip = transform.Find("Animator/Image_Watching").gameObject;
            Go_Watch_Tip.gameObject.SetActive(false);

            Animator_Tips = transform.Find("Animator/View_Time/Image_BG01/Text_Tips").GetComponent<Animator>();
            m_FastCommand = transform.Find("Animator/View_Shortcut");

            Go_Video = transform.Find("Animator/View_Viedo").gameObject;
            Go_Video_Func = transform.Find("Animator/View_Viedo/BtnGroup02").gameObject;
            Go_Video_Speed = transform.Find("Animator/View_Viedo/BtnGroup01").gameObject;
            Go_BulletChat = transform.Find("Animator/View_BulletChat").gameObject;
            Go_BulletView = transform.Find("Animator/View_BulletRoot").gameObject;
            Go_Bullet_Open = transform.Find("Animator/View_Viedo/BtnGroup02/Toggle_Open/Open").gameObject;
            Go_Bullet_Close = transform.Find("Animator/View_Viedo/BtnGroup02/Toggle_Open/Cloce").gameObject;

            InputField = transform.Find("Animator/View_BulletChat/InputField").GetComponent<InputField>();
            Image_PlayOrStop = transform.Find("Animator/View_Viedo/BtnGroup01/Btn_Suspend").GetComponent<Image>();
            Button_Video_Close =transform.Find("Animator/View_Viedo/Btn_Close").GetComponent<Button>();
            Button_Stop = transform.Find("Animator/View_Viedo/BtnGroup01/Btn_Suspend").GetComponent<Button>();
            Button_Speed = transform.Find("Animator/View_Viedo/BtnGroup01/Btn_Next").GetComponent<Button>();
            Button_Slow = transform.Find("Animator/View_Viedo/BtnGroup01/Btn_Last").GetComponent<Button>();
            Button_Barrage = transform.Find("Animator/View_Viedo/BtnGroup02/Btn_BulletChat").GetComponent<Button>();
            Button_Barrage_Back = transform.Find("Animator/View_BulletChat/Btn_Back").GetComponent<Button>();
            Button_Barrage_Send = transform.Find("Animator/View_BulletChat/Btn_01").GetComponent<Button>();

            Toggle_Barrage = transform.Find("Animator/View_Viedo/BtnGroup02/Toggle_Open").GetComponent<Toggle>();
            Toggle_Like = transform.Find("Animator/View_Viedo/BtnGroup02/Toggle_Like").GetComponent<Toggle>();
            Toggle_Mark = transform.Find("Animator/View_Viedo/BtnGroup02/Toggle_Collect").GetComponent<Toggle>();

            Text_Prompt_Name = transform.Find("Animator/View_BattleName/Text_Name").GetComponent<Text>();
            Button_Prompt = transform.Find("Animator/View_BattleName/Button_Element_Restrain").GetComponent<Button>();
            Button_Element = transform.Find("Animator/View_Left/Button_Element_Restrain").GetComponent<Button>();

            Button_Sequence = transform.Find("Animator/View_Sequence/Btn_Open").GetComponent<Button>();
            canvas = transform.GetComponent<Canvas>();
            Text_Satge = transform.Find("Animator/Button_TrialGate/Text").GetComponent<Text>();
            Button_TrialGate = transform.Find("Animator/Button_TrialGate").GetComponent<Button>();
            TrialGate_Root = transform.Find("Animator/Button_TrialGate/Fire").transform;
            Button_TrialGateSkill = transform.Find("Animator/Button_TrialGateSkill").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            Button_Close.onClick.AddListener(listener.OnButton_CloseTargetClicked);
            Button_Exit.onClick.AddListener(listener.OnButton_ExitClicked);
            Button_Team.onClick.AddListener(listener.OnButton_Team);

            m_BtnCommand.onClick.AddListener(listener.OnButton_Command);
            m_BtnCommandCancle.onClick.AddListener(listener.OnButton_CommandCancle);
            Button_Menu_Close.onClick.AddListener(listener.OncloseBtnClicked);
            Button_Revoke.onClick.AddListener(listener.OnButton_Revoke);
            Button_SpeedUp.onClick.AddListener(listener.OnButton_SpeedUp);
            Button_StopHangup.onClick.AddListener(listener.OnButton_StopHangup);
            Button_Hundred.onClick.AddListener(listener.OnButton_Hundred);
            btnCamp.onClick.AddListener(listener.OnBtnCampClicked);

            Button_Video_Close.onClick.AddListener(listener.OnButton_Video_Close_Clicked);
            Button_Stop.onClick.AddListener(listener.OnButton_Stop_Clicked);
            Button_Speed.onClick.AddListener(listener.OnButton_Speed_Clicked);
            Button_Slow.onClick.AddListener(listener.OnButton_Slow_Clicked);
            Button_Barrage.onClick.AddListener(listener.OnButton_Barrage_Clicked);
            Button_Barrage_Back.onClick.AddListener(listener.OnButton_Barrage_Back_Clicked);
            Button_Barrage_Send.onClick.AddListener(listener.OnButton_Barrage_Send_Clicked);
            Toggle_Barrage.onValueChanged.AddListener(listener.OnToggle_Barrage_ValueChanged);
            Toggle_Like.onValueChanged.AddListener(listener.OnToggle_Like_ValueChanged);
            Toggle_Mark.onValueChanged.AddListener(listener.OnToggle_Mark_ValueChanged);

            Button_Prompt.onClick.AddListener(listener.OnButton_Prompt_Clicked);
            Button_Element.onClick.AddListener(listener.OnButton_Element_Clicked);
            Button_Sequence.onClick.AddListener(listener.OnButton_Sequence_Clicked);
            Button_TrialGate.onClick.AddListener(listener.OnButton_TrialGate_Clicked);
            Button_TrialGateSkill.onClick.AddListener(listener.OnButton_TrialGateSkill_Clicked);
        }

        public interface IListener
        {
            void OnButton_CloseTargetClicked();
            void OnButton_ExitClicked();
            void OnButton_Team();

            void OnButton_Command();
            void OnButton_CommandCancle();
            void OncloseBtnClicked();
            void OnButton_Revoke();
            void OnBtnCampClicked();
            void OnButton_SpeedUp();
            void OnButton_StopHangup();
            void OnButton_Hundred();

            void OnButton_Video_Close_Clicked();
            void OnButton_Stop_Clicked();
            void OnButton_Speed_Clicked();
            void OnButton_Slow_Clicked();
            void OnButton_Barrage_Clicked();
            void OnToggle_Barrage_ValueChanged(bool isOn);
            void OnToggle_Like_ValueChanged(bool isOn);
            void OnToggle_Mark_ValueChanged(bool isOn);        
            void OnButton_Prompt_Clicked();
            void OnButton_Element_Clicked();
            void OnButton_Sequence_Clicked();
            void OnButton_TrialGate_Clicked();
            void OnButton_TrialGateSkill_Clicked();
            void OnButton_Barrage_Back_Clicked();
            void OnButton_Barrage_Send_Clicked();
        }

        public void SetCommandCancleActive(bool b)
        {
            if (b != m_BtnCommandCancle.gameObject.activeSelf)
                m_BtnCommandCancle.gameObject.SetActive(b);
        }
    }
}
