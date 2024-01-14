using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using System;

namespace Logic
{
    public class UI_Adventure_Map : UIComponent
    {
        public EAdventurePageType PageType { get; } = EAdventurePageType.Map;

        #region 界面组件
        private GameObject scrollCell;
        private RectTransform content;
        private GridLayoutGroup group;
        #endregion
        //顺序遍历，第一个未完成探索的地图id
        public static uint firstUnfinishMapId = 0;
        private Dictionary<uint, uint> dictCellIndex = new Dictionary<uint, uint>();

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            Parse();
        }

        public override void Show()
        {
            base.Show();
            UpdateView();
        }

        public override void OnDestroy()
        {
            dictCellIndex.Clear();
            base.OnDestroy();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        #region function
        private void Parse()
        {
            scrollCell = transform.Find("Scroll View/Viewport/Content/Item").gameObject;
            scrollCell.SetActive(false);
            content = transform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            group = transform.Find("Scroll View/Viewport/Content").GetComponent<GridLayoutGroup>();
        }

        private void UpdateView()
        {
            dictCellIndex.Clear();
            firstUnfinishMapId = 0;
            FrameworkTool.DestroyChildren(scrollCell.transform.parent.gameObject, scrollCell.name);

            //List<uint> mapIds = Sys_Adventure.Instance.MapIds;

            var mapDatas = CSVAdventureMap.Instance.GetAll();
            for (int i = 0, len = mapDatas.Count; i < len; i++)                            
            {
                GameObject go = GameObject.Instantiate<GameObject>(scrollCell, scrollCell.transform.parent);
                go.SetActive(true);
                uint mapId = mapDatas[i].id;
                UI_MapScrollCell cell = new UI_MapScrollCell(mapId);
                cell.Init(go.transform);
                dictCellIndex.Add(mapId, (uint)i);
                cell.UpdateCellView();
            }
            UpdateScrollPos();
        }
        private void UpdateScrollPos()
        {
            if (firstUnfinishMapId != 0)
            {
                dictCellIndex.TryGetValue(firstUnfinishMapId, out uint index);
                float cellX = group.cellSize.x;
                float spX = group.spacing.x;
                float offSetX = 0 - index * (cellX + spX);
                content.anchoredPosition = new Vector3(offSetX, 0, 0);
            }
        }
        #endregion

        #region 响应事件

        #endregion

        public class UI_MapScrollCell : UIComponent
        {
            private uint adventrueMapId;
            private bool isLock;

            private Button btnBlank;
            private Button btnGoto;
            private Image imgMapIcon;
            private Text txtMapName;
            private Text txtMapDesc;
            private Text txtLock;
            private GameObject goLock;
            private GameObject goFinish;
            private GameObject exploreCell;
            private GameObject goBtnGoto;

            public UI_MapScrollCell(uint mapId)
            {
                adventrueMapId = mapId;
            }

            protected override void Loaded()
            {
                base.Loaded();
                btnBlank = transform.GetComponent<Button>();
                btnBlank.onClick.AddListener(OnBtnBlankClick);
                imgMapIcon = transform.Find("Map_Box/Map").GetComponent<Image>();
                txtMapName = transform.Find("Text_Name").GetComponent<Text>();
                txtMapDesc = transform.Find("Text_Describe").GetComponent<Text>();
                txtLock = transform.Find("Lock/Text").GetComponent<Text>();
                goLock = transform.Find("Lock").gameObject;
                goFinish = transform.Find("Finish").gameObject;
                exploreCell = transform.Find("ExploreGrp/Item").gameObject;
                exploreCell.SetActive(false);
                btnGoto = transform.Find("Button_Goto").GetComponent<Button>();
                btnGoto.onClick.AddListener(OnBtnBlankClick);
                goBtnGoto = transform.Find("Button_Goto").gameObject;
            }
            public void UpdateCellView()
            {
                CSVAdventureMap.Data advMapData = CSVAdventureMap.Instance.GetConfData(adventrueMapId);
                if (advMapData != null)
                {
                    ImageHelper.SetIcon(imgMapIcon, advMapData.mapIcon);
                    txtMapName.text = LanguageHelper.GetTextContent(advMapData.name);
                    txtMapDesc.text = LanguageHelper.GetTextContent(advMapData.mapDes);
                    uint mapId = advMapData.mapId;
                    CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(mapId);
                    if (mapData != null)
                    {
                        uint userLevel = Sys_Role.Instance.Role.Level;
                        uint unLockLevel = 0;
                        int mapLv = mapData.map_lv[0];
                        if (mapLv > 0)
                        {
                            unLockLevel = (uint)mapLv;
                        }
                        isLock = userLevel < unLockLevel;
                        goLock.SetActive(isLock);
                        if (isLock)
                        {
                            txtLock.text = LanguageHelper.GetTextContent(600000199, unLockLevel.ToString());
                        }
                        FrameworkTool.DestroyChildren(exploreCell.transform.parent.gameObject, exploreCell.name);
                        Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(mapId);
                        List<ENPCMarkType> Keys = new List<ENPCMarkType>(explorationData.dict_Process.Keys);
                        int len = Keys.Count;
                        bool isFinish = false;
                        bool flag = false;
                        for (int i = 0; i < len; i++)
                        {
                            Sys_Exploration.ExplorationProcess explorationProcess = explorationData.GetExplorationProcess(Keys[i]);
                            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData((uint)Keys[i]);
                            GameObject go = GameObject.Instantiate<GameObject>(exploreCell, exploreCell.transform.parent);
                            uint curNum = explorationProcess.CurNum;
                            uint maxNum = explorationProcess.TargetNum;
                            go.SetActive(true);
                            Text txtTitle = go.transform.Find("Text_Name").GetComponent<Text>();
                            txtTitle.text = LanguageHelper.GetTextContent(cSVMapExplorationMarkData.List_lan);
                            Text txtValue = go.transform.Find("Text_Value").GetComponent<Text>();
                            if (curNum < maxNum)
                            {
                                flag = true;
                                if (firstUnfinishMapId == 0)
                                {
                                    firstUnfinishMapId = adventrueMapId;
                                }
                                txtValue.text = LanguageHelper.GetTextContent(600000195, curNum.ToString(), maxNum.ToString());
                            }
                            else
                            {
                                txtValue.text = LanguageHelper.GetTextContent(600000194, curNum.ToString(), maxNum.ToString());
                            }
                        }
                        if (!flag)
                        {
                            isFinish = true;
                        }
                        goFinish.SetActive(isFinish);
                        goBtnGoto.SetActive(!isLock && !isFinish);
                    }
                }
            }

            private void OnBtnBlankClick()
            {
                if (isLock)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(600000196));
                }
                else
                {
                    CSVAdventureMap.Data advMapData = CSVAdventureMap.Instance.GetConfData(adventrueMapId);
                    UIManager.OpenUI(EUIID.UI_Map, false, new Sys_Map.TargetMapParameter(advMapData.mapId));
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Map_Cell_MapId:" + advMapData.mapId);
                }

            }
        }
    }
}
