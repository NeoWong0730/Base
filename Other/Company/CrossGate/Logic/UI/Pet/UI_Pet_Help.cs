using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Lib.Core;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class UI_Pet_Help_Layout
    {
        public Transform transform;
        public Text amountText;
        public Toggle openToggle;
        public Button closeBtn;
        public Button ruleBtn;
        public Button saveBtn;

        public InfinityGridLayoutGroup infinity;
       
        public void Init(Transform transform)
        {

            this.transform = transform;
            amountText = transform.Find("Animator/Text/Text_Amount").GetComponent<Text>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            ruleBtn = transform.Find("Animator/Btn_Rule").GetComponent<Button>();
            saveBtn = transform.Find("Animator/Below/Button_ok").GetComponent<Button>();
            saveBtn.gameObject.AddComponent<ButtonCtrl>();
            openToggle = transform.Find("Animator/Below/Toggle").GetComponent<Toggle>();
            infinity = transform.Find("Animator/Scroll_View/Grid").gameObject.gameObject.GetNeedComponent<InfinityGridLayoutGroup>(); 
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            ruleBtn.onClick.AddListener(listener.OnRuleBtnClicked);
            saveBtn.onClick.AddListener(listener.OnSaveBtnClicked);
            openToggle.onValueChanged.AddListener(listener.ToggleValueChange);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnRuleBtnClicked();
            void OnSaveBtnClicked();
            void ToggleValueChange(bool isOn);
        }
    }

    public class UI_Pet_Help : UIBase, UI_Pet_Help_Layout.IListener
    {
        public class UI_Pet_HelpItem
        {
            public ClientPet pet;
            private Image headImage;
            private Text petNameText;
            private Text autoNumText;
            private GameObject selectGo;
            private GameObject blackGo;
            public Button funcBtn;

            private Action<UI_Pet_HelpItem> action;

            public void Init(Transform transform)
            {
                funcBtn = transform.Find("Image_BGNone").GetComponent<Button>();
                funcBtn.onClick.AddListener(OnFuncBtnClicked);
                headImage = transform.Find("GameObject/HeadImage").GetComponent<Image>();
                petNameText = transform.Find("GameObject/Image_Namebg/Text_Name").GetComponent<Text>();
                autoNumText = transform.Find("GameObject/Image_Number/Text").GetComponent<Text>();
                selectGo = transform.Find("GameObject/Image_Selected").gameObject;
                blackGo = transform.Find("GameObject/Image_Black").gameObject;
            }

            private void OnFuncBtnClicked()
            {
                action?.Invoke(this);
            }

            public void RegisterAction(Action<UI_Pet_HelpItem> action)
            {
                this.action = action;
            }

            public void SetInfo(ClientPet pet)
            {
                this.pet = pet;
                if (null != pet)
                {
                    ImageHelper.SetIcon(headImage, pet.petData.bust);
                    petNameText.text = pet.GetPetNmae();
                    blackGo.SetActive(pet.petUnit.SimpleInfo.Loyalty < 1);
                }
            }

            public void SetSelectState(bool state)
            {
                selectGo.SetActive(state);
            }

            public void SetAutoNum(int num)
            {
                bool show = num != -1;
                if(show)
                {
                    autoNumText.text = (num + 1).ToString();
                }
                autoNumText.transform.parent.gameObject.SetActive(show);
            }
        }


        private UI_Pet_Help_Layout layout = new UI_Pet_Help_Layout();

        private Dictionary<GameObject, UI_Pet_HelpItem> helpDic = new Dictionary<GameObject, UI_Pet_HelpItem>();
        private List<UI_Pet_HelpItem> helpList = new List<UI_Pet_HelpItem>();
        private List<ClientPet> AutoPetList = new List<ClientPet>();
        private List<uint> severAutoPetList = new List<uint>();
        private int infinityCount;
        private bool toggleValue;
        private uint selectPetUid;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.infinity.minAmount = 15;
            layout.infinity.updateChildrenCallback = UpdateChildrenCallback;
            SetPetAutoItem();
        }

        private void SetPetAutoItem()
        {
            for (int i = 0; i < layout.infinity.transform.childCount; i++)
            {
                GameObject go = layout.infinity.transform.GetChild(i).gameObject;
                UI_Pet_HelpItem helpItem = new UI_Pet_HelpItem();
                helpItem.Init(go.transform);
                helpItem.RegisterAction(OnSkillSelect);
                helpDic.Add(go, helpItem);
                helpList.Add(helpItem);
            }
        }

        private void OnSkillSelect(UI_Pet_HelpItem petHelpItem)
        {
            selectPetUid = petHelpItem.pet.GetPetUid();
            SetSelectState();
            if (severAutoPetList.Contains(selectPetUid))
            {
                severAutoPetList.Remove(selectPetUid);
            }
            else
            {
                if (petHelpItem.pet.petUnit.SimpleInfo.Loyalty < 1)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12267));
                    return;
                }
                else if (severAutoPetList.Count >= Sys_Pet.Instance.MaxAutoBlinkNum)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10937));
                    return;
                }
                else
                {
                    severAutoPetList.Add(selectPetUid);
                }
            }
            SetIcomNums();
            NumViewChange();
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= infinityCount)
                return;
            if (helpDic.ContainsKey(trans.gameObject))
            {
                UI_Pet_HelpItem helpItem = helpDic[trans.gameObject];
                ClientPet pet = AutoPetList[index];
                helpItem.SetInfo(pet);
                helpItem.SetSelectState(selectPetUid == pet.GetPetUid());
                helpItem.SetAutoNum(severAutoPetList.IndexOf(pet.GetPetUid()));
            }
        }

        protected override void OnShow()
        {
            severAutoPetList.Clear();
            severAutoPetList.AddRange(Sys_Pet.Instance.petAutoBlinkList);
            toggleValue = Sys_Pet.Instance.useAutoBlink;
            AutoPetList = Sys_Pet.Instance.GetHaveAutoBlinkSkillPets();
            infinityCount = AutoPetList.Count;
            SetView();
        }

        private void SetSelectState()
        {
            for (int i = 0; i < helpList.Count; i++)
            {
                UI_Pet_HelpItem helpItem = helpList[i];
                if (null != helpItem.pet)
                {
                    helpItem.SetSelectState(selectPetUid == helpItem.pet.GetPetUid());
                }
            }
        }

        private void SetIcomNums()
        {
            for (int i = 0; i < helpList.Count; i++)
            {
                UI_Pet_HelpItem helpItem = helpList[i];
                if(null != helpItem.pet)
                {
                    helpItem.SetAutoNum(severAutoPetList.IndexOf(helpItem.pet.GetPetUid()));
                }
            }
        }

        private void SetView()
        {
            SetToggle();
            NumViewChange();
            layout.infinity.SetAmount(infinityCount);
        }

        private void SetToggle()
        {
            layout.openToggle.isOn = toggleValue;
        }

        public void NumViewChange()
        {
            layout.amountText.text = string.Format("{0}/{1}", severAutoPetList.Count.ToString(), Sys_Pet.Instance.MaxAutoBlinkNum.ToString());
        }

        public void OncloseBtnClicked()
        {
            CloseSelf();
        }

        public void OnRuleBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(10898) });
        }

        public void OnSaveBtnClicked()
        {
            Sys_Pet.Instance.OnPetAutoBlinkSetReq(severAutoPetList, toggleValue);
            CloseSelf();
        }

        public void ToggleValueChange(bool isOn)
        {
            toggleValue = isOn;
        }
    }
}

