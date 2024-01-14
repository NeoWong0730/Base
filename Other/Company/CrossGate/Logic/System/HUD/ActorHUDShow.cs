using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Lib.Core;
using DG.Tweening;
using Framework;

namespace Logic
{
    public class ActorHUDShow : IHUDComponent
    {
        public GameObject mRootGameObject;
        private RectTransform rectTransform;
        private Text m_Name;
        private Vector3 offest;
        private uint appellation;
        public string name;
        public bool hudBack;

        private GameObject m_NpcRoot;
        private GameObject m_HeroRoot;
        private NpcActorShow m_NpcActorShow;
        private HeroActorShow m_HeroActorShow;

        private Vector3 mountOffest;
        private bool b_OnMount;
        private TansformWorldToScreen.WorldToScreenData _worldToScreenData;
        private TansformWorldToScreen _tansformWorldToScreen;

        public ulong uID;

        public void Construct(GameObject gameObject, TansformWorldToScreen tansformWorldToScreen)
        {
            mRootGameObject = gameObject;
            _tansformWorldToScreen = tansformWorldToScreen;
            _worldToScreenData = tansformWorldToScreen.Request();
            _worldToScreenData.to = gameObject.transform as RectTransform;

            m_NpcActorShow = HUDFactory.Get<NpcActorShow>();
            m_HeroActorShow = HUDFactory.Get<HeroActorShow>();
            ParseCp();
        }

        public void SetUID(ulong actorId)
        {
            uID = actorId;
            m_HeroActorShow.uID = uID;

            m_NpcActorShow.SetUID(uID);
        }

        private void ParseCp()
        {
            rectTransform = mRootGameObject.transform as RectTransform;
            m_NpcRoot = mRootGameObject.transform.Find("Root/NpcRoot").gameObject;
            m_HeroRoot = mRootGameObject.transform.Find("Root/HeroRoot").gameObject;
            m_Name = mRootGameObject.transform.Find("Root/NpcRoot/Text_Name").GetComponent<Text>();
            m_NpcActorShow.BindGameObject(m_NpcRoot);
            m_HeroActorShow.BindGameObject(m_HeroRoot);
        }

        public void SetTarget(Transform transform)
        {
            _worldToScreenData.from = transform;
        }

        public void CalOffest(Vector3 _offest)
        {
            offest = _offest;
            _worldToScreenData.positionOffset = _offest;
        }

        public void ShowOrHide(bool flag)
        {
            mRootGameObject.SetActive(flag);
        }

        public void SetAppellation_Name(EFightOutActorType eFightOutActorType, uint _appellation, string _name,bool isBack=false)
        {
            appellation = _appellation;
            name = _name;
            string content = BackNameString(_name, isBack);
            Sys_HUD.Instance.SetOutFightNameText(eFightOutActorType, m_Name, content);
            Sys_HUD.Instance.SetOutFightAppellationText(eFightOutActorType, m_NpcActorShow.Location, _appellation);
            m_NpcActorShow.SetAppellation(appellation);
        }

        public void UpdateName(EFightOutActorType eFightOutActorType, string _name,bool isBack=false)
        {
            name = _name;
            string content = BackNameString(_name,isBack);
            Sys_HUD.Instance.SetOutFightNameText(eFightOutActorType, m_Name, content);
        }

        public void UpdateAppellatiom_Name(EFightOutActorType eFightOutActorType,bool isBack=false)
        {
            string content = BackNameString(name, hudBack);
            Sys_HUD.Instance.SetOutFightNameText(eFightOutActorType, m_Name, content);
            Sys_HUD.Instance.SetOutFightAppellationText(eFightOutActorType, m_NpcActorShow.Location, appellation);
        }
        private string BackNameString(string _name,bool _back=false)
        {
            hudBack = _back;
            string _content = string.Empty;
            if (hudBack)
            {
                _content = string.Concat(LanguageHelper.GetTextContent(2014918),_name);
            }
            else
            {
                _content = name;
            }
            return _content;
        }

        #region Npc
        #region 功能提示
        public void UpdateState(uint iconId)
        {
            m_NpcActorShow.UpdateState(iconId);
        }

        public void UpdateStateFlag(int type)
        {
            m_NpcActorShow.UpdateStateFlag(type);
        }

        public void ClearStateFlag()
        {
            m_NpcActorShow.ClearStateFlag();
        }

        public void ShowOrHideFun1Icon(bool flag)
        {
            m_NpcActorShow.ShowOrHideFun1Icon(flag);
        }

        public void ShowOrHideFun2Icon(bool flag)
        {
            m_NpcActorShow.ShowOrHideFun2Icon(flag);
        }

        public void ShowOrHideFun3Icon(bool flag)
        {
            m_NpcActorShow.ShowOrHideFun3Icon(flag);
        }
        #endregion

        #region 好感度
        public void UpdateFavirability(uint val)
        {
            m_NpcActorShow.UpdateFavirability(val);
        }

        public void ClearFavirability()
        {
            m_NpcActorShow.ClearFavirability();
        }
        #endregion

        #region 世界boss
        public void UpdateBattleState(bool _bInBattleState)
        {
            m_NpcActorShow.UpdateBattleState(_bInBattleState);
        }

        public void UpdateWorldBoss(uint _level, uint _iconId)
        {
            m_NpcActorShow.UpdateWorldBoss(_level, _iconId);
        }

