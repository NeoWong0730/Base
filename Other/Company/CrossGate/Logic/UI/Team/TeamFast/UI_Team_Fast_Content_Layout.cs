using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public partial class UI_Team_Fast_Layout
{
    #region LeftChildItem
    public class LeftChildItem : IntClickItem//ToggleIntClickItem
    {
        private CP_ToggleGroup mToggleGroup;

        private Text mLightText;
        private Text mDarkText;

        private CP_Toggle m_Toggle;
        public string text { set { mLightText.text = value; mDarkText.text = value; } }

        public uint ID { get; set; }

        private Transform m_ImgMatch;
        public override void Load(Transform root)
        {
            mTransform = root;

            m_Toggle = mTransform.GetComponent<CP_Toggle>();

            mToggleGroup = mTransform.GetComponent<CP_ToggleGroup>();

            mLightText = mTransform.Find("Btn_Menu_Light/Text_Menu").GetComponent<Text>();
            mDarkText = mTransform.Find("Btn_Menu_Dark/Text_Menu").GetComponent<Text>();

            base.Load(root);

            m_ImgMatch = root.Find("Image_Match");

            m_Toggle.onValueChanged.AddListener(OnClick);

        }
        public override ClickItem Clone()
        {
            return Clone<LeftChildItem>(this);
        }

        protected void OnClick(bool b)
        {
            if (b)
                clickItemEvent?.Invoke(Index);
        }

        public void SetToggleOn(bool b, bool sendmessage = false)
        {
            m_Toggle.SetSelected(b, sendmessage);
        }

        public void SetMatchActive(bool b)
        {
            if (m_ImgMatch.gameObject.activeSelf != b)
                m_ImgMatch.gameObject.SetActive(b);
        }
    }

    #endregion
}
public partial class UI_Team_Fast_Layout
{
    #region LeftItem
    public class LeftItem : IntClickItem
    {
        public Toggle mItem;

        public CP_ToggleRegistry m_ToggleGroup;
        public Transform mChild;

        public ClickItemGroup<LeftChildItem> mChildItemGroup = null;

        private Text mDarkTex;
        private Text mLightTex;

        private string m_Textstr;
        public string NameText { get { return m_Textstr; } set { m_Textstr = value; SetText(value); } }

        public Action<uint, int, uint, int> OnClickChildAc;
        public uint ID { get; set; }

        public bool isMark { get; set; }

        private bool m_bHoldOn = false;
        public bool HoldOn { set { /*m_bHoldOn = value;*/ SetToggleValue(value); } }

        private Transform mDarkSign;
        private Transform mLightSign;

        private bool m_bIsSign = true;
        public bool isSign { get { return m_bIsSign; } set { if (m_bIsSign == value) return; m_bIsSign = value; SetSign(value); } }


        public int mCurFocusChildIndex = -1;

        public Button m_Btn;
        public CP_TransformContainer m_TransformContainer;

        public CP_TransformContainer m_DropTransformContainer;

        private Transform m_TransDropSign;
        private bool IsOn { get; set; } = false;

        public bool isDrop { get; set; } = false;
        public override void Load(Transform root)
        {
            mTransform = root;

            mItem = mTransform.Find("ItemBig").GetComponent<Toggle>();

            m_Btn = mTransform.Find("ItemBig/Button").GetComponent<Button>();

            m_TransformContainer = mItem.GetComponent<CP_TransformContainer>();
            m_DropTransformContainer = mTransform.Find("ItemBig/Drop").GetComponent<CP_TransformContainer>();
            m_TransDropSign = mTransform.Find("ItemBig/Drop");

            mDarkTex = mItem.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            mLightTex = mItem.transform.Find("Btn_Menu_Light/Text_Menu_Dark").GetComponent<Text>();

            mDarkSign = mItem.transform.Find("Btn_Menu_Dark/Text_Menu_Dark/Button_Unfold");
            mLightSign = mItem.transform.Find("Btn_Menu_Light/Button_Close");

            mChild = mTransform.Find("MenuSmall");

            m_ToggleGroup = mChild.GetComponent<CP_ToggleRegistry>();

            Transform childItem = mChild.Find("ItemSmall");


            LeftChildItem leftChildItem = new LeftChildItem();
            leftChildItem.Load(childItem);

            mChildItemGroup = new ClickItemGroup<LeftChildItem>(leftChildItem);
            mChildItemGroup.SetAddChildListenter(OnAddChild);

            //  mItem.onValueChanged.AddListener(OnClickToggle);

            m_Btn.onClick.AddListener(OnClickBtn);
        }

