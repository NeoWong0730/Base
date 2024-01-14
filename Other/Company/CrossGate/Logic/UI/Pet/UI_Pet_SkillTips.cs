using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    [Flags]
    public enum EPetRemakeSkillState
    {
        Common = 1,
        Senior = 2,
        All = 3,
    }

    public class UI_Pet_SkillTips_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private Toggle toggle1;
        private Toggle toggle2;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Content/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            toggle1 = transform.Find("Animator/View_Up/Toggle").GetComponent<Toggle>();
            toggle2 = transform.Find("Animator/View_Up/Toggle (1)").GetComponent<Toggle>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            toggle1.onValueChanged.AddListener(listener.OnSeniorValueChange);
            toggle2.onValueChanged.AddListener(listener.OnCommonValueChange);
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
            void OnCommonValueChange(bool isOn);
            void OnSeniorValueChange(bool isOn);
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_SkillTips : UIBase, UI_Pet_SkillTips_Layout.IListener
    {
        private UI_Pet_SkillTips_Layout layout = new UI_Pet_SkillTips_Layout();
        private List<uint> skillIdList;
        private int infinityCount;
        private EPetRemakeSkillState currentSelect = EPetRemakeSkillState.Common | EPetRemakeSkillState.Senior;
       

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
        }

        protected override void OnShow()
        {
            SetListView();
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        private void SetListView()
        {
            skillIdList = Sys_Pet.Instance.GetRemakeSkillBySortType(currentSelect);
            infinityCount = skillIdList.Count;
            layout.SetInfinityGridCell(infinityCount);
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnSkillSelect);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= infinityCount)
                return;
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            if (index < skillIdList.Count)
            {
                uint skillId = skillIdList[index];
                entry.SetData(skillId, false, false);
            }
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            uint skillId = petSkillCeil.petSkillBase.skillId;
            if (skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skillId, 0));
            }
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_SkillTips);
        }

        public void OnCommonValueChange(bool isOn)
        {
            currentSelect = isOn ? currentSelect | EPetRemakeSkillState.Common : currentSelect & ~EPetRemakeSkillState.Common;
            SetListView();
        }

        public void OnSeniorValueChange(bool isOn)
        {
            currentSelect = isOn ? currentSelect | EPetRemakeSkillState.Senior : currentSelect & ~EPetRemakeSkillState.Senior;
            SetListView();
        }
    }
}