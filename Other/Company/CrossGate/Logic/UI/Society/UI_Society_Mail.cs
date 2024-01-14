using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using Table;

namespace Logic
{
    public partial class UI_Society : UIBase, UI_Society_Layout.IListener
    {
        private ScrollGridVertical scrollGridVertical;
        private int curSelectIndex = -1;
        private Dictionary<GameObject, GridWarp> grids = new Dictionary<GameObject, GridWarp>();

        private GameObject mailRedPoint;
        private Text title;
        private Text content;
        private Text day;
        private Button deleteButton;          //删除
        private Button getButton;             //领取
        private Button allGetButton;          //一键领取
        private Button deleteAllReadButton;  //删除已读
        private Transform itemParent;
        private Transform giftRoot;

        private List<Button> buttons = new List<Button>();

        public void OnClickMailToggle()
        {
            layout.recentToggle_Light.SetActive(false);
            layout.recentToggle_Dark.SetActive(true);

            layout.groupToggle_Light.SetActive(false);
            layout.groupToggle_Dark.SetActive(true);

            layout.contactsToggle_Light.SetActive(false);
            layout.contactsToggle_Dark.SetActive(true);

            layout.mailToggle_Light.SetActive(true);
            layout.mailToggle_Dark.SetActive(false);

            layout.recentRoot.SetActive(false);
            layout.groupRoot.SetActive(false);
            layout.contactsRoot.SetActive(false);
            layout.mailRoot.SetActive(true);
            layout.buttonRoot.SetActive(false);

            layout.recentRightRoot.SetActive(false);
            layout.groupRightRoot.SetActive(false);
            layout.contactsRightRoot.SetActive(false);
            layout.mailRightRoot.SetActive(true);
            layout.inputRoot.SetActive(false);

            Sys_Mail.Instance.bMailShowing = true;

            Sys_Mail.Instance.CalExpirmeMail();
            Sys_Mail.Instance.SortMail();
            RefreshLeftView();
            Sys_Mail.Instance.eventEmitter.Trigger(Sys_Mail.EEvents.OnNoticeMailOver);

        }

        private void MailInit()
        {
            mailRedPoint = layout.mailToggle.transform.Find("UI_RedTips_Small").gameObject;
            title = layout.mailRightRoot.transform.Find("Image_Title/Text").GetComponent<Text>();
            content = layout.mailRightRoot.transform.Find("Image_Content/Text_Content").GetComponent<Text>();
            day = layout.mailRightRoot.transform.Find("Text_Day").GetComponent<Text>();
            deleteButton = layout.mailRightRoot.transform.Find("Btn_02").GetComponent<Button>();
            getButton = layout.mailRightRoot.transform.Find("Image_BG/Button_Get").GetComponent<Button>();
            allGetButton = layout.mailRoot.transform.Find("Button_GetAll").GetComponent<Button>();
            deleteAllReadButton = layout.mailRoot.transform.Find("Button_ReadAll").GetComponent<Button>();
            itemParent = layout.mailRightRoot.transform.Find("Image_BG/Image_Gift/Grid");
            giftRoot = layout.mailRightRoot.transform.Find("Image_BG");
            scrollGridVertical = layout.mailRoot.GetComponent<ScrollGridVertical>();
            scrollGridVertical.AddCellListener(OnCellUpdateCallback);
            scrollGridVertical.AddCreateCellListener(OnCreateCellCallback);

            deleteButton.onClick.AddListener(DeleteData);
            getButton.onClick.AddListener(OnGetMailAttachClicked);
            allGetButton.onClick.AddListener(OnGetAllMailAttachClicked);
            deleteAllReadButton.onClick.AddListener(OnDeleteAllReadClicked);
        }

