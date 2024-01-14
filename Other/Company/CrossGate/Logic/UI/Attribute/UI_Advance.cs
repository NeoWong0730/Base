using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;
using Packet;
using System;

namespace Logic
{
    public class UI_Advance : UIComponent
    {
        private Image beforeIcon;
        private Image beforeFrame;
        private Image afterIcon;
        private Image afterFrame;
        private Image maxIcon;
        private Image maxFrame;
        private Text beforeName;
        private Text afterName;
        private Text maxName;
        private Text beforeLevel;
        private Text afterLevel;
        private Text taskOpenLevel;
        private Button advanceBtn;
        private Button tipBtn;

        private GameObject targetGo;
        private GameObject skillGo;
        private GameObject normalGo;
        private GameObject maxGo;
        private GameObject attrGo;
        private GameObject attrRootGo;
        private GameObject levelUpGo;
        private GameObject tipMsg;

        #region Title
        private Text mTitle_text1;

        private Text mTitle_text2;
        private Image mTitle_img2;

        private Image mTitle_img3;
        private Transform mTitle_Fx3parent;

        AsyncOperationHandle<GameObject> requestRef;
        private GameObject titleEffect;

        #endregion   

        private ECareerType careerType;
        private uint currentlevel;
        private uint curPromoteCareerId;
        private CSVPromoteCareer.Data csvPromoteCareer;
        private CSVPromoteCareer.Data csvPromoteCareerNext;
        private List<UI_TargetItem> targetlist = new List<UI_TargetItem>();
        private List<UI_SkillItem> skilllist = new List<UI_SkillItem>();

        protected override void Loaded()
        {
            beforeIcon = transform.Find("View_Middle/View_Normal/Image_Icon").GetComponent<Image>();
            beforeFrame = transform.Find("View_Middle/View_Normal/Image_Icon/Image_Before_Frame").GetComponent<Image>();
            beforeName = transform.Find("View_Middle/View_Normal/Image_Icon/Text_Name").GetComponent<Text>();
            afterIcon = transform.Find("View_Middle/View_Normal/Image_Icon2").GetComponent<Image>();
            afterFrame = transform.Find("View_Middle/View_Normal/Image_Icon2/Image_Before_Frame").GetComponent<Image>();
            afterName = transform.Find("View_Middle/View_Normal/Image_Icon2/Text_Name").GetComponent<Text>();
            maxIcon = transform.Find("View_Middle/Image_Icon_Max").GetComponent<Image>();
            maxFrame = transform.Find("View_Middle/Image_Icon_Max/Image_Before_Frame").GetComponent<Image>();
            maxName = transform.Find("View_Middle/Image_Icon_Max/Text_Name").GetComponent<Text>();
            beforeLevel = transform.Find("View_Right/Text_Title04/Text_Lv1").GetComponent<Text>();
            afterLevel = transform.Find("View_Right/Text_Title04/Text_Lv2").GetComponent<Text>();
            taskOpenLevel = transform.Find("View_Middle/View_Normal/Image_BG_Buttom/Text_Condition").GetComponent<Text>();
            advanceBtn = transform.Find("View_Right/Button_Advance").GetComponent<Button>();
            advanceBtn.onClick.AddListener(OnAdvancebtnClick);
            tipBtn = transform.Find("View_Right/Text_Title04/Btn_Tips").GetComponent<Button>();
            tipBtn.onClick.AddListener(OnTipsbtnClick);

            targetGo = transform.Find("View_Middle/View_Normal/Scroll_View/Condition_List/Item").gameObject;
            attrGo = transform.Find("View_Right/Grid_Attr/Image_Attr").gameObject;
            skillGo = transform.Find("View_Right/Scroll_View/List/Item").gameObject;
            maxGo = transform.Find("View_Middle/Image_Icon_Max").gameObject;
            normalGo = transform.Find("View_Middle/View_Normal").gameObject;
            attrRootGo = transform.Find("View_Right/Grid_Attr").gameObject;
            levelUpGo = transform.Find("View_Right/Text_Title04").gameObject;
            tipMsg=transform.Find("View_Right/Text_Title09").gameObject;

            mTitle_text1 = transform.Find("View_Right/Title/Text").GetComponent<Text>();
            mTitle_text2 = transform.Find("View_Right/Title/Image1/Text").GetComponent<Text>();
            mTitle_img2 = transform.Find("View_Right/Title/Image1").GetComponent<Image>();
            mTitle_img3 = transform.Find("View_Right/Title/Image2").GetComponent<Image>();
            mTitle_Fx3parent = transform.Find("View_Right/Title/Image2/Fx");
        }

