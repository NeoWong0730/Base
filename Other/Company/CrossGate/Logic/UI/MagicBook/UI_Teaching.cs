using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using Lib.Core;
using Packet;
using DG.Tweening;

namespace Logic
{ 
    public class UI_Teaching_Item
    {
        private Transform transform; 
        private Image icon;
        private Text name;
        private Text describe;
        private Button Btn;
        private Image Btn_Image;
        private GameObject redPoint;

        private CSVTeaching.Data cSVTeachingData;
        public uint id;

        public void Init(GameObject gameObject)
        {
            transform = gameObject.transform;
            icon = transform.Find("List_Big/Image/Image_Icon").GetComponent<Image>();
            name = transform.Find("List_Big/Text_Name").GetComponent<Text>();
            describe = transform.Find("List_Big/Text_Describe").GetComponent<Text>();
            Btn = transform.Find("List_Big/Btn_Study").GetComponent<Button>();
            Btn_Image = Btn.GetComponent<Image>();
            redPoint = transform.Find("List_Big/Image_Dot").gameObject;
            Btn.onClick.AddListener(OnBtnClicked);
        }

        public void SetData(uint _id)
        {
            id = _id;
            CSVTeaching.Instance.TryGetValue(id, out cSVTeachingData);
            ImageHelper.SetIcon(icon, cSVTeachingData.icon);
            name.text = LanguageHelper.GetTextContent(cSVTeachingData.name);
            describe.text = LanguageHelper.GetTextContent(cSVTeachingData.desc);
            RefressData();
        }

        public void RefressData()
        {
            bool hasProcess = false;
            for (int i = 0; i < Sys_MagicBook.Instance.teachList.Count; ++i)
            {
                if (Sys_MagicBook.Instance.teachList[i].Id == id && Sys_MagicBook.Instance.teachList[i].Process == 1)
                {
                    hasProcess = true;
                    break;
                }
            }
            redPoint.SetActive(!hasProcess);
            ImageHelper.SetImageGray(Btn_Image, hasProcess, true);
        }