        private void RemoveAllListeners()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.RemoveAllListeners();
            }
            buttons.Clear();
        }

        private void RefreshLeftView()
        {
            scrollGridVertical.SetCellCount(Sys_Mail.Instance.mailInfo.mailDatas.Count);
            TextHelper.SetText(layout.mailLimitText, CSVLanguage.Instance.GetConfData(15130), Sys_Mail.Instance.mailInfo.mailDatas.Count.ToString());
            if (Sys_Mail.Instance.mailInfo.mailDatas.Count == 0)
            {
                curSelectIndex = -1;
                RefreshRightView(true, 0);
                return;
            }
            if (curSelectIndex == -1)
            {
                curSelectIndex = 0;
            }
            //scrollGridVertical.FixedPosition(0);
            SetSelect(curSelectIndex);
        }

        private void RefreshRightView(bool empty, ulong mailId)
        {
            if (empty)
            {
                layout.rightNoneRoot.SetActive(true);
                layout.mailRightRoot.SetActive(false);
                layout.rightMOLIRoot.SetActive(false);
                TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(10652));
            }
            else
            {
                RemoveAllListeners();

                layout.rightNoneRoot.SetActive(false);
                layout.mailRightRoot.SetActive(true);
                layout.rightMOLIRoot.SetActive(false);
                Sys_Mail.MailData mailData = Sys_Mail.Instance.GetMailData(mailId);

                if (mailData == null)
                {
                    DebugUtil.LogErrorFormat("未能找到邮件{0}", mailId);
                }
                else
                {
                    title.text = mailData.title;
                    content.text = mailData.content;
                    DateTime dateTime = Sys_Time.ConvertToLocalTime(mailData.time);
                    day.text = string.Format(CSVLanguage.Instance.GetConfData(2004113).words, dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute);
                    if (!mailData.Attach)
                    {
                        deleteButton.gameObject.SetActive(true);
                        getButton.gameObject.SetActive(false);
                        giftRoot.gameObject.SetActive(false);
                    }
                    else
                    {
                        giftRoot.gameObject.SetActive(true);
                        deleteButton.gameObject.SetActive(mailData.get);
                        getButton.gameObject.SetActive(!mailData.get);

                        int needCount = mailData.attach.Count;
                        FrameworkTool.CreateChildList(itemParent, needCount);

                        for (int i = 0; i < needCount; i++)
                        {
                            Transform obj = itemParent.GetChild(i);
                            Button button = obj.Find("Btn_Item").GetComponent<Button>();
                            Image image = obj.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                            Image imageSkill = obj.Find("Btn_Item/Image_Skill").GetComponent<Image>();
                            Image mBg = obj.Find("Btn_Item/Image_BG").GetComponent<Image>();
                            Text text = obj.Find("Text_Number").GetComponent<Text>();
                            GameObject bound = obj.Find("Text_Bound").gameObject;
                            GameObject clock = obj.Find("Image_Clock").gameObject;
                            GameObject get = obj.Find("Btn_Get").gameObject;

                            uint itemId = mailData.attach[i].ItemId;
                            uint itemCount = mailData.attach[i].ItemNum;

                            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                            ImageHelper.SetIcon(image, cSVItemData.icon_id);
                            ImageHelper.GetQualityColor_Frame(mBg, (int)cSVItemData.quality);
                            Sys_Skill.Instance.ShowPetSkillBook(imageSkill, cSVItemData);
                            text.gameObject.SetActive(true);
                            text.text = string.Format($"x{itemCount}");

                            bool bind = false;
                            uint TemplateId = mailData.attach[i].TemplateId;
                            if (TemplateId != 0)
                            {
                                if (cSVItemData.type_id == 1000)  //装备
                                {
                                    CSVEquipmentParameter.Data cSVEquipmentParameterData = CSVEquipmentParameter.Instance.GetConfData(TemplateId);
                                    bind = cSVEquipmentParameterData.is_band == 1;
                                }
                                else if (cSVItemData.type_id == 1103)//宠物
                                {
                                    CSVPetNewTemplate.Data cSVPetStencilData = CSVPetNewTemplate.Instance.GetConfData(TemplateId);
                                    bind = cSVPetStencilData.is_band;
                                }
                            }
                            else //普通
                            {
                                bind = false;
                            }

                            bound.SetActive(bind);
                            if (bind)
                            {
                                clock.SetActive(false);
                            }
                            else
                            {
                                clock.SetActive(mailData.attach[i].prohibitionSec != 0);
                            }
                            get.SetActive(mailData.get);

                            int preSec = mailData.attach[i].prohibitionSec;
                            if (preSec > 0)
                            {
                                preSec += (int)Sys_Time.Instance.GetServerTime() / 86400;
                            }
                            ItemData itemData = new ItemData((int)cSVItemData.box_id, 0, itemId, itemCount, 0, false,
                            bind, null, null, preSec);
                            
                            button.onClick.AddListener(() =>
                            {
                                uint typeId = itemData.cSVItemData.type_id;
                                if (typeId == (uint)EItemType.Equipment)
                                {
                                    itemData.EquipParam = TemplateId;
                                    UIManager.OpenUI(EUIID.UI_Equipment_Preview, false, itemData);
                                }
                                else if (typeId == (uint)EItemType.Crystal)
                                {
                                    CrystalTipsData crystalTipsData = new CrystalTipsData();
                                    crystalTipsData.itemData = itemData;
                                    crystalTipsData.bShowOp = false;
                                    crystalTipsData.bShowCompare = false;
                                    crystalTipsData.bShowDrop = false;
                                    crystalTipsData.bShowSale = false;
                                    UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
                                }
                                else if (typeId == (uint)EItemType.Ornament)
                                {
                                    OrnamentTipsData tipData = new OrnamentTipsData();
                                    tipData.equip = itemData;
                                    tipData.isShowOpBtn = false;
                                    tipData.isCompare = false;
                                    tipData.isShowLock = false;
                                    tipData.sourceUiId = EUIID.UI_Society;
                                    UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                                }
                                else
                                {
                                    PropMessageParam propParam = new PropMessageParam();
                                    propParam.itemData = itemData;
                                    propParam.needShowInfo = false;
                                    propParam.needShowMarket = true;
                                    propParam.sourceUiId = EUIID.UI_Society;
                                    UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                                }
                            });
                            buttons.Add(button);
                        }
                    }
                }
            }
        }

        private void OnGetAttach(ulong _id)
        {
            Dictionary<GameObject, GridWarp>.Enumerator enumerator = grids.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GridWarp gridWarp = enumerator.Current.Value;
                if (gridWarp.id == _id)
                {
                    gridWarp.Refresh();
                }
                if (gridWarp.dataIndex == curSelectIndex)
                {
                    RefreshRightView(false, _id);
                }
            }
            RefreshMailRedPoint();
        }

        private void OnAddMail()
        {
            OnAddOrDeleteMail();
        }

        private void OnDeleteMail()
        {
            OnAddOrDeleteMail();
        }

        private void OnAddOrDeleteMail()
        {
            TextHelper.SetText(layout.mailLimitText, CSVLanguage.Instance.GetConfData(15130), Sys_Mail.Instance.mailInfo.mailDatas.Count.ToString());
            RefreshMailRedPoint();
            if (!Sys_Mail.Instance.bMailShowing)
            {
                return;
            }
            scrollGridVertical.SetCellCount(Sys_Mail.Instance.mailInfo.mailDatas.Count);
            AutoRefreshSelect();
        }

        private void RefreshMailRedPoint()
        {
            mailRedPoint.SetActive(!Sys_Mail.Instance.ReadAllMail() || Sys_Mail.Instance.CanGetAttach());
        }

        private void OnCellUpdateCallback(ScrollGridCell cell)
        {
            GridWarp gridWarp = grids[cell.gameObject];
            gridWarp.SetData(Sys_Mail.Instance.mailInfo.mailDatas[cell.index].id, cell.index);
            if (curSelectIndex == cell.index)
            {
                gridWarp.Select();
            }
            else
            {
                gridWarp.Release();
            }
        }

        private void OnCreateCellCallback(ScrollGridCell cell)
        {
            GridWarp gridWarp = new GridWarp();
            gridWarp.BindGameObject(cell.gameObject);
            gridWarp.AddEvent(OnItemSelected);
            grids[cell.gameObject] = gridWarp;
        }

        private void OnItemSelected(GridWarp gridWarp)
        {
            if (curSelectIndex != gridWarp.dataIndex)
            {
                SetSelect(gridWarp.dataIndex);
                //curSelectIndex = gridWarp.dataIndex;
                //RefreshRightView(false, gridWarp.id);
                //Debug.Log(gridWarp.id);
            }
        }

        private void SetSelect(int _dataIndex)
        {
            curSelectIndex = _dataIndex;
            Dictionary<GameObject, GridWarp>.Enumerator enumerator = grids.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GridWarp gridWarp = enumerator.Current.Value;
                if (gridWarp.dataIndex == _dataIndex)
                {
                    Sys_Mail.Instance.ReadMail(gridWarp.id);
                    RefreshMailRedPoint();
                    gridWarp.Select();
                    RefreshRightView(false, gridWarp.id);
                }
                else
                {
                    gridWarp.Release();
                }
            }
        }

        private void DeleteData()
        {
            if (Sys_Mail.Instance.mailInfo.mailDatas.Count == 0)
                return;

            Sys_Mail.Instance.DeleteMailReq(Sys_Mail.Instance.mailInfo.mailDatas[curSelectIndex].id);

            //Sys_Mail.Instance.DeleteMail(Sys_Mail.Instance.mailInfo.mailDatas[curSelectIndex]);

            //if (Sys_Mail.Instance.mailInfo.mailDatas.Remove(Sys_Mail.Instance.mailInfo.mailDatas[curSelectIndex]))
            //{
            //    Sys_Mail.Instance.SaveMemory();
            //    scrollGridVertical.SetCellCount(Sys_Mail.Instance.mailInfo.mailDatas.Count);
            //}
            //AutoRefreshSelect();
        }

        private void OnGetMailAttachClicked()
        {
            //Sys_Mail.Instance.GetMailAttach(Sys_Mail.Instance.mailInfo.mailDatas[curSelectIndex].id);
            Sys_Mail.Instance.GetMailAttachReq(Sys_Mail.Instance.mailInfo.mailDatas[curSelectIndex].id);
        }

        //一键领取 
        //1原先的一键领取和一键已读是2个按钮执行的。当前合并到一个按钮逻辑里（一键领取）内。
        private void OnGetAllMailAttachClicked()
        {
            if (!Sys_Mail.Instance.CanGetAttach())
            {
                string content = CSVLanguage.Instance.GetConfData(10196).words;
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }
            Sys_Mail.Instance.GetMulMailAttachReq();
        }

        //删除已读 
        //点击后，未读邮件不会被删除。
        //点击后，未读有附件邮件不会被删除。
        //点击后，已读，有附件邮件不会被删除。
        //点击后，已读，没有附件的邮件将被删除。
        private void OnDeleteAllReadClicked()
        {
            Sys_Mail.Instance.DeleteAllReadAndNoAttachMail();
        }


        private void ReadAllMail()
        {
            Sys_Mail.Instance.ReadAll();
            RefreshMailRedPoint();
            foreach (var item in grids)
            {
                item.Value.UpdateMailReadState();
            }
        }

        private void AutoRefreshSelect()
        {
            if (Sys_Mail.Instance.mailInfo.mailDatas.Count == 0)
            {
                curSelectIndex = -1;
                RefreshRightView(true, 0);
                return;
            }
            if (curSelectIndex == -1)
            {
                curSelectIndex = 0;
            }
            if (curSelectIndex > Sys_Mail.Instance.mailInfo.mailDatas.Count - 1)
                SetSelect(Sys_Mail.Instance.mailInfo.mailDatas.Count - 1);
            else
                SetSelect(curSelectIndex);
        }


        public class GridWarp
        {
            public int dataIndex;
            public ulong id;
            public Sys_Mail.MailData mailData;
            public GameObject gameObject;
            private Image eventBg;
            private Text time;
            private Text title;
            private Text content;
            private Action<GridWarp> selectAction;
            private GameObject select;
            private GameObject imageGift;
            private GameObject mailread;
            private GameObject mailUnread;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                mailread = gameObject.transform.Find("Mail_Image/Image_MailOpen").gameObject;
                mailUnread = gameObject.transform.Find("Mail_Image/Image_Mail").gameObject;
                imageGift = gameObject.transform.Find("Image_Gift").gameObject;
                eventBg = gameObject.transform.Find("Image").GetComponent<Image>();
                select = gameObject.transform.Find("Image_Selected").gameObject;
                time = gameObject.transform.Find("Text_Number").GetComponent<Text>();
                title = gameObject.transform.Find("Text_Head").GetComponent<Text>();
                // title = gameObject.transform.Find("Text_Head").GetComponent<Text>();
                //title = gameObject.transform.Find("Text_Name").GetComponent<Text>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg.gameObject);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void SetData(ulong _id, int _dataIndex)
            {
                id = _id;
                mailData = Sys_Mail.Instance.GetMailData(_id);
                dataIndex = _dataIndex;
                Refresh();
            }

            public void UpdateMailReadState()
            {
                mailread.SetActive(mailData.read);
                mailUnread.SetActive(!mailData.read);
            }

            public void Refresh()
            {
                gameObject.name = id.ToString();
                time.text = id.ToString();
                title.text = mailData.title;
                mailread.SetActive(mailData.read);
                mailUnread.SetActive(!mailData.read);
                imageGift.SetActive(mailData.Attach && !mailData.get);
                UpdateTime();
            }

            public void UpdateTime()
            {
                if (!gameObject.activeSelf)
                    return;
                long _time = Sys_Mail.Instance.vaildtime - ((long)Sys_Time.Instance.GetServerTime() - (long)mailData.time) / 86400;
                if (_time < 0)
                {
                    _time = 0;
                }
                time.text = string.Format(CSVLanguage.Instance.GetConfData(2004112).words, _time);
            }

            public void AddEvent(Action<GridWarp> action)
            {
                selectAction = action;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                selectAction.Invoke(this);
            }

            public void Release()
            {
                select.SetActive(false);
            }

            public void Select()
            {
                select.SetActive(true);
                Refresh();
            }
        }
    }
}