        uint pageShowTime;
        public override void Show()
        {
            base.Show();
            SetValue();

            UIManager.HitPointShow(EUIID.UI_Attribute, ERoleViewType.ViewAdvance.ToString());
            pageShowTime = Sys_Time.Instance.GetServerTime();
        }

        public override void Hide()
        {
            base.Hide();
            DefaultAttrItem();
            DefaultSkillItem();
            DefaultTargetItem();
            AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetLoaded);

            UIManager.HitPointHide(EUIID.UI_Attribute, pageShowTime, ERoleViewType.ViewAdvance.ToString());
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Role.Instance.eventEmitter.Handle<uint>(Sys_Role.EEvents.OnUpdateCareerRank, OnUpdateCareerRank, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateMerchantInfo, OnUpdateMerchantInfo, toRegister);
            Sys_Advance.Instance.eventEmitter.Handle(Sys_Advance.EEvents.OnTeamLeaderCheckPromoteCareer, OnTeamLeaderCheckPromoteCareer, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnTeamMemberState, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnMemberLeave, toRegister);
        }

        private void OnMemberLeave(ulong role)
        {
            if (csvPromoteCareerNext.teamCondition != 0)
            {
                for (int i = 0; i < targetlist.Count; ++i)
                {
                    if (targetlist[i].type == EPromoteCareerType.FullTeam)
                    {
                        targetlist[i].RefreshShow(targetlist[targetlist.Count - 2].isFinish, i + 1);
                        break;
                    }
                }
                advanceBtn.enabled = true;
                ImageHelper.SetImageGray(advanceBtn.GetComponent<Image>(), !CanAdvanceNow());
            }
        }

        private void OnTeamMemberState(ulong role)
        {
            if (csvPromoteCareerNext.teamCondition != 0)
            {
                for (int i = 0; i < targetlist.Count; ++i)
                {
                    if (targetlist[i].type == EPromoteCareerType.FullTeam)
                    {
                        targetlist[i].RefreshShow(targetlist[targetlist.Count - 2].isFinish, i + 1);
                        break;
                    }
                }
                advanceBtn.enabled = true;
                ImageHelper.SetImageGray(advanceBtn.GetComponent<Image>(), !CanAdvanceNow());
            }
        }

        private void OnUpdateMerchantInfo()
        {
            for (int i = 0; i < targetlist.Count; ++i)
            {

                if (i > 0)
                {
                    targetlist[i].RefreshShow(targetlist[targetlist.Count - 2].isFinish, i + 1);
                }
                else
                {
                    targetlist[i].RefreshShow(true, i + 1);
                }
            }
        }

        private void OnTeamLeaderCheckPromoteCareer()
        {
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(csvPromoteCareerNext.advanceNpc);
            UIManager.CloseUI(EUIID.UI_Attribute);
        }

        private void OnTimeNtf(uint arg1, uint arg2)
        {
            for (int i = 0; i <targetlist.Count; ++i)
            {

                if (i > 0)
                {
                    targetlist[i].RefreshShow(targetlist[targetlist.Count - 2].isFinish, i+1);
                }
                else
                {
                    targetlist[i].RefreshShow(true, i+1);
                }
            }
        }

        private void OnUpdateCareerRank(uint nowrank)
        {
            SetValue();
        }

