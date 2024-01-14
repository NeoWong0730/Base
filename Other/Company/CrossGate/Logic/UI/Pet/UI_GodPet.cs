using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class GodPetItem
    {
        public PetUnit pet;
        private Text levelText;
        private Image peticonImage;
        private GameObject addGo;
        public Button selectBtn;
        private IListener listener;
        //private         
        public void Init(Transform transform)
        {
            peticonImage = transform.Find("Pet01/Image_Icon").GetComponent<Image>();
            levelText = transform.Find("Pet01/Image_Level/Text_Level/Text_Num").GetComponent<Text>();
            addGo = transform.Find("Image_Add").gameObject;
            selectBtn = transform.Find("ItemBg").GetComponent<Button>();
            selectBtn.onClick.AddListener(OpenSelectPetUI);
        }

        public void SetData(uint id)
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(id);
            if (null != clientPet)
            {
                pet = clientPet.petUnit;
            }
            else
            {
                pet = null;
            }
            RestView();
        }

        private void RestView()
        {
            if (null != pet)
            {
                CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(pet.SimpleInfo.PetId);
                ImageHelper.SetIcon(peticonImage, petData.icon_id);
                levelText.text = pet.SimpleInfo.Level.ToString();
                peticonImage.transform.parent.gameObject.SetActive(true);
                if (addGo.activeSelf)
                    addGo.SetActive(false);
            }
            else
            {
                if (!addGo.activeSelf)
                    addGo.SetActive(true);
                peticonImage.transform.parent.gameObject.SetActive(false);
            }
        }

        private void OpenSelectPetUI()
        {
            listener?.clickBtn();
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void clickBtn();
        }

    }

    public class UI_GodPet_Layout
    {
        public Button closeBtn;
        public Button getBtn;
        public Button resetBtn;
        public Button reviewBtn;
        public GameObject getViewGo;
        public GameObject resetViewGo;
        public PropItem resetPropItem;
        public PropItem getPropItem;
        public Toggle getToggle;
        public Toggle resetToggle;
        public Transform transform;
        public GameObject godPetItemGo;
        public Text tipsText;
        public Animator ani;
        private Text timesText;
        private Text timesTipsText;
        private uint bdCount = 0;
        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>();
            getViewGo = transform.Find("Animator/View_01").gameObject;
            getBtn = transform.Find("Animator/View_01/Btn_01").GetComponent<Button>();
            reviewBtn = transform.Find("Animator/Btn_Peiyang").GetComponent<Button>();
            reviewBtn.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_GodPetReview); });
            resetViewGo = transform.Find("Animator/View_02").gameObject;
            resetBtn = transform.Find("Animator/View_02/Btn_01").GetComponent<Button>();
            godPetItemGo = transform.Find("Animator/View_02/PetItem01").gameObject;
            tipsText = transform.Find("Animator/View_BG/Text_Tips").GetComponent<Text>();
            ani = transform.Find("Animator/View_BG/Circle").GetComponent<Animator>();
            timesText = transform.Find("Animator/View_02/Special/Text").GetComponent<Text>();
            timesTipsText = transform.Find("Animator/View_02/Special/Text_Tips").GetComponent<Text>();

            getToggle = transform.Find("Animator/Tab/TabItem01").GetComponent<Toggle>();
            resetToggle = transform.Find("Animator/Tab/TabItem02").GetComponent<Toggle>();
            resetPropItem = new PropItem();
            resetPropItem.BindGameObject(transform.Find("Animator/View_02/PropItem").gameObject);
            CSVParam.Data resetParam = CSVParam.Instance.GetConfData(713);
            if (null != resetParam)
            {
                string[] paramStr = resetParam.str_value.Split('|');
                if (paramStr.Length >= 2)
                {
                    uint id = Convert.ToUInt32(paramStr[0]);
                    int num = Convert.ToInt32(paramStr[1]);
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, num, true, false, false, false, false, true, true);
                    resetPropItem.SetData(new MessageBoxEvt(EUIID.UI_GodPet, itemData));
                }
            }

            getPropItem = new PropItem();
            getPropItem.BindGameObject(transform.Find("Animator/View_01/PropItem").gameObject);
            CSVParam.Data getParam = CSVParam.Instance.GetConfData(712);
            if (null != getParam)
            {
                string[] paramStr = getParam.str_value.Split('|');
                if (paramStr.Length >= 2)
                {
                    uint id = Convert.ToUInt32(paramStr[0]);
                    int num = Convert.ToInt32(paramStr[1]);
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, num, true, false, false, false, false, true, true);
                    getPropItem.SetData(new MessageBoxEvt(EUIID.UI_GodPet, itemData));
                }
            }
            bdCount = CSVPetNewParam.Instance.GetConfData(63).value;
            TextHelper.SetText(timesTipsText, 2021183, bdCount.ToString());
        }

        public void SetBdText()
        {
            TextHelper.SetText(timesText, 2021182, Sys_Pet.Instance.GoldPetExchangeNum.ToString(), bdCount.ToString());
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            getBtn.onClick.AddListener(listener.OnGetBtnClicked);
            resetBtn.onClick.AddListener(listener.OnResetBtnClicked);
            getToggle.onValueChanged.AddListener(listener.OnGetToggleClicked);
            resetToggle.onValueChanged.AddListener(listener.OnResetToggleClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnGetBtnClicked();
            void OnResetBtnClicked();
            void OnGetToggleClicked(bool isOn);
            void OnResetToggleClicked(bool isOn);
        }

    }

    public class UI_GodPet : UIBase, UI_GodPet_Layout.IListener, GodPetItem.IListener
    {
        private UI_GodPet_Layout layout = new UI_GodPet_Layout();
        private GodPetItem godPetItem;
        private uint getItemId;
        private int getNum;
        private uint resetItemId;
        private int resetItemNum;
        private uint currentIndex = 1;
        private Timer aniTimer;
        private uint selectPetUid;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            godPetItem = new GodPetItem();
            godPetItem.Init(layout.godPetItemGo.transform);
            godPetItem.Register(this);
            CSVParam.Data resetParam = CSVParam.Instance.GetConfData(713);
            if (null != resetParam)
            {
                string[] paramStr = resetParam.str_value.Split('|');
                if (paramStr.Length >= 2)
                {
                    resetItemId = Convert.ToUInt32(paramStr[0]);
                    resetItemNum = Convert.ToInt32(paramStr[1]);
                }
            }


            CSVParam.Data getParam = CSVParam.Instance.GetConfData(712);
            if (null != getParam)
            {
                string[] paramStr = getParam.str_value.Split('|');
                if (paramStr.Length >= 2)
                {
                    getItemId = Convert.ToUInt32(paramStr[0]);
                    getNum = Convert.ToInt32(paramStr[1]);
                }
            }
        }

        protected override void OnShow()
        {
            ToggleState();
            RefreshUI();
            godPetItem.SetData(selectPetUid);
        }

        protected override void OnHide()
        {
            aniTimer?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnAddPet, Reset, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshMainBagData, RestTemple, toRegister);
        }

        private void RestTemple(int id)
        {
            RefreshUI();
            godPetItem.SetData(selectPetUid);
        }

        private void Reset()
        {
            selectPetUid = 0;
            RefreshUI();
            godPetItem.SetData(selectPetUid);
        }

        public void clickBtn()
        {
            List<PetUnit> godPetList = Sys_Pet.Instance.GetGodPetList();
            if (godPetList.Count > 0)
            {
                SelectPetParam selectPet = new SelectPetParam();
                selectPet.PetList = godPetList;
                selectPet.commonId = 0;
                selectPet.action = SelectAction;
                UIManager.OpenUI(EUIID.UI_SelectPet, false, selectPet);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021177));
            }
        }

        private void SelectAction(uint uid)
        {
            ClientPet clientPet = Sys_Pet.Instance.GetPetByUId(uid);
            if (null != clientPet)
            {
                PetUnit pet = clientPet.petUnit;
                if (null != pet)
                {
                    if (Sys_Pet.Instance.fightPet != null && Sys_Pet.Instance.fightPet.IsSamePet(pet))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021176));
                        return;
                    }

                    if(clientPet.GetPetUid() == Sys_Pet.Instance.mountPetUid)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021168));
                        return;
                    }

                    if (clientPet.GetPetIsDomestication())
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2021169);
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            selectPetUid = uid;
                            godPetItem.SetData(uid);
                            ButtonState();
                            UIManager.CloseUI(EUIID.UI_SelectPet);
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    }
                    else
                    {
                        selectPetUid = uid;
                        godPetItem.SetData(uid);
                        ButtonState();
                        UIManager.CloseUI(EUIID.UI_SelectPet);
                    }
                }
            }
        }

        private void ToggleState()
        {
            if (currentIndex == 1)
            {
                layout.getToggle.isOn = true;
            }
            else
            {
                layout.resetToggle.isOn = true;
            }
        }

        private void RefreshUI()
        {
            bool isGet = currentIndex == 1;
            layout.SetBdText();

            layout.getViewGo.SetActive(isGet);
            layout.resetViewGo.SetActive(!isGet);
            ButtonState();

            uint languid = isGet ? 2021180u : 2021181u;
            TextHelper.SetText(layout.tipsText, languid);

            if (isGet)
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(getItemId, getNum, true, false, false, false, false, true, true);
                layout.getPropItem.SetData(new MessageBoxEvt(EUIID.UI_GodPet, itemData));
            }
            else
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(resetItemId, resetItemNum, true, false, false, false, false, true, true);
                layout.resetPropItem.SetData(new MessageBoxEvt(EUIID.UI_GodPet, itemData));
            }

        }

        public void OncloseBtnClicked()
        {
            selectPetUid = 0;
            currentIndex = 1;
            layout.getToggle.isOn = true;
            CloseSelf();
        }

        public void OnGetBtnClicked()
        {
            if (getNum > Sys_Bag.Instance.GetItemCount(getItemId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021172));
                return;
            }

            PromptBoxParameter.Instance.OpenPromptBox(2021175, 0, () =>
            {
                layout.ani.Play("Exchange", -1, 0);
                AnimationClip ani = layout.ani.runtimeAnimatorController.animationClips[0];
                aniTimer?.Cancel();
                aniTimer = Timer.Register(ani.averageDuration, () =>
                {
                    Sys_Pet.Instance.ExchangeGoldPetReq();
                }, null, false, false);
            });
        }

        public void OnResetBtnClicked()
        {
            if (selectPetUid == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021171));
                return;
            }

            if (resetItemNum > Sys_Bag.Instance.GetItemCount(resetItemId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021170));
                return;
            }
            var clientPet = Sys_Pet.Instance.GetPetByUId(selectPetUid);
            if (null != clientPet && clientPet.HasEquipDemonSpiritSphere())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002043));
                return;
            }

            PromptBoxParameter.Instance.OpenPromptBox(2021173, 0, () =>
            {
                layout.ani.Play("Rebirth", -1, 0);
                AnimationClip ani = layout.ani.runtimeAnimatorController.animationClips[0];
                aniTimer?.Cancel();
                aniTimer = Timer.Register(ani.averageDuration, () =>
                {
                    Sys_Pet.Instance.PetReExchangeGoldPetReq(selectPetUid);
                }, null, false, false);
            });

        }

        private void ButtonState()
        {
            ImageHelper.SetImageGray(layout.getBtn, !(getNum <= Sys_Bag.Instance.GetItemCount(getItemId)), true);
            ImageHelper.SetImageGray(layout.resetBtn, !(resetItemNum <= Sys_Bag.Instance.GetItemCount(resetItemId) && selectPetUid != 0), true);
        }



        public void OnGetToggleClicked(bool isOn)
        {
            if (currentIndex != 1)
            {
                aniTimer?.Cancel();
                currentIndex = 1;
                RefreshUI();
                selectPetUid = 0;
                godPetItem.SetData(selectPetUid);
            }
        }

        public void OnResetToggleClicked(bool isOn)
        {
            if (currentIndex != 2)
            {
                aniTimer?.Cancel();
                currentIndex = 2;
                RefreshUI();
                selectPetUid = 0;
                godPetItem.SetData(selectPetUid);
            }
        }

    }
}
