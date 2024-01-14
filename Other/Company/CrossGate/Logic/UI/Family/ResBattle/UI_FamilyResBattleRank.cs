using System.Collections.Generic;
using Logic;
using Logic.Core;
using Packet;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战排行
public class UI_FamilyResBattleRank : UIBase, UI_FamilyResBattleRank.Layout.IListener {
    // UI_CampChallenge.Tab
    public class Tab : UISelectableElement {
        public CP_Toggle toggle;
        public Text name1;
        public Text name1_1;

        protected override void Loaded() {
            this.name1 = this.transform.Find("Object/Text").GetComponent<Text>();
            this.name1_1 = this.transform.Find("Object_Selected/Text").GetComponent<Text>();

            this.toggle = this.transform.GetComponent<CP_Toggle>();
            this.toggle.onValueChanged.AddListener(this.Switch);
        }

        public void Refresh(Sys_FamilyResBattle.ERoleSortType type) {
            uint lanId = 0;
            if (type == Sys_FamilyResBattle.ERoleSortType.All) {
                lanId = 3230000027;
            }
            else if (type == Sys_FamilyResBattle.ERoleSortType.Red) {
                lanId = 3230000028;
            }
            else if (type == Sys_FamilyResBattle.ERoleSortType.Blue) {
                lanId = 3230000029;
            }

            TextHelper.SetText(this.name1, lanId);
            this.name1_1.text = this.name1.text;
        }

        public void Switch(bool arg) {
            if (arg) {
                this.onSelected?.Invoke(this.id, true);
            }
        }
        public override void SetSelected(bool toSelected, bool force) {
            this.toggle.SetSelected(toSelected, true);
        }
    }

    public class Line : UIComponent {
        public Image icon;

        public Text rank;
        public Text name;
        public Text familyName;
        public Text commited;
        public Text score;

        protected override void Loaded() {
            this.icon = this.transform.Find("Rank/Image_Icon").GetComponent<Image>();
            this.rank = this.transform.Find("Rank/Text_Rank").GetComponent<Text>();
            this.name = this.transform.Find("Text_Name").GetComponent<Text>();
            this.familyName = this.transform.Find("Text_Family").GetComponent<Text>();
            this.commited = this.transform.Find("Text_Resource").GetComponent<Text>();
            this.score = this.transform.Find("Text_Score").GetComponent<Text>();
        }

        public void Refresh(BattleRoleMapData info, int index) {
            if (index < 3) {
                uint iconId = 993901;
                if (index == 0) {
                    iconId = 993901;
                }
                else if (index == 1) {
                    iconId = 993902;
                }
                else if (index == 2) {
                    iconId = 993903;
                }

                this.icon.gameObject.SetActive(true);
                this.rank.gameObject.SetActive(false);

                ImageHelper.SetIcon(this.icon, iconId);
            }
            else {
                this.icon.gameObject.SetActive(false);
                this.rank.gameObject.SetActive(true);

                TextHelper.SetText(this.rank, (index + 1).ToString());
            }

            TextHelper.SetText(this.name, info.RoleName.ToStringUtf8());
            BattleGuildMapData guild = Sys_FamilyResBattle.Instance.GetFamilyByRoleId(info.RoleId);
            if (guild != null) {
                TextHelper.SetText(this.familyName, guild.GuildName.ToStringUtf8());
            }
            else {
                TextHelper.SetText(this.familyName, "Null");
            }

            TextHelper.SetText(this.commited, info.Resource.ToString());
            TextHelper.SetText(this.score, info.Score.ToString());
        }
    }

    public class Layout : LayoutBase {
        public InfinityGrid infinity;
        public GameObject tabProto;
        public Transform tabProtoParent;

        public GameObject rankProto;
        public GameObject emptyNode;

        public GameObject myRankNode;
        public Image myIcon;
        public Text myRank;
        public Text myName;
        public Text myFamilyName;
        public Text myCommited;
        public Text myScore;

        public void Parse(GameObject root) {
            this.Set(root);

            this.emptyNode = this.transform.Find("Animator/View_Personal/Empty").gameObject;
            this.infinity = this.transform.Find("Animator/View_Personal/Scroll_Rank/Viewport/Content").GetComponent<InfinityGrid>();
            this.tabProto = this.transform.Find("Animator/ScrollView_Menu/List/Proto").gameObject;
            this.tabProtoParent = this.tabProto.transform.parent;

            this.myRankNode = this.transform.Find("Animator/View_Personal/MyRank").gameObject;
            this.myIcon = this.transform.Find("Animator/View_Personal/MyRank/Image_Icon").GetComponent<Image>();
            this.myRank = this.transform.Find("Animator/View_Personal/MyRank/Text_Rank").GetComponent<Text>();
            this.myName = this.transform.Find("Animator/View_Personal/MyRank/Text_Name").GetComponent<Text>();
            this.myFamilyName = this.transform.Find("Animator/View_Personal/MyRank/Text_Family").GetComponent<Text>();
            this.myCommited = this.transform.Find("Animator/View_Personal/MyRank/Text_Score").GetComponent<Text>();
            this.myScore = this.transform.Find("Animator/View_Personal/MyRank/Text_Familyscore").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener) {
            this.infinity.onCreateCell += listener.OnCreateCell;
            this.infinity.onCellChange += listener.OnCellChange;
        }