        private void OnBtnClicked()
        {
            if (Sys_Hint.Instance.PushForbidOprationInFight())
            {
                return;
            }
            if (Sys_Team.Instance.HaveTeam)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(900000099));
                return;
            }
            Sys_MagicBook.Instance.ClickTeachingReq(id);
            UIManager.CloseUI(EUIID.UI_MagicBook);
        }

        public void RemoveEvents()
        {
            if (null != Btn)
            {
                Btn.onClick.RemoveListener(OnBtnClicked);
            }
        }
    }

    public class UI_Teaching
    {
        private Transform transform;
        private Transform teachItemParent;
        private Transform teachItemTrans;
        private Transform strategyItemParent;
        private Transform strategyItemTrans;
        private Text num;
        private Slider slider;
        private Button btn;
        private GameObject redGo;
        private GameObject strategyItemView;
        private GameObject teachItemView;
        private GameObject topView;

        private RectTransform strategyItemBigTrans;
        private RectTransform strategyItemSmallTrans;
        private RectTransform strategyItemParentRect;
        private VerticalLayoutGroup strategyItemParentVG;
        private ScrollRect strategyItemParentScroll;

        private CSVCareerTeaching.Data cSVCareerTeachingData;
        private List<uint> IdList = new List<uint>();
        private List<UI_Teaching_Item> itemTeachList = new List<UI_Teaching_Item>();
        private List<UI_Strategy_Item> itemStrategyList = new List<UI_Strategy_Item>();

        private uint currentChapterId;
        private uint currentTeachId;
        private bool canReward;
        private bool isTeaching;
        private bool isFaq;

        private int curSelectedIndex;
        private int lastSelectedIndex;

        public void Init(GameObject gameObject)
        {
            transform = gameObject.transform;
            teachItemParent = transform.Find("ListScroll/Viewport").transform;
            teachItemTrans = transform.Find("ListScroll/Viewport/Item").transform;
            strategyItemParent = transform.Find("ListScroll_01/Viewport").transform;
            strategyItemTrans = transform.Find("ListScroll_01/Viewport/Item").transform;
            num = transform.Find("Title/Slider_Exp/Text_Percent").GetComponent<Text>();
            slider = transform.Find("Title/Slider_Exp").GetComponent<Slider>();
            redGo = transform.Find("Title/RawImage").gameObject;
            teachItemView = transform.Find("ListScroll").gameObject;
            strategyItemView = transform.Find("ListScroll_01").gameObject;
            topView = transform.Find("Title").gameObject;

            strategyItemParentRect = transform.Find("ListScroll_01/Viewport").GetComponent<RectTransform>();
            strategyItemBigTrans = transform.Find("ListScroll_01/Viewport/Item/List_Big").GetComponent<RectTransform>();
            strategyItemSmallTrans = transform.Find("ListScroll_01/Viewport/Item/SmallItem/List_Small").GetComponent<RectTransform>();
            strategyItemParentVG = strategyItemParent.GetComponent<VerticalLayoutGroup>();
            strategyItemParentScroll = transform.Find("ListScroll_01").GetComponent<ScrollRect>();
            btn = transform.Find("Title/Btn_Aware").GetComponent<Button>();
            btn.onClick.AddListener(OnBtnClicked);
        }

        public void RefreshChapter(uint id)
        {
            currentChapterId = id;
            isTeaching = currentChapterId == 201;
            isFaq = currentChapterId == 207;

            IdList.Clear();
            curSelectedIndex = 0;
            lastSelectedIndex = 0;
            CSVCareerTeaching.Instance.TryGetValue(Sys_Role.Instance.Role.Career, out cSVCareerTeachingData);
            if (cSVCareerTeachingData == null)
            {
                teachItemView.gameObject.SetActive(false);
               strategyItemView.gameObject.SetActive(false);

                return;
            }
            teachItemView.SetActive(isTeaching);
            strategyItemView.SetActive(!isTeaching);
            IdList = Sys_MagicBook.Instance.CheckTeachChapterInfo(currentChapterId, cSVCareerTeachingData);
            ReviewChapters();
            RefreshChapterTop();
        }

        private void ReviewChapters()
        {
            for (int i = 0; i < itemTeachList.Count; ++i)
            {
                itemTeachList[i].RemoveEvents();
            }
            for (int i = 0; i < itemStrategyList.Count; ++i)
            {
                itemStrategyList[i].RemoveEvents();
            }
            itemTeachList.Clear();
            itemStrategyList.Clear();

            FrameworkTool.DestroyChildren(teachItemParent.gameObject, teachItemTrans.name);
            FrameworkTool.DestroyChildren(strategyItemParent.gameObject, strategyItemTrans.name);
            IdList.Sort();
            int count = IdList.Count;
            if (isTeaching)
            {
                FrameworkTool.CreateChildList(teachItemParent, count);
                for (int i = 0; i < count; i++)
                {
                    Transform tran = teachItemParent.GetChild(i);
                    UI_Teaching_Item item = new UI_Teaching_Item();
                    item.Init(tran.gameObject);
                    item.SetData(IdList[i]);
                    itemTeachList.Add(item);
                }
                FrameworkTool.ForceRebuildLayout(teachItemParent.gameObject);
            }
            else
            {
                FrameworkTool.CreateChildList(strategyItemParent, count);
                for (int i = 0; i < count; i++)
                {
                    Transform tran = strategyItemParent.GetChild(i);
                    UI_Strategy_Item item = new UI_Strategy_Item();
                    item.Init(tran.gameObject);
                    item.AddClickListener(OnItemSelect);
                    item.SetData(IdList[i], isFaq);
                    itemStrategyList.Add(item);
                }
                FrameworkTool.ForceRebuildLayout(strategyItemParent.gameObject);
            }
        }

        private void OnItemSelect(UI_Strategy_Item item)
        {
            curSelectedIndex = 0;
            for (int i = 0; i < itemStrategyList.Count; ++i)
            {
                if (itemStrategyList[i].id == item.id && !itemStrategyList[i].messageGo.activeInHierarchy)
                {
                    itemStrategyList[i].SetMessageShow(true);
                    curSelectedIndex = i;
                }
                else
                {
                    itemStrategyList[i].SetMessageShow(false);
                }
            }
            SetPosView(curSelectedIndex);
            lastSelectedIndex = curSelectedIndex;
            FrameworkTool.ForceRebuildLayout(strategyItemParent.gameObject);
            if (!isFaq)
            {
                Sys_MagicBook.Instance.ClickTeachingReq(item.id);
            }
        }

        public void SetPosView(int select)
        {
            if (select == 0)
            {
                return;
            }
            float spaY = strategyItemParentVG.spacing;
            float itemBigY = strategyItemBigTrans.sizeDelta.y;
            float itemSmallY = strategyItemSmallTrans.sizeDelta.y;

            float itemY = select * itemBigY + select * spaY;
            strategyItemParentScroll.StopMovement();
            if (select == itemStrategyList.Count - 1 && lastSelectedIndex == 0)
            {
                strategyItemParentScroll.content.DOLocalMoveY(strategyItemParentRect.sizeDelta.y + itemSmallY, 0.3f);
            }
            else
            {
                strategyItemParentScroll.content.DOLocalMoveY(Mathf.Min(strategyItemParentRect.sizeDelta.y < 0 ? 0 : strategyItemParentRect.sizeDelta.y, itemY), 0.3f);
            }
        }


        public void RefreshChapterTop()
        {
            topView.SetActive(!isFaq);
            if (isFaq)
            {
                return;
            }
            slider.maxValue = IdList.Count + 0.0f;
            Chapter server_Chapter = Sys_MagicBook.Instance.GetSeverChapterByChapterId(currentChapterId);

            float value = 0;
            if (null != server_Chapter)
            {
                value = server_Chapter.Process;
            }
            slider.value = value;
            canReward = Sys_MagicBook.Instance.CheckChapterReward(currentChapterId);
            redGo.SetActive(canReward);
            TextHelper.SetText(num, string.Format("{0}/{1}", value.ToString(), IdList.Count.ToString()));
        }

        public void RefreshCeilProcess()
        {
            if (isTeaching)
            {
                for (int i = 0; i < itemTeachList.Count; i++)
                {
                    itemTeachList[i].RefressData();
                }
            }
            else
            {
                for (int i = 0; i < itemStrategyList.Count; i++)
                {
                    itemStrategyList[i].RefressData();
                }
            }
        }

        private void OnBtnClicked()
        {
            if (!canReward)
            {       
                UIManager.OpenUI(EUIID.UI_MagicBook_Tips, false, currentChapterId);
            }
            else
            {
                Sys_MagicBook.Instance.MagicDictGetChapterAwardReq(currentChapterId);
            }
        } 
    }
}
