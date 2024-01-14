using System;
using System.Collections.Generic;
using Framework;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;
using static Packet.PetPkAttr.Types;

namespace Logic {
    public class SubmitPetLayout {
        public interface IListener {
            void OnBtnSubmited();
            void OnbasicSkillToggleValueChanged(bool arg);
            void OnremouldSkillToggleValueChanged(bool arg);

            void OnCreateSkillCell(InfinityGridCell cell);
            void OnCellSkillChange(InfinityGridCell cell, int index);

            void OnCreatePetCell(InfinityGridCell cell);
            void OnCellPetChange(InfinityGridCell cell, int index);
        }

        public void RegisterEvents(IListener listener) {
            this.btnSubmit.onClick.AddListener(listener.OnBtnSubmited);
            this.basicSkillToggle.onValueChanged.AddListener(listener.OnbasicSkillToggleValueChanged);
            this.remouldSkillToggle.onValueChanged.AddListener(listener.OnremouldSkillToggleValueChanged);
            this.infinityGridSkill.onCreateCell += listener.OnCreateSkillCell;
            this.infinityGridSkill.onCellChange += listener.OnCellSkillChange;

            this.infinityGridPet.onCreateCell += listener.OnCreatePetCell;
            this.infinityGridPet.onCellChange += listener.OnCellPetChange;
        }

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public Button btnSubmit;
        public Text filterTip;
        public TrListRegistry registry;

        public EmptyStatus emptyStatus;
        public Text emptyText;

        // 中间部分
        public Text petName;
        public Text petLv;
        public Text petGrade;
        public Text remouldTime;

        public GameObject mountTag;
        public Image rareIcon;
        public Image cardQuality;
        public Image cardLevel;

        public GameObject eleGo;

        // 技能
        public Toggle basicSkillToggle;
        public Toggle remouldSkillToggle;
        public InfinityGrid infinityGridSkill;

        public RawImage rawImage;

        // 宠物列表
        public InfinityGrid infinityGridPet;

        public AssetDependencies assetDependencies;

        public void Parse(GameObject root, IListener listener = null) {
            this.mRoot = root;
            this.mTrans = root.transform;

            this.assetDependencies = this.mTrans.GetComponent<AssetDependencies>();

            this.btnSubmit = this.mTrans.Find("Animator/Part3/Btn_Sumbit").GetComponent<Button>();
            this.filterTip = this.mTrans.Find("Animator/Part3/Text").GetComponent<Text>();

            this.emptyStatus = this.mTrans.Find("Animator").GetComponent<EmptyStatus>();
            this.emptyText = this.mTrans.Find("Animator/View_Npc022/View_Npc02_frame/Text_frame").GetComponent<Text>();
            this.registry = this.mTrans.Find("Animator/View_Left/Scroll_View_Gem/Viewport/Content").GetComponent<TrListRegistry>();

            this.petName = this.mTrans.Find("Animator/View_Middle/Detail/Image_Namebg/Text_Name").GetComponent<Text>();
            this.petLv = this.mTrans.Find("Animator/View_Middle/Detail/Level/Text_Level").GetComponent<Text>();
            // 分数
            this.petGrade = this.mTrans.Find("Animator/View_Middle/Detail/Image_Point/Text_Num").GetComponent<Text>();
            // 改造次数
            this.remouldTime = this.mTrans.Find("Animator/View_Middle/Detail/Image_Namebg/Text_Remake").GetComponent<Text>();

            // 骑
            this.mountTag = this.mTrans.Find("Animator/View_Middle/Detail/Image_Quality/Image_Mount").gameObject;
            // 稀有
            this.rareIcon = this.mTrans.Find("Animator/View_Middle/Detail/Image_Quality/Image_Rare").GetComponent<Image>();
            this.cardQuality = this.mTrans.Find("Animator/View_Middle/Detail/Image_Quality/Image_Card").GetComponent<Image>();
            this.cardLevel = this.mTrans.Find("Animator/View_Middle/Detail/Image_Quality/Image_Card/Text_CardLevel").GetComponent<Image>();
            this.eleGo = this.mTrans.Find("Animator/View_Middle/Detail/Image_Attr/EleProto").gameObject;

            this.basicSkillToggle = this.mTrans.Find("Animator/View_Right/Part2/Menu/ListItem").GetComponent<Toggle>();
            this.remouldSkillToggle = this.mTrans.Find("Animator/View_Right/Part2/Menu/ListItem (1)").GetComponent<Toggle>();

            this.infinityGridSkill = this.mTrans.Find("Animator/View_Right/Part2/Scroll_View_Skill").GetComponent<InfinityGrid>();
            this.infinityGridPet = this.mTrans.Find("Animator/View_Left/Scroll_View_Gem").GetComponent<InfinityGrid>();

            this.rawImage = this.mTrans.Find("Animator/View_Middle/Detail/Charapter").GetComponent<RawImage>();

            if (listener != null) {
                this.RegisterEvents(listener);
            }
        }
    }

