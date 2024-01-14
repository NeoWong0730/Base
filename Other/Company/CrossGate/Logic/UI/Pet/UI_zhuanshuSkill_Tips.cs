using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Table;
using System;

namespace Logic
{
    public class UI_zhuanshuSkill_Tips : UIBase
    {
        private Button btnClose;

        private PetSkillItem02 skill;
        private GameObject objPassvieTag;
        private Text textName;
        private Text textdesc;
        private Text textdescYuanHe;
        private Text textMp;
        private Text textMpValue;
        private GameObject bg;
        private RectTransform rectScroll;

        private uint skillId = 0;
        private uint tipsId = 0;
        private uint skillYuanId = 0;

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                if (arg is Tuple<uint, uint> tuple)
                {
                    skillId = tuple.Item1;
                    tipsId = tuple.Item2;
                }
                else
                {
                    skillId = 0;
                    tipsId = 0;
                }
            }
            else
            {
                skillId = 0;
                tipsId = 0;
            }
            skillYuanId = skillId + 1;
        }

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Animator/Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            skill = new PetSkillItem02();
            skill.Bind(transform.Find("Animator/View_SkillTips/PetSkillItem02").gameObject);
            skill.EnableLongPress(false);
            objPassvieTag = transform.Find("Animator/View_SkillTips/Text_Passive").gameObject;
            textName = transform.Find("Animator/View_SkillTips/Text_Name").GetComponent<Text>();
            textdesc = transform.Find("Animator/View_SkillTips/bg/Scroll View/Content/Item/Text_Describe").GetComponent<Text>();
            textdescYuanHe = transform.Find("Animator/View_SkillTips/bg/Scroll View/Content/Item (1)/Text_Describe").GetComponent<Text>();
            textMp = transform.Find("Animator/View_SkillTips/Text_Mp").GetComponent<Text>();
            textMpValue = transform.Find("Animator/View_SkillTips/Text_Mp/Text").GetComponent<Text>();
            bg = transform.Find("Animator/View_SkillTips/bg").gameObject;
            rectScroll = transform.Find("Animator/View_SkillTips/bg/Scroll View").GetComponent<RectTransform>();
        }

        protected override void OnShow()
        {
            rectScroll.GetComponent<ContentSizeFitter>().enabled = true;
            rectScroll.sizeDelta = new Vector2(rectScroll.rect.width, 250f);
            rectScroll.GetComponent<ScrollRect>().enabled = false;
            UpdatePanel();
        }

        protected override void OnShowEnd()
        {
            //强刷刷新布局：处理嵌套layout 自动布局与content size fitter 时，布局刷新失败。后续有更好方法可以再进行优化
            FrameworkTool.ForceRebuildLayout(bg);

            if (rectScroll.rect.height > 360) //开启滑动视图
            {
                rectScroll.GetComponent<ContentSizeFitter>().enabled = false;
                rectScroll.sizeDelta = new Vector2(rectScroll.rect.width, 360f);
                rectScroll.GetComponent<ScrollRect>().enabled = true;
            }
        }

        private void UpdatePanel()
        {
            textMp.gameObject.SetActive(false);
            objPassvieTag.SetActive(true);
            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
            CSVPassiveSkillInfo.Data skillYuanHeInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillYuanId);
            if (skillInfo != null)
            {
                skill.SetDate(skillId);
                textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                textdesc.text = LanguageHelper.GetTextContent(skillInfo.desc);
            }
            else
            {
                Debug.LogErrorFormat("not found skillId={0}", skillId);
            }
            if (skillYuanHeInfo != null)
            {
                textdescYuanHe.text = LanguageHelper.GetTextContent(skillYuanHeInfo.desc);
            }
            else
            {
                Debug.LogErrorFormat("not found YuanHeSkillId={0}", skillYuanId);
            }
        }


        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_zhuanshuSkill_Tips);
        }
    }
}