        public override ClickItem Clone()
        {
            return Clone<LeftItem>(this);
        }


        private void SetToggleValue(bool b)
        {
            //if (mItem != null)
            //    mItem.isOn = b;
            IsOn = b;

            m_TransformContainer.ShowHideBySetActive(IsOn);

            if (IsOn == false)
                SetDrop(false);
        }
        private void OnAddChild(LeftChildItem item)
        {
            if (item == null)
                return;

            item.clickItemEvent.AddListener(OnClickChild);

        }

        private void OnClickChild(int nindex)
        {
            var item = mChildItemGroup.getAt(nindex);

            if (item == null)
                return;

            if (mCurFocusChildIndex < 0)
            {
                m_ToggleGroup.allowSwitchOff = false;
            }

            //取消父节点的选中态，将选中交给子节点
            m_TransformContainer.ShowHideBySetActive(false);
            IsOn = false;

            mCurFocusChildIndex = nindex;

            OnClickChildAc?.Invoke(ID, Index, item.ID, nindex);
        }

        private void OnClickBtn()
        {
            bool bsend = !IsOn;

            SetToggleValue(true);

            SetDrop(!isDrop);

            if (bsend)
                clickItemEvent?.Invoke(Index);
        }

        private void SetDrop(bool b)
        {
            isDrop = b;

            if (isDrop)
                ShowChild();
            else
                HideChild();

            m_DropTransformContainer.ShowHideBySetActive(isDrop);
        }
        private void LostFocus()
        {
            if (mChildItemGroup == null)
                return;

            for (int i = 0; i < mChildItemGroup.items.Count; i++)
            {
                mChildItemGroup.items[i].SetToggleOn(false, true);
            }
        }

        private void ShowChild()
        {
            mChild.gameObject.SetActive(true);

            // if (isMark == false)
            {
                m_ToggleGroup.allowSwitchOff = true;
                LostFocus();
            }

            //clickItemEvent?.Invoke(Index);
        }

        private void HideChild()
        {
            m_ToggleGroup.allowSwitchOff = true;

            mChild.gameObject.SetActive(false);

            mCurFocusChildIndex = -1;

        }

        private bool getChildActive()
        {
            return mChild.gameObject.activeSelf;
        }
        public void SetText(string text)
        {
            mDarkTex.text = text;
            mLightTex.text = text;

        }

        private void SetSign(bool b)
        {
            //mDarkSign.gameObject.SetActive(b);
            //mLightSign.gameObject.SetActive(b);
        }


        public int getItemByID(uint id)
        {
            int count = mChildItemGroup.Count;
            for (int i = 0; i < count; i++)
            {
                var item = mChildItemGroup.getAt(i);

                if (item != null && item.ID == id)
                    return i;
            }

            return -1;
        }

        public LeftChildItem getItem(int index)
        {
            return mChildItemGroup.getAt(index);
        }

        public void SetChildCount(int count)
        {
            mChildItemGroup.SetChildSize(count);

            if (count > 0 && m_TransDropSign.gameObject.activeSelf == false)
                m_TransDropSign.gameObject.SetActive(true);
            else if(count == 0 && m_TransDropSign.gameObject.activeSelf)
                m_TransDropSign.gameObject.SetActive(false);
        }

        public void SetChildFocus(int index)
        {
            var item = mChildItemGroup.getAt(index);

            if (item == null)
                return;

            HoldOn = true;

            SetDrop(true);

            item.SetToggleOn(true, true);
        }
    }

    #endregion
}
//
public partial class UI_Team_Fast_Layout
{

    private ClickItemGroup<LeftItem> mLeftItemGroup = null;

    private Transform mLeftItemParent;

    private ScrollRect m_LeftScroll;
    private void LoadLeftContent(Transform root)
    {
        m_LeftScroll = root.Find("Animator/View_Apply/Middle_Menu/Scroll_View").GetComponent<ScrollRect>();
        mLeftItemParent = root.Find("Animator/View_Apply/Middle_Menu/Scroll_View/TabList");

        Transform item = mLeftItemParent.Find("MenuItem");

        LeftItem leftItem = new LeftItem();
        leftItem.Load(item);

        mLeftItemGroup = new ClickItemGroup<LeftItem>(leftItem);
        mLeftItemGroup.SetAddChildListenter(OnAddChild);
    }