    public class UI_SubmitPet : UIBase, SubmitPetLayout.IListener, UI_SubmitPet.PetTab.IListener {
        public class Element : UIComponent {
            public Image icon;
            public Text number;

            protected override void Loaded() {
                this.icon = this.transform.Find("Image_Attr").GetComponent<Image>();
                this.number = this.transform.Find("Image_Attr/Text").GetComponent<Text>();
            }

            public void RefreshContent(AttrPair attr) {
                ImageHelper.SetIcon(this.icon, CSVAttr.Instance.GetConfData(attr.AttrId).attr_icon);
                TextHelper.SetText(this.number, attr.AttrValue.ToString());
            }
        }

        public class PetTab : UIComponent {
            public interface IListener {
                void OnPetTabClicked(int index, TrList trList);
            }

            private int index;
            private ClientPet pet;

            public Text petNameText;
            public Text diffuctText;
            public Text dangweiText;
            public Text petLevelText;
            public Image petIcon;
            public Image petQuality;
            public Button btnPet;
            public TrList trList;

            private readonly IListener listener;

            protected override void Loaded() {
                this.petNameText = this.transform.Find("Text_name").GetComponent<Text>();
                this.diffuctText = this.transform.Find("Text_Difficult/Text").GetComponent<Text>();
                this.dangweiText = this.transform.Find("Text_Lv").GetComponent<Text>();
                this.petLevelText = this.transform.Find("Pet01/Image_Level/Text_Level/Text_Num").GetComponent<Text>();
                this.petIcon = this.transform.Find("Pet01/Image_Icon").GetComponent<Image>();
                this.petQuality = this.transform.Find("Pet01/Image1").GetComponent<Image>();
                this.btnPet = this.transform.Find("Image_BG").GetComponent<Button>();
                this.trList = this.transform.GetComponent<TrList>();

                this.btnPet.onClick.AddListener(this.OnClicked);
            }

            public PetTab(IListener listener) {
                this.listener = listener;
            }

            public void RefreshContent(int index, ClientPet pet) {
                this.index = index;
                this.pet = pet;

                TextHelper.SetText(this.petNameText, Sys_Pet.Instance.GetPetName(pet));
                TextHelper.SetText(this.petLevelText, 2009330, pet.petUnit.SimpleInfo.Level.ToString());

                TextHelper.SetText(this.diffuctText, pet.petData.card_lv.ToString());
                uint lowG = pet.GetPetMaxGradeCount() - pet.GetPetGradeCount();
                bool isMax = lowG == 0;
                if (isMax) {
                    TextHelper.SetText(this.dangweiText, LanguageHelper.GetTextContent(11713, pet.GetPetCurrentGradeCount().ToString(),
                        pet.GetPetBuildMaxGradeCount().ToString()));
                }
                else {
                    TextHelper.SetText(this.dangweiText, LanguageHelper.GetTextContent(11712, pet.GetPetCurrentGradeCount().ToString(),
                        pet.GetPetBuildMaxGradeCount().ToString(), lowG.ToString()));
                }

                ImageHelper.SetIcon(this.petIcon, pet.petData.icon_id);
                //ImageHelper.SetIcon(this.petQuality, Sys_Pet.Instance.SetPetQuality(pet.petData.card_type));
            }

            private void OnClicked() {
                this.listener?.OnPetTabClicked(this.index, trList);
            }
        }

        public class PetSkill : UIComponent {
            public Image itemicon;

            protected override void Loaded() {
                this.itemicon = this.transform.Find("Image_Skill").GetComponent<Image>();
            }

            public void RefreshContent(uint skillId) {
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null) {
                        ImageHelper.SetIcon(this.itemicon, skillInfo.icon);
                    }
                    else {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null) {
                        ImageHelper.SetIcon(this.itemicon, skillInfo.icon);
                    }
                    else {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
            }
        }

        private readonly SubmitPetLayout layout = new SubmitPetLayout();
        private readonly List<UI_Pet_Advance_Sub> uI_Pet_Advance_Subs = new List<UI_Pet_Advance_Sub>();
        private readonly COWVd<Element> elements = new COWVd<Element>();

        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;

        private uint taskId;
        private int taskGoalIndex;
        private uint conditionId = 0;
        private List<ClientPet> petsList = new List<ClientPet>(0);
        public int selectedIndex = 0;

