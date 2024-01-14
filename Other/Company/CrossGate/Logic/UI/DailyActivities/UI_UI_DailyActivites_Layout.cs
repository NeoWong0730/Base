using Framework;
using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;
namespace Logic
{

    public partial class UI_UI_DailyActivites_Layout
    {
        class TypeItem : CPToggleIntClickItem
        {
            public Image mImIcon;

            private Text mTexName;

            private Image mImRed;

            private Text mTexFocusName;


            private Transform m_TransRewardRed;
            public uint ConfigID { get; set; } = 0;
            public TypeItem()
            {
            }
            public TypeItem(Transform root)
            {
                Load(root);
            }
            public override void Load(Transform root)
            {
                base.Load(root);

               // mImIcon = root.Find("Image").GetComponent<Image>();

                mTexName = root.Find("Text").GetComponent<Text>();

                mImRed = root.Find("Image_RedTips").GetComponent<Image>();

                mTexFocusName = root.Find("Text_Select").GetComponent<Text>();

                m_TransRewardRed = root.Find("Image_Red");
            }


            public override ClickItem Clone()
            {
                return Clone<TypeItem>(this) as ClickItem;
            }



            public void SetName(string name)
            {
                mTexName.text = name;
                mTexFocusName.text = name;
            }

            public void SetRed(bool b)
            {
                mImRed.gameObject.SetActive(b);
            }

            public void SetIcon(Sprite sprite)
            {
                //mImIcon.sprite = sprite;
            }


            public void SetRewardRed(bool active)
            {
                m_TransRewardRed.gameObject.SetActive(active);
            }
        }

        public class RightDailyItem : UI_Daily_Common.DailyItem
        {
            private UI_ScrollGrid_Element mElement;

            private Transform m_TransNewTips;

           // private Transform m_TransRewardRed;

            private Transform m_TransBtnRewardRed;

            private IListener mListener;
            public RightDailyItem() { }
            public RightDailyItem(Transform root) { Load(root); }

            public Action<bool,uint> OnFocusAction;

            public Transform m_TransDifficult;
            public Text m_TexDifficult;
            public override void Load(Transform root)
            {
                base.Load(root);

                mElement = root.GetComponent<UI_ScrollGrid_Element>();

                m_TransNewTips = root.Find("Image_New");

              //  m_TransRewardRed = root.Find("Image_RedTips");
                m_TransBtnRewardRed = root.Find("Btn_01/Image_Red");

                m_TransDifficult = root.Find("Image_Dif");
                m_TexDifficult = root.Find("Image_Dif/Text").GetComponent<Text>();

                mElement.ValueChange.AddListener(OnFocus);
            }

            public override ClickItem Clone()
            {
                return Clone<RightDailyItem>(this);
            }

            public void SetListener(IListener listener)
            {
                mListener = listener;

                if (mBtn != null)
                    mBtn.onClick.AddListener(OnClickGoto);
            }
            private void OnClickGoto()
            {
                mListener?.OnClickGotoJoin(ConfigID);
            }
            private void OnFocus(bool b)
            {
                OnFocusAction?.Invoke(b, ConfigID);
            }

            public void SetNewTips(bool active)
            {
                if (m_TransNewTips.gameObject.activeSelf != active)
                    m_TransNewTips.gameObject.SetActive(active);
            }

            public void SetRewardRed(bool active)
            {
                //if (m_TransRewardRed.gameObject.activeSelf != active)
                //    m_TransRewardRed.gameObject.SetActive(active);

                if (m_TransBtnRewardRed.gameObject.activeSelf != active)
                    m_TransBtnRewardRed.gameObject.SetActive(active);
            }
        }
    }
    public  partial class UI_UI_DailyActivites_Layout
    {
        private IListener mListener;

        private ClickItemGroup<TypeItem> mLeftGroup;

        private ClickItemGroup<RightDailyItem> mRightGroup;

        private Button mBtnClose;

        private Button mBtnCalendar;

        private Button mBtnBell;

        //private Button mBtnReward;
        //private Transform mTransRewardHaveGet;


        //private Text mTexActivity;

        private Transform mDetail;

