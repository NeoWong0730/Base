using Packet;
using UnityEngine;
using Logic.Core;
using Logic;
using UnityEngine.UI;
using Table;
using Lib.Core;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;
using System.Collections.Generic;
using System;
using System.Linq;

public class UI_Menu : UIBase, UI_Menu_Layout.IListener
{
    private UI_Menu_Layout Layout;

    private bool m_IsEnter = false;
    private bool m_IsEnterNpcLook = false;
    private Text m_TxtEnter;
    private Timer time;
    private uint curlevel;

    private Animator animator01;
    private Animator animator03;

    private UI_TaskOrTeamMain taskOrTeam;

    private UI_MiniMap ui_MiniMap;

    private UI_SpecialTask ui_SpecialTask;
    private UI_MapExplorationTip uI_MapExplorationTip;

    private UI_Menu_Convey ui_Convey;
    private UI_CommonTip ui_ComTip;

    private UI_Menu_RedPoint redPoint;

    private UI_ServerActivityMenu uI_ServerActivityMenu;
    private UI_Daily_Interface uI_Daily_Interface;

    private int nLastRecodeFPS = 0;
    private string sFPSFormat = "FPS:{0}";

    private CSVCollection.Data curCSVCollectionData;
    private Timer collectTimer;
    private Timer transTimer;
    private float transAllTime = 1.0f;
    private uint onedaytime;
    private uint starttime;

    private CSVDetect.Data curCSVDetectData;
    private Timer probeTimer;

    /// <summary>
    /// Update调用的次数
    /// </summary>
    int nUpdateCount;

    private Timer m_DailyLimiteTimer;
    private float LimiteTimeSceond = 0;

    protected override void OnInit()
    {
        //按照30帧跑120/30
        SetIntervalFrame(4);
    }

    protected override void OnLoaded()
    {
        Layout = new UI_Menu_Layout();
        Layout.Parse(gameObject);
        Layout.RegisterEvents(this);

        //m_TxtEnter = Layout.Btn_Copy.transform.Find("Text").GetComponent<Text>();

        ui_MiniMap = AddComponent<UI_MiniMap>(transform.Find("Animator/View_Map"));

        ui_SpecialTask = AddComponent<UI_SpecialTask>(transform.Find("Animator/View_SpecialTask"));

        uI_MapExplorationTip = AddComponent<UI_MapExplorationTip>(transform.Find("Animator/View_SpecialTask01"));

        ui_Convey = new UI_Menu_Convey();
        ui_Convey.Init(transform.Find("Animator/View_Convey"));
        ui_Convey.Hide();

        redPoint = gameObject.GetComponent<UI_Menu_RedPoint>();
        if (redPoint == null)
        {
            redPoint = gameObject.AddComponent<UI_Menu_RedPoint>();
        }
        redPoint.Init(this);

        uI_ServerActivityMenu = AddComponent<UI_ServerActivityMenu>(Layout.m_ServerActivityMenu.transform);
        uI_Daily_Interface = AddComponent<UI_Daily_Interface>(Layout.m_DailyInterface.transform);

        Layout.Btn_Repair.gameObject.SetActive(false);
        Layout.Btn_Uplifted.gameObject.SetActive(false);

        transAllTime = float.Parse(CSVParam.Instance.GetConfData(311).str_value) / 1000f;

        onedaytime = CSVWeatherTime.Instance.GetConfData(1).time;
        starttime = Framework.TimeManager.ConvertFromZeroTimeZone(CSVWeatherTime.Instance.GetConfData(4).time);
    }

    protected override void OnShow()
    {
        SetIconMessage();

        //Layout.view_FPS.SetActive(OptionManager.Instance.mShowFPS.Get());
        TryLoadTaskOrTeam();

        RefreshSurvivalPvp();
        CheckCookingRed();

        ui_MiniMap?.Show();
        ui_SpecialTask?.Show();
        uI_MapExplorationTip?.Show();
        UpdateCollect();
        uI_ServerActivityMenu?.Show();
        uI_Daily_Interface?.Show();
        OnDayNightChange();
        SetExp();
        CheckExplorationRewardTip();
        CheckProbeSkillOpen();
        Sys_Bag.Instance.CheckTempBag();
        Sys_Pet.Instance.CheckTempPetBag();
        animator01 = Layout.Grid01Go.GetComponent<Animator>();
        animator03 = Layout.probebuttonGo.GetComponent<Animator>();

        OnTeamRedInfo();
        CheckNeedShowUplifted();
        OnRefreshCrystal();
        CheckInquiryArea(m_IsEnterNpcLook);
        RefreshDailyLimite();
        RefreshEnemySwitch();
        RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnUIMenuShow, null);
        Sys_LivingSkill.Instance.RegisterButton(Layout.Btn_LifeSkill);

        Layout.m_Privilege_Button.gameObject.SetActive(Sys_Attr.Instance.privilegeBuffIdList.Count != 0);
        if (Sys_HundredPeopleArea.Instance.IsInstance)
        {
            Layout.Btn_HundreadPeopelBuff.gameObject.SetActive(true);
            bool isBigger = Sys_HundredPeopleArea.Instance.IsBigerThanAwakeLevel(out uint awakeId, out uint awakeBufferId);
            // var csvBuff = CSVBuff.Instance.GetConfData(awakeBufferId);
            // if (csvBuff != null) {
            //     ImageHelper.SetIcon(Layout.hundredBufferIcon, csvBuff.icon);
            // }
            // 光效控制
            Layout.hundredBufferRed.SetActive(isBigger);
            Layout.hundredBufferBlue.SetActive(!isBigger);
        }
        else
        {
            Layout.Btn_HundreadPeopelBuff.gameObject.SetActive(false);
        }

        HandleTrigger();
        OnBattlePass();
        HandleFamilyResBattleBtns();
        UpdateSKillRedPoint();
        RefreshAboutMap();

        Sys_MainMenu.Instance.InitMenuRedPoint();
        ShowMenuBtn();
        OnUpdateTimeAndPowerState();
        //OnRefreshFreeDrawState(); 删除

