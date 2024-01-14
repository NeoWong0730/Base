using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Table;
using Packet;

namespace Logic
{
    public class ChangeSkillInfo
    {
        public uint raceId;
        public uint gridId;
    }

    public class TransformSkillLookCeil
    {
        public Transform transform;
        private Image icon;
        private Image tipIcon;
        private Text openLv;
        private Text name;
        private GameObject lockGo;
        private GameObject tipGo;
        private Button button;
        public Action<uint> action;

        private uint skillIdGroup;

        public void Init(Transform _transform)
        {
            transform = _transform;
            icon = transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>();
            tipIcon = transform.Find("PetSkillItem01/Image_Typebg/Image_Type").GetComponent<Image>();
            openLv = transform.Find("View_Lock_Transfiguration/Image_BG/Text").GetComponent<Text>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            lockGo = transform.Find("View_Lock_Transfiguration").gameObject;
            tipGo = transform.Find("PetSkillItem01/Image_Tip").gameObject;

            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        public void SetData(uint _id)
        {
            skillIdGroup = _id;
            lockGo.SetActive(false);
            icon.gameObject.SetActive(true);
            name.gameObject.SetActive(true);
            CSVRaceChangeGroup.Data group = CSVRaceChangeGroup.Instance.GetConfData(skillIdGroup);
            uint skillId = group.skill_id * 1000 + group.max_level;
            CSVPassiveSkillInfo.Data info = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
            CSVRaceSkillType.Data skillType = CSVRaceSkillType.Instance.GetConfData(skillId);
            if (info != null && skillType != null)
            {
                ImageHelper.SetIcon(icon, info.icon);
                ImageHelper.SetIcon(tipIcon, skillType.skill_type);
                TextHelper.SetText(name, info.name);
                tipGo.gameObject.SetActive(skillType.type == 1);
            }
        }

        public void AddAction(Action<uint> action)
        {
            this.action = action;
        }

        private void OnClicked()
        {
            action?.Invoke(skillIdGroup);
        }
    }

    public class UI_Transfiguration_SkillTips : UIBase
    {
        private Text basicTitle;
        private Text basicTips;
        private Text mutexTitle;
        private Text mutexTips;
        private Button closeBtn;
        private GameObject basicSkillRoot;
        private GameObject mutexSkillRoot;
        private GameObject skillRoot;
        private GameObject mutexView;

        private ChangeSkillInfo info;
        private List<uint> basicSkills = new List<uint>();
        private List<uint> mutexSkills = new List<uint>();
        private List<TransformSkillLookCeil> basicSkillItems = new List<TransformSkillLookCeil>();
        private List<TransformSkillLookCeil> mutexSkillItems = new List<TransformSkillLookCeil>();

        protected override void OnLoaded()
        {
            basicTitle = transform.Find("Animator/Scroll View/Viewport/Content/Item/Title/Image_Title/Text").GetComponent<Text>();
            mutexTitle = transform.Find("Animator/Scroll View/Viewport/Content/Item (1)/Title/Image_Title/Text").GetComponent<Text>();
            basicTips = transform.Find("Animator/Scroll View/Viewport/Content/Item/Title/Text_Tips").GetComponent<Text>();
            mutexTips = transform.Find("Animator/Scroll View/Viewport/Content/Item (1)/Title/Text_Tips").GetComponent<Text>();
            basicSkillRoot = transform.Find("Animator/Scroll View/Viewport/Content/Item/SkillGroup").gameObject;
            mutexSkillRoot = transform.Find("Animator/Scroll View/Viewport/Content/Item (1)/SkillGroup").gameObject;
            skillRoot = transform.Find("Animator/Scroll View/Viewport/Content").gameObject;
            mutexView = transform.Find("Animator/Scroll View/Viewport/Content/Item (1)").gameObject;
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                info = arg as ChangeSkillInfo;
            }
        }

        protected override void OnShow()
        {          
            basicSkills = Sys_Transfiguration.Instance.GetSkillIdsByGridId(info.gridId, info.raceId, out mutexSkills);
            SetBasicSkillData();
            SetMutexSkillData();
            ForceRebuildLayout(skillRoot);
        }

        protected override void OnHide()
        {
            DefaultItem();
        }

        private void SetBasicSkillData()
        {
            TextHelper.SetText(basicTitle, 2013302);
            TextHelper.SetText(basicTips, 2013303);
            basicSkillItems.Clear();
            FrameworkTool.CreateChildList(basicSkillRoot.transform,basicSkills.Count);
            for(int i=0; i< basicSkills.Count; ++i)
            {
                TransformSkillLookCeil ceil = new TransformSkillLookCeil();
                ceil.Init(basicSkillRoot.transform.GetChild(i));
                ceil.SetData(basicSkills[i]);
                ceil.AddAction(OnSelectItem);
                basicSkillItems.Add(ceil);
            }
        }

        private void SetMutexSkillData()
        {
            if (mutexSkills.Count == 0)
            {
                mutexView.SetActive(false);
                return;
            }
            else
            {
                mutexView.SetActive(true);
                TextHelper.SetText(mutexTitle, 2013304);
                TextHelper.SetText(mutexTips, 2013305);
                mutexSkillItems.Clear();
                FrameworkTool.CreateChildList(mutexSkillRoot.transform, mutexSkills.Count);
                for (int i = 0; i < mutexSkills.Count; ++i)
                {
                    TransformSkillLookCeil ceil = new TransformSkillLookCeil();
                    ceil.Init(mutexSkillRoot.transform.GetChild(i));
                    ceil.SetData(mutexSkills[i]);
                    ceil.AddAction(OnSelectItem);
                    mutexSkillItems.Add(ceil);
                }
            }
        }

        private void OnSelectItem(uint gridId)
        {
            CSVRaceChangeGroup.Data group = CSVRaceChangeGroup.Instance.GetConfData(gridId);
            uint skillId = group.skill_id * 1000 + group.max_level;
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
        }

        private void DefaultItem()
        {
            FrameworkTool.DestroyChildren(basicSkillRoot, basicSkillRoot.transform.GetChild(0).name);
            FrameworkTool.DestroyChildren(mutexSkillRoot, mutexSkillRoot.transform.GetChild(0).name);
            basicSkills.Clear();
            mutexSkills.Clear();
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }

        private void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Transfiguration_SkillTips);
        }
    }
}
