using System;
using System.Collections.Generic;
using Framework;
using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战某队伍成员
public class UI_FamilyResBattleTeamMember : UIBase, UI_FamilyResBattleTeamMember.Layout.IListener {
    public class Member : UIComponent {
        public Image careerIcon;
        public Text careerText;
        public Text roleName;
        public Text roleLevel;
        public Image roleTitleIcon;
        public Text roleTitleText;
        public Text score;

        public Transform nodeEmptyPos;

        public VirtualGameObject VGO { get; set; } = null;

        protected override void Loaded() {
            careerIcon = transform.Find("real/Image_Prop").GetComponent<Image>();
            careerText = transform.Find("real/Image_Prop/Text_Profession").GetComponent<Text>();
            roleName = transform.Find("real/Text_Name").GetComponent<Text>();
            roleLevel = transform.Find("real/Text_Level").GetComponent<Text>();
            
            nodeEmptyPos = transform.Find("empty");

            roleTitleIcon = transform.Find("real/Title/Image1").GetComponent<Image>();
            roleTitleText = transform.Find("real/Title/Image1/Text").GetComponent<Text>();

            score = transform.Find("real/Score_bg/Text_Score/Text_Value").GetComponent<Text>();
        }

        public bool hasLoadedModel = false;
        public HeroLoader m_heroLoader;

        public void TryLoadModel() {
            if (hasLoadedModel) {
                return;
            }

            uint heroID = mem.HeroId;
            uint weaponId = mem.WeaponItemID;
            uint occupation = mem.Career;

            m_heroLoader = HeroLoader.Create(true);

            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(occupation);
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(weaponId);

            var dress = Sys_Fashion.Instance.GetDressData(mem.FashionList, heroID);
            m_heroLoader.LoadHero(heroID, weaponId, ELayerMask.ModelShow, dress, o => {
                m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(VGO, null);
                GameObject go = m_heroLoader.heroDisplay?.GetPart(EHeroModelParts.Main).gameObject;
                go.SetActive(false);

                uint dressId = 0;
                for (int i = 0, length = mem.FashionList.Count; i < length; ++i) {
                    EHeroModelParts part;
                    bool bresult = Sys_Fashion.Instance.parts.TryGetValue(mem.FashionList[i].FashionId, out part);
                    if (bresult && part == EHeroModelParts.Main)
                    {
                        dressId = mem.FashionList[i].FashionId;
                        break;
                    }
                }
                uint id = (uint)(dressId * 10000 + heroID);
                CSVFashionModel.Data csvFashion = CSVFashionModel.Instance.GetConfData(id);
                var activeCharId = csvFashion.action_show_id;
                m_heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(activeCharId, weaponId, Constants.IdleAndRunAnimationClipHashSet, EStateType.Idle, go);
            });

            hasLoadedModel = true;
        }
        
        public void TryUnloadModel() {
            m_heroLoader?.Dispose();
            m_heroLoader = null;

            hasLoadedModel = false;
        }

        public BattleCoreRoleMapData mem;

        public void Refresh(BattleCoreRoleMapData mem, ulong guildId) {
            if (mem.RoleId == 0) {
                return;
            }
            
            gameObject.SetActive(true);
            this.mem = mem;

            TextHelper.SetText(roleLevel, 1000002, mem.Level.ToString());
            TextHelper.SetText(score, mem.TotalScore.ToString());
            if (guildId == Sys_FamilyResBattle.Instance.redFamilyId) {
                var r = Sys_FamilyResBattle.Instance.redRoles[mem.RoleId];
                TextHelper.SetText(roleName, r.RoleName.ToStringUtf8());
            }
            else {
                var r = Sys_FamilyResBattle.Instance.blueRoles[mem.RoleId];
                TextHelper.SetText(roleName, r.RoleName.ToStringUtf8());
            }

            CSVCareer.Data csvCareer = CSVCareer.Instance.GetConfData(mem.Career);
            ImageHelper.SetIcon(careerIcon, csvCareer.icon);
            TextHelper.SetText(careerText, csvCareer.name);

            var csvTitle = CSVTitle.Instance.GetConfData(mem.Title);
            string titlestr = string.Empty;

            if (csvTitle != null) {
                if (csvTitle.titleShowLan != 0) {
                    titlestr = LanguageHelper.GetTextContent(csvTitle.titleShowLan);
                }

                if (csvTitle.titleGetType == 12) {
                    titlestr = string.Empty;
                    csvTitle = null;
                }
            }

            TextHelper.SetText(roleTitleText, titlestr);
            var colors = csvTitle?.titleShow;
            if (colors != null && colors.Count >= 3) {
                TextHelper.SetTextGradient(roleTitleText, colors[0], colors[1]);
                TextHelper.SetTextOutLine(roleTitleText, colors[2]);
            }

            uint iconID = csvTitle == null ? 0 : csvTitle.titleShowIcon;
            if (iconID != 0) {
                roleTitleIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(roleTitleIcon, iconID);
            }
            else {
                roleTitleIcon.gameObject.SetActive(false);
            }

            TryLoadModel();
        }
    }

