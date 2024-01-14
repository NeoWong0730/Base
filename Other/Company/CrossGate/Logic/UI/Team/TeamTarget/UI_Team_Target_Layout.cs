using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public partial class UI_Team_Target_Layout
{

    class InputEdit
    {
        UIAnimatorControl mAnimatorControl;

        Transform mRoot;

        private InputField mEditInputField;
        private Button mEditCancle;
        private Button mEditSure;

        private float animatoTime;

        int mState = -1;

        public Action OnClickSureAc;

        public string text
        {
            get { return mEditInputField.text; }
            set { mEditInputField.text = value; }
        }
        public void Load(Transform root)
        {
            mRoot = root;

            mAnimatorControl = mRoot.GetComponent<UIAnimatorControl>();

            mEditInputField = root.Find("InputTarget/InputField").GetComponent<InputField>();
            mEditCancle = root.Find("InputTarget/Button_Cancel").GetComponent<Button>();
            mEditSure = root.Find("InputTarget/Button_OK").GetComponent<Button>();


            mEditSure.onClick.AddListener(OnClickEditSure);
            mEditCancle.onClick.AddListener(OnClickEditCancle);

            mEditInputField.onValueChanged.AddListener(OnInputFieldEnd);
        }

        private void OnInputFieldEnd(string str)
        {
            if (string.IsNullOrEmpty(str) == false && str.Length > 4)
            {
                str = str.Substring(0, 4);

                text = str;
            }
        }
        public void Hide()
        {
            if (mAnimatorControl == null)
            {
                OnHide();
                return;
            }


            mAnimatorControl.PlayExit();

            animatoTime = mAnimatorControl.ExitTime;

            mState = 1;
        }

        private void OnHide()
        {
            mRoot.gameObject.SetActive(false);
        }
        public void Show()
        {
            if (mAnimatorControl == null)
            {
                OnShow();
                return;
            }

            animatoTime = mAnimatorControl.EnterTime;

            mAnimatorControl.PlayEnter();

            mState = 0;

            OnShow();
        }

        private void OnShow()
        {
            mRoot.gameObject.SetActive(true);
        }
        public bool isActive()
        {
            return mRoot.gameObject.activeSelf;
        }

        private void OnClickEditSure()
        {

            if (isSafeString(text) == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }

            if (text.Length > 4)
            {
                return;
            }
                

            OnClickSureAc?.Invoke();

            Hide();
        }

        private bool isSafeString(string strtext)
        {
            if (Sys_RoleName.Instance.HasBadNames(strtext))
            {
               // Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return false;
            }

            //if (Sys_WordInput.Instance.HasLimitWord(strtext))
            //{
                
            //    return false;
            //}

            if (string.IsNullOrEmpty(strtext))
            {
              
                return false;
            }

            if (string.IsNullOrWhiteSpace(strtext))
            {
                return false;
            }
            

            
            return true;
        }
        private void OnClickEditCancle()
        {
            Hide();
        }

        public void Update(float deltaTime)
        {
            if (mState >= 0)
            {
                animatoTime -= deltaTime;

                if (animatoTime <= 0)
                {
                    if (mState == 1)
                        OnHide();

                    mState = -1;
                }
            }

        }
    }

}


//
public partial class UI_Team_Target_Layout
{
    private Transform mLeftItemParent;

    private ScrollRect m_LeftScroll;

    IListener m_listener;

    private Text mTips;

    private ClickItemGroup<LeftItem> mLeftItemGroup = new ClickItemGroup<LeftItem>();

    public ClickItemGroup<LeftItem> ItemGroup { get { return mLeftItemGroup; } }

    private RectTransform mRequest;
    private UI_ScrollGrid mMinRequest;
    private UI_ScrollGrid mMaxRequest;

    private InputField mInputField;
    private Toggle mToggle;

    private Toggle m_TogRobbot;

   // private Transform mCustomInput;
    //private InputField mCustomInputField;

