using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;

namespace Logic
{
    /// <summary> 七日登入 </summary>
    public class UI_SevenDaysSign : UI_OperationalActivityBase
    {
        #region 界面组件
        /// <summary> 时间节点 </summary>
        private GameObject go_TimeNode;
        /// <summary> 剩余时间-天 </summary>
        private Text text_Day;
        /// <summary> 剩余时间-时 </summary>
        private Text text_Hour;
        /// <summary> 剩余时间-分 </summary>
        private Text text_Minute;
        /// <summary> 签到模版列表 </summary>
        private List<GameObject> list_SignItem = new List<GameObject>();
        /// <summary> 模型图片 </summary>
        private RawImage rawImage_Model;
        /// <summary> 模型显示统一脚本 </summary>
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        #endregion
        #region 数据定义
        /// <summary> 道具模版列表 </summary>
        private List<PropItem> list_PropItem = new List<PropItem>();
        /// <summary> 更新行为 </summary>
        public Action updateAction = null;
        /// <summary> 定时器 </summary>
        private Timer timer_SevenDaysSign = null;
        /// <summary> 动画下标 </summary>
        int ani_index = 0;
        #endregion
        #region 系统函数
        protected override void Loaded()
        {
            
        }
        protected override void InitBeforOnShow()
        {
            OnParseComponent();
        }

        public override void OnDestroy()
        {
            _UnloadModel();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            SetModel();
            RefreshView();
        }
        public override void Hide()
        {
            _UnloadModel();
            timer_SevenDaysSign?.Cancel();
            base.Hide();
        }
        protected override void Refresh()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void Update()
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            Transform tr = transform.Find("Card");
            for (int i = 0; i < tr.childCount; i++)
            {
                GameObject go = tr.GetChild(i).gameObject;
                list_SignItem.Add(go);
                GameObject go_Item = go.transform.Find("Item").gameObject;
                PropItem propItem = new PropItem();
                propItem.BindGameObject(go_Item);
                list_PropItem.Add(propItem);
                Button button_Sign = go.transform.Find("on_receive/Group/Button").GetComponent<Button>();
                uint id = (uint)i + 1;
                button_Sign.onClick.AddListener(() => { OnClick_Sign(id); });
            }
            go_TimeNode = transform.Find("Title/Text_Remain").gameObject;
            text_Day = transform.Find("Title/Text_Remain/Text1").GetComponent<Text>();
            text_Hour = transform.Find("Title/Text_Remain/Text2").GetComponent<Text>();
            text_Minute = transform.Find("Title/Text_Remain/Text3").GetComponent<Text>();
            assetDependencies = transform.GetComponent<AssetDependencies>();
            rawImage_Model = transform.Find("Image1/Award/Image_Award").GetComponent<RawImage>();
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSignSevenDayData, OnUpdateSignSevenDayData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle<uint>(Sys_OperationalActivity.EEvents.ReceiveSignReward, OnReceiveSignReward, toRegister);
        }
        #endregion
        #region 外部脚本逻辑
        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(2000, 0, 0);

