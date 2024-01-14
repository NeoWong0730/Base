using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class UI_JSBattle: UIBase
{
    /// <summary> 界面关闭按钮 </summary>
    private Button closeBtn;
    /// <summary> 购买次数按钮 </summary>
    private Button buyTimesBtn;
    /// <summary> 换一批按钮-刷新挑战角色 </summary>
    private Button refreshChallengeBtn;
    /// <summary> 一键扫荡按钮 </summary>
    private Button oneKeyChallengeBtn;
    /// <summary> 幸运伙伴-按钮 </summary>
    private Button luckPartnerBtn;
    /// <summary> 幸运种族-按钮 </summary>
    private Button luckGenusBtn;
    /// <summary> 排名按钮 </summary>
    private Button rankBtn;
    /// <summary> 战斗纪录按钮 </summary>
    private Button rerordBtn;
    /// <summary> 奖励按钮 </summary>
    private Button rewardBtn;
    /// <summary> 幸运伙伴-图标 </summary>
    private Image luckPartnerImage;
    /// <summary> 幸运种族-图标 </summary>
    private Image luckGenusImage;
    /// <summary> 我的名次文本 </summary>
    private Text myRankText;
    /// <summary> 我的综合评分 </summary>
    private Text myScoreText;
    /// <summary> 今日挑战次数 </summary>
    private Text todayBattleTimesText;

    private UI_CurrencyTitle currencyTitle;

    private UI_JsBattle_RedPoint redPoint;
    public class UI_ChallengeInfoView : UIComponent
    {
        private Button closeBtn;
        private Image careerImage;
        private Image roleIconImage;
        private Text careerText;
        private Text roleNameText;
        private Text levelText;
        private Text pointText;
        private Transform partnerTran;
        private List<List<float>> pos;
        private Transform posTran;
        protected override void Loaded()
        {
            closeBtn =transform.Find("GameObject").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClicked);
            roleIconImage = transform.Find("Team_Player (1)/Head").GetComponent<Image>();
            careerImage = transform.Find("Team_Player (1)/Image_IconPro").GetComponent<Image>();
            careerText = transform.Find("Team_Player (1)/Text_Profession").GetComponent<Text>();
            roleNameText = transform.Find("Team_Player (1)/Text_Name").GetComponent<Text>();
            levelText = transform.Find("Team_Player (1)/Image_Icon/Text_Number").GetComponent<Text>();
            pointText = transform.Find("Team_Player (1)/Text_Point/Text").GetComponent<Text>();
            partnerTran = transform.Find("Team_Player (1)/ButtonScroll/Viewport");
            posTran = transform.Find("Team_Player (1)");
            pos = ReadHelper.ReadArray2_ReadFloat(CSVDecisiveArenaParam.Instance.GetConfData(12).str_value,'|', '&');
        }

        private void CloseBtnClicked()
        {
            base.Hide();
        }

        public void RefreshInfo(VictoryArenaOppoUnit data,int index)
        {
            if (null != data)
            {
                TextHelper.SetText(roleNameText, data.Name.ToStringUtf8());
                TextHelper.SetText(levelText, data.Level.ToString());
                TextHelper.SetText(pointText, data.Score.ToString());
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(data.Career);
                CharacterHelper.SetHeadAndFrameData(roleIconImage, (uint)data.HeroId, data.Photo, data.PhotoFrame);

                if (null != cSVCareerData)
                {
                    TextHelper.SetText(careerText, cSVCareerData.name);
                    
                    ImageHelper.SetIcon(careerImage, cSVCareerData.logo_icon);
                }
                else
                {
                    DebugUtil.LogError($"Not Find {data.Career} In CSVCareer");
                }

                var partneFomation = data.Formation.Partners;
                int count = 0;
                if (null != partneFomation)
                {
                    count = partneFomation.Count;
                    FrameworkTool.CreateChildList(partnerTran, partneFomation.Count);
                    for (int i = 0; i < count; i++)
                    {
                        Image image = partnerTran.GetChild(i).transform.Find("Head").GetComponent<Image>();
                        CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(partneFomation[i].PartnerTid);
                        if (null != cSVPartnerData)
                        {
                            ImageHelper.SetIcon(image, cSVPartnerData.battle_headID);
                        }
                        else
                        {
                            DebugUtil.LogError($"Not Find Peratner Id = {partneFomation[i].PartnerTid}");
                        }
                    }
                }
                var petFormation = data.Formation.Pets;
                if (null != petFormation)
                {
                    var startIndex = null != partneFomation ? partneFomation.Count : 0;
                    count = null != partneFomation ? (partneFomation.Count + petFormation.Count) : petFormation.Count;
                    FrameworkTool.CreateChildList(partnerTran, count);
                    for (int i = startIndex ; i < count; i++)
                    {
                        Image image = partnerTran.GetChild(i).transform.Find("Head").GetComponent<Image>();
                        CSVPetNew.Data cSVPartnerData = CSVPetNew.Instance.GetConfData(petFormation[i - startIndex].PetTid);
                        if (null != cSVPartnerData)
                        {
                            ImageHelper.SetIcon(image, cSVPartnerData.icon_id);
                        }
                        else
                        {
                            DebugUtil.LogError($"Not Find Pet Id = {petFormation[i - startIndex].PetTid}");
                        }
                    }
                }
                if (null != pos && index >= 0 && index < pos.Count && pos[index].Count >= 2)
                {
                    posTran.localPosition = new Vector3(pos[index][0], pos[index][1], 0);
                }
                base.Show();
            }
        }

    }
    Timer m_UpdateShowScenePosTimer;
    private AssetDependencies mInfoAssetDependencies;
    public enum EJSModelShow
    {
        Role,
        Pet
    }
    public enum EJsModelPos
    {
        Pos0 = 0,
        Pos0_Pet,
        Pos1,
        Pos1_Pet,
        Pos2,
        Pos2_Pet,
    }
    private ShowSceneControl m_ShowSceneControl = null;
    private UI_ChallengeInfoView uI_ChallengeInfoView;
    private List<JsBattleShowModelItem> m_ShowPosList = new List<JsBattleShowModelItem>(6);

    private List<JsBattleModelShowBase> m_ModelShow = new List<JsBattleModelShowBase>(6);
    /// <summary> 对手信息视图 </summary>
    List<UI_JSChallengeInfo> uI_JSChallengeInfos = new List<UI_JSChallengeInfo>(3);

    protected override void OnLoaded()
    {
        mInfoAssetDependencies = transform.GetComponent<AssetDependencies>();
        closeBtn =transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
        closeBtn.onClick.AddListener(OnCloseBtnClicked);
        refreshChallengeBtn = transform.Find("Animator/View_JSBattle/Btn_01").GetComponent<Button>();
        refreshChallengeBtn.onClick.AddListener(OnRefreshChallengeBtnClicked);

        oneKeyChallengeBtn = transform.Find("Animator/View_JSBattle/Btn_02").GetComponent<Button>();
        oneKeyChallengeBtn.onClick.AddListener(OnOneKeyChallengeBtnClicked);

        buyTimesBtn = transform.Find("Animator/View_JSBattle/Top/Grid/Change/Btn_01_Small").GetComponent<Button>();
        buyTimesBtn.onClick.AddListener(OnBuyBtnClicked);

        luckPartnerBtn = transform.Find("Animator/View_JSBattle/Top/Btn_arrow").GetComponent<Button>();
        luckPartnerBtn.onClick.AddListener(OnLuckPartnerBtnClick);
        luckGenusBtn = transform.Find("Animator/View_JSBattle/Top/Image_zhongzu").GetComponent<Button>();
        luckGenusBtn.onClick.AddListener(OnLuckGenusBtnClick);

        rankBtn = transform.Find("Animator/View_JSBattle/Grid/Btn_Rank").GetComponent<Button>();
        rankBtn.onClick.AddListener(OnRankBtnClicked);
        rerordBtn = transform.Find("Animator/View_JSBattle/Grid/Btn_Rerord").GetComponent<Button>();
        rerordBtn.onClick.AddListener(OnRerordBtnClicked);
        rewardBtn = transform.Find("Animator/View_JSBattle/Grid/Btn_Reward").GetComponent<Button>();
        rewardBtn.onClick.AddListener(OnRewardBtnClicked);

        luckPartnerImage = transform.Find("Animator/View_JSBattle/Top/Partner_Head").GetComponent<Image>();
        luckGenusImage = transform.Find("Animator/View_JSBattle/Top/Image_zhongzu").GetComponent<Image>();

        myRankText = transform.Find("Animator/View_JSBattle/Top/Grid/Rank_Num").GetComponent<Text>();
        myScoreText = transform.Find("Animator/View_JSBattle/Top/Grid/Point_Num (1)").GetComponent<Text>();
        todayBattleTimesText = transform.Find("Animator/View_JSBattle/Top/Grid/Time_Num (2)").GetComponent<Text>();

        uI_ChallengeInfoView = AddComponent<UI_ChallengeInfoView>(transform.Find("Animator/VIew_Player")); 

        UI_JSChallengeInfo temp1 = AddComponent<UI_JSChallengeInfo>(transform.Find("Animator/View_JSBattle/Model1"));
        temp1.Init(0, ShowChallengeInfoByIndex);
        UI_JSChallengeInfo temp2 = AddComponent<UI_JSChallengeInfo>(transform.Find("Animator/View_JSBattle/Model2"));
        temp2.Init(1, ShowChallengeInfoByIndex);
        UI_JSChallengeInfo temp3 = AddComponent<UI_JSChallengeInfo>(transform.Find("Animator/View_JSBattle/Model3"));
        temp3.Init(2, ShowChallengeInfoByIndex);
        uI_JSChallengeInfos.Add(temp1);
        uI_JSChallengeInfos.Add(temp2);
        uI_JSChallengeInfos.Add(temp3);
        currencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
        
        redPoint = gameObject.AddComponent<UI_JsBattle_RedPoint>();
        redPoint?.Init(this);
    }

    private void ShowChallengeInfoByIndex(int index)
    {
        var challenges = Sys_JSBattle.Instance.oppoList;
        var challengeCount = challenges.Count;
        if (challengeCount > 1 && challengeCount <= 3 && index >= 0 && index <= (challengeCount - 1))
        {
            uI_ChallengeInfoView.RefreshInfo(challenges[index], index);
        }
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_JSBattle.Instance.eventEmitter.Handle(Sys_JSBattle.EEvents.ChallengeInfoRefresh, RefreshChallengeInfo, toRegister);
        Sys_JSBattle.Instance.eventEmitter.Handle(Sys_JSBattle.EEvents.BuyTimesEnd, RefreshTimes, toRegister);
    }

    protected override void OnOpen(object arg = null)
    {

    }
    public List<uint> currencyList = new List<uint>() { 1, 2 };
    protected override void OnShow()
    {
        currencyTitle.SetData(currencyList);
        LoadShowScene();
        Sys_JSBattle.Instance.VictoryArenaRefreshReq();
        RefreshView();
        m_UpdateShowScenePosTimer?.Cancel();
        m_UpdateShowScenePosTimer = Timer.Register(0.5f, UpdataShowScenePosPosition);
    }

    protected override void OnHide()
    {
        buttonTimer?.Cancel();
        if (null != uI_ChallengeInfoView)
        {
            uI_ChallengeInfoView.Hide();
        }
        UnLoadShowScene();
        m_UpdateShowScenePosTimer?.Cancel();
    }

    protected override void OnDestroy()
    {
        currencyTitle.Dispose();
    }
    
    public void UpdataShowScenePosPosition()
    {
        int count = m_ShowPosList.Count;
        for (int i = 0; i < count; i++)
        {
            if(i % 2 == 0)
            {
                Transform objTrans = m_ShowPosList[i].VGO.transform;

                Vector3 posshow = m_ShowSceneControl.mCamera.WorldToViewportPoint(objTrans.position);


                var itemposition = GetMemberItemPosition(i);
                Vector3 uiitem = UIManager.mUICamera.WorldToViewportPoint(itemposition);

                posshow.x = uiitem.x;

                Vector3 position = m_ShowSceneControl.mCamera.ViewportToWorldPoint(posshow);

                objTrans.position = position;
            }
        }
    }

    public Vector3 GetMemberItemPosition(int i)
    {
        return uI_JSChallengeInfos[i / 2].transform.position;
    }

    #region 模型加载
    public void LoadShowScene()
    {
        GameObject scene = GameObject.Instantiate<GameObject>(mInfoAssetDependencies.mCustomDependencies[0] as GameObject);

        m_ShowSceneControl = new ShowSceneControl();

        scene.transform.SetParent(GameCenter.sceneShowRoot.transform);

        m_ShowSceneControl.Parse(scene);


        for (int i = 1; i < 4; i++)
        {
           Transform objTrans = m_ShowSceneControl.mRoot.transform.Find("Pos_" + i.ToString());

            VirtualGameObject vobj = new VirtualGameObject();
            vobj.SetGameObject(objTrans.gameObject,true);

            m_ShowPosList.Add(new JsBattleShowModelItem() { VGO = vobj});

            Transform objTransPet = m_ShowSceneControl.mRoot.transform.Find("Pos_" + i + "/Pos_Pet_ " + i.ToString());

            VirtualGameObject vobjPet = new VirtualGameObject();
            vobjPet.SetGameObject(objTransPet.gameObject, true);

            m_ShowPosList.Add(new JsBattleShowModelItem() { VGO = vobjPet });
        }

    }
    

    public void UnLoadShowScene()
    {
        if (m_ShowSceneControl == null)
            return;

        m_ShowSceneControl.Dispose();

        m_ShowSceneControl = null;

        m_ShowPosList.Clear();

        int count = m_ModelShow.Count;
        for (int i = 0; i < count; i++)
        {
            m_ModelShow[i].Dispose();
            m_ModelShow[i] = null;
        }
        m_ModelShow.Clear();
    }

    public void RestShowScene()
    {
        int count = m_ShowPosList.Count;

        for (int i = 0; i < count; i++)
        {
            var value = m_ShowPosList[i];

            if (value.ModelShow != null)
            {
                value.SetModelShow (null);
            }
        }
    }

    public void UpdateShowScene()
    {
        int count = m_ModelShow.Count;

        for (int i = count - 1; i >= 0; i--)
        {
            var value = m_ModelShow[i];

            if (value != null)
            {
                value.Dispose();

                m_ModelShow.RemoveAt(i);
            }
        }
    }

    public void SetMemberModel(EJSModelShow type, EJsModelPos ePos, uint index, uint id,  uint caree = 0,uint WeaponID = 0, uint dressId = 0, Dictionary<uint, List<dressData>> DressValue = null)
    {
        JsBattleModelShowBase modelShow = null;

        modelShow = m_ModelShow.Find(o => o.index == index);

        if (modelShow != null)
        {
            modelShow.ChangeWeapon(WeaponID);
        }
        
        if (modelShow == null)
        {
            modelShow = CreateModeShow(type, ePos, id, caree, WeaponID, dressId, DressValue);

            modelShow.index = index;

            m_ModelShow.Add(modelShow);

        }

        var value = GetModelPosition(ePos);

        if (value.ModelShow != null)
        {
            value.ModelShow.SetActive(false);
        }
        value.SetModelShow(modelShow);

    }

    private JsBattleShowModelItem GetModelPosition(EJsModelPos ePos)
    {
        JsBattleShowModelItem posObj = null;

        posObj = m_ShowPosList[(int)ePos];

        return posObj;
    }

    private JsBattleModelShowBase CreateModeShow(EJSModelShow type, EJsModelPos ePos,uint id, uint caree = 0,uint WeaponID = 0, uint dressId = 0, Dictionary<uint, List<dressData>> DressValue = null)
    {
        JsBattleModelShowBase modeShow = null;

        switch (type)
        {
            case EJSModelShow.Role:
                var mrms = new JsBattleModelShow();
                mrms.WeaponID = WeaponID;
                mrms.DressID = dressId;
                modeShow = mrms;
                
                break;
            case EJSModelShow.Pet:
                modeShow = new JsBattlePetModelShow();
                break;
        }

        var parent = GetModelPosition(ePos);

        modeShow.Parent = parent.VGO;

        modeShow.LoadModel(id, caree,WeaponID, DressValue);

        return modeShow;
    }
    #endregion

    private void RefreshView()
    {
        SystemVictoryArena systemVictoryArena = Sys_JSBattle.Instance.GetSysTemVictoryData();
        if (null != systemVictoryArena)
        {
            var partnerConfig = CSVPartner.Instance.GetConfData(systemVictoryArena.LuckyPartnerId);
            if (null != partnerConfig)
            {
                ImageHelper.SetIcon(luckPartnerImage, partnerConfig.battle_headID);
            }
            else
            {
                DebugUtil.LogError($"Not Find Id = {systemVictoryArena.LuckyPartnerId} In CSVPartner");
            }

            var genusConfig = CSVGenus.Instance.GetConfData(systemVictoryArena.LuckyPetRace);
            if (null != genusConfig)
            {
                ImageHelper.SetIcon(luckGenusImage, genusConfig.rale_icon);
            }
            else
            {
                DebugUtil.LogError($"Not Find Id = {systemVictoryArena.LuckyPetRace} In genusConfig");
            }
        }

        TextHelper.SetText(myScoreText, Sys_Attr.Instance.rolePower.ToString());
    }

    private void RefreshTimes()
    {
        RoleVictoryArenaDaily roleVictoryArenaDaily = Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();
        if (null != roleVictoryArenaDaily)
        {
            todayBattleTimesText.text = roleVictoryArenaDaily.LeftChallengeTimes.ToString();
        }
    }


    private void RefreshChallengeInfo()
    {
        if (null != uI_ChallengeInfoView)
        {
            uI_ChallengeInfoView.Hide();
        }
        RefreshTimes();
        TextHelper.SetText(myRankText, 2024710u, Sys_JSBattle.Instance.GetMyCurrentRank().ToString());
        
        var challenges = Sys_JSBattle.Instance.oppoList;
        RestShowScene();
        UpdateShowScene();
        if (challenges.Count > 3)//挑战对手数量异常不处理
        {
            return;
        }
        for (int i = 0; i < challenges.Count; i++)
        {
            var challenge = challenges[i];
            uI_JSChallengeInfos[i].RefreshInfo(challenge);
            SetMemberModel(EJSModelShow.Role, (EJsModelPos)(i * 2), (uint)(i * 2), challenge.HeroId, challenge.Career, challenge.WeaponItemID, GetDressId(challenge.FashionInfo.FashionInfos), Sys_Fashion.Instance.GetDressData(challenge.FashionInfo.FashionInfos, challenge.HeroId));
            if(challenge.FollowPetInfo!= 0)
            {
                SetMemberModel(EJSModelShow.Pet, (EJsModelPos)(i * 2 + 1), (uint)(i * 2 + 1), challenge.FollowPetInfo, 0,challenge.FollowPetSuitId);
            }
           
        }
        CheckRefreshButtonState();
        //强刷刷新布局：处理嵌套layout 自动布局与content size fitter 时，布局刷新失败。后续有更好方法可以再进行优化
        if (myRankText.transform.parent != null)
        {
            FrameworkTool.ForceRebuildLayout(myRankText.transform.parent.gameObject);
        }
    }

    private void CheckRefreshButtonState()
    {
        if(null != refreshChallengeBtn)
        {
            uint nextRefreshTime = Sys_JSBattle.Instance.GetNextChallengesTimes();
            uint severTime = Sys_Time.Instance.GetServerTime();
            ButtonHelper.Enable(refreshChallengeBtn, severTime >= nextRefreshTime);

            if(severTime >= nextRefreshTime)
            {
                TextHelper.SetText(refreshChallengeBtn.transform.Find("Text_01").GetComponent<Text>(), 2004093u);
            }
            buttonTimer?.Cancel();
            if (severTime < nextRefreshTime)
            {
                var dur = nextRefreshTime - severTime;
                buttonTimer = Timer.Register(dur, CheckRefreshButtonState,
                    (f)=> {
                    TextHelper.SetText(refreshChallengeBtn.transform.Find("Text_01").GetComponent<Text>(), 2024719u, ((int)Math.Ceiling(dur - f)).ToString());
                    });
            }
        }
    }

    Timer buttonTimer;

    private uint GetDressId(RepeatedField<MapRoleFashionInfo> fashInfo)
    {
        int count = fashInfo.Count;

        for (int i = 0; i < count; ++i)
        {
            EHeroModelParts part;
            bool bresult = Sys_Fashion.Instance.parts.TryGetValue(fashInfo[i].FashionId, out part);
            if (bresult && part == EHeroModelParts.Main)
            {
                return fashInfo[i].FashionId;
            }
        }

        return 100;
    }

    #region ClickEvent
    private void OnCloseBtnClicked()
    {
        UIManager.CloseUI(EUIID.UI_JSBattle);
    }

    private void OnBuyBtnClicked()
    {
        var dailyData = Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();
        if(dailyData.LeftBuyTimes == 0)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024702));
            return;
        }
        if(null != dailyData)
        {
            var buyCostData = CSVDecisiveArenaParam.Instance.GetConfData(8);
            var buyGetTimesData = CSVDecisiveArenaParam.Instance.GetConfData(9);
           
            if (null != buyCostData && null != buyGetTimesData)
            {
                List<uint> costList = ReadHelper.ReadArray_ReadUInt(buyCostData.str_value, '|');
                if(costList.Count >=2)
                {
                    ItemIdCount itemIdCount = new ItemIdCount(costList[0], costList[1]);
                    string str = LanguageHelper.GetTextContent(2024701, itemIdCount.count.ToString(), LanguageHelper.GetTextContent(itemIdCount.CSV.name_id), buyGetTimesData.str_value, dailyData.LeftBuyTimes.ToString());
                    PromptBoxParameter.Instance.OpenPromptBox(str, 0,
                    () => {
                        if (itemIdCount.Enough)
                        {
                            Sys_JSBattle.Instance.VictoryArenaBuyChallengeTimesReq();
                        }
                        else
                        {
                            Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.GoldCoin, itemIdCount.count);
                        }
                    });
                }
                    
            }
        }
    }

    private void OnRefreshChallengeBtnClicked()
    {
        uint nextRefreshTime = Sys_JSBattle.Instance.GetNextChallengesTimes();
        uint severTime = Sys_Time.Instance.GetServerTime();
        if (severTime > nextRefreshTime)
        {
            Sys_JSBattle.Instance.VictoryArenaRefreshReq();
        }
    }

    private void OnOneKeyChallengeBtnClicked()
    {
        RoleVictoryArenaDaily roleVictoryArenaDaily = Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();
        if (null != roleVictoryArenaDaily)
        {
            if (roleVictoryArenaDaily.LeftChallengeTimes > 0)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024703));
                }
                else
                {
                    PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(2024724), 0,
                               () => {
                                   Sys_JSBattle.Instance.VictoryArenaFastChallengeReq();
                               });
                    
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024707));
                if (roleVictoryArenaDaily.LeftBuyTimes > 0)
                {
                    var buyCostData = CSVDecisiveArenaParam.Instance.GetConfData(8);
                    var buyGetTimesData = CSVDecisiveArenaParam.Instance.GetConfData(9);

                    if (null != buyCostData && null != buyGetTimesData)
                    {
                        List<uint> costList = ReadHelper.ReadArray_ReadUInt(buyCostData.str_value, '|');
                        if (costList.Count >= 2)
                        {
                            ItemIdCount itemIdCount = new ItemIdCount(costList[0], costList[1]);
                            string str = LanguageHelper.GetTextContent(2024701, itemIdCount.count.ToString(), LanguageHelper.GetTextContent(itemIdCount.CSV.name_id), buyGetTimesData.str_value, roleVictoryArenaDaily.LeftBuyTimes.ToString());
                            PromptBoxParameter.Instance.OpenPromptBox(str, 0,
                            () => {
                                if (itemIdCount.Enough)
                                {
                                    Sys_JSBattle.Instance.VictoryArenaBuyChallengeTimesReq();
                                }
                                else
                                {
                                    Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.GoldCoin, itemIdCount.count);
                                }
                            });
                        }
                    }
                }
            }
        }
    }

    private void OnLuckPartnerBtnClick()
    {
        UIManager.OpenUI(EUIID.UI_Partner);
    }

    private void OnLuckGenusBtnClick()
    {
        SystemVictoryArena systemVictoryArena = Sys_JSBattle.Instance.GetSysTemVictoryData();
        if (null != systemVictoryArena)
        {
            var genusConfig = CSVGenus.Instance.GetConfData(systemVictoryArena.LuckyPetRace);
            if (null != genusConfig)
            {
                UIManager.OpenUI(EUIID.UI_JSBattle_Tips, false, LanguageHelper.GetTextContent(genusConfig.rale_name));

            }
            else
            {
                DebugUtil.LogError($"Not Find Id = {systemVictoryArena.LuckyPetRace} In genusConfig");
            }
        }
       
    }

    private void OnRankBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_JSBattle_Rank);
    }

    private void OnRerordBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_JSBattle_Record);
    }

    private void OnRewardBtnClicked()
    {
        UIManager.OpenUI(EUIID.UI_JSBattle_Reward);
    }
    #endregion
}