        public void ClearWorldBoss()
        {
            m_NpcActorShow.ClearWorldBoss();
        }
        #endregion

        #region BattleCd
        public void CreateNpcBattleCd(int cdTime)
        {
            m_NpcActorShow.CreateNpcBattleCd(cdTime);
        }
        #endregion


        #region 箭头
        public void ShowNpcArrow()
        {
            m_NpcActorShow.ShowNpcArrow();
        }

        public void HideNpcArrow()
        {
            m_NpcActorShow.HideNpcArrow();
        }
        #endregion

        #region 红色感叹号

        public void ShowSliderNotice(uint durtion)
        {
            m_NpcActorShow.ShowSliderNotice(durtion);
        }

        public void HideSliderNotice()
        {
            m_NpcActorShow.HideSliderNotice();
        }

        #endregion

        #endregion

        #region Hero
        #region Fx
        public void PlayLevelUpFx()
        {
            m_HeroActorShow.PlayLevelUpFx();
        }

        public void PlayAdvanceUpFx()
        {
            m_HeroActorShow.PlayAdvanceUpFx();
        }

        public void PlayReputationUpFx()
        {
            m_HeroActorShow.PlayReputationUpFx();
        }

        public void ClearFx()
        {
            m_HeroActorShow.ClearFx();
        }

        public void UpdateHeroFunState(uint state)
        {
            m_HeroActorShow.UpdateHeroFunState(state);
        }
        #endregion
        #region Title
        public void ClearTitle()
        {
            m_HeroActorShow.ClearTitle();
        }

        public void CreateTitle(uint title, string name, uint pos)
        {
            m_HeroActorShow.CreateTitle(title, name, pos);
        }

        public void UpdateFamilyTitleName(uint title)
        {
            m_HeroActorShow.UpdateFamilyTitleName(title);
        }

        public void UpdateBGroupTitleName(uint title, string name, uint pos)
        {
            m_HeroActorShow.UpdateBGroupTitleName(title, name, pos);
        }

        #endregion

        #region TeamLogo
        public void CreateTeamLogo(uint teamLogoId)
        {
            m_HeroActorShow.CreateTeamLogo(teamLogoId);
        }

        public void ClearTeamLogo()
        {
            m_HeroActorShow.ClearTeamLogo();
        }


        public void CreateTeamFx()
        {
            m_HeroActorShow.CreateTeamFx();
        }

        public void ClearTeamFx()
        {
            m_HeroActorShow.ClearTeamFx();
        }

        #endregion

        #region FamilyResourceBattle
        public void OnCreateFamilyBattle(string famliyName, uint pos)
        {
            m_HeroActorShow?.CreateFamilyBattle(famliyName, pos);
        }

        public void OnClearFamilyBattle()
        {
            m_HeroActorShow?.ExitFamilyBattle();
        }

        public void OnUpdateFamilyName(string name, uint pos)
        {
            m_HeroActorShow?.UpdateFamilyName(name, pos);
        }

        public void OnUpdateFamilyBattleResource(uint resourceId)
        {
            m_HeroActorShow?.OnUpdateFamilyBattleResource(resourceId);
        }

        public void OnUpdateGuildBattleName()
        {
            Sys_HUD.Instance.SetNameRed(m_Name, name);
        }

        public void OnUpdateFamilyTeamNum(uint teamNum, uint maxCount)
        {
            m_HeroActorShow?.OnUpdateFamilyTeamNum(teamNum, maxCount);
        }

        #endregion

        public void OnUpMount(Vector3 offest)
        {
            if (!b_OnMount)
            {
                b_OnMount = true;
                mountOffest = offest;
                _worldToScreenData.positionOffset += mountOffest;
            }
        }

        public void OnDownMount()
        {
            if (b_OnMount)
            {
                _worldToScreenData.positionOffset -= mountOffest;
                b_OnMount = false;
                mountOffest = Vector3.zero;
            }
        }

        public void OnScaleUp(uint scale)
        {
            float newOffest = offest.y * scale / 100f;
            _worldToScreenData.positionOffset = new Vector3(0, newOffest, 0);
        }

        public void OnResetScale()
        {
            _worldToScreenData.positionOffset = offest;
        }
        #endregion

        public void Dispose()
        {
            mountOffest = Vector3.zero;
            b_OnMount = false;
            ClearFx();
            ClearStateFlag();
            name = string.Empty;
            appellation = 0;
            _tansformWorldToScreen.Release(ref _worldToScreenData);
            if (mRootGameObject != null)
            {
                mRootGameObject.SetActive(false);
            }
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
            }
            if (m_Name != null)
            {
                m_Name.text = string.Empty;
            }
            m_HeroActorShow.Dispose();
            m_NpcActorShow.Dispose();
            HUDFactory.Recycle(m_HeroActorShow);
            HUDFactory.Recycle(m_NpcActorShow);
        }

        public class NpcActorShow : IHUDComponent
        {
            private GameObject m_Go;
            public Text Location;                   //称谓

            private Image icon;                     //爱心任务
            private bool bFuncPrompt1;              //功能提示1（正上方 爱心任务等等）

            private bool bFuncPrompt2;              //功能提示2（右上方 调查报告等等）
            private int type;                       //功能提示2 具体显示类型
            private GameObject asynChild;           //功能提示2
            private Image asynIcon;                 //功能提示2