    private InputEdit mInputEdit = new InputEdit();
    public void Loaded(Transform root)
    {

        mLeftItemParent = root.Find("Animator/TargetScroll");
        m_LeftScroll = mLeftItemParent.GetComponent<ScrollRect>();

        Transform litem = mLeftItemParent.Find("TabList/TargetItem");
        mLeftItemGroup.AddChild(litem);

        mLeftItemGroup.SetAddChildListenter(OnChildAdd);

        mRequest = root.Find("Animator/Request").GetComponent<RectTransform>();

        mMinRequest = root.Find("Animator/Request/Min").GetComponent<UI_ScrollGrid>();
        mMaxRequest = root.Find("Animator/Request/Max").GetComponent<UI_ScrollGrid>();

        Button close = root.Find("View_TipsBg01_Small/Btn_Close").GetComponent<Button>();

        close.onClick.AddListener(OnClickClose);

        Button surebtn = root.Find("Animator/Button_OK").GetComponent<Button>();
        surebtn.onClick.AddListener(OnClickSure);

        mToggle = root.Find("Animator/Image_Background/Toggle_Match").GetComponent<Toggle>();

        mToggle.isOn = true;
        mToggle.onValueChanged.AddListener(OnClickAuto);


        mInputField = root.Find("Animator/InputField_Describe").GetComponent<InputField>();
        Button deleteInput = root.Find("Animator/InputField_Describe/Button_Delete").GetComponent<Button>();
        deleteInput.onClick.AddListener(OnClickDeleteInputText);

        mMinRequest.FocusChangeEvent.AddListener(OnMinFocus);
        mMaxRequest.FocusChangeEvent.AddListener(OnMaxFocus);

        mInputField.onEndEdit.AddListener(OnTextInputEnd);


        mTips = root.Find("Animator/Text_Tips").GetComponent<Text>();

        Transform CustomInput = root.Find("Animator/EditorDialog");

        mInputEdit.Load(CustomInput);
        mInputEdit.OnClickSureAc = OnClickCustomOK;


        m_TogRobbot = root.Find("Animator/Image_Background/Toggle_Robbot").GetComponent<Toggle>();

    }

    public void Init()
    {

    }

    public void Update(float dt)
    {
        mInputEdit.Update(dt);
    }

    public void Hide()
    {
        if (ItemGroup != null)
        {
            for (int i = 0; i < ItemGroup.items.Count; i++)
            {
                var children = ItemGroup.items[i];

                if (children == null)
                    continue;

                children.Mark = false;

                children.SetToggleValue(false,true);
            }
        }



    }
    public LeftChildItem FindGrandSonByID(uint id)
    {
        if (ItemGroup == null)
            return null;

        for (int i = 0; i < ItemGroup.items.Count; i++)
        {
            var children = ItemGroup.items[i];

            if (children == null)
                continue;

            for (int n = 0; n < children.mChildItemGroup.items.Count; n++)
            {
                var childItem = children.mChildItemGroup.items[n];

                if (childItem.ID == id)
                    return childItem;
            }
        }

        return null;
    }

    /// <summary>
    /// 查找组队类型的节点
    /// </summary>
    /// <param name="id">组队类型id</param>
    /// <returns></returns>
    public LeftItem FindChildByID(uint id)
    {
        if (ItemGroup == null)
            return null;

        for (int i = 0; i < ItemGroup.items.Count; i++)
        {
            var children = ItemGroup.items[i];

            if (children == null)
                continue;

            if (children.ID == id)
                return children;
        }

        return null;
    }

    public bool FindItem(uint id, out LeftItem parentItem, out LeftChildItem childItem)
    {
        bool result = false;
       
        int count = ItemGroup == null ? 0 : ItemGroup.items.Count;


        parentItem = new LeftItem();
        childItem = new LeftChildItem();


        for (int i = 0; i < count; i++)
        {
            var children = ItemGroup.items[i];

            if (children == null)
                continue;

            for (int n = 0; n < children.mChildItemGroup.items.Count; n++)
            {
                var childvalue = children.mChildItemGroup.items[n];

                if (childvalue.ID == id)
                {
                    result = true;

                    childItem = childvalue;

                    parentItem = children;

                    break;
                }
                    
            }

            if (result)
                break;
        }
        return result;
    }
    public void SetLevelRangle(int min, int max)
    {
        mMinRequest.SetValusRange(min, max);

        mMaxRequest.SetValusRange(min, max);


    }

    public void MinLevevFocus(int index)
    {
        mMinRequest.Focus(index);
    }

    public void MaxLevelFocus(int index)
    {
        mMaxRequest.Focus(index);
    }

    public void SetAutoMatchRobbot(bool active)
    {
        m_TogRobbot.isOn = active;
    }
    private void OnMinFocus(int index)
    {
        m_listener?.OnMinLvChange(index);
    }

    private void OnMaxFocus(int index)
    {
        m_listener?.OnMaxLvChange(index);
    }
    public void SetDesc(string text)
    {
        mInputField.text = text;
    }

    public void SetTips(string text)
    {
        mTips.text = text;
    }