public class UI_JSChallengeInfo : UIComponent
{
    private Image bg_100afterImage;
    private Image bg_4_99Image;
    private Image bg_1_3Image;
    private Image rankImage;
    private Image careerImage;
    private Text rankNumText;
    private Text roleNameText;
    private Text levelText;
    private Text pointText;
    private Button challengeBtn;
    private Button showChallengeInfoBtn;
    private Action<int> action;
    private int index;
    
    protected override void Loaded()
    {
        bg_100afterImage = transform.Find("Rank/BG1").GetComponent<Image>();
        bg_4_99Image = transform.Find("Rank/BG2").GetComponent<Image>();
        bg_1_3Image = transform.Find("Rank/BG3").GetComponent<Image>();
        rankImage = transform.Find("Rank/Image_Rank").GetComponent<Image>();
        careerImage = transform.Find("Name/Profession").GetComponent<Image>();
        rankNumText = transform.Find("Rank/Text").GetComponent<Text>();
        roleNameText = transform.Find("Name").GetComponent<Text>();
        levelText = transform.Find("Lv").GetComponent<Text>();
        pointText = transform.Find("Point").GetComponent<Text>();
        challengeBtn = transform.Find("Btn").GetComponent<Button>();
        challengeBtn.onClick.AddListener(ChallengeBtnClicked);
        showChallengeInfoBtn = transform.Find("Texture").GetComponent<Button>();
        showChallengeInfoBtn.onClick.AddListener(ShowChallengeInfoBtnClicked);
    }

