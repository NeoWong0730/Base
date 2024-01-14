using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;
using Logic.Core;
using Framework;
using UnityEngine.EventSystems;

namespace Logic
{

    public partial class UI_CurrencyTitle
    {
        private GameObject mRoot;
        private GameObject currency1;
        private GameObject currency2;
        private GameObject currency3;
        private GameObject currencySpecial;
        private Image mIconImage1;
        private Image mIconImage2;
        private Image mIconImage3;
        private Image mIconImageSpecial;
        private Button mCurrency1;
        private Button mCurrency2;
        private Button mCurrency3;
        private Button mCurrencySpecial;
        private Text mCountText1;
        private Text mCountText2;
        private Text mCountText3;
        private Text mCountTextSpecial;

        private Button mButtonLock1;
        private Button mButtonLock2;
        private Button mButtonLock3;
        private Button mButtonLockSpecial;
        private Transform mLockParent1;
        private Transform mLockParent2;
        private Transform mLockParent3;
        private Transform mLockParentSpecial;

        public static readonly List<uint> DefaultCurrencies = new List<uint>() { 1, 2, 3 };
        private List<uint> datas = new List<uint>() { 1, 2, 3 };
        private Dictionary<uint, Text> textbinds = new Dictionary<uint, Text>();

        CoroutineHandler mCoroutineHandler;

        private GameObject m_ViewLockTips;
        private Image m_ClickClose;
        private Image m_FrozenIcon;
        private Text m_FrozenTitle;
        private Text m_FrozenAllNum;
        private Text m_FrozenContent1;
        private Text m_FrozenContent2;
        private Image m_FrozenIcon2;
        private Text m_FrozenNextNum;