        private UI_ScrollGridEx mScrollGridEx;

        private Button mBtnJoin;

        private Transform mTypeFocusTrans;

        private GameObject awardGo;


        private UIAward m_UIAward = new UIAward();
        public void Load(Transform root)
        {
            Transform leftContentItem = root.Find("Animator/View_Left/Scroll_View/Viewport/Toggle_Cell");
            leftContentItem.gameObject.SetActive(false);

            mLeftGroup = new ClickItemGroup<TypeItem>() { OriginItem = new TypeItem(leftContentItem) };
            mLeftGroup.SetAddChildListenter(OnAddLeftItem);

            mTypeFocusTrans = root.Find("Animator/View_Right/Scroll_View/GameObject");

            Transform rightContentItem = root.Find("Animator/View_Right/Scroll_View/Viewport/ShopItem");

            mRightGroup = new ClickItemGroup<RightDailyItem>(new RightDailyItem(rightContentItem));
       

            mBtnClose = root.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();

            mBtnCalendar = root.Find("Animator/View_Right/Btn_Calendar").GetComponent<Button>();

            mBtnBell = root.Find("Animator/View_Right/Btn_Bell").GetComponent<Button>();

            //mBtnReward = root.Find("Animator/View_Right/Btn_Award").GetComponent<Button>();
           // mTransRewardHaveGet = root.Find("Animator/View_Right/Btn_Award/Image_Dot");

            //mTexActivity = root.Find("Animator/View_Right/Btn_Activity/Text_Activity/Text").GetComponent<Text>();

            mDetail = root.Find("Animator/View_Right/UI_Activity_Message");

            mScrollGridEx = root.Find("Animator/View_Right/Scroll_View").GetComponent<UI_ScrollGridEx>();

            mBtnJoin = root.Find("Animator/View_Right/Scroll_View/GameObject/Btn_01").GetComponent<Button>();

            awardGo = root.Find("Animator/View_Right/Award").gameObject;


            m_UIAward.Load(root.Find("Animator/View_Right/Award"));
        }


        public Transform GetDetail()
        {
            return mDetail;
        }


        private void OnAddLeftItem(TypeItem item)
        {
            item.clickItemEvent.AddListener(OnClickLeftItem);
        }

        private void OnAddRightItem(RightDailyItem item)
        {
            item.clickItemEvent.AddListener(OnClickRightItem);

            item.OnFocusAction = OnFocusRightItem;

            item.SetListener(mListener);
        }
        public void setListener(IListener listener)
        {
            mListener = listener;

            mRightGroup.SetAddChildListenter(OnAddRightItem);

            mBtnClose.onClick.AddListener(listener.OnClickClose);

            mBtnBell.onClick.AddListener(listener.OnBell);

            mBtnCalendar.onClick.AddListener(listener.OnCalendar);

            //mBtnReward.onClick.AddListener(listener.OnClickActivity);

            mBtnJoin.onClick.AddListener(listener.OnClickJoin);

            m_UIAward.SetListener(listener);
        }


        private void OnClickLeftItem(int index)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            mListener?.OnClickLeftItem(item.ConfigID);
        }

        private void OnClickRightItem(int index)
        {
            var item = mRightGroup.getAt(index);

            if (item == null)
                return;

            mListener?.OnClickRightItem(item.ConfigID);
        }

        private void OnFocusRightItem(bool b, uint configID)
        {
            mListener?.OnFocusRightItem(b, configID);
        }
        public interface IListener
        {
            void OnClickLeftItem(uint id);

            void OnClickRightItem(uint id);

            void OnClickClose();

            void OnCalendar();

            void OnBell();


            void OnClickActivity();

            void OnFocusRightItem(bool b, uint configID);

            void OnClickJoin();

            void OnClickGotoJoin(uint id);


            void OnClickRaward(uint id);
        }





        public void SetActivityTex(string tex)
        {
            //mTexActivity.text = tex;
        }


        public void SetBtnJoinActive(bool active)
        {
            if (mBtnJoin.gameObject.activeSelf != active)
                mBtnJoin.gameObject.SetActive(active);
        }