    public void Init(int index, Action<int> action)
    {
        this.action = action;
        this.index = index;
    }

    public void RefreshInfo(VictoryArenaOppoUnit data)
    {
        if(null != data)
        {
            TextHelper.SetText(roleNameText, data.Name.ToStringUtf8());
            TextHelper.SetText(levelText, 2024712u, data.Level.ToString());
            TextHelper.SetText(pointText, 2024713u, data.Score.ToString());
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(data.Career);
            if(null!= cSVCareerData)
            {
                ImageHelper.SetIcon(careerImage, cSVCareerData.logo_icon);
            }
            else
            {
                DebugUtil.LogError($"Not Find {data.Career} In CSVCareer");
            }
            var rankNum = data.Rank;
            bg_100afterImage.gameObject.SetActive(rankNum > 99);
            bg_4_99Image.gameObject.SetActive(3 < rankNum && rankNum < 100);
            bool isHasMedel = rankNum < 4;
            bg_1_3Image.gameObject.SetActive(isHasMedel);
            rankImage.gameObject.SetActive(isHasMedel);
            rankNumText.gameObject.SetActive(!isHasMedel);
            if (isHasMedel)
            {
                uint iconId = Sys_Rank.Instance.GetRankIcon((int)rankNum);
                ImageHelper.SetIcon(rankImage, iconId);
            }
            else
            {
                TextHelper.SetText(rankNumText, rankNum.ToString());
            }
        }
    }

