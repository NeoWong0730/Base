using Framework;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Table;
using Packet;
using Lib.Core;
using UnityEngine.EventSystems;

namespace Logic
{
    public partial class UI_Lotto_Layout
    {
        private IListener m_Listener;

        private Button m_BtnRule;

        private Transform m_TransMessage;
        private Button m_BtnOnce;
        private Button m_BtnFivth;
        private Text m_TexCount;

        private Image m_coinImage;
        private Text m_CoinCount;
        private Button m_BtnAddCoin;

        private Button m_BtnRewardDetail;

        private Text m_TexOnceCost;
        private Text m_TexFivthCost;
        private Text m_TexTips;
        private Text m_TexLeftNum;


        private Button m_BtnClose;
        private Button m_BtnDetail;
        private Button m_BtnBack;

        private RawImage m_RImgShowScene;


        private Button m_BtnCostOnceIcon;
        private Button m_BtnCostFiveIcon;
        private Button m_BtnHadIcon;


        private Transform m_TransTitle;

        protected PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

        private ShowSceneControl m_ShowSceneControl;

        private AssetDependencies m_AssetDependencies;

        private Toggle m_TogBig;
        private Toggle m_TogNormal;
        private Toggle m_TogAutoCompletement;


        /// <summary>
        /// ////////////////////////////////////////////
        /// </summary>
        private GameObject lottoRewardGo;
        private GameObject oneRewardGo;
        private GameObject fiveRewardGo;

        private GameObject showSceneGo;

        private Text oneCost;
        private Text fiveCost;

        private GameObject[] rewardGos = new GameObject[3];

        private ClickItemGroup<RectRewardItem> oneRewardGroup = new ClickItemGroup<RectRewardItem>() { AutoClone = false };
        private ClickItemGroup<RectRewardItem> fiveReardGroup = new ClickItemGroup<RectRewardItem>() { AutoClone = false };

        private Button btnOnce;
        private Button btnFive;

        private Button btnClose;

        private GameObject extraRewardGo;
        RewardExtra rewardExtra = new RewardExtra();

        private uint totalTicketNum;
        public void Load(Transform root)
        {
            m_TransTitle = root.Find("Animator/View_Title07");
            m_TransMessage = root.Find("Animator/View_Message");

            rewardGos[0] = root.Find("Animator/View_BG").gameObject;
            rewardGos[1] = m_TransTitle.gameObject;
            rewardGos[2] = m_TransMessage.gameObject;

            m_BtnRewardDetail = m_TransMessage.Find("Btn_Details").GetComponent<Button>();

            m_BtnOnce = m_TransMessage.Find("View_Btn01/Btn_Once").GetComponent<Button>();

            m_BtnFivth = m_TransMessage.Find("View_Btn01/Btn_Many").GetComponent<Button>();

            m_TexOnceCost = m_TransMessage.Find("Cost01/Text_Number").GetComponent<Text>();
            m_TexFivthCost = m_TransMessage.Find("Cost02/Text_Number").GetComponent<Text>();

            m_TexCount = m_TransMessage.Find("Image_Number/Text_Number").GetComponent<Text>();

            m_coinImage = m_TransMessage.Find("Image_Number2/Image_Icon").GetComponent<Image>();
            m_CoinCount = m_TransMessage.Find("Image_Number2/Text_Number").GetComponent<Text>();


            m_TexTips = m_TransMessage.Find("BG2/Text_Tip").GetComponent<Text>();

            m_TexLeftNum = m_TransMessage.Find("BG2/Text_Num").GetComponent<Text>();


            m_BtnClose = root.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();

            m_BtnDetail = root.Find("Animator/View_Title07/Btn_Detail").GetComponent<Button>();

          

            m_RImgShowScene = root.Find("Animator/ShowSceneImage").GetComponent<RawImage>();

            m_BtnCostOnceIcon = m_TransMessage.Find("Cost01/Image_Icon").GetComponent<Button>();
            m_BtnCostFiveIcon = m_TransMessage.Find("Cost02/Image_Icon").GetComponent<Button>();
            m_BtnHadIcon = m_TransMessage.Find("Image_Number/Image_Icon").GetComponent<Button>();
            m_BtnAddCoin = m_TransMessage.Find("Image_Number2/Button_Add").GetComponent<Button>();

            m_BtnRule = root.Find("Animator/View_Title07/Btn_Detail").GetComponent<Button>();

            m_BtnBack = root.Find("Animator/Btn_Skip").GetComponent<Button>();


            m_AssetDependencies = m_RImgShowScene.GetComponent<AssetDependencies>();

            m_TogBig = m_TransMessage.Find("Menu/ListItem").GetComponent<Toggle>();
            m_TogNormal = m_TransMessage.Find("Menu/ListItem (1)").GetComponent<Toggle>();
            m_TogAutoCompletement = m_TransMessage.Find("Toggle").GetComponent<Toggle>();
        }

