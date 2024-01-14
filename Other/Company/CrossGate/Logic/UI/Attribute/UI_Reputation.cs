using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Framework;
using Packet;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class UI_Reputation_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public Slider reputationSlider;
        public Text reputationNum;
        public Text title;
        public Text level;
        public Text tips;

        public Text numExp;
        public Text getReputationNumByExp;
        public Text getAddNumByExp;
        public Text lockExp;
        public Text numExploit;
        public Text getReputationNumByExploit;
        public Text getAddNumByExploit;
        public Text lockExploit;
        public Button expExchaneBtn;
        public Button exploitExchaneBtn;
        public Button lookEffectBtn;
        public Button peopleBtn;
        public Button rankBtn;
        public Button tipBtn;
        public Button expTip;
        public Button exploitTip;

        public GameObject attrGo;
        public GameObject roleViewGo;
        public GameObject NoViewGo;

        public Image getReputationNumByExpIcon;
        public Image getReputationNumByExploitIcon;
        public Image expIcon;
        public Image exploitIcon;

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            reputationSlider = transform.Find("Animator/View_Right/View_Level/Slider_Lv").GetComponent<Slider>();
            reputationNum = transform.Find("Animator/View_Right/View_Level/Text_Percent").GetComponent<Text>();
            title = transform.Find("Animator/View_Right/View_Level/Text_Name").GetComponent<Text>();
            level = transform.Find("Animator/View_Right/View_Level/Text").GetComponent<Text>();
            tips = transform.Find("Animator/View_Right/View_Des/Text_Des").GetComponent<Text>();
             lookEffectBtn = transform.Find("Animator/View_Right/View_Des/Button").GetComponent<Button>();
            peopleBtn = transform.Find("Animator/View_Right/View_Get/NPCfeel/Button").GetComponent<Button>();
            rankBtn = transform.Find("Animator/View_Left/Button").GetComponent<Button>();
            attrGo = transform.Find("Animator/View_Right/View_Attribute/Attr_Grid/Attr").gameObject;
            roleViewGo = transform.Find("Animator/View_Left/Rank").gameObject;
            NoViewGo = transform.Find("Animator/View_Empty").gameObject;
            numExp = transform.Find("Animator/View_Right/View_Get/View_Exp/Text_Cost").GetComponent<Text>();
            getReputationNumByExp = transform.Find("Animator/View_Right/View_Get/View_Exp/Text_Count02").GetComponent<Text>();
            getAddNumByExp = transform.Find("Animator/View_Right/View_Get/View_Exp/Text_Count02/Text_Total02").GetComponent<Text>();
            lockExp = transform.Find("Animator/View_Right/View_Get/View_Exp/Text_Lock").GetComponent<Text>();
            expExchaneBtn = transform.Find("Animator/View_Right/View_Get/View_Exp/Button_Exp").GetComponent<Button>();

            numExploit = transform.Find("Animator/View_Right/View_Get/View_Exploit/Text_Cost").GetComponent<Text>();
            getReputationNumByExploit = transform.Find("Animator/View_Right/View_Get/View_Exploit/Text_Count02").GetComponent<Text>();
            getAddNumByExploit = transform.Find("Animator/View_Right/View_Get/View_Exploit/Text_Count02/Text_Total02").GetComponent<Text>();
            lockExploit = transform.Find("Animator/View_Right/View_Get/View_Exploit/Text_Lock").GetComponent<Text>();
            exploitExchaneBtn = transform.Find("Animator/View_Right/View_Get/View_Exploit/Button_Exp").GetComponent<Button>();

            getReputationNumByExpIcon = transform.Find("Animator/View_Right/View_Get/View_Exp/Image_Icon02").GetComponent<Image>();
            getReputationNumByExploitIcon = transform.Find("Animator/View_Right/View_Get/View_Exploit/Image_Icon02").GetComponent<Image>();
            expIcon = transform.Find("Animator/View_Right/View_Get/View_Exp/Text_Cost/Image_Coin").GetComponent<Image>();
            exploitIcon = transform.Find("Animator/View_Right/View_Get/View_Exploit/Text_Cost/Image_Coin").GetComponent<Image>();
            tipBtn = transform.Find("Animator/View_Right/View_Level/Button").GetComponent<Button>();
            expTip = transform.Find("Animator/View_Right/View_Get/View_Exp/Button").GetComponent<Button>();
            exploitTip = transform.Find("Animator/View_Right/View_Get/View_Exploit/Button").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
            lookEffectBtn.onClick.AddListener(listener.OnLookBtnClicked);
            rankBtn.onClick.AddListener(listener.OnRankBtnClicked);
            peopleBtn.onClick.AddListener(listener.OnPeopleBtnClicked);
            expExchaneBtn.onClick.AddListener(listener.OnExpBtnClicked);
            exploitExchaneBtn.onClick.AddListener(listener.OnExploitBtnClicked);
            tipBtn.onClick.AddListener(listener.OnTipBtnClicked);
            expTip.onClick.AddListener(listener.OnExpTipBtnClicked);
            exploitTip.onClick.AddListener(listener.OnExploitTipBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
            void OnExpBtnClicked();
            void OnExploitBtnClicked();
            void OnRankBtnClicked();
            void OnPeopleBtnClicked();
            void OnLookBtnClicked();
            void OnTipBtnClicked();
            void OnExpTipBtnClicked();
            void OnExploitTipBtnClicked();
        }
    }

    public class UI_Reputation_Rank_Item 
    {
        private Transform transform;
        private Text name;
        private Text level;
        private Text profession;
        private Image professionIcon;
        private Button button;
        private RawImage rawImage;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        private RankDescRole rankDescRole;
        private int index;

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        public void Init(GameObject go)
        {
            transform = go.transform;
            name = transform.Find("Text_Name").GetComponent<Text>();
            level = transform.Find("Text_Level").GetComponent<Text>();
            profession = transform.Find("Image_Prop/Text_Profession").GetComponent<Text>();
            professionIcon = transform.Find("Image_Prop").GetComponent<Image>();
            button = transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(OnButtonClicked);
            rawImage = transform.Find("RawImage").GetComponent<RawImage>();
        }

        private void OnButtonClicked()
        {
            if (rankDescRole.RoleId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6105));
                return;
            }
            CmdSocialGetBriefInfoAck info = new CmdSocialGetBriefInfoAck();
            info.HeroId = rankDescRole.HeroId;
            info.RoleId = rankDescRole.RoleId;
            info.Name = rankDescRole.Name;
            info.Level = rankDescRole.Level;
            info.Occ = rankDescRole.Career;
            info.CareerRank = rankDescRole.CareerRank;
            info.GuildName = rankDescRole.GuildName;
            info.RoleHead = rankDescRole.Photo;
            info.RoleHeadFrame = rankDescRole.PhotoFrame;
            CSVCharacter.Data characterData = CSVCharacter.Instance.GetConfData(rankDescRole.HeroId);
            if (null != characterData)
            {
                info.HeadIcon = characterData.headid;
            }
            Sys_Role_Info.InfoParmas infoParmas = new Sys_Role_Info.InfoParmas();
            infoParmas.Clear();
            infoParmas.eType = Sys_Role_Info.EType.Chat;
            infoParmas.mInfo = info;
            UIManager.OpenUI(EUIID.UI_Team_Player, false, infoParmas);
        }

        public  void  Destroy()
        {
            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
        }

        public void SetData(RankDescRole _rankDescRole, int _index,ShowSceneControl _showSceneControl)
        {
            showSceneControl = _showSceneControl;
            rankDescRole = _rankDescRole;
            index = _index;
            if (rankDescRole != null)
            {
                _LoadShowModel();
                name.text = rankDescRole.Name.ToStringUtf8();
                level.text = LanguageHelper.GetTextContent(4811, rankDescRole.Level.ToString());
                CSVCareer.Instance.TryGetValue(rankDescRole.Career, out CSVCareer.Data data);
                profession.text = LanguageHelper.GetTextContent(data.name);
                ImageHelper.SetIcon(professionIcon,data.icon);
            }

            Transform objTrans = showSceneControl.mRoot.transform.Find("Pos_" + index.ToString());
            Vector3 pos = UIManager.mUICamera.WorldToScreenPoint(transform.position);
            var pos1 = showSceneControl.mCamera.WorldToScreenPoint(objTrans.position);
            float posX = (pos1.x - pos.x) / 2;
            transform.localPosition = new Vector3(transform.localPosition.x + posX / (Screen.width / 1280.0f) - 60f, transform.localPosition.y, transform.localPosition.z);
        }

        private void _LoadShowModel()
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }

            Dictionary<uint, List<dressData>> DressValue = Sys_Fashion.Instance.GetDressData(rankDescRole.Fashions.FashionInfos, rankDescRole.HeroId);
            heroLoader.LoadHero(rankDescRole.HeroId, GetModolEquipId(), ELayerMask.ModelShow, DressValue, LoadHeroEnd);
            heroLoader.LoadWeaponPart(GetRankFashionWeaponId(rankDescRole.Fashions.FashionInfos), GetModolEquipId());
        }

        private GameObject modelGo;

        private void LoadHeroEnd(GameObject go)
        {
            Transform objTrans = showSceneControl.mRoot.transform.Find("Pos_" + index.ToString());
            Transform heroTrans = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).transform;
            heroTrans.parent = objTrans;
            heroTrans.localPosition = Vector3.zero;         

            Vector3 pos = UIManager.mUICamera.WorldToScreenPoint(transform.position);
            var pos1 = showSceneControl.mCamera.WorldToScreenPoint(objTrans.position);
            float posX =( pos1.x - pos.x)/2;
            transform.localPosition = new Vector3(transform.localPosition.x + posX / (Screen.width / 1280.0f)-20f, transform.localPosition.y, transform.localPosition.z);
            transform.gameObject.SetActive(true);
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0 && null != rankDescRole)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(rankDescRole.Career);
                uint highId = GetRankHighId();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, GetModolEquipId(), go: modelGo);

                GameObject mainGo = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
                mainGo.SetActive(false);
                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, heroLoader.heroDisplay.mAnimation, mainGo);
            }
        }

        private uint GetRankHighId()
        {
            if (null != rankDescRole)
            {
                List<uint> fashionIds = new List<uint>();
                for (int i = 0; i < rankDescRole.Fashions.FashionInfos.Count; i++)
                {
                    fashionIds.Add(rankDescRole.Fashions.FashionInfos[i].FashionId);
                }
                uint id = Sys_Fashion.Instance.GetDressedClothesFashionId(fashionIds);
                id = (uint)(id * 10000 + rankDescRole.HeroId);
                CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(id);
                if (cSVFashionModelData != null)
                {
                    return cSVFashionModelData.action_show_id;
                }
            }
            return 0;
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

        private uint GetModolEquipId()
        {
            uint equipId = rankDescRole.EquipId;
            if (equipId == 0)
            {
                equipId = Constants.UMARMEDID;
            }
            return equipId;
        }
    }

    public class UI_Reputation : UIBase, UI_Reputation_Layout.IListener
    {
        private UI_Reputation_Layout layout = new UI_Reputation_Layout();
        private uint csvFameLevel;
        private uint maxCsvFameAdditionKey;
        private int expPer;
        private int exploitPer;
        private uint worldOpenLv;
        private uint worldCurLv;
        private CSVFameRank.Data csvFameRankData;
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private List<UI_Reputation_Rank_Item> list = new List<UI_Reputation_Rank_Item>();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Reputation.Instance.eventEmitter.Handle(Sys_Reputation.EEvents.OnReputationUpdate, OnReputationUpdate, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnAddExp, OnAddExp, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnExtraExp, OnAddExp, toRegister);
        }

        protected override void OnShow()
        {
            LoadShowScene();
            SetLvValue();
            SetExpShow();
            SetExploitShow();
            SetAttr();
            InitModelData();
        }

        protected override void OnHide()
        {            
            DefaultAttrItem();
            DefaultListItem();
            showSceneControl?.Dispose();
        }

        private void SetLvValue()
        {
            csvFameLevel = Sys_Reputation.Instance.danLevel;
            csvFameRankData = CSVFameRank.Instance.GetConfData(csvFameLevel);
            layout.title.text = LanguageHelper.GetTextContent(csvFameRankData.name);
            layout.level.text = LanguageHelper.GetTextContent(2020918, Sys_Reputation.Instance.danLevel.ToString(), Sys_Reputation.Instance.specificLevel.ToString());
            float maxValue = csvFameRankData.lvup_cost;
            ulong curValue = Sys_Reputation.Instance.reputationValue;
            if (csvFameLevel == 10 && Sys_Reputation.Instance.specificLevel >= 100)
            {
                layout.reputationSlider.value = 1;
            }
            else
            {
                layout.reputationSlider.value = curValue / maxValue;
            }
            layout.reputationNum.text = LanguageHelper.GetTextContent(2010321, curValue.ToString(),((int)maxValue).ToString());
            GetMaxCSVFameAdditionKey();
        }

        private void SetExpShow()
        {
            int expOpenLv = 0;
            int.TryParse(CSVParam.Instance.GetConfData(727).str_value, out expOpenLv);
            int reputationPer = 0;
            string[] val = CSVParam.Instance.GetConfData(728).str_value.Split('|');
            int.TryParse(val[0], out expPer);
            int.TryParse(val[1], out reputationPer);
            if ((ulong)expPer > Sys_Role.Instance.Role.Exp)
            {
                if (Sys_Role.Instance.Role.ExtraExp == 0)
                {
                    layout.numExp.text = "<color=red>" + Sys_Role.Instance.Role.Exp.ToString() + "</color>/" + expPer.ToString();
                }
                else
                {
                    layout.numExp.text = "<color=red>-" + Sys_Role.Instance.Role.ExtraExp.ToString() + "</color>/" + expPer.ToString();
                }
            }
            else
            {
                layout.numExp.text = "<color=#785D49>" + Sys_Role.Instance.Role.Exp.ToString() + "</color>/" + expPer.ToString();
            }
            ImageHelper.SetIcon(layout.getReputationNumByExpIcon, 10000007);
            ImageHelper.SetIcon(layout.expIcon, 10000004);
            uint.TryParse(CSVParam.Instance.GetConfData(732).str_value, out worldOpenLv);    
            if (CSVWorldLevel.Instance.TryGetValue(Sys_Role.Instance.openServiceDay, out CSVWorldLevel.Data openServiceDay) && openServiceDay != null)
            {
                worldCurLv = openServiceDay.world_level;     
            }
            if (Sys_Role.Instance.Role.Level < expOpenLv || worldCurLv< worldOpenLv)
            {
                ImageHelper.SetImageGray(layout.expExchaneBtn.GetComponent<Image>(), true);
                layout.lockExp.gameObject.SetActive(true);
                layout.lockExp.text = LanguageHelper.GetTextContent(2020914, expOpenLv.ToString(), worldOpenLv.ToString());
            }
            else
            {
                ImageHelper.SetImageGray(layout.expExchaneBtn.GetComponent<Image>(), false);
                layout.lockExp.gameObject.SetActive(false);
            }

            if (Sys_Reputation.Instance.yesterdayMaxLevel > Sys_Reputation.Instance.reputationLevel)
            {
                uint id = (Sys_Reputation.Instance.yesterdayMaxLevel - Sys_Reputation.Instance.reputationLevel) / 10;
                if (id > 0)
                {
                    if (id > maxCsvFameAdditionKey)
                    {
                        id = maxCsvFameAdditionKey;
                    }
                    layout.getAddNumByExp.gameObject.SetActive(true);
                    uint addnum = CSVFameAddition.Instance.GetConfData(id).add_ratio + 100;
                    float addPre = addnum / 100f;
                    layout.getAddNumByExp.text = LanguageHelper.GetTextContent(2020912, addnum.ToString());
                    layout.getReputationNumByExp.text = "x" + (reputationPer * addPre).ToString();
                }
                else
                {
                    layout.getReputationNumByExp.text = "x" + reputationPer.ToString();
                    layout.getAddNumByExp.gameObject.SetActive(false);
                }
            }
            else
            {
                layout.getReputationNumByExp.text = "x" + reputationPer.ToString();
                layout.getAddNumByExp.gameObject.SetActive(false);
            }
        }

        private void SetExploitShow()
        {
            int exploitOpenLv = 0;
            int.TryParse(CSVParam.Instance.GetConfData(729).str_value, out exploitOpenLv);
            int reputationPer = 0;
            string[] val = CSVParam.Instance.GetConfData(730).str_value.Split('|');
            int.TryParse(val[0], out exploitPer);
            int.TryParse(val[1], out reputationPer);
            long count = Sys_Bag.Instance.GetItemCount(8);
            if ((long)exploitPer > count)
            {
                layout.numExploit.text = "<color=red>" + count.ToString() + "</color>/" + exploitPer.ToString();
            }
            else
            {
                layout.numExploit.text = "<color=#785D49>" + count.ToString() + "</color>/" + exploitPer.ToString();
            }

            ImageHelper.SetIcon(layout.getReputationNumByExploitIcon, 10000007);
            ImageHelper.SetIcon(layout.exploitIcon, 10000008);
            //功勋不足
            if (Sys_Experience.Instance.exPerienceLevel < exploitOpenLv)
            {
                ImageHelper.SetImageGray(layout.exploitExchaneBtn.GetComponent<Image>(), true);
                layout.lockExploit.gameObject.SetActive(true);
                layout.lockExploit.text = LanguageHelper.GetTextContent(2020915, exploitOpenLv.ToString());
            }
            else
            {
                ImageHelper.SetImageGray(layout.exploitExchaneBtn.GetComponent<Image>(), false);
                layout.lockExploit.gameObject.SetActive(false);
            }
            if (Sys_Reputation.Instance.yesterdayMaxLevel > Sys_Reputation.Instance.reputationLevel)
            {
                uint id = (Sys_Reputation.Instance.yesterdayMaxLevel - Sys_Reputation.Instance.reputationLevel) / 10;
                if (id > 0)
                {
                    if (id > maxCsvFameAdditionKey)
                    {
                        id = maxCsvFameAdditionKey;
                    }
                    layout.getAddNumByExploit.gameObject.SetActive(true);
                    uint addnum = CSVFameAddition.Instance.GetConfData(id).add_ratio + 100;
                    float addpre = addnum / 100f;
                    layout.getAddNumByExploit.text = LanguageHelper.GetTextContent(2020912, addnum.ToString());
                    layout.getReputationNumByExploit.text = "x" + (reputationPer * addpre).ToString();
                }
                else
                {
                    layout.getAddNumByExploit.gameObject.SetActive(false);
                    layout.getReputationNumByExploit.text = "x" + reputationPer.ToString();
                }
            }
            else
            {
                layout.getAddNumByExploit.gameObject.SetActive(false);
                layout.getReputationNumByExploit.text = "x" + reputationPer.ToString();
            }
        }

        private void SetAttr()
        {
            uint point = 0;
            uint pointNext = 0;
            for (int i = 0; i < csvFameRankData.rank_attr.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                point = GetAttrById(csvFameLevel, Sys_Reputation.Instance.specificLevel, csvFameRankData.rank_attr[i][0]);
                if (Sys_Reputation.Instance.specificLevel == 99&&CSVFameRank.Instance.ContainsKey(csvFameLevel + 1))
                {
                    pointNext = GetAttrById(csvFameLevel + 1, 0, csvFameRankData.rank_attr[i][0]);
                }
                else
                {
                    pointNext = GetAttrById(csvFameLevel, Sys_Reputation.Instance.specificLevel + 1, csvFameRankData.rank_attr[i][0]);
                }
                go.transform.Find("Text_Name").GetComponent<Text>().text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(csvFameRankData.rank_attr[i][0]).name);
                go.transform.Find("Text1").GetComponent<Text>().text = point.ToString();
                Text txt = go.transform.Find("Text2").GetComponent<Text>();
                if (pointNext - point == 0)
                {
                    txt.gameObject.SetActive(false);
                }
                else
                {
                    txt.gameObject.SetActive(true);
                    txt.text =  pointNext.ToString();
                }
            }
            layout.attrGo.SetActive(false);
        }

        private uint GetAttrById(uint frameLevel, uint level, uint attrId)
        {
            uint Point = 0;
            uint addPoint = 0;
            CSVFameRank.Data data = CSVFameRank.Instance.GetConfData(frameLevel);
            for (int i = 0; i < data.rank_attr.Count; ++i)
            {
                if (data.rank_attr[i][0] == attrId)
                {
                    Point = data.rank_attr[i][1];
                    break;
                }
            }
            for (int j = 0; j < data.lv_attr.Count; ++j)
            {
                if (data.lv_attr[j][0] == attrId)
                {
                    addPoint = csvFameRankData.lv_attr[j][1] * level;
                    break;
                }
            }
            return Point + addPoint;
        }

        private void DefaultAttrItem()
        {
            layout.attrGo.SetActive(true);
            FrameworkTool.DestroyChildren(layout.attrGo.transform.parent.gameObject, layout.attrGo.transform.name);
        }

        private void DefaultListItem()
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].Destroy();
            }
            list.Clear();
        }

        private void OnReputationUpdate()
        {
            SetLvValue();
            SetExpShow();
            SetExploitShow();
            DefaultAttrItem();
            SetAttr();
        }

        private void OnCurrencyChanged(uint id, long value)
        {
            if (id == 8)
            {
                SetExploitShow();
            }
        }

        private void OnAddExp()
        {
            SetExpShow();
        }

        private void InitModelData()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(50901, false) || Sys_Reputation.Instance.rankDesRoleList.Count==0)
            {
                layout.roleViewGo.SetActive(false);
                layout.NoViewGo.SetActive(true);
                return;
            }
            layout.roleViewGo.SetActive(true);
            layout.NoViewGo.SetActive(false);
            DefaultListItem();
            for (int i = 0; i < layout.roleViewGo.transform.childCount; ++i)
            {
                Transform tans = layout.roleViewGo.transform.GetChild(i);
                if (tans.name == 1.ToString() && Sys_Reputation.Instance.rankDesRoleList.Count >= 1)
                {
                    UI_Reputation_Rank_Item item = new UI_Reputation_Rank_Item();
                    item.Init(layout.roleViewGo.transform.GetChild(i).gameObject);          
                    item.SetData(Sys_Reputation.Instance.rankDesRoleList[0], 1, showSceneControl);
                    list.Add(item);
                    tans.gameObject.SetActive(true);
                }
                else if (tans.name == 2.ToString() && Sys_Reputation.Instance.rankDesRoleList.Count >= 2)
                {
                    UI_Reputation_Rank_Item item = new UI_Reputation_Rank_Item();
                    item.Init(layout.roleViewGo.transform.GetChild(i).gameObject);             
                    item.SetData(Sys_Reputation.Instance.rankDesRoleList[1], 2, showSceneControl);
                    list.Add(item);
                    tans.gameObject.SetActive(true);
                }
                else if (tans.name == 3.ToString() && Sys_Reputation.Instance.rankDesRoleList.Count >= 3)
                {
                    UI_Reputation_Rank_Item item = new UI_Reputation_Rank_Item();
                    item.Init(layout.roleViewGo.transform.GetChild(i).gameObject);               
                    item.SetData(Sys_Reputation.Instance.rankDesRoleList[2], 3, showSceneControl);
                    list.Add(item);
                    tans.gameObject.SetActive(true);
                }
                else
                {
                    tans.gameObject.SetActive(false);
                }
            }
        }

        private void GetMaxCSVFameAdditionKey()
        {
            uint maxKey = 0;
            foreach (var data in CSVFameAddition.Instance.GetAll())
            {
                if (maxKey < data.id)
                {
                    maxKey = data.id;
                }
            }
            maxCsvFameAdditionKey = maxKey;
        }
     
        private void LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);
        }

        #region ButtonClicked
        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Reputation);
        }

        public void OnExpBtnClicked()
        {
            if (csvFameLevel == 10 && Sys_Reputation.Instance.specificLevel == 100)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6103));
            }
            else if (Sys_Role.Instance.Role.Exp < (ulong)expPer)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6101));
            }
            else if (worldCurLv <worldOpenLv)
            {             
            }
            else
            {
                Sys_Reputation.Instance.ExpExchangeReq();
            }
        }

        public void OnExploitBtnClicked()
        {
            if (csvFameLevel == 10 && Sys_Reputation.Instance.specificLevel == 100)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6103));
            }
            else if (Sys_Bag.Instance.GetItemCount(8) < (long)exploitPer)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6102));
            }
            else
            {
                Sys_Reputation.Instance.ExploitExchangeReq();
            }
        }

        public void OnRankBtnClicked()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(50901, false))
            {
                OpenUIRankParam param = new OpenUIRankParam();
                param.initType = 6;
                param.initSubType = 1;
                UIManager.OpenUI(EUIID.UI_Rank, false, param);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent( 6104));
            }
        }

        public void OnPeopleBtnClicked()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(EFuncOpen.FO_NpcFavorability, true))
            {
                UIManager.OpenUI(EUIID.UI_FavorabilityMain);
            }
        }

        public void OnLookBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Reputation_Tips);
        }

        public void OnTipBtnClicked()
        {
            UIRuleParam rParam = new UIRuleParam();
            rParam.StrContent = LanguageHelper.GetTextContent(2020933);
            rParam.Pos = CameraManager.mUICamera.WorldToScreenPoint(layout.tipBtn.GetComponent<RectTransform>().position - new Vector3(2.7f, 1.5f, 0));
            UIManager.OpenUI(EUIID.UI_Rule, false, rParam);
        }

        public void OnExpTipBtnClicked()
        {
            UIRuleParam rParam = new UIRuleParam();
            rParam.StrContent = LanguageHelper.GetTextContent(2020934);
            rParam.Pos = CameraManager.mUICamera.WorldToScreenPoint(layout.expTip.GetComponent<RectTransform>().position - new Vector3(2.7f, 1.5f, 0));
            UIManager.OpenUI(EUIID.UI_Rule, false, rParam);
        }

        public void OnExploitTipBtnClicked()
        {
            UIRuleParam rParam = new UIRuleParam();
            rParam.StrContent = LanguageHelper.GetTextContent(2020934);
            rParam.Pos = CameraManager.mUICamera.WorldToScreenPoint(layout.expTip.GetComponent<RectTransform>().position - new Vector3(2.7f, 1.5f, 0));
            UIManager.OpenUI(EUIID.UI_Rule, false, rParam);
        }
        #endregion
    }
}