        private void SetValue()
        {
            DefaultAttrItem();
            DefaultSkillItem();
            DefaultTargetItem();
            curPromoteCareerId = Sys_Advance.Instance.SetAdvanceRank();
            if (CSVPromoteCareer.Instance.TryGetValue(curPromoteCareerId, out csvPromoteCareer) && csvPromoteCareer != null)
            {
            }
            else
            {
                DebugUtil.LogErrorFormat("进阶表不存在key：" + curPromoteCareerId);
                return;
            }
            tipMsg.SetActive(Sys_Role.Instance.Role.CareerRank == 2);
            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
            if (CSVHeadframe.Instance.TryGetValue(csvPromoteCareer.head, out CSVHeadframe.Data cSVHeadframeData))
            {
                ImageHelper.SetIcon(beforeFrame, cSVHeadframeData.HeadframeIcon);
                ImageHelper.SetIcon(maxFrame, cSVHeadframeData.HeadframeIcon);
            }
            if (CSVPromoteCareer.Instance.TryGetValue(curPromoteCareerId + 1, out csvPromoteCareerNext) && csvPromoteCareerNext != null)
            {
                normalGo.SetActive(true);
                maxGo.SetActive(false);
                attrRootGo.SetActive(true);
                levelUpGo.SetActive(true);
                if (heroData != null)
                {
                    ImageHelper.SetIcon(beforeIcon, heroData.headid);
                    ImageHelper.SetIcon(afterIcon, heroData.headid);
                }
 
                if (CSVHeadframe.Instance.TryGetValue(csvPromoteCareer.head, out CSVHeadframe.Data cSVNextHeadframeData))
                {
                    ImageHelper.SetIcon(afterFrame, cSVNextHeadframeData.HeadframeIcon);
                }
                beforeName.text = LanguageHelper.GetTextContent(csvPromoteCareer.professionLan);
                afterName.text = LanguageHelper.GetTextContent(csvPromoteCareerNext.professionLan);
                mTitle_text1.transform.parent.gameObject.SetActive(true);
                UpdateTitle(CSVTitle.Instance.GetConfData(csvPromoteCareerNext.title));
                if (CSVTask.Instance.GetConfData(csvPromoteCareerNext.conditions[0]).taskLvLowerLimit != 0)
                {
                    taskOpenLevel.text = LanguageHelper.GetTextContent(2005033, CSVTask.Instance.GetConfData(csvPromoteCareerNext.conditions[0]).taskLvLowerLimit.ToString());
                }
                else
                {
                    taskOpenLevel.text = string.Empty;
                }
                if (csvPromoteCareer.levelLimit == csvPromoteCareerNext.levelLimit)
                {
                    levelUpGo.SetActive(false);
                }
                else
                {
                    levelUpGo.SetActive(true);
                    beforeLevel.text = csvPromoteCareer.levelLimit.ToString();
                    afterLevel.text = csvPromoteCareerNext.levelLimit.ToString();
                }
                AddTargetList();
                AddAttrList();
                AddSkillList(false);
                advanceBtn.enabled = true;
                ImageHelper.SetImageGray(advanceBtn.GetComponent<Image>(), !CanAdvanceNow());
            }
            else
            {
                normalGo.SetActive(false);
                maxGo.SetActive(true);
                attrRootGo.SetActive(false);
                levelUpGo.SetActive(false);
                maxName.text = LanguageHelper.GetTextContent(CSVTitle.Instance.GetConfData(csvPromoteCareer.title).titleLan);
                mTitle_text1.transform.parent.gameObject.SetActive(false);
                if (heroData != null)
                {
                    ImageHelper.SetIcon(maxIcon, heroData.headid);
                }
                AddSkillList(true);
                ImageHelper.SetImageGray(advanceBtn.GetComponent<Image>(), true);
                advanceBtn.enabled = false;
            }
        }