        public void LoadLottoResult(Transform root)
        {
            lottoRewardGo = root.Find("Animator/result").gameObject;
            oneRewardGo = root.Find("Animator/result/View_Once").gameObject;
            var onceItemTrans = oneRewardGo.transform.Find("item");
            oneRewardGroup.AddChild(onceItemTrans);
            fiveRewardGo = root.Find("Animator/result/View_Five").gameObject;
            extraRewardGo = root.Find("Animator/result/View_Extra").gameObject;
            rewardExtra.Load(extraRewardGo.transform);

            int count = fiveRewardGo.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                string itemName = "Item0" + (i + 1).ToString();

                var item = fiveRewardGo.transform.Find(itemName);

                if (item != null)
                    fiveReardGroup.AddChild(item);
            }

            btnOnce = root.Find("Animator/result/View_Buttons/Btn_Once").GetComponent<Button>();
            btnFive = root.Find("Animator/result/View_Buttons/Btn_Many").GetComponent<Button>();
            btnClose = root.Find("Animator/result/GameObject").GetComponent<Button>();
            oneCost = root.Find("Animator/result/View_Buttons/Btn_Once/Cost01/Text_Number").GetComponent<Text>();
            fiveCost = root.Find("Animator/result/View_Buttons/Btn_Many/Cost02/Text_Number").GetComponent<Text>();
            m_BtnBack.gameObject.SetActive(false);
        }

        public void SetListener(IListener listener)
        {
            m_BtnRewardDetail.onClick.AddListener(listener.OnClickReawrdDetail);
            m_BtnOnce.onClick.AddListener(listener.OnClickOnce);

            m_BtnFivth.onClick.AddListener(listener.OnClickMany);

            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnDetail.onClick.AddListener(listener.OnClickDetail);

            m_BtnCostOnceIcon.onClick.AddListener(listener.OnClickLottoIcon);
            m_BtnCostFiveIcon.onClick.AddListener(listener.OnClickLottoIcon);
            m_BtnHadIcon.onClick.AddListener(listener.OnClickLottoIcon);

            m_BtnRule.onClick.AddListener(listener.OnClickRule);

            m_BtnBack.onClick.AddListener(listener.OnClickSkip);

            m_TogBig.onValueChanged.AddListener(listener.OnBigLottoState);
            m_TogNormal.onValueChanged.AddListener(listener.OnNormalLottoState);
            m_BtnAddCoin.onClick.AddListener(listener.OnClickAddCoin);
            m_TogAutoCompletement.onValueChanged.AddListener(listener.OnAutoCompletementState);

            m_Listener = listener;
        }

        public void SetResultListener()
        {
            btnOnce.onClick.AddListener(m_Listener.OnClickOnce);
            btnFive.onClick.AddListener(m_Listener.OnClickMany);
            btnClose.onClick.AddListener(m_Listener.OnClickAwardResultClose);
        }


        public void SetMessageActive(bool b)
        {
            if (m_TransMessage.gameObject.activeSelf != b)
                m_TransMessage.gameObject.SetActive(b);

            if (m_BtnBack.gameObject.activeSelf == b)
                m_BtnBack.gameObject.SetActive(!b);

            if (m_TransTitle.gameObject.activeSelf != b)
                m_TransTitle.gameObject.SetActive(b);
        }

        public void SetCoinImage()
        {
            ImageHelper.SetIcon(m_coinImage, 992501);   //mobi iconid = 992501
        }

        public void SetShowSceneCameraActive(bool enabled)
        {
            m_ShowSceneControl.mCamera.gameObject.SetActive(enabled);
        }