        RefreshBlessState();
    }

    protected override void OnHide()
    {
        nUpdateCount = 0;

        ui_MiniMap?.Hide();
        ui_SpecialTask?.Hide();
        uI_MapExplorationTip?.Hide();
        IntteruptTransTipAction();
        transTimer?.Cancel();
        taskOrTeam?.OnHide();
        uI_ServerActivityMenu?.Hide();
        uI_Daily_Interface?.Hide();

        if (m_DailyLimiteTimer != null)
            m_DailyLimiteTimer.Cancel();

        OnTransTipIntterupt();

        //临时处理方案
        UIManager.CloseUI(EUIID.UI_TipsEquipment);
        UIManager.CloseUI(EUIID.UI_Family_Empowerment);
        UIManager.CloseUI(EUIID.UI_Family_DeedsLv_Popup);
    }

    protected override void OnClose()
    {
        Sys_LivingSkill.Instance.UnRegisterButton();

        if (m_DailyLimiteTimer != null)
            m_DailyLimiteTimer.Cancel();
    }

    #region NearbyNpc
    /*
    void OnNearNpcClose()
    {
        cacheNearbyNpcIDs.Clear();

        foreach (var data in nearByNpcItems)
        {
            GameObject.DestroyImmediate(data.Value.root);
        }
        nearByNpcItems.Clear();

        Layout.view_nearbyNpc.SetActive(false);
    }


    List<ulong> cacheNearbyNpcIDs = new List<ulong>(12);
    Dictionary<ulong, UI_Menu_Layout.NearByNpcItem> nearByNpcItems = new Dictionary<ulong, UI_Menu_Layout.NearByNpcItem>(11);
    //List<ulong> delsNpcUID = new List<ulong>();
    void OnNearNpc(Npc npc)
    {
        Layout.view_nearbyNpc.SetActive(true);

        if (!cacheNearbyNpcIDs.Contains(npc.uID))
        {
            GameObject nearByNpcItemGo = GameObject.Instantiate(Layout.nearbyNpcPrefab);
            nearByNpcItemGo.SetActive(true);
            UI_Menu_Layout.NearByNpcItem nearByNpcItem = new UI_Menu_Layout.NearByNpcItem(nearByNpcItemGo, npc);
            nearByNpcItem.Update();
            nearByNpcItem.root.transform.SetParent(Layout.nearbyNpcContent.transform, false);
            nearByNpcItems.Add(npc.uID, nearByNpcItem);

            cacheNearbyNpcIDs.Add(npc.uID);
        }
        else
        {
            nearByNpcItems[npc.uID].npcInfo = npc;
            nearByNpcItems[npc.uID].Update();
        }
    }

    void OnLeaveNpc(ulong npcUID)
    {
        if (cacheNearbyNpcIDs.IndexOf(npcUID) >= 0)
        {
            cacheNearbyNpcIDs.Remove(npcUID);
        }

        if (nearByNpcItems.ContainsKey(npcUID))
        {
            GameObject.DestroyImmediate(nearByNpcItems[npcUID].root);
            nearByNpcItems.Remove(npcUID);
        }

        if (cacheNearbyNpcIDs.Count == 0)
            OnNearNpcClose();
    }
    */

    void OnNearNpcClose()
    {
        for (int i = 0, len = nearByNpcItems.Count; i < len; ++i)
        {
            DestroyNearByNpcItem(nearByNpcItems[i]);
        }
        nearByNpcItems.Clear();
        Layout.view_nearbyNpc.SetActive(false);
    }

    //Dictionary<ulong, UI_Menu_Layout.NearByNpcItem> nearByNpcItems = new Dictionary<ulong, UI_Menu_Layout.NearByNpcItem>(11);
    //List<ulong> delsNpcUID = new List<ulong>();
    List<UI_Menu_Layout.NearByNpcItem> nearByNpcItems = new List<UI_Menu_Layout.NearByNpcItem>(16);
    Stack<UI_Menu_Layout.NearByNpcItem> nearByNpcItemPools = new Stack<UI_Menu_Layout.NearByNpcItem>();

    UI_Menu_Layout.NearByNpcItem CreateNearByNpcItem()
    {
        UI_Menu_Layout.NearByNpcItem item;
        if (nearByNpcItemPools.Count > 0)
        {
            item = nearByNpcItemPools.Pop();
        }
        else
        {
            GameObject nearByNpcItemGo = GameObject.Instantiate(Layout.nearbyNpcPrefab, Layout.nearbyNpcContent.transform, false);
            item = new UI_Menu_Layout.NearByNpcItem(nearByNpcItemGo);
        }
        item.root.SetActive(true);
        return item;
    }

    void DestroyNearByNpcItem(UI_Menu_Layout.NearByNpcItem item)
    {
        item.Dispose();
        nearByNpcItemPools.Push(item);
    }

    void OnNearNpcChange()
    {
        NearbyNpcSystem nearbyNpcSystem = GameCenter.mNearbyNpcSystem;
        if (nearbyNpcSystem.Count() > 0)
        {
            Layout.view_nearbyNpc.SetActive(true);

            IReadOnlyList<Npc> newNpcs = nearbyNpcSystem.GetNewNearNpc();

            for (int i = nearByNpcItems.Count - 1; i >= 0; --i)
            {
                UI_Menu_Layout.NearByNpcItem item = nearByNpcItems[i];
                if (!nearbyNpcSystem.IsNearNpc(item.npcInfo.uID))
                {
                    DestroyNearByNpcItem(item);
                    nearByNpcItems.RemoveAt(i);
                }
            }

            for (int i = 0, len = newNpcs.Count; i < len; ++i)
            {
                UI_Menu_Layout.NearByNpcItem item = CreateNearByNpcItem();
                item.SetData(newNpcs[i]);
                nearByNpcItems.Add(item);
            }
        }
        else
        {
            OnNearNpcClose();
        }
    }

    #endregion

    #region Collect

    void InitCollect()
    {
        Layout.view_Collect.gameObject.SetActive(false);
        Layout.collectIconBg.fillAmount = 0;
        Layout.collectName.text = string.Empty;

        Layout.view_Probe.gameObject.SetActive(false);
        Layout.probeProgress.fillAmount = 0;
    }

    private AsyncOperationHandle<GameObject> mHandle;

    void UpdateCollect()
    {
        if (curCSVCollectionData == null)
        {
            return;
        }

        if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Collection)
        {
            Layout.view_Collect.gameObject.SetActive(true);

            if (curCSVCollectionData.collectionAnimator != null && curCSVCollectionData.collectionAnimator.Count > 1)
            {
                ImageHelper.SetIcon(Layout.collectIcon, curCSVCollectionData.collectionAnimator[0]);
                ImageHelper.SetIcon(Layout.collectIconBg, curCSVCollectionData.collectionAnimator[1]);
            }
            TextHelper.SetText(Layout.collectName, curCSVCollectionData.collectionName);
        }
        else if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Inquiry)
        {
            Layout.view_Probe.gameObject.SetActive(true);
        }
        else
        {
            InitCollect();
        }
    }

    void OnStartCollect(CSVCollection.Data cSVCollectionData)
    {
        curCSVCollectionData = cSVCollectionData;
        Layout.view_Collect.gameObject.SetActive(true);
        Layout.collectIconBg.fillAmount = 0;

        if (curCSVCollectionData.collectionAnimator != null && curCSVCollectionData.collectionAnimator.Count > 1)
        {
            ImageHelper.SetIcon(Layout.collectIcon, curCSVCollectionData.collectionAnimator[0]);
            ImageHelper.SetIcon(Layout.collectIconBg, curCSVCollectionData.collectionAnimator[1]);
        }

        TextHelper.SetText(Layout.collectName, cSVCollectionData.collectionName);

        float allTime = curCSVCollectionData.collectionProgress / 1000f;
        if (allTime == 0)
        {
            Layout.view_Collect.gameObject.SetActive(false);
            return;
        }
        collectTimer?.Cancel();
        collectTimer = Timer.Register(allTime, null, (curTime) =>
        {
            if (Layout.collectIconBg != null)
                Layout.collectIconBg.fillAmount = (curTime / allTime);
        }, false, true);
    }

    void OnStartInquiry(CSVDetect.Data cSVDetectData)
    {
        curCSVDetectData = cSVDetectData;
        Layout.view_Probe.gameObject.SetActive(true);
        Layout.probeProgress.fillAmount = 0;

        float allTime = curCSVDetectData.duration / 1000f;
        probeTimer?.Cancel();
        probeTimer = Timer.Register(allTime, () =>
        {
            if (Layout.view_Probe != null)
                Layout.view_Probe.gameObject.SetActive(false);
        }, (curTime) =>
        {
            if (Layout.probeProgress != null)
                Layout.probeProgress.fillAmount = 1 - (curTime / allTime);
        }, false, true);
    }

    void OnInquiryCompleted()
    {
        InitCollect();
    }

    void OnCollectEnded(ulong uid)
    {
        InitCollect();
    }

    void OnCollectFaild()
    {
        InitCollect();
    }

    void OnInterrputInquiry()
    {
        probeTimer?.Cancel();
        InitCollect();
    }

    void OnInterrputCollect()
    {
        collectTimer?.Cancel();
        InitCollect();
    }

    void OnAddExp()
    {
        Layout.roleLv.text = Sys_Role.Instance.Role.Level.ToString();
        SetExp();
    }

    private void OnUpdateExp()
    {
        if (Sys_Pet.Instance.fightPet.HasFightPet())
        {
            SimplePet petUnit = Sys_Pet.Instance.fightPet.GetSimplePet();
            if (curlevel != petUnit.Level)
            {
                Layout.petLv.text = petUnit.Level.ToString();
                curlevel = petUnit.Level;
                if (Layout.petlevelupfx.activeInHierarchy)
                {
                    Layout.petlevelupfx.SetActive(false);
                }
                Layout.petlevelupfx.SetActive(true);
            }
        }
    }

    private void OnRoleUpdateAttr()
    {
        Layout.roleMagic.value = (float)Sys_Attr.Instance.curMp / Sys_Attr.Instance.pkAttrs[17];
        Layout.roleLife.value = (float)Sys_Attr.Instance.curHp / Sys_Attr.Instance.pkAttrs[15];
    }

    private void OnPetUpdateAttr()
    {
        if (!Sys_Pet.Instance.fightPet.HasFightPet())
            return;
        FightSimplePet petUnit = Sys_Pet.Instance.fightPet;
        Layout.petLife.value = petUnit.GetHpSliderValue();
        Layout.petMagic.value = petUnit.GetHpSliderValue();
    }

    private void OnTransTipStart(uint npcId)
    {
        transTimer?.Cancel();
        transTimer = null;
        ui_Convey?.Show();
        ui_Convey?.StartTip(npcId);
        transTimer = Timer.Register(transAllTime, OnExcuteTelNpc, OnTransUpdate);
    }

    private void OnExcuteTelNpc()
    {
        ui_Convey?.Hide();
        Sys_Map.Instance.ExcuteTelNpc();
    }

    private void OnTransUpdate(float time)
    {
        float progress = time / transAllTime;
        ui_Convey?.SetProgress(progress);
    }

    private void OnTransTipIntterupt()
    {
        IntteruptTransTipAction();
        transTimer?.Cancel();
        transTimer = null;
        ui_Convey?.Hide();
    }

    private void IntteruptTransTipAction()
    {
        if (transTimer != null && !transTimer.isCompleted) {
            ActionCtrl.Instance.InterruptCurrent();

            if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                Sys_Pet.Instance.ForceStop();
            }
        }
    }

    private void OnAddPet()
    {
        if (Layout.petShow.gameObject.activeInHierarchy)
        {
            Layout.petAddPointRedPoint.SetActive(Sys_Pet.Instance.IsHasFightPetPointNotUse());
            return;
        }
        else
        {
            Layout.petShow.gameObject.SetActive(true);
            if (!Sys_Pet.Instance.fightPet.HasFightPet())
            {
                Layout.petIcon.gameObject.SetActive(false);
                Layout.nopetGo.gameObject.SetActive(true);
                Layout.petLvGo.gameObject.SetActive(false);
                Layout.petName.gameObject.SetActive(false);
                Layout.petLv.gameObject.SetActive(false);
                Layout.petLife.gameObject.SetActive(false);
                Layout.petMagic.gameObject.SetActive(false);
                Layout.petLifeGo.gameObject.SetActive(false);
                Layout.petMagicGo.gameObject.SetActive(false);
                ImageHelper.SetIcon(Layout.petIcon, 993301);
                Layout.petAddPointRedPoint.SetActive(false);
            }
            else
            {
                Layout.petIcon.gameObject.SetActive(true);
                Layout.nopetGo.gameObject.SetActive(false);
                Layout.petLvGo.gameObject.SetActive(true);
                Layout.petLv.gameObject.SetActive(true);
                Layout.petLife.gameObject.SetActive(true);
                Layout.petMagic.gameObject.SetActive(true);
                Layout.petLifeGo.gameObject.SetActive(true);
                Layout.petMagicGo.gameObject.SetActive(true);

                SetFightPetData();
            }
        }
    }

    private void SetFightPetData()
    {
        FightSimplePet petUnit = Sys_Pet.Instance.fightPet;
        SimplePet simplePet = petUnit.GetSimplePet();
        CSVPetNew.Data petdate = CSVPetNew.Instance.GetConfData(simplePet.PetId);
        ImageHelper.SetIcon(Layout.petIcon, petdate.icon_id);
        if (simplePet.Name.IsEmpty)
            Layout.petName.text = LanguageHelper.GetTextContent(petdate.name);
        else
            Layout.petName.text = simplePet.Name.ToStringUtf8();
        Layout.petLv.text = simplePet.Level.ToString();
        curlevel = simplePet.Level;
        Layout.petLife.value = petUnit.GetHpSliderValue();
        Layout.petMagic.value = petUnit.GetMpSliderValue();
        Layout.petAddPointRedPoint.SetActive(Sys_Pet.Instance.IsHasFightPetPointNotUse());
    }

    private void OnAddFightPet()
    {
        Layout.petShow.gameObject.SetActive(true);
        Layout.petIcon.gameObject.SetActive(true);
        Layout.nopetGo.gameObject.SetActive(false);
        Layout.petName.gameObject.SetActive(true);
        Layout.petLv.gameObject.SetActive(true);
        Layout.petLife.gameObject.SetActive(true);
        Layout.petMagic.gameObject.SetActive(true);
        Layout.petLifeGo.gameObject.SetActive(true);
        Layout.petMagicGo.gameObject.SetActive(true);
        SetFightPetData();
    }

    private void ShowOrHideTemplePetBag(int count)
    {
        bool show = count > 0;
        Layout.Btn_TempPetBag.gameObject.SetActive(show);
        if (show)
        {
            GameObject fullGo = Layout.Btn_TempPetBag.transform.Find("Image_Full").gameObject;
            CSVPetNewParam.Data maxCount = CSVPetNewParam.Instance.GetConfData(10u);
            fullGo.SetActive(null != maxCount && count >= maxCount.value);
        }
    }

    private void OnFunctionOpen(Sys_FunctionOpen.FunctionOpenData data)
    {
        if (!Sys_FamilyResBattle.Instance.InFamilyBattle)
        {
            CheckProbeSkillOpen();
        }
    }

    private void OnEndEnter(uint _, int __)
    {
        HandleFamilyResBattleBtns();
    }

    private void CheckProbeSkillOpen()
    {
        bool isOpen = Sys_FunctionOpen.Instance.IsOpen(20501, false);
        Layout.Btn_Look.enabled = isOpen;
        ImageHelper.SetImageGray(Layout.Btn_Look.transform.Find("Image_Icon_01").GetComponent<Image>(), !isOpen, !isOpen);
        Layout.Btn_Look.transform.Find("Image_Lock").gameObject.SetActive(!isOpen);
        Layout.Btn_Look.transform.Find("Image_Icon").gameObject.SetActive(isOpen);
        isOpen = Sys_FunctionOpen.Instance.IsOpen(20502, false);
        Layout.Btn_Old.enabled = isOpen;
        ImageHelper.SetImageGray(Layout.Btn_Old.transform.Find("Image_Icon_01").GetComponent<Image>(), !isOpen, !isOpen);
        Layout.Btn_Old.transform.Find("Image_Lock").gameObject.SetActive(!isOpen);
        Layout.Btn_Old.transform.Find("Image_Icon").gameObject.SetActive(isOpen);
        isOpen = Sys_FunctionOpen.Instance.IsOpen(20503, false);
        Layout.Btn_Eye.enabled = isOpen;
        ImageHelper.SetImageGray(Layout.Btn_Eye.transform.Find("Image_Icon_01").GetComponent<Image>(), !isOpen, !isOpen);
        Layout.Btn_Eye.transform.Find("Image_Lock").gameObject.SetActive(!isOpen);
        Layout.Btn_Eye.transform.Find("Image_Icon").gameObject.SetActive(isOpen);

        Layout.Go_NormalMenu.SetActive(true);
        Layout.Go_ProbeMenu.SetActive(false);
    }

    void SetExp()
    {
        CSVCharacterAttribute.Data data = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level + 1);
        if (Sys_Role.Instance.Role != null && data != null)
        {
            if (Sys_Role.Instance.Role.ExtraExp == 0)
            {
                Layout.exp.fillAmount = (float)Sys_Role.Instance.Role.Exp / data.upgrade_exp;
            }
            else
            {
                Layout.exp.fillAmount = 0;
            }
            uint.TryParse(CSVParam.Instance.GetConfData(968).str_value, out uint compensationExpPercent);
        }
    }

    #endregion

    #region 任务/组队
    public bool isTaskExpand { get; set; } = true;
    private void TryLoadTaskOrTeam()
    {
        if (taskOrTeam == null)
        {
            taskOrTeam = new UI_TaskOrTeamMain();
            taskOrTeam.SetUI(this);
            taskOrTeam.Init(Layout.TaskOrTeam);
        }
        TryRefresh();
    }
    private void TryRefresh()
    {
        if (taskOrTeam != null)
        {
            if (isTaskExpand)
            {
                taskOrTeam.SetMode();
                Layout.Btn_Task.gameObject.SetActive(false);
                taskOrTeam.Show();
                taskOrTeam.FunctionState();
            }
            else
            {
                if (Sys_HundredPeopleArea.Instance.IsInstance)
                {
                    OnTaskOrTeam_ButtonClicked();
                }
            }
        }
    }
    #endregion

    void UpdateSKillRedPoint()
    {
        if (Sys_Skill.Instance.ExistedLearnShowTipSkill())
        {
            Layout.Btn_SkillLearn.gameObject.FindChildByName("Image_Dot").SetActive(true);
        }
        else
        {
            Layout.Btn_SkillLearn.gameObject.FindChildByName("Image_Dot").SetActive(false);
        }
    }

    void OnSubmited(int menuId, uint id, TaskEntry taskEntry)
    {
        UpdateSKillRedPoint();
    }

    #region Uplifted
    private void OnUpdateLevel()
    {
        UpdateSKillRedPoint();
        CheckNeedShowUplifted();
    }

    private void OnRefreshChangeData(int changeType, int curBoxId)
    {
        CheckNeedShowUplifted();
    }

    private void OnTaskStatusChanged(TaskEntry arg1, ETaskState arg2, ETaskState arg3)
    {
        CheckNeedShowUplifted();
    }

    private void OnUpdateAttr()
    {
        CheckNeedShowUplifted();
    }

    private void UpdateAttrCallBack()
    {
        CheckNeedShowUplifted();
    }

    private void OnUpdatePetUplift()
    {
        CSVUplifted.Data data = CSVUplifted.Instance.GetConfData(3);
        if (Sys_Uplifted.Instance.upliftedClosedDic.TryGetValue(data.id, out UpliftOpenClose openClose) && openClose != null && openClose.isClose)
        {
            for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
            {
                uint total =  Sys_Pet.Instance.petsList[i].petUnit.PetPointPlanData.TotalPoint;
                uint index = Sys_Pet.Instance.petsList[i].petUnit.PetPointPlanData.CurrentPlanIndex;
                uint use = Sys_Pet.Instance.petsList[i].petUnit.PetPointPlanData.Plans[(int)index].UsePoint;
                if ((total- use) >= data.Parameter)
                {
                    Layout.Btn_Uplifted.gameObject.SetActive(true);
                    return;
                }
            }
        }
    }

    private void OnHpMpPoolUpdate()
    {
        CheckNeedShowUplifted();
    }

    private void OnExperienceUpgrade()
    {
        CheckNeedShowUplifted();

    }

    private void OnUpdateExperienceInfo()
    {
        CheckNeedShowUplifted();
    }

    private void OnCloseUpliftItem()
    {
        CheckNeedShowUplifted();
    }

    private void UpgradeBuildSkill()
    {
        CheckNeedShowUplifted();
    }

    private void OnRefreshNextDayUpliftItem()
    {
        CheckNeedShowUplifted();
    }

    private void UpdateLevelUpButtonState(uint arg)
    {
        CheckNeedShowUplifted();
    }

    #endregion

    protected override void OnDestroy()
    {
        taskOrTeam?.OnDestroy();
        taskOrTeam = null;
        ui_MiniMap?.OnDestroy();
        uI_MapExplorationTip?.OnDestroy();
        ui_SpecialTask?.OnDestroy();
    }

    protected override void OnUpdate()
    {
        ++nUpdateCount;

        //OnUpdateFPS();
        //每隔6个逻辑帧刷新 200ms
        int v1 = nUpdateCount % 5;
        if (v1 == 1)
        {
            ui_MiniMap?.ExecUpdate();
        }
        else if (v1 == 2)
        {
            //ui_ComTip?.OnUpdate();
            uI_ServerActivityMenu?.OnRefreshBtnActivitys();
        }
        else if (v1 == 3)
        {
            CheckNeedRepairEequipment();
        }

        //每隔30个逻辑帧刷新 1000ms
        int v2 = nUpdateCount % 30;
        if (v2 == 4)
        {
            OnUpdateDaySlider();
        }
        else if (v2 == 5)
        {
            OnMessageBagButtonShow();
        }
        else if (v2 == 6)
        {
            if (Sys_SurvivalPvp.Instance.isMatching)
            {
                RefreshSurivalMatchTime();
            }
        }

        //每隔300个逻辑帧刷新 10000ms
        if (nUpdateCount % 300 == 0)
        {
            OnUpdateTimeAndPowerState();
        }
    }

    protected void OnUpdateFPS()
    {
        //if (!OptionManager.Instance.mShowFPS.Get())
        //    return;
        //
        //if (nLastRecodeFPS != Framework.TimeManager.FPS)
        //{
        //    nLastRecodeFPS = Framework.TimeManager.FPS;
        //    RefreshFPS();
        //}
    }
    public void OnMessageBagButtonShow()
    {
        Layout.Btn_MessageBag.gameObject.SetActive(Sys_MessageBag.Instance.IsMessageButtonShow());
        Layout.messageBagRedPoint.SetActive(!Sys_MessageBag.Instance.isClick);
        Sys_MessageBag.Instance.MessageBagButtonShow(Layout.countText, Layout.m_TextMessageBag);
    }

    protected void OnPingNtf(int pingValue)
    {
        if (pingValue < 250)
        {
            Layout.wifiObj[2].SetActive(true);
            Layout.wifiObj[1].SetActive(true);
            Layout.wifiObj[0].SetActive(true);
        }
        else if (pingValue < 800)
        {
            Layout.wifiObj[2].SetActive(false);
            Layout.wifiObj[1].SetActive(true);
            Layout.wifiObj[0].SetActive(true);
        }
        else if (pingValue < 1500)
        {
            Layout.wifiObj[2].SetActive(false);
            Layout.wifiObj[1].SetActive(false);
            Layout.wifiObj[0].SetActive(true);
        }
        else
        {
            Layout.wifiObj[2].SetActive(false);
            Layout.wifiObj[1].SetActive(false);
            Layout.wifiObj[0].SetActive(false);
        }
    }

    protected void OnUpdateTimeAndPowerState()
    {
        Layout.timeText.text = System.DateTime.Now.ToString("HH:mm");
        Layout.power.value = GetBatteryLevel() * 0.01f;
    }

    public int GetBatteryLevel()
    {
        int leftBattery = 100;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        if (SDKManager.sdk.IsHaveSdk)
            leftBattery = SDKManager.GetBatteryLevel();
#endif
        return leftBattery;
    }


    protected void OnUpdateDaySlider()
    {
        Layout.Image_Circle.fillAmount = 1 - (float)Sys_Weather.Instance.GetNextDayOrNightTime() / onedaytime;
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnAddExp, OnAddExp, toRegister);
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnExtraExp, OnAddExp, toRegister);

        Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnTransTipStart, OnTransTipStart, toRegister);
        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnTransTipIntterupt, OnTransTipIntterupt, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnAddPet, OnAddPet, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnAddFightPet, OnAddFightPet, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnAddPet, toRegister);

        Sys_Exploration.Instance.eventEmitter.Handle(Sys_Exploration.EEvents.ExplorationRewardNotice, CheckExplorationRewardTip, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_ApplyListOpRes, OnTeamRedInfo, toRegister);
        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_InfoNtf, OnTeamRedInfo, toRegister);
        Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnEnemySwitch, RefreshEnemySwitch, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle<int, int>(Sys_Pet.EEvents.OnPatrolStateChange, OnEnemySwitch, toRegister);
        Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnUsingUpdate, OnUsingUpdate, toRegister);
        Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnAddUpdate, CheckChangeHeadRedpoint, toRegister);
        Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnExpritedUpdate, CheckChangeHeadRedpoint, toRegister);
        Sys_Attr.Instance.eventEmitter.Handle<uint, uint>(Sys_Attr.EEvents.OnPrivilegeBuffUpdate, OnPrivilegeBuffUpdate, toRegister);
        Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, toRegister);
        Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateOpenServiceDay, CheckChangeHeadRedpoint, toRegister);

        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnBeginEnter, toRegister);
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnUIEndExit, toRegister);

        Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.ActivityState, OnBattlePass, toRegister);

        Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.RedState, OnBattlePass, toRegister);

        Sys_MainMenu.Instance.eventEmitter.Handle(Sys_MainMenu.EEvents.OnRefreshMenuRedPoint, RefreshMenuRedPoint, toRegister);
    }

    protected override void ProcessEvents(bool toRegister)
    {
        CollectionAction.eventEmitter.Handle<CSVCollection.Data>(CollectionAction.EEvents.StartCollect, OnStartCollect, toRegister);
        Sys_CollectItem.Instance.eventEmitter.Handle<ulong>(Sys_CollectItem.EEvents.OnCollectEnded, OnCollectEnded, toRegister);
        Sys_CollectItem.Instance.eventEmitter.Handle(Sys_CollectItem.EEvents.OnCollectFaild, OnCollectFaild, toRegister);
        CollectionAction.eventEmitter.Handle(CollectionAction.EEvents.InterrputCollect, OnInterrputCollect, toRegister);
        InquiryAction.eventEmitter.Handle<CSVDetect.Data>(InquiryAction.EEvents.StartInquiry, OnStartInquiry, toRegister);
        InquiryAction.eventEmitter.Handle(InquiryAction.EEvents.InquiryCompleted, OnInquiryCompleted, toRegister);
        InquiryAction.eventEmitter.Handle(InquiryAction.EEvents.InterrputInquiry, OnInterrputInquiry, toRegister);
        Sys_Bag.Instance.eventEmitter.Handle<bool>(Sys_Bag.EEvents.OnShowOrHideMenuTempBagIcon, OnShowOrHideTempBagIcon, toRegister);
        Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnPlayMenuBagAnim, OnBagAnimatorPlay, toRegister);
        Sys_Bag.Instance.eventEmitter.Handle<bool>(Sys_Bag.EEvents.OnRefreshBagFull, OnRefreshBagFull, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateExp, OnUpdateExp, toRegister);
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnRoleHpMpUpdate, OnRoleUpdateAttr, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetHpMpUpdate, OnPetUpdateAttr, toRegister);
        Sys_Net.Instance.eventEmitter.Handle<int>(Sys_Net.EEvents.OnPingNtf, OnPingNtf, toRegister);

        Sys_Pet.Instance.eventEmitter.Handle<int>(Sys_Pet.EEvents.OnTemplePetBagChange, ShowOrHideTemplePetBag, toRegister);
        Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnFunctionOpen, toRegister);
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndEnter, OnEndEnter, toRegister);

        Sys_Fight.Instance.eventEmitter.Handle(Sys_Fight.EEvents.StartReConnect, OnStartReConnect, toRegister);
        Sys_Fight.Instance.eventEmitter.Handle(Sys_Fight.EEvents.Reconnected, OnReconnected, toRegister);

        Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnNearNpcChange, OnNearNpcChange, toRegister);
        //Sys_Npc.Instance.eventEmitter.Handle<ulong>(Sys_Npc.EEvents.OnLeaveNpc, OnLeaveNpc, toRegister);
        //Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnNearNpcClose, OnNearNpcClose, toRegister);

        Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toRegister);
        Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, toRegister);
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, UpdateAttrCallBack, toRegister);
        Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnGetPetInfoForUplift, OnUpdatePetUplift, toRegister);
        Sys_LivingSkill.Instance.eventEmitter.Handle<uint>(Sys_LivingSkill.EEvents.OnLevelUp, UpdateLevelUpButtonState, toRegister);

        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnHpMpPoolUpdate, OnHpMpPoolUpdate, toRegister);
        Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnUpdateExperienceInfo, OnUpdateExperienceInfo, toRegister);
        Sys_Experience.Instance.eventEmitter.Handle(Sys_Experience.EEvents.OnExperienceUpgrade, OnExperienceUpgrade, toRegister);
        Sys_Uplifted.Instance.eventEmitter.Handle(Sys_Uplifted.EEvents.OnCloseUpliftItem, OnCloseUpliftItem, toRegister);
        Sys_Uplifted.Instance.eventEmitter.Handle(Sys_Uplifted.EEvents.OnRefreshNextDayUpliftItem, OnRefreshNextDayUpliftItem, toRegister);
        Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpgradeBuildSkill, UpgradeBuildSkill, toRegister);
        Sys_ElementalCrystal.Instance.eventEmitter.Handle<bool>(Sys_ElementalCrystal.EEvents.OnSetActiveMenuCrystal, SetActiveBtnCrystal, toRegister);

        Sys_Daily.Instance.eventEmitter.Handle(Sys_Daily.EEvents.NewNotice, OnNewDailyLimite, toRegister);
        Sys_Daily.Instance.eventEmitter.Handle(Sys_Daily.EEvents.RemoveNotice, OnNewDailyLimite, toRegister);

        Sys_Inquiry.eventEmitter.Handle(Sys_Inquiry.EEvents.EnterAnyInquiryArea, EnterAnyInquiryArea, toRegister);
        Sys_Inquiry.eventEmitter.Handle(Sys_Inquiry.EEvents.ExitAllNoInquiryArea, ExitAllNoInquiryArea, toRegister);

        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, onLoadMapOk, toRegister);
        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, onLoadMapOk, toRegister);
        Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnReadMapMail, OnReadMapTip, toRegister);
        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnMapBitChanged, this.RefreshAboutMap, toRegister);

        Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.MatchPanleOpen, OnSurvivalPvpMatch, toRegister);
        Sys_SurvivalPvp.Instance.eventEmitter.Handle(Sys_SurvivalPvp.EEvents.MatchCancle, OnSurvivalPvpMatchCancle, toRegister);
        Sys_MessageBag.Instance.eventEmitter.Handle<int>(Sys_MessageBag.EEvents.OnButtonShow, OnMessageBag, toRegister);

        Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnReceiveRedPacket, UpdateFamilyRedPacket, toRegister);

        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, OnSubmited, toRegister);
        Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnGetTitle, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<bool, bool>(Sys_FamilyResBattle.EEvents.OnSignupChanged, OnSignupChanged, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<uint, uint, long>(Sys_FamilyResBattle.EEvents.OnStageChanged, OnStageChanged, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnReceiveSignupNtf, OnReceiveSignupNtf, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnSignupSettingChanged, OnSignupSettingChanged, toRegister);
        Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnReName, OnReNameUpdate, toRegister);

        Sys_Blessing.Instance.eventEmitter.Handle(Sys_Blessing.EEvents.InfoRefresh, RefreshBlessState, toRegister);
        Sys_Blessing.Instance.eventEmitter.Handle(Sys_Blessing.EEvents.TakeAward, RefreshBlessState, toRegister);
    }

    private void SetIconMessage()
    {
        Layout.petlevelupfx.SetActive(false);
        Layout.roleLv.text = Sys_Role.Instance.Role.Level.ToString();
        Layout.roleName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
        Layout.roleMagic.value = (float)Sys_Attr.Instance.curMp / Sys_Attr.Instance.pkAttrs[17];
        Layout.roleLife.value = (float)Sys_Attr.Instance.curHp / Sys_Attr.Instance.pkAttrs[15];
        Sys_Head.Instance.SetHeadAndFrameData(Layout.roleIcon);
        bool isopen = Sys_FunctionOpen.Instance.IsOpen(10501, false);
        Layout.petShow.gameObject.SetActive(isopen);
        Layout.petIcon.gameObject.SetActive(isopen);
        if (!Sys_Pet.Instance.fightPet.HasFightPet())
        {
            Layout.petIcon.gameObject.SetActive(false);
            Layout.nopetGo.gameObject.SetActive(true);
            Layout.petLvGo.gameObject.SetActive(false);
            Layout.petName.gameObject.SetActive(false);
            Layout.petLv.gameObject.SetActive(false);
            Layout.petLife.gameObject.SetActive(false);
            Layout.petLifeGo.gameObject.SetActive(false);
            Layout.petMagicGo.gameObject.SetActive(false);
            Layout.petMagic.gameObject.SetActive(false);
            ImageHelper.SetIcon(Layout.petIcon, 993301);
            Layout.petAddPointRedPoint.SetActive(false);
        }
        else
        {
            Layout.petIcon.gameObject.SetActive(true);
            Layout.nopetGo.gameObject.SetActive(false);
            Layout.petLvGo.gameObject.SetActive(true);
            Layout.petName.gameObject.SetActive(true);
            Layout.petLv.gameObject.SetActive(true);
            Layout.petLife.gameObject.SetActive(true);
            Layout.petMagic.gameObject.SetActive(true);
            Layout.petLifeGo.gameObject.SetActive(true);
            Layout.petMagicGo.gameObject.SetActive(true);
            SetFightPetData();
        }

        Layout.Grid01Go.gameObject.SetActive(true);
        Layout.probebuttonGo.gameObject.SetActive(false);
        OnRefreshBagFull(Sys_Bag.Instance.bAnyMainBagFull);
        CheckChangeHeadRedpoint();
    }

    private void CheckChangeHeadRedpoint()
    {
        Layout.changeHeadRedPoint.SetActive(Sys_Head.Instance.CheckShowRedPoint() || Sys_Title.Instance.Red()|| Sys_Attr.Instance.CheckWorldLevelChanged());
    }

    private void OnGetTitle(uint titleId)
    {
        CheckChangeHeadRedpoint();
    }

    private void OnSignupChanged(bool old, bool newSignup)
    {
        this.HandleFamilyResBattleBtns();
    }

    private void OnStageChanged(uint oldStage, uint newStage, long endOfStageTime)
    {
        this.HandleFamilyResBattleBtns();
    }

    private void OnReceiveSignupNtf()
    {
        this.HandleFamilyResBattleBtns();
    }

    private void OnSignupSettingChanged()
    {
        this.HandleFamilyResBattleBtns();
    }
    private void OnReNameUpdate()
    {
        Layout.roleName.text =Sys_Role.Instance.Role.Name.ToStringUtf8();
    }
    private void CheckNeedRepairEequipment()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(10304, false)) { return; }

        bool isNeed = Sys_Equip.Instance.IsNeedRepair();
        Layout.Btn_Repair.gameObject.SetActive(isNeed);
    }

    private void CheckNeedShowUplifted()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(50501, false))
        {
            return;
        }
        int countNow = Sys_Uplifted.Instance.CheckupliftedIsShow();
        Layout.Btn_Uplifted.gameObject.SetActive(countNow > 0);
        if (Sys_Uplifted.Instance.isFxShow)
        {
            Sys_Uplifted.Instance.isFxShow = countNow >= Sys_Uplifted.Instance.countBefore;
        }
        else
        {
            Sys_Uplifted.Instance.isFxShow = countNow > Sys_Uplifted.Instance.countBefore;
        }
        Sys_Uplifted.Instance.countBefore = countNow;
        Layout.Go_Uplifted_Fx.gameObject.SetActive(Sys_Uplifted.Instance.isFxShow);
    }

    private void CheckExplorationRewardTip()
    {
        int count = Sys_Exploration.Instance.list_ExplorationRewardNotice.Count;
        ImageHelper.SetIcon(Layout.explorationIcon, count < 2 ? (uint)993601 : 993602);
        Layout.Btn_ExplorationTip.gameObject.SetActive(count > 0);
    }

    private void EnterAnyInquiryArea()
    {
        m_IsEnterNpcLook = true;
        CheckInquiryArea(m_IsEnterNpcLook);
    }

    private void ExitAllNoInquiryArea()
    {
        m_IsEnterNpcLook = false;
        CheckInquiryArea(m_IsEnterNpcLook);
    }


    private void OnPrivilegeBuffUpdate(uint id, uint op)
    {
        Layout.m_Privilege_Button.gameObject.SetActive(Sys_Attr.Instance.privilegeBuffIdList.Count != 0);
    }

    private void OnDayNightChange()
    {
        if (Sys_Weather.Instance.isDay)
        {
            ImageHelper.SetIcon(Layout.Image_Weather_DayOrNight, 990702);
            ImageHelper.SetIcon(Layout.Image_Circle, 990704);
        }
        else
        {
            ImageHelper.SetIcon(Layout.Image_Weather_DayOrNight, 990701);
            ImageHelper.SetIcon(Layout.Image_Circle, 990703);
        }
    }

    private void CheckInquiryArea(bool isEnter)
    {
        Animator animator = Layout.Btn_Look.transform.Find("Image_Icon").GetComponent<Animator>();
        if (isEnter)
        {
            Layout.Btn_Look_Effect.gameObject.SetActive(true);
            if (animator.enabled && Layout.Go_ProbeMenu.activeInHierarchy)
            {
                animator.Play("Look", -1, 0);
            }
        }
        else
        {
            Layout.Btn_Look_Effect.gameObject.SetActive(false);
            if (animator.enabled)
            {
                animator.Update(0);
            }
        }
    }

    private void OnTeamRedInfo()
    {
        Layout.m_TransTeamRed.gameObject.SetActive(Sys_Team.Instance.ApplyRolesCount > 0);
    }

    private void HandleTrigger()
    {
        Sys_Hangup.Instance.PendingAction?.Invoke();
        Sys_Hangup.Instance.PendingAction = null;

        Sys_WorldBoss.Instance.PendingAction?.Invoke();
        Sys_WorldBoss.Instance.PendingAction = null;

        Sys_FamilyResBattle.Instance.PendingActionForRule?.Invoke();
        Sys_FamilyResBattle.Instance.PendingActionForRule = null;

        Sys_FamilyResBattle.Instance.pendingForOpenResult?.Invoke();
        Sys_FamilyResBattle.Instance.pendingForOpenResult = null;

        Sys_FamilyResBattle.Instance.TryOpenEnterMsgBox();
    }

    private void RefreshAboutMap()
    {
        // 刷新数字
        int mailCount = Sys_Map.Instance.unReadedMapMails.Count;
        if (mailCount > 0)
        {
            this.Layout.mapMailGo.gameObject.SetActive(true);
            this.Layout.mapMailCountText.text = mailCount.ToString();
        }
        else
        {
            this.Layout.mapMailGo.gameObject.SetActive(false);
        }
    }

    private void HandleFamilyResBattleBtns()
    {
        if (Sys_FamilyResBattle.Instance.InFamilyBattle)
        {
            Layout.familyResBattleHider.ShowHideBySetActive(false);
        }

        bool toShow = Sys_FamilyResBattle.Instance.chooseSignupSetting;
        toShow &= (Sys_FamilyResBattle.Instance.PendingAction != null);
        toShow &= (!Sys_FamilyResBattle.Instance.hasSigned);
        toShow &= (Sys_FamilyResBattle.Instance.stage == Sys_FamilyResBattle.EStage.Signup);
        this.Layout.Button_FamilyEesBattleSignupTip.gameObject.SetActive(toShow);
    }

    private void OnBattlePass()
    {
        if (Layout.m_BtnBattlePass.gameObject.activeSelf != Sys_BattlePass.Instance.isActive)
            Layout.m_BtnBattlePass.gameObject.SetActive(Sys_BattlePass.Instance.isActive);

        Layout.m_TransBattlePassRedDot.gameObject.SetActive(Sys_BattlePass.Instance.IsRedDotActive());
    }

    private void RefreshBlessState()
    {
        if (Layout.m_BtnBless.gameObject.activeSelf != Sys_Blessing.Instance.IsActive)
            Layout.m_BtnBless.gameObject.SetActive(Sys_Blessing.Instance.IsActive);

        Layout.m_TransBlessRedDot.gameObject.SetActive(Sys_Blessing.Instance.HaveReward());
        

    }
    #region ButtonClicked

    public void OnClose_ButtonClicked()
    {
        //animator01.enabled = true;
        //animator02.enabled = true;
        //animator01.Play("Close", -1, 0);
        //animator02.Play("Close", -1, 0);
        //time?.Cancel();
        //time = Timer.Register(0.11f, () =>
        //{
        //    Layout.buttonGo.SetActive(false);
        //    Layout.Btn_Change.gameObject.SetActive(false);
        //    Layout.Btn_Open.gameObject.SetActive(true);
        //}, null, false, true);
        UIManager.CloseUI(EUIID.UI_MainMenu);
    }

    public void OnProbeClose_ButtonClicked()
    {
        animator03.enabled = true;
        animator03.Play("Close", -1, 0);
        time?.Cancel();
        time = Timer.Register(0.11f, () =>
        {
            Layout.buttonGo.SetActive(false);
            Layout.Btn_Change.gameObject.SetActive(false);
            Layout.Btn_Open.gameObject.SetActive(true);
        }, null, false, true);

    }

    public void OnOpen_ButtonClicked()
    {
        //animator01.enabled = true;
        //animator02.enabled = true;
        //Layout.buttonGo.SetActive(true);
        //Layout.Btn_Change.gameObject.SetActive(true);
        //Layout.Btn_Open.gameObject.SetActive(false);
        //animator01.Play("Open", -1, 0);
        //animator02.Play("Open", -1, 0);
        UIManager.OpenUI(EUIID.UI_MainMenu);
    }

    public void OnBag_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Bag,false,1);
    }

    public void OnSafeBox_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_SafeBox);
    }

    public void OnPartner_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Partner);
    }

    public void OnButtonClicked_FamilyEesBattleSignupTip()
    {
        UIManager.OpenUI(EUIID.UI_Family, false, new UI_FamilyOpenParam()
        {
            familyMenuEnum = (uint)UI_Family.EFamilyMenu.FamilyPVP,
        });
        UIManager.OpenUI(EUIID.UI_FamilyResBattleMain, false, 1u);
    }

    public void OnSkill_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_SkillUpgrade);
    }

    public void OnEquip_ButtonClicked()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(10300, true))
            return;

        UIManager.OpenUI(EUIID.UI_Equipment);
    }

    public void OnBtn_ClueTask_ButtonClicked()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(EFuncOpen.FO_ClueTask, true)) { return; }
        UIManager.OpenUI(EUIID.UI_ClueTaskMain, true);
    }

    public void OnRole_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Career);
    }

    public void OnHeader_ButtonClicked()
    {
        if (Sys_FunctionOpen.Instance.IsOpen(10101, false))
            UIManager.OpenUI(EUIID.UI_Attribute);
    }

    public void OnTaskOrTeam_ButtonClicked()
    {
        isTaskExpand = true;
        TryLoadTaskOrTeam();
    }

    public void OnTempBag_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_TemporaryBag);
    }

    public void OnTreasure_ButtonCliked()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(22101, true)) { return; }
        UIManager.OpenUI(EUIID.UI_Treasure);
    }

    public void OnShowOrHideTempBagIcon(bool flag)
    {
        Layout.Btn_TempBag.gameObject.SetActive(flag);
    }

    public void OnBagAnimatorPlay()
    {
        Animator animator = Layout.Btn_Bag.GetComponent<Animator>();
        animator.enabled = true;
        animator.Play("Open", -1, 0);
    }

    public void OnRefreshBagFull(bool full)
    {
        GameObject gameObject = Layout.Btn_Bag.transform.Find("Image_Full").gameObject;
        gameObject.SetActive(full);
    }

    public void OnBtn_NewPet_ButtonClicked()
    {
        Sys_Pet.Instance.OnGetPetInfoReq(null, EPetUiType.UI_Message);
    }


    public void OnBtn_Multi_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Multi_PlayType);
    }

    public void OnBtn_Single_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Onedungeons);
    }

    public void OnBtn_AreaProtection_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_AreaProtection);
    }
    public void OnBtn_Favorability_ButtonClicked()
    {
        // 组队跟随的时候让点开吗？
        if (/*!Sys_Team.Instance.canManualOperate || */Sys_FunctionOpen.Instance.IsOpen(EFuncOpen.FO_NpcFavorability, true))
        {
            UIManager.OpenUI(EUIID.UI_FavorabilityMain);
        }
    }

    public void OnBtn_Family_ButtonClicked()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(30401, true)) return;

        Sys_Family.Instance.OpenUI_Family();
    }

    public void OnBtn_Repair_ButtonClicked()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(10304, true))
            return;

        Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
        data.curEquip = null;
        data.opType = Sys_Equip.EquipmentOperations.Repair;
        UIManager.OpenUI(EUIID.UI_Equipment, false, data);
    }

    public void OnBtn_Weather_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Weather);
    }

    public void OnBtn_LifeSkill_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_LifeSkill_Message);
    }

    public void OnBtn_ExplorationTip_ButtonClicked()
    {
        List<uint> list = Sys_Exploration.Instance.list_ExplorationRewardNotice;
        if (list.Count > 0)
        {
            Sys_Npc.Instance.ReqNpcGetActiveReward(list[0]);
        }
    }

    public void OnChange_ButtonClicked()
    {
        if (!Sys_FunctionOpen.Instance.IsOpen(20500, false))
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007617));
        }
        else
        {
            if (Layout.Grid01Go.gameObject.activeInHierarchy)
            {
                Layout.Grid01Go.gameObject.SetActive(false);
                Layout.probebuttonGo.gameObject.SetActive(true);
                Layout.Go_NormalMenu.SetActive(false);
                Layout.Go_ProbeMenu.SetActive(true);
                Animator animator = Layout.Btn_Look.transform.Find("Image_Icon").GetComponent<Animator>();
                if (animator.enabled)
                    animator.Play("Look", -1, 0);
            }
            else
            {
                Layout.Grid01Go.gameObject.SetActive(true);
                Layout.probebuttonGo.gameObject.SetActive(false);
                Layout.Go_NormalMenu.SetActive(true);
                Layout.Go_ProbeMenu.SetActive(false);
            }
        }
    }

    public void OnBtn_OldClicked()
    {
    }

    public void OnBtn_EyeClicked()
    {
    }

    public void OnBtn_LookClicked()
    {
        if (m_IsEnterNpcLook)
        {

            if (Sys_Team.Instance.canManualOperate)
            {
                Sys_Inquiry.Instance.Inquiry();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5008));
            }
        }
        else
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5007));
        }
    }

    public void OnBtn_TempPetBagClicked()
    {
        UIManager.OpenUI(EUIID.UI_Temple_Storage);
    }

    public void OnBtn_EnergysparClicked()
    {
        UIManager.OpenUI(EUIID.UI_Energyspar);
    }

    public void OnBtn_UpliftedClicked()
    {
        UIManager.OpenUI(EUIID.UI_Uplifted);
        Layout.Go_Uplifted_Fx.gameObject.SetActive(false);
        Sys_Uplifted.Instance.isFxShow = false;
    }

    public void OnBtn_DailyClicked()
    {
        UIManager.OpenUI(EUIID.UI_DailyActivites_Limite);
    }
    public void OnBtn_HangupClicked()
    {
        Sys_Hangup.Instance.SetEnemySwitch();
    }

    public void OnBtn_HundreadPeopelBuffClicked()
    {
        UIManager.OpenUI(EUIID.UI_HundredPeopelAwakenTip);
    }

    public void OnBtn_MessageBag()
    {
        Sys_MessageBag.Instance.isClick = true;
        Layout.messageBagRedPoint.SetActive(false);
        UIManager.OpenUI(EUIID.UI_MessageBag);
    }
    public void OnBtn_ServivalPvpClicked()
    {
        if (Sys_SurvivalPvp.Instance.isCanOpenMainUI())
            UIManager.OpenUI(EUIID.UI_SurvivalPvp);
    }
    public void OnBtn_Cook_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Cooking_Collect);
    }
    public void OnBtn_OpenTrialGateSkill()
    {
        UIManager.OpenUI(EUIID.UI_TrialSkillDeploy);
    }
    public void OnBtn_OpenBossTower()
    {
        Sys_ActivityBossTower.Instance.OpenCurStateMainView();
    }

    public void OnBtn_Bless()
    {
        UIManager.OpenUI(EUIID.UI_Blessing);
    }
    #endregion

    void OnStartReConnect()
    {
        Layout.BlockImage.SetActive(true);
    }

    public void OnBtn_TravellerAwakeningBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_Awaken);
    }

    void OnReconnected()
    {
        Layout.BlockImage.SetActive(false);
    }

    private void SetActiveBtnCrystal(bool active)
    {
        Layout.Btn_Crystal.gameObject.SetActive(active);
    }

    public void OnBtn_CrystalClicked()
    {
        Sys_ElementalCrystal.Instance.OpenTips();
        if (Sys_ElementalCrystal.Instance.durabilityType == 0)
        {
            SetActiveBtnCrystal(false);
            Sys_ElementalCrystal.Instance.durabilityType = 1;
        }
    }

    public void OnBtn_BattlePassBtnClicked()
    {
        var uiid = Sys_BattlePass.Instance.isFristActive ? EUIID.UI_BattlePass_Popup : EUIID.UI_BattlePass;

        UIManager.OpenUI(uiid);
    }
    private void OnRefreshCrystal()
    {
        ItemData equipCrystal = Sys_ElementalCrystal.Instance.GetEquipedCrystal();
        if (equipCrystal != null)
        {
            if (equipCrystal.crystal.Durability == 0)
            {
                Sys_ElementalCrystal.Instance.durabilityType = 1;
                SetActiveBtnCrystal(true);
            }
            else if (equipCrystal.crystal.Durability > 0 && equipCrystal.crystal.Durability < (uint)(equipCrystal.maxDurability * 0.1f))
            {
                if (Sys_ElementalCrystal.Instance.durabilityType == 0)
                {
                    SetActiveBtnCrystal(true);
                }
            }
        }
    }


    private void RefreshDailyLimite()
    {
        int count = Sys_Daily.Instance.NoticeList.Count;

        bool isNeedShow = count > 0;
        bool isShow = Layout.m_BtnDaily.gameObject.activeSelf;

        if (isNeedShow != isShow)
        {
            if (Sys_FamilyResBattle.Instance.InFamilyBattle)
            {
                isNeedShow = false;
            }
            Layout.m_BtnDaily.gameObject.SetActive(isNeedShow);
        }

        if (isNeedShow == false)
            return;

        var fristLimiteKey = Sys_Daily.Instance.NoticeList[0];

        var fristLimite = Sys_Daily.Instance.GetDailyNotice(fristLimiteKey);

        var data = CSVDailyActivity.Instance.GetConfData(fristLimite.ID);

        Layout.m_TexDailyNum.text = count.ToString();
        Layout.m_TexDailyName.text = LanguageHelper.GetTextContent(data.ActiveName);
        Layout.m_TexDailyTime.text = string.Empty;

        if (m_DailyLimiteTimer != null)
            m_DailyLimiteTimer.Cancel();

        var offsetTime = fristLimite.EndNoticeTime - Sys_Daily.Instance.GetTodayTimeSceond();

        LimiteTimeSceond = fristLimite.EndNoticeTime;

        m_DailyLimiteTimer = Timer.Register(offsetTime, OnLimiteTimeOver, OnTickLimiteTime);

    }

    private void OnTickLimiteTime(float value)
    {
        float lasttime = LimiteTimeSceond - Sys_Daily.Instance.GetTodayTimeSceond();
        Layout.m_TexDailyTime.text = getTimestinrg(lasttime);
    }

    private string getTimestinrg(float time)
    {
        int hour = (int)(time / 3600);
        int mins = (int)((time - 3600 * hour) / 60);

        int sceond = (int)(time - hour * 3600 - mins * 60);

        string hourstr = hour < 10 ? ("0" + hour.ToString()) : hour.ToString();
        string minsstr = mins < 10 ? ("0" + mins.ToString()) : mins.ToString();
        string sceondstr = sceond < 10 ? ("0" + sceond.ToString()) : sceond.ToString();

        return hourstr + ":" + minsstr + ":" + sceondstr;
    }
    private void OnLimiteTimeOver()
    {
        RefreshDailyLimite();
    }
    private void OnNewDailyLimite()
    {
        RefreshDailyLimite();
    }

    private void onLoadMapOk()
    {
        RefreshSurvivalPvp();
        Layout.m_TrialGateSkillBtn.gameObject.SetActive(Sys_Map.Instance.CurMapId == Sys_ActivityTrialGate.Instance.trialMapId);
        Layout.m_BossTowerBtn.gameObject.SetActive(Sys_ActivityBossTower.Instance.CheckIsOnBossTowerMap());
    }

    private void OnReadMapTip(uint mapId)
    {
        RefreshAboutMap();
    }

    private void OnSurvivalPvpMatch()
    {
        if (Sys_SurvivalPvp.Instance.isMatching)
        {
            Layout.m_AnServivalPvp.Play("Match");
        }
    }

    private void OnSurvivalPvpMatchCancle()
    {
        if (Sys_SurvivalPvp.Instance.isMatching == false)
        {
            Layout.m_AnServivalPvp.Play("Entrance");
        }
    }

    public void OnMessageBag(int type)
    {
        Sys_MessageBag.Instance.isClick = false;
        OnMessageBagButtonShow();
    }
    private void RefreshSurvivalPvp()
    {
        bool ispvpMap = Sys_SurvivalPvp.Instance.isSurvivalPvpMap(Sys_Map.Instance.CurMapId);

        if (Layout.m_SurvivalPvp.gameObject.activeSelf != ispvpMap)
            Layout.m_SurvivalPvp.gameObject.SetActive(ispvpMap);

        if (ispvpMap && Sys_SurvivalPvp.Instance.isMatching)
        {
            Layout.m_AnServivalPvp.Play("Match");
        }
        else if (ispvpMap)
        {
            Layout.m_AnServivalPvp.Play("Entrance");
        }
    }

    private void RefreshSurivalMatchTime()
    {
        Layout.m_TexTimeServivalPvp.text = Sys_SurvivalPvp.Instance.GetMatchTimeString();
    }
    public void RefreshEnemySwitch()
    {
        bool enemyOnOff = Sys_Hangup.Instance.cmdHangUpDataNtf.EnemyOnOff;
        bool state = enemyOnOff || Sys_Pet.Instance.isSeal; //开关||巡逻中

        Layout.go_hangup1.gameObject.SetActive(!state);
        Layout.go_hangup2.gameObject.SetActive(state);
    }
    public void OnEnemySwitch(int _, int __)
    {
        RefreshEnemySwitch();
    }

    private void OnUsingUpdate()
    {
        Sys_Head.Instance.SetHeadAndFrameData(Layout.roleIcon);
    }

    private void OnBeginEnter(uint stackID, int nID)
    {
        if (nID == (int)EUIID.UI_Chat)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Layout.m_View_Map.SetActive(AspectRotioController.IsExpandState);
#else
            Layout.m_View_Map.SetActive(false);
#endif
        }
        if (nID == (int)EUIID.UI_MainMenu)
        {
            OnUpdateMenuBtn(true);
            FunShowState(false);
        }
    }

    private void OnUIEndExit(uint stackID, int nID)
    {
        if (nID == (int)EUIID.UI_Chat)
        {
            Layout.m_View_Map.SetActive(true);
        }
        if (nID == (int)EUIID.UI_MainMenu)
        {
            OnUpdateMenuBtn(false);
            FunShowState(true);
        }
    }

    public void OnCook_ButtonClicked()
    {
        UIManager.OpenUI(EUIID.UI_Cooking_Main);
    }

    public void OnBtn_SpecialBtnClicked()
    {
        if (Layout.m_Special_Grid.activeInHierarchy)
        {
            Layout.m_SpecialBtn_Arrow.localEulerAngles = new Vector3(0, 0, 90);
            Layout.m_Special_Grid.SetActive(false);
        }
        else
        {
            Layout.m_SpecialBtn_Arrow.localEulerAngles = new Vector3(0, 0, -90);
            Layout.m_Special_Grid.SetActive(true);
        }
    }

    public void OnBtn_PrivilegeBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_Welfare_Main);
    }

    public void OnBtnClickedReadMapMail()
    {
        foreach (var mapId in Sys_Map.Instance.unReadedMapMails)
        {
            UIManager.OpenUI(EUIID.UI_ReadMapTip, false, mapId);
            break;
        }
    }

    public void OnBtn_OpenFamilyRedPacket()
    {
        Layout.View_RedPacket.gameObject.SetActive(false);
        Sys_Family.Instance.QueryEnvelopeInfoReq();
    }
    private void UpdateFamilyRedPacket()
    {
        Layout.View_RedPacket.gameObject.SetActive(true);
        Sys_Family.Instance.getRedPacketViewIsShow = true;
        Sys_Family.Instance.ShowTime(Layout.View_RedPacket.gameObject);
    }

    private void CheckCookingRed()
    {
        bool has = Sys_Cooking.Instance.HasSubmitItem(0) || Sys_Cooking.Instance.HasReward();
        Layout.m_CookingRed.SetActive(has);
    }

    private void ShowMenuBtn()
    {
        Layout.Btn_Open.gameObject.SetActive(true);
        Layout.Btn_Close_Normal.gameObject.SetActive(false);
        RefreshMenuRedPoint();
    }
    private void RefreshMenuRedPoint()
    {
        Layout.open_RedPoint.SetActive(Sys_MainMenu.MenuRedPoint.Value > 0);
    }
    /// <summary>
    /// 打开或关闭UI_MainMenu 其他功能状态
    /// </summary>
    private void FunShowState(bool active)
    {
        int num = Sys_MainMenu.Instance.GetFunctionRaw();
        if (num > 1)
            Layout.m_ViewWarning.SetActive(active);
        if (num > 2)
        {
            ui_SpecialTask.gameObject.SetActive(active);
            uI_MapExplorationTip.gameObject.SetActive(active);
        }
        if (num > 3)
            Layout.m_ViewTask.SetActive(active);
        if (num > 6)
            Layout.m_ViewIcon.SetActive(active);
    }

    private void OnUpdateMenuBtn(bool isOpen)
    {
        Layout.Btn_Open.gameObject.SetActive(!isOpen);
        Layout.Btn_Close_Normal.gameObject.SetActive(isOpen);
    }
}
