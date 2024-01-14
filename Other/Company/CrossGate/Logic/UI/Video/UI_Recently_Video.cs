using Packet;
using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;

namespace Logic
{ 
    public class UI_Recently_Video : UIComponent
    {
        private Text curType;
        private  InputField nameInputField;
        private Button typeBtn;
        private Button searchBtn;
        private GameObject challengeGo;
        private GameObject pkGo;
        private Transform challengeParent;
        private Transform pkParent;
        private InfinityGrid infinityGrid;
        private InfinityGrid infinityChallengGrid;
        private InfinityGrid infinityPkGrid;
        private int infinityCount;
        private int infinityChallengeCount;
        private int infinityPkCount;
        private List<ClientVideo> videoList = new List<ClientVideo>();
        private List<UI_Video_Type> challengeTypeList = new List<UI_Video_Type>();
        private List<UI_Video_Type> pkTypeList = new List<UI_Video_Type>();

        #region 系统函数

        protected override void Loaded()
        {
            curType = transform.Find("Btn_Screen/Text").GetComponent<Text>();
            nameInputField = transform.Find("InputField").GetComponent<InputField>();
            challengeGo = transform.Find("List_01").gameObject;
            pkGo = transform.Find("List_02").gameObject;
            typeBtn = transform.Find("Btn_Screen").GetComponent<Button>();
            searchBtn = transform.Find("Btn_Search").GetComponent<Button>();
            typeBtn.onClick.AddListener(OnType_ButtonClicked);
            searchBtn.onClick.AddListener(OnSearch_ButtonClicked);
            challengeParent = transform.Find("List_01/Scroll View/Viewport/Content").transform;
            pkParent = transform.Find("List_02/Scroll View/Viewport/Content").transform;
            infinityGrid = transform.Find("Content/Scroll_View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
            infinityChallengGrid = transform.Find("List_01/Scroll View").GetComponent<InfinityGrid>();
            infinityChallengGrid.onCreateCell += OnCreateChallengeTypeCell;
            infinityChallengGrid.onCellChange += OnCellChallengeTypeChange;
            infinityPkGrid = transform.Find("List_02/Scroll View").GetComponent<InfinityGrid>();
            infinityPkGrid.onCreateCell += OnCreatePkTypeCell;
            infinityPkGrid.onCellChange += OnCellPkTypeChange;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForAwake(toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnLastUpload, LastUpload, toRegister);
            Sys_Video.Instance.eventEmitter.Handle<uint>(Sys_Video.EEvents.OnSelectViewType, SelectViewType, toRegister);
        }

        public override void Show()
        {
            base.Show();
            challengeGo.SetActive(false);
            pkGo.SetActive(false);
            nameInputField.text = string.Empty;
            Sys_Video.Instance.LastUploadListReq(0, 0);
            TextHelper.SetText(curType, 2023205);
        }

        public override void Hide()
        {
            base.Hide();
            infinityGrid.Clear();
            infinityChallengGrid.Clear();
            infinityPkGrid.Clear();
            videoList.Clear();
        }

        #endregion

        #region  事件回调
        private void LastUpload()
        {
            challengeGo.SetActive(false);
            pkGo.SetActive(false);
            videoList.Clear();
            videoList.AddRange(Sys_Video.Instance.recentlyList);
            InitRecentlyVideo();
        }

        private void SelectViewType(uint type)
        {
            if (CSVVideoType.Instance.TryGetValue(type, out Framework.Table.FCSVVideoType.Data data))
            {
                if (data.Type == 1)
                {
                    for (int i = 0; i < pkTypeList.Count; ++i)
                    {
                        pkTypeList[i].SetToggleOff();
                    }
                }
                else
                {
                    for (int i = 0; i < challengeTypeList.Count; ++i)
                    {
                        challengeTypeList[i].SetToggleOff();
                    }
                }
                TextHelper.SetText(curType, data.Name);
            }
        }
        #endregion

        #region Function  
        private void InitRecentlyVideo()
        {
            infinityCount = videoList.Count;
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
        }

        private void SetTypeView(bool isShow)
        {
            challengeTypeList.Clear();
            pkTypeList.Clear();
            if (isShow)
            {
                infinityChallengeCount = Sys_Video.Instance.challengeTypeList.Count;
                infinityPkCount = Sys_Video.Instance.pkTypeList.Count;
                infinityChallengGrid.CellCount = infinityChallengeCount;
                infinityChallengGrid.ForceRefreshActiveCell();
                infinityPkGrid.CellCount = infinityPkCount;
                infinityPkGrid.ForceRefreshActiveCell();
                challengeParent.localPosition = Vector3.zero;
                pkParent.localPosition = Vector3.zero;
            }
            else
            {
                infinityChallengGrid.Clear();
                infinityPkGrid.Clear();
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Video_Item entry = new UI_Video_Item();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnItemSelect);
            cell.BindUserData(entry);
        }

        private void OnItemSelect(UI_Video_Item item)
        {
            UIManager.OpenUI(EUIID.UI_VideoDetails, false, item.clientVideo);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            ClientVideo clientVideo = new ClientVideo();
            clientVideo = videoList[index];
            UI_Video_Item entry = cell.mUserData as UI_Video_Item;
            entry.SetData(clientVideo,index);
        }
        private void OnCreateChallengeTypeCell(InfinityGridCell cell)
        {
            UI_Video_Type entry = new UI_Video_Type();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            cell.BindUserData(entry);
        }

        private void OnCellChallengeTypeChange(InfinityGridCell cell, int index)
        {
            uint Id = Sys_Video.Instance.challengeTypeList[index];
            UI_Video_Type entry = cell.mUserData as UI_Video_Type;
            entry.SetData(Id, VideoButton.LastUp);
            challengeTypeList.Add(entry);
        }

        private void OnCreatePkTypeCell(InfinityGridCell cell)
        {
            UI_Video_Type entry = new UI_Video_Type();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            cell.BindUserData(entry);
        }

        private void OnCellPkTypeChange(InfinityGridCell cell, int index)
        {
            uint Id = Sys_Video.Instance.pkTypeList[index];
            UI_Video_Type entry = cell.mUserData as UI_Video_Type;
            entry.SetData(Id, VideoButton.LastUp);
            pkTypeList.Add(entry);
        }

        #endregion

        #region 按钮响应

        public void OnSearch_ButtonClicked()
        {
            ulong.TryParse(nameInputField.text, out ulong Id);
            videoList.Clear();
            videoList.AddRange(Sys_Video.Instance.SearchVideoListByAuthorId(Id, EVideoViewType.RecentlyVideoView));
            InitRecentlyVideo();
        }

        private void OnType_ButtonClicked()
        {
            if (challengeGo.activeInHierarchy)
            {
                SetTypeView(false);
               challengeGo.SetActive(false);
                pkGo.SetActive(false);
            }
            else
            {
                challengeGo.SetActive(true);
                pkGo.SetActive(true);
                SetTypeView(true);
            }
        }

        #endregion
    }
}
