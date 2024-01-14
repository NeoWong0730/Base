using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{

    public class UI_Pet_DemonReform_Layout
    {
        private Button closeBtn;
        private Button activeBtn;
        public Text needTipsText;
        public Text needTips2Text;
        /// <summary> 激活魔魂界面 </summary>
        private GameObject activeGo;
        /// <summary> 魔魂激活后展示界面 </summary>
        private GameObject activeEndGo;
        public PropItem activePropItem;
        public Animator activeAni;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            activeBtn = transform.Find("Animator/View_Content/Activate/Btn_01").GetComponent<Button>();
            activeGo = transform.Find("Animator/View_Content/Activate").gameObject;
            needTipsText = transform.Find("Animator/View_Content/Activate/Text_01").GetComponent<Text>();
            needTips2Text = transform.Find("Animator/View_Content/Image_Name/Text1").GetComponent<Text>();

            activeEndGo = transform.Find("Animator/View_Content/Image_Full").gameObject;
            activePropItem = new PropItem();
            activePropItem.BindGameObject(transform.Find("Animator/View_Content/Activate/PropItem").gameObject);
            activeAni = transform.Find("Animator/View_Content").GetComponent<Animator>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            activeBtn.onClick.AddListener(listener.OnActiveBtnClicked);
        }


        public void SetActiveStatelView(bool active)
        {
            activeEndGo.SetActive(active);
            activeGo.SetActive(!active);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnActiveBtnClicked();
        }
    }

    public class UI_Pet_DemonReform : UIBase, UI_Pet_DemonReform_Layout.IListener
    {
        private UI_Pet_DemonReform_Layout layout = new UI_Pet_DemonReform_Layout();
        List<uint> levels;
        private ClientPet currentPet;
        //当选择的宠物激活需要另一个宠物时
        private uint selectPetUid;
        private UI_Pet_DemonParam param;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            layout.activePropItem.SetData(itemN, EUIID.UI_Pet_DemonReform);
            layout.activePropItem.btnNone.gameObject.SetActive(true);
            layout.activePropItem.Layout.imgQuality.gameObject.SetActive(false);

            levels = Sys_Pet.Instance.EquipActiveLevelLimit;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnOwnDemonSpiritPetSelect, OnSelectPet, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDemonSpiritRemake, OnActiveDemonSpiritRemake, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            param = arg as UI_Pet_DemonParam;
            var currentPetUid = param.tuple;

            currentPet = Sys_Pet.Instance.GetPetByUId(currentPetUid);
        }

        protected override void OnShow()
        {
            if (null != param)
            {
                RefreshView();
            }
        }

        protected override void OnHide()
        {
        }


        Timer timer;
        private void OnActiveDemonSpiritRemake()
        {
            AnimationClip an = layout.activeAni.runtimeAnimatorController.animationClips[0];
            float _timer = an.averageDuration;
            layout.activeAni.Play("Fx", -1, 0f);
            timer?.Cancel();

            timer = Timer.Register(_timer, () =>
            {
                //Sys_Pet.Instance.eventEmitter.Trigger<uint>(Sys_Pet.EEvents.OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd, 1u);
                Sys_Pet.Instance.eventEmitter.Trigger<uint>(Sys_Pet.EEvents.OnActiveOwnDemonSpiritOrRemakeAniUiAnimatorEnd, 2u);
                UIManager.CloseUI(EUIID.UI_Pet_DemonReform);
            });
        }

        private void RefreshView()
        {
            if(null != currentPet)
            {
                var equipLevelLimits = Sys_Pet.Instance.DemonSpiritRemakeTimesByEquipLevel;
                TextHelper.SetText(layout.needTips2Text, 680002047u, currentPet.GetEquipSphereTotalLevel().ToString(), equipLevelLimits[(int)param.type - 1].ToString());

                bool isActive = currentPet.GetPetDemonSoulRemakeTimes() >= param.type;
                if (!isActive)
                {
                    SetActiveItem();
                }
                layout.SetActiveStatelView(isActive);
            }
        }

        private void OnSelectPet(uint petUid)
        {
            selectPetUid = petUid;
            var cp = Sys_Pet.Instance.GetPetByUId(selectPetUid);
            if (null != cp)
            {
                //宠物id 同道具id
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(cp.petData.id, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                layout.activePropItem.SetData(itemN, EUIID.UI_Pet_DemonReform);
                layout.activePropItem.btnNone.gameObject.SetActive(false);
                layout.activePropItem.Layout.imgIcon.enabled = true;
            }
        }

        private void SetActiveItem()
        {
            List<uint> needList = null;
            var equipLevelLimits = Sys_Pet.Instance.DemonSpiritRemakeTimesByEquipLevel;
            if (param.type == 1)
            {
                needList = currentPet.petData.extra1_remake_cost;
            }
            else if (param.type == 2)
            {
                needList = currentPet.petData.extra2_remake_cost;
            }
 
            if (null != needList && needList.Count >= 3)
            {
                var costType = needList[0];
                layout.needTipsText.gameObject.SetActive(costType == 1);
                if (costType == 1)//消耗宠物
                {
                    var langId = 680002029u;
                    if (needList[1] == 0)
                    {
                        langId = 680002030u;
                    }
                    
                    TextHelper.SetText(layout.needTipsText, langId, needList[2].ToString());
                    if (selectPetUid == 0)
                    {
                        PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                        layout.activePropItem.SetData(itemN, EUIID.UI_Pet_DemonReform);
                        layout.activePropItem.txtNumber.gameObject.SetActive(false);
                        layout.activePropItem.btnNone.gameObject.SetActive(true);
                        layout.activePropItem.Layout.imgIcon.enabled = false;
                        layout.activePropItem.Layout.imgQuality.gameObject.SetActive(false);
                    }
                    else
                    {
                        var cp = Sys_Pet.Instance.GetPetByUId(selectPetUid);
                        if(null != cp)
                        {
                            //宠物id 同道具id
                            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(cp.petData.id, 0, true, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
                            layout.activePropItem.SetData(itemN, EUIID.UI_Pet_DemonReform);
                            layout.activePropItem.btnNone.gameObject.SetActive(false);
                            layout.activePropItem.Layout.imgIcon.enabled = true;
                        }
                    }
                }
                else if(costType == 2)// 消耗道具
                {
                    var itemId = needList[1];
                    var itemNum = needList[2];
                    PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, true, false, false, false, false, true, true, true);
                    layout.activePropItem.SetData(itemN, EUIID.UI_Pet_DemonReform);
                    layout.activePropItem.Layout.imgIcon.enabled = true;
                }
            }
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonReform);
        }

        public void OnActiveBtnClicked()
        {
            if (null != currentPet)
            {
                List<uint> needList = null;
                var equipLevelLimits = Sys_Pet.Instance.DemonSpiritRemakeTimesByEquipLevel;
                var currentLimitLevel = equipLevelLimits[(int)param.type - 1];
                if(currentLimitLevel > currentPet.GetEquipSphereTotalLevel())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002010u));
                    return;
                }

                if (param.type - 1 > currentPet.GetPetDemonSoulRemakeTimes())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002049u));
                    return;
                }

                if (param.type == 1)
                {
                    needList = currentPet.petData.extra1_remake_cost;
                }
                else if (param.type == 2)
                {
                    needList = currentPet.petData.extra2_remake_cost;
                }
                var costType = needList[0];
                if (costType == 1)//消耗宠物
                {
                    if (selectPetUid == 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002011u));
                    }
                    else
                    {
                        var cp = Sys_Pet.Instance.GetPetByUId(selectPetUid);
                        
                        if (cp.petUnit != null && cp.petUnit.Islocked)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, cp.GetPetNmae(), cp.GetPetNmae());
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Pet.Instance.OnPetLockReq(cp.GetPetUid(), false);
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                            return;
                        }
                        
                        if (null != cp)
                        {
                            PromptBoxParameter.Instance.Clear();
                            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680002058, cp.GetPetNmae(), cp.petUnit.SimpleInfo.Score.ToString());
                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                if (null != cp)
                                {
                                    Sys_Pet.Instance.PetSoulAddRemakeCountReq(currentPet.GetPetUid(), param.type - 1, selectPetUid);
                                }
                            });
                            PromptBoxParameter.Instance.SetCancel(true, null);
                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                    }
                }
                else if (costType == 2)// 消耗道具
                {
                    var itemId = needList[1];
                    var itemNum = needList[2];
                    var itemCount = new ItemIdCount(itemId, itemNum);
                    if(!itemCount.Enough)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002025u));
                    }
                    else
                    {
                        Sys_Pet.Instance.PetSoulAddRemakeCountReq(currentPet.GetPetUid(), param.type - 1, 0);
                    }
                }
            }
        }

        private void ItemGridBeClicked(PropItem bulidItem)
        {
            if (null != currentPet)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonPet, false, param);
            }
        }
    }
}