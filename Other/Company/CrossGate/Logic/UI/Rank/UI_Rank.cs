using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Rank_Layout
    {
        private Button closeBtn;
        private Button setBtn;
        public Transform pageTrans;
        public Transform infoTrans;
        public Transform noneTrans;

        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
            setBtn = transform.Find("Animator/View_Right/Btn_Setting").GetComponent<Button>();
            pageTrans = transform.Find("Animator/View_Left");
            infoTrans = transform.Find("Animator/View_Right");
            noneTrans = transform.Find("Animator/View_None");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            setBtn.onClick.AddListener(listener.SetBtnClicked);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void SetBtnClicked();
        }
    }

    public class UI_Rank_TypeListSub
    {
        private Transform transform;
        private CP_ToggleEx parToggle;
        private Text textLight;
        private Text textDark;
        public CSVRanklistmain.Data typeSubData;
        private Action<UI_Rank_TypeListSub> action;
        public void Bind(GameObject go)
        {
            transform = go.transform;
            parToggle = go.GetComponent<CP_ToggleEx>();
            parToggle?.onValueChanged.AddListener(OnToggleClick);
            textLight = transform.Find("Image_Select/Text_Light").GetComponent<Text>();
            textDark = transform.Find("Text_Dark").GetComponent<Text>();
        }

        private void OnToggleClick(bool isOn)
        {
            if (isOn)
            {
                parToggle.SetSelected(true, false);
                action?.Invoke(this);
            }
        }

        public void AddListener(Action<UI_Rank_TypeListSub> _action)
        {
            action = _action;
        }

        public void OpenTrigger()
        {
            parToggle.SetSelected(true, false);
            action?.Invoke(this);
        }

        public void SetSub(CSVRanklistmain.Data typeSubData)
        {
            if (null != typeSubData)
            {
                this.typeSubData = typeSubData;
                uint languageId = typeSubData.Name;
                textLight.text = LanguageHelper.GetTextContent(languageId);
                textDark.text = LanguageHelper.GetTextContent(languageId);

                if (null != parToggle && typeSubData.id == Sys_Rank.Instance.InitTypeSub)
                {
                    parToggle.SetSelected(true, true);
                }
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "数据空");
            }
        }

        public void HideToggle()
        {
            parToggle.SetSelected(false, false);
        }

    }

    public class UI_Rank_TypeListParent
    {
        private Transform transform;
        private Toggle parToggle;
        private GameObject subToggleGo;
        private Transform subTogglePar;
        private Text textLight;
        private Text textDark;
        private Image backImage;
        private Action<uint> parentAction;
        private List<UI_Rank_TypeListSub> subCeils = new List<UI_Rank_TypeListSub>();
        private Image arrowToggleGo;
        private Action<CSVRanklistmain.Data>  subAction;
        public CSVRanklistsort.Data parentData;
        public void Bind(GameObject go)
        {
            transform = go.transform;

            subToggleGo = transform.Find("Toggle_Select01").gameObject;
            subTogglePar = transform.Find("Content_Small");
            parToggle = go.GetComponent<Toggle>();
            parToggle.onValueChanged.AddListener(OnToggleClick);
            textLight = transform.Find("GameObject/Image_Select/Text_Light").GetComponent<Text>();
            textDark = transform.Find("GameObject/Text_Dark").GetComponent<Text>();
            backImage = transform.Find("GameObject").GetComponent<Image>();
            arrowToggleGo = transform.Find("GameObject/Image_Frame").GetComponent<Image>();

        }

        private void OnToggleClick(bool isOn)
        {
            if (isOn)
            {
                if (null != parentData)
                {
                    parentAction?.Invoke(this.parentData.id);
                }
            }
        }

        public void AddListener(Action<uint> _action, Action<CSVRanklistmain.Data>  _subAction)
        {
            parentAction = _action;
            subAction = _subAction;
        }

        public void SetSub(CSVRanklistsort.Data parentData)
        {
            if (null != parentData)
            {
                this.parentData = parentData;
                uint languageId = parentData.langid;
                textLight.text = LanguageHelper.GetTextContent(languageId);
                textDark.text = LanguageHelper.GetTextContent(languageId);
                ImageHelper.SetIcon(backImage, parentData.RankPic);
                List<CSVRanklistmain.Data>  tySubList = Sys_Rank.Instance.GetAllRankTypeSubByType(parentData.id);
                for (int i = 0; i < tySubList.Count; i++)
                {
                    UI_Rank_TypeListSub subCeil = new UI_Rank_TypeListSub();
                    GameObject go = GameObject.Instantiate<GameObject>(subToggleGo, subTogglePar);
                    subCeil.Bind(go);
                    subCeil.AddListener(SubClicked);
                    subCeil.SetSub(tySubList[i]);

                    subCeils.Add(subCeil);
                    go.SetActive(true);
                }
            }
        }

        public void SubClicked(UI_Rank_TypeListSub typeSub)
        {
            if (null != typeSub.typeSubData)
            {
                subAction?.Invoke(typeSub.typeSubData);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "CSVRankMain.Data is null");
            }
        }

        bool isShow = false;
        public void SetParState(uint _parId, bool isInit)
        {
            if (parentData.id == _parId && !isShow)
            {
                isShow = true;
                subTogglePar.gameObject.SetActive(true);
                if(isInit)
                {
                    bool isShowEnd = true;
                    for (int i = 0; i < subCeils.Count; i++)
                    {
                        if (subCeils[i].typeSubData.id == Sys_Rank.Instance.InitTypeSub)
                        {
                            subCeils[i].OpenTrigger();
                            isShowEnd = false;
                        }
                    }
                    if (isShowEnd)
                        subCeils[0].OpenTrigger();
                }
                else
                {
                    subCeils[0].OpenTrigger();
                }
            }
            else if (!isInit || parentData.id != _parId)
            {
                isShow = false;
                subTogglePar.gameObject.SetActive(false);
            }
            SetArrow(_parId == parentData.id && subTogglePar.gameObject.activeSelf);
        }

        public void HideAllSub()
        {
            for (int i = 0; i < subCeils.Count; i++)
            {
                subCeils[i].HideToggle();
            }
        }

        private void SetArrow(bool select)
        {
            float rotateZ = select ? 0f : 90f;
            arrowToggleGo.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        public void InitToggle()
        {
            if (parToggle.isOn)
            {
                if (null != parentData)
                {
                    parentAction?.Invoke(this.parentData.id);
                }
                parToggle.isOn = true;
            }
            else
            {
                parToggle.isOn = true;
            }
            SetArrow(true);
        }

    }

    public class UI_Rank_TypeList
    {
        private Transform transform;
        private bool listInit = false;
        private GameObject toggleSelectGo;
        private Transform parentGo;
        private List<UI_Rank_TypeListParent> parentCeils = new List<UI_Rank_TypeListParent>();
        List<CSVRanklistsort.Data>  typeList = new List<CSVRanklistsort.Data>();
        public uint showId;
        private IListener listener;
        public void Init(Transform transform)
        {
            this.transform = transform;
            toggleSelectGo = transform.Find("Scroll01/Toggle_Select01").gameObject;
            parentGo = transform.Find("Scroll01/Content");
        }

        public void Close()
        {
            for (int i = 0; i < parentCeils.Count; i++)
            {
                parentCeils[i].HideAllSub();
            }
        }

        public void Show()
        {
            if (!listInit)
            {
                parentCeils.Clear();
                typeList = Sys_Rank.Instance.GetAllRankType();
                bool isCanShow = true;
                for (int i = 0; i < typeList.Count; i++)
                {
                    if (typeList[i].id == 14u && !Sys_Achievement.Instance.CheckAchievementIsCanShow())
                        isCanShow = false;
                    else
                        isCanShow = true;
                    if (isCanShow)
                    {
                        UI_Rank_TypeListParent parentCeil = new UI_Rank_TypeListParent();
                        GameObject go = GameObject.Instantiate<GameObject>(toggleSelectGo, parentGo);
                        parentCeil.Bind(go);
                        parentCeil.AddListener(OnClickPar, OnClickSub);
                        parentCeil.SetSub(typeList[i]);
                        parentCeils.Add(parentCeil);
                        go.SetActive(true);
                    }
                }
                listInit = true;
                showId = Sys_Rank.Instance.InitType;
                ResetList(true);
            }
            else
            {
                ResetList(true);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }

        private void OnClickPar(uint id)
        {
            showId = id;
            ResetList(false);
        }

        private void OnClickSub(CSVRanklistmain.Data subTypeData)
        {
            listener?.OnSubClick(subTypeData);
        }

        private void ResetList(bool setInit)
        {
            for (int i = 0; i < parentCeils.Count; i++)
            {
                UI_Rank_TypeListParent item = parentCeils[i];
                item.SetParState(showId, setInit);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSubClick(CSVRanklistmain.Data subTypeData);
        }
    }

    public class UI_Rank_InfoCeil
    {
        public Image rankNumImage;
        private Text rankNumText;
        private Text text01;
        private Text text02;
        private Text text03;
        private Text pvpNameText;
        private Text pvpStarText;
        private Button detailBtn;
        private uint rankType;
        private uint subType;
        private GameObject noneGo;

        private RankUnitData unitdata;
        public void Init(Transform transform)
        {
            rankNumImage = transform.Find("Image_Rank").GetComponent<Image>();
            rankNumText = transform.Find("Text_Number").GetComponent<Text>();
            text01 = transform.Find("Text01").GetComponent<Text>();
            text02 = transform.Find("Text02").GetComponent<Text>();
            text03 = transform.Find("Text03").GetComponent<Text>();
            pvpNameText = transform.Find("Text_PVP").GetComponent<Text>();
            pvpStarText = transform.Find("Text_PVP/Text_Num").GetComponent<Text>();
            detailBtn = transform.Find("Btn_Detail").GetComponent<Button>();
            detailBtn.onClick.AddListener(DetailBtnClicked);
            noneGo = transform.Find("Text_None")?.gameObject;
        }

        private void DetailBtnClicked()
        {
            if (null != unitdata)
            {
                if (rankType == 1)
                {
                    RankDataRole rankDataRole = unitdata.RoleData;
                    if (null != rankDataRole)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataRole.RoleId, rankDataRole.PetUid);
                    }
                }
                else if (rankType == 2)
                {
                    RankDataCareer rankDataCareer = unitdata.CareerData;
                    if (null != rankDataCareer)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataCareer.RoleId, 0);
                    }
                }
                else if (rankType == 3)
                {
                    RankDataEquip rankDataEquip = unitdata.EquipData;
                    if (null != rankDataEquip)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataEquip.RoleId, 0, rankDataEquip.EquipId);
                    }
                }
                else if (rankType == 4)
                {
                    RankDataAttr rankDataAttr = unitdata.AttrData;
                    if (null != rankDataAttr)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataAttr.RoleId, 0);
                    }
                }
                else if (rankType == 5)
                {
                    RankDataArena rankDataRole = unitdata.ArenaData;
                    if (null != rankDataRole)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataRole.RoleId, 0);
                    }
                }
                else if (rankType == 6)
                {
                    RankDataGrowth rankDataGrowth = unitdata.GrowthData;
                    if (null != rankDataGrowth)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataGrowth.RoleId, 0);
                    }
                }
                else if (rankType == 7)
                {

                }
                else if (rankType == 8)
                {
                    RankDataRedEnv rankDataRedEnv = unitdata.RedEnvData;
                    if (null != rankDataRedEnv)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataRedEnv.RoleId, 0);
                    }
                }
                else if (rankType == 9)
                {
                    RankDataLeisure rankDataLeisure = unitdata.LeisureData;
                    if (null != rankDataLeisure)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataLeisure.RoleId, 0);
                    }
                }
                else if (rankType == 10)
                {
                    RankDataLovely rankDataLovely = unitdata.LovelyData;
                    if (null != rankDataLovely)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, rankDataLovely.RoleId);
                    }
                }
                else if (rankType == 14)
                {
                    RankDataAchievement achievementData = unitdata.AchievementData;
                    if (null != achievementData)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, achievementData.RoleId);
                    }
                }
                else if (rankType == 15)
                {
                    RankDataTianTi tiantitData = unitdata.TiantiData;
                    if (null != tiantitData)
                    {
                        Sys_Rank.Instance.RankUnitDescReq(rankType, subType, tiantitData.RoleId);
                    }
                }
            }
        }

        private string GetNameString(ByteString name)
        {
            if (name.IsEmpty)
            {
                return LanguageHelper.GetTextContent(540000028);
            }
            return name.ToStringUtf8();
        }

        public void SetBaseData(RankUnitData unitdata, uint big, uint sub, bool isMySelf)
        {
            bool isTextPvpShow = false;

            if (null != unitdata)
            {
                this.unitdata = unitdata;
                rankType = big;
                subType = sub;
                uint ranuNum = unitdata.Rank;
                detailBtn.gameObject.SetActive(!isMySelf && big != 11);
                bool hasRankNum = ranuNum == 0; //0 为未上榜
                noneGo?.SetActive(hasRankNum);
                uint iconId = Sys_Rank.Instance.GetRankIcon((int)ranuNum);
                bool isshowIcon = iconId != 0;
                rankNumImage.gameObject.SetActive(isshowIcon);
                rankNumText.gameObject.SetActive(!isshowIcon && !hasRankNum);
                if (isshowIcon)
                {
                    ImageHelper.SetIcon(rankNumImage, iconId);
                }
                else
                {
                    TextHelper.SetText(rankNumText, ranuNum.ToString());
                }
                uint rankScore = unitdata.Score;
                if (big == 1u) //个人信息
                {
                    RankDataRole rankDataRole = unitdata.RoleData;
                    if (sub != 4u && null != rankDataRole)
                    {
                        TextHelper.SetText(text01, rankDataRole.Name.ToStringUtf8());
                        CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDataRole.Career);
                        if (null != careerData)
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDataRole.Career}");
                        }

                        if (sub == 1u) //综合评分
                        {
                            TextHelper.SetText(text03, rankScore.ToString());
                        }
                        else if (sub == 2u) // 等级
                        {
                            TextHelper.SetText(text03, rankDataRole.Level.ToString());
                        }
                        else if (sub == 3u) //人物
                        {
                            TextHelper.SetText(text03, rankScore.ToString());
                        }
                    }
                    else
                    {
                        TextHelper.SetText(text01, Sys_Pet.Instance.GetPetNmaeBySeverName(rankDataRole.PetId, rankDataRole.PetName));
                        TextHelper.SetText(text02, rankDataRole.Name.ToStringUtf8());
                        TextHelper.SetText(text03, rankScore.ToString());
                    }

                }
                else if (big == 2u) // 职业
                {
                    RankDataCareer rankDataCareer = unitdata.CareerData;
                    if (null != rankDataCareer)
                    {
                        TextHelper.SetText(text01, rankDataCareer.Name.ToStringUtf8());
                        TextHelper.SetText(text02, GetNameString(rankDataCareer.GuildName));
                        TextHelper.SetText(text03, rankScore.ToString());
                    }
                }
                else if (big == 3u) // 装备
                {
                    RankDataEquip rankDataEquip = unitdata.EquipData;
                    if (null != rankDataEquip)
                    {
                        CSVItem.Data equipData = CSVItem.Instance.GetConfData(rankDataEquip.EquipId);
                        if (null != equipData)
                        {
                            TextHelper.SetText(text01, LanguageHelper.GetTextContent(equipData.name_id));
                        }
                        else
                        {
                            TextHelper.SetText(text01, 540000028);
                            //DebugUtil.LogError($"not find CSVEquipment.Data config id =  {rankDataEquip.EquipId}");
                        }
                        TextHelper.SetText(text02, rankDataEquip.Name.ToStringUtf8());
                        TextHelper.SetText(text03, rankScore.ToString());
                    }
                }
                else if (big == 4u) // 属性
                {
                    RankDataAttr rankDataAttr = unitdata.AttrData;
                    if (null != rankDataAttr)
                    {
                        TextHelper.SetText(text01, rankDataAttr.Name.ToStringUtf8());
                        CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDataAttr.Career);
                        if (null != careerData)
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDataAttr.Career}");
                        }

                        CSVRanklistmain.Data mainData = CSVRanklistmain.Instance.GetConfData(big * 100 + sub);

                        TextHelper.SetText(text03, !mainData.percentage ? rankScore.ToString() : LanguageHelper.GetTextContent(2022122, (rankScore / 100.0f).ToString()));

                    }
                }
                else if (big == 5u) // 荣耀竞技场
                {
                    RankDataArena rankDataArena = unitdata.ArenaData;
                    if (null != rankDataArena)
                    {
                        TextHelper.SetText(text01, rankDataArena.Name.ToStringUtf8());
                        CSVArenaSegmentInformation.Data item = CSVArenaSegmentInformation.Instance.GetConfData((uint)rankDataArena.DanLv);
                        isTextPvpShow = null != item;
                        if (isTextPvpShow)
                        {
                            TextHelper.SetText(pvpNameText, isTextPvpShow ? LanguageHelper.GetTextContent(item.RankDisplay) : LanguageHelper.GetTextContent(540000028));
                            TextHelper.SetText(pvpStarText, LanguageHelper.GetTextContent(540000029, rankDataArena.Star.ToString()));
                        }
                        else
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(540000028));
                        }
                        string tempText03 = "";
                        if (sub == 1u) // //本服榜
                        {
                            tempText03 = rankDataArena.GlobalRank == 0 ? LanguageHelper.GetTextContent(2022103) : rankDataArena.GlobalRank.ToString();
                        }
                        else if (sub == 2u) // //本服榜
                        {
                            ;
                            var severInfo = Sys_Login.Instance.FindServerInfoByID(rankDataArena.ServerId);
                            tempText03 = null != severInfo ? severInfo.ServerName : LanguageHelper.GetTextContent(540000028);
                        }
                        TextHelper.SetText(text03, tempText03);
                    }
                }
                else if (big == 6u) // 成长路
                {
                    if (sub == 1u) // 声望
                    {
                        RankDataGrowth rankDataGrowth = unitdata.GrowthData;
                        if (null != rankDataGrowth)
                        {
                            TextHelper.SetText(text01, rankDataGrowth.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDataGrowth.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDataGrowth.Career}");
                            }
                            TextHelper.SetText(text03, Sys_Reputation.Instance.GetRankAndLevelTitle(rankScore));
                        }
                    }
                    else if(sub == 2u)//驯养值
                    {
                        RankDataGrowth rankDataGrowth = unitdata.GrowthData;
                        if (null != rankDataGrowth)
                        {
                            TextHelper.SetText(text01, rankDataGrowth.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDataGrowth.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDataGrowth.Career}");
                            }
                            TextHelper.SetText(text03,Sys_PetDomesticate.Instance.GetRankStageText(rankScore));
                        }
                    }
                }
                else if (big == 8u)//红包榜 TODO
                {
                    if (sub == 1u)//家族红包排行
                    {

                    }
                    else if (sub == 2u)//个人红包排行
                    {
                        RankDataRedEnv redEnvData = unitdata.RedEnvData;
                        if (null != redEnvData)
                        {
                            TextHelper.SetText(text01, redEnvData.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(redEnvData.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {redEnvData.Career}");
                            }
                            TextHelper.SetText(text03, rankScore.ToString());
                        }
                    }
                }
                else if (big == 9u)
                {

                    RankDataLeisure rankDataLeisure = unitdata.LeisureData;
                    if (null != rankDataLeisure)
                    {
                        TextHelper.SetText(text01, rankDataLeisure.Name.ToStringUtf8());
                        CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDataLeisure.Career);
                        if (null != careerData)
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDataLeisure.Career}");
                        }
                        TextHelper.SetText(text03, rankScore.ToString());
                    }

                }
                else if (big == 10u)
                {
                    RankDataLovely rankDataLovely = unitdata.LovelyData;
                    if (rankDataLovely != null)
                    {
                        TextHelper.SetText(text01, rankDataLovely.Name.ToStringUtf8());
                        CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(rankDataLovely.Career);
                        if (null != careerData)
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {rankDataLovely.Career}");
                        }
                        TextHelper.SetText(text03, unitdata.Score.ToString());
                    }
                }
                else if (big == 11u)
                {
                    if (sub == 1u) // //本服榜
                    {
                        RankDataGuild rankDataGuild = unitdata.GuildData;
                        if (rankDataGuild != null)
                        {
                            TextHelper.SetText(text01, rankDataGuild.GuildName.ToStringUtf8());
                            TextHelper.SetText(text02, rankDataGuild.Level.ToString());
                            TextHelper.SetText(text03, (unitdata.Score / 100.0f).ToString("0.#"));
                        }
                    }
                }
                else if (big == 14u)//成就
                {
                    if (sub == 1u)
                    {
                        RankDataAchievement achievement = unitdata.AchievementData;
                        if (achievement != null)
                        {
                            TextHelper.SetText(text01, achievement.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(achievement.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {achievement.Career}");
                            }
                            TextHelper.SetText(text03, rankScore.ToString());
                        }
                    }
                }
                else if (big == 15u)
                {

                    RankDataTianTi tianti = unitdata.TiantiData;
                    if (null != tianti)
                    {
                        TextHelper.SetText(text01, tianti.Name.ToStringUtf8());
                        uint danlvid = Sys_LadderPvp.Instance.GetDanLvIDByScore((uint)tianti.Score);

                        CSVTianTiSegmentInformation.Data item = CSVTianTiSegmentInformation.Instance.GetConfData(danlvid);
                        isTextPvpShow = null != item;
                        if (isTextPvpShow)
                        {
                            TextHelper.SetText(pvpNameText, isTextPvpShow ? LanguageHelper.GetTextContent(item.RankDisplay) : LanguageHelper.GetTextContent(540000028));
                            TextHelper.SetText(pvpStarText, LanguageHelper.GetTextContent(540000029, tianti.Score.ToString()));
                        }
                        else
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(540000028));
                        }
                        string tempText03 = "";
                        if (sub == 1u) // //本服榜
                        {
                            tempText03 = tianti.GlobalRank == 0 ? LanguageHelper.GetTextContent(2022103) : tianti.GlobalRank.ToString();
                        }
                        else if (sub == 2u) // //本服榜
                        {
                            ;
                            var severInfo = Sys_Login.Instance.FindServerInfoByID(tianti.ServerId);
                            tempText03 = null != severInfo ? severInfo.ServerName : LanguageHelper.GetTextContent(540000028);
                        }
                        TextHelper.SetText(text03, tempText03);
                    }
                }
                pvpNameText.gameObject.SetActive(isTextPvpShow);
                text02.gameObject.SetActive(!isTextPvpShow);
            }
            else
            {
                if (isMySelf)
                {
                    detailBtn.gameObject.SetActive(false);
                    noneGo?.SetActive(true);
                    rankNumImage.gameObject.SetActive(false);
                    rankNumText.gameObject.SetActive(false);
                    if (big == 1u) //个人信息
                    {
                        if (sub != 4u)
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }

                            if (sub == 1u) //综合评分
                            {
                                TextHelper.SetText(text03, Sys_Attr.Instance.power.ToString());
                            }
                            else if (sub == 2u) // 等级
                            {
                                TextHelper.SetText(text03, Sys_Role.Instance.Role.Level.ToString());
                            }
                            else if (sub == 3u) //人物
                            {
                                TextHelper.SetText(text03, Sys_Attr.Instance.rolePower.ToString());
                            }
                        }
                        else
                        {
                            ClientPet hightPet = Sys_Pet.Instance.GetHightPowerPet();
                            TextHelper.SetText(text01, Sys_Pet.Instance.GetPetNmaeByClient(hightPet));
                            TextHelper.SetText(text02, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            TextHelper.SetText(text03, null == hightPet ? LanguageHelper.GetTextContent(540000028) : hightPet.petUnit.SimpleInfo.Score.ToString());
                        }

                    }
                    else if (big == 2u) // 职业
                    {
                        TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                        TextHelper.SetText(text02, Sys_Family.Instance.GetFamilyName());
                        TextHelper.SetText(text03, Sys_Attr.Instance.power.ToString());
                    }
                    else if (big == 3u) // 装备
                    {
                        ItemData weaponItem = null;
                        CSVRanklistmain.Data mainData = CSVRanklistmain.Instance.GetConfData(big * 100 + sub);
                        weaponItem = Sys_Equip.Instance.SameEquipment(mainData.Owndata);
                        TextHelper.SetText(text02, Sys_Role.Instance.Role.Name.ToStringUtf8());
                        if (null == weaponItem)
                        {
                            TextHelper.SetText(text01, 540000028);
                            TextHelper.SetText(text03, 540000028);
                        }
                        else
                        {
                            CSVItem.Data equipData = CSVItem.Instance.GetConfData(weaponItem.Id);
                            if (null != equipData)
                            {
                                TextHelper.SetText(text01, LanguageHelper.GetTextContent(equipData.name_id));
                            }
                            else
                            {
                                TextHelper.SetText(text01, 540000028);
                                //DebugUtil.LogError($"not find CSVEquipment.Data config id =  {rankDataEquip.EquipId}");
                            }
                            TextHelper.SetText(text03, Sys_Equip.Instance.CalEquipTotalScore(weaponItem).ToString());
                        }
                    }
                    else if (big == 4u) // 属性
                    {
                        TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                        CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                        if (null != careerData)
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                        }
                        else
                        {
                            DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                        }

                        CSVRanklistmain.Data mainData = CSVRanklistmain.Instance.GetConfData(big * 100 + sub);
                        Sys_Attr.Instance.pkAttrs.TryGetValue(mainData.Owndata, out long rankScore);
                        TextHelper.SetText(text03, !mainData.percentage ? rankScore.ToString() : LanguageHelper.GetTextContent(2022122, (rankScore / 100.0f).ToString()));
                    }
                    else if (big == 5u) // 荣耀竞技场
                    {
                        TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                        CSVArenaSegmentInformation.Data item = CSVArenaSegmentInformation.Instance.GetConfData((uint)Sys_Pvp.Instance.Level);
                        isTextPvpShow = null != item;
                        if (isTextPvpShow)
                        {
                            TextHelper.SetText(pvpNameText, isTextPvpShow ? LanguageHelper.GetTextContent(item.RankDisplay) : LanguageHelper.GetTextContent(540000028));
                            TextHelper.SetText(pvpStarText, LanguageHelper.GetTextContent(540000029, Sys_Pvp.Instance.Star.ToString()));
                        }
                        else
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(540000028));
                        }
                        string tempText03 = "";
                        if (sub == 1u) // 本服榜
                        {
                            tempText03 = LanguageHelper.GetTextContent(2022103);
                        }
                        else if (sub == 2u) // 跨服榜
                        {
                            var severInfo = Sys_Login.Instance.GetServerInfo();
                            tempText03 = null != severInfo ? severInfo.ServerName : LanguageHelper.GetTextContent(540000028);
                        }
                        TextHelper.SetText(text03, tempText03);
                    }
                    else if (big == 6u) // 成长路
                    {
                        if (sub == 1u) // 声望
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }
                            TextHelper.SetText(text03, Sys_Reputation.Instance.GetRankAndLevelTitle(Sys_Reputation.Instance.reputationLevel));
                        }
                        else if(sub == 2u)//驯养值
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }
                            TextHelper.SetText(text03, Sys_PetDomesticate.Instance.GetRankStageText());
                        }
                    }
                    else if (big == 8u)//家族红包 TODO
                    {
                        if (sub == 1u)//家族红包排行
                        {

                        }
                        else if (sub == 2u)//个人红包排行
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }
                            TextHelper.SetText(text03, Sys_Family.Instance.totalSendMoney.ToString());
                        }
                    }
                    else if (big == 9u)
                    {
                        if (sub == 1u)
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }
                            TextHelper.SetText(text03, Sys_Cooking.Instance.curScore.ToString());
                        }
                        else if (sub == 2u)
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }
                            TextHelper.SetText(text03, Sys_Fashion.Instance.fashionPoint.ToString());
                        }
                    }
                    else if (big == 11u)
                    {
                        if (sub == 1u) // //本服榜
                        {
                            if (Sys_Family.Instance.familyData.isInFamily)
                            {
                                TextHelper.SetText(text01, Sys_Family.Instance.GetFamilyName());
                                TextHelper.SetText(text02, Sys_Family.Instance.familyData.GetGuildLevel().ToString());
                                TextHelper.SetText(text03, (Sys_FamilyResBattle.Instance.score / 100.0f).ToString("0.#"));
                            }
                            else
                            {
                                noneGo?.SetActive(false);
                                TextHelper.SetText(text01, "");
                                TextHelper.SetText(text02, "");
                                TextHelper.SetText(text03, "");
                            }

                        }
                    }
                    else if (big == 14u)//成就
                    {
                        if (sub == 1u) 
                        {
                            TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                            CSVCareer.Data careerData = CSVCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career);
                            if (null != careerData)
                            {
                                TextHelper.SetText(text02, LanguageHelper.GetTextContent(careerData.name));
                            }
                            else
                            {
                                DebugUtil.Log(ELogType.eNone, $"not find CSVCareer.Data config id =  {Sys_Role.Instance.Role.Career}");
                            }
                            TextHelper.SetText(text03, Sys_Achievement.Instance.GetAchievementStar(0, 0, EAchievementDegreeType.Finished, true, 1, 0, true).ToString());
                        }
                    }
                    else if (big == 15)//
                    {


                        TextHelper.SetText(text01, Sys_Role.Instance.Role.Name.ToStringUtf8());
                        CSVTianTiSegmentInformation.Data item = CSVTianTiSegmentInformation.Instance.GetConfData((uint)Sys_LadderPvp.Instance.LevelID);

                        isTextPvpShow = null != item;
                        if (isTextPvpShow && Sys_LadderPvp.Instance.MyInfoRes != null && Sys_LadderPvp.Instance.MyInfoRes.RoleInfo!= null)
                        {
                            TextHelper.SetText(pvpNameText, isTextPvpShow ? LanguageHelper.GetTextContent(item.RankDisplay) : LanguageHelper.GetTextContent(540000028));

                            TextHelper.SetText(pvpStarText, LanguageHelper.GetTextContent(540000029, Sys_LadderPvp.Instance.MyInfoRes.RoleInfo.Base.Score.ToString()));
                        }
                        else
                        {
                            TextHelper.SetText(text02, LanguageHelper.GetTextContent(540000028));
                        }
                        string tempText03 = "";
                        if (sub == 1u) // 本服榜
                        {
                            tempText03 = LanguageHelper.GetTextContent(2022103);
                        }
                        else if (sub == 2u) // 跨服榜
                        {
                            var severInfo = Sys_Login.Instance.GetServerInfo();
                            tempText03 = null != severInfo ? severInfo.ServerName : LanguageHelper.GetTextContent(540000028);
                        }
                        TextHelper.SetText(text03, tempText03);

                    }
                    pvpNameText.gameObject.SetActive(isTextPvpShow);
                    text02.gameObject.SetActive(!isTextPvpShow);
                }
            }
        }
    }

    public class UI_Rank_RightView
    {
        private List<Text> titleTexts = new List<Text>();
        private CP_ToggleRegistry CP_ToggleRegistryTab;
        private List<CP_Toggle> topToggles = new List<CP_Toggle>();

        private GameObject titleParentGo;
        private GameObject selfGo;
        private GameObject noneGo;

        private UI_Rank_InfoCeil mySelfItem;
        private int infinityCount;

        private CSVRanklistmain.Data currentData;
        private RankDataBase serverData;
        private int toggleId;
        private List<GameObject> rankNumFxGo = new List<GameObject>();

        private InfinityGrid _infinityGrid;
        public void Init(Transform transform)
        {
            int togglenum = Sys_Rank.Instance.GetRankGroupType();
            Transform togglesParent = transform.Find("Top_List/Grid");
            CP_ToggleRegistryTab = togglesParent.GetComponent<CP_ToggleRegistry>();
            CP_ToggleRegistryTab.onToggleChange = OnTabChanged;
            FrameworkTool.CreateChildList(togglesParent, togglenum);
            for (int i = 0; i < togglenum; i++)
            {
                Transform toggleTrans = togglesParent.GetChild(i);
                uint langId = 550000001u + (uint)i;
                TextHelper.SetText(toggleTrans.Find("Text").GetComponent<Text>(), langId);
                TextHelper.SetText(toggleTrans.Find("Text_Select").GetComponent<Text>(), langId);
                CP_Toggle toggle = toggleTrans.GetComponent<CP_Toggle>();
                toggle.id = i;
                topToggles.Add(toggle);
            }
            titleParentGo = transform.Find("Image_Title").gameObject;
            Transform textParent = titleParentGo.transform.Find("Grid");
            for (int i = 0; i < textParent.childCount; i++)
            {
                titleTexts.Add(textParent.GetChild(i).Find("Text").GetComponent<Text>());
            }

            _infinityGrid = transform.Find("Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            selfGo = transform.Find("Image_Bg04").gameObject;
            mySelfItem = new UI_Rank_InfoCeil();
            mySelfItem.Init(selfGo.transform.Find("Item01"));
            noneGo = transform.Find("View_None").gameObject;
            rankNumFxGo.Add(transform.Find("Scroll_View/Fx_ui_Rank_NO1").gameObject);
            rankNumFxGo.Add(transform.Find("Scroll_View/Fx_ui_Rank_NO2").gameObject);
            rankNumFxGo.Add(transform.Find("Scroll_View/Fx_ui_Rank_NO3").gameObject);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Rank_InfoCeil entry = new UI_Rank_InfoCeil();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Rank_InfoCeil entry = cell.mUserData as UI_Rank_InfoCeil;

            entry.SetBaseData(serverData.GetRankDataByIndex(index), currentData.RankType, currentData.Subtype, false);
            if (index < rankNumFxGo.Count)
            {
                Transform fxParent = entry.rankNumImage.transform;
                rankNumFxGo[index].transform.SetParent(fxParent, false);
                for (int i = 0; i < fxParent.childCount; i++)
                {
                    fxParent.GetChild(i).gameObject.SetActive(false);
                }
                rankNumFxGo[index].SetActive(true);
            }
        }

        private void OnTabChanged(int curToggle, int old)
        {
            toggleId = curToggle;
            Sys_Rank.Instance.RankQueryReq(currentData.RankType, currentData.Subtype, (uint)toggleId);
        }

        private void ShowTopTogle(int index,int totalcount)
        {
            int count =  topToggles.Count;

            if (totalcount < count)
                count = totalcount;

            if (index <= 0 && index >= count)
                return;

            for (int i = index; i < count; i++)
            {
                topToggles[i].gameObject.SetActive(true);
            }
        }

        private void HideAllTop()
        {
            for (int i = 0; i < topToggles.Count; i++)
            {
                topToggles[i].gameObject.SetActive(false);
            }
        }

        public void SetRankData(RankDataBase serverData)
        {
            this.serverData = serverData;
            infinityCount = serverData.GetOtherDataCount();
            _infinityGrid.CellCount = infinityCount;
            _infinityGrid.ForceRefreshActiveCell();
            if (infinityCount >= 0)
                _infinityGrid.MoveToIndex(0);
            bool isNone = infinityCount == 0;
            titleParentGo.SetActive(!isNone);
            noneGo.SetActive(isNone);
            SetSelfInfo(isNone);
        }

        private void SetSelfInfo(bool isNone)
        {
            if (null != serverData && !isNone)
            {
                RankUnitData selfD = serverData.GetSelfData();
                mySelfItem.SetBaseData(selfD, currentData.RankType, currentData.Subtype, true);
            }
            selfGo.SetActive(!isNone);
        }

        private int GetRankTeamCount(uint rankid)
        {
            if (rankid == 501u || rankid == 502u || rankid == 1501u || rankid == 1502u)
            {
                var stages = PvpStageLvHelper.GetLevelStage();

                if (PvpStageLvHelper.IsOpenLevel80 == false)
                    return stages.Count;
                    
            }

            return topToggles.Count;
        }
        public void SetBaseData(CSVRanklistmain.Data mainData)
        {
            if (null != mainData)
            {
                currentData = mainData;
                int count = titleTexts.Count;
                if (null != mainData.Tittle && mainData.Tittle.Count >= count)
                {
                    for (int i = 0; i < count; i++)
                    {
                        TextHelper.SetText(titleTexts[i], mainData.Tittle[i]);
                    }
                }
                HideAllTop();
                int groupCount = GetRankTeamCount(mainData.id);//topToggles.Count;
                int index = 0;
                bool isGroup = mainData.Group;
                if (!isGroup)
                {
                    index = groupCount;
                }
                else if (isGroup && !mainData.GlobalRank)
                {
                    index = 1;
                }

                if (isGroup)
                {
                    ShowTopTogle(index, groupCount - index + 1);

                    if (index >= 0 && index < groupCount)
                        CP_ToggleRegistryTab.SwitchTo(topToggles[index].id);
                }
                else
                {
                    Sys_Rank.Instance.RankQueryReq(currentData.RankType, currentData.Subtype, 0);
                }
            }
        }
    }

    public class UI_Rank : UIBase, UI_Rank_Layout.IListener, UI_Rank_TypeList.IListener
    {
        private UI_Rank_Layout layout = new UI_Rank_Layout();
        private UI_Rank_RightView rightView;
        private UI_Rank_TypeList typeList;
        private UI_CurrencyTitle currency;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            typeList = new UI_Rank_TypeList();
            typeList.Init(layout.pageTrans);
            typeList.RegisterListener(this);

            rightView = new UI_Rank_RightView();
            rightView.Init(layout.infoTrans);
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Rank.Instance.eventEmitter.Handle<uint>(Sys_Rank.EEvents.GetRankRes, Function, toRegister);
        }

        protected override void OnShowEnd()
        {

        }
        protected override void OnOpen(object arg)
        {
            OpenUIRankParam param = arg as OpenUIRankParam;
            if (param != null)
            {
                Sys_Rank.Instance.InitType = param.initType;
                Sys_Rank.Instance.InitTypeSub = param.initSubType;
            }
            else
            {
                Sys_Rank.Instance.initType = 0;
                Sys_Rank.Instance.initTypeSub = 0;
            }
        }
        protected override void OnShow()
        {
            currency.InitUi();
            typeList.Show();
            Sys_Rank.Instance.RankGetSetStateReq();
        }

        protected override void OnHide()
        {
            Sys_Rank.Instance.SetSendState(true);
        }

        protected override void OnClose()
        {
            typeList.Close();
        }

        protected override void OnDestroy()
        {
            currency.Dispose();
        }

        public void CloseBtnClicked()
        {

            UIManager.CloseUI(EUIID.UI_Rank);
        }

        public void SetBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Rank_Setting);
        }

        private void Function(uint key)
        {
            RankDataBase rankDataBase = Sys_Rank.Instance.GetRankDataBaseByKey(key);
            if (null != rankDataBase)
            {
                rightView.SetRankData(rankDataBase);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, $"RankDataBase Dictionary not find key = {key} ");
            }
        }

        public void OnSubClick(CSVRanklistmain.Data subTypeData)
        {
            if (null != subTypeData)
            {
                rightView.SetBaseData(subTypeData);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "CSVRanklistmain.Data is null");
            }
        }
    }
}