    private void ChallengeBtnClicked()
    {
        RoleVictoryArenaDaily roleVictoryArenaDaily = Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();
        if (null != roleVictoryArenaDaily)
        {
            if(roleVictoryArenaDaily.LeftChallengeTimes > 0)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024703));
                }
                else
                {
                    Sys_JSBattle.Instance.VictoryArenaChallengeReq((uint)index);
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024707));
                if (roleVictoryArenaDaily.LeftBuyTimes > 0)
                {
                    var buyCostData = CSVDecisiveArenaParam.Instance.GetConfData(8);
                    var buyGetTimesData = CSVDecisiveArenaParam.Instance.GetConfData(9);

                    if (null != buyCostData && null != buyGetTimesData)
                    {
                        List<uint> costList = ReadHelper.ReadArray_ReadUInt(buyCostData.str_value, '|');
                        if (costList.Count >= 2)
                        {
                            ItemIdCount itemIdCount = new ItemIdCount(costList[0], costList[1]);
                            string str = LanguageHelper.GetTextContent(2024701, itemIdCount.count.ToString(), LanguageHelper.GetTextContent(itemIdCount.CSV.name_id), buyGetTimesData.str_value, roleVictoryArenaDaily.LeftBuyTimes.ToString());
                            PromptBoxParameter.Instance.OpenPromptBox(str, 0,
                            () => {
                                if (itemIdCount.Enough)
                                {
                                    Sys_JSBattle.Instance.VictoryArenaBuyChallengeTimesReq();
                                }
                                else
                                {
                                    Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.GoldCoin, itemIdCount.count);
                                }
                            });
                        }
                    }
                }
            }
        }
    }



    private void ShowChallengeInfoBtnClicked()
    {
        action?.Invoke(index);
    }
}