            private Transform m_NpcFunStateParent;
            private AsyncOperationHandle<GameObject> requestRef_FunState;
            private GameObject m_funStateObj;
            private uint appellation;

            private GameObject worldBoss_outBattle;
            private Text worldBoss_level;
            private Image worldBoss_Icon;
            private GameObject worldBoss_inBattle;
            private bool bFuncPrompt3;              //功能提示3 世界bosshud是否触发
            private uint level;
            private bool _bInBattleState = false;   //是否在战斗中
            private bool bInBattleState
            {
                get { return _bInBattleState; }
                set
                {
                    if (_bInBattleState != value)
                    {
                        _bInBattleState = value;
                        if (_bInBattleState)
                        {
                            worldBoss_outBattle.SetActive(false);
                            worldBoss_inBattle.SetActive(bFuncPrompt3);
                        }
                        else
                        {
                            worldBoss_inBattle.SetActive(false);
                            worldBoss_outBattle.SetActive(bFuncPrompt3);
                            worldBoss_level.text = level.ToString();
                        }
                    }
                }
            }

            private GameObject favirabilityRoot;
            private Image favirabilityicon;
            private Text favirabilityVal;

            private GameObject m_NpcBattleCd;
            private Text m_NpcBattleShow;
            private int m_CdTime;
            private Timer m_NpcBattleTimer;

            private GameObject m_SliderNotice;
            private Image m_Slider1;
            private Image m_Slider2;
            private Image m_Slider3;

            private GameObject m_NpcArrow;

            private Npc m_Npc;
            public ulong uID;
            private CSVCollection.Data m_CSVCollectionData;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                icon = m_Go.transform.Find("Image_Question").GetComponent<Image>();
                Location = m_Go.transform.Find("Text_Location").GetComponent<Text>();
                asynChild = m_Go.transform.Find("View_Emotion").gameObject;
                asynIcon = m_Go.transform.Find("View_Emotion/View_Emotion01/Image_EmotionBG").GetComponent<Image>();
                m_NpcFunStateParent = m_Go.transform.Find("FunctionState");

                worldBoss_outBattle = m_Go.transform.Find("WorldBoss/Image").gameObject;
                worldBoss_inBattle = m_Go.transform.Find("WorldBoss/Batting").gameObject;
                worldBoss_level = m_Go.transform.Find("WorldBoss/Image/Text").GetComponent<Text>();
                worldBoss_Icon = m_Go.transform.Find("WorldBoss/Image").GetComponent<Image>();

                favirabilityRoot = m_Go.transform.Find("Text_Name/Favirability").gameObject;
                favirabilityVal = m_Go.transform.Find("Text_Name/Favirability/Text").GetComponent<Text>();
                favirabilityicon = favirabilityRoot.GetComponent<Image>();

                m_NpcBattleCd = m_Go.transform.Find("BattingCd").gameObject;
                m_NpcBattleShow = m_Go.transform.Find("BattingCd/GameObject").GetComponent<Text>();
                HideNpcBattleCd();
                m_NpcFunStateParent.gameObject.SetActive(false);

                m_SliderNotice = m_Go.transform.Find("Slide_Notice").gameObject;
                m_Slider1 = m_Go.transform.Find("Slide_Notice/Slider01/Fill").GetComponent<Image>();
                m_Slider2 = m_Go.transform.Find("Slide_Notice/Slider02/Fill").GetComponent<Image>();
                m_Slider3 = m_Go.transform.Find("Slide_Notice/Slider03/Fill").GetComponent<Image>();

                m_NpcArrow = m_Go.transform.Find("Image_Arrow").gameObject;
            }

            public void SetUID(ulong uID)
            {
                this.uID = uID;
                GameCenter.TryGetSceneNPC(uID, out m_Npc);
                if (m_Npc != null)
                {
                    m_CSVCollectionData = CSVCollection.Instance.GetConfData(m_Npc.cSVNpcData.CollectionID);
                }
            }

            public void SetAppellation(uint appe)
            {
                appellation = appe;
            }

            #region 功能提示1
            public void UpdateState(uint iconId)
            {
                if (iconId == 0)
                {
                    bFuncPrompt1 = false;
                    icon.gameObject.SetActive(false);
                    return;
                }
                icon.gameObject.SetActive(true);
                ImageHelper.SetIcon(icon, iconId);
                icon.SetNativeSize();
                bFuncPrompt1 = true;
            }

            public void ShowOrHideFun1Icon(bool flag)
            {
                if (bFuncPrompt1)
                {
                    icon.gameObject.SetActive(flag);
                }
            }
            #endregion

