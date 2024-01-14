using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Pet_Exchange_Layout
    {
        public Transform transform;
        public Button closeButton;
        public GameObject leftViewGo;
        public GameObject rightViewGo;
        public GameObject petListViewGo;
        public GameObject noneGo;
        public void Init(Transform transform)
        {
            this.transform = transform;
            leftViewGo = transform.Find("Animator/View_Message/View_Messag/View_Left").gameObject;
            rightViewGo = transform.Find("Animator/View_Message/View_Messag/View_Right").gameObject;
            petListViewGo = transform.Find("Animator/View_Message/Scroll_View_Pets").gameObject;
            noneGo = transform.Find("Animator/View_Message/View_None").gameObject;
            closeButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OncloseBtnClicked);

        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }

    }

    public class UI_Pet_ExchangeLeftView : UIComponent
    {
        public ClientPet clientPet;
        public UI_PetLeftView_Common uI_PetLeftView_Common;
        //改名
        private Button changeNameBtn;
        //更多按钮功能集

        //private Image morebtn_arrow;
        private GameObject moreBtngrid;
        private Button moreBtn;
        public Button releaseBtn;

        public Button peiyangButton;
        public Button attrBBtn;

        public Button maxGradesEffect;
        public Text degreeText;
        
        private CP_Toggle toggleLock;
        protected override void Loaded()
        {
            uI_PetLeftView_Common = AddComponent<UI_PetLeftView_Common>(gameObject.transform);
            moreBtn = transform.Find("Button_More").GetComponent<Button>();
            moreBtn.onClick.AddListener(OnMoreBtnClicked);
            moreBtngrid = transform.Find("Btn_Grid").gameObject;

            releaseBtn = transform.Find("Btn_Grid/Btn_01").GetComponent<Button>();
            releaseBtn.onClick.AddListener(OnReleaseBtnClick);


            changeNameBtn = transform.Find("Btn_Grid/Button_ChangeName").GetComponent<Button>();
            changeNameBtn.onClick.AddListener(OnChangeNameClick);

            peiyangButton = transform.Find("Grid_FunctionBtn/Btn_Peiyang").GetComponent<Button>();
            peiyangButton.onClick.AddListener(OnPeiyangBtnClick);
            maxGradesEffect = transform.Find("Grid_FunctionBtn/Btn_Add").GetComponent<Button>();
            maxGradesEffect.onClick.AddListener(MaxGradeBtnClicked);
            attrBBtn = transform.Find("Button_Attribute").GetComponent<Button>();
            attrBBtn.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Element); });

            degreeText = transform.Find("Text_Degree").GetComponent<Text>();
            
            toggleLock = transform.Find("Toggle_Lock").GetComponent<CP_Toggle>();
            toggleLock.onValueChanged.AddListener(OnToggleLock);
        }

        #region more

        private void OnMoreBtnClicked()
        {
            if (clientPet != null)
            {
                bool isExpand = moreBtngrid.activeSelf;
                moreBtngrid.SetActive(!isExpand);

                float rotateZ = isExpand ? 180f : 0f;
                moreBtn.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, rotateZ);
            }
        }

        public void HideMoreGo()
        {
            moreBtngrid.SetActive(false);
            moreBtn.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, 180);
        }
        #endregion

        public override void Hide()
        {
            base.Hide();
            clientPet = null;
            uI_PetLeftView_Common.Hide();
            HideMoreGo();
        }

        public override void Show()
        {
            base.Show();
            uI_PetLeftView_Common.Show();
        }

        private void OnReleaseBtnClick()
        {
            if(Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit))
            {
                return;
            }
            else if(GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();  //战斗内提示：当前处于战斗中，无法进行该操作
                return;
            }
            else if (Sys_Pet.Instance.fightPet.IsSamePet(clientPet.GetPetUid()))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12203));
                return;
            }
            else if (Sys_Pet.Instance.IsLastPetEntExpiredTick(clientPet.petUnit.SimpleInfo.ExpiredTick > 0))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10927));
                return;
            }
            else if (clientPet.HasEquipDemonSpiritSphere())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002040));
                return;
            }
            bool hightScore = !clientPet.petUnit.SimpleInfo.Bind && Sys_Pet.Instance.GetPetIsHightScore(clientPet.petUnit);
            if (hightScore)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Sale, false, clientPet.GetPetUid());
                
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11969u, clientPet.abandonCoin.ToString());
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (clientPet != null)
                    {
                        Sys_Pet.Instance.OnPetAbandonPetReq(clientPet.petUnit.Uid);
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public void SetValue(ClientPet _clientPet)
        {
            if (null != _clientPet)
            {
                if (clientPet == null)
                {
                    clientPet = _clientPet;
                }
                else
                {
                    uint curUid = clientPet.petUnit.Uid;
                    clientPet = _clientPet;
                    if (curUid != _clientPet.petUnit.Uid)
                    {
                        HideMoreGo();
                    }
                }
            }
            else
            {
                clientPet = null;
                HideMoreGo();
            }

            uI_PetLeftView_Common.SetValue(_clientPet);
            SetGradesState();
            OnUpdateLockState();
        }

        private void SetGradesState()
        {
            if (null != clientPet)
            {
                uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                bool isMax = lowG == 0;
                TextHelper.SetText(degreeText, LanguageHelper.GetTextContent(11803, clientPet.GetPetCurrentGradeCount().ToString(), clientPet.GetPetBuildMaxGradeCount().ToString()));
            }
        }

        private void OnChangeNameClick()
        {
            UIManager.OpenUI(EUIID.UI_Modification_Name, false, clientPet);
        }

        private void OnPeiyangBtnClick()
        {
            if (null == clientPet)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10694));
                return;
            }

            if (!Sys_FunctionOpen.Instance.IsOpen(10511, false))
                return;
            if (clientPet.petUnit.SimpleInfo.Bind)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101565));
            }
            else
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(10531, true))//宠物功能开启条件
                    return;
            }
        }

        private void MaxGradeBtnClicked()
        {
            if (null != clientPet)
            {
                uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                CSVPetNewParam.Data csv = CSVPetNewParam.Instance.GetConfData(29);
                if (null != csv)
                {
                    uint skillId = csv.value;
                    UIManager.OpenUI(EUIID.UI_PetSkill_Tips, false, new Tuple<uint, uint>(skillId, lowG));
                }
            }
        }
        
        private void OnToggleLock(bool isOn)
        {
            if (null != clientPet)
            {
                if (clientPet.petUnit.Islocked == isOn)
                    Sys_Pet.Instance.OnPetLockReq(clientPet.petUnit.Uid, !isOn);
            }
        }
        
        public void OnUpdateLockState()
        {
            if (null != clientPet)
            {
                ClientPet data = Sys_Pet.Instance.GetPetByUId(clientPet.petUnit.Uid);
                toggleLock.SetSelected(!data.petUnit.Islocked, true);
            }
        }
    }

    public class UI_Pet_ExchangeRightView : UIComponent
    {
        public class UI_PetExchangeGetItem : UIComponent
        {
            public PropItem propItem;
            public Text itemLvText;
            public Text itemNumText;
            protected override void Loaded()
            {
                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);
                itemLvText = transform.Find("Text_Lv").GetComponent<Text>();
                itemNumText = transform.Find("Text_Num").GetComponent<Text>();
            }

            public override void SetData(params object[] arg)
            {
                if(null != arg && arg.Length >= 2)
                {
                    uint itemId = Convert.ToUInt32(arg[0]);
                    float itemNums = Convert.ToSingle(arg[1]);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_SelectItem, new PropIconLoader.ShowItemData(itemId, 1, false, false, false, false, false, false, false, true)));
                    propItem.txtNumber.gameObject.SetActive(true);
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                    TextHelper.SetText(propItem.txtName, cSVItemData.name_id);
                    propItem.txtName.gameObject.SetActive(true);
                    itemLvText.text = cSVItemData.lv.ToString();
                    itemNumText.text = (itemNums * 100).ToString("0.#") + "%";
                }
            }
        }

        private COWVd<UI_PetExchangeGetItem> exchangeItems = new COWVd<UI_PetExchangeGetItem>();
         
        private Image iconImage;
        private Button ruleBtn;
        private Button exchangeBtn;
        public Transform itemParent;
        private GameObject itemGo;
        private GameObject tipsGo;//评分提示
        private Text conditonTipsText;//品质提示
        private ClientPet clientPet;
        private int targetCount;
        List<uint> items = new List<uint>();
        Dictionary<uint, float> itemAndWeight = new Dictionary<uint, float>();
        protected override void Loaded()
        {
            iconImage = transform.Find("Image_Icon").GetComponent<Image>();
            ruleBtn = transform.Find("Text_Tips/Button").GetComponent<Button>();
            ruleBtn.onClick.AddListener(RuleBtnClicked);
            exchangeBtn = transform.Find("Btn_Exchange").GetComponent<Button>();
            exchangeBtn.onClick.AddListener(ExchangeBtnClicked);
            itemGo = transform.Find("Scroll View/Viewport/Content/Image_Item").gameObject;
            itemParent = transform.Find("Scroll View/Viewport/Content");
            tipsGo = transform.Find("Text_Tips2").gameObject;
            TextHelper.SetText(tipsGo.GetComponent<Text>(), LanguageHelper.GetTextContent(11005, Sys_Pet.Instance.MinExChangeScore.ToString()));
            conditonTipsText = transform.Find("Text_Tips3").GetComponent<Text>();
        }

        private void RuleBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(10906, Sys_Pet.Instance.MinExChangeScore.ToString()) });
        }

        private void ExchangeBtnClicked()
        {
            if (Sys_Pet.Instance.IsUniquePet(clientPet.petData.id))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12469));
                return;
            }
            else if (Sys_Pet.Instance.IsPetBeEffectWithSecureLock(clientPet.petUnit))
            {
                return;
            }
            else if (clientPet.HasPartnerPet())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12381));
                return;
            }
            else if (clientPet.IsHasEquip())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12382));
                return;
            }
            else if (Sys_Pet.Instance.MinExChangeScore > clientPet.petUnit.SimpleInfo.Score)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10932));
                return;
            }
            else if(!Sys_Pet.Instance.GetPetIsHightScore(clientPet.petUnit))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12103));
                return;
            }
            else if(Sys_Pet.Instance.fightPet.IsSamePet(clientPet.GetPetUid()))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10933));
                return;
            }
            else if (Sys_Pet.Instance.IsLastPetEntExpiredTick(clientPet.petUnit.SimpleInfo.ExpiredTick > 0))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10934));
                return;
            }
            else if(clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000657));
                return;
            }
            else if (clientPet.GetPetUid() == Sys_Pet.Instance.mountPetUid)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000663));
                return;
            }
            else if (clientPet.HasEquipDemonSpiritSphere())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002039));
                return;
            }
            else if (clientPet.petUnit.Islocked)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, clientPet.GetPetNmae(), clientPet.GetPetNmae());
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {

                    Sys_Pet.Instance.OnPetLockReq(clientPet.GetPetUid(), false);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                return;
            }
            
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11626, clientPet.GetPetNmae());
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {

                Sys_Pet.Instance.OnPetDeComposeReq(clientPet.GetPetUid());
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public override void SetData(params object[] arg)
        {
            if(null != arg && arg.Length >= 1)
            {
                clientPet = arg[0] as ClientPet;
                Refresh();
            }
            else
            {
                Hide();
            }
        }

        protected override void Refresh()
        {
            if(null != clientPet)
            {
                ImageHelper.SetIcon(iconImage, clientPet.petData.icon_id);
                bool scoreEnough = clientPet.petUnit.SimpleInfo.Score >= Sys_Pet.Instance.MinExChangeScore;
                bool petQualityEnough = Sys_Pet.Instance.GetPetIsHightScore(clientPet.petUnit);
                bool hasContract = clientPet.HasPartnerPet();
                bool hasEquip = clientPet.IsHasEquip();
                if (scoreEnough && petQualityEnough && !hasContract && !hasEquip)
                {
                    itemAndWeight = Sys_Pet.Instance.GetExchangeItemData(clientPet.petUnit.SimpleInfo.Score);
                    items = new List<uint>(itemAndWeight.Keys);
                    targetCount = items.Count;
                }
                else
                {
                    items.Clear();
                    targetCount = 0;
                }

                if(hasContract)
                {
                    TextHelper.SetText(conditonTipsText, 12381);
                }
                else if(hasEquip)
                {
                    TextHelper.SetText(conditonTipsText, 12382);
                }
                else if(!petQualityEnough)
                {
                    TextHelper.SetText(conditonTipsText, 12103);
                }
                conditonTipsText.gameObject.SetActive(!petQualityEnough || hasContract || hasEquip);
                tipsGo.SetActive(!scoreEnough);
                exchangeItems.TryBuildOrRefresh(itemGo, itemParent, targetCount, RefreshItems);
            }
            else
            {
                Hide();
            }
        }

        private void RefreshItems(UI_PetExchangeGetItem item, int index)
        {
            uint key = items[index];
            item.SetData(key, itemAndWeight[key]);
        }

        public override void Hide()
        {
            clientPet = null;
            base.Hide();
        }

        public override void Show()
        {
            base.Show();
        }
    }

    public class UI_Pet_Exchange : UIBase, UI_Pet_Exchange_Layout.IListener
    {
        private UI_Pet_Exchange_Layout layout = new UI_Pet_Exchange_Layout();

        private UI_CurrencyTitle UI_CurrencyTitle;

        public UI_Pet_ExchangeLeftView leftview;
        public UI_Pet_ExchangeRightView rightview;
        private UI_Pet_ViewList pet_ViewList;

        private ClientPet currentChoosePet;
        #region ui_pet_message timeweekfun

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            rightview = AddComponent<UI_Pet_ExchangeRightView>(layout.rightViewGo.transform);
            leftview = AddComponent<UI_Pet_ExchangeLeftView>(layout.leftViewGo.transform);
            leftview.uI_PetLeftView_Common.assetDependencies = transform.GetComponent<AssetDependencies>();
            pet_ViewList = AddComponent<UI_Pet_ViewList>(layout.petListViewGo.transform);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<UI_Pet_Cell>(Sys_Pet.EEvents.OnChoosePetCell, ChoosePetCell, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeStatePet, SetFightState, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnReNamePet, ResetPetValue, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnNumberChangePet, AbandonPet, toRegister);

            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnUpdatePetInfo, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetLockChange, OnPetLockChange, toRegister);
            pet_ViewList.ProcessEvents(toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
            {
                currentChoosePet = Sys_Pet.Instance.GetPetByUId(Convert.ToUInt32(arg));
            }
            
        }

        protected override void OnShowEnd()
        {
            UI_CurrencyTitle.InitUi();
        }

        protected override void OnShow()
        {
            Sys_Pet.Instance.ChangeFightPetIndex();
            SetInitClient();
            LeftShow();
        }

        public void ChoosePetCell(UI_Pet_Cell uI_Pet_Cell)
        {
            if (!uI_Pet_Cell.longState)
            {
                if (uI_Pet_Cell.gridState == EPetGridState.Normal)
                {
                    currentChoosePet = uI_Pet_Cell.pet;
                    UpdateInfo();
                }
            }
        }

        private void LeftHide()
        {
            leftview.Hide();
            pet_ViewList.Hide();
        }

        private void LeftShow()
        {
            leftview.Show();
            pet_ViewList.Show();
            if (null != currentChoosePet)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.GetPetUid());
                if (index != -1)
                {
                    pet_ViewList?.SetPosView((uint)index);
                }
            }
            UpdateInfo();
        }

        private void SetFightState()
        {
            ReGetChoosePet();
            pet_ViewList.UpdateAllgrid();
            leftview.SetValue(currentChoosePet);
        }

        private void ResetPetValue()
        {
            ReGetChoosePet();
            rightview.SetData(currentChoosePet);
            leftview.SetValue(currentChoosePet);
        }

        private void OnUpdatePetInfo()
        {
            ResetPetValue();
        }
        
        private void OnPetLockChange()
        {
            if (leftview != null)
                leftview.OnUpdateLockState();
        }

        private void ReGetChoosePet()
        {
            if (Sys_Pet.Instance.petsList.Count > 0)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.GetPetUid());
                if (index != -1)
                {
                    currentChoosePet = Sys_Pet.Instance.GetClientPet2Postion(index);
                }
                else
                {
                    currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
                }

            }
            else
            {
                currentChoosePet = null;
            }
        }

        public void AbandonPet()
        {
            ReGetChoosePet();
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            rightview.SetData(currentChoosePet);
            leftview.SetValue(currentChoosePet);
            pet_ViewList.UpdateAllgrid();
            if (null != currentChoosePet)
            {
                int index = Sys_Pet.Instance.GetPetListIndexByUid(currentChoosePet.petUnit.Uid);
                if (index != -1)
                    pet_ViewList.SetSelect((uint)index);
            }
            else
            {
                leftview.Hide();
                pet_ViewList.SetSelect(0);
            }
            layout.noneGo.SetActive(currentChoosePet == null);

        }

        private void SetInitClient()
        {
            if (currentChoosePet == null)
            {
                currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
            }
            else
            {
                ClientPet tempPet = Sys_Pet.Instance.GetPetByUId(currentChoosePet.GetPetUid());
                if (null == tempPet)
                {
                    currentChoosePet = Sys_Pet.Instance.GetPostion2ClientPet();
                }
            }
        }


        protected override void OnHide()
        {
            LeftHide();
            CameraManager.mCamera.gameObject.SetActive(true);
        }

        protected override void OnDestroy()
        {
            UI_CurrencyTitle?.Dispose();
        }

        protected override void OnUpdate()
        {
            leftview.uI_PetLeftView_Common?.ExecUpdate();
        }


        #endregion

        #region 点击事件
        public void OncloseBtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUnloadModel);
            UIManager.CloseUI(EUIID.UI_Pet_Exchange);
        }
        #endregion
    }
}