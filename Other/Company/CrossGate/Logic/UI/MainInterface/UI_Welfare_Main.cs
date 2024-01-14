using Packet;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Welfare_Item : UIComponent
    {
        public uint id;
        private PrivilegeBuff buffData;
        private CSVPrivilege.Data csvData;
        public Timer timer;

        private Image icon;
        private Text name;
        private Text timeLeft;
        private Text message;
        private Button delBtn;
        private float cutDownTime;

        protected override void Loaded()
        {
            icon = transform.Find("Image_BG/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            name = transform.Find("Image_BG/Text_Name").GetComponent<Text>();
            timeLeft = transform.Find("Image_BG/Text_Time").GetComponent<Text>();
            message = transform.Find("Image_BG/Text_Buff0").GetComponent<Text>();
            delBtn = transform.Find("Image_BG/Btn_Delete").GetComponent<Button>();
            delBtn.onClick.AddListener(OnDelBtnClicked);
        }

        public void Show(PrivilegeBuff _buffData)
        {
            buffData = _buffData;
            csvData = CSVPrivilege.Instance.GetConfData(id);
            if (csvData == null|| !csvData.Display)
            {
                gameObject.SetActive(false);
                return;
            }
            if (csvData.id == 5)
            {
               if(_buffData.Params.Count != 0)
                {
                    uint cardId = _buffData.Params[0];
                    CSVItem.Data csvItemData = CSVItem.Instance.GetConfData(cardId);
                    ImageHelper.SetIcon(icon, csvItemData.icon_id);
                    TextHelper.SetText(name,LanguageHelper.GetTextContent( csvItemData.name_id));
                    string str = null;
                    CSVRaceChangeCard.Data curCSVRaceChangeCardData = CSVRaceChangeCard.Instance.GetConfData(cardId);
                    if (curCSVRaceChangeCardData != null)
                    {
                        for (int i = 0; i < curCSVRaceChangeCardData.base_attr.Count; ++i)
                        {
                            uint id = (uint)curCSVRaceChangeCardData.base_attr[i][0];
                            int value = curCSVRaceChangeCardData.base_attr[i][1];
                            CSVAttr.Data csvAttrData = CSVAttr.Instance.GetConfData(id);
                            if (value > 0)
                            {
                                if (csvAttrData.show_type == 1)
                                {
                                    str += LanguageHelper.GetTextContent(csvAttrData.name) + "+" + value.ToString() + "；";
                                }
                                else
                                {
                                    str += LanguageHelper.GetTextContent(csvAttrData.name) + "+" + ((float)value / 100).ToString() + "%；";
                                }
                            }
                            else
                            {
                                if (csvAttrData.show_type == 1)
                                {
                                    str += LanguageHelper.GetTextContent(csvAttrData.name) + value.ToString() + "；";
                                }
                                else
                                {
                                    str += LanguageHelper.GetTextContent(csvAttrData.name) + ((float)value / 100).ToString() + "%；";

                                }
                            }
                        }
                        float addNum = 0;
                        if (Sys_Transfiguration.Instance.shapeShiftAddDic.ContainsKey(curCSVRaceChangeCardData.type))
                        {
                            addNum = Sys_Transfiguration.Instance.shapeShiftAddDic[curCSVRaceChangeCardData.type]/100.0f;
                        }
                        str += LanguageHelper.GetTextContent(2013023) + ": " +addNum .ToString("0.00")+"%；";
                    }         
                    TextHelper.SetText(message, str);
                    delBtn.gameObject.SetActive(true);
                }
            }
            else
            {
                ImageHelper.SetIcon(icon, csvData.Icon);
                TextHelper.SetText(name, csvData.Name);
                delBtn.gameObject.SetActive(false);

                if (id == 3)
                {
                    ResetHangupReward();
                }
                else
                {
                    message.text = LanguageHelper.GetTextContent(csvData.Des);
                }
            }
            timer?.Cancel();
            if (buffData.ExpireTime <= Sys_Time.Instance.GetServerTime())
            {
              Sys_Attr.Instance.DelPrivilegeBuffReq(id, false);
            }
            else
            {
                cutDownTime = buffData.ExpireTime - Sys_Time.Instance.GetServerTime();
                timer = Timer.Register(cutDownTime, null, ShowTime, false, true);
            }
        }

        private void ShowTime(float time)
        {
            timeLeft.text = LanguageHelper.GetTextContent(4089, LanguageHelper.TimeToString((uint)(cutDownTime - time), LanguageHelper.TimeFormat.Type_4));
        }

        public void ResetHangupReward()
        {
            uint.TryParse(CSVParam.Instance.GetConfData(1025).str_value, out uint maxTime);
            message.text = LanguageHelper.GetTextContent(csvData.Des, maxTime.ToString(), LanguageHelper.TimeToString(Sys_Hangup.Instance.monthOfflineReward.LastTime, LanguageHelper.TimeFormat.Type_4), Sys_Hangup.Instance.monthOfflineReward.Exp.ToString());
        }
        public UI_Welfare_Item(uint _id) : base()
        {
            id = _id;
        }

        private void OnDelBtnClicked()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = "确定要取消当前变身状态吗";
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Attr.Instance.DelPrivilegeBuffReq(buffData.Id, true);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
    }

    public class UI_Welfare_Main : UIBase
    {
        private GameObject itemGo;
        private Button closeBtn;
        private List<UI_Welfare_Item> items = new List<UI_Welfare_Item>();

        protected override void OnLoaded()
        {
            itemGo = transform.Find("Animator/Image/Scroll_View/Viewport/Content/Item").gameObject;
            closeBtn = transform.Find("Image_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnClick);
        }

        protected override void OnShow()
        {
            AddList();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
           Sys_Attr.Instance.eventEmitter.Handle<uint,uint>(Sys_Attr.EEvents.OnPrivilegeBuffUpdate, OnPrivilegeBuffUpdate,toRegister);
           Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnResetMonthOfflineReward, OnResetMonthOfflineReward, toRegister);
        }

        protected override void OnHide()
        {
            DefaultItem();
        }        

        private void AddList()
        {
            FrameworkTool.CreateChildList(itemGo.transform.parent.transform, Sys_Attr.Instance.privilegeBuffIdList.Count+1);
            for (int i = 0; i < Sys_Attr.Instance.privilegeBuffIdList.Count; i++)
            {
                uint id = Sys_Attr.Instance.privilegeBuffIdList[i];
                UI_Welfare_Item item = new UI_Welfare_Item(id);
                item.Init(itemGo.transform.parent.GetChild(i+1).transform);
                item.Show(Sys_Attr.Instance.privilegeBuffDic[id]);
                items.Add(item);
            }
            itemGo.SetActive(false);
        }

        private void DefaultItem()
        {
            for (int i = 0; i < items.Count; ++i)
            {
                items[i].timer?.Cancel();
                items[i].OnDestroy();
            }
            items.Clear();
            FrameworkTool.DestroyChildren(itemGo.transform.parent.gameObject, itemGo.transform.name);
            itemGo.SetActive(true);
        }

        private void OnPrivilegeBuffUpdate(uint id, uint op)
        {
            if (Sys_Attr.Instance.privilegeBuffIdList.Count == 0)
            {
                UIManager.CloseUI(EUIID.UI_Welfare_Main);
                return;
            }
            if (op == 0)
            {
                GameObject go = GameObject.Instantiate<GameObject>(itemGo, itemGo.transform.parent);
                go.SetActive(true);
                UI_Welfare_Item item = new UI_Welfare_Item(id);
                item.Init(go.transform);
                item.Show(Sys_Attr.Instance.privilegeBuffDic[id]);
                items.Add(item);
            }
            else if (op == 1)
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    if (items[i].id == id)
                    {
                        items[i].timer?.Cancel();
                        if (items[i].transform.name == itemGo.transform.name)
                        {
                            items[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            items[i].gameObject.DestoryAllChildren();
                        }
                        items.Remove(items[i]);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    if (items[i].id == id)
                    {
                        items[i].Show(Sys_Attr.Instance.privilegeBuffDic[id]);
                    }
                }
            }
        }

        private void OnResetMonthOfflineReward()
        {
            for(int i=0;i< items.Count; ++i)
            {
                if (items[i].id == 3)
                {
                    items[i].ResetHangupReward();
                }
            }
        }

        private void OnCloseBtnClick()
        {
            UIManager.CloseUI(EUIID.UI_Welfare_Main);
        }
    }
}
