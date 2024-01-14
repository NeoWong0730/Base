using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_PromptBox : UIBase
    {
        private Text m_ContentText = null;

        private Text m_CostTip;
        private Image m_CostIcon;
        private Text m_CostNum;

        private Button m_BtnCancel = null;
        private Text m_TextCancel = null;

        private Button m_BtnConfirm = null;
        private Text m_TextConfirm = null;

        private GameObject m_TitleGo;
        private GameObject m_FirstTipsGo;
        private Text m_TitleText;

        private Toggle m_NoTipsToggle;

        private Action m_ActionCancle = null;
        private Action m_ActionConfirm = null;
        private Action<bool> m_ActionToggle = null;
        //private Action m_ActionOther = null;

        private float m_time = 0;

        private bool isFinish = false;

        PromptBoxParameter parameter = new PromptBoxParameter();

        protected override void OnLoaded()
        {
            ParseComponent();
        }

        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_Hint.Instance.eventEmitter.Handle(Sys_Hint.EEvents.RefreshPromptBoxData, UpdateView, toRegister);
        }

        protected override void OnOpen(object arg1 = null)
        {
            parameter.Copy(arg1 as PromptBoxParameter);
        }
        public override void OnSetData(object arg)
        {
            parameter.Copy(arg as PromptBoxParameter);
            UpdateView();
        }
        protected override void OnShow()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (parameter == null)
                return;
            if (PromptBoxParameter.Instance.needDingTo)
            {
                parameter.Copy(PromptBoxParameter.Instance);
            }
            isFinish = false;

            m_BtnConfirm.gameObject.SetActive(parameter.useConfirm);
            m_BtnCancel.gameObject.SetActive(parameter.useCancel);
            m_FirstTipsGo.SetActive(parameter.useToggle);

            if (parameter.tipType == PromptBoxParameter.TipType.Text)
            {
                m_ContentText.text = parameter.content;
                m_TitleGo.gameObject.SetActive(false);
                m_CostTip.gameObject.SetActive(false);
            }
            else if (parameter.tipType == PromptBoxParameter.TipType.Item)
            {
                m_ContentText.text = parameter.content;

                m_TitleGo.gameObject.SetActive(false);
                m_CostTip.gameObject.SetActive(true);
                ImageHelper.SetIcon(m_CostIcon, CSVItem.Instance.GetConfData(parameter.itemId).icon_id);
                uint costColorId = Sys_Bag.Instance.GetItemCount(parameter.itemId) >= parameter.itemNum ? (uint)2007201 : 2007202;
                m_CostNum.text = LanguageHelper.GetLanguageColorWordsFormat(parameter.itemNum.ToString(), costColorId);
            }
            else if (parameter.tipType == PromptBoxParameter.TipType.TitleText)
            {
                m_ContentText.text = parameter.content;
                m_TitleGo.gameObject.SetActive(true);
                m_TitleText.text = parameter.title;
                m_CostTip.gameObject.SetActive(false);
            }

            TextHelper.SetText(m_TextConfirm, parameter.confirmTextId > 0 ? parameter.confirmTextId : 1000906);
            TextHelper.SetText(m_TextCancel, parameter.cancleTextId > 0 ? parameter.cancleTextId : 1000905);

            m_ActionConfirm = parameter.onConfirm;

            m_ActionCancle = parameter.onCancel;

            m_ActionToggle = parameter.onToggle;

            m_NoTipsToggle.isOn = parameter.isToggleChecked;

            if (parameter.useOther)
            {
                //m_ActionOther = parameter.onOther;
                //uint tempOtherTextId = parameter.otherTextId != 0 ? parameter.otherTextId : 40303;
                //LanguageManager.Instance.GetText(m_TextConfirm, tempOtherTextId);
            }

            if (parameter.eCountDownType != PromptBoxParameter.ECountDownType.None)
            {
                m_time = parameter.time;

                if (parameter.eCountDownType == PromptBoxParameter.ECountDownType.SetEnable)
                {
                    SetButtonCountDownState(false);
                }
            }
        }
        protected override void OnUpdate()
        {
            if ((parameter.eCountDownType != PromptBoxParameter.ECountDownType.None) && m_time > 0)
            {
                float lasttime = m_time;
                m_time -= deltaTime;

                int intlt = (int)lasttime;
                int intnt = (int)m_time;

                if (intlt != intnt)
                {
                    if (parameter.eCountdown == PromptBoxParameter.ECountdown.Confirm)
                        TextHelper.SetText(m_TextConfirm, parameter.confirmTextId > 0 ? parameter.confirmTextId : 1000906, intnt.ToString());

                    if (parameter.eCountdown == PromptBoxParameter.ECountdown.Cancel)
                        TextHelper.SetText(m_TextCancel, parameter.cancleTextId > 0 ? parameter.cancleTextId : 1002501, intnt.ToString());
                }

                if (m_time <= 0)
                {
                    OnTimeOver();
                }
            }            
        }
        private void ParseComponent()
        {
            m_ContentText = transform.Find("Animator/Text_Tip").GetComponent<Text>();

            m_CostTip = transform.Find("Animator/Text_Cost").GetComponent<Text>();
            m_CostIcon = transform.Find("Animator/Text_Cost/Image_Icon").GetComponent<Image>();
            m_CostNum = transform.Find("Animator/Text_Cost/Text_Num").GetComponent<Text>();

            m_BtnConfirm = transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            m_TextConfirm = transform.Find("Animator/Buttons/Button_Sure/Text").GetComponent<Text>();

            m_BtnCancel = transform.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            m_TextCancel = transform.Find("Animator/Buttons/Button_Cancel/Text").GetComponent<Text>();

            m_TitleGo = transform.Find("Animator/Image_Titlebg01").gameObject;
            m_TitleText = transform.Find("Animator/Image_Titlebg01/Text_Title").GetComponent<Text>();

            m_FirstTipsGo = this.transform.Find("Animator/View_Toggle").gameObject;
            m_NoTipsToggle = this.transform.Find("Animator/View_Toggle/Toggle_Read").GetComponent<Toggle>();

            m_BtnCancel.onClick.AddListener(OnBtnCancel);
            m_BtnConfirm.onClick.AddListener(OnBtnConfirm);
            m_NoTipsToggle.onValueChanged.AddListener(OnValueChanged);
            //m_BtnOther.onClick.AddListener(OnBtnOther);
        }

        private void OnBtnCancel()
        {

            UIManager.CloseUI(EUIID.UI_PromptBox, true);

            isFinish = true;
            m_ActionCancle?.Invoke();
        }

        private void OnBtnConfirm()
        {

            UIManager.CloseUI(EUIID.UI_PromptBox, true);

            isFinish = true;
            m_ActionConfirm?.Invoke();
        }

        private void OnValueChanged(bool value)
        {
            m_ActionToggle?.Invoke(value);
        }


        private void OnBtnOther()
        {
            //UIManager.CloseUI(EUIID.UI_PromptBox, false);
            //if (m_ActionOther != null)
            //{
            //    m_ActionOther();
            //}
        }

        protected override void OnHide()
        {
            if (!isFinish)
            {
                parameter.onNoOperator?.Invoke();
            }
            PromptBoxParameter.Instance.ClearOnHideOrDestroy();
            //parameter.Clear();
            //m_ActionCancle = null;
            //m_ActionConfirm = null;


            // m_ActionOther = null;
        }

        protected override void OnDestroy()
        {
            PromptBoxParameter.Instance.ClearOnHideOrDestroy();
        }
        public void RefreshToggleChecked()
        {
            m_NoTipsToggle.isOn = parameter.isToggleChecked;
        }

        private void OnTimeOver()
        {
            if(parameter.eCountDownType == PromptBoxParameter.ECountDownType.AutoTrigger)
            {
                if (parameter.eCountdown == PromptBoxParameter.ECountdown.Cancel)
                    OnBtnCancel();

                if (parameter.eCountdown == PromptBoxParameter.ECountdown.Confirm)
                    OnBtnConfirm();
            }
            else
            {
                SetButtonCountDownState(true);
            }
        }

        private void SetButtonCountDownState(bool enable)
        {
            switch(parameter.eCountdown)
            {
                case PromptBoxParameter.ECountdown.Confirm:
                    ButtonHelper.Enable(m_BtnConfirm, enable);                   
                    break;
                case PromptBoxParameter.ECountdown.Cancel:
                    ButtonHelper.Enable(m_BtnCancel, enable);
                    break;
                default:
                    break;
            }
            if(enable)
            {
                TextHelper.SetText(m_TextConfirm, 1000906);
                TextHelper.SetText(m_TextCancel, 1000905);
            }
        }
    }

    public class PromptBoxParameter : Singleton<PromptBoxParameter>
    {
        public enum TipType
        {
            Text,
            Item,
            TitleText
        }

        public TipType tipType = TipType.Text;

        public string content = string.Empty;
        public string title = string.Empty;
        public uint itemId = 0;
        public uint itemNum = 0;

        public bool useConfirm = false;
        public bool useOther = false;
        public bool useCancel = false;
        public bool useToggle = false;

        public bool isToggleChecked = true;

        public Action onConfirm = null;
        public Action onOther = null;
        public Action onCancel = null;
        public Action onNoOperator = null;//没有操作回调
        public Action<bool> onToggle = null;

        public uint confirmTextId = 0;
        public uint otherTextId = 0;
        public uint cancleTextId = 0;

        public float time = 0;
        private ECountdown _eCountdown = ECountdown.None;
        private EPriorityType _ePriority = EPriorityType.None;
        public bool needDingTo = false;//是否需要被顶替
        /// <summary>
        /// 设置ECountdown类型统一调用函数SetCountdown
        /// </summary>
        public ECountdown eCountdown
        {
            get
            {
                return _eCountdown;
            }

            private set { }
        }

        /// <summary> 倒计时指定类型 </summary>
        public enum ECountdown
        {
            None,
            /// <summary> 确定按钮 </summary>
            Confirm,
            /// <summary> 取消按钮 </summary>
            Cancel
        }

        public ECountDownType _eCountDownType = ECountDownType.None;

        /// <summary>
        /// 设置ECountDownType类型统一调用函数SetCountdown
        /// </summary>
        public ECountDownType eCountDownType
        {
            get
            {
                return _eCountDownType;
            }

            private set { }
        }

        /// <summary> 倒计时确认框类型 </summary>
        public enum ECountDownType
        {
            None,
            /// <summary> 倒计时自动触发 </summary>
            AutoTrigger,
            /// <summary> 按钮设置灰态无法点击 </summary>
            SetEnable
        }

        /// <summary> 弹窗优先级，数字越大优先级越高 </summary>
        public enum EPriorityType
        {
            /// <summary> 没有优先级 </summary>
            None = 0,
            /// <summary> 家族兽蛋 </summary>
            FamilyPetEgg = 1,
            /// <summary> 家族酒会 </summary>
            FamilyParty = 2,
            /// <summary> 家族资源战 </summary>
            FamilyResBattle = 3,
            /// <summary> 训练家族兽 </summary>
            FamilyPetTraining = 4,
        }

        public void SetConfirm(bool use, Action action, uint nameID = 0)
        {
            useConfirm = use;
            onConfirm = action;
            confirmTextId = nameID;
        }

        public void SetOther(bool use, Action action, uint nameID = 0)
        {
            useOther = use;
            onOther = action;
            otherTextId = nameID;
        }

        public void SetCancel(bool use, Action action, uint nameID = 0)
        {
            useCancel = use;
            onCancel = action;
            cancleTextId = nameID;
        }

        public void SetCountdown(float ntime, ECountdown countdown, ECountDownType countdownType = ECountDownType.AutoTrigger)
        {
            time = ntime;
            _eCountDownType = countdownType;
            _eCountdown = countdown;
        }

        public void SetToggleChanged(bool use, Action<bool> action)
        {
            useToggle = use;
            onToggle = action;
        }

        public void SetToggleChecked(bool value)
        {
            isToggleChecked = value;
        }

        public void Clear()
        {
            tipType = TipType.Text;

            content = string.Empty;
            title = string.Empty;
            itemId = 0;
            itemNum = 0;

            useConfirm = false;
            useOther = false;
            useCancel = false;
            useToggle = false;

            onConfirm = null;
            onOther = null;
            onCancel = null;
            onNoOperator = null;
            onToggle = null;

            confirmTextId = 0;
            otherTextId = 0;
            cancleTextId = 0;
            time = 0;
            _eCountdown = ECountdown.None;
            _eCountDownType = ECountDownType.None;
            _ePriority = EPriorityType.None;
            needDingTo = false;
        }
        /// <summary> 界面隐藏或销毁是需要清理的数据 </summary>
        public void ClearOnHideOrDestroy()
        {
            _ePriority = EPriorityType.None;
        }
        public void Copy(PromptBoxParameter other)
        {
            if (other == null)
            {
                return;
            }

            tipType = other.tipType;
            itemId = other.itemId;
            itemNum = other.itemNum;

            content = other.content;
            title = other.title;
            useConfirm = other.useConfirm;
            useOther = other.useOther;
            useCancel = other.useCancel;
            useToggle = other.useToggle;
            isToggleChecked = other.isToggleChecked;
            onConfirm = other.onConfirm;
            onOther = other.onOther;
            onCancel = other.onCancel;
            onToggle = other.onToggle;
            confirmTextId = other.confirmTextId;
            otherTextId = other.otherTextId;
            cancleTextId = other.cancleTextId;

            //倒计时设置
            time = other.time;
            _eCountdown = other.eCountdown;
            _eCountDownType = other.eCountDownType;
            _ePriority = other._ePriority;

            onNoOperator = other.onNoOperator;

            needDingTo = false;
        }

        public void OpenPromptBox(uint contentId, uint titleId = 0, Action confirm = null, Action cancel = null)
        {
            Instance.Clear();
            Instance.title = Table.CSVLanguage.Instance.GetConfData(titleId == 0 ? 5520 : titleId)?.words;
            Instance.content = Table.CSVLanguage.Instance.GetConfData(contentId)?.words;
            Instance.SetConfirm(true, () =>
            {
                confirm?.Invoke();
            });

            Instance.SetCancel(true, () =>
            {
                cancel?.Invoke();
            });

            UIManager.OpenUI(EUIID.UI_PromptBox, false, Instance);
        }

        public void OpenPromptBox(uint contentId, uint titleId = 0, bool isChecked = true, Action confirm = null, Action cancel = null , Action<bool> toggleChanged = null)
        {
            Instance.Clear();
            Instance.title = Table.CSVLanguage.Instance.GetConfData(titleId == 0 ? 5520 : titleId)?.words;
            Instance.content = Table.CSVLanguage.Instance.GetConfData(contentId)?.words;
            Instance.SetConfirm(true, () =>
            {
                confirm?.Invoke();
            });

            Instance.SetCancel(true, () =>
            {
                cancel?.Invoke();
            });

            Instance.SetToggleChanged(true, (value) =>
            {
                toggleChanged?.Invoke(value);
            });

            Instance.SetToggleChecked(isChecked);

            UIManager.OpenUI(EUIID.UI_PromptBox, false, Instance);
        }

        public void OpenPromptBox(string content, uint titleId = 0, bool isChecked = true, Action confirm = null, Action cancel = null, Action<bool> toggleChanged = null)
        {
            Instance.Clear();
            Instance.title = Table.CSVLanguage.Instance.GetConfData(titleId == 0 ? 5520 : titleId)?.words;
            Instance.content = content;
            Instance.SetConfirm(true, () =>
            {
                confirm?.Invoke();
            });

            Instance.SetCancel(true, () =>
            {
                cancel?.Invoke();
            });
            Instance.SetToggleChanged(true, (value) =>
            {
                toggleChanged?.Invoke(value);
            });

            Instance.SetToggleChecked(isChecked);

            UIManager.OpenUI(EUIID.UI_PromptBox, false, Instance);
        }

        public void OpenPromptBox(string content, uint titleId = 0, Action confirm = null, Action cancel = null, ECountdown eCountdown = ECountdown.None, float countDownTime = 0)
        {
            Instance.Clear();
            Instance.title = Table.CSVLanguage.Instance.GetConfData(titleId == 0 ? 5520 : titleId)?.words;
            Instance.content = content;
            Instance.SetConfirm(true, () =>
            {
                confirm?.Invoke();
            });

            Instance.SetCancel(true, () =>
            {
                cancel?.Invoke();
            });
            Instance.SetCountdown(countDownTime, eCountdown);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, Instance);
        }

        public void OpenPromptBox(string content, uint titleId, Action confirm, Action cancel, uint confirmNameId, uint cancelNameId, ECountDownType eCountdownType, ECountdown eCountdown, float countDownTime)
        {
            Instance.Clear();
            Instance.title = Table.CSVLanguage.Instance.GetConfData(titleId == 0 ? 5520 : titleId)?.words;
            Instance.content = content;
            Instance.SetConfirm(true, () =>
            {
                confirm?.Invoke();
            }, confirmNameId);

            Instance.SetCancel(true, () =>
            {
                cancel?.Invoke();
            }, cancelNameId);
            Instance.SetCountdown(countDownTime, eCountdown, eCountdownType);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, Instance);
        }

        /// <summary>
        /// 通用活动提示含优先级的弹窗
        /// </summary>
        public bool OpenActivityPriorityPromptBox(EPriorityType priority, string content, Action confirm, Action cancel = null, float cd = 0, ECountdown countdown = ECountdown.Cancel, ECountDownType countdownType = ECountDownType.AutoTrigger)
        {
            if (priority == EPriorityType.None || (int)priority > (int)_ePriority)
            {
                CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
                if (mapData != null && !mapData.DisablePromptBox)
                {
                    Instance.Clear();
                    Instance.needDingTo = true;
                    Instance._ePriority = priority;
                    Instance.content = content;
                    Instance.SetConfirm(true, confirm);
                    Instance.SetCancel(true, cancel);
                    if (cd > 0)
                    {
                        Instance.SetCountdown(cd, countdown, countdownType);
                    }
                    Sys_Hint.Instance.eventEmitter.Trigger(Sys_Hint.EEvents.RefreshPromptBoxData);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, Instance);
                    return true;
                }
            }
            return false;
        }
    }
}



