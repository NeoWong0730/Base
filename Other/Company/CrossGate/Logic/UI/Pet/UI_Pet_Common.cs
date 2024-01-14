using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using Lib.Core;
using static Packet.PetPkAttr.Types;
using UnityEngine.ResourceManagement.AsyncOperations;
using Packet;

namespace Logic
{
    public enum EDwState
    {
        Cut = 1,
        Have = 2,
        Full = 3,
    }

    public class Advance_DwSub : UIComponent
    {
        private Transform _transform;
        private GameObject light1Go;
        private GameObject light2Go;

        protected override void Loaded()
        {
            light1Go = transform.Find("Image_Grade").gameObject;
            light2Go = transform.Find("Image_Grade (1)").gameObject;
        }
        
        public void BingGameObject(GameObject go)
        {
            _transform = go.transform;
            light1Go = transform.Find("Image_Grade").gameObject;
            light2Go = transform.Find("Image_Grade (1)").gameObject;
        }

        public void SetState(EDwState eDwState)
        {
            light1Go.SetActive(eDwState != EDwState.Cut);
            light2Go.SetActive(eDwState == EDwState.Full);
        }
    }


    public class UI_Pet_Advance_Sub
    {
        public uint id;
        public Slider advSlider;
        public Text per;
        public GameObject prepGo;
        public GameObject prepGoGroup;
        public Transform remakeGradeTran;
        public Image remakeImage;
        public Image remakeBgImage;
        public Text remakeGradeText;
        public EBaseAttr attr;

        public COWVd<Advance_DwSub> dwsubVd = new COWVd<Advance_DwSub>();

        public float minWidth = 40;
        public float maxWidth = 195;
        public void Init(Transform transform)
        {
            advSlider = transform.Find("Slider").GetComponent<Slider>();
            maxWidth = transform.Find("Slider").GetComponent<RectTransform>().rect.width;
            per = transform.Find("Text_Value").GetComponent<Text>();
            prepGo = transform.Find("Grid_Grade/Image_GradeBG01").gameObject;
            prepGoGroup = transform.Find("Grid_Grade").gameObject;
            remakeGradeTran = transform.Find("Grid_Grade/Image_RemakeBG");
            remakeImage = remakeGradeTran.Find("Image_Grade").GetComponent<Image>();
            remakeBgImage = remakeGradeTran.Find("Image").GetComponent<Image>();
            remakeGradeText = remakeGradeTran.Find("Text").GetComponent<Text>();
            transform.gameObject.GetComponent<Text>().raycastTarget = true;
            UI_LongPressButton advance_LongPressButton = transform.gameObject.AddComponent<UI_LongPressButton>();
            advance_LongPressButton.onStartPress.AddListener(OnLongPressed);
            advance_LongPressButton.onRelease.AddListener(OnPointerUp);
        }
        public UI_Pet_Advance_Sub()
        {

        }
        public UI_Pet_Advance_Sub(uint _id, EBaseAttr _attr)
        {
            id = _id;
            attr = _attr;
        }

