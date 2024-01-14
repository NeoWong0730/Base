using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public enum ERankDetailType
    {
        Player,
        Charactor,
        PVP,
        LadderPvp
    }

    public class UI_Rank_DetailParam
    {
        public ERankDetailType showType;
        public RankDescRole rankDescRole = null;
        public RankDescArena rankDescArena = null;
        public RankDescTianTi randDesTianti = null;
    }

    public class UI_Rank_Detail01 : UIBase
    {        
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        private Text titleText;
        private RawImage rawImage;
        private Button closeBtn;
        private Text nameText;

        private GameObject playerGo;
        private Text totalScoreText;
        private Button playerBtn;
        private Text roleScore;
        private Text petScore;  

        private GameObject pvpGo;
        private Text pvpRankNameText;
        private Text pvpRankStarText;
        private GameObject ImagePartnerGo;
        private Text pvpData1Text;
        private Text pvpData2Text;
        private Text pvpData3Text;
        private Text pvpData4Text;

        private UI_Rank_DetailParam uI_Rank_DetailParam;
        AsyncOperationHandle<GameObject> m_Hand;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseUI);

            titleText = transform.Find("Animator/View_Title/Text_Title").GetComponent<Text>();
            rawImage = transform.Find("Animator/View_Left/RawImage").GetComponent<RawImage>();
            nameText = transform.Find("Animator/View_Left/Text_Name").GetComponent<Text>();

            playerGo = transform.Find("Animator/View_Right/View_Player").gameObject;
            totalScoreText = playerGo.transform.Find("Text_Number").GetComponent<Text>();
            playerBtn = playerGo.transform.Find("Btn_01").GetComponent<Button>();
            playerBtn.onClick.AddListener(CheckBtnClick);
            roleScore = playerGo.transform.Find("Image_Bg01/Text_Number").GetComponent<Text>();
            petScore = playerGo.transform.Find("Image_Bg02/Text_Number").GetComponent<Text>();

            pvpGo = transform.Find("Animator/View_Right/View_PVP").gameObject;
            pvpRankNameText = pvpGo.transform.Find("Text_Title").GetComponent<Text>();
            pvpRankStarText = pvpGo.transform.Find("Text_Number").GetComponent<Text>();
            ImagePartnerGo = pvpGo.transform.Find("View_Rank").gameObject;
            pvpData1Text = pvpGo.transform.Find("Attr_Grid/Image_Attr/Attr/Text").GetComponent<Text>();
            pvpData2Text = pvpGo.transform.Find("Attr_Grid/Image_Attr (1)/Attr/Text").GetComponent<Text>();
            pvpData3Text = pvpGo.transform.Find("Attr_Grid/Image_Attr (2)/Attr/Text").GetComponent<Text>();
            pvpData4Text = pvpGo.transform.Find("Attr_Grid/Image_Attr (3)/Attr/Text").GetComponent<Text>();

            assetDependencies = gameObject.GetComponent<AssetDependencies>();
        }

        protected override void OnOpen(object arg)
        {
            uI_Rank_DetailParam = arg as UI_Rank_DetailParam;
        }

        protected override void OnShow()
        {
            OnCreateModel();
            RefreshView();
        }

        protected override void OnHide()
        {
            _UnloadShowContent();
        }

        protected override void OnDestroy()
        {
            uI_Rank_DetailParam = null;
            AddressablesUtil.ReleaseInstance(ref m_Hand, null);
        }

        private void RefreshView()
        {
            if(null != uI_Rank_DetailParam)
            {
                playerGo.SetActive(uI_Rank_DetailParam.showType == ERankDetailType.Player);
                pvpGo.SetActive(uI_Rank_DetailParam.showType == ERankDetailType.PVP || uI_Rank_DetailParam.showType == ERankDetailType.LadderPvp);
                TextHelper.SetText(nameText, uI_Rank_DetailParam.rankDescRole.Name.ToStringUtf8());
                
                switch (uI_Rank_DetailParam.showType)
                {
                    case ERankDetailType.Player:
                        TextHelper.SetText(titleText, 2022111);
                        RefreshPlayerView();
                        break;
                    case ERankDetailType.Charactor:
                        TextHelper.SetText(titleText, 2022113);
                        RefreshCharactorView();
                        break;
                    case ERankDetailType.PVP:
                        TextHelper.SetText(titleText, 2022115);
                        RefreshPvpView();
                        break;
                    case ERankDetailType.LadderPvp:
                        TextHelper.SetText(titleText, 2022115);
                        RefreshLadderPvpView();
                        break;
                }
            }
        }

        private void RefreshPlayerView()
        {
            RankDescRole rankDescRole = uI_Rank_DetailParam.rankDescRole;
            if (null != rankDescRole)
            {
                TextHelper.SetText(totalScoreText, rankDescRole.TotalScore.ToString());
                TextHelper.SetText(roleScore, rankDescRole.RoleScore.ToString());
                TextHelper.SetText(petScore, (rankDescRole.TotalScore - rankDescRole.RoleScore).ToString());
            }
        }

        private void RefreshCharactorView()
        {

        }

        private void RefreshPvpView()
        {
            RankDescRole rankDescRole = uI_Rank_DetailParam.rankDescRole;
            if (null != rankDescRole)
            {
                TextHelper.SetText(pvpData1Text, rankDescRole.TotalScore.ToString());
                CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDescRole.Career);
                if (null != careerData)
                {
                    TextHelper.SetText(pvpData4Text, LanguageHelper.GetTextContent(careerData.name));
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDescRole.Career}");
                }
            }

            RankDescArena rankDescArena = uI_Rank_DetailParam.rankDescArena;
            if (null != rankDescArena)
            {
                CSVArenaSegmentInformation.Data item = CSVArenaSegmentInformation.Instance.GetConfData((uint)rankDescArena.DanLv);
                if (null != item)
                {
                    FrameworkTool.DestroyChildren(ImagePartnerGo);
                    LoadPvPLevel(item.RankIcon);
                }
                TextHelper.SetText(pvpRankNameText, null != item ? LanguageHelper.GetTextContent(item.RankDisplay) : LanguageHelper.GetTextContent(540000028));
                TextHelper.SetText(pvpRankStarText, LanguageHelper.GetTextContent(540000029, rankDescArena.Star.ToString()));

                TextHelper.SetText(pvpData2Text, LanguageHelper.GetTextContent(2022122, (rankDescArena.TotalNum == 0 ? 0 : (((float)rankDescArena.WinNum / rankDescArena.TotalNum) * 100)).ToString("0.00")));
                TextHelper.SetText(pvpData3Text, LanguageHelper.GetTextContent(2022121, rankDescArena.WinNum.ToString(), (rankDescArena.TotalNum - rankDescArena.WinNum).ToString()));
            }
        }

        private void RefreshLadderPvpView()
        {
            RankDescRole rankDescRole = uI_Rank_DetailParam.rankDescRole;
            if (null != rankDescRole)
            {
                TextHelper.SetText(pvpData1Text, rankDescRole.TotalScore.ToString());
                CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDescRole.Career);
                if (null != careerData)
                {
                    TextHelper.SetText(pvpData4Text, LanguageHelper.GetTextContent(careerData.name));
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDescRole.Career}");
                }
            }

            RankDescTianTi rankDesctianti = uI_Rank_DetailParam.randDesTianti;
            if (null != rankDesctianti) 
            {
                uint danlv = Sys_LadderPvp.Instance.GetDanLvIDByScore((uint)rankDesctianti.Score);
                CSVTianTiSegmentInformation.Data item = CSVTianTiSegmentInformation.Instance.GetConfData(danlv);
                if (null != item)
                {
                    FrameworkTool.DestroyChildren(ImagePartnerGo); 
                    LoadPvPLevel(item.RankIcon);
                }
                TextHelper.SetText(pvpRankNameText, null != item ? LanguageHelper.GetTextContent(item.RankDisplay) : LanguageHelper.GetTextContent(540000028));
                TextHelper.SetText(pvpRankStarText,  " " + rankDesctianti.Score.ToString());

                TextHelper.SetText(pvpData2Text, LanguageHelper.GetTextContent(2022122, (rankDesctianti.TotalNum == 0 ? 0 : (((float)rankDesctianti.WinNum / rankDesctianti.TotalNum) * 100)).ToString("0.00")));
                TextHelper.SetText(pvpData3Text, LanguageHelper.GetTextContent(2022121, rankDesctianti.WinNum.ToString(), (rankDesctianti.TotalNum - rankDesctianti.WinNum).ToString()));
            }
        }

        private void LoadPvPLevel(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            AsyncOperationHandle<GameObject> handle = default;
            AddressablesUtil.InstantiateAsync(ref handle, name, null, true, ImagePartnerGo.transform);
        }

        private void CheckBtnClick()
        {
            RankDescRole rankDescRole = uI_Rank_DetailParam.rankDescRole;
            if (null == rankDescRole)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(560000001));
            }
            else
            {
                if (Sys_Rank.Instance.OpenTeamPlayerView(rankDescRole))
                {
                    CloseUI();
                }
            }            
        }

        private void CloseUI()
        {
            CloseSelf();
        }

        #region ModelShow
        private void OnCreateModel()
        {
            if (null != uI_Rank_DetailParam)
            {
                _LoadShowScene();
                _LoadShowModel();
                RankDescRole rankDescRole = uI_Rank_DetailParam.rankDescRole;
                Dictionary<uint, List<dressData>> DressValue = Sys_Fashion.Instance.GetDressData(rankDescRole.Fashions.FashionInfos, rankDescRole.HeroId);
                heroLoader.LoadWeaponPart(GetRankFashionWeaponId(rankDescRole.Fashions.FashionInfos), GetModolEquipId());
            }                
        }

        private uint GetModolEquipId()
        {
            uint equipId = uI_Rank_DetailParam.rankDescRole.EquipId;
            if (equipId == 0)
            {
                equipId = Constants.UMARMEDID;
            }
            return equipId;
        }

        private uint GetRankFashionWeaponId(RepeatedField<MapRoleFashionInfo> fashInfo)
        {
            List<uint> dre = new List<uint>();
            for (int i = 0; i < fashInfo.Count; i++)
            {
                dre.Add(fashInfo[i].FashionId);
            }
            return Sys_Fashion.Instance.GetDressedWeaponFashionId(dre);
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }

        private void _LoadShowModel()
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }
            RankDescRole rankDescRole = uI_Rank_DetailParam.rankDescRole;
            Dictionary<uint, List<dressData>> DressValue = Sys_Fashion.Instance.GetDressData(rankDescRole.Fashions.FashionInfos, rankDescRole.HeroId);
            heroLoader.LoadHero(rankDescRole.HeroId, GetModolEquipId(), ELayerMask.ModelShow, DressValue, LoadHeroEnd);
        }

        private GameObject modelGo;

        private void LoadHeroEnd(GameObject go)
        {
            modelGo = rawImage.gameObject;
            modelGo.SetActive(false);
            heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
        }

        private void _UnloadShowContent()
        {
            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0 && null != uI_Rank_DetailParam)
            {
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(uI_Rank_DetailParam.rankDescRole.Career);
                uint highId = GetRankHighId();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, GetModolEquipId(), go: modelGo);
            }
        }

        private uint GetRankHighId()
        {
            if (null != uI_Rank_DetailParam)
            {
                List<uint> fashionIds = new List<uint>();
                for (int i = 0; i < uI_Rank_DetailParam.rankDescRole.Fashions.FashionInfos.Count; i++)
                {
                    fashionIds.Add(uI_Rank_DetailParam.rankDescRole.Fashions.FashionInfos[i].FashionId);
                }
                uint id = Sys_Fashion.Instance.GetDressedClothesFashionId(fashionIds);
                id = (uint)(id * 10000 + uI_Rank_DetailParam.rankDescRole.HeroId);
                CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);
                if (cSVFashionModelData != null)
                {
                    return cSVFashionModelData.action_show_id;
                }
            }
            return 0;
        }
        #endregion
    }
}