    public class Layout : LayoutBase {
        public AssetDependencies assetDependencies;
        public Text totalScore;
        public Transform posNode;

        public void Parse(GameObject root) {
            this.Set(root);

            assetDependencies = transform.Find("Animator/View_Team").GetComponent<AssetDependencies>();
            totalScore = transform.Find("Animator/Score_bg/Text_Score/Text_Value").GetComponent<Text>();
            posNode = transform.Find("Animator/View_Team/Scroll_View/Viewport");
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
            // void OnCreateCell(InfinityGridCell cell);
            // void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public Layout layout = new Layout();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    private ulong guildId;
    private BattleCoreTeamMapData team;
    private List<Member> showPosList = new List<Member>(5);

    public int memberCount {
        get { return team.Roles.Count; }
    }

    protected override void OnOpen(object arg) {
        if (arg is UI_FamilyResBattleTop.ArgTransfer tp) {
            guildId = tp.roleid;
            team = tp.team;
        }
    }

    protected override void OnOpened() {
        var score = Sys_FamilyResBattle.Instance.TotalScore(guildId, (int) team.Rank);
        TextHelper.SetText(layout.totalScore, score.ToString());
    }

    private ShowSceneControl showSceneControl = null;

    protected override void OnShow() {
        GameObject scene = GameObject.Instantiate<GameObject>(layout.assetDependencies.mCustomDependencies[0] as GameObject);
        scene.transform.SetParent(GameCenter.sceneShowRoot.transform);

        showSceneControl = new ShowSceneControl();
        showSceneControl.Parse(scene);

        showPosList.Clear();
        for (int i = 0; i < 5; ++i) {
            var t = layout.posNode.Find("Item" + i.ToString());
            t.gameObject.SetActive(false);
        }
        for (int i = 0; i < memberCount; i++) {
            Transform objTrans = showSceneControl.mRoot.transform.Find("Pos_" + i.ToString());
            VirtualGameObject vobj = new VirtualGameObject();
            vobj.SetGameObject(objTrans.gameObject, true);
            var one = new Member() {
                VGO = vobj
            };
            one.Init(layout.posNode.Find("Item" + i.ToString()));
            showPosList.Add(one);
        }

        int min = Math.Min( team.Roles.Count, showPosList.Count);
        for (int i = 0, count = min; i < count; i++) {
            var mem = showPosList[i];
            Transform objTrans = mem.VGO.transform;
            Vector3 posshow = showSceneControl.mCamera.WorldToViewportPoint(objTrans.position);
            var itemposition = mem.nodeEmptyPos.position;
            Vector3 uiitem = UIManager.mUICamera.WorldToViewportPoint(itemposition);

            posshow.x = uiitem.x;
            Vector3 position = showSceneControl.mCamera.ViewportToWorldPoint(posshow);
            objTrans.position = position;

            mem.Refresh(team.Roles[i], guildId);
        }
    }

    protected override void OnHide() {
        if (showSceneControl == null) {
            return;
        }

        showSceneControl.Dispose();
        showSceneControl = null;

        for (int i = 0, length = showPosList.Count; i < length; ++i) {
            showPosList[i].TryUnloadModel();
        }

        showPosList.Clear();
    }
}