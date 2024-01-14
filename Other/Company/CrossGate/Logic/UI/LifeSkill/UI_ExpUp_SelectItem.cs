using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


namespace Logic
{
    public class UI_ExpUp_SelectItem : UIBase
    {
        private uint skillId;
        private Transform parent;
        private Image closeClick;
        private List<uint> items = new List<uint>();
        private List<UpSelectGrid> upSelectGrids = new List<UpSelectGrid>();
        private float triggerLongPress = 1f;
        private float normalinterval = 0.25f;
        private float mininterval = 0.18f;
        private float accelerate = 0.02f;

        protected override void OnOpen(object arg)
        {
            skillId = (uint)arg;
        }

        protected override void OnLoaded()
        {
            parent = transform.Find("Animator/Scroll_View/Grid");
            closeClick = transform.Find("Animator/ClickClose").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(closeClick);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { Close(); });
        }

        protected override void OnShow()
        {
            InitData();
        }

        private void InitData()
        {
            upSelectGrids.Clear();
            items = CSVLifeSkill.Instance.GetConfData(skillId).add_proficiency_item;
            int needCount = items.Count;
            FrameworkTool.CreateChildList(parent, needCount);
            for (int i = 0; i < parent.childCount; i++)
            {
                UpSelectGrid upSelectGrid = new UpSelectGrid(triggerLongPress, normalinterval, mininterval, accelerate);
                upSelectGrid.BindGameObject(parent.GetChild(i).gameObject);
                upSelectGrid.SetData(items[i]);
                upSelectGrids.Add(upSelectGrid);
            }
        }

        private void Close()
        {
            UIManager.CloseUI(EUIID.UI_ExpUp_SelectItem);
        }
        protected override void OnHide()
        {
            foreach (var item in upSelectGrids)
            {
                item.Dispose();
            }
        }

        public class UpSelectGrid
        {
            private Transform transform;
            private Image mEventBg;
            private Image mIcon;
            private Image mQuality;
            private Button mIconButton;
            private Text mItemCount;
            private Text mAddExpCount;
            private Text mitemName;
            private GameObject mSelectObj;
            private UI_LongPressButton uI_LongPressButton;
            private uint itemId;
            private uint skillId;
            private LivingSkill livingSkill;
            private CSVItem.Data cSVItemData;
            private int count;              //当次长按向服务器请求的个数
            private int remainCount;        //剩余个数
            private int curCount;
            private int debugCount;

            private float triggerLongPress = 1f;
            private float normalinterval = 0.4f;
            private float mininterval = 0.25f;
            private float accelerate = 0.02f;
            private float interval = 0.3f;
            private float nextRecordTime = 0.3f;
            private string ExpFullcontent;
            private string itemNotEnough;
            private string ExpGet;

            public UpSelectGrid(float _triggerLongPress, float _normalinterval, float _mininterval, float _accelerate)
            {
                this.triggerLongPress = _triggerLongPress;
                this.normalinterval = _normalinterval;
                this.mininterval = _mininterval;
                this.accelerate = _accelerate;
            }

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                mEventBg = transform.Find("EventBG").GetComponent<Image>();
                mIcon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                mQuality = transform.Find("PropItem/Btn_Item/Image_BG").GetComponent<Image>();
                mIconButton = transform.Find("PropItem/Btn_Item").GetComponent<Button>();
                mItemCount = transform.Find("PropItem/Text_Number").GetComponent<Text>();
                mitemName = transform.Find("Text_Name").GetComponent<Text>();
                mAddExpCount = transform.Find("Text").GetComponent<Text>();
                mSelectObj = transform.Find("Image_Select").gameObject;
                uI_LongPressButton = mEventBg.gameObject.AddComponent<UI_LongPressButton>();
                uI_LongPressButton.onLongPress.AddListener(OnLongPressed);
                uI_LongPressButton.onClickDown.AddListener(onClickDown);
                uI_LongPressButton.onRelease.AddListener(OnRelease);
                uI_LongPressButton.interval = 1;
                ExpFullcontent = CSVLanguage.Instance.GetConfData(2010110).words;
                ExpGet = CSVLanguage.Instance.GetConfData(2010109).words;
                itemNotEnough = CSVLanguage.Instance.GetConfData(2010128).words;

                mIconButton.onClick.AddListener(OnIconButtonClicked);
            }