public class JsBattleShowModelItem
{
    public VirtualGameObject VGO { get; set; } = null;

    public JsBattleModelShowBase ModelShow { get; private set; } = null;

    public void SetModelShow(JsBattleModelShowBase modelshow)
    {
        if (ModelShow == modelshow)
        {
            ModelShow.SetActive(true);
            return;
        }
        ModelShow = modelshow;

        if (ModelShow != null)
        {
            ModelShow.SetActive(true);
            ModelShow.SetParent(VGO);
        }
    }
}

public class JsBattleModelShowBase
{
    public VirtualGameObject Parent { get; set; }

    public GameObject go;
    public uint index { get; set; }

    public bool IsUsed { get; set; } = false;
    public virtual void LoadModel(uint id, uint caree = 0, uint weaponId = 0, Dictionary<uint, List<dressData>> DressValue = null, bool isPreview = true)
    {
    }

    public virtual void SetActive(bool active)
    {

    }

    public virtual void ChangeWeapon(uint weaponId)
    {

    }
    public virtual void SetParent(VirtualGameObject parent)
    {
        Parent = parent;
    }
    public virtual void Dispose()
    {
    }
}

public class JsBattleModelShow: JsBattleModelShowBase
{
    public uint ModelID { get; set; }

    public ulong RoleID { get; set; }