        #region Title
        public void UpdateTitle(CSVTitle.Data cSVTitleData)
        {
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType(1);
                        TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType(2);
                        TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType(3);
                        ImageHelper.SetIcon(mTitle_img3, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
        }

        private void LoadTitleEffectAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetLoaded);
        }

        private void OnAssetLoaded(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect = handle.Result;
            if (null != titleEffect)
            {
                titleEffect.transform.SetParent(mTitle_Fx3parent);
                RectTransform rectTransform = titleEffect.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void SetTitleShowType(int type)
        {
            if (type == 1)
            {
                mTitle_text1.gameObject.SetActive(true);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(true);
                mTitle_img2.gameObject.SetActive(true);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else
            {
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(true);
                mTitle_Fx3parent.gameObject.SetActive(true);
            }
        }
        #endregion

        #region Function
        private void AddTargetList()
        {
            targetlist.Clear();
            if (csvPromoteCareerNext != null)
            {
                Sys_Advance.Instance.SetTargets();
                for (int i = 0; i < Sys_Advance.Instance.targetlist.Count; ++i)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(targetGo, targetGo.transform.parent);
                    UI_TargetItem target = new UI_TargetItem(csvPromoteCareerNext.id, Sys_Advance.Instance.targetlist[i].funId, Sys_Advance.Instance.targetlist[i].type);
                    target.Init(go.transform);
                    target.Show();
                    targetlist.Add(target);
                    if (i > 0)
                    {
                        target.RefreshShow(targetlist[targetlist.Count - 2].isFinish, targetlist.Count);
                    }
                    else
                    {
                        target.RefreshShow(true, targetlist.Count);
                    }
                }
                targetGo.SetActive(false);
            }
        }

        private void AddSkillList(bool isMax)
        {
            skilllist.Clear();
            if (csvPromoteCareer.skill_lv == null || csvPromoteCareer.skill_lv.Count == 0)
            {

                return;
            }
            if (csvPromoteCareer != null)
            {
                for (int i = 0; i < csvPromoteCareer.skill.Count; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(skillGo, skillGo.transform.parent);
                    UI_SkillItem skill = new UI_SkillItem(curPromoteCareerId, isMax);
                    skill.Init(go.transform);
                    skill.Refresh(i);
                    skilllist.Add(skill);
                }
                skillGo.SetActive(false);
            }
        }

        private void AddAttrList()
        {
            attrGo.SetActive(false);
            if (csvPromoteCareerNext.propertyAdd == null)
                return;
            for (int i = 0; i < csvPromoteCareerNext.propertyAdd.Count; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(attrGo, attrGo.transform.parent);
                go.SetActive(true);
                uint attrId = csvPromoteCareerNext.propertyAdd[i][0];
                go.transform.Find("Text_Attr").GetComponent<Text>().text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrId).name);
                uint curPropertyAdd = 0;
                if (csvPromoteCareer.propertyAdd != null && csvPromoteCareer.propertyAdd.Count != 0)
                {
                    curPropertyAdd = csvPromoteCareer.propertyAdd[i][1];
                }
                if (CSVAttr.Instance.GetConfData(attrId).show_type == 1)
                {
                    go.transform.Find("Text_Attr/Text_Number").GetComponent<Text>().text = "+" + (csvPromoteCareerNext.propertyAdd[i][1] - curPropertyAdd).ToString();
                }
                else
                {
                    go.transform.Find("Text_Attr/Text_Number").GetComponent<Text>().text = LanguageHelper.GetTextContent(2007704, (csvPromoteCareerNext.propertyAdd[i][1] / 100 - curPropertyAdd / 100).ToString());
                }
                if (i % 2 == 0)
                {
                    go.transform.Find("Text_Attr/Image_Line_Attr01").gameObject.SetActive(true);
                }
                else
                {
                    go.transform.Find("Text_Attr/Image_Line_Attr01").gameObject.SetActive(false);
                }
            }
        }

        private void DefaultSkillItem()
        {
            skillGo.SetActive(true);
            for (int i = 0; i < skilllist.Count; ++i) { skilllist[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(skillGo.transform.parent.gameObject, skillGo.transform.name);
        }

        private void DefaultTargetItem()
        {
            targetGo.SetActive(true);
            for (int i = 0; i < targetlist.Count; ++i) { targetlist[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(targetGo.transform.parent.gameObject, targetGo.transform.name);
        }

        private void DefaultAttrItem()
        {
            attrGo.SetActive(true);
            FrameworkTool.DestroyChildren(attrGo.transform.parent.gameObject, attrGo.transform.name);
        }

        #endregion

        #region ButtonClick
        private void OnAdvancebtnClick()
        {
            if (csvPromoteCareerNext != null && CanAdvanceNow())
            {
                if (csvPromoteCareerNext.teamCondition != 0)
                {
                    Sys_Role.Instance.TeamLeaderCheckPromoteCareerReq();
                }
                else
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(csvPromoteCareerNext.advanceNpc);
                    UIManager.CloseUI(EUIID.UI_Attribute);
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005031));
            }
        }

        private void OnTipsbtnClick()
        {
            UIManager.OpenUI(EUIID.UI_Advance_Warning, false, false);
        }

        private bool CanAdvanceNow()
        {
            foreach (var item in targetlist)
            {
                if (!item.isFinish)
                {

                    return false;
                }
            }
            return true;
        }

        #endregion
    }

    public class UI_TargetItem : UIComponent
    {
        private uint id;
        private uint funId;
        private int taskIndex;
        public EPromoteCareerType type;
        private int fullExp;
        private Text title;
        private Text message;
        private Button btn;
        private GameObject finishGo;
        private GameObject upGo;
        private GameObject needSubmitBeforeTaskTipGo;
        private CSVPromoteCareer.Data csvPromoteCareerData;
        public bool isFinish;
        public bool isReqReceive;
        private Timer timer;

        public UI_TargetItem(uint _id, uint _funId, EPromoteCareerType _type) : base()
        {
            id = _id;
            funId = _funId;
            type = _type;
        }

        protected override void Loaded()
        {
            title = transform.Find("Text_Title").GetComponent<Text>();
            message = transform.Find("Text_Content").GetComponent<Text>();
            btn = transform.Find("Image_Unget/Button_Go").GetComponent<Button>();
            btn.onClick.AddListener(OnbtnClicked);
            finishGo = transform.Find("Image_Get").gameObject;
            upGo = transform.Find("Image_Unget").gameObject;
            needSubmitBeforeTaskTipGo = transform.Find("Text01").gameObject;
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            base.ProcessEventsForEnable(toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnReceived, toRegister);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            timer?.Cancel();
        }

        private void OnReceived(int taskCategory, uint taskId, TaskEntry taskEntry)
        {
            if (taskId == funId && taskCategory == (int)ETaskCategory.Advance && isReqReceive)
            {
                Sys_Task.Instance.TryDoTask(taskEntry, true, false, true);
                isReqReceive = false;
            }
        }

        private void OnbtnClicked()
        {
            if (type == EPromoteCareerType.Task)
            {
                isReqReceive = false;
                if (TaskHelper.HasReceived(funId))
                {
                    TaskEntry taskEntry = Sys_Task.Instance.GetTask(funId);
                    Sys_Task.Instance.TryDoTask(taskEntry, true, false, true);
                }
                else
                {
                    CSVTask.Instance.TryGetValue(funId, out CSVTask.Data cSVTaskData);
                    if (cSVTaskData != null && cSVTaskData.taskCategory == (int)ETaskCategory.Advance)
                    {
                        Sys_Task.Instance.ReqReceive(funId, true);
                        isReqReceive = true;
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(csvPromoteCareerData.task_hintlan[taskIndex]));
                    }
                }
            }
            else if (type == EPromoteCareerType.Title)
            {
                UIManager.CloseUI(EUIID.UI_Attribute);
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1000001);
            }
            else if (type == EPromoteCareerType.SkillUpdata)
            {
                UIManager.OpenUI(EUIID.UI_SkillUpgrade);
            }
            else if(type == EPromoteCareerType.FullTeam)
            {
                UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
            }
            else if (type == EPromoteCareerType.MercheetLevel)
            {
                UIManager.OpenUI(EUIID.UI_MerchantFleet);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_DailyActivites);
            }
        }


        public void RefreshShow(bool lastTaskIsFinish, int indexInList)
        {
            isFinish = false;
            csvPromoteCareerData = CSVPromoteCareer.Instance.GetConfData(id);
            for (int i = 0; i < csvPromoteCareerData.conditions.Count; ++i)
            {
                if (csvPromoteCareerData.conditions[i] == funId)
                {
                    taskIndex = i;
                }
            }
            needSubmitBeforeTaskTipGo.SetActive(false);
            title.text = LanguageHelper.GetTextContent(2005032, LanguageHelper.GetTextContent((uint)(10127 + indexInList)));
            btn.gameObject.SetActive(type != EPromoteCareerType.OpenServerTime);
            if (type == EPromoteCareerType.Task)
            {
                message.text = LanguageHelper.GetTextContent(csvPromoteCareerData.task_dislan[taskIndex]);
                if (lastTaskIsFinish)
                {
                    if (TaskHelper.HasSubmited(funId))
                    {
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    }
                    else
                    {
                        ImageHelper.SetImageGray(upGo.GetComponent<Image>(), !TaskHelper.HasReceived(funId));
                        upGo.SetActive(true);
                        finishGo.SetActive(false);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
            else if (type == EPromoteCareerType.Title)
            {
                message.text = LanguageHelper.GetTextContent(2005003, LanguageHelper.GetTextContent(CSVTitle.Instance.GetConfData(funId).titleLan));
                if (lastTaskIsFinish)
                {
                    if (Sys_Title.Instance.TitleGet(funId))
                    {
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    }
                    else
                    {
                        upGo.SetActive(true);
                        finishGo.SetActive(false);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
            else if (type == EPromoteCareerType.SkillUpdata)
            {
                message.text = LanguageHelper.GetTextContent(2005005, csvPromoteCareerData.skillLimit[0].ToString(), csvPromoteCareerData.skillLimit[1].ToString(), csvPromoteCareerData.skillLimit[2].ToString());
                if (lastTaskIsFinish)
                {
                    int count = 0;
                    foreach (var skills in Sys_Skill.Instance.bestSkillInfos)
                    {
                        if (skills.Value.Level >= csvPromoteCareerData.skillLimit[1] && skills.Value.Rank >= csvPromoteCareerData.skillLimit[2])
                        {
                            count++;
                        }
                    }
                    if (count >= csvPromoteCareerData.skillLimit[0])
                    {
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    }
                    else
                    {
                        upGo.SetActive(true);
                        finishGo.SetActive(false);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
            else if (type == EPromoteCareerType.OpenServerTime)
            {
                int days = csvPromoteCareerData.serverConditions / 86400;
                uint canAdvanceTime = Sys_Role.Instance.openServiceGameTime + (uint)csvPromoteCareerData.serverConditions;
                uint nowTime = Sys_Time.Instance.GetServerTime();
                if (canAdvanceTime <= nowTime)
                {
                    TextHelper.SetText(message, 2029416, (Sys_Role.Instance.Role.CareerRank+1).ToString(), days.ToString());
                }
                else
                {
                    uint leftTime = canAdvanceTime - nowTime;
                    timer?.Cancel();
                    timer = Timer.Register(leftTime, () =>
                    {
                        timer.Cancel();
                        TextHelper.SetText(message, 2029416, (Sys_Role.Instance.Role.CareerRank + 1).ToString(), days.ToString());
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    },
                    (time) =>
                    {
                        uint cutDown = leftTime - (uint)time;
                        uint day = cutDown / (3600 * 24);
                        uint hour = cutDown % (3600 * 24) / 3600;
                        uint min = cutDown % 3600 / 60;
                        uint sec = cutDown % 60;
                        TextHelper.SetText(message, 2029415, days.ToString(), day.ToString(), hour.ToString(), min.ToString(), sec.ToString());
                    }, false, false);
                }
                if (lastTaskIsFinish)
                {
                    if (canAdvanceTime <= nowTime)
                    {
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    }
                    else
                    {
                        upGo.SetActive(true);
                        finishGo.SetActive(false);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
            else if (type == EPromoteCareerType.MercheetLevel)
            {
                message.text = LanguageHelper.GetTextContent(2005006, csvPromoteCareerData.mercheetLevel.ToString());
                if (lastTaskIsFinish)
                {

                    if (Sys_MerchantFleet.Instance.MerchantLevel >= csvPromoteCareerData.mercheetLevel)
                    {
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    }
                    else
                    {
                        upGo.SetActive(true);
                        finishGo.SetActive(false);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
            else if (type == EPromoteCareerType.FullTeam)
            {
                int count = Sys_Team.Instance.TeamMemsCount;
                for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; ++i)
                {
                    TeamMem teamMem = Sys_Team.Instance.getTeamMem(i);
                    if (teamMem.IsRob() || teamMem.IsLeave() || teamMem.IsOffLine())
                    {
                        count--;
                    }
                }
                message.text =LanguageHelper.GetTextContent(2005008, csvPromoteCareerData.teamCondition.ToString(), count.ToString(), csvPromoteCareerData.teamCondition.ToString());
                if (lastTaskIsFinish)
                {
                    if (count == csvPromoteCareerData.teamCondition)
                    {
                        upGo.SetActive(false);
                        finishGo.SetActive(true);
                        isFinish = true;
                    }
                    else
                    {
                        upGo.SetActive(true);
                        finishGo.SetActive(false);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
            else
            {
                message.text = LanguageHelper.GetTextContent(2005001, funId.ToString());
                if (lastTaskIsFinish)
                {
                    if (Sys_Role.Instance.Role.Exp >= CSVCharacterAttribute.Instance.GetConfData(funId+1).upgrade_exp)
                    {
                        finishGo.SetActive(true);
                        upGo.SetActive(false);
                        isFinish = true;
                    }
                    else
                    {
                        finishGo.SetActive(false);
                        upGo.SetActive(true);
                    }
                }
                else
                {
                    needSubmitBeforeTaskTipGo.SetActive(true);
                    upGo.SetActive(false);
                    finishGo.SetActive(false);
                }
            }
        }
    }

    public class UI_SkillItem : UIComponent
    {
        private Image icon;
        private Text name;
        private Text beforelevel;
        private uint id;
        private bool isMax;
        private CSVPromoteCareer.Data csvData;

        public UI_SkillItem(uint _id, bool _isMax) : base()
        {
            id = _id;
            isMax = _isMax;
        }

        protected override void Loaded()
        {
            icon = transform.Find("SkillItem02/Image_Icon").GetComponent<Image>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            beforelevel = transform.Find("Text_Before_Level").GetComponent<Text>();
        }

        public void Refresh(int index)
        {
            csvData = CSVPromoteCareer.Instance.GetConfData(id);
            ImageHelper.SetIcon(icon, CSVActiveSkillInfo.Instance.GetConfData(csvData.skill[index]).icon);
            name.text = LanguageHelper.GetTextContent(2005038, LanguageHelper.GetTextContent(CSVActiveSkillInfo.Instance.GetConfData(csvData.skill[index]).name));
            if (isMax)
            {
                beforelevel.text = LanguageHelper.GetTextContent(2005036, csvData.skill_lv[index].ToString());
            }
            else
            {
                beforelevel.text = LanguageHelper.GetTextContent(2005035, csvData.skill_lv[index].ToString(), csvData.skill_max[index].ToString());
            }
        }
    }
}