using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_DemonPreview_Layout
    {
        private Button closeBtn;
        private Text titleText;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Content/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            titleText = transform.Find("Animator/Content/Image_BG/Image_Title/Text").GetComponent<Text>();
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

        public void SetTitleText(uint skillId)
        {
            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);

            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    TextHelper.SetText(titleText, 680002045u, LanguageHelper.GetTextContent(skillInfo.name));
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                }
            }
            else
            {

                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    TextHelper.SetText(titleText, 680002045u, LanguageHelper.GetTextContent(skillInfo.name));
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0}", skillId);
                }
            }
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_Pet_DemonPreview : UIBase, UI_Pet_DemonPreview_Layout.IListener
    {
        private UI_Pet_DemonPreview_Layout layout = new UI_Pet_DemonPreview_Layout();
        private uint currentSkillId;
        List<uint> levels;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            levels = Sys_Pet.Instance.EquipActiveLevelLimit;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            currentSkillId = Convert.ToUInt32(arg);
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            layout.SetTitleText(currentSkillId);
            layout.SetInfinityGridCell(levels.Count + 1);
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
            GameObject go = cell.mRootTransform.gameObject;

            cell.BindUserData(go);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= levels.Count + 1)
                return;
            GameObject entry = cell.mUserData as GameObject;
            uint skillId = currentSkillId + (uint)index;
            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);
            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    TextHelper.SetText(entry.transform.Find("Text_Title").GetComponent<Text>(), 680002046, skillInfo.level.ToString());
                    TextHelper.SetText(entry.transform.Find("Text").GetComponent<Text>(), Sys_Skill.Instance.GetSkillDesc(skillId));
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                }
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    TextHelper.SetText(entry.transform.Find("Text_Title").GetComponent<Text>(), 680002046, skillInfo.level.ToString());
                    TextHelper.SetText(entry.transform.Find("Text").GetComponent<Text>(), LanguageHelper.GetTextContent(skillInfo.desc));
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0}", skillId);
                }
            }
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonPreview);
        }

    }
}