    public uint DressID { get; set; } = 0;

    public uint WeaponID { get; set; }

    protected HeroLoader m_heroLoader;

    private void DisplayControlLoaded(int intValue)
    {
        if (m_heroLoader == null || m_heroLoader.heroDisplay.bMainPartFinished == false)
            return;

        CSVCharacter.Data cSVCharacterData = CSVCharacter.Instance.GetConfData(ModelID);

        if (cSVCharacterData == null)
            return;

        if (DressID == 0)
            return;
        go = m_heroLoader.heroDisplay?.GetPart(EHeroModelParts.Main).gameObject;
        go.SetActive(false);

        uint id = (uint)(DressID * 10000 + ModelID);
        CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);

        var activeid = cSVFashionModelData.action_show_id;


        m_heroLoader.heroDisplay?.mAnimation.UpdateHoldingAnimations(activeid, WeaponID == 0 ? cSVCharacterData.show_weapon_id : WeaponID,
        Constants.IdleAndRunAnimationClipHashSet, go: go);
    }

    public override void LoadModel(uint heroID, uint occupation, uint weaponId, Dictionary<uint, List<dressData>> DressValue, bool isPreview = true)
    {
        ModelID = heroID;

        m_heroLoader = HeroLoader.Create(true);


        m_heroLoader.LoadHero(heroID, weaponId, ELayerMask.ModelShow, DressValue, o =>
        {

            m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(Parent, null);
        });


        m_heroLoader.heroDisplay.onLoaded += DisplayControlLoaded;
    }

    public override void ChangeWeapon(uint weaponId)
    {
        m_heroLoader.LoadWeaponPart(m_heroLoader.showParts[(int)EHeroModelParts.Weapon], weaponId);
    }

    public override void SetParent(VirtualGameObject parent)
    {
        if (Parent == parent)
            return;

        Parent = parent;

        if (m_heroLoader == null || m_heroLoader.heroDisplay == null)
            return;

        var value = m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

        if (value == null)
            return;

        value.SetParent(Parent, null);
    }

    public override void SetActive(bool active)
    {
        if (m_heroLoader == null || m_heroLoader.heroDisplay == null)
            return;

        var value = m_heroLoader.heroDisplay.GetPart(EHeroModelParts.Main);

        if (value == null)
            return;

        if (value.gameObject == null)
            return;

        if (value.gameObject.activeSelf != active)
        {
            value.gameObject.SetActive(active);

            m_heroLoader.heroDisplay?.mAnimation.Play((uint)EStateType.Idle);
        }


    }

    public override void Dispose()
    {
        Parent = null;
        go = null;

        m_heroLoader?.Dispose();

        m_heroLoader = null;
    }
}

