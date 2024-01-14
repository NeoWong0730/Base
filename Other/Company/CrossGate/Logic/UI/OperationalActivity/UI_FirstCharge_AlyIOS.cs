using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using Packet;
using System;
using Framework;

namespace Logic
{
    public class UI_FirstCharge_AlyIOS : UIBase
    {
        private CSVFirstCharge.Data firstChargeData;
        private uint selectDayNum = 1;

        private Button btnClose;
        private RawImage rawImgReward;
        private Text txtReward;
        private Text txtReward2;
        private Text txtTips;
        private CP_ToggleRegistry toggleRegistry;
        private Button btnGet;
        private Text txtbtnGet;
        private Button btnGoCharge;
        private GameObject goLeftArrow;
        private GameObject goRightArrow;
        private ScrollRect scroll;

        private InfinityGrid infinity;
        private Dictionary<GameObject, UI_FirstRechargeCell> CeilGrids = new Dictionary<GameObject, UI_FirstRechargeCell>();
        private List<List<int>> rewards = new List<List<int>>();
        private List<GameObject> listRedPoints = new List<GameObject>();

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private Image eventImage;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;
        private Rotater rotater;

        #region 系统函数
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            toggleRegistry.SwitchTo(Sys_OperationalActivity.Instance.GetFirstChargeShowDay());
        }
        protected override void OnHide()
        {
            OnDestroyModel();
        }
        protected override void OnDestroy()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateFirstChargeGiftData, OnUpdateFirstChargeGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_Bg/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            rawImgReward = transform.Find("Animator/View_Left/Texture").GetComponent<RawImage>();
            txtReward = transform.Find("Animator/View_Left/Text").GetComponent<Text>();
            txtReward2 = transform.Find("Animator/View_Left/Text2").GetComponent<Text>();
            txtTips = transform.Find("Animator/View_Right/Tips_1").GetComponent<Text>();
            btnGet = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            btnGet.onClick.AddListener(OnBtnGetClick);
            btnGoCharge = transform.Find("Animator/View_Right/Btn_02").GetComponent<Button>();
            btnGoCharge.onClick.AddListener(OnBtnGoChargeClick);
            txtbtnGet = transform.Find("Animator/View_Right/Btn_01/Text").GetComponent<Text>();
            goLeftArrow = transform.Find("Animator/View_Right/Btn_Arrow_Left").gameObject;
            goRightArrow = transform.Find("Animator/View_Right/Btn_Arrow_Right").gameObject;
            scroll = transform.Find("Animator/View_Right/Scroll View").GetComponent<ScrollRect>();
            scroll.onValueChanged.AddListener(OnScrollValueChanged);

            infinity = transform.Find("Animator/View_Right/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;

            toggleRegistry = transform.Find("Animator/View_Right/List").GetComponent<CP_ToggleRegistry>();
            toggleRegistry.onToggleChange = OnDaySelect;

            listRedPoints.Clear();
            GameObject goPoint1 = toggleRegistry.transform.Find("ListItem/Image_Dot").gameObject;
            listRedPoints.Add(goPoint1);
            GameObject goPoint2 = toggleRegistry.transform.Find("ListItem (1)/Image_Dot").gameObject;
            listRedPoints.Add(goPoint2);
            GameObject goPoint3 = toggleRegistry.transform.Find("ListItem (2)/Image_Dot").gameObject;
            listRedPoints.Add(goPoint3);

            assetDependencies = transform.GetComponent<AssetDependencies>();
            eventImage = transform.Find("Animator/View_Left/EventImage").GetComponent<Image>();
        }
        private void UpdateView()
        {
            Sys_OperationalActivity.Instance.SetFirstChargeFirstRedPoint();
            firstChargeData = Sys_OperationalActivity.Instance.GetFirstChargeGiftData();
            if (firstChargeData != null)
            {
                //ios不修改道具描述
                //txtTips.text = LanguageHelper.GetTextContent(firstChargeData.Title_Des);

                UpdateLeftView();
                List<uint> rewardDesc = new List<uint>();
                switch (selectDayNum)
                {
                    case 1:
                        rewards = firstChargeData.Reward_Items_d1;
                        rewardDesc = firstChargeData.Item_Des_d1;
                        break;
                    case 2:
                        rewards = firstChargeData.Reward_Items_d2;
                        rewardDesc = firstChargeData.Item_Des_d2;
                        break;
                    case 3:
                        rewards = firstChargeData.Reward_Items_d3;
                        rewardDesc = firstChargeData.Item_Des_d3;
                        break;
                }
                if (rewardDesc.Count >= 2)
                {
                    txtReward.text = LanguageHelper.GetTextContent(rewardDesc[0]);//道具描述
                    txtReward2.text = LanguageHelper.GetTextContent(rewardDesc[1]);//道具描述2
                }

                bool canMove = rewards.Count > 3;
                scroll.enabled = canMove;
                scroll.normalizedPosition = new Vector2(0, 0);
                goLeftArrow.SetActive(false);
                goRightArrow.SetActive(canMove);
                infinity.CellCount = rewards.Count;
                infinity.ForceRefreshActiveCell();
                UpdateBtnState();
                UpdateBtnRedPoint();
            }
        }
        private void UpdateLeftView()
        {
            OnCreateModel();
        }
        private void UpdateBtnState()
        {
            bool isCharge = Sys_OperationalActivity.Instance.IsFirstCharge;
            ImageHelper.SetImageGray(btnGet.transform.GetComponent<Image>(), false, true);
            btnGoCharge.gameObject.SetActive(!isCharge);
            if (isCharge)
            {
                uint state = Sys_OperationalActivity.Instance.ListFirstChargeGiftState[(int)selectDayNum - 1];
                switch (state)
                {
                    case (uint)ChargeRewardStatus.None:
                        if (selectDayNum == 2)
                        {
                            txtbtnGet.text = LanguageHelper.GetTextContent(11745);//第二天领取
                        }
                        else if (selectDayNum == 3)
                        {
                            txtbtnGet.text = LanguageHelper.GetTextContent(11746);//第三天领取
                        }
                        break;
                    case (uint)ChargeRewardStatus.CanReceive:
                        txtbtnGet.text = LanguageHelper.GetTextContent(11744);//领取
                        break;
                    case (uint)ChargeRewardStatus.Receivid:
                        txtbtnGet.text = LanguageHelper.GetTextContent(4725);//已领取
                        ImageHelper.SetImageGray(btnGet.transform.GetComponent<Image>(), true, true);
                        break;
                }
            }
            else
            {
                txtbtnGet.text = LanguageHelper.GetTextContent(4908);//领取红包
            }
        }
        private void UpdateBtnRedPoint()
        {
            var states = Sys_OperationalActivity.Instance.ListFirstChargeGiftState;
            for (int i = 0; i < states.Count; i++)
            {
                if (listRedPoints.Count > i)
                {
                    bool canGet = states[i] == (uint)ChargeRewardStatus.CanReceive;
                    listRedPoints[i].SetActive(canGet);
                }
            }
        }
        #endregion