            #region          功能提示2
            public void UpdateStateFlag(int type)
            {
                this.type = type;
                bFuncPrompt2 = true;
                uint iconId = 0;
                switch (type)
                {
                    case 1:
                        DestroyFunStateObject();
                        iconId = 994001;
                        asynChild.SetActive(true);
                        ImageHelper.SetIcon(asynIcon, iconId, true);
                        break;
                    case 2:
                        DestroyFunStateObject();
                        iconId = 994002;
                        asynChild.SetActive(true);
                        ImageHelper.SetIcon(asynIcon, iconId, true);
                        break;
                    case 3:
                        DestroyFunStateObject();
                        iconId = 994003;
                        asynChild.SetActive(true);
                        ImageHelper.SetIcon(asynIcon, iconId, true);
                        break;
                    case 4: //采集
                        DestroyFunStateObject();
                        if (m_Npc != null)
                        {
                            iconId = m_CSVCollectionData.collectionFun_icon;
                            asynChild.SetActive(true);
                            ImageHelper.SetIcon(asynIcon, iconId, true);
                        }
                        break;
                    case 5:
                        DestroyFunStateObject();
                        iconId = 994005;
                        asynChild.SetActive(true);
                        ImageHelper.SetIcon(asynIcon, iconId, true);
                        break;
                    case 6:
                        if (asynChild.activeSelf)
                        {
                            asynChild.SetActive(false);
                        }
                        m_NpcFunStateParent.gameObject.SetActive(true);
                        DestroyFunStateObject();
                        m_funStateObj = GameObject.Instantiate<GameObject>(HUD.prefab_ViewCookAsset);
                        SetFunStateObj();
                        break;
                    case 7:
                        if (asynChild.activeSelf)
                        {
                            asynChild.SetActive(false);
                        }
                        m_NpcFunStateParent.gameObject.SetActive(true);
                        DestroyFunStateObject();
                        m_funStateObj = GameObject.Instantiate<GameObject>(HUD.prefab_ViewShopAsset);
                        SetFunStateObj();
                        break;
                    default:
                        break;
                }
            }

            private void SetFunStateObj()
            {
                if (null != m_funStateObj)
                {
                    m_funStateObj.transform.SetParent(m_NpcFunStateParent);
                    RectTransform rectTransform = m_funStateObj.transform as RectTransform;
                    rectTransform.localPosition = Vector3.zero;
                    rectTransform.localEulerAngles = Vector3.zero;
                    rectTransform.localScale = Vector3.one;

                    if (appellation != 0)
                    {
                        rectTransform.anchoredPosition = new Vector2(0, 55);
                    }
                    else
                    {
                        rectTransform.anchoredPosition = new Vector2(0, 0);
                    }
                }
            }

            private void DestroyFunStateObject()
            {
                if (m_funStateObj != null)
                {
                    GameObject.Destroy(m_funStateObj);
                }
            }

            public void ClearStateFlag()
            {
                asynChild.SetActive(false);
                m_NpcFunStateParent.gameObject.SetActive(false);
                DestroyFunStateObject();
                bFuncPrompt2 = false;
                type = 0;
            }

            public void ShowOrHideFun2Icon(bool flag)
            {
                if (bFuncPrompt2)
                {
                    if (flag)
                    {
                        UpdateStateFlag(type);
                    }
                    else
                    {
                        asynChild.SetActive(false);
                        m_NpcFunStateParent.gameObject.SetActive(false);
                    }
                }
            }
            #endregion


            #region 世界boss
            public void UpdateBattleState(bool _bInBattleState)
            {
                bInBattleState = _bInBattleState;
            }

            public void UpdateWorldBoss(uint _level, uint _iconId)
            {
                worldBoss_outBattle.SetActive(true);
                bFuncPrompt3 = true;
                level = _level;
                worldBoss_level.text = _level.ToString();
                ImageHelper.SetIcon(worldBoss_Icon, _iconId);
            }

            public void ClearWorldBoss()
            {
                worldBoss_outBattle.SetActive(false);
                bFuncPrompt3 = false;
            }

            public void ShowOrHideFun3Icon(bool flag)
            {
                if (bFuncPrompt3)
                {
                    if (flag)
                    {
                        if (bInBattleState)
                        {
                            worldBoss_inBattle.SetActive(true);
                            worldBoss_outBattle.SetActive(false);
                        }
                        else
                        {
                            worldBoss_inBattle.SetActive(false);
                            worldBoss_outBattle.SetActive(true);
                            worldBoss_level.text = level.ToString();
                        }
                    }
                    else
                    {
                        worldBoss_inBattle.SetActive(false);
                        worldBoss_outBattle.SetActive(false);
                    }
                }
            }
            #endregion

            #region 好感度
            public void UpdateFavirability(uint val)
            {
                favirabilityRoot.SetActive(true);
                favirabilityVal.text = val.ToString();
            }

            public void ClearFavirability()
            {
                favirabilityRoot.SetActive(false);
            }

            #endregion

            #region BattleCd
            public void CreateNpcBattleCd(int cdTime)
            {
                m_NpcBattleCd.SetActive(true);
                m_CdTime = cdTime;
                m_NpcBattleTimer?.Cancel();
                m_NpcBattleTimer = Timer.Register(cdTime, HideNpcBattleCd, OnNpcBattleCdUpdateCallback);
            }

            private void HideNpcBattleCd()
            {
                m_NpcBattleShow.text = string.Empty;
                m_NpcBattleCd.SetActive(false);
            }

            private void OnNpcBattleCdUpdateCallback(float dt)
            {
                int remainTime = 0;
                remainTime = m_CdTime - (int)dt;
                TextHelper.SetText(m_NpcBattleShow, remainTime.ToString());
            }
            #endregion

            #region 箭头
            public void ShowNpcArrow()
            {
                m_NpcArrow.SetActive(true);
            }

            public void HideNpcArrow()
            {
                m_NpcArrow.SetActive(false);
            }
            #endregion

            #region 红色感叹号