class JsBattlePetModelShow : JsBattleModelShowBase
{
    private CSVPetNew.Data petData;
    protected DisplayControl<EPetModelParts> m_DisplayControl;
    private uint fashionId;
    public override void LoadModel(uint id, uint caree = 0, uint weaponid = 0, Dictionary<uint, List<dressData>> DressValue = null, bool isPreview = true)
    {
        fashionId = weaponid;
        petData = CSVPetNew.Instance.GetConfData(id);
        m_DisplayControl = DisplayControl<EPetModelParts>.Create((int)EHeroModelParts.Count);
        m_DisplayControl.eLayerMask = ELayerMask.ModelShow;
        m_DisplayControl.onLoaded = OnLoadend;

        m_DisplayControl.LoadMainModel(EPetModelParts.Main, petData.model_show, EPetModelParts.None, "Pos");

    }


    private void OnLoadend(int id)
    {
        if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
            return;
        go = m_DisplayControl.GetPart(EPetModelParts.Main).gameObject;
        go.SetActive(false);
        uint showFashion = 0;
        if (fashionId > 0)
        {
            CSVPetEquipSuitAppearance.Data suitAppearanceData = CSVPetEquipSuitAppearance.Instance.GetConfData(fashionId);
            if (null != suitAppearanceData)
            {
                showFashion = suitAppearanceData.show_id;
            }
        }
        SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, showFashion, go.transform);
        var value = m_DisplayControl.GetPart(id);
        value.gameObject.transform.SetParent(Parent.transform, false);
        m_DisplayControl.mAnimation.UpdateHoldingAnimations(petData.action_id_show, Constants.UMARMEDID, go: go);
    }

    public override void SetParent(VirtualGameObject parent)
    {
        if (Parent == parent)
            return;

        Parent = parent;

        if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
            return;

        var value = m_DisplayControl.GetPart(EPetModelParts.Main);

        if (value != null)
            value.gameObject.transform.SetParent(Parent.transform, false);
    }

    public override void SetActive(bool active)
    {
        if (m_DisplayControl == null || m_DisplayControl.bMainPartFinished == false)
            return;

        var value = m_DisplayControl.GetPart(EPetModelParts.Main);

        if (value != null && value.gameObject.activeSelf != active)
        {
            value.gameObject.SetActive(active);
            m_DisplayControl?.mAnimation.Play((uint)EStateType.Idle);
        }

    }
    public override void Dispose()
    {
        Parent = null;
        go = null;
        DisplayControl<EPetModelParts>.Destory(ref m_DisplayControl);
    }
}