        #region event
        private void OnBtnGetClick()
        {
            if (Sys_OperationalActivity.Instance.IsFirstCharge)
            {
                uint state = Sys_OperationalActivity.Instance.ListFirstChargeGiftState[(int)selectDayNum - 1];
                if (state == (uint)ChargeRewardStatus.CanReceive)
                {
                    //Sys_OperationalActivity.Instance.ReportFirstChargeClickEventHitPoint("GetReward" + selectDayNum);
                    Sys_OperationalActivity.Instance.GetFirstChargeGiftReq(selectDayNum - 1);
                    UIManager.HitButton(EUIID.UI_FirstCharge_AlyIOS, "GetReward_" + selectDayNum.ToString());
                }
            }
            else
            {
                //跳支付宝IOS红包
                Sys_OperationalActivity.Instance.JunpToAlipayActivityPage();
                UIManager.HitButton(EUIID.UI_FirstCharge_AlyIOS, "GoAlipay");
            }
        }
        private  void OnBtnGoChargeClick()
        {
            //跳转 商城-充值 界面
            MallPrama mallPrama = new MallPrama
            {
                mallId = 101,
                shopId = 1001,
                isCharge = true
            };
            //Sys_OperationalActivity.Instance.ReportFirstChargeClickEventHitPoint("GotoChargeMall");
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
            UIManager.HitButton(EUIID.UI_FirstCharge_AlyIOS, "GoChargeMall");
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_FirstRechargeCell mCell = new UI_FirstRechargeCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FirstRechargeCell mCell = cell.mUserData as UI_FirstRechargeCell;
            mCell.UpdateCellView((uint)rewards[index][0], (uint)rewards[index][1], (uint)rewards[index][2]);
        }
        private void OnBtnCloseClick()
        {
            Sys_OperationalActivity.Instance.ReportFirstChargeClickEventHitPoint("Close");
            this.CloseSelf();
        }
        private void OnDaySelect(int day, int oldDay)
        {
            selectDayNum = (uint)day;
            Sys_OperationalActivity.Instance.ReportFirstChargeClickEventHitPoint("SelectDay" + selectDayNum);
            UpdateView();
        }
        private void OnUpdateFirstChargeGiftData()
        {
            UpdateView();
        }
        private void OnScrollValueChanged(Vector2 v)
        {
            var cellSizeX = 232;
            var cellCount = rewards.Count;
            var spacingX = -27.27f;
            float ContentLen = cellCount * cellSizeX + (cellCount - 1) * spacingX;
            float value = (cellSizeX / 2) / ContentLen;
            bool showLeft = v.x > value;
            bool showRight = cellCount > 3 && v.x < 1 - value;
            goLeftArrow.SetActive(showLeft);
            goRightArrow.SetActive(showRight);
        }
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            if (!Sys_OperationalActivity.Instance.CheckFirstChargeIsShow())
            {
                this.CloseSelf();
            }
        }
        #endregion

        #region  ModelShow
        private void OnCreateModel()
        {
            OnDestroyModel();
            _LoadShowScene();
            _LoadShowModel();
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            rawImgReward.gameObject.SetActive(true);
            rawImgReward.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            string _modelPath = firstChargeData.Show_Item[(int)selectDayNum - 1];

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x, showSceneControl.mModelPos.transform.localPosition.y + float.Parse(firstChargeData.Show_height[(int)selectDayNum - 1]) / 10000, showSceneControl.mModelPos.transform.localPosition.z);
            var euler = firstChargeData.spin_coordinate[(int)selectDayNum - 1];
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(float.Parse(euler[0]), float.Parse(euler[1]), float.Parse(euler[2]));
            var size = (float)firstChargeData.Item_Size[(int)selectDayNum - 1];
            showSceneControl.mModelPos.transform.localScale = new Vector3(size / 10000, size / 10000, size / 10000);
            rotater = showSceneControl.mModelPos.gameObject.AddComponent<Rotater>();
            rotater.speed = firstChargeData.spin_speed;
            rotater.rotateType = (Rotater.ERotateType)firstChargeData.spin_axle;
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;
                uint weaponId = Constants.UMARMEDID;
                uint charId = uint.Parse(firstChargeData.Show_Id[(int)selectDayNum - 1]);
                if (charId > 0)
                {
                    petDisplay.mAnimation.UpdateHoldingAnimations(charId, weaponId);
                    GameObject mainGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                    mainGo.SetActive(false);
                    _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, petDisplay.mAnimation, mainGo);
                }
            }
        }

        private void OnDestroyModel()
        {
            _uiModelShowManagerEntity?.Dispose();
            _uiModelShowManagerEntity = null;
            rawImgReward.gameObject.SetActive(false);
            rawImgReward.texture = null;
            rotater = null;
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }
        #endregion

        public class UI_FirstRechargeCell
        {
            private Transform transform;
            private Text txtName;
            private PropItem propItem;

            public void Init(Transform trans)
            {
                transform = trans;
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);
            }

            public void UpdateCellView(uint id, uint count, uint modId)
            {
                CSVItem.Data item = CSVItem.Instance.GetConfData(id);
                if (item != null)
                {
                    txtName.text = LanguageHelper.GetTextContent(item.name_id);
                    PropIconLoader.ShowItemData itemData;
                    if (Sys_OperationalActivity.Instance.CheckFirstChargeNeedSpecialPop(id))
                    {
                        itemData = new PropIconLoader.ShowItemData(id, count, true, false, false, false, false, true, false, true, OnItemClick, true, false);
                    }
                    else
                    {
                        itemData = new PropIconLoader.ShowItemData(id, count, true, false, false, false, false, true, false, true, OnClick, true, false);
                    }
                    if (item.type_id == (uint)EItemType.Equipment)
                    {
                        itemData.SetUnSpawnEquipQualityByEquipParamId(modId);
                    }
                    propItem.SetData(itemData, EUIID.UI_FirstCharge);
                }
            }

            private void OnClick(PropItem item)
            {
                ItemData mItemData = new ItemData(0, 0, item.ItemData.id, (uint)item.ItemData.count, 0, false, false, null, null, 0);
                uint typeId = mItemData.cSVItemData.type_id;
                if (typeId == (uint)EItemType.Equipment)
                {
                    mItemData.EquipParam = item.ItemData.equipPara;
                    UIManager.OpenUI(EUIID.UI_Equipment_Preview, false, mItemData);
                }
                else
                {
                    //UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);

                    //string[] strs = paramData.str_value.Split('|');
                    PropMessageParam propParam = new PropMessageParam();
                    propParam.itemData = mItemData;
                    propParam.showBtnCheck = false;
                    //propParam.targetEUIID = uint.Parse(strs[2]);
                    //if (strs.Length >= 4)
                    //{
                    //    propParam.checkOpenParam = uint.Parse(strs[3]);
                    //}
                    propParam.sourceUiId = EUIID.UI_FirstCharge;
                    UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                }
            }

            private void OnItemClick(PropItem item)
            {
                ItemData mItemData = new ItemData(0, 0, item.ItemData.id, (uint)item.ItemData.count, 0, false, false, null, null, 0);
                CSVParam.Data paramData = Sys_OperationalActivity.Instance.GetFirstChargeSpecialPopParam(item.ItemData.id);
                if (paramData != null)
                {
                    uint typeId = mItemData.cSVItemData.type_id;
                    if (typeId == (uint)EItemType.Equipment)
                    {
                        mItemData.EquipParam = item.ItemData.equipPara;
                        UIManager.OpenUI(EUIID.UI_Equipment_Preview, false, mItemData);
                    }
                    else
                    {
                        string[] strs = paramData.str_value.Split('|');
                        PropMessageParam propParam = new PropMessageParam();
                        propParam.itemData = mItemData;
                        propParam.showBtnCheck = true;
                        propParam.targetEUIID = uint.Parse(strs[2]);
                        if (strs.Length >= 4)
                        {
                            propParam.checkOpenParam = uint.Parse(strs[3]);
                        }
                        propParam.sourceUiId = EUIID.UI_FirstCharge;
                        UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                    }
                }
            }
        }
    }
}