    private void OnAddChild(LeftItem leftItem)
    {
        if (leftItem == null)
            return;

        leftItem.clickItemEvent.AddListener(OnClickChild);

        leftItem.OnClickChildAc = OnClickGrandSon;
    }

    private void OnClickChild(int index)
    {
        var item = getLeftItem(index);

        if (item == null)
            return;

        int count = mLeftItemGroup.Count;

        for (int i = 0; i < count; i++)
        {
            var value = mLeftItemGroup.getAt(i);
            if (value != null && item != value)
            {
                value.HoldOn = false;
            }
        }
        m_listener?.OnClickTypeContent(item.ID, index);
    }

    private void OnClickGrandSon(uint parentID, int parent, uint ID, int index)
    {
        m_listener?.OnClickChildContent(parentID, parent, ID, index);
    }
    public LeftItem getLeftItem(int index)
    {
        return mLeftItemGroup.getAt(index);
    }

    public void SetLeftItemCount(int count)
    {
        mLeftItemGroup.SetChildSize(count);
    }

    public int GetLeftItemCount()
    {
        return mLeftItemGroup.Count;
    }
    public void SetMark(int index, bool b)
    {
        var itemLayout = getLeftItem(index);

        if (itemLayout == null)
            return;

        itemLayout.isMark = b;


        if (b)
        {
            for (int i = 0; i < mLeftItemGroup.Count; i++)
            {
                if (i == index)
                    continue;

                var item = getLeftItem(i);
                item.isMark = false;

            }
        }

    }

    public int getLeftItemByID(uint id)
    {
        int count = mLeftItemGroup.Count;

        for (int i = 0; i < count; i++)
        {
            var item = mLeftItemGroup.getAt(i);

            if (item != null && item.ID == id)
                return i;
        }
        return -1;
    }
    public void RestContentLayout()
    {
        if (mLeftItemGroup != null)
        {
            for (int i = 0; i < mLeftItemGroup.items.Count; i++)
            {
                var children = mLeftItemGroup.items[i];

                if (children == null)
                    continue;

                children.HoldOn = false;
            }
        }
    }


    private void RestContent()
    {
        if (mLeftItemGroup != null)
        {
            for (int i = 0; i < mLeftItemGroup.items.Count; i++)
            {
                var children = mLeftItemGroup.items[i];

                if (children == null)
                    continue;
                children.isMark = false;
                children.HoldOn = false;
            }
        }
    }


    public void FocueChildItem(uint typeID, uint id)
    {


        var typeIndex = getLeftItemByID(typeID);

        if (typeIndex < 0)
            return;

        int childIndex = getFocusChildIndex(id, typeIndex);

        if (childIndex == -1)
            return;

        var typeItem = getLeftItem(typeIndex);

        var childItem = typeItem.getItem(childIndex);

        //typeItem.HoldOn = true;

        //childItem.SetToggleOn(true, true);

        typeItem.SetChildFocus(childIndex);

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_LeftScroll.content);

        var scrollRectTrans = m_LeftScroll.transform as RectTransform;

        if (scrollRectTrans.rect.height >= m_LeftScroll.content.rect.height)
            return;

        RectTransform chidRect = childItem.mTransform as RectTransform;

        var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(m_LeftScroll.content, chidRect);

        var offset = m_LeftScroll.content.rect.height - scrollRectTrans.rect.height;

        var value = 1 - (Mathf.Abs(bounds.center.y) - scrollRectTrans.rect.height / 2) / (m_LeftScroll.content.rect.height - scrollRectTrans.rect.height);

        value = Mathf.Clamp(value, 0, 1);

        m_LeftScroll.verticalNormalizedPosition = value;

       // m_listener?.OnFocusChildContent(typeItem.ID, typeIndex, id, childIndex);
    }

    /// <summary>
    /// 获取玩法 UI控件的下标
    /// </summary>
    /// <param name="id">玩法ID</param>
    /// <param name="parentIndex">父控件的下标，可通过getFocusTypeIndex 获得</param>
    /// <returns></returns>
    private int getFocusChildIndex(uint id, int parentIndex)
    {
        var typeItem = getLeftItem(parentIndex);

        if (typeItem == null)
            return -1;

        int index = typeItem.getItemByID(id);

        return index;
    }
}