        public void SetShowSceneActive(bool b)
        {
            if (showSceneGo != null && showSceneGo.activeSelf != b)
                showSceneGo.SetActive(b);

            m_RImgShowScene.gameObject.SetActive(false);
        }
        public void SetShowScene(ShowSceneControl sceneControl)
        {
            float width = m_RImgShowScene.rectTransform.rect.width;
            float height = m_RImgShowScene.rectTransform.rect.height;

            var rt = sceneControl.GetTemporary(0, 0, 0, RenderTextureFormat.RGB565, 1,false);

            m_RImgShowScene.texture = rt;
        }

        public void SetItemCount(long count)
        {
            m_TexCount.text = count.ToString();
        }

        public void SetCoinCount(string count)
        {
            m_CoinCount.text = count;
        }

        public void RefreshLottoTicketShow()
        {
            m_TexLeftNum.text = Sys_pub.Instance.CurrentLottoTimes.ToString() + "/" + Sys_pub.Instance.TotalLottoTimes.ToString();
            m_TexTips.text = LanguageHelper.GetTextContent(2021130, Sys_pub.Instance.TotalLottoTimes.ToString()); 
        }


        public void SetCostOnceTex(int num, bool isEnuge)
        {
            m_TexOnceCost.text = num.ToString();
            m_TexOnceCost.color = isEnuge ? new Color(0.545f, 0.3529f, 0.419f) : Color.red;
        }

        public void SetCostManyTex(int num, bool isEnuge)
        {
            m_TexFivthCost.text = num.ToString();

            m_TexFivthCost.color = isEnuge ? new Color(0.545f,0.3529f,0.419f) : Color.red;
        }

        public void SetResultOneTex(int num, bool isEnuge)
        {
            oneCost.text = num.ToString();
            oneCost.color = isEnuge ? new Color(0.545f, 0.3529f, 0.419f) : Color.red;
        }

        public void SetResultFiveTex(int num, bool isEnuge)
        {
            fiveCost.text = num.ToString();
            fiveCost.color = isEnuge ? new Color(0.545f, 0.3529f, 0.419f) : Color.red;
        }

        public bool IsAutoCompletementToggleCheck()
        {
            return m_TogAutoCompletement.isOn;
        }
        public void OnCloseLayout()
        {
           
        }

        public void ShowItemInfo(uint id)
        {
            m_ItemData.id = id;
          
            var boxEvt = new MessageBoxEvt(EUIID.UI_Lotto,m_ItemData);
            boxEvt.b_ForceShowScource = true;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }

        public void LoadShowScene()
        {
            if (m_ShowSceneControl != null)
                return;

            showSceneGo = m_AssetDependencies.mCustomDependencies[0] as GameObject;

            if (showSceneGo == null)
                return;

            showSceneGo = GameObject.Instantiate(showSceneGo);

            showSceneGo.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneGo.transform.localPosition = new Vector3(0, -20f, 0);

            var trans = showSceneGo.transform.Find("Floor");
            if(trans != null)
            {
                var planerReflections = trans.GetComponent<UnityEngine.Rendering.Universal.PlanarReflections>();
                if(planerReflections != null)
                    planerReflections.m_settings.m_ClipPlaneOffset = -20;
            }

            LoadScene(showSceneGo);

            m_ShowSceneControl = new ShowSceneControl();

            m_ShowSceneControl.Parse(showSceneGo);

            //SetShowScene(m_ShowSceneControl);
        }

        public void SetType(int type)
        {
            if (type == 0)
                m_TogBig.isOn = true;

            if (type == 1)
                m_TogNormal.isOn = true;
        }

