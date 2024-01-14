using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;
using Framework;

//
public partial class UI_Team_FrightingClick_Layout
{


    IListener m_listener;


    private Button mAddCommandBtn;
    private Button mClearCommandBtn;
    private Button mClearAllCommandBtn;
    private Button mCommandItemBtn;


    private Button mClose;

    private List<CommandItem> mItems = new List<CommandItem>();

    private Transform mItemParentTran;
    


  //  private int mEditState = 0; // 0 修改 1 添加

   // private int mEditIndex = -1;



    private Toggle mTogOwn;
    private Toggle mTogEnemty;


    private Transform m_TransImageBlack;
    private Transform m_TransAnimator;

    private Text m_TexSideClear;

    UIAnimatorControl mAnimatorControl;
    public class CommandItem
    {
        public Button Btn { get; set; }

        public int Index { get; set; }

        public Text text { get; set; }

        public ClickItemEvent ClickEvent = new ClickItemEvent();
        public ClickItemEvent ClickEditEvent = new ClickItemEvent();
        public void OnClick()
        {
            ClickEvent?.Invoke(Index);
        }

    }

    public void Loaded(Transform root)
    {
        mCommandItemBtn = root.Find("Animator/Scroll_View/TabList/Button_Order").GetComponent<Button>();
        mAddCommandBtn = root.Find("Animator/Scroll_View/TabList/Button_Add").GetComponent<Button>();

        mClearCommandBtn = root.Find("Animator/Button_Default").GetComponent<Button>();
        mClearAllCommandBtn = root.Find("Animator/Button_Clear").GetComponent<Button>();

        mClose = root.Find("Animator/Btn_Close").GetComponent<Button>();

        mItemParentTran = mCommandItemBtn.transform.parent;

        mCommandItemBtn.gameObject.SetActive(false);

        mClose.onClick.AddListener(OnClickClose);
        mAddCommandBtn.onClick.AddListener(OnClickAddCommand);

        mClearCommandBtn.onClick.AddListener(OnClickClearAll);
        mClearAllCommandBtn.onClick.AddListener(OnClickClear);


        m_TexSideClear = mClearCommandBtn.transform.Find("Text").GetComponent<Text>();
        Text clearAllText = mClearAllCommandBtn.transform.Find("Text").GetComponent<Text>();

        //clearText.text = LanguageHelper.GetTextContent(2002081);
        //clearAllText.text = LanguageHelper.GetTextContent(2002082);

        mTogOwn = root.Find("Animator/TeamToggle/MyTeam").GetComponent<Toggle>();
        mTogEnemty = root.Find("Animator/TeamToggle/EnemyTeam").GetComponent<Toggle>();

        m_TransImageBlack = root.Find("Image_Black");
        m_TransAnimator = root.Find("Animator");

        mAnimatorControl = root.GetComponent<UIAnimatorControl>();
    }

    public void Init()
    {



    }
    public void RegisterEvents(IListener listener)
    {
        m_listener = listener;

        mTogOwn.onValueChanged.AddListener(listener.OnClickMyTeam);
        mTogEnemty.onValueChanged.AddListener(listener.OnClickEnemyTeam);
 
    }

    public void SetActive(bool bstate)
    {
        m_TransImageBlack.gameObject.SetActive(bstate);
        m_TransAnimator.gameObject.SetActive(bstate);

        if (bstate)
            mAnimatorControl.PlayEnter();
    }

    public void SetMyTeamToggleState(bool bstate)
    {
        mTogOwn.isOn = bstate;
    }

    public void SetEnemyTeamToggleState(bool bstate)
    {
        mTogEnemty.isOn = bstate;
    }

    public void SetSideClearTex(uint lanuge)
    {
        m_TexSideClear.text = LanguageHelper.GetTextContent(lanuge);
    }
    private CommandItem CloneItem()
    {
        GameObject go = GameObject.Instantiate<GameObject>(mCommandItemBtn.gameObject);

        go.transform.SetParent(mItemParentTran,false);

        CommandItem item = new CommandItem();

        item.Btn = go.GetComponent<Button>();

        item.text = go.transform.Find("Text").GetComponent<Text>();

        item.Btn.onClick.AddListener(item.OnClick);
        item.ClickEvent.AddListener(OnClickCommandItem);
        return item;
    }

    public void SetCommand(List<string> commands)
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

                mItems[i].Btn.gameObject.transform.SetSiblingIndex(i);
            }
        }
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
        m_listener?.Close();
    }

    private void OnClickCommandItem(int index)
    {
       m_listener.Command(index);
    }

    private void OnClickAddCommand()
    {
        m_listener?.AddCommand();

    }

    private void OnClickClear()
    {
        m_listener?.Clear();
    }

    private void OnClickClearAll()
    {
        m_listener?.ClearAll();
    }
    public interface IListener
    {
        void Clear();
        void ClearAll();
        void AddCommand();

        void Command( int index);

        void Close();

        void OnClickMyTeam(bool state);
        void OnClickEnemyTeam(bool state);
    }



 


 

 

  





}
