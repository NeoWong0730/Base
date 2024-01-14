using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UIPopdownItem : UISelectableElement {
        public uint optionId;
        public string optionName;

        public Text text;
        public Button button;
        public GameObject highlight;

        protected override void Loaded() {
            this.button = this.transform.GetComponent<Button>();
            this.text = this.transform.Find("Text").GetComponent<Text>();
            this.highlight = this.transform.Find("Image").gameObject;
            this.button.onClick.AddListener(this.OnBtnClicked);
        }

        public void SetHighlight(bool setHighLight = false) {
            this.highlight.SetActive(setHighLight);
        }

        public virtual void Refresh(uint zoneId, int index) {
            this.optionId = zoneId;
        }

        private void OnBtnClicked() {
            this.onSelected?.Invoke((int) this.optionId, true);
        }

        public override void SetSelected(bool toSelected, bool force) {
            this.OnBtnClicked();
        }
    }

    public class UI_MapFilterNpc : UIBase {
        public class SearchItem : UIComponent {
            public Text npcName;
            public Text mapName;
            public Text coord;
            public Text ownerFunction;
            public Button btnGoto;

            public uint npcId;

            protected override void Loaded() {
                this.npcName = this.transform.Find("Text1").GetComponent<Text>();
                this.mapName = this.transform.Find("Text2").GetComponent<Text>();
                this.coord = this.transform.Find("Text3").GetComponent<Text>();
                this.ownerFunction = this.transform.Find("Text4").GetComponent<Text>();

                this.btnGoto = this.transform.Find("Btn_01_Small").GetComponent<Button>();
                this.btnGoto.onClick.AddListener(this.OnBtnGotoClicked);
            }

            public void Refresh(int index, uint npcId) {
                this.npcId = npcId;

                var csvNpc = CSVNpc.Instance.GetConfData(npcId);
                if (csvNpc != null) {
                    this.Show();

                    TextHelper.SetText(this.npcName, LanguageHelper.GetNpcTextContent(csvNpc.name));
                    var csvNpcPathFind = CSVNpcPathFinding.Instance.GetConfData(npcId);
                    if (csvNpcPathFind != null) {
                        TextHelper.SetText(this.ownerFunction, csvNpcPathFind.FuncitonName);
                    }
                    else {
                        TextHelper.SetText(this.ownerFunction, "???");
                    }

                    var csvMap = CSVMapInfo.Instance.GetConfData(csvNpc.mapId);
                    if (csvMap != null) {
                        TextHelper.SetText(this.mapName, csvMap.name);
                    }
                    else {
                        TextHelper.SetText(this.mapName, "???");
                    }

                    Vector3 pos = default;
                    Quaternion eular = default;
                    Sys_Map.Instance.GetNpcPos(csvNpc.mapId, npcId, ref pos, ref eular);
                    TextHelper.SetText(this.coord, 4535, pos.x.ToString(), (-pos.z).ToString());
                }
                else {
                    this.Hide();
                }
            }

            private void OnBtnGotoClicked() {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(this.npcId);
                UIManager.ClearUntilMain();
            }
        }

        public class MapPopdown : UIPopdownItem {
            public virtual void Refresh(uint zoneId, int index) {
                base.Refresh(zoneId, index);

                if (zoneId != ALL_MAP_ID) {
                    var csv = CSVMapInfo.Instance.GetConfData(zoneId);
                    if (csv != null) {
                        TextHelper.SetText(this.text, csv.name);
                        this.optionName = this.text.text;
                    }
                }
                else {
                    string content = LanguageHelper.GetTextContent(4534);
                    TextHelper.SetText(this.text, content);
                    this.optionName = content;
                }
            }
        }

        public Button btnSearch;
        public InputField input;
        public InfinityGrid infinity;
        public GameObject noNpcs;
        public CP_PopdownList popdownList;

        public static readonly int ALL_MAP_ID = int.MaxValue - 1;
        public int currentMapId = ALL_MAP_ID;
        public bool filterUnopenMap = true;

        public bool useAllPopDown = true;

        // 考虑全部的特殊情况，id == 0 就是全部地图
        public List<uint> mapIds = new List<uint>();
        public UIElementCollector<MapPopdown> popdownVds = new UIElementCollector<MapPopdown>();

        public List<uint> thisMapNpcIds = new List<uint>();
        public List<uint> filteredNpcIds = new List<uint>();

        protected override void OnLoaded() {
            this.input = this.transform.Find("Animator/SearchInputField").GetComponent<InputField>();
            this.popdownList = this.transform.Find("Animator/PopupList").GetComponent<CP_PopdownList>();

            this.infinity = this.transform.Find("Animator/List/Scroll View").GetComponent<InfinityGrid>();
            this.infinity.onCreateCell += this.OnCreateCell;
            this.infinity.onCellChange += this.OnCellChange;
            this.noNpcs = this.transform.Find("Animator/List/Empty").gameObject;

            this.btnSearch = this.transform.Find("Animator/Btn_Find").GetComponent<Button>();
            this.btnSearch.onClick.AddListener(this.OnBtnSearchClicked);
        }

        private void OnBtnSearchClicked() {
            this.CalcFilterNpcs();
            this.RefreshNpcList();
        }

        public void CalcFilterNpcs() {
            string inputs = this.input.text.Trim();
            if (!string.IsNullOrEmpty(inputs)) {
                this.filteredNpcIds.Clear();
                // var inputChars = inputs.ToCharArray();
                for (int i = 0, length = this.thisMapNpcIds.Count; i < length; ++i) {
                    var npcId = this.thisMapNpcIds[i];
                    var csvNpc = CSVNpc.Instance.GetConfData(npcId);
                    if (csvNpc != null) {
                        string npcName = LanguageHelper.GetNpcTextContent(csvNpc.name);
                        if (npcName.Contains(inputs)) {
                            this.filteredNpcIds.Add(npcId);
                            // break;
                        }
                        // else {
                        //     for (int j = 0, lengthJ = inputChars.Length; j < lengthJ; ++j) {
                        //         if (npcName.IndexOf(inputChars[j]) != -1) {
                        //             this.filteredNpcIds.Add(npcId);
                        //             break;
                        //         }
                        //     }
                        // }
                    }
                }
            }
            else {
                this.filteredNpcIds.Clear();
                this.filteredNpcIds.AddRange(this.thisMapNpcIds);
            }
        }

        public void CalcThisMapNpcs() {
            this.thisMapNpcIds.Clear();
            if (this.currentMapId == ALL_MAP_ID) {
                for (int i = 0, length = this.mapIds.Count; i < length; ++i) {
                    var mapId = this.mapIds[i];
                    this.GetNpcsByMapId(mapId, this.thisMapNpcIds);
                }
            }
            else {
                this.GetNpcsByMapId((uint) this.currentMapId, this.thisMapNpcIds);
            }
        }

        // 获取当前地图中的npcs
        private void GetNpcsByMapId(uint mapId, List<uint> npcs) {
            var dataList = CSVNpcPathFinding.Instance.GetAll();
            for (int i = 0, length = dataList.Count; i < length; ++i)
            {
                var line = dataList[i];
                if (line.RegionID == mapId)
                {
                    npcs.Add(line.id);
                }
            }
        }

        private void OnCreateCell(InfinityGridCell cell) {
            SearchItem entry = new SearchItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index) {
            var entry = cell.mUserData as SearchItem;
            entry.Refresh(index, this.filteredNpcIds[index]);
        }

        protected override void OnOpen(object arg) {
            if (arg is Tuple<uint, uint> tp) {
                this.currentMapId = (int) tp.Item1;
                this.filterUnopenMap = (int) tp.Item1 != 0;
            }
            else {
                this.currentMapId = ALL_MAP_ID;
                this.filterUnopenMap = true;
            }
        }

        protected override void OnOpened() {
            this.GetMaps();

            // 构建下拉选项框
            this.popdownVds.BuildOrRefresh(this.popdownList.optionProto, this.popdownList.optionParent, this.mapIds, this.OnRefreshPopDown);
            int index = this.mapIds.IndexOf((uint) this.currentMapId);
            if (index == -1) {
                if (this.popdownVds.Count > 0) {
                    index = 0;
                    this.currentMapId = (int) this.mapIds[index];
                    this.popdownVds[index].SetSelected(true, true);
                }
            }
            else {
                this.popdownVds[index].SetSelected(true, true);
            }
        }

        private void GetMaps()
        {
            this.mapIds.Clear();
            var dataList = CSVNpcPathFinding.Instance.GetAll();
            for (int i = 0, length = dataList.Count; i < length; ++i)
            {
                var line = dataList[i];
                if (this.filterUnopenMap)
                {
                    bool isOpen = true; // 地图是否解锁
                    if (isOpen)
                    {
                        this.mapIds.Add(line.RegionID);
                    }
                }
                else
                {
                    this.mapIds.Add(line.RegionID);
                }
            }

            // 懒得引入hash去重了
            this.mapIds = this.mapIds.Distinct().ToList();
            if (useAllPopDown)
            {
                // 插入 全部 特殊地图
                this.mapIds.Insert(0, (uint)ALL_MAP_ID);
            }
        }

        public void RefreshNpcList() {
            var count = this.filteredNpcIds.Count;
            this.noNpcs.SetActive(count <= 0);
            this.infinity.CellCount = this.filteredNpcIds.Count;
            this.infinity.ForceRefreshActiveCell();
        }

        private void OnRefreshPopDown(MapPopdown vd, uint mapId, int index) {
            vd.SetUniqueId((int) mapId);
            vd.SetSelectedAction((zondId, force) => {
                this.popdownVds.ForEach((e) => { e.SetHighlight(false); });
                vd.SetHighlight(true);

                this.popdownList.Expand(false);
                this.popdownList.SetSelected(vd.optionName);
                
                // 选中
                int idx = this.mapIds.IndexOf((uint)zondId);
                this.popdownList.MoveTo(false, 1f * (idx + 1) / this.mapIds.Count);

                this.currentMapId = zondId;
                this.CalcThisMapNpcs();
                this.CalcFilterNpcs();

                this.RefreshNpcList();
            });
            vd.Refresh(mapId, index);
            vd.SetHighlight(false);
        }
    }
}