        public UI_CurrencyTitle(GameObject _gameObject)
        {
            mRoot = _gameObject;
            ParseComponent();
            InitData();
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, true);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnFrozenCurrency, InitUi, true);
            mRoot.gameObject.SetActive(false);
        }

        public void SetData(List<uint> _datas, uint _id = 5)
        {
            ClearData();
            for (int i = 0; i < _datas.Count; i++)
            {
                if (i > 2)
                    break;
                datas.Add(_datas[i]);
            }
            SpecialCoinId = _id;
            InitData();
            InitUi();
        }


        private void ParseComponent()
        {
            currency1 = mRoot.transform.Find("Image_Property01").gameObject;
            currency2 = mRoot.transform.Find("Image_Property02").gameObject;
            currency3 = mRoot.transform.Find("Image_Property03").gameObject;
            currencySpecial = mRoot.transform.Find("Image_PropertySpecial").gameObject;
            mIconImage1 = mRoot.transform.Find("Image_Property01/Image_Icon").GetComponent<Image>();
            mIconImage2 = mRoot.transform.Find("Image_Property02/Image_Icon").GetComponent<Image>();
            mIconImage3 = mRoot.transform.Find("Image_Property03/Image_Icon").GetComponent<Image>();
            mIconImageSpecial = mRoot.transform.Find("Image_PropertySpecial/Image_Icon").GetComponent<Image>();
            mCurrency1 = mRoot.transform.Find("Image_Property01/Button_Add").GetComponent<Button>();
            mCurrency2 = mRoot.transform.Find("Image_Property02/Button_Add").GetComponent<Button>();
            mCurrency3 = mRoot.transform.Find("Image_Property03/Button_Add").GetComponent<Button>();
            mCurrencySpecial = mRoot.transform.Find("Image_PropertySpecial/Button_Add").GetComponent<Button>();
            mCountText1 = mRoot.transform.Find("Image_Property01/Text_Number").GetComponent<Text>();
            mCountText2 = mRoot.transform.Find("Image_Property02/Text_Number").GetComponent<Text>();
            mCountText3 = mRoot.transform.Find("Image_Property03/Text_Number").GetComponent<Text>();
            mCountTextSpecial = mRoot.transform.Find("Image_PropertySpecial/Text_Number").GetComponent<Text>();
            mButtonLock1 = mRoot.transform.Find("Image_Property01/Button_Lock").GetComponent<Button>();
            mButtonLock2 = mRoot.transform.Find("Image_Property02/Button_Lock").GetComponent<Button>();
            mButtonLock3 = mRoot.transform.Find("Image_Property03/Button_Lock").GetComponent<Button>();
            mButtonLockSpecial = mRoot.transform.Find("Image_PropertySpecial/Button_Lock").GetComponent<Button>();
            mLockParent1 = mRoot.transform.Find("Image_Property01/View_Lock");
            mLockParent2 = mRoot.transform.Find("Image_Property02/View_Lock");
            mLockParent3 = mRoot.transform.Find("Image_Property03/View_Lock");
            mLockParentSpecial = mRoot.transform.Find("Image_PropertySpecial/View_Lock");
            mCurrency1.onClick.AddListener(OnmCurrency1ButtonClicked);
            mCurrency2.onClick.AddListener(OnmCurrency2ButtonClicked);
            mCurrency3.onClick.AddListener(OnmCurrency3ButtonClicked);
            mCurrencySpecial.onClick.AddListener(OnmCurrencySpecialButtonClicked);
            mButtonLock1.onClick.AddListener(OnButtonLockClicked1);
            mButtonLock2.onClick.AddListener(OnButtonLockClicked2);
            mButtonLock3.onClick.AddListener(OnButtonLockClicked3);
            mButtonLockSpecial.onClick.AddListener(OnButtonLockClickedSpecial);
        }

        private void InitData()
        {
            if (datas.Count == 1)
            {
                textbinds.Add(datas[0], mCountText3);
            }
            else if (datas.Count == 2)
            {
                textbinds.Add(datas[0], mCountText2);
                textbinds.Add(datas[1], mCountText3);
            }
            else
            {
                textbinds.Add(datas[0], mCountText1);
                textbinds.Add(datas[1], mCountText2);
                textbinds.Add(datas[2], mCountText3);
            }
            if (SpecialCoinId != 0)
            {
                textbinds.Add(SpecialCoinId, mCountTextSpecial);
            }
        }

        public void InitUi()
        {
            mRoot.gameObject.SetActive(true);
            if (datas.Count == 1)
            {
                mRoot.transform.GetChild(0).gameObject.SetActive(true);
                mRoot.transform.GetChild(1).gameObject.SetActive(false);
                mRoot.transform.GetChild(2).gameObject.SetActive(false);
                SetGo(mRoot.transform.GetChild(0).gameObject, datas[0]);
            }
            else if (datas.Count == 2)
            {
                mRoot.transform.GetChild(0).gameObject.SetActive(true);
                mRoot.transform.GetChild(1).gameObject.SetActive(true);
                mRoot.transform.GetChild(2).gameObject.SetActive(false);
                SetGo(mRoot.transform.GetChild(1).gameObject, datas[0]);
                SetGo(mRoot.transform.GetChild(0).gameObject, datas[1]);
            }
            else
            {
                mRoot.transform.GetChild(0).gameObject.SetActive(true);
                mRoot.transform.GetChild(1).gameObject.SetActive(true);
                mRoot.transform.GetChild(2).gameObject.SetActive(true);
                SetGo(mRoot.transform.GetChild(2).gameObject, datas[0]);
                SetGo(mRoot.transform.GetChild(1).gameObject, datas[1]);
                SetGo(mRoot.transform.GetChild(0).gameObject, datas[2]);
            }
            SpecialInitUI();
        }

        private void SetGo(GameObject go, uint id)
        {
            Image icon = go.transform.Find("Image_Icon").GetComponent<Image>();
            Text text = go.transform.Find("Text_Number").GetComponent<Text>();
            ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(id).small_icon_id);
            text.text = Sys_Bag.Instance.GetValueFormat((Sys_Bag.Instance.GetItemCount(id)));

            GameObject buttonAdd = go.transform.Find("Button_Add").gameObject;
            bool showAddBtn = true;
            if (id == (uint)ECurrencyType.FamilyStamina)
            {
                showAddBtn = false;
            }
            buttonAdd.SetActive(showAddBtn);
            Image infoIcon = buttonAdd.transform.Find("Button_Add (1)").GetComponent<Image>();
            uint iconId = 0;
            if (id == (uint)ECurrencyType.Spar
                || id == (uint)ECurrencyType.CompetitiveIntegral
                || id == (uint)ECurrencyType.Aid
                || id == (uint)ECurrencyType.CaptainPoint
                || id == (uint)ECurrencyType.Knowledge
                || id == (uint)ECurrencyType.AkaDyaAncientGoldenCoin
                || id == (uint)ECurrencyType.GuildCurrency
                || id == (uint)ECurrencyType.CurrencyPoint
                || id == (uint)ECurrencyType.Currency_35
                || id == (uint)ECurrencyType.TransformationPoint
                || id == (uint)ECurrencyType.Currency_34
                || id == (uint)ECurrencyType.Currency_36)
            {
                iconId = 990402;
            }
            else
            {
                bool isTrue = id == (uint) ECurrencyType.Runefragment || id == (uint) ECurrencyType.ActivityToken;
                iconId = isTrue ? 994701u : 990401u;
            }
            ImageHelper.SetIcon(infoIcon, iconId);

            GameObject buttonLock = go.transform.Find("Button_Lock").gameObject;
            buttonLock.SetActive(Sys_Bag.Instance.CurrencyFrozened(id));
        }


        private void ClearData()
        {
            textbinds.Clear();
            datas.Clear();
        }

        #region ButtonAdd
        private void OnmCurrency1ButtonClicked()
        {
            if (datas.Count == 1)
            {

            }
            else if (datas.Count == 2)
            {

            }
            else
            {
                OnClicked(datas[0]);
            }
        }

        private void OnmCurrency2ButtonClicked()
        {
            if (datas.Count == 1)
            {

            }
            else if (datas.Count == 2)
            {
                OnClicked(datas[0]);
            }
            else
            {
                OnClicked(datas[1]);
            }
        }

        private void OnmCurrency3ButtonClicked()
        {
            if (datas.Count == 1)
            {
                OnClicked(datas[0]);
            }
            else if (datas.Count == 2)
            {
                OnClicked(datas[1]);
            }
            else
            {
                OnClicked(datas[2]);
            }
        }

        private void OnClicked(uint currencyId)
        {
            ECurrencyType eCurrencyType = (ECurrencyType)currencyId;
            switch (eCurrencyType)
            {
                case ECurrencyType.None:
                    break;
                case ECurrencyType.Diamonds:
                    OnDiamondsClicked();
                    break;
                case ECurrencyType.GoldCoin:
                case ECurrencyType.SilverCoin:
                    Sys_Bag.Instance.TryOpenExchangeCoinUI(eCurrencyType, 0);
                    break;
                case ECurrencyType.Experience:
                    break;
                case ECurrencyType.Vitality:
                    break;
                case ECurrencyType.Spar:
                case ECurrencyType.CompetitiveIntegral:
                case ECurrencyType.Aid:
                case ECurrencyType.CaptainPoint:
                case ECurrencyType.Knowledge:
                case ECurrencyType.AkaDyaAncientGoldenCoin:
                case ECurrencyType.GuildCurrency:
                case ECurrencyType.ActivityToken:
                case ECurrencyType.CurrencyPoint:
                case ECurrencyType.Currency_35:
                case ECurrencyType.TransformationPoint:
                case ECurrencyType.Currency_34:
                case ECurrencyType.Currency_36:
                    OpenRule(eCurrencyType);
                    break;
                case ECurrencyType.Prestige:
                    break;
                case ECurrencyType.Feats:
                    break;
                case ECurrencyType.Runefragment:
                    UIManager.OpenUI(EUIID.UI_CurrencyRuneTips);
                    break;
                case ECurrencyType.PersonalContribution:
                    break;
                case ECurrencyType.FamilyCoin:
                    break;
                case ECurrencyType.FashionPoint:
                    if (UIManager.IsOpen(EUIID.UI_Mall))
                    {
                        UIManager.CloseUI(EUIID.UI_Mall);
                    }
                    UIManager.OpenUI(EUIID.UI_Fashion_LuckyDraw);
                    break;
                case ECurrencyType.Max:
                    break;
                default:
                    break;
            }
        }

        private void OnDiamondsClicked()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(40101, true))
            {
                if (UIManager.IsOpen(EUIID.UI_Mall))
                {
                    Sys_Mall.Instance.eventEmitter.Trigger(Sys_Mall.EEvents.OnTelCharge);
                }
                else
                {
                    MallPrama param = new MallPrama();
                    param.mallId = 101u;
                    param.isCharge = true;

                    UIManager.OpenUI(EUIID.UI_Mall, false, param);
                }
            }
        }

        private void OpenRule(ECurrencyType eCurrencyType)
        {
            uint titleLan = 0;
            uint contentLan = 0;
            switch (eCurrencyType)
            {
                case ECurrencyType.Spar:
                    titleLan = 1110001;
                    contentLan = 1100001;
                    break;
                case ECurrencyType.CompetitiveIntegral:
                    titleLan = 1110002;
                    contentLan = 1100002;
                    break;
                case ECurrencyType.Aid:
                    titleLan = 1110003;
                    contentLan = 1100003;
                    break;
                case ECurrencyType.CaptainPoint:
                    titleLan = 1110004;
                    contentLan = 1100004;
                    break;
                case ECurrencyType.Knowledge:
                    titleLan = 1110005;
                    contentLan = 1100005;
                    break;
                case ECurrencyType.AkaDyaAncientGoldenCoin:
                    titleLan = 1110006;
                    contentLan = 1100006;
                    break;
                case ECurrencyType.GuildCurrency:
                    titleLan = 1110007;
                    contentLan = 1100007;
                    break;
                case ECurrencyType.ActivityToken:
                    titleLan = 1500000022;
                    contentLan = 3940000000;
                    break;
                case ECurrencyType.CurrencyPoint:
                    titleLan = 1110008;
                    contentLan = 1100008;
                    break;
                case ECurrencyType.Currency_35:
                    titleLan = 3899000019;
                    contentLan = 3899000020;
                    break;
                case ECurrencyType.TransformationPoint:
                    titleLan = 2013030;
                    contentLan = 2013031;
                    break;
                case ECurrencyType.Currency_34:
                    titleLan = 590003100;
                    contentLan = 590003101;
                    break;
                case ECurrencyType.Currency_36:
                    titleLan = 3899000037;
                    contentLan = 3899000038;
                    break;
                default:
                    break;
            }
            PointRuleEvt evt = new PointRuleEvt();
            evt.titleLan = titleLan;
            evt.contentLan = contentLan;
            UIManager.OpenUI(EUIID.UI_PointRuleTips, false, evt);
        }

        #endregion

        #region Frozen

        private void OnButtonLockClicked1()
        {
            if (datas.Count == 1)
            {

            }
            else if (datas.Count == 2)
            {

            }
            else
            {
                OnButtonLockClicked(mLockParent1, datas[0]);
            }
        }

        private void OnButtonLockClicked2()
        {
            if (datas.Count == 1)
            {

            }
            else if (datas.Count == 2)
            {
                OnButtonLockClicked(mLockParent2, datas[0]);
            }
            else
            {
                OnButtonLockClicked(mLockParent2, datas[1]);
            }
        }

        private void OnButtonLockClicked3()
        {
            if (datas.Count == 1)
            {
                OnButtonLockClicked(mLockParent3, datas[0]);
            }
            else if (datas.Count == 2)
            {
                OnButtonLockClicked(mLockParent3, datas[1]);
            }
            else
            {
                OnButtonLockClicked(mLockParent3, datas[2]);
            }
        }

        private void OnButtonLockClicked(Transform parent, uint currencyId)
        {
            if (!parent.gameObject.activeSelf)
            {
                parent.gameObject.SetActive(true);
                m_ViewLockTips = GameObject.Instantiate<GameObject>(GlobalAssets.GetAsset<GameObject>(GlobalAssets.sPrefab_ViewLock_Tips));
                m_ViewLockTips.transform.SetParent(parent);
                m_ViewLockTips.transform.localPosition = Vector3.zero;
                m_ViewLockTips.transform.localScale = Vector3.one;

                m_ClickClose = m_ViewLockTips.transform.Find("Blank").GetComponent<Image>();
                m_FrozenIcon = m_ViewLockTips.transform.Find("Text_Title/Image_Icon").GetComponent<Image>();
                m_FrozenTitle = m_ViewLockTips.transform.Find("Text_Title").GetComponent<Text>();
                m_FrozenAllNum = m_ViewLockTips.transform.Find("Text_Title/Text_Num").GetComponent<Text>();
                m_FrozenContent1 = m_ViewLockTips.transform.Find("Text_Tips01").GetComponent<Text>();
                m_FrozenContent2 = m_ViewLockTips.transform.Find("Text_Tips02").GetComponent<Text>();
                m_FrozenIcon2 = m_ViewLockTips.transform.Find("Text_Tips03/Image_Icon").GetComponent<Image>();
                m_FrozenNextNum = m_ViewLockTips.transform.Find("Text_Tips03/Text_Num").GetComponent<Text>();

                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_ClickClose);
                eventListener.AddEventListener(EventTriggerType.PointerClick, (_) =>
                {
                    GameObject.Destroy(m_ViewLockTips);
                    parent.gameObject.SetActive(false);
                });

                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(currencyId);
                if (cSVItemData != null)
                {
                    ImageHelper.SetIcon(m_FrozenIcon, cSVItemData.icon_id);
                    TextHelper.SetText(m_FrozenTitle, LanguageHelper.GetTextContent(2022600, CSVLanguage.Instance.GetConfData(cSVItemData.name_id).words));
                    TextHelper.SetText(m_FrozenAllNum, Sys_Bag.Instance.GetAllFrozenNum(currencyId).ToString());
                    TextHelper.SetText(m_FrozenContent1, LanguageHelper.GetTextContent(2022601, Sys_Bag.Instance.GetAllFrozenNum(currencyId).ToString(),
                        CSVLanguage.Instance.GetConfData(cSVItemData.name_id).words, Sys_Bag.Instance.GetFrozenDay(currencyId, 0).ToString()));
                    TextHelper.SetText(m_FrozenContent2, LanguageHelper.GetTextContent(2022602, Sys_Bag.Instance.GetFrozenDay(currencyId, 1).ToString()));
                    TextHelper.SetText(m_FrozenNextNum, Sys_Bag.Instance.GetUnFrozenNum(currencyId).ToString());
                    ImageHelper.SetIcon(m_FrozenIcon2, cSVItemData.icon_id);
                }
            }
        }
        #endregion

        private void OnCurrencyChanged(uint id, long value)
        {
            long from = 0;
            if (!textbinds.ContainsKey(id))
                return;
            Text changeTex = textbinds[id];
            if (changeTex == null)
                return;
            from = Sys_Bag.Instance.GetItemCount(id);
            //uint.TryParse(changeTex.text, out from);
            if (mCoroutineHandler != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineHandler);
            }
            mCoroutineHandler = CoroutineManager.Instance.StartHandler(Refresh(from, value, 1, changeTex));
        }

        private IEnumerator Refresh(long from, long to, float duration, Text text)
        {
            bool running = true;
            float accumulateTime = 0f;
            long currentValue = from;
            if (to < 10000000 && from < 10000000)
            {
                if (from >= to)
                {
                    currentValue = to;
                    text.text = currentValue.ToString();
                    yield break;
                }
                while (running)
                {
                    yield return new WaitForEndOfFrame();
                    accumulateTime += Time.deltaTime;
                    if (accumulateTime >= duration)
                    {
                        running = false;
                        currentValue = to;
                    }
                    else
                    {
                        currentValue = (long)Mathf.CeilToInt(Mathf.Lerp(from, to, accumulateTime / duration));
                    }
                    text.text = currentValue.ToString();
                }
            }
            else
            {
                text.text = Sys_Bag.Instance.GetValueFormat(to);
            }
        }
        public void Dispose()
        {
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, false);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnFrozenCurrency, InitUi, false);
            if (mCoroutineHandler != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineHandler);
            }
            ClearData();
        }
    }


    public partial class UI_CurrencyTitle
    {
        private uint SpecialCoinId = 5;
        List<uint> tipList = new List<uint>();
        private void SpecialInitUI()
        {
            for (int i = 0; i < datas.Count; i++)
            {
                tipList.Add(datas[i]);
            }
            tipList.Add(SpecialCoinId);
            mCountTextSpecial.text = Sys_Bag.Instance.GetItemCount(SpecialCoinId).ToString();
            ImageHelper.SetIcon(mIconImageSpecial, CSVItem.Instance.GetConfData(SpecialCoinId).small_icon_id);

        }
        private void OnmCurrencySpecialButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Property_Tips, false, tipList);
        }

        private void OnButtonLockClickedSpecial()
        {


        }

    }
}


