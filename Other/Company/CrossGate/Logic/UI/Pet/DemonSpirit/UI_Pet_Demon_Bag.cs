using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Pet_Demon_Bag_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public Transform typeTransform;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Content/Scroll_Rank").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();
            typeTransform = transform.Find("Animator/ScrollView_Menu");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_Demon_Bag : UIBase, UI_Pet_Demon_Bag_Layout.IListener, UI_Pet_Demon_Bag_Type.IListener
    {
        private UI_Pet_Demon_Bag_Layout layout = new UI_Pet_Demon_Bag_Layout();
        private UI_Pet_Demon_Bag_Type types;
        private int selectType = 1;
        private List<PetSoulBeadInfo> sphereTemp = new List<PetSoulBeadInfo>();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            types = new UI_Pet_Demon_Bag_Type();
            types.Init(layout.typeTransform);
            types.Register(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDemonSpiritSphere, OnActiveDemonSpiritSphere, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnRefreshDemonSpiritSkill, OnRefreshDemonSpiritSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnDemonSpiritUpgrade, OnRefreshDemonSpiritSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill, OnRefreshDemonSpiritSkill, toRegister);

        }

        private void OnActiveDemonSpiritSphere()
        {
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, true);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }

        private void OnRefreshDemonSpiritSkill()
        {
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, true);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }

        private void OnRefreshDemonSpiritSkill(uint isCrit)
        {
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, true);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }

        protected override void OnOpen(object arg = null)
        {
        }

        protected override void OnShow()
        {
            types.OnSelect(0);
        }

        protected override void OnHide()
        {
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_Pet_DemonSpiritSphere entry = new UI_Pet_DemonSpiritSphere();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= sphereTemp.Count)
                return;
            UI_Pet_DemonSpiritSphere entry = cell.mUserData as UI_Pet_DemonSpiritSphere;
            entry.SetView(sphereTemp[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Demon_Bag);
        }

        public void OnType(int index)
        {
            selectType = index + 1;
            sphereTemp = Sys_Pet.Instance.GetMySpheres((uint)selectType, true);
            layout.SetInfinityGridCell(sphereTemp.Count);
        }
    }

    public class UI_Pet_Demon_Bag_Type
    {
        public class TypeToggle
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private int m_index;
            private Action<int> m_Action;
            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnToggle);
            }

            private void OnToggle(bool isOn)
            {
                if (isOn)
                {
                    m_Action?.Invoke(m_index);
                }
            }

            public void Register(Action<int> action, int index)
            {
                m_Action = action;
                m_index = index;
            }

            public void OnSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<TypeToggle> m_listToggles = new List<TypeToggle>(4);

        private IListener m_Listener;

        public void Init(Transform trans)
        {
            transform = trans;

            Transform parent = transform.Find("List");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                TypeToggle toggle = new TypeToggle();
                toggle.Init(parent.GetChild(i));
                toggle.Register(OnToggleType, i);

                m_listToggles.Add(toggle);
            }
        }

        private void OnToggleType(int index)
        {
            m_Listener?.OnType(index);
        }

        public void Register(IListener listener)
        {
            m_Listener = listener;
        }

        public void OnSelect(int index)
        {
            m_listToggles[index].OnSelect(true);
        }

        public interface IListener
        {
            void OnType(int index);
        }
    }

    public class UI_Pet_DemonSpiritSphere
    {
        private PropItem propItem;
        protected Button backBtn;
        protected Button priviewBtn;
        private Button activeBtn;
        private GameObject lockGo;
        private GameObject UnLockGo;

        private GameObject equipGo;
        private Image sphereImage;
        private Text nameText;
        private Text LevelText;
        private Text scoreText;
        private Text percentText;
        private Slider percentSlider;
        //默认技能
        private GameObject skill1Go;
        //解锁技能
        private GameObject skill2Go;
        private Button unlockBtn;
        private Button sphereBtn;
        private Button skill1Btn;
        private Button skill2Btn;
        protected PetSoulBeadInfo sphereTemp;
        protected CSVSoulBead.Data sphereData;
        public virtual void Init(Transform transform)
        {
            lockGo = transform.Find("Lock").gameObject;
            UnLockGo = transform.Find("Unlock").gameObject;

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Lock/PropItem").gameObject);
            activeBtn = transform.Find("Lock/Btn_Unlock").GetComponent<Button>();
            activeBtn.onClick.AddListener(OnActiveBtnClicked);
            equipGo = transform.Find("Image_Full")?.gameObject;
            sphereImage = transform.Find("Image").GetComponent<Image>();
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            scoreText = transform.Find("Text_Name/Text_Score").GetComponent<Text>();
            LevelText = transform.Find("Text_Name/Text").GetComponent<Text>();
            percentText = transform.Find("Unlock/EXP/Text_Percent").GetComponent<Text>();
            percentSlider = transform.Find("Unlock/EXP/Slider_Lv").GetComponent<Slider>();
            skill1Go = transform.Find("Unlock/Skill1").gameObject;
            skill1Btn = transform.Find("Unlock/Skill1/Button").GetComponent<Button>();
            skill1Btn.onClick.AddListener(Skill1BtnClicked);
            skill2Go = transform.Find("Unlock/Skill2").gameObject;
            skill2Btn = transform.Find("Unlock/Skill2/Button").GetComponent<Button>();
            skill2Btn.onClick.AddListener(Skill2BtnClicked);
            BingBackBtn(transform);
            backBtn.onClick.AddListener(OnBackBtnClicked);
        }

        protected virtual void BingBackBtn(Transform transform)
        {
            backBtn = transform.Find("background").GetComponent<Button>();
            priviewBtn = transform.Find("Button_Prview").GetComponent<Button>();
            priviewBtn.onClick.AddListener(OnPriviewBtnClicked);
        }

        private void OnPriviewBtnClicked()
        {
            if (null != sphereTemp)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Demon_SkillPreview, false, new Tuple<uint, uint>(sphereTemp.Type, 0));
            }
        }

        protected virtual void OnBackBtnClicked()
        {
            if(null != sphereTemp && sphereTemp.Level > 0)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonUpgrade, false, sphereTemp);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002054));
            }
        }

        public virtual void SetView(PetSoulBeadInfo sphereTemp)
        {
            if (null != sphereTemp)
            {
                this.sphereTemp = sphereTemp;
                uint level = sphereTemp.Level;
                bool isActive = level > 0;
                lockGo.SetActive(!isActive);
                var sphereId = sphereTemp.Type * 10000 + 1;
                if (isActive)
                {
                    sphereId = sphereTemp.Type * 10000 + sphereTemp.Level;
                }

                sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                if (null != sphereData)
                {
                    ImageHelper.SetIcon(sphereImage, sphereData.icon);
                    TextHelper.SetText(LevelText, 680003020, level.ToString());
                    TextHelper.SetText(nameText, 680003010u + sphereData.type - 1);

                    if (isActive)
                    {
                        uint skill1 = sphereTemp.SkillIds[0];
                        uint skill2 = sphereTemp.SkillIds[1];
                        SetSkill(skill1Go, skill1);
                        SetSkill(skill2Go, skill2);
                        UnLockGo.SetActive(true);
                        percentText.gameObject.SetActive(sphereData.exp != 0);
                        TextHelper.SetText(percentText, 680003039, sphereTemp.Exp.ToString(), sphereData.exp.ToString());
                        TextHelper.SetText(scoreText, 680003021, sphereData.score.ToString());
                        scoreText.gameObject.SetActive(true);
                        if (sphereData.exp == 0)
                        {
                            percentSlider.value = 1;
                        }
                        else
                        {
                            percentSlider.value = (sphereTemp.Exp + 0f) / sphereData.exp;
                        }
                        equipGo?.SetActive(sphereTemp.PetUid != 0);
                    }
                    else
                    {
                        var unLockData = CSVSoulBeadUnlock.Instance.GetConfData(sphereTemp.Type * 10000 + sphereTemp.Index);
                        if (null != unLockData && null != unLockData.item_cost && unLockData.item_cost.Count >= 2)
                        {
                            var itemId = unLockData.item_cost[0];
                            var itemNum = unLockData.item_cost[1];
                            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(itemId, itemNum, true, false, false, false, false, true, true, true);
                            propItem.SetData(itemN, EUIID.UI_Pet_DemonReform);
                            propItem.Layout.imgIcon.enabled = true;
                        }
                        TextHelper.SetText(LevelText, 680003020, sphereData.level.ToString());
                        scoreText.gameObject.SetActive(false);
                        equipGo?.SetActive(false);
                        UnLockGo.SetActive(false);
                    }
                }
            }

        }

        private void SetSkill(GameObject skillGo, uint skillId)
        {
            bool hasSkill = skillId > 0;
            if (hasSkill)
            {
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("Text_Level").GetComponent<Text>(), 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("Text_Level").GetComponent<Text>(), 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
                var unlockGo = skillGo.transform.Find("Lock").gameObject;
                var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                levelGroundImageGo?.SetActive(true);
                unlockGo?.SetActive(false);
            }
            else
            {
                var lockData = CSVSoulBead.Instance.GetSkillLockLevelData(sphereData.type);
                if (null != lockData)
                {
                    TextHelper.SetText(skillGo.transform.Find("Text_Level").GetComponent<Text>(), 680003023, lockData.level.ToString());//解锁等级
                    var unlockGo = skillGo.transform.Find("Lock").gameObject;
                    var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                    levelGroundImageGo?.SetActive(false);
                    unlockGo?.SetActive(true);
                }
            }
            skillGo.transform.Find("PetSkillItem01").gameObject.SetActive(hasSkill);
        }

        private void Skill1BtnClicked()
        {
            uint skill = sphereTemp.SkillIds[0];
            if (skill > 0)
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 1));

        }

        private void Skill2BtnClicked()
        {
            uint skill = sphereTemp.SkillIds[1];
            if (skill > 0)
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 2));
        }

        private void OnActiveBtnClicked()
        {
            if (null != sphereTemp)
            {
                var unLockData = CSVSoulBeadUnlock.Instance.GetConfData(sphereTemp.Type * 10000 + sphereTemp.Index);
                if (null != unLockData && null != unLockData.item_cost && unLockData.item_cost.Count >= 2)
                {
                    var itemId = unLockData.item_cost[0];
                    var itemNum = unLockData.item_cost[1];
                    ItemIdCount itemIdCount = new ItemIdCount(itemId, itemNum);
                    if (itemIdCount.Enough)
                    {
                        Sys_Pet.Instance.PetSoulBeadOperateReq((uint)PetSoulBeadOpType.SoulBeadOpTypeActive, sphereTemp.Index, sphereTemp.Type, 0);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680002012));
                    }
                }

            }

        }
    }
}