        private void OnLongPressed()
        {
            if (attr == EBaseAttr.Vit)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001108);
            }
            else if (attr == EBaseAttr.Snh)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001109);
            }
            else if (attr == EBaseAttr.Inten)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Tips01, false,2001110);
            }
            else if (attr == EBaseAttr.Speed)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001112);
            }
            else if (attr == EBaseAttr.Magic)
            {
                UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001111);
            }
        }

        private void OnPointerUp()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Tips01);
        }

        public virtual void RefreshItem(ClientPet clientPet, uint type = 0)
        {
            if(null != clientPet)
            {
                float currentConfigValue = GetAdvanceConfigValue(clientPet.petUnit.SimpleInfo.PetId);
                float currentPetValue = clientPet.GetPetGradeAttr(attr) + 0f;
                per.text = string.Format("{0}/{1}", clientPet.GetPetAllGradeAttr(attr).ToString("0.#"), (currentConfigValue + clientPet.GetPetBuildGradeAttr(attr)).ToString("0.#"));
                DwShow(currentPetValue, currentConfigValue);
                advSlider.value = 1.0f;
            }
            else
            {
                advSlider.value = 0f;
                per.text = "";
                DwShow(0, 0);
            }

            SetRemakeGrade(clientPet);
        }

        int prep;
        public void DwShow(float currentD, float petConfigD)
        {
            advSlider.gameObject.SetActive(true);
            prep = (int)currentD - (int)petConfigD;
            RectTransform advSliderRect = advSlider.GetComponent<RectTransform>();
            Vector2 offset = advSliderRect.anchoredPosition;
            float ad_width = ((petConfigD / 50) * maxWidth);
            if (ad_width < minWidth)
            {
                ad_width = minWidth;
            }
            else if (ad_width > maxWidth)
            {
                ad_width = maxWidth;
            }

            advSliderRect.sizeDelta = new Vector2(ad_width, advSliderRect.sizeDelta.y);
            RectTransform prepGoGrouprect = prepGoGroup?.GetComponent<RectTransform>();
            prepGoGrouprect.anchoredPosition = new Vector2(offset.x + advSliderRect.sizeDelta.x + prepGoGrouprect.sizeDelta.x / 2 + 5, prepGoGroup.transform.localPosition.y);
            prepGoGroup?.gameObject.SetActive(true);


            if(currentD > 0)
            {
                if (prep > 0)
                {
                    dwsubVd.TryBuildOrRefresh(prepGo, prepGoGroup.transform, 4 + prep, OnCowVdRefresh);
                }
                else
                {
                    dwsubVd.TryBuildOrRefresh(prepGo, prepGoGroup.transform, 4, OnCowVdRefresh);                    
                }
            }
           
            prepGo.SetActive(false);
        }

        public void OnCowVdRefresh(Advance_DwSub advance_DwSub, int index)
        {  
            if (prep >= 0)
            {
                if (index < 4)
                {
                    advance_DwSub.SetState(EDwState.Have);
                }
                else
                {
                    advance_DwSub.SetState(EDwState.Full);
                }
            }
            else
            {
                if ((-1 * prep)  + index - 4 >= 0)
                {
                    advance_DwSub.SetState(EDwState.Cut);
                }
                else
                {
                    advance_DwSub.SetState(EDwState.Have);
                }
            }
        }

        public float GetAdvanceConfigValue(uint id)
        {
            float advanceValue = 0;
            CSVPetNew.Data currentpet = CSVPetNew.Instance.GetConfData(id);
            if (null != currentpet)
            {
                if (attr == EBaseAttr.Vit)
                {
                    advanceValue = currentpet.endurance + 0f;
                }
                else if (attr == EBaseAttr.Snh)
                {
                    advanceValue = currentpet.strength + 0f;
                }
                else if (attr == EBaseAttr.Inten)
                {
                    advanceValue = currentpet.strong + 0f;
                }
                else if (attr == EBaseAttr.Speed)
                {
                    advanceValue = currentpet.speed + 0f;
                }
                else if (attr == EBaseAttr.Magic)
                {
                    advanceValue = currentpet.magic + 0f;
                }
            }
            return advanceValue;
        }

        public void SetRemakeGrade(ClientPet clientPet)
        {
            bool hasPet = null != clientPet;
            bool hasRemakeGrade = hasPet && clientPet.GetPetBuildGradeAttr(attr) > 0;
            remakeGradeTran.gameObject.SetActive(hasRemakeGrade);
            if (hasPet)
            {
                var grade = clientPet.GetPetBuildGradeAttr(attr);
                ImageHelper.SetIcon(remakeImage, Sys_Pet.Instance.GetGradeStarImageId(grade));
                remakeBgImage.color = Sys_Pet.Instance.GetGradeStarBgColor(grade);
                TextHelper.SetText(remakeGradeText, "+" + grade.ToString(), CSVWordStyle.Instance.GetConfData(Sys_Pet.Instance.GetGradeStarTextStyleId(grade)));
                remakeGradeTran.SetAsLastSibling();
            }
            
        }
    }

    public class UI_Pet_AdvanceView
    {
        List<UI_Pet_Advance_Sub> uI_Pet_Advance_Subs = new List<UI_Pet_Advance_Sub>();        

        public void Init(Transform transform)
        {
            uI_Pet_Advance_Subs.Clear();
            UI_Pet_Advance_Sub vit_Sub = new UI_Pet_Advance_Sub(5, EBaseAttr.Vit);
            vit_Sub.Init(transform.Find("Vit/Text_Vit"));
            uI_Pet_Advance_Subs.Add(vit_Sub);

            UI_Pet_Advance_Sub pow_Sub = new UI_Pet_Advance_Sub(7, EBaseAttr.Snh);
            pow_Sub.Init(transform.Find("Pow/Text_Pow"));
            uI_Pet_Advance_Subs.Add(pow_Sub);

            UI_Pet_Advance_Sub str_Sub = new UI_Pet_Advance_Sub(9, EBaseAttr.Inten);
            str_Sub.Init(transform.Find("Str/Text_Str"));
            uI_Pet_Advance_Subs.Add(str_Sub);

            UI_Pet_Advance_Sub mp_Sub = new UI_Pet_Advance_Sub(11, EBaseAttr.Magic);
            mp_Sub.Init(transform.Find("Ma/Text_Ma"));
            uI_Pet_Advance_Subs.Add(mp_Sub);

            UI_Pet_Advance_Sub spe_Sub = new UI_Pet_Advance_Sub(13, EBaseAttr.Speed);
            spe_Sub.Init(transform.Find("Spe/Text_Spe"));
            uI_Pet_Advance_Subs.Add(spe_Sub);
        }

        private void OnLongPressed()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Tips01, false, 2001113);
        }

        private void OnPointerUp()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Tips01);
        }

        public void RefreshView(ClientPet client)
        {
            for (int i = 0; i < uI_Pet_Advance_Subs.Count; i++)
            {
                uI_Pet_Advance_Subs[i].RefreshItem(client);
            } 
        }
    }

    public class UI_Pet_BuildAdvance_Sub: UI_Pet_Advance_Sub
    {
        private Text bulidper;
        public UI_Pet_BuildAdvance_Sub(uint _id, EBaseAttr _attr)
        {
            id = _id;
            attr = _attr;
        }

        public new void Init(Transform transform)
        {
            base.Init(transform);
            bulidper = transform.Find("Text_Plus")?.GetComponent<Text>();
        }

        public void ShowBulidPercent(bool state)
        {
            if(null != bulidper)
                bulidper.gameObject.SetActive(state);
        }

        /// <summary>
        /// type  0 代表显示本身的属性(分子分母形式) 1 代表显示预览(当前属性形式 + 范围) 2 代表显示未保存属性（分子分母+预览值 形式 ）,3重塑档位 4 空态
        /// </summary>
        /// <param name="client"></param>
        /// <param name="selectItemId"></param>
        /// <param name="type"></param>
        public override void RefreshItem(ClientPet clientPet, uint type = 0)
        {
            if (null != clientPet)
            {
                //改造获取档位
                var buildGrade = clientPet.GetPetBuildGradeAttr(attr);
                //配置表 档位
                var currentConfigValue = GetAdvanceConfigValue(clientPet.petUnit.SimpleInfo.PetId);
                //已有档位（初始档位+改造确定档位）
                var currentPetValue = clientPet.GetPetAllGradeAttr(attr) + 0f;
                if (type == 0)
                {
                    per.text = string.Format("{0}/{1}", currentPetValue.ToString("0.#"), (currentConfigValue + buildGrade).ToString("0.#"));
                    if (null != bulidper)
                        bulidper.text = "";
                    DwShow(clientPet.GetPetGradeAttr(attr), currentConfigValue);
                    SetRemakeGrade(clientPet);
                }
                else if(type == 1)
                {
                    per.text = currentPetValue.ToString("0.#");
                    int[] _value = Sys_Pet.Instance.BuildGradeLimits;
                    if (null != _value && _value.Length >= 2)
                    {
                        if (null != bulidper)
                            bulidper.text = LanguageHelper.GetTextContent(10926,  Sys_Pet.Instance.BuildGradeLimits[0].ToString(), Sys_Pet.Instance.BuildGradeLimits[1].ToString());
                    }
                    DwShow(clientPet.GetPetGradeAttr(attr), currentConfigValue);
                    SetRemakeGrade(clientPet);
                }
                else if(type == 2)
                {
                    uint previewGrade = clientPet.GetPetBuildTempGradeAttr(attr);
                    per.text = string.Format("{0}/{1}", (currentPetValue + previewGrade).ToString("0.#"), (currentConfigValue + buildGrade + previewGrade).ToString("0.#"));
                    if (null != bulidper)
                        bulidper.text = "";
                    DwShow(clientPet.GetPetGradeAttr(attr), currentConfigValue);
                    var grade = buildGrade + clientPet.GetPetBuildTempGradeAttr(attr);
                    remakeGradeTran.gameObject.SetActive(grade > 0);
                    ImageHelper.SetIcon(remakeImage, Sys_Pet.Instance.GetGradeStarImageId(grade));
                    remakeBgImage.color = Sys_Pet.Instance.GetGradeStarBgColor(grade);
                    TextHelper.SetText(remakeGradeText, "+" + grade.ToString(), CSVWordStyle.Instance.GetConfData(Sys_Pet.Instance.GetGradeStarTextStyleId(grade)));
                    remakeGradeTran.SetAsLastSibling();
                }
                else if (type == 3)
                {
                    // 重塑的档位
                    uint previewGrade = clientPet.GetPetBuildTempGradeAttr(attr);
                    uint baseGrade = clientPet.GetPetGradeAttr(attr);
                    per.text = string.Format("{0}/{1}", (baseGrade + previewGrade).ToString("0.#"), (currentConfigValue + previewGrade).ToString("0.#"));
                    if (null != bulidper)
                        bulidper.text = "";
                    DwShow(clientPet.GetPetGradeAttr(attr), currentConfigValue);
                    var grade =  previewGrade;
                    remakeGradeTran.gameObject.SetActive(grade > 0);
                    ImageHelper.SetIcon(remakeImage, Sys_Pet.Instance.GetGradeStarImageId(grade));
                    remakeBgImage.color = Sys_Pet.Instance.GetGradeStarBgColor(grade);
                    TextHelper.SetText(remakeGradeText, "+" + grade.ToString(), CSVWordStyle.Instance.GetConfData(Sys_Pet.Instance.GetGradeStarTextStyleId(grade)));
                    remakeGradeTran.SetAsLastSibling();
                }
                else if(type == 4)
                {
                    advSlider.gameObject.SetActive(false);
                    prepGoGroup.gameObject.SetActive(false);
                    per.text = "";
                    if (null != bulidper)
                        bulidper.text = "";
                }
                else if (type == 5)
                {
                    currentPetValue = clientPet.GetPetGradeAttr(attr);
                    uint previewGrade = clientPet.GetPetBuildTempGradeTotalAttr(attr) + clientPet.GetPetBuildTempGradeAttr(attr);
                    per.text = string.Format("{0}/{1}", (currentPetValue + previewGrade).ToString("0.#"), (currentConfigValue + previewGrade).ToString("0.#"));
                    if (null != bulidper)
                        bulidper.text = "";
                    DwShow(clientPet.GetPetGradeAttr(attr), currentConfigValue);
                    remakeGradeTran.gameObject.SetActive(previewGrade > 0);
                    ImageHelper.SetIcon(remakeImage, Sys_Pet.Instance.GetGradeStarImageId(previewGrade));
                    remakeBgImage.color = Sys_Pet.Instance.GetGradeStarBgColor(previewGrade);
                    TextHelper.SetText(remakeGradeText, "+" + previewGrade.ToString(), CSVWordStyle.Instance.GetConfData(Sys_Pet.Instance.GetGradeStarTextStyleId(previewGrade)));
                    remakeGradeTran.SetAsLastSibling();
                }
                advSlider.value = 1.0f;
            }
            else
            {
                advSlider.value = 0f;
                per.text = "";
                DwShow(0, 0);
            }
            
        }

    }

    public class UI_Pet_BuildAdvanceView
    {
        private List<UI_Pet_BuildAdvance_Sub> uI_Pet_Advance_Subs = new List<UI_Pet_BuildAdvance_Sub>();
        private Text skillcurrentPercent;
        private Text skillNextPercent;
        public void Init(Transform transform)
        {
            uI_Pet_Advance_Subs.Clear();
            UI_Pet_BuildAdvance_Sub vit_Sub = new UI_Pet_BuildAdvance_Sub(5, EBaseAttr.Vit);
            vit_Sub.Init(transform.Find("Vit/Text_Vit"));
            uI_Pet_Advance_Subs.Add(vit_Sub);

            UI_Pet_BuildAdvance_Sub pow_Sub = new UI_Pet_BuildAdvance_Sub(7, EBaseAttr.Snh);
            pow_Sub.Init(transform.Find("Pow/Text_Pow"));
            uI_Pet_Advance_Subs.Add(pow_Sub);

            UI_Pet_BuildAdvance_Sub str_Sub = new UI_Pet_BuildAdvance_Sub(9, EBaseAttr.Inten);
            str_Sub.Init(transform.Find("Str/Text_Str"));
            uI_Pet_Advance_Subs.Add(str_Sub);

            UI_Pet_BuildAdvance_Sub mp_Sub = new UI_Pet_BuildAdvance_Sub(11, EBaseAttr.Magic);
            mp_Sub.Init(transform.Find("Ma/Text_Ma"));
            uI_Pet_Advance_Subs.Add(mp_Sub);

            UI_Pet_BuildAdvance_Sub spe_Sub = new UI_Pet_BuildAdvance_Sub(13, EBaseAttr.Speed);
            spe_Sub.Init(transform.Find("Spe/Text_Spe"));
            uI_Pet_Advance_Subs.Add(spe_Sub);

            skillcurrentPercent = transform.Find("Field/Text_Mp/Text_Value")?.GetComponent<Text>();
            skillNextPercent = transform.Find("Field/Text_Mp/Text_Plus")?.GetComponent<Text>();
        }

        /// <summary>
        /// type  0 代表显示本身的属性(分子分母形式) 1 代表显示预览(当前属性形式 + 范围) 2 代表显示未保存属性（分子分母+预览值 形式 ）3重塑档位 4 空态
        /// 5 重置属性
        /// </summary>
        /// <param name="client"></param>
        /// <param name="selectItemId"></param>
        /// <param name="type"></param>
        public void RefreshView(ClientPet client, uint type = 0)
        {
            bool showRemakeAttr = type == 1;
            for (int i = 0; i < uI_Pet_Advance_Subs.Count; i++)
            {
                uI_Pet_Advance_Subs[i].RefreshItem(client, type);
                uI_Pet_Advance_Subs[i].ShowBulidPercent(showRemakeAttr);
            }

            if (null != client)
            {
                //技能格子数量
                int currentCeilNum = client.GetPeBuildtSkillGridsCount();
                //获取技能实际数量 不包括 0 技能
                int currentSkillNum = client.GetPeBuildtSkillNotZeroCount();
                if (null != skillcurrentPercent)
                {
                    if (type == 0)
                    {
                        skillcurrentPercent.text = string.Format("{0}/{1}", currentSkillNum.ToString(), currentCeilNum.ToString());
                    }
                    else if(type == 1)
                    {
                        skillcurrentPercent.text = currentSkillNum.ToString();
                    }
                    else if(type == 2)
                    {
                        skillcurrentPercent.text = string.Format("{0}/{1}", (currentSkillNum + client.GetBuildPreviewSkillNotZeroCount()).ToString(), (currentCeilNum + client.GetBuildPreviewSkillCount()).ToString());
                    }
                    else if(type == 4)
                    {
                        skillcurrentPercent.text = "";
                    }
                    else if(type == 5)
                    {
                        skillcurrentPercent.text = string.Format("{0}/{1}", (currentSkillNum - client.GetReRemakdPreviewSkillNotZeroCount()).ToString(), (currentCeilNum).ToString());
                    }
                    
                }
                
                if (showRemakeAttr)
                {
                    //CSVPetRemake.Data data= Sys_Pet.Instance.GetRemakeData((uint)client.GetPeBuildCount() + 1, 0);
                    //List<uint> add_skill_num = Sys_Pet.Instance.GetRemakeSkillNums(data);
                    //skillNextPercent.text = LanguageHelper.GetTextContent(10926, (currentCeilNum + add_skill_num[0]).ToString(), (currentCeilNum + add_skill_num[1]).ToString());
                    int index = client.GetPeBuildCount();
                    var tempData = Sys_Pet.Instance.BuildSkillNum;
                    skillNextPercent.text = LanguageHelper.GetTextContent(10926, "0", tempData[index].ToString());
                }
            }
            else
            {
                if (null != skillcurrentPercent)
                {
                    skillcurrentPercent.text = "";
                }
            }
            if(null != skillNextPercent)
            {
                skillNextPercent.gameObject.SetActive(showRemakeAttr);
            }
        }
    }

    public class UI_PetLeftView_Common : UIComponent
    {
        public ClientPet clientPet;
        public GameObject Fx_pet_level_up;
        private CSVPetNew.Data curCsvData;
        //模型3渲2
        public Image eventImage;
        // 属相属性相关
        private GameObject attrGo;
        //品质相关
        private Image cardQuality;
        //卡片品质名称 金银普
        //private Text cardName;
        private Image cardLevel;
        //宠物名称等级
        private Text petName;        
        private Text petLevel;
        //种族图标
        //private Image petzz;
        private Text petZzNameText;
        //标识相关 - 稀有，出战状态-骑宠--出战等级-档位信息-改造信息
        private GameObject rareTab;
        private GameObject battleTab;
        private GameObject mountTab;
        private GameObject isMountGo;
        private Text battleLevel;
        private Text gradeInfoText;
        private Text remakeText;
        //评分
        private Text comprehensiveGgrade;
        //private GameObject model;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;

        private AsyncOperationHandle<GameObject> requestRef;
        private AsyncOperationHandle<GameObject> perfectFxrequestRef;
        private AsyncOperationHandle<GameObject> demonSpiritFxrequestRef;
        protected override void Loaded()
        {
            eventImage = transform.Find("EventImage").GetComponent<Image>();

            petName = transform.Find("Image_Namebg/Text_Name").GetComponent<Text>();
            petLevel = transform.Find("Image_Namebg/Text_Level")?.GetComponent<Text>();
            //petzz= transform.Find("Image_Namebg/Image_Type/Image").GetComponent<Image>();
            petZzNameText = transform.Find("Image_Namebg/Text_Race")?.GetComponent<Text>();

            battleTab = transform.Find("Image_Battle").gameObject;
            rareTab = transform.Find("Image_Rare").gameObject;
            mountTab = transform.Find("Image_Ride")?.gameObject;
            isMountGo = transform.Find("Image_Mount")?.gameObject;
            battleLevel = transform.Find("Image_Namebg/Text_Combat")?.GetComponent<Text>();
            gradeInfoText = transform.Find("Number/Text")?.GetComponent<Text>();
            remakeText = transform.Find("Image_Namebg/Text_Amount")?.GetComponent<Text>();

            cardQuality = transform.Find("Image_Card")?.GetComponent<Image>();

            //cardName = transform.Find("Image_Card/Text_CardName").GetComponent<Text>();
            cardLevel = transform.Find("Image_Card/Text_CardLevel")?.GetComponent<Image>();
            comprehensiveGgrade = transform.Find("Image_Point/Text_Comprehensive_Grade/Text_Num").GetComponent<Text>();
            attrGo = transform.Find("Image_Attr/Image_Bg").gameObject;
            attrGo.gameObject.SetActive(false);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            if (null != cardQuality)
            {
                cardQuality.raycastTarget = true;
                UI_LongPressButton card_tipsLongPressButton = cardQuality.gameObject.AddComponent<UI_LongPressButton>();
                card_tipsLongPressButton.onStartPress.AddListener(OnLongPressed);
                card_tipsLongPressButton.onRelease.AddListener(OnPointerUp);
            }
        }

        private void OnLongPressed()
        {
            UIManager.OpenUI(EUIID.UI_Card_Tips, false, LanguageHelper.GetTextContent(10999, LanguageHelper.GetTextContent(10999u + curCsvData.card_type), curCsvData.card_lv.ToString()));
        }

        private void OnPointerUp()
        {
            UIManager.CloseUI(EUIID.UI_Card_Tips);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUnloadModel, UnloadModel, toRegister);
        }


        public override void Show()
        {
            base.Show();
            CheckPetFashion();
        }

        public override void Hide()
        {
            base.Hide();

            UnloadModel();
            clientPet = null;
        }

        void UpdateView()
        {
            bool isNull = null != clientPet;
            if (isNull)
            {
                petName.text = Sys_Pet.Instance.GetPetName(clientPet);
                if(null != petLevel)
                    petLevel.text = LanguageHelper.GetTextContent(2009330, clientPet.petUnit.SimpleInfo.Level.ToString());
                CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(clientPet.petData.race);
                if(null != cSVGenusData)
                {
                    if(null != petZzNameText)
                    {
                        TextHelper.SetText(petZzNameText, cSVGenusData.rale_name);
                    }                    
                }
                else
                {
                    petZzNameText?.gameObject.SetActive(false);
                }
                ImageHelper.SetIcon(rareTab.GetComponent<Image>(), Sys_Pet.Instance.GetQuality_ScoreImage(clientPet));
                battleTab.SetActive(Sys_Pet.Instance.fightPet.IsSamePet(clientPet.petUnit));
                mountTab?.SetActive(false);
                if(null != cardQuality)
                {
                    ImageHelper.GetPetCardLevel(cardLevel, curCsvData.card_lv);
                    ImageHelper.SetIcon(cardQuality, Sys_Pet.Instance.SetPetQuality(curCsvData.card_type));
                }
                   
                comprehensiveGgrade.text = clientPet.petUnit.SimpleInfo.Score.ToString();

                uint styleId = 151;
                if (Sys_Role.Instance.Role.Level < clientPet.petData.participation_lv)
                {
                    styleId = 22;
                }
                CSVWordStyle.Data worldStyleData = CSVWordStyle.Instance.GetConfData(styleId);

                TextHelper.SetText(battleLevel, LanguageHelper.GetTextContent(12461, clientPet.petData.participation_lv.ToString()), worldStyleData);
                isMountGo?.SetActive(clientPet.petData.mount);
                if (null != gradeInfoText)
                {
                    uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                    bool isMax = lowG == 0;
                    if (isMax)
                    {
                        TextHelper.SetText(gradeInfoText, LanguageHelper.GetTextContent(11713, clientPet.GetPetCurrentGradeCount().ToString(), clientPet.GetPetBuildMaxGradeCount().ToString()));
                    }
                    else
                    {
                        TextHelper.SetText(gradeInfoText, LanguageHelper.GetTextContent(11712, clientPet.GetPetCurrentGradeCount().ToString(), clientPet.GetPetBuildMaxGradeCount().ToString(), lowG.ToString()));
                    }
                }
                
                if (null != remakeText)
                {
                    bool remakeIsOpen = Sys_FunctionOpen.Instance.IsOpen(10581);
                    int count = clientPet.GetPeBuildCount();
                    TextHelper.SetText(remakeText, LanguageHelper.GetTextContent(11879, count.ToString()));
                    remakeText.gameObject.SetActive(remakeIsOpen && count > 0);
                }
            }
            else
            {
                petName.text = "";
                if(null != petLevel)
                    petLevel.text = "";
                battleTab.SetActive(false);
                mountTab?.SetActive(false);
                isMountGo?.SetActive(false);
                if(null != gradeInfoText)
                {
                    gradeInfoText.text = "";
                }
                if (null != remakeText)
                {
                    remakeText.gameObject.SetActive(false);
                }
            }

            rareTab.SetActive(isNull);
            comprehensiveGgrade.transform.parent.gameObject.SetActive(isNull);
            battleLevel.transform.parent.gameObject.SetActive(isNull);
            SetPetPkAttr();
        }

        public void SetPetPkAttr()
        {
            for (int i = 0; i < attrGo.transform.parent.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(attrGo.transform.parent.GetChild(i).gameObject);
            }

            if(null != clientPet)
            {
                List<AttrPair> eleList = clientPet.GetPetEleAttrList();
                for (int i = 0; i < eleList.Count; i++)
                {
                    AttrPair attrPair = eleList[i];
                    CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attrPair.AttrId);
                    GameObject go = GameObject.Instantiate<GameObject>(attrGo, attrGo.transform.parent);
                    ImageHelper.SetIcon(go.transform.Find("Image_Attr").GetComponent<Image>(), cSVAttrData.attr_icon);
                    TextHelper.SetText(go.transform.Find("Image_Attr/Text").GetComponent<Text>(), attrPair.AttrValue.ToString());
                    go.SetActive(true);
                }
            }
           
        }

        public void SetValue(ClientPet _clientPet)
        {
            if(null != _clientPet)
            {
                curCsvData = CSVPetNew.Instance.GetConfData(_clientPet.petUnit.SimpleInfo.PetId);
                if (clientPet == null)
                {
                    clientPet = _clientPet;
                    _LoadShowScene();
                    _LoadShowModel(_clientPet);
                }
                else
                {
                    uint curUid = clientPet.petUnit.Uid;
                    clientPet = _clientPet;
                    if (curUid != _clientPet.petUnit.Uid)
                    {
                        UnloadModel();
                        _LoadShowScene();
                        _LoadShowModel(_clientPet);
                    }
                }
            }
            else
            {
                clientPet = null;
                UnloadModel();
            }
            
            UpdateView();

        }

        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            //petDisplay?.Dispose();
            //petDisplay = null;
            if (null != petDisplay && null != petDisplay.mAnimation)
            {
                petDisplay.mAnimation.StopAll();
            }
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();            
            showSceneControl = null;
            petDisplay = null;
            modelGo = null;
            if (requestRef.IsValid())
            {
                AddressablesUtil.Release<GameObject>(ref requestRef, MHandle_Completed);
            }
            if(perfectFxrequestRef.IsValid())
            {
                AddressablesUtil.Release<GameObject>(ref perfectFxrequestRef, PerfectFxMHandle_Completed);
            }
            if (demonSpiritFxrequestRef.IsValid())
            {
                AddressablesUtil.Release<GameObject>(ref demonSpiritFxrequestRef, DemonSpiritFxMHandle_Completed);
            }
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3((int)(EUIID.UI_Pet_Message), 0, 0);
            if (assetDependencies.mCustomDependencies[0].name == "Pet_DevelopShowScene")
            {
                Fx_pet_level_up = sceneModel.transform.Find("bg/Fx_pet_level_up").gameObject;
            }
            showSceneControl.Parse(sceneModel);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }       
        private void _LoadShowModel(ClientPet clientPet)
        {
            string _modelPath = Sys_Pet.Instance.GetPetModelPath(clientPet);
            //player = GameCenter.modelShowWorld.CreateActor<ModelShowActor>(clientPet.petUnit.PetId);
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.Rotate(new Vector3(curCsvData.angle1, curCsvData.angle2, curCsvData.angle3));
            showSceneControl.mModelPos.transform.localScale = new Vector3(curCsvData.size, curCsvData.size, curCsvData.size);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + curCsvData.translation, curCsvData.height, showSceneControl.mModelPos.transform.localPosition.z);
        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint highId = clientPet.petUnit.SimpleInfo.PetId;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                CheckPetFashion();
                modelGo.SetActive(false);
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, curCsvData.weapon, Constants.PetShowAnimationClipHashSet, go: modelGo);
                ani_index = 0;
                checkAni = true;
                uint lowG = clientPet.GetPetMaxGradeCount() - clientPet.GetPetGradeCount();
                if (lowG == 0)
                {
                    LoadPetGeerAssetAsyn(Sys_Pet.Instance.GetPetGearFxPath(clientPet.petData, true));
                    var perfectPath = Sys_Pet.Instance.GetPetRemakePerfectFxPath(clientPet);
                    if (null != perfectPath)
                    {
                        LoadPetPerfectAssetAsyn(perfectPath);
                    }
                    if (Sys_Pet.Instance.IsNeedShowDemonSpiritFx(clientPet))
                    {
                        LoadPetDemonSpiritAssetAsyn(Sys_Pet.Instance.DemonSpiritFxString);
                    }
                }
            }
        }  

        bool checkAni = false;
        int ani_index = 0;
        protected override void Update()
        {
            if (null != petDisplay && null != petDisplay.mAnimation && checkAni && null != modelGo && modelGo.activeSelf)
            {
                if (petDisplay?.mAnimation.GetClipCount() == Constants.PetShowAnimationClip.Count)
                {
                    checkAni = false;
                    PlayAnimator();
                }
            }
        }

        private void CheckPetFashion()
        {
            if(null != modelGo)
            {
                SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, clientPet.GetPetSuitFashionId(), modelGo.transform);
            }
        }

        private void PlayAnimator()
        {
            if (null != petDisplay && Constants.PetShowAnimationClip.Count > ani_index)
            {
                petDisplay.mAnimation?.CrossFade(Constants.PetShowAnimationClip[ani_index], Constants.CORSSFADETIME, CrossFadeEnd);
            }
            else
            {
                petDisplay.mAnimation?.CrossFade((uint)EStateType.Idle, Constants.CORSSFADETIME);
            }
        }

        private void CrossFadeEnd()
        {
            ani_index += 1;
            PlayAnimator();
        }

        private void SetFx(GameObject fxGo)
        {
            if (null != modelGo)
            {
                Transform fxParent = showSceneControl.mRoot.transform.Find("Fx");
                if (null != fxParent)
                {
                    if (fxGo != null)
                    {
                        LayerMaskUtil.Setlayer(fxGo.transform, ELayerMask.ModelShow);
                    }
                }

                fxGo.transform.SetParent(fxParent, false);
            }
        }

        private void PetFxSetLayer(Transform _transform, int layer)
        {
            if (_transform != null)
            {
                transform.gameObject.layer = layer;
                int count = _transform.childCount;
                for (int i = 0; i < count; ++i)
                {
                    PetFxSetLayer(_transform.GetChild(i), layer);
                }
            }
        }

        private void LoadPetGeerAssetAsyn(string path)
        {
            if(null != path)
            {
                AddressablesUtil.InstantiateAsync(ref requestRef, path, MHandle_Completed);
            }
        }

        private void LoadPetPerfectAssetAsyn(string path)
        {
            if (null != path)
            {
                AddressablesUtil.InstantiateAsync(ref perfectFxrequestRef, path, PerfectFxMHandle_Completed);
            }
        }

        private void LoadPetDemonSpiritAssetAsyn(string path)
        {
            if (null != path)
            {
                AddressablesUtil.InstantiateAsync(ref demonSpiritFxrequestRef, path, DemonSpiritFxMHandle_Completed);
            }
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            SetFx(handle.Result);
        }

        private void PerfectFxMHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            SetFx(handle.Result);
        }

        private void DemonSpiritFxMHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            SetFx(handle.Result);
        }

        public void OnDrag(BaseEventData eventData)
        {
            if(null != clientPet)
            {
                PointerEventData ped = eventData as PointerEventData;
                Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
                AddEulerAngles(angle);
            }            
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }
    }

    public class UI_Pet_DomesticationList : UI_Pet_ViewList
    {
        private List<int> petIndex;
        public override void InitGrid()
        {
            gridIndex = PetListCount();
            petIndex?.Clear();
            petIndex = Sys_Pet.Instance.GetDomesticationListIndex();
            for (int i = 0; i < gridIndex; ++i)
            {
                if(petIndex.Contains(i))
                {
                    if (i < uI_Pet_Cells.Count)
                    {
                        uI_Pet_Cells[i].ReSetData();
                        uI_Pet_Cells[i].SetActive(true);
                    }
                    else
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(petItemGo, petGridGo.transform);
                        InstanitatePetCell(go.transform, i, true);
                    }
                }
                else
                {
                    if (i < uI_Pet_Cells.Count)
                    {
                        uI_Pet_Cells[i].SetActive(false);
                    }
                    else
                    {
                        GameObject go = GameObject.Instantiate<GameObject>(petItemGo, petGridGo.transform);
                        InstanitatePetCell(go.transform, i, false);
                    }
                }
            }

            if (gridIndex < uI_Pet_Cells.Count)
            {
                for (int i = gridIndex; i < uI_Pet_Cells.Count; i++)
                {
                    uI_Pet_Cells[i].SetActive(false);
                }
            }
            scrollRect.StopMovement();
        }

        public override void Show()
        {
            InitGrid();
        }
    }

    public class UI_Pet_ViewList : UIComponent
    {
        protected GridLayoutGroup petGridGo;
        protected GameObject petItemGo;
        protected ScrollRect scrollRect;

        protected List<UI_Pet_Cell> uI_Pet_Cells = new List<UI_Pet_Cell>();

        protected InfinityGrid _infinityGrid;
        protected override void Loaded()
        {
            scrollRect = transform.GetComponent<ScrollRect>();
            petGridGo = transform.Find("Grid").GetComponent<GridLayoutGroup>();
            petItemGo = transform.Find("Grid/PetItem01").gameObject;
            ClientPet temp = Sys_Pet.Instance.GetPostion2ClientPet();
            for (int i = 0; i < petGridGo.transform.childCount; i++)
            {
                InstanitatePetCell(petGridGo.transform.GetChild(i), i, true);
            }
        }

        protected void InstanitatePetCell(Transform trn, int index, bool initState)
        {
            UI_Pet_Cell cell = new UI_Pet_Cell((uint)index);
            cell.Init(trn);
            cell.ReSetData();
            cell.SetActive(initState);
            uI_Pet_Cells.Add(cell);
        }

        public override void Show()
        {
            InitGrid();
        }

        public void ProcessEvents(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateExp, UpdateExp, toRegister);
        }

        protected void UpdateExp()
        {
            UpdateAllgrid();
        }

        public override void Hide()
        {
            scrollRect.content.localPosition = new Vector2(0, 0);
        }
        public int gridIndex;

        public virtual void InitGrid()
        {
            gridIndex = PetListCount();
            for (int i = 0; i < gridIndex; ++i)
            {
                if(i < uI_Pet_Cells.Count)
                {
                    uI_Pet_Cells[i].ReSetData();
                    uI_Pet_Cells[i].SetActive(true);
                }
                else
                {
                    GameObject go = GameObject.Instantiate<GameObject>(petItemGo, petGridGo.transform);
                    InstanitatePetCell(go.transform, i, true);
                }
            }

            if(gridIndex < uI_Pet_Cells.Count)
            {
                for (int i = gridIndex; i < uI_Pet_Cells.Count; i++)
                {
                    uI_Pet_Cells[i].SetActive(false);
                }
            }
            scrollRect.StopMovement();
        }

        public void UpdateAllgrid()
        {
            for (int i = 0; i < uI_Pet_Cells.Count; i++)
            {
                uI_Pet_Cells[i].ReSetData();
            }
        }

        public void SetSelect(uint select)
        {
            for (int i = 0; i < uI_Pet_Cells.Count; i++)
            {
                uI_Pet_Cells[i].RefreshSelect(select);
            } 
        }

        public void SetPosView(uint select)
        {
            float top = petGridGo.padding.top;
            float spaY = petGridGo.spacing.y;
            float celly = petGridGo.cellSize.y;
            float p_y = (select) * celly + top + (select) * spaY;
            scrollRect.StopMovement();
            scrollRect.content.localPosition = new Vector2(0, p_y);
            //scrollRect.content.DOLocalMoveY(p_y, 0.3f);// new Vector2(0, , 0);
        }

        public void CheckUnlcokRedState()
        {
            if (Sys_Pet.Instance.costBagNum < Sys_Pet.Instance.MaxCostListCout)
            {
                int index = gridIndex;
                if (Sys_Pet.Instance.MaxLevelListCount > Sys_Pet.Instance.bagNum)
                {
                    index -= 2;
                }
                else
                {
                    index -= 1;
                }
                bool showRend = Sys_Pet.Instance.PetBagCostRedState();
                uI_Pet_Cells[index].SetRedState(showRend);
            }
        }

        public virtual int PetListCount()
        {
            int count = (int)Sys_Pet.Instance.devicesCount;
            if(Sys_Pet.Instance.bagNum < Sys_Pet.Instance.MaxLevelListCount)
            {
                count += 1;
            }
            if (Sys_Pet.Instance.costBagNum < Sys_Pet.Instance.MaxCostListCout)
            {
                count += 1;
            }
            return count;
        }
    }

    public class UI_PetEquipSlot : UIComponent
    {
        private Button petMagicCoreType1Btn;
        private Image type1Image;
        private Image type1QualityImage;
        private Button petMagicCoreType2Btn;
        private Image type2Image;
        private Image type2QualityImage;
        private Button petMagicCoreType3Btn;
        private Image type3Image;
        private Image type3QualityImage;
        private ClientPet clientPet;
        public EUIID openUI = EUIID.UI_Pet_Message;
        protected override void Loaded()
        {
            type1Image = transform.Find("Btn_mowen/Image_Icon").GetComponent<Image>();
            type2Image = transform.Find("Btn_mojing/Image_Icon").GetComponent<Image>();
            type3Image = transform.Find("Btn_mocui/Image_Icon").GetComponent<Image>();
            
            type1QualityImage = transform.Find("Btn_mowen/Image_Quality").GetComponent<Image>();
            type2QualityImage = transform.Find("Btn_mojing/Image_Quality").GetComponent<Image>();
            type3QualityImage = transform.Find("Btn_mocui/Image_Quality").GetComponent<Image>();

            petMagicCoreType1Btn = transform.Find("Btn_mowen").GetComponent<Button>();
            petMagicCoreType2Btn = transform.Find("Btn_mojing").GetComponent<Button>();
            petMagicCoreType3Btn = transform.Find("Btn_mocui").GetComponent<Button>();

            petMagicCoreType1Btn.onClick.AddListener(MoWenBtnClicked);
            petMagicCoreType2Btn.onClick.AddListener(MoJingBtnClicked);
            petMagicCoreType3Btn.onClick.AddListener(MocuiBtnClicked);
        }

        public void SetPetEquip(PetUnit petUnit)
        {
            ClientPet clientPet = new ClientPet(petUnit);
            this.clientPet = clientPet;
            SetPetEquip(clientPet);
        }

        public void SetPetEquip(ClientPet clientPet)
        {
            this.clientPet = clientPet;
            if(null != clientPet)
            {
                if (!clientPet.IsHasEquip())
                {
                    type1Image.gameObject.SetActive(false);
                    type2Image.gameObject.SetActive(false);
                    type3Image.gameObject.SetActive(false);
                    type1QualityImage.gameObject.SetActive(false);
                    type2QualityImage.gameObject.SetActive(false);
                    type3QualityImage.gameObject.SetActive(false);

                }
                else
                {
                    ItemData petEquip1 = clientPet.GetPetEquipByType(1);
                    bool hasEquip = petEquip1 != null;
                    if (hasEquip)
                    {
                        type1QualityImage.color = SetBgQuality(petEquip1.petEquip.Color);
                        ImageHelper.SetIcon(type1Image, petEquip1.cSVItemData.icon_id);
                    }
                    type1Image.gameObject.SetActive(hasEquip);
                    type1QualityImage.gameObject.SetActive(hasEquip);
                    ItemData petEquip2 = clientPet.GetPetEquipByType(2);
                    hasEquip = petEquip2 != null;
                    if (hasEquip)
                    {
                        type2QualityImage.color = SetBgQuality(petEquip2.petEquip.Color);
                        ImageHelper.SetIcon(type2Image, petEquip2.cSVItemData.icon_id);
                    }
                    type2Image.gameObject.SetActive(hasEquip);
                    type2QualityImage.gameObject.SetActive(hasEquip);
                    ItemData petEquip3 = clientPet.GetPetEquipByType(3);
                    hasEquip = petEquip3 != null;
                    if (hasEquip)
                    {
                        type3QualityImage.color = SetBgQuality(petEquip3.petEquip.Color);
                        ImageHelper.SetIcon(type3Image, petEquip3.cSVItemData.icon_id);
                    }
                    type3Image.gameObject.SetActive(hasEquip);
                    type3QualityImage.gameObject.SetActive(hasEquip);
                }
            }
        }

        public Color SetBgQuality(uint quality)
        {
            Color qualityColor;
            switch ((EItemQuality)quality)
            {
                case EItemQuality.White:
                    qualityColor = new Color(210/255f,202/255f,225/255f,255/255f);
                    break;
                case EItemQuality.Green:
                    qualityColor = new Color(87 / 255f, 221 / 255f, 102 / 255f, 255 / 255f);
                    break;
                case EItemQuality.Blue:
                    qualityColor = new Color(50 / 255f, 214 / 255f, 255 / 255f, 255 / 255f);
                    break;
                case EItemQuality.Purple:
                    qualityColor = new Color(172 / 255f, 134 / 255f, 242 / 255f, 255 / 255f);
                    break;
                case EItemQuality.Orange:
                    qualityColor = new Color(245 / 255f, 194 / 255f, 116 / 255f, 255 / 255f);
                    break;
                default:
                    qualityColor = new Color(210 / 255f, 202 / 255f, 225 / 255f, 255 / 255f);
                    break;
            }

            return qualityColor;
        }

        private void OnClickeSlot(uint type)
        {
            if (null != clientPet)
            {
                ItemData petEquip = clientPet.GetPetEquipByType(type);
                if (null != petEquip)
                {
                    PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                    petEquipTipsData.openUI = openUI;
                    petEquipTipsData.petUid = clientPet.GetPetUid();
                    petEquipTipsData.petEquip = petEquip;
                    petEquipTipsData.isCompare = false;
                    petEquipTipsData.isShowOpBtn = true;
                    petEquipTipsData.isShowLock = clientPet.isShowLock;
                    UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
                }
                else
                {
                    if (openUI == EUIID.UI_Pet_Message)
                    {
                        if (clientPet.petUnit.SimpleInfo.ExpiredTick > 0)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000667));
                            return;
                        }
                        UIManager.OpenUI(EUIID.UI_SelectPetEquip, false, new Tuple<uint, uint, uint>(clientPet.GetPetUid(), type, 1018201u));
                    }
                }
            }
        }

        private void MocuiBtnClicked()
        {
            OnClickeSlot(3);
        }

        private void MoJingBtnClicked()
        {
            OnClickeSlot(2);
        }

        private void MoWenBtnClicked()
        {
            OnClickeSlot(1);
            
        }
    }
}
