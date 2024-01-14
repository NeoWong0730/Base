using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using Framework;
using System;
using Lib.Core;

public partial class UI_Team_OrederCompiler_Layout
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

        public Func<bool> OnClickSureAc;

        public string text {
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
            if (string.IsNullOrEmpty(str) == false && str.Length > 2)
            {
                str = str.Substring(0, 2);

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
           bool reslut = OnClickSureAc == null ? true: OnClickSureAc.Invoke();

            if (!reslut)
                return;

            Hide();
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

public partial class UI_Team_OrederCompiler_Layout
{
    private class FastItem : ClickItem
    {
        private Text m_TexName;
        private Transform m_Add;
        private Button m_BtnClear;

        private ButtonLongPress m_BtnLP;

        private Transform m_TransHad;

        private Button m_Btn;
        public int ID { get; set; }

        public int Index { get; set; }

        private IListener m_Listener;
        public override void Load(Transform root)
        {
            base.Load(root);

            m_TransHad = root.Find("Had");

            m_TexName = root.Find("Had/Text").GetComponent<Text>();
            m_Add = root.Find("Image_Add");

            m_BtnClear = root.Find("Had/Button_Clear").GetComponent<Button>();
            m_BtnLP = root.GetComponent<ButtonLongPress>();
            m_Btn = root.GetComponent<Button>();
        }

        public void SetListener(IListener listener)
        {
            m_BtnLP.onClick.AddListener(listener.OnLongPress);
            m_BtnClear.onClick.AddListener(OnClikckRemove);
            m_Btn.onClick.AddListener(OnClickBtn);

            m_Listener = listener;
        }
        private void OnClikckRemove()
        {
            if (m_Listener != null)
                m_Listener.OnRemoveFast(ID);
        }
        private void OnClickBtn()
        {
            if (m_Listener != null)
                m_Listener.OnClickFastAdd(Index);
        }
        public void SetName(string name)
        {
            string tex = name;
            if (string.IsNullOrEmpty(name) == false && name.Length > 2)
            {
                tex = name.Substring(0, 2);
            }
            m_TexName.text = tex;
        }

        public void SetEdit(bool b)
        {
            //m_Add.gameObject.SetActive(b);

            m_BtnClear.gameObject.SetActive(b);
        }

        public void SetHad(bool b)
        {
            if (b != m_TransHad.gameObject.activeSelf)
                m_TransHad.gameObject.SetActive(b);

           // if (b == m_Add.gameObject.activeSelf)
                m_Add.gameObject.SetActive(!b);

            m_Btn.enabled = !b;
        }
    }

    private ClickItemGroup<FastItem> m_FastGroup = new ClickItemGroup<FastItem>() { AutoClone = false };
    private void LoadFast(Transform root)
    {
        int count = root.childCount;

        for (int i = 0; i < count; i++)
        {
            var item = root.Find("Button0"+(i+1).ToString());
            m_FastGroup.AddChild(item);
        }

      
    }

    private void OnAddFastItem(FastItem fastItem)
    {
        fastItem.SetListener(m_listener);
    }

    public int GetFastCount()
    {
        return m_FastGroup.Count;
    }

    public void SetFastCount(int count)
    {
        m_FastGroup.SetChildSize(count);
    }

    public void SetFastItem(int index, string name,int id)
    {
        var item = m_FastGroup.getAt(index);

        if (item == null)
            return;

        item.SetName(name);
        item.ID = id;
        item.Index = index;
    }

    public void SetFastEdit(bool b)
    {
        int count = m_FastGroup.Count; ;

        for (int i = 0; i < count; i++)
        {
            var item = m_FastGroup.getAt(i);

            if (item != null)
                item.SetEdit(b);
        }
    }

    public void SetFastItemHad(int index, bool bHad)
    {
        var item = m_FastGroup.getAt(index);

        if (item == null)
            return;

        item.SetHad(bHad);
    }
}

//
public partial class UI_Team_OrederCompiler_Layout
{


    IListener m_listener;


    private Button mAddCommandBtn;
    private Button mCommandItemBtn;

    private Toggle mMyCommandTog;
    private Toggle mEnemyCommandTog;

    private Button mSystemDefaultBtn;

    private Button mClose;

    private List<CommandItem> mItems = new List<CommandItem>();

    private Transform mItemParentTran;


    private Button mBtnBackBoard;
    //编辑框

    //private Transform mEditTrans;
    //private InputField mEditInputField;
    //private Button mEditCancle;
    //private Button mEditSure;

    InputEdit mInputEdit = new InputEdit();

    private int mEditState = 0; // 0 修改 1 添加

    private int mEditIndex = -1;


    public class CommandItem
    {
        public Button Btn { get; set; }

        public Button EditBtn { get; set; }
        public int Index { get; set; }


        public Text text { get; set; }

        public ClickItemEvent ClickEvent = new ClickItemEvent();
        public ClickItemEvent ClickEditEvent = new ClickItemEvent();
        public void OnClick()
        {
            ClickEvent?.Invoke(Index);
        }

        public void OnClickEdit()
        {
            ClickEditEvent?.Invoke(Index);
        }
    }

    public void Loaded(Transform root)
    {
        mBtnBackBoard = root.Find("Image_Black").GetComponent<Button>();

        mCommandItemBtn = root.Find("Animator/OederScroll/TabList/Button_Order").GetComponent<Button>();
        mAddCommandBtn = root.Find("Animator/OederScroll/TabList/Button_Add").GetComponent<Button>();

        mMyCommandTog = root.Find("Animator/TeamToggle/MyTeam").GetComponent<Toggle>();
        mEnemyCommandTog = root.Find("Animator/TeamToggle/EnemyTeam").GetComponent<Toggle>();

        mSystemDefaultBtn = root.Find("Animator/Button_Default").GetComponent<Button>();

        mClose = root.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();

        mItemParentTran = mCommandItemBtn.transform.parent;

        mCommandItemBtn.gameObject.SetActive(false);
        mMyCommandTog.onValueChanged.AddListener(OnClickMine);
        mEnemyCommandTog.onValueChanged.AddListener(OnClickEnemy);
        mSystemDefaultBtn.onClick.AddListener(OnClickRest);
        mClose.onClick.AddListener(OnClickClose);
        mAddCommandBtn.onClick.AddListener(OnClickAddCommand);

        Transform EditTrans = root.Find("Animator/EditorDialog");

        mInputEdit.Load(EditTrans);

        mInputEdit.OnClickSureAc = OnClickEditSure;
        //mEditInputField = mEditTrans.Find("InputTarget/InputField").GetComponent<InputField>();
        //mEditCancle = mEditTrans.Find("InputTarget/Button_Cancel").GetComponent<Button>();
        //mEditSure = mEditTrans.Find("InputTarget/Button_OK").GetComponent<Button>();


        //mEditSure.onClick.AddListener(OnClickEditSure);
        //mEditCancle.onClick.AddListener(OnClickEditCancle);

        LoadFast(root.Find("Animator/Layout"));
    }

    public void Init(int state)
    {
       
        mMyCommandTog.isOn = (state == 0);

        mEnemyCommandTog.isOn = (state == 1);


    }

    public void SetAddBtnActive(bool active)
    {
        if (mAddCommandBtn.gameObject.activeSelf != active)
            mAddCommandBtn.gameObject.SetActive(active);
    }
    public void SetSystemOrderState(bool b)
    {
        mSystemDefaultBtn.enabled = b;

        ImageHelper.SetImageGray(mSystemDefaultBtn.GetComponent<Image>(), !b);
    }
    public bool MyCommandTogState()
    {
       return mMyCommandTog.isOn;
    }
    public void RegisterEvents(IListener listener)
    {
        m_listener = listener;

        m_FastGroup.SetAddChildListenter(OnAddFastItem);

        mBtnBackBoard.onClick.AddListener(listener.OnClickBack);
    }

    public void Update(float dt)
    {
        mInputEdit.Update(dt);
    }
    private CommandItem CloneItem()
    {
        GameObject go = GameObject.Instantiate<GameObject>(mCommandItemBtn.gameObject);

        go.transform.SetParent(mItemParentTran, false);

        CommandItem item = new CommandItem();

        item.Btn = go.GetComponent<Button>();

        item.text = go.transform.Find("Text").GetComponent<Text>();

        item.EditBtn = go.transform.Find("EditBtn").GetComponent<Button>();

        item.Btn.onClick.AddListener(item.OnClick);
        item.ClickEvent.AddListener(OnClickCommandItem);

        item.EditBtn.onClick.AddListener(item.OnClickEdit);
        item.ClickEditEvent.AddListener(OnClickEdit);



        return item;
    }

    public void SetCommandSize(int count)
    {
        CalcItems(count);

        for (int i = 0; i < mItems.Count; i++)
        {
            if (i >= count)
                mItems[i].Btn.gameObject.SetActive(false);
            else
            {
                mItems[i].Btn.gameObject.SetActive(true);

            }
        }
    }
    public void SetCommand(List<string> commands,bool activeEditor = true,Func<string,int,bool> func = null)
    {
        int count = commands.Count;

        CalcItems(count);

        for (int i = 0; i < mItems.Count; i++)
        {
            if (i >= count)
                mItems[i].Btn.gameObject.SetActive(false);
            else
            {
                mItems[i].Btn.gameObject.SetActive(true);

                mItems[i].text.text = commands[i];

                mItems[i].Index = i;

                bool isCustom = func == null ? false:func.Invoke(commands[i],i);

                bool isShowEditor = activeEditor == false ? false : isCustom;

                mItems[i].EditBtn.gameObject.SetActive(isShowEditor);

                mItems[i].Btn.gameObject.transform.SetSiblingIndex(i);
            }
        }

        mAddCommandBtn.gameObject.SetActive(commands.Count < 9);
    }

    private void CalcItems(int count)
    {
        if (count > mItems.Count)
        {
            int hadCount = mItems.Count;
            for (int i = 0; i < count - hadCount; i++)
            {
                CommandItem item = CloneItem();

                mItems.Add(item);
            }
        }

    }

    public void OnClickClose()
    {
        m_listener.Close();
    }
    private void ShowEdit(string defalutstr)
    {
        //mEditTrans.gameObject.SetActive(true);

        mInputEdit.Show();

        mInputEdit.text = defalutstr;
    }

    private void HideEdit()
    {
        //mEditTrans.gameObject.SetActive(false);

        //mInputEdit.Hide();
    }
    private void OnClickEdit(int index)
    {
        mEditState = 0;
        mEditIndex = index;
        ShowEdit(mItems[index].text.text);
    }
    private void OnClickCommandItem(int index)
    {
        m_listener.Command(index);
    }

    private void OnClickMine(bool state)
    {
        if (state)
            m_listener.MyCommand();
    }

    private void OnClickEnemy(bool state)
    {
        if (state)
            m_listener.EnemyCommand();
    }

    private void OnClickRest()
    {
        m_listener.ResetCommand();
    }

    private bool OnClickEditSure()
    {
        //HideEdit();

        string newStr = mInputEdit.text;

        if (newStr.Length > 2)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002251));
            return false;
        }

        if (newStr.Length == 0)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002252));
            return false;
        }

        if (string.IsNullOrWhiteSpace(newStr))
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10768));
            return false;
        }

        if (newStr.Contains(" "))
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10768));
            return false;
        }

        bool isIllegal =  IllegalWordDetection.IllegalWordsExistJudgement(newStr);

        if (isIllegal)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002253));
            return false;
        }

        if (mEditState == 0)
        {
            //mItems[mEditIndex].text.text = newStr;

            m_listener.EditCommand(mEditIndex, newStr);

            mEditIndex = -1;
            return true;
        }


        if (mEditState == 1)
        {
            m_listener.AddCommand(newStr);
        }

        return true;
    }


    //private void OnClickEditCancle()
    //{
    //   // HideEdit();
    //}


    private void OnClickAddCommand()
    {
        mEditState = 1;
        ShowEdit(string.Empty);

    }
    public interface IListener
    {
        void MyCommand();
        void EnemyCommand();

        void ResetCommand();

        void EditCommand(int index, string newStr);

        void AddCommand(string newstr);

        void Command(int index);

        void Close();

        void OnLongPress();
        void OnRemoveFast(int id);

        void OnClickFastAdd(int index);
        void OnClickBack();
    }
















}