        public CSVSubmitPetCondition.Data csvSubmitPet {
            get {
                return CSVSubmitPetCondition.Instance.GetConfData(this.conditionId);
            }
        }
        public ClientPet current {
            get {
                if (0 <= this.selectedIndex && this.selectedIndex < this.petsList.Count) {
                    return this.petsList[this.selectedIndex];
                }
                return null;
            }
        }

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject, this);

            this.uI_Pet_Advance_Subs.Clear();
            UI_Pet_Advance_Sub vit_Sub = new UI_Pet_Advance_Sub(5, EBaseAttr.Vit);

            vit_Sub.Init(this.transform.Find("Animator/View_Right/Part1/Grid/Vit/Text_Vit"));
            this.uI_Pet_Advance_Subs.Add(vit_Sub);

            UI_Pet_Advance_Sub pow_Sub = new UI_Pet_Advance_Sub(7, EBaseAttr.Snh);
            pow_Sub.Init(this.transform.Find("Animator/View_Right/Part1/Grid/Pow/Text_Pow"));
            this.uI_Pet_Advance_Subs.Add(pow_Sub);

            UI_Pet_Advance_Sub str_Sub = new UI_Pet_Advance_Sub(9, EBaseAttr.Inten);
            str_Sub.Init(this.transform.Find("Animator/View_Right/Part1/Grid/Str/Text_Str"));
            this.uI_Pet_Advance_Subs.Add(str_Sub);

            UI_Pet_Advance_Sub mp_Sub = new UI_Pet_Advance_Sub(11, EBaseAttr.Magic);
            mp_Sub.Init(this.transform.Find("Animator/View_Right/Part1/Grid/Ma/Text_Ma"));
            this.uI_Pet_Advance_Subs.Add(mp_Sub);