            public void ShowSliderNotice(uint duration)
            {
                m_SliderNotice.SetActive(true);
                DOTween.To(() => { return m_Slider1.fillAmount; }, value => { m_Slider1.fillAmount = value; }, 1, duration / 1000f);
                DOTween.To(() => { return m_Slider2.fillAmount; }, value => { m_Slider2.fillAmount = value; }, 1, duration / 1000f);
                DOTween.To(() => { return m_Slider3.fillAmount; }, value => { m_Slider3.fillAmount = value; }, 1, duration / 1000f).onComplete = HideSliderNotice;
            }

            public void HideSliderNotice()
            {
                m_SliderNotice.SetActive(false);
                m_Slider1.fillAmount = 0;
                m_Slider2.fillAmount = 0;
                m_Slider3.fillAmount = 0;
            }

            #endregion

            public void Dispose()
            {
                bFuncPrompt1 = false;
                if (icon != null)
                {
                    icon.gameObject.SetActive(false);
                }
                if (Location != null)
                {
                    Location.text = string.Empty;
                }
                bInBattleState = false;
                bFuncPrompt3 = false;
                if (worldBoss_outBattle != null)
                {
                    worldBoss_outBattle.SetActive(false);
                }
                if (worldBoss_inBattle != null)
                {
                    worldBoss_inBattle.SetActive(false);
                }
                if (worldBoss_level != null)
                {
                    worldBoss_level.text = string.Empty;
                }

                if (favirabilityRoot != null)
                {
                    favirabilityRoot.SetActive(false);
                }
                if (favirabilityVal != null)
                {
                    favirabilityVal.text = string.Empty;
                }
                m_NpcBattleTimer?.Cancel();
                m_NpcBattleShow.text = string.Empty;
                m_NpcBattleCd.SetActive(false);
                appellation = 0;
                HideNpcArrow();
                HideSliderNotice();
            }
        }

        public class HeroActorShow : IHUDComponent
        {
            public enum FunState
            {
                None = 0,//无
                Battle = 1,//战斗
                Collect = 2,//采集
            }

            private GameObject m_Go;

            private bool b_HasTitle;
            private GameObject titleRoot;
            private Text mTitle_text1;
            private Text mTitle_text2;
            private Image mTitle_img2;
            private Image mTitle_img3;
            private Transform mTitle_Fx3parent;
            private AsyncOperationHandle<GameObject> requestRef_Title;
            private GameObject titleEffect;

            private GameObject levelUp;
            private GameObject advanceUp;
            private GameObject reputationUp;
            private Timer timerLevelUp;
            private Timer timerAdvance;
            private Timer timerReputation;
            private float time;

            private Transform m_HeroFunStateParent;
            private AsyncOperationHandle<GameObject> requestRef_FunState;
            private GameObject m_funStateObj;
            private Vector3 m_OffestInFamilyResBattle_HasRes;
            private Vector3 m_OffestInFamilyResBattle_HasNoneRes;

            private Transform m_TeamParent;
            private GameObject m_TeamInstantiateGo;
            private Vector3 m_LogoSpawnOffest;
            private Vector3 m_LogoDynamicOffest;
            private uint m_TeamLogoId;

            private Transform m_FamilyBattleParent;
            private GameObject m_FamilyBattleGo;
            private bool b_EnterFamilyBattle;
            private Vector3 m_FamilyBattleSpawnOffest;
            public ulong uID;


            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                titleRoot = m_Go.transform.Find("Title").gameObject;
                mTitle_text1 = m_Go.transform.Find("Title/Text").GetComponent<Text>();
                mTitle_text2 = m_Go.transform.Find("Title/Image/Text").GetComponent<Text>();
                mTitle_img2 = m_Go.transform.Find("Title/Image").GetComponent<Image>();
                mTitle_img3 = m_Go.transform.Find("Title/Image1").GetComponent<Image>();
                mTitle_Fx3parent = m_Go.transform.Find("Title/Image1/Fx");
                m_TeamParent = m_Go.transform.Find("TeamMark");
                m_FamilyBattleParent = m_Go.transform.Find("FamilyBattle");
                titleRoot.SetActive(false);
                time = float.Parse(CSVParam.Instance.GetConfData(810).str_value) / 1000f;

                m_HeroFunStateParent = m_Go.transform.Find("FunctionState");

                m_LogoDynamicOffest = new Vector3(0, 70, 0);

                m_OffestInFamilyResBattle_HasNoneRes = new Vector3(0, 30, 0);
                m_OffestInFamilyResBattle_HasRes = new Vector3(0, 60, 0);
            }

            #region TeamLogo

            public void CreateTeamLogo(uint teamLogoId)
            {
                CSVTeamLogo.Data cSVTeamLogoData = CSVTeamLogo.Instance.GetConfData(teamLogoId);
                if (cSVTeamLogoData != null)
                {
                    ClearTeamLogo();
                    m_TeamLogoId = teamLogoId;
                    if (cSVTeamLogoData.TeamIcon == 0)
                    {
                        return;
                    }
                    m_TeamInstantiateGo = GameObject.Instantiate<GameObject>(HUD.prefab_TeamLogoAsset);
                    m_TeamInstantiateGo.transform.SetParent(m_TeamParent);
                    m_TeamInstantiateGo.transform.localPosition = m_LogoSpawnOffest;
                    m_TeamInstantiateGo.transform.localScale = Vector3.one;
                    Image teamIcon = m_TeamInstantiateGo.transform.Find("Image_TeamMark").GetComponent<Image>();
                    ImageHelper.SetIcon(teamIcon, cSVTeamLogoData.TeamIcon);
                    if (b_HasTitle)
                    {
                        m_TeamInstantiateGo.transform.localPosition = m_LogoDynamicOffest + m_LogoSpawnOffest;
                    }
                    else
                    {
                        m_TeamInstantiateGo.transform.localPosition = m_LogoSpawnOffest;
                    }
                }
            }

            public void ClearTeamLogo()
            {
                if (m_TeamInstantiateGo != null)
                {
                    GameObject.Destroy(m_TeamInstantiateGo);
                }
            }

            public void CreateTeamFx()
            {
                ClearTeamFx();
                if (m_TeamInstantiateGo != null)
                {
                    GameObject fx = m_TeamInstantiateGo.transform.Find("Image_TeamMarkEff").gameObject;
                    CSVTeamLogo.Data cSVTeamLogoData = CSVTeamLogo.Instance.GetConfData(m_TeamLogoId);
                    if (cSVTeamLogoData != null)
                    {
                        if (cSVTeamLogoData.FullTeamIcon != 0)
                        {
                            fx.SetActive(true);
                            ImageHelper.SetIcon(fx.GetComponent<Image>(), cSVTeamLogoData.FullTeamIcon);
                        }
                    }
                }
            }

            public void ClearTeamFx()
            {
                if (m_TeamInstantiateGo != null)
                {
                    GameObject fx = m_TeamInstantiateGo.transform.Find("Image_TeamMarkEff").gameObject;
                    if (fx.activeSelf)
                    {
                        fx.SetActive(false);
                    }
                }
            }

            #endregion

            #region Title
            public void ClearTitle()
            {
                b_HasTitle = false;
                titleRoot.SetActive(false);
                if (m_TeamInstantiateGo != null)
                {
                    if (m_TeamInstantiateGo.activeSelf)
                    {
                        m_TeamInstantiateGo.transform.localPosition = m_LogoSpawnOffest;
                    }
                }
            }

            public void UpdateFamilyTitleName(uint titleId)
            {
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(titleId);
                if (cSVTitleData != null)
                {
                    if (titleId == Sys_Title.Instance.familyTitle)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);

                            uint pos = Sys_Family.Instance.familyData.CheckMe() == null ? 0 : Sys_Family.Instance.familyData.CheckMe().Position;
                            if (pos == 0)
                            {
                                TextHelper.SetText(mTitle_text1, LanguageHelper.GetTextContent(2005799, Sys_Family.Instance.GetFamilyName()));
                            }
                            else
                            {
                                TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleFamiltyName());
                            }
                            TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);

                            uint pos = Sys_Family.Instance.familyData.CheckMe() == null ? 0 : Sys_Family.Instance.familyData.CheckMe().Position;
                            if (pos == 0)
                            {
                                TextHelper.SetText(mTitle_text2, LanguageHelper.GetTextContent(2005799, Sys_Family.Instance.GetFamilyName()));
                            }
                            else
                            {
                                TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleFamiltyName());
                            }
                            TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                        }
                    }
                }
            }

            public void UpdateBGroupTitleName(uint titleId, string name, uint pos)
            {
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(titleId);
                if (cSVTitleData != null)
                {
                    if (titleId == Sys_Title.Instance.bGroupTitle)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleWarriorGroupName(name, pos));
                            TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleWarriorGroupName(name, pos));
                            TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                        }
                    }
                }
            }
            
            public void CreateTitle(uint title, string name, uint pos)
            {
                if (m_TeamInstantiateGo != null)
                {
                    if (m_TeamInstantiateGo.activeSelf)
                    {
                        m_TeamInstantiateGo.transform.localPosition = m_LogoDynamicOffest + m_LogoSpawnOffest;
                    }
                }
                b_HasTitle = true;
                titleRoot.SetActive(true);
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(title);
                if (cSVTitleData != null)
                {
                    if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);

                            if (pos == 0)
                            {
                                TextHelper.SetText(mTitle_text1, LanguageHelper.GetTextContent(2005799, name));
                            }
                            else
                            {
                                pos = pos % 10000;
                                CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(pos);
                                if (cSVFamilyPostAuthorityData != null)
                                {
                                    string postName = CSVLanguage.Instance.GetConfData(cSVFamilyPostAuthorityData.PostName).words;
                                    string str = LanguageHelper.GetTextContent(2005800, name, postName);
                                    TextHelper.SetText(mTitle_text1, str);
                                }
                            }
                            TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);

                            if (pos == 0)
                            {
                                TextHelper.SetText(mTitle_text2, LanguageHelper.GetTextContent(2005799, name));
                            }
                            else
                            {
                                pos = pos % 10000;
                                CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(pos);
                                if (cSVFamilyPostAuthorityData != null)
                                {
                                    string postName = CSVLanguage.Instance.GetConfData(cSVFamilyPostAuthorityData.PostName).words;
                                    string str = LanguageHelper.GetTextContent(2005800, name, postName);
                                    TextHelper.SetText(mTitle_text2, str);
                                }
                            }
                            TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                        }
                    }
                    else if(cSVTitleData.id == Sys_Title.Instance.bGroupTitle)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleWarriorGroupName(name, pos));
                            TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleWarriorGroupName(name, pos));
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
                                //TextHelper.SetTextGradient(mTitle_text1, cSVTitleData.titleShow[0], cSVTitleData.titleShow[1]);
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
                AddressablesUtil.InstantiateAsync(ref requestRef_Title, path, OnAssetsLoaded);
            }

            private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
            {
                titleEffect = handle.Result;
                if (null != titleEffect)
                {
                    titleEffect.transform.SetParent(mTitle_Fx3parent);
                    RectTransform rectTransform = titleEffect.transform as RectTransform;
                    rectTransform.localEulerAngles = Vector3.zero;
                    rectTransform.localScale = Vector3.one;
                    rectTransform.localPosition = Vector3.zero;
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

            #region Fx
            public void PlayLevelUpFx()
            {
                DestroyLevelUp();
                levelUp = GameObject.Instantiate<GameObject>(HUD.prefab_LevelUpAsset);
                levelUp.transform.SetParent(m_Go.transform);
                levelUp.transform.localPosition = new Vector3(50.2f, 14, 0);
                levelUp.transform.localScale = Vector3.one;

                timerLevelUp?.Cancel();
                timerLevelUp = Timer.Register(time, DestroyLevelUp);
            }

            private void DestroyLevelUp()
            {
                if (levelUp != null)
                {
                    GameObject.Destroy(levelUp);
                }
            }

            public void PlayAdvanceUpFx()
            {
                DestroyAdvanceUp();
                advanceUp = GameObject.Instantiate<GameObject>(HUD.prefab_AdvanceUpAsset);
                advanceUp.transform.SetParent(m_Go.transform);
                advanceUp.transform.localPosition = new Vector3(50.2f, 14, 0);
                advanceUp.transform.localScale = Vector3.one;

                timerAdvance?.Cancel();
                timerAdvance = Timer.Register(time, DestroyAdvanceUp);
            }

            private void DestroyAdvanceUp()
            {
                if (advanceUp != null)
                {
                    GameObject.Destroy(advanceUp);
                }
            }

            private bool hasShow = false;
            public void PlayReputationUpFx()
            {
                if (hasShow)
                {
                    timerReputation?.Cancel();
                    DestroyReputationUp();
                }
                hasShow = true;
                reputationUp = GameObject.Instantiate<GameObject>(HUD.prefab_PrestigeUpAsset);
                reputationUp.transform.SetParent(m_Go.transform);
                reputationUp.transform.localPosition = new Vector3(50.2f, 14, 0);
                reputationUp.transform.localScale = Vector3.one;
                timerReputation?.Cancel();
                timerReputation = Timer.Register(time, DestroyReputationUp);
            }

            private void DestroyReputationUp()
            {
                if (reputationUp != null)
                {
                    GameObject.Destroy(reputationUp);
                    hasShow = false;
                }
            }

            public void ClearFx()
            {
                timerLevelUp?.Cancel();
                DestroyLevelUp();

                timerReputation?.Cancel();
                DestroyReputationUp();

                timerAdvance?.Cancel();
                DestroyAdvanceUp();
            }
            #endregion

            #region FuncState
            public void UpdateHeroFunState(uint state)
            {
                FunState funState = (FunState)state;
                switch (funState)
                {
                    case FunState.None:
                        ClearFunState();
                        break;
                    case FunState.Battle:
                        ClearFunState();
                        m_funStateObj = GameObject.Instantiate<GameObject>(HUD.prefab_ViewBattingAsset);
                        SetFunStateObj();
                        //LoadHeroState(Sys_HUD.view_Battle);
                        break;
                    case FunState.Collect:
                        ClearFunState();
                        m_funStateObj = GameObject.Instantiate<GameObject>(HUD.prefab_ViewCollectAsset);
                        SetFunStateObj();
                        //LoadHeroState(Sys_HUD.view_Collect);
                        break;
                    default:
                        break;
                }
            }

            private void ClearFunState()
            {
                if (m_funStateObj != null)
                {
                    GameObject.Destroy(m_funStateObj);
                    m_funStateObj = null;
                }
            }

            private void SetFunStateObj()
            {
                if (null != m_funStateObj)
                {
                    m_funStateObj.transform.SetParent(m_HeroFunStateParent);
                    RectTransform rectTransform = m_funStateObj.transform as RectTransform;
                    if (b_EnterFamilyBattle)
                    {
                        rectTransform.localPosition = m_OffestInFamilyResBattle_HasNoneRes;
                    }
                    else
                    {
                        rectTransform.localPosition = Vector3.zero;
                    }
                    rectTransform.localEulerAngles = Vector3.zero;
                    rectTransform.localScale = Vector3.one;

                    if (b_HasTitle)
                    {
                        rectTransform.anchoredPosition = new Vector2(0, 55);
                    }
                    else
                    {
                        rectTransform.anchoredPosition = new Vector2(0, 0);
                    }
                }
            }

            #endregion

            #region FamilyBattle

            public void CreateFamilyBattle(string name, uint pos)
            {
                b_EnterFamilyBattle = true;
                if (m_FamilyBattleGo == null)
                {
                    m_FamilyBattleGo = GameObject.Instantiate<GameObject>(HUD.prefab_FamilyBattleAsset);
                    m_FamilyBattleGo.transform.SetParent(m_FamilyBattleParent);
                    m_FamilyBattleGo.transform.localPosition = m_FamilyBattleSpawnOffest;
                    m_FamilyBattleGo.transform.localScale = Vector3.one;
                }

                if (b_HasTitle)
                {
                    titleRoot.SetActive(false);
                }
                if (m_TeamParent.gameObject.activeSelf)
                {
                    m_TeamParent.gameObject.SetActive(false);
                }
                ////队伍
                //GameObject mumGo = m_FamilyBattleGo.transform.Find("Grid/People/Image_People").gameObject;
                //Text mumNumText = m_FamilyBattleGo.transform.Find("Grid/People/Image_People/Text_Num").GetComponent<Text>();

                //if (teamNum == 0)
                //{
                //    mumGo.SetActive(false);
                //}
                //else
                //{
                //    mumGo.SetActive(true);
                //    TextHelper.SetText(mumNumText, LanguageHelper.GetTextContent(3230000026, teamNum.ToString(), maxCount.ToString()));
                //}



                //家族名字
                Text familyName = m_FamilyBattleGo.transform.Find("Text_Family").GetComponent<Text>();
                if (pos == 0)
                {
                    TextHelper.SetText(familyName, LanguageHelper.GetTextContent(2005799, name));
                }
                else
                {
                    pos = pos % 10000;
                    CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(pos);
                    if (cSVFamilyPostAuthorityData != null)
                    {
                        string postName = CSVLanguage.Instance.GetConfData(cSVFamilyPostAuthorityData.PostName).words;
                        string str = LanguageHelper.GetTextContent(2005800, name, postName);
                        TextHelper.SetText(familyName, str);
                    }
                }

            }

            public void UpdateFamilyName(string name, uint pos)
            {
                if (m_FamilyBattleGo == null)
                {
                    DebugUtil.LogErrorFormat("UpdateFamilyName=======> m_FamilyBattleGo=null ");
                    return;
                }
                Text familyName = m_FamilyBattleGo.transform.Find("Text_Family").GetComponent<Text>();
                if (pos == 0)
                {
                    TextHelper.SetText(familyName, LanguageHelper.GetTextContent(2005799, name));
                }
                else
                {
                    pos = pos % 10000;
                    CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = CSVFamilyPostAuthority.Instance.GetConfData(pos);
                    if (cSVFamilyPostAuthorityData != null)
                    {
                        string postName = CSVLanguage.Instance.GetConfData(cSVFamilyPostAuthorityData.PostName).words;
                        string str = LanguageHelper.GetTextContent(2005800, name, postName);
                        TextHelper.SetText(familyName, str);
                    }
                }
            }

            public void ExitFamilyBattle()
            {
                if (b_HasTitle)
                {
                    titleRoot.SetActive(true);
                }
                m_TeamParent.gameObject.SetActive(true);
                ClearFamilyBattle();
            }



            public void ClearFamilyBattle()
            {
                b_EnterFamilyBattle = false;
                if (m_FamilyBattleGo != null)
                {
                    GameObject.Destroy(m_FamilyBattleGo);
                    m_FamilyBattleGo = null;
                }
            }

            public void OnUpdateFamilyBattleResource(uint id)
            {
                if (m_FamilyBattleGo == null)
                {
                    return;
                }
                Image image_Mark = m_FamilyBattleGo.transform.Find("Grid/Image_Mark").GetComponent<Image>();
                CSVFamilyResBattleResParameter.Data cSVFamilyResBattleResParameter = CSVFamilyResBattleResParameter.Instance.GetConfData(id);
                if (id == 0)
                {
                    image_Mark.gameObject.SetActive(false);
                    if (m_funStateObj != null)
                    {
                        RectTransform rectTransform = m_funStateObj.transform as RectTransform;
                        rectTransform.localPosition = m_OffestInFamilyResBattle_HasNoneRes;
                    }
                }
                else
                {
                    image_Mark.gameObject.SetActive(true);
                    if (m_funStateObj != null)
                    {
                        RectTransform rectTransform = m_funStateObj.transform as RectTransform;
                        rectTransform.localPosition = m_OffestInFamilyResBattle_HasRes;
                    }
                    ImageHelper.SetIcon(image_Mark, cSVFamilyResBattleResParameter.IconID);
                }
            }

            public void OnUpdateFamilyTeamNum(uint memNum, uint maxCount)
            {
                if (m_FamilyBattleGo == null)
                {
                    Debug.LogErrorFormat("m_FamilyBattleGo=null");
                    return;
                }
                GameObject go = m_FamilyBattleGo.transform.Find("Grid/People/Image_People").gameObject;
                Text mumNumText = m_FamilyBattleGo.transform.Find("Grid/People/Image_People/Text_Num").GetComponent<Text>();
                if (memNum == 0)
                {
                    go.SetActive(false);
                    TextHelper.SetText(mumNumText, string.Empty);
                }
                else
                {
                    go.SetActive(true);
                    TextHelper.SetText(mumNumText, LanguageHelper.GetTextContent(3230000026, memNum.ToString(), maxCount.ToString()));
                }
            }
            #endregion

            public void Dispose()
            {
                ClearFamilyBattle();
                b_HasTitle = false;
                if (titleRoot != null)
                {
                    titleRoot.SetActive(false);
                }
                if (m_TeamInstantiateGo != null)
                {
                    GameObject.Destroy(m_TeamInstantiateGo);
                }
                if (mTitle_text1 != null)
                {
                    mTitle_text1.text = string.Empty;
                }
                if (mTitle_text2 != null)
                {
                    mTitle_text2.text = string.Empty;
                }
                ClearFunState();
                m_TeamLogoId = 0;
                AddressablesUtil.ReleaseInstance(ref requestRef_Title, OnAssetsLoaded);
            }
        }
    }
}