            showSceneControl.Parse(sceneModel);
            rawImage_Model.gameObject.SetActive(true);
            //设置RenderTexture纹理到RawImage
            rawImage_Model.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }
        private void _LoadShowModel()
        {
            CSVSigninRewardModel.Data cSVSigninRewardModelData = CSVSigninRewardModel.Instance.GetConfData(1);
            if (null == cSVSigninRewardModelData) return;
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
            }
            petDisplay.onLoaded = (int obj) =>
            {
                if (obj == 0)
                {
                    petDisplay.mAnimation.UpdateHoldingAnimations(cSVSigninRewardModelData.action_id, cSVSigninRewardModelData.weapon_id, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, null,
                        () =>
                        {
                            PlayAnimator();
                        });
                }
            };

            string _modelPath = cSVSigninRewardModelData.model;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            Vector3 localRotation = new Vector3(cSVSigninRewardModelData.rotationx / 10000.0f, cSVSigninRewardModelData.rotationy / 10000.0f, cSVSigninRewardModelData.rotationz / 10000.0f);
            Vector3 localScale = new Vector3(cSVSigninRewardModelData.scale / 10000.0f, cSVSigninRewardModelData.scale / 10000.0f, cSVSigninRewardModelData.scale / 10000.0f);
            Vector3 localPosition = new Vector3(cSVSigninRewardModelData.positionx / 10000.0f, cSVSigninRewardModelData.positiony / 10000.0f, cSVSigninRewardModelData.positionz / 10000.0f);
            showSceneControl.mModelPos.transform.localEulerAngles = localRotation;
            showSceneControl.mModelPos.transform.localScale = localScale;
            showSceneControl.mModelPos.transform.localPosition = localPosition;
        }
        public void _UnloadModel()
        {
            _UnloadShowContent();
        }
        private void _UnloadShowContent()
        {
            if (rawImage_Model != null)
            {
                rawImage_Model.gameObject.SetActive(false);
                rawImage_Model.texture = null;
            }
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetTime();
            int count = list_SignItem.Count;
            for (int i = 0; i < count; i++)
            {
                SetSignItem(list_SignItem[i].transform, list_PropItem[i], (uint)(i + 1));
            }
        }
        /// <summary>
        /// 设置模版
        /// </summary>
        private void SetModel()
        {
            _UnloadModel();
            _LoadShowScene();
            _LoadShowModel();
        }
        /// <summary>
        /// 设置时间
        /// </summary>
        private void SetTime()
        {
            updateAction = () =>
            {
                uint time = 0;
                uint endTime = Sys_OperationalActivity.Instance.cmdSignSevenDayDataNtf.EndTime;
                uint nowTime = Sys_Time.Instance.GetServerTime();
                if (endTime > nowTime)
                {
                    time = endTime - nowTime;
                }
                uint day = time / 86400;
                uint hour = time % 86400 / 3600;
                uint minute = time % 3600 / 60;

                if (time < 0)
                {
                    time = 0;
                    go_TimeNode.gameObject.SetActive(false);
                    updateAction = null;
                }
                text_Day.text = day.ToString("D2");
                text_Hour.text = hour.ToString("D2");
                text_Minute.text = minute.ToString("D2");
            };
            go_TimeNode.SetActive(true);
            updateAction?.Invoke();
            timer_SevenDaysSign?.Cancel();
            timer_SevenDaysSign = Timer.Register(1f, () =>
            {
                updateAction?.Invoke();
            }, null, true);
        }
        /// <summary>
        /// 设置签到模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="propItem"></param>
        /// <param name="Id"></param>
        private void SetSignItem(Transform tr, PropItem propItem, uint Id)
        {
            int index = (int)Id - 1;
            var cmdSignSevenDayDataNtf = Sys_OperationalActivity.Instance.cmdSignSevenDayDataNtf;

            bool isUnSign = cmdSignSevenDayDataNtf.LoginCount <= index;
            bool isSigned = Sys_OperationalActivity.Instance.IsSign((int)Id);
            bool isSigning = !isUnSign && !isSigned;

            /// <summary> 未领取节点 </summary>
            GameObject go_UnReceived = tr.Find("un_received").gameObject;
            go_UnReceived.SetActive(isUnSign);
            /// <summary> 正领取节点 </summary>
            GameObject go_OnReceive = tr.Find("on_receive").gameObject;
            go_OnReceive.SetActive(isSigning);
            /// <summary> 已领取节点 </summary>
            GameObject go_Received = tr.Find("off_reveive").gameObject;
            GameObject go_ReceivedMark = tr.Find("received").gameObject;
            go_Received.SetActive(isSigned);
            go_ReceivedMark.SetActive(isSigned);
            /// <summary> 显示未领取状态 </summary>
            GameObject go_UnReceivedState = go_UnReceived.transform.Find("Text_State").gameObject;
            go_UnReceivedState.SetActive(!isSigned);
            /// <summary> 道具名称 </summary>
            Text text_ItemName1 = go_UnReceived.transform.Find("Text_Name").GetComponent<Text>();
            Text text_ItemName2 = go_OnReceive.transform.Find("Group/Image2/Text_Name").GetComponent<Text>();
            Text text_ItemName3 = go_Received.transform.Find("Top/Image2/Text_Name").GetComponent<Text>();
            /// <summary> 道具显示 </summary>
            CSVSigninReward.Data cSVSigninRewardData = CSVSigninReward.Instance.GetConfData(Id);
            if (null == cSVSigninRewardData) return;
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(cSVSigninRewardData.itemId);
            if (list_drop.Count > 0)
            {
                propItem.SetActive(true);
                ItemIdCount itemIdCount = list_drop[0];
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false, true, false, true, OnItemClick, false, false);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, itemData));
                ImageHelper.SetImageGray(propItem.Layout.imgIcon, isSigned, true);
                text_ItemName1.text = LanguageHelper.GetTextContent(itemIdCount.CSV.name_id);
                text_ItemName2.text = text_ItemName1.text;
                text_ItemName3.text = text_ItemName1.text;
            }
            else
            {
                propItem.SetActive(false);
                text_ItemName1.text = string.Empty;
                text_ItemName2.text = string.Empty;
                text_ItemName3.text = string.Empty;
            }
            Animator animator_Sign = tr.GetComponent<Animator>();
            if (null != animator_Sign)
                animator_Sign.enabled = false;
        }

        private void OnItemClick(PropItem itemData)
        {
            if (itemData.ItemData.id == 500023)
            {
                CSVParam.Data paramData = CSVParam.Instance.GetConfData(1332);
                if (paramData != null)
                {
                    string[] strs = paramData.str_value.Split('|');
                    if (uint.Parse(strs[1]) == itemData.ItemData.id)
                    {
                        ItemData mItemData = new ItemData(0, 0, itemData.ItemData.id, (uint)itemData.ItemData.count, 0, false, false, null, null, 0);
                        //mItemData.EquipParam = itemData.ItemData.EquipPara;
                        // mItemData.SetQuality(item.ItemData.Quality);
                        PropMessageParam propParam = new PropMessageParam();
                        propParam.itemData = mItemData;
                        propParam.showBtnCheck = true;
                        propParam.targetEUIID = uint.Parse(strs[2]);
                        propParam.checkOpenParam = uint.Parse(strs[3]);
                        propParam.sourceUiId = EUIID.UI_OperationalActivity;
                        UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                    }
                }
            }
            else
            {
                ItemData mItemData = new ItemData(0, 0, itemData.ItemData.id, (uint)itemData.ItemData.count, 0, false, false, null, null, 0);
                mItemData.EquipParam = itemData.ItemData.EquipPara;
                // mItemData.SetQuality(item.ItemData.Quality);
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = mItemData;
                propParam.showBtnCheck = false;
                propParam.sourceUiId = EUIID.UI_OperationalActivity;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="id"></param>
        private void OnClick_Sign(uint id)
        {
            Sys_OperationalActivity.Instance.SendSignSevenDaySignReq(id);
            UIManager.HitButton(EUIID.UI_OperationalActivity, id.ToString(), EOperationalActivity.SevenDaysSign.ToString());
        }
        /// <summary>
        /// 更新签到数据
        /// </summary>
        private void OnUpdateSignSevenDayData()
        {
            RefreshView();
        }
        /// <summary>
        /// 领取签到奖励
        /// </summary>
        /// <param name="id"></param>
        private void OnReceiveSignReward(uint id)
        {
            int index = (int)id - 1;
            if (index < 0 || index > list_SignItem.Count)
                return;

            GameObject go_Sign = list_SignItem[index];
            Animator animator_Sign = go_Sign.transform.GetComponent<Animator>();
            PropItem propItem = list_PropItem[index];
            ImageHelper.SetImageGray(propItem.Layout.imgIcon, true, true);
            if (null != animator_Sign)
            {
                animator_Sign.enabled = true;
                animator_Sign.Play("Receive");
            }
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 播放动画
        /// </summary>
        private void PlayAnimator()
        {
            if (null == petDisplay || null == petDisplay.mAnimation)
                return;

            if (Constants.UIModelShowAnimationClip.Count > ani_index)
            {
                petDisplay.mAnimation.CrossFade(Constants.UIModelShowAnimationClip[ani_index], Constants.CORSSFADETIME, () =>
                {
                    ani_index += 1;
                    PlayAnimator();
                });
            }
            else
            {
                petDisplay.mAnimation.CrossFade((uint)EStateType.UI_Show_Idle, Constants.CORSSFADETIME);
            }
        }
        #endregion
    }
}