            UI_Pet_Advance_Sub spe_Sub = new UI_Pet_Advance_Sub(13, EBaseAttr.Speed);
            spe_Sub.Init(this.transform.Find("Animator/View_Right/Part1/Grid/Spe/Text_Spe"));
            this.uI_Pet_Advance_Subs.Add(spe_Sub);
        }

        protected override void OnDestroy() {
            //for (int i = 0, length = uI_Pet_Advance_Subs.Count; i < length; ++i) {
            //}
        }

        protected override void OnHide() {
            this._UnloadShowContent();
        }

        protected override void OnOpen(object arg) {
            if (arg != null) {
                var tp = arg as Tuple<uint, int, uint>;
                this.taskId = tp.Item1;
                this.taskGoalIndex = tp.Item2;
                this.conditionId = tp.Item3;

                this.petsList = Sys_Pet.Instance.petsList.FindAll(this.FilterPet);
                this.petsList.Sort(this.Sort);
            }
        }

        private int Sort(ClientPet left, ClientPet right) {
            return (int)(left.GetPetCurrentGradeCount() - (long)right.GetPetCurrentGradeCount());
        }

        private bool FilterPet(ClientPet pet) {
            bool rlt = true;

            long max = pet.GetPetMaxGradeCount();
            long cur = pet.GetPetCurrentGradeCount();
            if (rlt && this.csvSubmitPet.PetsID != 0) {
                rlt &= (pet.petData.id == this.csvSubmitPet.PetsID);
            }
            if (rlt && this.csvSubmitPet.PetsGear != 0) {
                rlt &= (cur == this.csvSubmitPet.PetsGear);
            }
            if (rlt && this.csvSubmitPet.PetsGearMore != 0) {
                rlt &= (cur >= this.csvSubmitPet.PetsGearMore);
            }
            if (rlt && this.csvSubmitPet.PetsArrest != 0) {
                rlt &= (pet.petData.card_lv == this.csvSubmitPet.PetsArrest);
            }
            if (rlt && this.csvSubmitPet.PetsArrestMore != 0) {
                rlt &= (pet.petData.card_lv >= this.csvSubmitPet.PetsArrestMore);
            }
            if (rlt && this.csvSubmitPet.PetsScore != 0) {
                rlt &= (pet.petUnit.SimpleInfo.Score >= this.csvSubmitPet.PetsScore);
            }
            if (rlt && this.csvSubmitPet.PetsLessenGear != 0) {
                rlt &= ((max - (long)pet.GetPetGradeCount()) == this.csvSubmitPet.PetsLessenGear);
            }
            if (rlt && this.csvSubmitPet.PetsLessenGearMore != 0) {
                rlt &= ((max - (long)pet.GetPetGradeCount()) <= this.csvSubmitPet.PetsLessenGearMore);
            }
            rlt &= (!pet.IsHasEquip());
            return rlt;
        }

        protected override void OnShow() {
            this.RefreshAll();
        }

        public void RefreshAll() {
            EEmptyStatus status = this.petsList.Count == 0 ? EEmptyStatus.Empty : EEmptyStatus.UnEmpty;
            if (this.layout.emptyStatus != null) {
                this.layout.emptyStatus.Refresh(status);
                if (status == EEmptyStatus.Empty) {
                    TextHelper.SetText(this.layout.emptyText, this.csvSubmitPet.NoSubmitPetstips);
                }
            }

            if (status == EEmptyStatus.UnEmpty) {
                this.layout.infinityGridPet.CellCount = this.petsList.Count;
                this.layout.infinityGridPet.ForceRefreshActiveCell();
                
                this.RefreshRight(this.selectedIndex);
            }
        }

        public void OnPetTabClicked(int index, TrList trList) {
            layout.registry.SwitchTo(trList, false);
            this.RefreshRight(index);
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_Task.Instance.eventEmitter.Handle(Sys_Task.EEvents.OnSubmitedPet, this.OnSubmitedPet, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, this.OnHeroTel, toRegister);
        }

        private void OnSubmitedPet() {
            this.CloseSelf();
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1260000002u));
        }
        private void OnHeroTel() {
            this.CloseSelf();
        }

        private List<AttrPair> attrList = new List<AttrPair>();
        public void RefreshRight(int index) {
            this.selectedIndex = index;

            if (this.current != null) {
                this.Load3D();
                this.RefreshAttr(this.current);

                TextHelper.SetText(this.layout.filterTip, this.csvSubmitPet.tips);

                TextHelper.SetText(this.layout.petName, Sys_Pet.Instance.GetPetName(this.current));
                TextHelper.SetText(this.layout.petLv, 2009330, this.current.petUnit.SimpleInfo.Level.ToString());
                TextHelper.SetText(this.layout.petGrade, this.current.petUnit.SimpleInfo.Score.ToString());

                var preCount = this.current.GetPeBuildCount();
                if (preCount > 0) {
                    this.layout.remouldTime.gameObject.SetActive(true);
                    TextHelper.SetText(this.layout.remouldTime, 11879, this.current.GetPeBuildCount().ToString());
                }
                else {
                    this.layout.remouldTime.gameObject.SetActive(false);
                }

                ImageHelper.GetPetCardLevel(this.layout.cardLevel, this.current.petData.card_lv);
                ImageHelper.SetIcon(this.layout.cardQuality, Sys_Pet.Instance.SetPetQuality(this.current.petData.card_type));
                ImageHelper.SetIcon(this.layout.rareIcon, Sys_Pet.Instance.GetQuality_ScoreImage(this.current));

                this.layout.mountTag.SetActive(this.current.petData.mount);

                // 元素
                this.attrList = this.current.GetPetEleAttrList();
                int elementCount = this.attrList.Count;
                this.elements.TryBuildOrRefresh(this.layout.eleGo, this.layout.eleGo.transform.parent, elementCount, this.OnElementRefresh);

                // 技能
                ESkillIndex tabIndex = ESkillIndex.basic;
                this.RefreshSkill(this.current, tabIndex);
            }
        }

        private void OnElementRefresh(Element element, int elementIndex) {
            element.RefreshContent(this.attrList[elementIndex]);
        }

        private void Load3D() {
            this._UnloadShowContent();
            this.LoadShowScene();
            this.LoadShowModel(this.current.petUnit.SimpleInfo.PetId);
        }

        private void LoadShowScene() {
            if (this.showSceneControl == null) {
                this.showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(this.layout.assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            this.showSceneControl.Parse(sceneModel);

            this.layout.rawImage.gameObject.SetActive(true);
            this.layout.rawImage.texture = this.showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (this.petDisplay == null) {
                this.petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                this.petDisplay.onLoaded = this.OnShowModelLoaded;
            }
        }

        private void LoadShowModel(uint petid) {
            string _modelPath = this.current.petData.model_show;

            this.petDisplay.eLayerMask = ELayerMask.ModelShow;
            this.petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            this.petDisplay.GetPart(EPetModelParts.Main).SetParent(this.showSceneControl.mModelPos, null);
            this.showSceneControl.mModelPos.transform.localPosition = new Vector3(this.showSceneControl.mModelPos.transform.localPosition.x + this.current.petData.translation,
                this.showSceneControl.mModelPos.transform.localPosition.y + this.current.petData.height, this.showSceneControl.mModelPos.transform.localPosition.z);
            this.showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(this.current.petData.angle1, this.current.petData.angle2, this.current.petData.angle3);
            this.showSceneControl.mModelPos.transform.localScale = new Vector3(this.current.petData.size, this.current.petData.size, this.current.petData.size);
        }

        private void OnShowModelLoaded(int obj) {
            if (obj == 0) {
                uint weaponId = Constants.UMARMEDID;
                this.petDisplay.mAnimation.UpdateHoldingAnimations(this.current.petData.action_id_show, weaponId);
            }
        }

        private void _UnloadShowContent() {
            this.layout.rawImage.gameObject.SetActive(false);
            this.layout.rawImage.texture = null;
            //this.petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            this.showSceneControl?.Dispose();
        }

        public void RefreshAttr(ClientPet client) {
            for (int i = 0; i < this.uI_Pet_Advance_Subs.Count; i++) {
                this.uI_Pet_Advance_Subs[i].RefreshItem(client);
            }
        }

        public enum ESkillIndex {
            basic = 0,
            remould = 1,
        }

        private readonly List<uint> skillIdList = new List<uint>();
        public void RefreshSkill(ClientPet client, ESkillIndex tabIndex) {
            this.layout.basicSkillToggle.isOn = tabIndex == ESkillIndex.basic;

            //this.layout.remouldSkillToggle.enabled = client.petUnit.BuildInfo.BuildSkills.Count != 0;
            //this.layout.remouldSkillToggle.isOn = (this.layout.remouldSkillToggle.enabled && tabIndex == ESkillIndex.remould);

            this.skillIdList.Clear();
            int infinityCount = 0;

            if (tabIndex == ESkillIndex.basic) {
                for (int i = 0; i < client.petUnit.BaseSkillInfo.UniqueSkills.Count; i++) {
                    this.skillIdList.Add(client.petUnit.BaseSkillInfo.UniqueSkills[i]);
                }

                for (int i = 0; i < client.petUnit.BaseSkillInfo.Skills.Count; i++) {
                    this.skillIdList.Add(client.petUnit.BaseSkillInfo.Skills[i]);
                }
                infinityCount = this.skillIdList.Count;
                this.layout.infinityGridSkill.CellCount = infinityCount;
                this.layout.infinityGridSkill.ForceRefreshActiveCell();
            }
            else if (tabIndex == ESkillIndex.remould) {
                for (int i = 0; i < client.petUnit.BuildInfo.BuildSkills.Count; i++) {
                    this.skillIdList.Add(client.petUnit.BuildInfo.BuildSkills[i]);
                }
                infinityCount = client.GetPeBuildtSkillCount();
                this.layout.infinityGridSkill.CellCount = infinityCount;
                this.layout.infinityGridSkill.ForceRefreshActiveCell();
            }
        }

        public void OnCellSkillChange(InfinityGridCell cell, int index) {
            PetSkill entry = cell.mUserData as PetSkill;
            if (index < this.skillIdList.Count) {
                entry.Show();
                uint skillId = this.skillIdList[index];
                entry.RefreshContent(skillId);
            }
            else {
                entry.Hide();
            }
        }

        public void OnCreateSkillCell(InfinityGridCell cell) {
            PetSkill entry = new PetSkill();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        public void OnCellPetChange(InfinityGridCell cell, int index) {
            PetTab entry = cell.mUserData as PetTab;
            if (0 <= index && index < this.petsList.Count) {
                entry.Show();
                var pet = this.petsList[index];
                entry?.trList.ShowHideBySetActive(this.selectedIndex == index);
                entry.RefreshContent(index, pet);
            }
            else {
                entry.Hide();
            }
        }

        public void OnCreatePetCell(InfinityGridCell cell) {
            PetTab entry = new PetTab(this);
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        public void OnremouldSkillToggleValueChanged(bool isOn) {
            if (isOn) {
                this.RefreshSkill(this.current, ESkillIndex.remould);
            }
        }

        public void OnbasicSkillToggleValueChanged(bool isOn) {
            if (isOn) {
                this.RefreshSkill(this.current, ESkillIndex.basic);
            }
        }

        public void OnBtnSubmited() {
            if (this.current != null) {
                if (!Sys_Pet.Instance.IsPetBeEffectWithSecureLock(this.current.petUnit, true)) {
                    if (Sys_Pet.Instance.GetPetIsHightScore(this.current.petUnit)) {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(1260000001).words;
                        PromptBoxParameter.Instance.SetConfirm(true, _Submit);
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    }
                    else {
                        _Submit();
                    }
                }
            }

            void _Submit() {
                Sys_Task.Instance.ReqSubmitPet(this.taskId, this.taskGoalIndex, this.current.GetPetUid());
            }
        }
    }
}
