using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class UI_MagicChange_Layout
    {
        public class HeroItem
        {
            uint headID;
            uint heroID;

            GameObject gameObject;
            Image icon;
            Image tip;
            public Toggle toggle;
            Action<uint> action;

            public HeroItem(GameObject _gameObject)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);

                icon = gameObject.FindChildByName("Image_Icon").GetComponent<Image>();
                tip = gameObject.FindChildByName("Image_tip").GetComponent<Image>();
                toggle = gameObject.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);             
            }

            public void Refresh(uint _headID, uint _heroID, Action<uint> _action)
            {
                headID = _headID;
                heroID = _heroID;
                action = _action;

                ImageHelper.SetIcon(icon, headID);
                tip.gameObject.SetActive(Sys_Role.Instance.Role.HeroId == _heroID);
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(heroID);
                }
            }
        }

        public Transform transform;

        public Button closeButton;
        public Button useButton;
        private Image eventImage;
        private Image nameIcon;

        private AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        //private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        public GameObject heroRoot;
        public GameObject heroIemObject;

        public GameObject propItemObj;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            useButton = transform.gameObject.FindChildByName("Btn_05").GetComponent<Button>();
            eventImage = transform.gameObject.FindChildByName("EventImage").GetComponent<Image>();
            nameIcon = transform.gameObject.FindChildByName("Image_Name").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

            assetDependencies = transform.GetComponent<AssetDependencies>();

            heroRoot = transform.gameObject.FindChildByName("Content");
            heroIemObject = transform.gameObject.FindChildByName("Head");

            propItemObj = transform.gameObject.FindChildByName("PropItem");
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            useButton.onClick.AddListener(listener.OnClickUseButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();
            void OnClickUseButton();
        }

        public void SetNameIcon()
        {
            ImageHelper.SetIcon(nameIcon, CSVCharacter.Instance.GetConfData(UI_MagicChange.currentSelectHeroID).name_icon);
        }

        public void LoadShowScene()
        {
            GameObject scene = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            showSceneControl = new ShowSceneControl();
            scene.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(scene);
        }

        public void LoadShowModel(uint heroID)
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }

            heroLoader.LoadHero(heroID, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });
        }

        void OnShowModelLoaded(int obj)
        {
            if (heroLoader == null || heroLoader.heroDisplay.bMainPartFinished == false)
                return;

            //_uiModelShowManagerEntity?.Dispose();
            //_uiModelShowManagerEntity = null;

            GameObject mainGo = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
            mainGo.SetActive(false);

            uint highId = 0;

            uint id = Sys_Fashion.Instance.GetDressedId(EHeroModelParts.Main);
            id = (uint)(id * 10000 + UI_MagicChange.currentSelectHeroID);
            CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);
            if (cSVFashionModelData != null)
            {
                highId = cSVFashionModelData.action_show_id;
            }

            heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon(), Constants.IdleAndRunAnimationClipHashSet, EStateType.Idle, mainGo);
         
            //_uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, heroLoader.heroDisplay.mAnimation, mainGo);
        }

        public void UnLoadShowModel()
        {
            if (heroLoader == null)
                return;

            heroLoader.Dispose();
            heroLoader = null;
        }

        public void UnLoadShowScene()
        {
            if (showSceneControl == null)
                return;

            showSceneControl.Dispose();
            showSceneControl = null;
        }

        void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }
    }

    public class UI_MagicChange : UIBase, UI_MagicChange_Layout.IListener
    {
        UI_MagicChange_Layout layout = new UI_MagicChange_Layout();

        public static uint currentSelectHeroID;

        Dictionary<uint, UI_MagicChange_Layout.HeroItem> items = new Dictionary<uint, UI_MagicChange_Layout.HeroItem>();

        PropItem propItem;

        uint costItemID;
        uint costItemCount;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            string[] strs = CSVParam.Instance.GetConfData(1547).str_value.Split('|');
            costItemID = uint.Parse(strs[0]);
            costItemCount = uint.Parse(strs[1]);
        }

        protected override void OnShow()
        {
            currentSelectHeroID = Sys_Role.Instance.Role.HeroId;

            layout.LoadShowScene();

            Init();

            if (items.ContainsKey(Sys_Role.Instance.Role.HeroId))
            {
                items[Sys_Role.Instance.Role.HeroId].toggle.isOn = true;
                items[Sys_Role.Instance.Role.HeroId].toggle.onValueChanged.Invoke(true);
            }
        }

        protected override void OnHide()
        {
            layout.UnLoadShowModel();
            layout.UnLoadShowScene();
        }

        void Init()
        {
            propItem = new PropItem();
            propItem.BindGameObject(layout.propItemObj);
            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(costItemID, costItemCount, true, false, false, false, false, true, true, true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_MagicChange, showItem));

            layout.SetNameIcon();

            layout.heroRoot.DestoryAllChildren();
            items.Clear();

            var csvDict = CSVCharacter.Instance.GetAll();
            if (csvDict != null)
            {
                foreach (var kvp in csvDict)
                {
                    if (kvp.active != 0)
                    {
                        GameObject itemObj = GameObject.Instantiate(layout.heroIemObject);
                        UI_MagicChange_Layout.HeroItem item = new UI_MagicChange_Layout.HeroItem(itemObj);
                        item.Refresh(kvp.headid, kvp.id, (heroID) =>
                        {
                            currentSelectHeroID = heroID;

                            layout.UnLoadShowModel();
                            layout.LoadShowModel(currentSelectHeroID);

                            layout.SetNameIcon();

                            layout.useButton.interactable = !(Sys_Role.Instance.Role.HeroId == currentSelectHeroID);
                        });
                        items[kvp.id] = item;

                        itemObj.transform.SetParent(layout.heroRoot.transform, false);
                    }
                }
            }
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_MagicChange);
        }

        public void OnClickUseButton()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushForbidOprationInFight();
                return;
            }

            if (currentSelectHeroID == Sys_Role.Instance.Role.HeroId)
            {
                return;
            }

            if (Sys_Bag.Instance.GetItemCount(costItemID) < costItemCount)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                return;
            }

            void OnConform()
            {
                Sys_MagicChange.Instance.ReqMagicShapeShift(currentSelectHeroID);
            }

            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13497, LanguageHelper.GetTextContent(CSVCharacter.Instance.GetConfData(Sys_Role.Instance.HeroId).name), LanguageHelper.GetTextContent(CSVCharacter.Instance.GetConfData(currentSelectHeroID).name));
            PromptBoxParameter.Instance.SetConfirm(true, OnConform);
            PromptBoxParameter.Instance.SetCancel(true, null);

            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
    }
}
