using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Framework;
using DG.Tweening;

namespace Logic
{
    //Boss特性界面
    public class UI_BossTower_Feature : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnHide()
        {
            OnDestroyModel();
        }
        protected override void OnClose()
        {
            OnDestroyModel();
            for (int i = 0; i < btnFeatures.Length; i++)
            {
                btnFeatures[i].onClick.RemoveAllListeners();
            }
            btnFeatures = null;
            textFeatures = null;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        RawImage rawImage;
        Text bossName;
        Button[] btnFeatures = new Button[4];
        Text[] textFeatures = new Text[4];
        Button btnGoMove;
        InfinityGrid infinityGrid;
        Transform parent;
        Text checkText;
        #endregion
        #region 数据
        AssetDependencies assetDependencies;
        ShowSceneControl showSceneControl;
        DisplayControl<EPetModelParts> displayControl;

        CSVBOSSTower.Data curFeatureData;
        List<BossTowerFeatureGroupData> bossTowerFeatureGroupDataList = new List<BossTowerFeatureGroupData>();
        List<FloorItem> floorItemList = new List<FloorItem>();
        static uint curGroupId;
        static uint defaultGroupId;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            assetDependencies = transform.GetComponent<AssetDependencies>();
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/View_Left/Scroll View").GetComponent<InfinityGrid>();
            parent = transform.Find("Animator/View_Left/Grid");
            rawImage = transform.Find("Animator/View_Right/Image_Boss").GetComponent<RawImage>();
            bossName = transform.Find("Animator/View_Right/Name_Bottom/Text_Name").GetComponent<Text>();
            Transform specificTrans = transform.Find("Animator/View_Right/Specific");
            for (int i = 0; i < 4; i++)
            {
                btnFeatures[i] = specificTrans.GetChild(i).GetComponent<Button>();
                if (i == 0)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(0); });
                else if (i == 1)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(1); });
                else if (i == 2)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(2); });
                else if (i == 3)
                    btnFeatures[i].onClick.AddListener(() => { OnClickFeature(3); });
                textFeatures[i] = specificTrans.GetChild(i).Find("Text").GetComponent<Text>();
            }
            btnGoMove = transform.Find("Animator/View_Right/Btn_01").GetComponent<Button>();
            checkText = transform.Find("Animator/CheckText").GetComponent<Text>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });
            btnGoMove.onClick.AddListener(OnClickGoMove);

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnRefreshBossTowerReset, OnRefreshBossTowerReset, toRegister);
        }
        private void OnRefreshBossTowerReset()
        {
            RefreshItem();
        }
        float defaultSize = 176;
        private void OnClickFeature(int index)
        {
            if (curFeatureData != null && curFeatureData.textDetails_id != null)
            {
                Button btn = btnFeatures[index];
                uint lanId = curFeatureData.textDetails_id[index];
                checkText.text = LanguageHelper.GetTextContent(lanId);
                float diffY = checkText.preferredHeight / defaultSize;
                Vector3 btnPos = btn.GetComponent<RectTransform>().position;
                Vector3 worldPos = new Vector3(btnPos.x, btnPos.y + diffY, btnPos.z);
                UIRuleParam param = new UIRuleParam();
                param.StrContent = LanguageHelper.GetTextContent(lanId);
                param.Pos = CameraManager.mUICamera.WorldToScreenPoint(worldPos);
                UIManager.OpenUI(EUIID.UI_Rule, false, param);
            }
        }
        //寻路npc
        private void OnClickGoMove()
        {
            uint lanId = Sys_ActivityBossTower.Instance.CheckCurBossTowerState();
            if (lanId != 0) return;
            if (curFeatureData != null)
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(curFeatureData.npc_id);
                UIManager.CloseUI(EUIID.UI_DailyActivites);
                CloseSelf();
            }
        }
        #endregion
        private void InitView()
        {
            if (Sys_Team.Instance.HaveTeam)
            {
                if (Sys_Team.Instance.isCaptain())
                    btnGoMove.gameObject.SetActive(true);
                else
                    btnGoMove.gameObject.SetActive(false);
            }
            else
                btnGoMove.gameObject.SetActive(true);
            RefreshItem();
        }
        private void RefreshFeatureData()
        {
            btnGoMove.gameObject.SetActive(curGroupId == defaultGroupId);
            if (curFeatureData != null)
            {
                bossName.text = LanguageHelper.GetTextContent(curFeatureData.bossName);
                SetTextFeatures();
                if (curFeatureData.text_id != null && curFeatureData.text_id.Count > 0)
                {
                    for (int i = 0; i < curFeatureData.text_id.Count; i++)
                    {
                        btnFeatures[i].gameObject.SetActive(true);
                        textFeatures[i].text = LanguageHelper.GetTextContent(curFeatureData.text_id[i]);
                    }
                }
            }
        }
        private void SetTextFeatures()
        {
            for (int i = 0; i < btnFeatures.Length; i++)
            {
                btnFeatures[i].gameObject.SetActive(false);
            }
        }
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
                showSceneControl = new ShowSceneControl();

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }
        private void _LoadShowModel()
        {
            if (curFeatureData != null)
            {
                displayControl = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                displayControl.onLoaded = OnShowModelLoaded;
                displayControl.eLayerMask = ELayerMask.ModelShow;

                displayControl.LoadMainModel(EPetModelParts.Main, curFeatureData.model_show, EPetModelParts.None, null);
                displayControl.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(0, curFeatureData.positiony / 10000f * 2.5f, 0);
                showSceneControl.mModelPos.transform.localEulerAngles = new Vector3(curFeatureData.rotationx / 10000f, curFeatureData.rotationy / 10000f, curFeatureData.rotationz / 10000f);
                showSceneControl.mModelPos.transform.localScale = new Vector3(curFeatureData.scale / 10000f, curFeatureData.scale / 10000f, curFeatureData.scale / 10000f);
            }
        }
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                GameObject mainGo = displayControl.GetPart(EPetModelParts.Main).gameObject;
                mainGo?.SetActive(false);
                displayControl.mAnimation.UpdateHoldingAnimations(curFeatureData.action_show_id, Constants.UMARMEDID, null, EStateType.Idle, mainGo);
            }
        }

        private void OnDestroyModel()
        {
            DisplayControl<EPetModelParts>.Destory(ref displayControl);
            displayControl?.Dispose();
            showSceneControl?.Dispose();
            showSceneControl = null;
        }
        #endregion
        private void RefreshItem()
        {
            curFeatureData = Sys_ActivityBossTower.Instance.GetBossTowerFeature();
            if (curFeatureData != null)
                curGroupId = defaultGroupId = curFeatureData.order;
            bossTowerFeatureGroupDataList = Sys_ActivityBossTower.Instance.bossTowerFeatureGroupDataList;
            if (bossTowerFeatureGroupDataList != null && bossTowerFeatureGroupDataList.Count > 0)
            {
                parent.gameObject.SetActive(bossTowerFeatureGroupDataList.Count > 1);
                RefreshFloorItem();
                RefreshProtoItem();
            }
        }
        List<ProtoItem> protoItemList = new List<ProtoItem>();
        private void RefreshProtoItem()
        {
            for (int i = 0; i < protoItemList.Count; i++)
            {
                ProtoItem cell = protoItemList[i];
                PoolManager.Recycle(cell);
            }
            protoItemList.Clear();
            FrameworkTool.CreateChildList(parent, bossTowerFeatureGroupDataList.Count);
            for (int i = 0; i < bossTowerFeatureGroupDataList.Count; i++)
            {
                Transform trans = parent.GetChild(i);
                ProtoItem cell = PoolManager.Fetch<ProtoItem>();
                cell.Init(trans);
                cell.SetData(bossTowerFeatureGroupDataList[i], OnRefreshSelect);
                protoItemList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void OnRefreshSelect(uint groupId)
        {
            curGroupId = groupId;
            curFeatureData = bossTowerFeatureGroupDataList.Find(o => o.groupId == groupId).GetCurBossTowerFeatureData();
            //for (int i = 0; i < floorItemList.Count; i++)
            //{
            //    if (curGroupId == floorItemList[i].data.groupId)
            //        floorItemList[i].Select();
            //    else
            //        floorItemList[i].Release();
            //}
            int index = bossTowerFeatureGroupDataList.FindIndex(o => o.groupId == groupId);
            MoveToIndex(index);
            RefreshFeatureData();
            OnCreateModel();
        }
        Timer moveTimer;
        private void MoveToIndex(int index)
        {
            if (index != -1)
            {
                float posX = ((infinityGrid.Spacing.x + infinityGrid.CellSize.x) * 0.5f + (index - 1) * (infinityGrid.Spacing.x + infinityGrid.CellSize.x)) * -1;
                float minContentX = bossTowerFeatureGroupDataList.Count <= 2 ? 0 : ((bossTowerFeatureGroupDataList.Count - 2) * (infinityGrid.CellSize.x + (bossTowerFeatureGroupDataList.Count - 1) * infinityGrid.Spacing.x) - 54) * -1;
                posX = posX >= 0 ? 0 : posX < minContentX ? minContentX : posX;
                infinityGrid.Content.DOLocalMoveX(Mathf.Min(infinityGrid.ContentSize.x >= 0 ? 0 : infinityGrid.ContentSize.x, posX), 0.3f);
                moveTimer?.Cancel();
                moveTimer = Timer.Register(0.3f, () =>
                {
                    moveTimer?.Cancel();
                    infinityGrid.Content.localPosition = new Vector3(posX, infinityGrid.Content.localPosition.y, 0);
                }, null, false, true);
            }
        }
        private void RefreshFloorItem()
        {
            infinityGrid.CellCount = bossTowerFeatureGroupDataList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            FloorItem entry = new FloorItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            floorItemList.Add(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            FloorItem entry = cell.mUserData as FloorItem;
            entry.SetData(bossTowerFeatureGroupDataList[index], RefreshProtoSelect);//索引数据
        }
        private void RefreshProtoSelect(BossTowerFeatureGroupData data)
        {
            curGroupId = data.groupId;
            curFeatureData = data.GetCurBossTowerFeatureData();
            for (int i = 0; i < protoItemList.Count; i++)
            {
                if (protoItemList[i].data.groupId == curGroupId)
                    protoItemList[i].Select();
                else
                    protoItemList[i].Release();
            }
            int index = bossTowerFeatureGroupDataList.FindIndex(o => o.groupId == curGroupId);
            MoveToIndex(index);
            RefreshFeatureData();
            OnCreateModel();
        }
        #region item
        public class ProtoItem
        {
            CP_Toggle toggle;
            public BossTowerFeatureGroupData data;
            Action<uint> action;
            public void Init(Transform trans)
            {
                toggle = trans.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnValueChanged);
            }
            public void SetData(BossTowerFeatureGroupData data, Action<uint> action)
            {
                this.data = data;
                this.action = action;
                if (data.groupId == curGroupId)
                {
                    Select();
                    action?.Invoke(data.groupId);
                }
            }
            private void OnValueChanged(bool isOn)
            {
                if (isOn)
                    action?.Invoke(data.groupId);
            }
            public void Select()
            {
                toggle.SetSelected(true, false);
            }
            public void Release()
            {
                toggle.SetSelected(false, false);
            }
        }
        public class FloorItem
        {
            RawImage rawImage;
            GameObject objSelect;
            Button btn;
            public BossTowerFeatureGroupData data;
            CSVBOSSTower.Data featureData;
            Action<BossTowerFeatureGroupData> action;
            public void Init(Transform trans)
            {
                rawImage = trans.GetComponent<RawImage>();
                objSelect = trans.Find("Image_select").gameObject;
                btn = trans.GetComponent<Button>();

                btn.onClick.AddListener(OnClick);
            }
            public void SetData(BossTowerFeatureGroupData data,Action<BossTowerFeatureGroupData> action)
            {
                this.data = data;
                this.action = action;
                if (data != null)
                    featureData = data.GetCurBossTowerFeatureData();
                if (featureData != null)
                    ImageHelper.SetTexture(rawImage, featureData.towerPicture, true);
                if (defaultGroupId == data.groupId)
                    Select();
                else
                    Release();
            }
            private void OnClick()
            {
                if (data != null)
                    action?.Invoke(data);
            }
            public void Select()
            {
                objSelect.SetActive(true);
            }
            public void Release()
            {
                objSelect.SetActive(false);
            }
        }
        #endregion
    }
}