        public void SetRewartActive(bool active)
        {
           // mTransRewardHaveGet.gameObject.SetActive(active);
        }
    }

    public partial class UI_UI_DailyActivites_Layout
    {
        public void SetLeftContentSize(int size)
        {
            mLeftGroup.SetChildSize(size);
        }

        public void SetLeftContentActive(int index, bool bstate)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            if (!bstate)
                item.Hide();
            else
                item.Show();
        }

        public void SetLeftContentName(int index, uint langueID)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(LanguageHelper.GetTextContent(langueID));
        }

        public void SetLeftContentRed(int index, bool active)
        {

            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            item.SetRed(active);
        }
        public void SetLeftContentImage(int index, uint id)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            ImageHelper.SetIcon(item.mImIcon,id);
           
        }


        public void SetLeftContentRewardRed(int index, bool active)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            item.SetRewardRed(active);
        }
        public void SetLeftContentConfigID(int index,uint configID)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            item.ConfigID = configID;
            item.Index = index;
        }
        public void SetFoucsLeftContent(int index)
        {
            var item = mLeftGroup.getAt(index);

            if (item == null)
                return;

            item.Togg.SetSelected(true,true);
        }
    }

    public partial class UI_UI_DailyActivites_Layout
    {
        public void setRightContentSize(int size)
        {
            mRightGroup.SetChildSize(size);

            mTypeFocusTrans.gameObject.SetActive(size > 0);
        }

        public void SetRightContentName(int index, uint langue, uint desc)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            item.SetName(LanguageHelper.GetTextContent(langue));
            item.SetDesc(LanguageHelper.GetTextContent(desc));
        }

        public void SetRightContentConfigID(int index, uint configid)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;


            item.ConfigID = configid;

            item.SetItemName(configid.ToString());
        }
        public void SetRightContentIndex(int index)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;


            item.Index = index;
        }
        public void SetRightContentActivity(int index, uint cur, uint total)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;
            uint styleid = cur >= total ? 74u : 128u;

            var styledata = CSVWordStyle.Instance.GetConfData(styleid);

            item.SetActivity(cur , total, styledata);
        }

        public void SetRightContentMaskActive(int index, bool active)
        {
            var item = mRightGroup.getAt(index);

            if (item == null)
                return;
            item.SetMaskActive(active);
        }
        public void SetRightContentDifficultText(int index, string text)
        {
            var item = mRightGroup.getAt(index);

            if (item == null)
                return;

    
           item.m_TransDifficult.gameObject.SetActive(string.IsNullOrEmpty(text) == false);          

            item.m_TexDifficult.text = text;
        }
        public void SetRightContentTimes(int index, uint cur, uint total)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            if (cur > total)
                cur = total;

            string str = total == 0 ? LanguageHelper.GetTextContent(2010255) : (cur + "/" + total);

            item.SetTimes(str);
        }

        public void SetRightContentIcon(int index, uint iconID)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            item.SetIcon(iconID);
        }

        public void SetRightContentNewTips(int index, bool active)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            item.SetNewTips(active);
        }

        public void SetRightContentRewardRed(int index, bool active)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            item.SetRewardRed(active);
        }
        public int GetRightContentItemIndex(uint configID)
        {
            return mRightGroup.items.FindIndex(o => o.ConfigID == configID);
        }
        public void SetRightScrollGridActive(bool state)
        {
            mScrollGridEx.gameObject.SetActive(state);
        }

        public void SetFocusRightItem(uint id)
        {
            int index =  GetRightContentItemIndex(id);

            if (index < 0 || index >= mRightGroup.Count)
                return;

            mScrollGridEx.SetFocus(index);
        }

        public void SetRightMark(int index, UI_Daily_Common.DailyItem.EState eState,uint langueid)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            item.SetState(eState, langueid);
        }

        public void SetRightOpState(int index, UI_Daily_Common.DailyItem.EOpState eOpState)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            item.SetOpState(eOpState);
        }

        /// <summary>
        /// 设置限制文字
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        /// <param name="model"> 1 等级， 2 时间</param>
        /// <param name="value"></param>
        public void SetRightLimit(int index, int model,string value, CSVWordStyle.Data textStyle)
        {
            var item = mRightGroup.getAt(index);
            if (item == null)
                return;

            if (model == 1)
                item.SetLimitLevel(value,textStyle);

            if (model == 2)
                item.SetLimitTime(value,textStyle);
        }


        public void SetRightPlyerCoutType(int index, uint iconid)
        {
            var item = mRightGroup.getAt(index);

            if (item == null)
                return;

            item.SetPlayerType(iconid);
        }


        public void SetRightAmount(int index, string value)
        {
            var item = mRightGroup.getAt(index);

            if (item == null)
                return;

            item.SetAmount(value);
        }
    }


    public partial class UI_UI_DailyActivites_Layout
    {
        class UIAward
        {
            public class AwardItem : ClickItem
            {
                PropItem m_Item;

                PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

                public Transform m_CanGetTransform;

                public uint ID;

                public Action<uint> OnClickBtn;
                public override void Load(Transform root)
                {
                    base.Load(root);


                    m_Item = new PropItem();

                    m_Item.BindGameObject(root.Find("PropItem").gameObject);

                    m_CanGetTransform = root.Find("PropItem/Fx_ui_Select");



                }

                public void SetItem(uint id, uint count)
                {
                    m_ItemData.id = id;
                    m_ItemData.count = count;

                    m_Item.SetData(new MessageBoxEvt() { sourceUiId = EUIID.UI_DailyActivites, itemData = m_ItemData });
                }

                public void SetGanGet()
                {
                    m_ItemData.bUseTips = false;
                    m_ItemData.onclick = OnClickItem;
                    m_Item.SetGot(false);
                    m_CanGetTransform.gameObject.SetActive(true);
                }

                public void SetHadGet()
                {
                    m_ItemData.onclick = null;
                    m_ItemData.bUseTips = true;
                    m_Item.SetGot(true);
                    m_CanGetTransform.gameObject.SetActive(false);
                }

                public void SetNormal()
                {
                    m_ItemData.onclick = null;
                    m_ItemData.bUseTips = true;
                    m_Item.SetGot(false);
                    m_CanGetTransform.gameObject.SetActive(false);
                }


                private void OnClickItem(PropItem item)
                {
                    if (OnClickBtn != null)
                        OnClickBtn.Invoke(ID);
                }
            }

            public Slider m_SliProcess;

            public ClickItemGroup<AwardItem> AwardGroup = new ClickItemGroup<AwardItem>() { AutoClone = false };

            private IListener m_Listener;

            public Transform m_TransAward;
            public void Load(Transform root)
            {
                m_TransAward = root;

                var gridtrans = root.Find("Grid");

                var childcount = gridtrans.childCount;

                for (int i = 0; i < childcount; i++)
                {
                    string name = i == 0 ? "Item" : string.Format("Item ({0})", i.ToString());

                    var child = gridtrans.Find(name);

                    AwardItem childitem = new AwardItem();
                    childitem.Load(child);
                    childitem.OnClickBtn = OnClickAward;

                    AwardGroup.AddChild(childitem);
                }


                m_SliProcess = root.Find("Slider_Star").GetComponent<Slider>();


            }

            public void SetListener(IListener listener)
            {
                m_Listener = listener;
            }

            private void OnClickAward(uint id)
            {
                if (m_Listener != null)
                    m_Listener.OnClickRaward(id);
            }

        }
            public void SetAward(int index, uint bindid,uint id, uint count)
            {
                var item = m_UIAward.AwardGroup.getAt(index);

                if (item == null)
                    return;

                item.SetItem(id, count);
                item.ID = bindid;
            }

            public void SetAwardProcess(float process)
            {
                m_UIAward.m_SliProcess.value = process;
            }

        public void SetRewardState(int index, uint state)
        {
            var item = m_UIAward.AwardGroup.getAt(index);

            if (item == null)
                return;

            if (state == 1)
                item.SetGanGet();
            else if (state == 2)
                item.SetHadGet();
            else
                item.SetNormal();
        }


        public void SetRewardActive(bool active)
        {
            m_UIAward.m_TransAward.gameObject.SetActive(active);
        }
    }
}