        public interface IListener {
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public Layout layout = new Layout();

    public List<uint> tabIds = new List<uint>(3) {
        (uint)Sys_FamilyResBattle.ERoleSortType.All,
        (uint)Sys_FamilyResBattle.ERoleSortType.Red,
        (uint)Sys_FamilyResBattle.ERoleSortType.Blue,
    };
    public int currentTabId = 0;
    public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }
    protected override void OnDestroy() {
        this.tabVds.Clear();
    }

    protected override void OnOpened() {
        this.tabVds.BuildOrRefresh<uint>(this.layout.tabProto, this.layout.tabProtoParent, this.tabIds, (vd, id, indexOfVdList) => {
            vd.SetUniqueId(indexOfVdList);
            vd.SetSelectedAction((innerId, force) => {
                this.currentTabId = innerId;

                this.RefreshRank(this.currentTabId);
                this.RefreshMy();
            });
            vd.Refresh((Sys_FamilyResBattle.ERoleSortType)indexOfVdList);
        });

        // 默认选中Tab
        if (this.tabVds.CorrectId(ref this.currentTabId, this.tabIds)) {
            if (this.tabVds.TryGetVdById(this.currentTabId, out var vd)) {
                vd.SetSelected(true, true);
            }
        }
        else {
            Debug.LogError("Can't run here!");
        }
    }

    public void RefreshMy() {
        if ((Sys_FamilyResBattle.ERoleSortType)this.currentTabId == Sys_FamilyResBattle.ERoleSortType.Blue) {
            this.layout.myRankNode.SetActive(false);
        }
        else {
            int index = -1;
            for (int i = 0, length = this.roleLs.Count; i < length; ++i) {
                if (this.roleLs[i].RoleId == Sys_Role.Instance.RoleId) {
                    index = i;
                    break;
                }
            }

            if (index == -1) {
                this.layout.myRankNode.SetActive(false);
                return;
            }
            else {
                this.layout.myRankNode.SetActive(true);
            }

            BattleRoleMapData info = this.roleLs[index];
            if (index < 3) {
                uint iconId = 993901;
                if (index == 0) {
                    iconId = 993901;
                }
                else if (index == 1) {
                    iconId = 993902;
                }
                else if (index == 2) {
                    iconId = 993903;
                }

                this.layout.myIcon.gameObject.SetActive(true);
                this.layout.myRank.gameObject.SetActive(false);

                ImageHelper.SetIcon(this.layout.myIcon, iconId);
            }
            else {
                this.layout.myIcon.gameObject.SetActive(false);
                this.layout.myRank.gameObject.SetActive(true);

                TextHelper.SetText(this.layout.myRank, (index + 1).ToString());
            }

            TextHelper.SetText(this.layout.myName, Sys_Role.Instance.sRoleName);
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            TextHelper.SetText(this.layout.myFamilyName, Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName.ToStringUtf8());
            TextHelper.SetText(this.layout.myCommited, info.Resource.ToString());
            TextHelper.SetText(this.layout.myScore, info.Score.ToString());
        }
    }

    public List<BattleRoleMapData> roleLs = new List<BattleRoleMapData>();
    public void RefreshRank(int tabIndex) {
        // todo 有待优化
        this.roleLs = Sys_FamilyResBattle.Instance.GetRoles((Sys_FamilyResBattle.ERoleSortType)(tabIndex));
        if (roleLs.Count <= 0) {
            layout.emptyNode.SetActive(true);

            this.layout.infinity.CellCount = 0;
            this.layout.infinity.ForceRefreshActiveCell();
        }
        else {
            layout.emptyNode.SetActive(false);

            this.roleLs.Sort((l, r) => {
                int rlt = (int)(r.Score - (long)l.Score);
                if (rlt == 0) {
                    rlt =  (int)(l.RoleId - r.RoleId);
                }
                return rlt;
            });
            this.layout.infinity.CellCount = this.roleLs.Count;
            this.layout.infinity.ForceRefreshActiveCell();
        }
    }

    public void OnCreateCell(InfinityGridCell cell) {
        Line entry = new Line();
        entry.Init(cell.mRootTransform);
        cell.BindUserData(entry);
    }
    public void OnCellChange(InfinityGridCell cell, int index) {
        var entry = cell.mUserData as Line;
        entry.Refresh(this.roleLs[index], index);
    }
}