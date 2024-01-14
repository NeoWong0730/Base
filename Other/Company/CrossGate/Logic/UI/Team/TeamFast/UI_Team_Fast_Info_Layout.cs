using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;


//
public partial class UI_Team_Fast_Layout
{
    #region ContentItem

    public class ContentItemJobItem
    {
        public Image Icon;
        public Text Level;

        public Transform m_Transform;
        public void Load(Transform root)
        {
            Icon = root.Find("Image_Job").GetComponent<Image>();
            Level = root.Find("Text_Num").GetComponent<Text>();

            m_Transform = root;
        }

        public void SetActive(bool active)
        {
            if (m_Transform.gameObject.activeSelf != active)
                m_Transform.gameObject.SetActive(active);
        }
    }
    public class ContentItem : ButtonIntClickItem
    {
        private Text roleNameTex { get; set; }

        private Image propIcon { get; set; }

        private Image roleIcon { get; set; }

        private Image roleIconFrame { get; set; }

        private Text numberTex { get; set; }

        private Text professionTex { get; set; }

        private Text taskTex { get; set; }

        private Text taskTypeTex { get; set; }

        private Text taskInfoTex { get; set; }

        private Text countTex { get; set; }
        private Button applyBtn { get; set; }


        public string RoleName { set { if (roleNameTex == null) return;  roleNameTex.text = value; } }
        public uint PropIcon { set {
                if (propIcon == null)
                    return;

                propIcon.gameObject.SetActive(value != 0);

                if (value == 0)
                {
                    return;
                }

                ImageHelper.SetIcon( propIcon,value);
            } }
        public uint RoleIcon { set { if (roleIcon == null || value == 0) return; ImageHelper.SetIcon(roleIcon, value); } }
        public uint RoleFrameIcon
        {
            set
            {
                if (roleIconFrame == null) return;
                if (value == 0)
                {
                    roleIconFrame.gameObject.SetActive(false);
                }
                else
                {
                    roleIconFrame.gameObject.SetActive(true);
                    ImageHelper.SetIcon(roleIconFrame, value);
                }
            }
        }


        public string Number{ set { if (numberTex == null) return; numberTex.text = value; } }
        public uint Profession { set {
                if (professionTex == null)
                    return;

                professionTex.gameObject.SetActive(value != 0);

                if (value == 0)
                    return;

                professionTex.text = LanguageHelper.GetTextContent(value);
            } }
        public string Task { set { if (taskTex == null) return; taskTex.text = value; } }
        public string TaskType { set { if (taskTypeTex == null) return; taskTypeTex.text = value; } }
        public string Desc { set { if (taskInfoTex == null) return; taskInfoTex.text = value; } }
        public string Count { set { if (countTex == null) return; countTex.text = value; } }

        public uint ID { get; set; }

        List<ContentItemJobItem> m_MemberJoblist = new List<ContentItemJobItem>();
        public override void Load(Transform root)
        {
            mTransform = root;

            roleNameTex = mTransform.Find("Text_Name").GetComponent<Text>();

            //propIcon = mTransform.Find("Image_Prop").GetComponent<Image>();

            roleIcon = mTransform.Find("Image_BG/Head").GetComponent<Image>();

            roleIconFrame = mTransform.Find("Image_BG/Head/Image_Before_Frame").GetComponent<Image>();

            numberTex = mTransform.Find("Text_Number").GetComponent<Text>();

            //professionTex = mTransform.Find("Text_Profession").GetComponent<Text>();

            taskTex = mTransform.Find("Text_Task").GetComponent<Text>();

            taskTypeTex = mTransform.Find("Text_Task/Text_Type").GetComponent<Text>();

            countTex = mTransform.Find("Text_Task/Text_Number").GetComponent<Text>();

            taskInfoTex = mTransform.Find("Text_Content").GetComponent<Text>();

            applyBtn = mTransform.Find("Button_Apply").GetComponent<Button>();

            applyBtn.onClick.AddListener(OnClick);


            var jobtrans = mTransform.Find("Jobs");
            int count = jobtrans.childCount;

            for (int i = 0; i < count; i++)
            {
                string name = "Item0";

                if (i > 0)
                {
                    name = name + " (" + (i).ToString() + ")";
                }

               var transItem = jobtrans.Find(name);

                if (transItem != null)
                {
                    var itemj = new ContentItemJobItem();
                    itemj.Load(transItem);

                    m_MemberJoblist.Add(itemj);
                }
                
            }
        }

        public override ClickItem Clone()
        {
            return Clone<ContentItem>(this);
        }

        public void SetMemberCount(int count)
        {
            int mcount = m_MemberJoblist.Count;
            for (int i = 0; i < mcount; i++)
            {
                m_MemberJoblist[i].SetActive(i < count);
            }
        }
        public void SetMemberJob(int index, uint icon, uint level)
        {
            int mcount = m_MemberJoblist.Count;

            if (index >= mcount)
                return;

            ImageHelper.SetIcon(m_MemberJoblist[index].Icon,icon);
            m_MemberJoblist[index].Level.text = level.ToString();
        }
    }
    #endregion


    private ClickItemGroup<ContentItem> ContentItemGroup;

    private Transform mContentTrans;
    private void LoadContent(Transform root)
    {
        mContentTrans = root.Find("Animator/View_Apply/Right_Content/Scroll_View");

        Transform viewportTans = mContentTrans.Find("Viewport");

        Transform itemTrans = viewportTans.Find("Item");

        ContentItem item = new ContentItem();
        item.Load(itemTrans);

        ContentItemGroup = new ClickItemGroup<ContentItem>(item);

        ContentItemGroup.SetAddChildListenter(OnAddInfoItem);
    }
    private void OnAddInfoItem(ContentItem item)
    {
        if (item != null)
            item.clickItemEvent.AddListener(OnClickApply);
    }
    private void OnClickApply(int index)
    {
        var item = getContentItem(index);

        if (item != null)
            m_listener.Apply(index);
    }
    public ContentItem getContentItem(int index)
    {
        return ContentItemGroup.getAt(index);
    }

    public void SetContentItemCount(int count)
    {
        ContentItemGroup.SetChildSize(count);
    }

    /// <summary>
    /// 显示相匹配的队伍信息
    /// </summary>
    /// <param name="func">查询是否符合条件的回调</param>
    /// <param name="isType">是否显示一种类型，false 只显示与ID相等的控件</param>
    public void ShowItemCondition(Func<uint, bool, bool> func,bool isType)
    {
        int count = 0;
        for (int i = 0; i < ContentItemGroup.Count; i++)
        {
            var item = getContentItem(i);

            bool result = func.Invoke(item.ID, isType);

            if (result)
            {
                item.Show();
                count += 1;
            }
                
            else
                item.Hide();
        }

        SetEmptyActive(count == 0);
    }

}