        public void SetAutoCompletement(bool b)
        {
            m_TogAutoCompletement.isOn = b;
        }
    }

    public partial class UI_Lotto_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnClickOnce();
            void OnClickMany();

            void OnClickAwardResultClose();

            void OnClickReawrdDetail();

            void OnClickDetail();

            void OnOneAnimationEnd(PlayableDirector playableDirector);


            void OnFiveAnimationEnd(PlayableDirector playableDirector);


            //void OnCommonAnimationEnd(PlayableDirector playableDirector);

            void OnClickLottoIcon();

            void OnClickAddCoin();

            void OnClickRule();

            void OnClickSkip();

            void OnBigLottoState(bool b);

            void OnNormalLottoState(bool b);

            void OnAutoCompletementState(bool b);
        }
    }


    public partial class UI_Lotto_Layout
    {
        class RectRewardItem : IntClickItem
        {
            protected PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);
            public MessageBoxEvt boxEvt = new MessageBoxEvt();
            private Text itemNum;
            public override void Load(Transform root)
            {
                base.Load(root);

                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(root.Find("Image_BG").gameObject);
                itemNum = root.Find("Image_BG/Text").GetComponent<Text>();
                eventListener.ClearEvents();
                eventListener.AddEventListener(EventTriggerType.PointerClick, (ret) => { OnClick(); });
            }

            public void SetReward(uint id, uint count, uint quality)
            {
                itemData.id = id;
                itemData.count = count;

                if (quality == 5)
                {
                    itemNum.text = "";
                }
                else
                {
                    itemNum.text = count.ToString();
                }
                
                //DebugUtil.LogError("item world pos  = " + itemNum.gameObject.transform.position.ToString() + "screenPos = " + UIManager.mUICamera.WorldToScreenPoint(itemNum.gameObject.transform.position).ToString());

                SetData(new MessageBoxEvt(EUIID.UI_Lotto, itemData));
            }

            public void SetData(MessageBoxEvt _boxEvt)
            {
                SetData(_boxEvt.itemData, _boxEvt.sourceUiId);
            }

            public void SetData(PropIconLoader.ShowItemData itemData, EUIID uiId)
            {

                boxEvt.Reset(uiId, itemData);
            }

            private void OnClick()
            {
                if (itemData == null)
                    return;
                if (itemData.bUseClick)
                {
                    if (itemData.bUseTips)
                    {
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                    }

                    //itemData.onclick?.Invoke(this);
                }
            }
        }

        class RewardExtra:ClickItem
        {
            protected PropItem item;
            protected PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public override void Load(Transform root)
            {
                base.Load(root);

                var propItem = root.Find("PropItem");

                item = new PropItem();

                item.BindGameObject(propItem.gameObject);
            }

            public void SetReward(uint id, uint count)
            {
                itemData.id = id;
                itemData.count = count;

                item.SetData(new MessageBoxEvt(EUIID.UI_Lotto, itemData));

                var data = CSVItem.Instance.GetConfData(id);
                if(data != null)
                {
                    item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
                }
            }
        }
    }
    public partial class UI_Lotto_Layout
    {
        public enum LottoState
        {
            LottoMain,
            PlayTimeLine,
            LottoResult,
        }

        public enum CardFxState
        {
            NormalNormalLight,
            KingNormalLight,
            NormalGoldLight,
            KingGoldLight,
        }
        private PlayableDirector m_Once;
        private PlayableDirector m_Five;

        private Transform fx1Tran;
        private Transform[] fx5Trans = new Transform[5];

        private GameObject[] fxQualityGo = new GameObject[5];

        private GameObject[] mouseGo = new GameObject[2];

        private GameObject[] fxLottoNormalGo = new GameObject[2];
        private GameObject[] fxLottoGoldGo = new GameObject[2];

        private GameObject[] mouseCard = new GameObject[2];

        private SpriteRenderer oneSprite;
        private SpriteRenderer[] fiveSprite = new SpriteRenderer[5];

        LottoState currentState = LottoState.LottoMain;


        public void LoadScene(GameObject SceneGo)
        {

            m_Once = SceneGo.transform.Find("Timeline/Once").GetComponent<PlayableDirector>();
            m_Five = SceneGo.transform.Find("Timeline/Five").GetComponent<PlayableDirector>();

            m_Once.transform.gameObject.SetActive(true);
            m_Five.transform.gameObject.SetActive(true);

            fxQualityGo[0] = SceneGo.transform.Find("Fx/Fx_UI_Lottonew_blue").gameObject;
            fxQualityGo[1] = SceneGo.transform.Find("Fx/Fx_UI_Lottonew_green").gameObject;
            fxQualityGo[2] = SceneGo.transform.Find("Fx/Fx_UI_Lottonew_orange").gameObject;
            fxQualityGo[3] = SceneGo.transform.Find("Fx/Fx_UI_Lottonew_purple").gameObject;
            fxQualityGo[4] = SceneGo.transform.Find("Fx/Fx_UI_Lottonew_white").gameObject;

            fx1Tran = SceneGo.transform.Find("Fx1/Fx");
            for (int i = 0; i < 5; ++i)
            {
                var tran = SceneGo.transform.Find("Fx5/Fx5_" + (i + 1).ToString() + "/Fx" + (i + 1).ToString());
                fx5Trans[i] = tran;
            }

            mouseGo[0] = SceneGo.transform.Find("Pos/10006_show").gameObject;
            mouseGo[1] = SceneGo.transform.Find("Pos/97002_show").gameObject;

            fxLottoNormalGo[0] = SceneGo.transform.Find("Card/Fx_ka_chuxian/Fx_LottoGetShow_ka_chuxian_lan_01").gameObject;
            fxLottoNormalGo[1] = SceneGo.transform.Find("Card/Fx_ka_chuxian/Fx_LottoGetShow_ka_chuxian_lan_02").gameObject;

            fxLottoGoldGo[0] = SceneGo.transform.Find("Card/Fx_ka_chuxian/Fx_LottoGetShow_ka_chuxian_jin_01").gameObject;
            fxLottoGoldGo[1] = SceneGo.transform.Find("Card/Fx_ka_chuxian/Fx_LottoGetShow_ka_chuxian_jin_02").gameObject;

            mouseCard[0] = SceneGo.transform.Find("Card/Mouse_Card/97101").gameObject;
            mouseCard[1] = SceneGo.transform.Find("Card/Mouse_Card/97100").gameObject;

            oneSprite = SceneGo.transform.Find("Fx1/Item").GetComponent<SpriteRenderer>();

            for (int i = 0; i < 5; ++i)
            {
                fiveSprite[i] = SceneGo.transform.Find("Fx5/Fx5_" + (i + 1).ToString() + "/Item").GetComponent<SpriteRenderer>();
            }


            m_Once.stopped += m_Listener.OnOneAnimationEnd;
            m_Five.stopped += m_Listener.OnFiveAnimationEnd;
        }

        public void UnLoadScene()
        {
            m_Once = null;
            m_Five = null;

            if (m_ShowSceneControl != null)
                m_ShowSceneControl.Dispose();

            m_ShowSceneControl = null;
        }

        public void SetLottoRewardResult(LottoState state)
        {
            currentState = state;
            switch (state)
            {
                case LottoState.LottoMain:
                    for (int i = 0; i < rewardGos.Length; ++i)
                    {
                        rewardGos[i].SetActive(true);
                    }
                    lottoRewardGo.SetActive(false);
                    SetShowSceneActive(false);
                    break;
                case LottoState.PlayTimeLine:
                    for (int i = 0; i < rewardGos.Length; ++i)
                    {
                        rewardGos[i].SetActive(false);
                    }
                    lottoRewardGo.SetActive(false);
                    SetShowSceneActive(true);
                    break;
                case LottoState.LottoResult:
                    for (int i = 0; i < rewardGos.Length; ++i)
                    {
                        rewardGos[i].SetActive(false);
                    }
                    lottoRewardGo.SetActive(true);
                    SetShowSceneActive(true);
                    m_BtnBack.gameObject.SetActive(false);
                    break;
            }
        }

        public void SetCardAndModelMode(bool isNormal)
        {
            mouseCard[0].SetActive(!isNormal);
            mouseCard[1].SetActive(isNormal);
            mouseGo[0].SetActive(!isNormal);
            mouseGo[1].SetActive(isNormal);
        }

        public void SetCardFx(CardFxState state)
        {
            switch (state)
            {
                case CardFxState.NormalNormalLight:
                    fxLottoNormalGo[0].SetActive(true);
                    fxLottoNormalGo[1].SetActive(false);
                    fxLottoGoldGo[0].SetActive(false);
                    fxLottoGoldGo[1].SetActive(false);
                    break;
                case CardFxState.KingNormalLight:
                    fxLottoNormalGo[0].SetActive(false);
                    fxLottoNormalGo[1].SetActive(true);
                    fxLottoGoldGo[0].SetActive(false);
                    fxLottoGoldGo[1].SetActive(false);
                    break;
                case CardFxState.NormalGoldLight:
                    fxLottoNormalGo[0].SetActive(false);
                    fxLottoNormalGo[1].SetActive(false);
                    fxLottoGoldGo[0].SetActive(true);
                    fxLottoGoldGo[1].SetActive(false);
                    break;
                case CardFxState.KingGoldLight:
                    fxLottoNormalGo[0].SetActive(false);
                    fxLottoNormalGo[1].SetActive(false);
                    fxLottoGoldGo[0].SetActive(false);
                    fxLottoGoldGo[1].SetActive(true);
                    break;
            }
        }

        List<GameObject> tempGos = new List<GameObject>();
        public void SetOneCardFx(uint awardId)
        {
            var childCount = fx1Tran.childCount;
            if (childCount > 0)
            {
                tempGos.Clear();
                for (int i = 0; i < childCount; ++i)
                {
                    var child = fx1Tran.GetChild(i);
                    if (string.Compare(child.name, "Item", StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        tempGos.Add(child.gameObject);
                    }
                }
                for (int i = 0; i < tempGos.Count; ++i)
                {
                    GameObject.Destroy(tempGos[i]);
                }
            }
            var data = CSVAward.Instance.GetConfData(awardId);
            uint quality = data.quality;
            switch (quality)
            {
                case 5:
                    var go5 = GameObject.Instantiate(fxQualityGo[2]);
                    go5.transform.parent = fx1Tran;
                    go5.transform.localScale = new Vector3(1, 1, 1);
                    go5.transform.localPosition = new Vector3(0, 0, -2);
                    go5.SetActive(true);
                    break;
                case 4:
                    var go4 = GameObject.Instantiate(fxQualityGo[3]);
                    go4.transform.parent = fx1Tran;
                    go4.transform.localScale = new Vector3(1, 1, 1);
                    go4.transform.localPosition = new Vector3(0, 0, -2);
                    go4.SetActive(true);
                    break;
                case 3:
                    var go3 = GameObject.Instantiate(fxQualityGo[0]);
                    go3.transform.parent = fx1Tran;
                    go3.transform.localScale = new Vector3(1, 1, 1);
                    go3.transform.localPosition = new Vector3(0, 0, -2);
                    go3.SetActive(true);
                    break;
                case 2:
                    var go2 = GameObject.Instantiate(fxQualityGo[1]);
                    go2.transform.parent = fx1Tran;
                    go2.transform.localScale = new Vector3(1, 1, 1);
                    go2.transform.localPosition = new Vector3(0, 0, -2);
                    go2.SetActive(true);
                    break;

            }

            if (data != null)
            {
                if (data.quality == 5)
                {
                    var pet = CSVPetNew.Instance.GetConfData(data.itemId);
                    if (pet != null)
                    {
                        oneSprite.transform.localScale = new Vector3(0.1f, 0.1f, 1);
                        oneSprite.transform.localPosition = new Vector3(oneSprite.transform.localPosition.x, 0.0275f, -2f);
                        ImageHelper.SetSprite(oneSprite, pet.bust);
                        if (pet.bust == 0)
                        {
                            DebugUtil.LogError("Item icon id is 0 AwardId = " + awardId);
                        }
                    }
                }
                else
                {
                    var item = CSVItem.Instance.GetConfData(data.itemId);
                    oneSprite.transform.localScale = data.iconId != 0 ? new Vector3(0.085f, 0.085f, 1) : new Vector3(0.15f, 0.15f, 1);
                    oneSprite.transform.localPosition = new Vector3(oneSprite.transform.localPosition.x, 0.0275f, -2f);
                    ImageHelper.SetSprite(oneSprite, data.iconId!= 0 ?data.iconId :item.icon_id);
                    if (item.icon_id == 0)
                    {
                        DebugUtil.LogError("Item icon id is 0 AwardId = " + awardId);
                    }
                }
            }
            else
            {
                DebugUtil.LogError("Can't find data for awardId = " + awardId.ToString());
            }
        }

        public void SetFiveCardsFx(IList<uint> awardIds)
        {
#if UNITY_EDITOR
            if (awardIds.Count != 5)
                DebugUtil.LogError("qualities count is not 5");
#endif
            for (int k = 0; k < 5; ++k)
            {
                var tran = fx5Trans[k];
                var childCount = tran.childCount;
                if (childCount > 0)
                {
                    tempGos.Clear();
                    for (int i = 0; i < childCount; ++i)
                    {
                        var child = tran.GetChild(i);
                        if (string.Compare(child.name, "Item", StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            tempGos.Add(child.gameObject);
                        }
                    }
                    for (int i = 0; i < tempGos.Count; ++i)
                    {
                        GameObject.Destroy(tempGos[i]);
                    }
                }
            }


            for (int i = 0; i < 5; ++i)
            {
                var data = CSVAward.Instance.GetConfData(awardIds[i]);
                uint quality = data.quality;
                var tran = fx5Trans[i];
                switch (quality)
                {
                    case 5:
                        var go5 = GameObject.Instantiate(fxQualityGo[2]);
                        go5.transform.parent = tran;
                        go5.transform.localScale = new Vector3(1, 1, 1);
                        go5.transform.localPosition = new Vector3(0, 0, -2);
                        go5.SetActive(true);
                        break;
                    case 4:
                        var go4 = GameObject.Instantiate(fxQualityGo[3]);
                        go4.transform.parent = tran;
                        go4.transform.localScale = new Vector3(1, 1, 1);
                        go4.transform.localPosition = new Vector3(0, 0, -2);
                        go4.SetActive(true);
                        break;
                    case 3:
                        var go3 = GameObject.Instantiate(fxQualityGo[0]);
                        go3.transform.parent = tran;
                        go3.transform.localScale = new Vector3(1, 1, 1);
                        go3.transform.localPosition = new Vector3(0, 0, -2);
                        go3.SetActive(true);
                        break;
                    case 2:
                        var go2 = GameObject.Instantiate(fxQualityGo[1]);
                        go2.transform.parent = tran;
                        go2.transform.localScale = new Vector3(1, 1, 1);
                        go2.transform.localPosition = new Vector3(0, 0, -2);
                        go2.SetActive(true);
                        break;
                }

                if (data != null)
                {
                    if (data.quality == 5)
                    {
                        var pet = CSVPetNew.Instance.GetConfData(data.itemId);
                        if (pet != null)
                        {
                            fiveSprite[i].transform.localScale = new Vector3(0.1f, 0.1f, 1);
                            fiveSprite[i].transform.localPosition = new Vector3(fiveSprite[i].transform.localPosition.x, 0.0275f, -2f);
                            ImageHelper.SetSprite(fiveSprite[i], pet.bust);
                            if (pet.bust == 0)
                            {
                                DebugUtil.LogError("Item icon id is 0 AwardId = " + awardIds[i]);
                            }
                        }
                    }
                    else
                    {
                        var item = CSVItem.Instance.GetConfData(data.itemId);
                        fiveSprite[i].transform.localScale = data.iconId != 0 ? new Vector3(0.085f, 0.085f, 1) : new Vector3(0.15f, 0.15f, 1);
                        fiveSprite[i].transform.localPosition = new Vector3(fiveSprite[i].transform.localPosition.x, 0.01f, -2.8f);
                        ImageHelper.SetSprite(fiveSprite[i], data.iconId != 0? data.iconId : item.icon_id);
                        if (item.icon_id == 0)
                        {
                            DebugUtil.LogError("Item icon id is 0 AwardId = " + awardIds[i]);
                        }
                    }
                }
                else
                {
                    DebugUtil.LogError("Can't find data for awardId = " + awardIds[i].ToString());
                }
            }
        }

        public void SetOneReward(IList<uint> rewardIds)
        {
            oneRewardGo.SetActive(true);
            fiveRewardGo.SetActive(false);
            int count = rewardIds.Count;

            for (int i = 0; i < count; ++i)
            {
                var item = oneRewardGroup.getAt(i);

                if (item != null)
                {
                    var data = CSVAward.Instance.GetConfData(rewardIds[i]);
                    item.SetReward(data.itemId, data.itemNum,data.quality);
                    if (data != null)
                    {
                        if (data.quality == 5)
                        {
                            var pet = CSVPetNew.Instance.GetConfData(data.itemId);
                            if (pet != null)
                            {
                                oneSprite.transform.localScale = new Vector3(0.1f, 0.1f, 1);
                                oneSprite.transform.localPosition = new Vector3(0f, 0.0275f, -2f);
                            }
                        }
                        else
                        {
                            oneSprite.transform.localScale = data.iconId != 0 ? new Vector3(0.085f, 0.085f, 1) : new Vector3(0.15f, 0.15f, 1);
                            oneSprite.transform.localPosition = new Vector3(0f, 0.0275f, -2f);
                        }
                    }
                    else
                    {
                        DebugUtil.LogError("Can't find data for awardId = " + rewardIds[i].ToString());
                    }
                }
            }
        }

        //public void OutputTransformInfo(Transform tran)
        //{
        //    DebugUtil.LogError("-=================================================================");
        //    while (tran.parent != null)
        //    {
        //        tran = tran.parent;
        //        DebugUtil.LogError("tranName = " + tran.name + " world pos = " + tran.position.ToString() + " screen pos = " + UIManager.mUICamera.WorldToScreenPoint(tran.position).ToString());
        //    }
        //    DebugUtil.LogError("-=================================================================");
        //}

        public void SetFiveRewards(IList<uint> rewardIs)
        {
            fiveRewardGo.SetActive(true);
            oneRewardGo.SetActive(false);
            int count = rewardIs.Count;
            int childCount = fiveRewardGo.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                var childTran = fiveRewardGo.transform.GetChild(i);
                var pos = UIManager.mUICamera.WorldToScreenPoint(childTran.position);
                var pos1 = m_ShowSceneControl.mCamera.WorldToScreenPoint(fx5Trans[i].parent.GetChild(0).position);
                childTran.localPosition = new Vector3(childTran.localPosition.x + (pos1.x - pos.x) /(Screen.width / 1280.0f), childTran.localPosition.y, childTran.localPosition.z);
            }
            //OutputTransformInfo(fiveRewardGo.transform.GetChild(0).GetChild(0).GetChild(0));
            for (int i = 0; i < count; ++i)
            {
                var item = fiveReardGroup.getAt(i);

                if (item != null)
                {
                    var data = CSVAward.Instance.GetConfData(rewardIs[i]);
                    item.SetReward(data.itemId, data.itemNum, data.quality);
                    if (data != null)
                    {
                        if (data.quality == 5)
                        {
                            var pet = CSVPetNew.Instance.GetConfData(data.itemId);
                            if (pet != null)
                            {
                                fiveSprite[i].transform.localScale = new Vector3(0.1f, 0.1f, 1);
                                fiveSprite[i].transform.localPosition = new Vector3((i-2) * 0.108f, 0.0275f, -2f);
                            }
                        }
                        else
                        {
                            fiveSprite[i].transform.localScale = data.iconId != 0 ? new Vector3(0.085f, 0.085f, 1) : new Vector3(0.15f, 0.15f, 1);
                            fiveSprite[i].transform.localPosition = new Vector3((i-2) * 0.108f, 0.0275f, -2f);
                        }
                    }
                    else
                    {
                        DebugUtil.LogError("Can't find data for awardId = " + rewardIs[i].ToString());
                    }
                }
            }
        }

        public void SetRewardExtra(uint id, uint count)
        {
            if (count == 0 && extraRewardGo.activeSelf)
            {
                extraRewardGo.SetActive(false);
                return;
            }

            if (extraRewardGo.activeSelf == false)
                extraRewardGo.SetActive(true);

            rewardExtra.SetReward(id, count);
        }

        public void PushTips(uint itemId, uint count, bool isBroadCast)
        {
            var item = CSVItem.Instance.GetConfData(itemId);

            string name = LanguageHelper.GetTextContent(item.name_id);
            string hitContent = string.Format(LanguageHelper.GetTextContent(1000935),Constants.gHintColors_Items[item.quality-1],name,count.ToString());
            string chatContent = string.Format(LanguageHelper.GetTextContent(1000935),Constants.gChatColors_Item[item.quality-1], name, count.ToString());

            Sys_Hint.Instance.PushContent_GetReward(hitContent, item.id);

            if (isBroadCast)
                return;

            string chatStr = string.Format("[@{0}]{1}", Sys_Role.Instance.sRoleName, chatContent);

            Sys_Chat.Instance.PushMessage(ChatType.Person, null, chatStr);
        }

      
        public void PlayOpen1()
        {
            if (m_Once != null)
                m_Once.Play();
        }

    
        public void PlayOpen5()
        {
            if (m_Five != null)
                m_Five.Play();
        }
      
        public void PlayOpen1ToEnd()
        {
            if (m_Once != null)
            {
                m_Once.time = m_Once.duration;
                m_Once.Play();
            }
        }

        public void PlayOpen5ToEnd()
        {
            if(m_Five != null)
            {
                m_Five.time = m_Five.duration;
                m_Five.Play();
            }
        }

    }
}