            private void OnIconButtonClicked()
            {
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemId, 0, false, false, false, false, false, false, true);
                itemData.bShowBtnNo = false;
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message, itemData));
            }

            public void SetData(uint id)
            {
                this.itemId = id;
                cSVItemData = CSVItem.Instance.GetConfData(itemId);
                skillId = cSVItemData.fun_value[1];
                livingSkill = Sys_LivingSkill.Instance.livingSkills[skillId];
                RefreshIcon();
            }


            private void RefreshIcon()
            {
                mItemCount.gameObject.SetActive(true);
                mIcon.gameObject.SetActive(true);

                ImageHelper.SetIcon(mIcon, cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(mQuality, (int)cSVItemData.quality);
                TextHelper.SetText(mitemName, cSVItemData.name_id);
                if (cSVItemData.fun_parameter == "addproficiency")
                {
                    TextHelper.SetText(mAddExpCount, string.Format(CSVLanguage.Instance.GetConfData(2010130).words, cSVItemData.fun_value[2].ToString()));
                    //mAddExpCount.text = cSVItemData.fun_value[2].ToString();
                }
                else
                {
                    DebugUtil.LogErrorFormat($"道具{itemId} 不是增加熟练度道具");
                }
                curCount = (int)Sys_Bag.Instance.GetItemCount(itemId);
                remainCount = curCount;
                SetCount();
            }

            private void SetCount()
            {
                uint styleId = remainCount > 0 ? 83u : 84u;
                CSVWordStyle.Data cSVWordStyleData = CSVWordStyle.Instance.GetConfData(styleId);
                if (cSVWordStyleData != null)
                {
                    TextHelper.SetText(mItemCount, remainCount.ToString(), cSVWordStyleData);
                }
            }

            private void AddExp(uint exp)
            {
                livingSkill.AddProficiency(exp);
            }

            private void onClickDown()
            {
                count = 0;
                interval = normalinterval;
                nextRecordTime = normalinterval;
                debugCount = 0;
            }

            private void OnLongPressed(float dt)
            {
                if (dt - nextRecordTime >= 0)
                {
                    nextRecordTime = dt + interval;
                    interval -= accelerate;

                    if (interval <= mininterval)
                    {
                        interval = mininterval;
                    }

                    if (remainCount == 0)
                    {

                        //Sys_Hint.Instance.PushContent_Normal("道具不够了");
                        Sys_Hint.Instance.PushContent_Normal(itemNotEnough);
                        return;
                    }

                    //if (livingSkill.bExpFull)
                    //{
                    //    // Sys_Hint.Instance.PushContent_Normal("经验值已满");
                    //    Sys_Hint.Instance.PushContent_Normal(ExpFullcontent);
                    //    return;
                    //}
                    count += 1;
                    remainCount = curCount - count;
                    SetCount();
                    string itemName = CSVLanguage.Instance.GetConfData(cSVItemData.name_id).words;//道具名
                    uint exp = cSVItemData.fun_value[2];//熟练度
                    string skillName = CSVLanguage.Instance.GetConfData(livingSkill.cSVLifeSkillData.name_id).words;//技能名

                    Sys_Hint.Instance.PushContent_Normal(string.Format("消耗一个道具{0},获得熟练度{1}", itemName, exp));
                    AddExp(exp);
                }
            }

            private void OnRelease()
            {
                if (uI_LongPressButton.bLongPressed)
                {
                    curCount = remainCount;

                    if (count > 0)
                    {
                        //Sys_Hint.Instance.PushContent_Normal(string.Format("向服务器请求使用{0}个item", count));
                        Sys_Bag.Instance.UseItemById(itemId, (uint)count);
                    }
                }
                else
                {
                    if (remainCount == 0)
                    {
                        //Sys_Hint.Instance.PushContent_Normal("道具不够了");
                        Sys_Hint.Instance.PushContent_Normal(itemNotEnough);
                        return;
                    }
                    //if (livingSkill.bExpFull)
                    //{
                    //    // Sys_Hint.Instance.PushContent_Normal("经验值已满");
                    //    Sys_Hint.Instance.PushContent_Normal(ExpFullcontent);
                    //    return;
                    //}
                    remainCount--;
                    curCount = remainCount;
                    SetCount();

                    string itemName = CSVLanguage.Instance.GetConfData(cSVItemData.name_id).words;//道具名
                    uint exp = cSVItemData.fun_value[2];//熟练度
                    string skillName = CSVLanguage.Instance.GetConfData(CSVLifeSkill.Instance.GetConfData(skillId).name_id).words;//技能名
                                                                                                                                  //Sys_Hint
                    Sys_Hint.Instance.PushContent_Normal(string.Format(ExpGet, itemName, exp, skillName));
                    Sys_Bag.Instance.UseItemById(itemId, 1);
                    //Sys_Hint.Instance.PushContent_Normal(string.Format("向服务器请求使用1个item"));
                    AddExp(exp);
                }
            }

            public void Dispose()
            {
                GameObject.Destroy(uI_LongPressButton);
                mIconButton.onClick.RemoveListener(OnIconButtonClicked);
            }

            public void Select()
            {
                mSelectObj.SetActive(true);
            }

            public void Release()
            {
                mSelectObj.SetActive(false);
            }
        }
    }
}


