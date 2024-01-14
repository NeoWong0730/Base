using UnityEngine;
using System.Collections;
using Logic.Core;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_PetFriend_LevelAttr : IDisposable
    {
        CSVPetNewLoveUp.Data data = null;
        private Transform transform;
        private Text levelText;
        private GameObject lockGo;
        public void Init(Transform transform)
        {
            this.transform = transform;
            levelText = transform.Find("bg/Text_Level").GetComponent<Text>();
            lockGo = transform.Find("bg/Lock").gameObject;
        }

        public void SetData(CSVPetNewLoveUp.Data data, uint currentLevel)
        {
            this.data = data;
            RefreshData(currentLevel);
        }

        public void RefreshData(uint currentLevel)
        {
            if (null != data)
            {
                float tempa = data.id / 1000.0f;
                uint petId = (uint)Math.Floor(tempa);           
                uint level = data.id - petId * 1000;
                lockGo.SetActive(!Sys_Pet.Instance.GetPetIsActive(petId) || currentLevel < level);
                TextHelper.SetText(levelText, LanguageHelper.GetTextContent(2009426, level.ToString()));
                int allCount = 0;
                int oneCount = 0;
                if (null != data.OneselfEffec)
                {
                    oneCount = data.OneselfEffec.Count;
                }
                if (data.RaceType != 0)
                {
                    if (null != data.RaceEffec)
                    {
                        allCount = data.RaceEffec.Count + oneCount;
                    }
                }
                else
                {
                    allCount = oneCount;
                }
                FrameworkTool.CreateChildList(transform, allCount, 1);

                for (int i = 1; i < oneCount + 1; i++)
                {
                    Transform tra = transform.GetChild(i);
                    List<uint> attrlist = data.OneselfEffec[i - 1];
                    if (attrlist.Count >= 2)
                    {
                        CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrlist[0]);
                        if (null != attrInfo)
                        {
                            TextHelper.SetText(tra.GetComponent<Text>(), 
                                LanguageHelper.GetTextContent(attrInfo.show_type == 1 ? 10680u : 10682u,
                                LanguageHelper.GetTextContent(attrInfo.name), attrInfo.show_type == 1 ? attrlist[1].ToString() : (attrlist[1] / 100.0f).ToString()));
                        }
                    }

                }

                for (int i = oneCount + 1; i < allCount + 1; i++)
                {
                    Transform tra = transform.GetChild(i);
                    List<uint> attrlist = data.RaceEffec[i - oneCount - 1];
                    if (attrlist.Count >= 2)
                    {
                        CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrlist[0]);
                        if (null != attrInfo)
                        {
                            CSVGenus.Data zzData = CSVGenus.Instance.GetConfData(data.RaceType);
                            if(null != zzData)
                            {
                                TextHelper.SetText(tra.GetComponent<Text>(),
                                LanguageHelper.GetTextContent(attrInfo.show_type == 1 ? 10681u : 10683u,
                                LanguageHelper.GetTextContent(zzData.rale_name), LanguageHelper.GetTextContent(attrInfo.name), attrInfo.show_type == 1 ? attrlist[1].ToString() : (attrlist[1] / 100.0f).ToString()));
                            }                            
                        }
                    }
                }
            }
        }

        public void Dispose()
        {

        }
    }

    public class UI_Pet_BookReview_RightBackGround : UIComponent
    {
        private uint currentPetId;
        private GameObject backgroundGo;
        private List<UI_Pet_Background_Ceil> ceilList = new List<UI_Pet_Background_Ceil>();
        protected override void Loaded()
        {
            backgroundGo = transform.Find("Scroll_View/Grid/Btn01").gameObject;
        }


        public override void SetData(params object[] arg)
        {
            currentPetId = (uint)arg[0];
        }

        public override void Show()
        {
            base.Show();
            GenAllPetStoryCard();
        }

        private void GenAllPetStoryCard()
        {
            backgroundGo.SetActive(true);
            for (int i = 0; i < backgroundGo.transform.parent.transform.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(backgroundGo.transform.parent.transform.GetChild(i).gameObject);
            }

            ceilList.Clear();
            List<uint> thisPetstoryIds = Sys_Pet.Instance.GetPetAllStoryIdOfPetId(currentPetId);
            int storyCount = thisPetstoryIds.Count;
            for (int i = 0; i < storyCount; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(backgroundGo, backgroundGo.transform.parent);
                UI_Pet_Background_Ceil storyCardSub = new UI_Pet_Background_Ceil();
                storyCardSub.Init(go.transform);
                storyCardSub.SetCeilData(thisPetstoryIds[i], currentPetId, (uint)i);
                ceilList.Add(storyCardSub);
            }

            backgroundGo.SetActive(false);
        }

        public void RefreshCeil(uint stroyId)
        {
            for (int i = 0; i < ceilList.Count; i++)
            {
                UI_Pet_Background_Ceil item = ceilList[i];

                if (item.storyId == stroyId)
                {
                    item.ResetValue();
                }
            }
        }

        public void SelectState()
        {
            for (int i = 0; i < ceilList.Count; i++)
            {
                ceilList[i].SelectState();
            }

        }

        public override void Hide()
        {
            base.Hide();
        }
    }

    public class UI_Pet_BookReview_RightFriend : UIComponent
    {
        private uint currentPetId;
        private Button ruleBtn;
        private Button useBtn;
        private Text friendLevelText;
        private Text silderText;
        private Slider friendSlider;
        private Transform propItemParent;
        private Transform attrTransfrom;
        private GameObject LevelFullGo;
        private List<UI_PetFriend_LevelAttr> uI_PetFriend_LevelAttrs = new List<UI_PetFriend_LevelAttr>();
        private Button detailBtn;
        public CP_ToggleRegistry selectToggle;
        protected override void Loaded()
        {
            ruleBtn = transform.Find("bg/Btn_Help").GetComponent<Button>();
            ruleBtn.onClick.AddListener(OnRuleBtnCliecked);
            useBtn = transform.Find("Btn_01").GetComponent<Button>();
            useBtn.onClick.AddListener(FriendFunctionBtn);
            friendLevelText = transform.Find("bg/Text_Level").GetComponent<Text>();
            friendSlider = transform.Find("bg/Slider_Exp").GetComponent<Slider>();
            silderText = transform.Find("bg/Text_Value").GetComponent<Text>();
            attrTransfrom = transform.Find("Scroll_Attr/Content");
            propItemParent = transform.Find("ItemGroup");
            selectToggle = propItemParent.GetComponent<CP_ToggleRegistry>();
            LevelFullGo = transform.Find("Image_Full").gameObject;
            selectToggle.onToggleChange = OnSelectTypeChange;
            detailBtn = transform.Find("Button_Detail").GetComponent<Button>();
            detailBtn.onClick.AddListener(DetailBtnClicked);
        }
        private int selectType = 0;
        public void OnSelectTypeChange(int curToggle, int old)
        {
            if (curToggle == old)
                return;
            selectType = curToggle;
        }

        private void ItemGridBeClicked(PropItem bulidItem)
        {
            selectType = bulidItem.transform.GetComponent<CP_Toggle>().id;
            selectToggle.SwitchTo(selectType);
        }

        private void DetailBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Pet_BookReview_Tip);
        }

        private void OnRuleBtnCliecked()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2009498) });
        }

        public override void SetData(params object[] arg)
        {
            currentPetId = (uint)arg[0];
        }

        public override void Show()
        {
            base.Show();
            GenAllPetFriendAttr();
        }

        private void GenAllPetFriendAttr()
        {
            Packet.CmdPetGetHandbookRes.Types.HandbookData handbookData = Sys_Pet.Instance.GetPetBookData(currentPetId);
            uint level = 0;
            float exp = 0;
            if (null != handbookData)
            {
                level = handbookData.LoveLevel;
                exp = handbookData.LoveExp;
            }
            RefreshLevel(level, exp);

            for (int i = 0; i < uI_PetFriend_LevelAttrs.Count; i++)
            {
                PoolManager.Recycle(uI_PetFriend_LevelAttrs[i]);
            }

            uI_PetFriend_LevelAttrs.Clear();
            List<CSVPetNewLoveUp.Data>  allLoveData = Sys_Pet.Instance.GetPetLoveUpDataByPetId(currentPetId);
            int count = allLoveData.Count;
            FrameworkTool.CreateChildList(attrTransfrom, count);
            for (int i = 0; i < count; i++)
            {
                Transform tran = attrTransfrom.GetChild(i);
                UI_PetFriend_LevelAttr attr = PoolManager.Fetch<UI_PetFriend_LevelAttr>();
                attr.Init(tran);
                attr.SetData(allLoveData[i], level);
                uI_PetFriend_LevelAttrs.Add(attr);
            }
        }

        
        private void RefreshLevel(uint level, float exp)
        {
            TextHelper.SetText(friendLevelText, LanguageHelper.GetTextContent(2009296, level.ToString()));
            CSVPetNewLoveUp.Data cSVPetLoveData = CSVPetNewLoveUp.Instance.GetConfData(currentPetId * 1000 + level);
            bool isLevelFull = false;
            isLevelFull = null != cSVPetLoveData && cSVPetLoveData.exp == 0;
            if (!isLevelFull && null != cSVPetLoveData)
            {
                friendSlider.value = exp / cSVPetLoveData.exp;
                silderText.text = string.Format("{0}/{1}", exp.ToString(), cSVPetLoveData.exp.ToString());
            }
            
            LevelFullGo.SetActive(isLevelFull);
            useBtn.gameObject.SetActive(!isLevelFull);
            propItemParent.gameObject.SetActive(!isLevelFull);
            friendSlider.gameObject.SetActive(!isLevelFull);
            silderText.gameObject.SetActive(!isLevelFull);
            CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(currentPetId);
            if (!isLevelFull && null != cSVPetData)
            {
                var ids = new List<ItemIdCount>();
                var itemId1 = new ItemIdCount(cSVPetData.PetBooks, 1);
                
                ids.Add(itemId1);
                if (null != cSVPetData.pet_feel_lv && cSVPetData.pet_feel_lv.Count >= 2)
                {
                    var itemId2 = new ItemIdCount(cSVPetData.pet_feel_lv[0], cSVPetData.pet_feel_lv[1]);
                    ids.Add(itemId2);
                }
                    
                var count = ids.Count;
                FrameworkTool.CreateChildList(propItemParent, count);
                for (int i = 0; i < count; i++)
                {
                    Transform trans = propItemParent.GetChild(i);
                    CP_Toggle tempToggle = trans.GetComponent<CP_Toggle>();
                    tempToggle.id = i;
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(trans.gameObject);
                    PropIconLoader.ShowItemData rightItem = new PropIconLoader.ShowItemData(ids[i].id, ids[i].count, true, false, false, false, false, true, true,_onClick: ItemGridBeClicked);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_BookReview, rightItem));
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(ids[i].id);
                    if (null != cSVItemData)
                        TextHelper.SetText(propItem.txtName, cSVItemData.name_id);
                }
            }
            selectToggle.SwitchTo(selectType);
        }

        public void RefreshData()
        {
            Packet.CmdPetGetHandbookRes.Types.HandbookData handbookData = Sys_Pet.Instance.GetPetBookData(currentPetId);
            RefreshLevel(handbookData.LoveLevel, handbookData.LoveExp);
            if (null != handbookData)
            {
                for (int i = 0; i < uI_PetFriend_LevelAttrs.Count; i++)
                {
                    uI_PetFriend_LevelAttrs[i].RefreshData(handbookData.LoveLevel);
                }
            }            
        }

        public void FriendFunctionBtn()
        {
            if (!Sys_Pet.Instance.GetPetIsActive(currentPetId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10691));
                return;
            }

            Packet.CmdPetGetHandbookRes.Types.HandbookData handbookData = Sys_Pet.Instance.GetPetBookData(currentPetId);
            if (null != handbookData)
            {
                CSVPetNewLoveUp.Data data = CSVPetNewLoveUp.Instance.GetConfData(currentPetId * 1000 + handbookData.LoveLevel);
                if(null != data)
                {
                    List<uint> task = data.BackgroundStory;
                    if (null != task && task.Count >= 1)
                    {
                        uint taskid = task[task.Count - 1];
                        CSVTask.Data csvtask = CSVTask.Instance.GetConfData(taskid);
                        if (!Sys_Task.Instance.IsSubmited(taskid))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10692, LanguageHelper.GetTaskTextContent(csvtask.taskName)));
                            return;
                        }
                    }    
                }
                else
                {
                    DebugUtil.LogErrorFormat("config CSVPetNewNewLoveUp not find petId = {0}, BookLevel = {1}", currentPetId, handbookData.LoveLevel);
                    return;
                }

                ItemIdCount itemIdCount = null;
                CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(currentPetId);
                uint chipCount = 0;
                uint bookCount = 0;
                if (selectType > 0)
                {
                    if (null != cSVPetData.pet_feel_lv && cSVPetData.pet_feel_lv.Count >= 2)
                    {
                        chipCount = 1;
                        bookCount = 0;
                        itemIdCount = new ItemIdCount(cSVPetData.pet_feel_lv[0], cSVPetData.pet_feel_lv[1]);
                    }
                }
                else
                {
                    chipCount = 0;
                    bookCount = 1;
                    itemIdCount = new ItemIdCount(cSVPetData.PetBooks, bookCount);
                }

                if (itemIdCount.Enough)
                {
                    Sys_Pet.Instance.OnPetLoveExpUpReq(currentPetId, bookCount, chipCount);
                }
                else
                {
                    if (null != itemIdCount.CSV)
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009469, LanguageHelper.GetTextContent(itemIdCount.CSV.name_id)));
                }

            }
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