    public void OpenCustomInput()
    {
        //mCustomInput.gameObject.SetActive(true);
        mInputEdit.Show();
    }

    public void CloseCustomInput()
    {
        //mCustomInput.gameObject.SetActive(false);
        mInputEdit.Hide();
    }

    public void SetCustomInputText(string text)
    {
        // mCustomInputField.text = text;

        mInputEdit.text = text;
    }
    public void RegisterEvents(IListener listener)
    {
        m_listener = listener;

        m_TogRobbot.onValueChanged.AddListener(listener.OnTogAutoMatchRobbot);
    }


    public void SetAutoFind(bool b)
    {
        mToggle.isOn = b;
    }

    public void SetPlayTypeMask(bool b, int index)
    {
        var item = mLeftItemGroup.getAt(index);

        if (item == null)
            return;

        item.Mark = b;
    }

    public void SetPlayTypeMask(bool b, uint id)
    {
        var item = FindChildByID(id);

        if (item == null)
            return;

        item.Mark = b;
    }

    /// <summary>
    /// 设置焦点到目标上
    /// </summary>
    /// <param name="id">组队表中的id</param>
    public void SetGrandSonFocus(uint id,bool showChild = false)
    {
        LeftItem parent = null;
        LeftChildItem child = null;

        if (FindItem(id, out parent, out child) == false)
            return;

        if (showChild)
        {
            parent.SetToggleValue(true, true);

            

        }
           
        child.Togg.SetSelected(true, true);

        
    }


    public void MoveToTarget(uint id)
    {
        int count = mLeftItemGroup.Count;

        float realY = 0;

        bool result = false;
        for (int i = 0; i < count; i++)
        {
            var item = mLeftItemGroup.getAt(i);

            if (item == null || item.mTransform.gameObject.activeSelf == false)
                continue;

            var itemrect = item.mItem.transform as RectTransform;

            realY += itemrect.sizeDelta.y;

            //Debug.LogError("add height " + item.ID.ToString() + ", height " + itemrect.sizeDelta.y.ToString());
            if (item.mChild.gameObject.activeSelf)
            {
                int childcount = item.mChildItemGroup.Count;
                for (int n = 0; n < childcount; n++)
                {
                    var childitem = item.mChildItemGroup.getAt(n);

                    if (childitem != null && childitem.mTransform.gameObject.activeSelf)
                    {
                        var childitemrect = childitem.mTransform as RectTransform;

                        //Debug.LogError("add child height " + childitem.ID.ToString() + ", height " + childitemrect.sizeDelta.y.ToString());
                        if (childitem.ID == id)
                        {
                            result = true;
                            break;
                        }
                        else
                        {
                            realY += childitemrect.sizeDelta.y;
                        }
                    }

                }
            }


            if (result)
                break;
        }

        var pos = m_LeftScroll.content.anchoredPosition;
        pos.y = (realY - m_LeftScroll.viewport.sizeDelta.y*0.5f);

        m_LeftScroll.content.anchoredPosition = pos;

       // Debug.LogError("set scroll pos: " + pos);
    }
    private void OnClickClose()
    {
        m_listener?.Close();
    }

    private void OnClickSure()
    {
        m_listener?.AutoFind(mToggle.isOn);
    }


    private void OnChildAdd(LeftItem clickItem)
    {
        clickItem.ClickAction = OnClickChild;


    }

    private void OnGrandSonAdd(LeftChildItem grandson)
    {

    }
    private void OnClickChild(int parent, int child)
    {
        var itemLayout = ItemGroup.getAt(parent);

        m_listener?.ClickItem(itemLayout.ID, parent, child);
    }

    private void OnTextInputEnd(string str)
    {

        m_listener?.OnTextInputEnd(str);
    }

    private void OnClickDeleteInputText()
    {
        m_listener?.OnTextDelete();
    }

    private void OnClickCustomOK()
    {
        string text = mInputEdit.text;

        m_listener?.OnCustomInput(text);
    }

    private void OnClickCustomCancle()
    {
        CloseCustomInput();
    }


    private void OnClickAuto(bool state)
    {
        m_listener?.OnAutoChange(state);
    }
    public interface IListener
    {
        void Close();
        void AutoFind(bool b);

        void ClickItem(uint targetID, int parent, int child);

        void OnMinLvChange(int index);
        void OnMaxLvChange(int index);

        void OnTextInputEnd(string text);
        void OnTextDelete();

        void OnCustomInput(string str);

        void OnAutoChange(bool b);

        void OnTogAutoMatchRobbot(bool b);
    }
















}
