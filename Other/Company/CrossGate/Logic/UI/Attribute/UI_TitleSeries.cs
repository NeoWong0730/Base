using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using System;
using System.Text;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{
    public class UI_TitleSeries : UIBase
    {
        private Button closeBtn;
        private Button instructuon;
        private Button change;
        private Button collectButton;
        private Image collectImg;
        private Button infoButton;
        private Slider collectProgress;
        private GameObject collect_Fx;
        private Text collectProcessText;
        private Text seriesAttr;
        private TitleSeries curTitleSeries;
        private Transform seriesAttrParent;
        private GameObject seriesAttr_1;
        private GameObject seriesAttr_2;
        private Text point;

        private InfinityGridLayoutGroup infinity_left;
        private Transform infinityParent_left;
        private Dictionary<GameObject, TitleSeriesCeil> titleSeriesCeils = new Dictionary<GameObject, TitleSeriesCeil>();
        private int curtitleSeriesCeilIndex = 0;
        public int CurtitleSeriesCeilIndex
        {
            get { return curtitleSeriesCeilIndex; }
            set
            {
                if (curtitleSeriesCeilIndex != value)
                {
                    curtitleSeriesCeilIndex = value;
                    UpdateCurSelectTitleSeries();
                }
            }
        }

        private InfinityGridLayoutGroup infinity_right;
        private Transform infinityParent_right;
        private Dictionary<GameObject, TitleCeil> titleceils = new Dictionary<GameObject, TitleCeil>();
        private int curtitleSelectIndex = 0;
        private List<Title> titles = new List<Title>();

        private Transform titlePosParent;
        private Text curTotalSeriesPoint;

        private AsyncOperationHandle<GameObject> requestRef;
        private GameObject seriesGetEffect;
        private Transform seriesGetEffectParent;
        private Timer timer;


        protected override void OnLoaded()
        {
            seriesAttr = transform.Find("Animator/View_Right/Title01/View01/Text").GetComponent<Text>();
            infinityParent_left = transform.Find("Animator/View_Left/Scroll View01/Content");
            infinity_left = infinityParent_left.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity_left.minAmount = 6;
            infinity_left.updateChildrenCallback = UpdateChildrenCallback;
            for (int i = 0; i < infinityParent_left.childCount; i++)
            {
                GameObject go = infinityParent_left.GetChild(i).gameObject;
                TitleSeriesCeil titleSeriesCeil = new TitleSeriesCeil();
                titleSeriesCeil.BindGameObject(go);
                titleSeriesCeil.AddClickListener(OnTitleSeriesCeilSelected);
                titleSeriesCeils.Add(go, titleSeriesCeil);
            }

            infinityParent_right = transform.Find("Animator/View_Right/View_Content/Grid");
            infinity_right = infinityParent_right.gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            infinity_right.minAmount = 12;
            infinity_right.updateChildrenCallback = UpdateChildrenCallback1;
            for (int i = 0; i < infinityParent_right.childCount; i++)
            {
                GameObject go = infinityParent_right.GetChild(i).gameObject;
                TitleCeil titleCeil = new TitleCeil();
                titleCeil.BindGameObject(go);
                titleCeil.AddClickListener(OnTitleCeilSelected, OnTitleCeilLongPressed);
                titleceils.Add(go, titleCeil);
            }
            closeBtn = transform.Find("Animator/View_TipsBg03_Big/Btn_Close").GetComponent<Button>();
            change = transform.Find("Animator/View_Right/Title01/Btn01").GetComponent<Button>();
            infoButton = transform.Find("Animator/Btn_Rule").GetComponent<Button>();
            collectButton = transform.Find("Animator/View_Right/View_Collect/Btn_Collect").GetComponent<Button>();
            collectImg = collectButton.GetComponent<Image>();
            seriesGetEffectParent = transform.Find("Animator/View_Right/Title01/Fx");
            seriesAttrParent = transform.Find("Animator/View_Right/Title01/View02/Grid");
            seriesAttr_1 = transform.Find("Animator/View_Right/Title01/View01").gameObject;
            seriesAttr_2 = transform.Find("Animator/View_Right/Title01/View02").gameObject;
            point = transform.Find("Animator/View_Right/Title01/View02/Text_Point").GetComponent<Text>();
            titlePosParent = transform.Find("Animator/View_Down/Grid");
            curTotalSeriesPoint = transform.Find("Animator/View_Down/Text_Point").GetComponent<Text>();
            collectProcessText = transform.Find("Animator/View_Right/View_Collect/Text_Percent").GetComponent<Text>();
            collectProgress = transform.Find("Animator/View_Right/View_Collect/Slider").GetComponent<Slider>();
            collect_Fx = collectButton.transform.Find("Fx_ui_item").gameObject;
            RegistEvent();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnUpdateTitleSeriesPos, UpdateTitlePos, toRegister);
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnUpdateSeriesCollectProgress, UpdateCollectProgress, toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnNewRewardAvaliable, OnUpdateReward, toRegister);
            Sys_Title.Instance.eventEmitter.Handle(Sys_Title.EEvents.OnRemoveNewFlagForTitlePosChange, OnRefreshTitleSeriesCeil, toRegister);
        }

        protected override void OnShow()
        {
            infinity_left.SetAmount(Sys_Title.Instance.titleSeries.Count);
            UpdateCurSelectTitleSeries();
            UpdateTitlePos();
        }

        protected override void OnHide()
        {
            TitleSuitFirstCollectReq();

            Dictionary<GameObject, TitleCeil>.Enumerator enumerator = titleceils.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TitleCeil TitleCeil = enumerator.Current.Value;
                TitleCeil?.OnDispose();
            }

            AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetsLoaded);
        }

        private void TitleSuitFirstCollectReq()
        {
            bool needSort = false;
            for (int i = 0, count = Sys_Title.Instance.titleSeries.Count; i < count; i++)
            {
                TitleSeries titleSeries = Sys_Title.Instance.titleSeries[i];
                if (titleSeries.IsFirstactive)
                {
                    Sys_Title.Instance.TitleSuitFirstCollectReq(titleSeries.Id);
                    titleSeries.IsFirstactive = false;
                    needSort = true;
                }
            }
            if (needSort)
            {
                Sys_Title.Instance.SortTitleSeries();
            }
        }

        private void RegistEvent()
        {
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_TitleSeries);
            });

            change.onClick.AddListener(() =>
            {
                if (!curTitleSeries.active)
                {
                    string content = CSVLanguage.Instance.GetConfData(2020742).words;
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                UIManager.OpenUI(EUIID.UI_TitleSeriesSelect, false, curTitleSeries);
            });

            collectButton.onClick.AddListener(() =>
            {
                if (curTitleSeries.bNewAwardAvaliable)
                {
                    Sys_Title.Instance.GetRewardReq(curTitleSeries.Id);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_TitleAward, false, curTitleSeries);
                }
            });
            infoButton.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(2020746) });
            });
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            TitleSeriesCeil titleSeriesCeil = titleSeriesCeils[trans.gameObject];
            titleSeriesCeil.SetData(Sys_Title.Instance.titleSeries[index], index);
            if (index != curtitleSeriesCeilIndex)
            {
                titleSeriesCeil.Release();
            }
            else
            {
                titleSeriesCeil.Select();
            }
        }

        private void UpdateChildrenCallback1(int index, Transform trans)
        {
            TitleCeil titleCeil = titleceils[trans.gameObject];
            titleCeil.SetData(titles[index], index);
            if (index != curtitleSelectIndex)
            {
                titleCeil.Release();
            }
            else
            {
                titleCeil.Select();
            }
        }

        private void OnTitleSeriesCeilSelected(TitleSeriesCeil titleSeriesCeil)
        {
            CurtitleSeriesCeilIndex = titleSeriesCeil.dataIndex;
            OnSelectTitleSeries();
        }

        private void UpdateCurSelectTitleSeries()
        {
            curTitleSeries = Sys_Title.Instance.titleSeries[curtitleSeriesCeilIndex];
            titles = curTitleSeries.titles;
            infinity_right.SetAmount(titles.Count);

            if (!curTitleSeries.active)
            {
                seriesAttr_1.SetActive(true);
                seriesAttr_2.SetActive(false);
            }
            else
            {
                seriesAttr_1.SetActive(false);
                seriesAttr_2.SetActive(true);
                int attrCount = curTitleSeries.cSVTitleSeriesData.seriesProperty.Count;
                FrameworkTool.CreateChildList(seriesAttrParent, attrCount);
                for (int i = 0; i < attrCount; i++)
                {
                    Transform child = seriesAttrParent.GetChild(i);
                    Text attrName = child.GetComponent<Text>();
                    Text num = child.Find("Text").GetComponent<Text>();
                    uint attrid = curTitleSeries.cSVTitleSeriesData.seriesProperty[i][0];
                    uint attrnum = curTitleSeries.cSVTitleSeriesData.seriesProperty[i][1];
                    //TextHelper.SetText(attrName, CSVAttr.Instance.GetConfData(attrid).name);
                    //num.text = "+" + attrnum.ToString();
                    SetAttr(attrid, attrnum, attrName, num);
                }
            }
            point.text = curTitleSeries.cSVTitleSeriesData.seriesPoint.ToString();
            OnUpdateReward(curTitleSeries.Id);
            UpdateCollectProgress();
            if (curTitleSeries.IsFirstactive && !curTitleSeries.bPerformed)
            {
                CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(2);
                if (cSVSystemEffectData != null)
                {
                    LoadTitleEffectAssetAsyn(cSVSystemEffectData.FxPath);
                    curTitleSeries.bPerformed = true;
                }
            }
        }

        private void UpdateCollectProgress()
        {
            collectProcessText.text = curTitleSeries.activeCount.ToString() + "/" + curTitleSeries.maxActiveCount.ToString();
            collectProgress.value = curTitleSeries.progress;
        }

        private void OnUpdateReward(uint suitId)
        {
            if (suitId != curTitleSeries.Id)
                return;
            collect_Fx.SetActive(curTitleSeries.bNewAwardAvaliable);
            if (!curTitleSeries.bAllAwardGet)//可领取
            {
                ImageHelper.SetIcon(collectImg, 993101);//关闭
            }
            else
            {
                ImageHelper.SetIcon(collectImg, 993102);//打开
            }
        }

        //更新称号栏位
        private void UpdateTitlePos()
        {
            int titlePosCount = Sys_Title.Instance.TotalTitlePos;
            FrameworkTool.CreateChildList(titlePosParent, titlePosCount);
            for (int i = 0; i < titlePosCount; i++)
            {
                Transform child = titlePosParent.GetChild(i);
                GameObject go1 = child.Find("Title01").gameObject;
                GameObject go2 = child.Find("Title02").gameObject;

                bool unlock = Sys_Title.Instance.titlePos.Count >= i + 1;

                if (!unlock)
                {
                    go1.SetActive(false);
                    go2.SetActive(true);
                    SetLMText(go2.GetComponent<Text>(), i);
                    int needActiveCount = CSVTitleColumn.Instance.GetConfData((uint)(i + 1)).columnCondition;
                    string content = string.Format(CSVLanguage.Instance.GetConfData(2020732).words, needActiveCount.ToString(),
                        Sys_Title.Instance.GetActiveTitleSeriesCount().ToString(), needActiveCount.ToString());
                    go2.transform.Find("Text_Condition").GetComponent<Text>().text = content;
                }
                else
                {
                    Text seriesName = go1.transform.Find("Text_Series").GetComponent<Text>();
                    Text attrName = go1.transform.Find("Text_Series/Text_Property").GetComponent<Text>();
                    Text attrNum = go1.transform.Find("Text_Series/Text_Property/Text").GetComponent<Text>();

                    if (Sys_Title.Instance.titlePos[i] == 0)
                    {
                        go1.SetActive(true);
                        go2.SetActive(false);
                        SetLMText(go1.GetComponent<Text>(), i);
                        seriesName.gameObject.SetActive(true);
                        attrName.gameObject.SetActive(false);
                        attrNum.gameObject.SetActive(false);
                        TextHelper.SetText(seriesName, 2020731);
                    }
                    else
                    {
                        go1.SetActive(true);
                        go2.SetActive(false);
                        SetLMText(go1.GetComponent<Text>(), i);
                        seriesName.gameObject.SetActive(true);
                        attrName.gameObject.SetActive(true);
                        attrNum.gameObject.SetActive(true);
                        uint seriesId = Sys_Title.Instance.titlePos[i];
                        CSVTitleSeries.Data cSVTitleSeriesData = CSVTitleSeries.Instance.GetConfData(seriesId);
                        TextHelper.SetText(seriesName, cSVTitleSeriesData.seriesLan);
                        //TextHelper.SetText(attrName, CSVAttr.Instance.GetConfData(cSVTitleSeriesData.seriesProperty[0][0]).name);
                        SetAttr(cSVTitleSeriesData.seriesProperty[0][0], cSVTitleSeriesData.seriesProperty[0][1], attrName, attrNum);
                        //attrNum.text = "+"+cSVTitleSeriesData.seriesProperty[0][1].ToString();
                    }
                }
            }

            int totalSeriesPoint = 0;
            foreach (var item in Sys_Title.Instance.titlePos)
            {
                if (item == 0)
                    continue;
                CSVTitleSeries.Data cSVTitleSeriesData = CSVTitleSeries.Instance.GetConfData(item);
                if (cSVTitleSeriesData != null)
                {
                    totalSeriesPoint += cSVTitleSeriesData.seriesPoint;
                }
            }
            curTotalSeriesPoint.text = totalSeriesPoint.ToString();
        }

        private void SetAttr(uint attr1, uint value1, Text attrName1, Text attrValue1)
        {
            CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
            TextHelper.SetText(attrName1, cSVAttrData.name);
            if (cSVAttrData.show_type == 1)
            {
                attrValue1.text = string.Format("+{0}", value1.ToString());
            }
            else
            {
                attrValue1.text = string.Format("+{0}%", (value1 / 100f).ToString());
            }
        }

        private void SetLMText(Text text, int i)
        {
            string str = "";
            if (i == 0)
            {
                str = CSVLanguage.Instance.GetConfData(2020791).words;
            }
            else if (i == 1)
            {
                str = CSVLanguage.Instance.GetConfData(2020792).words;
            }
            else if (i == 2)
            {
                str = CSVLanguage.Instance.GetConfData(2020793).words;
            }
            else if (i == 3)
            {
                str = CSVLanguage.Instance.GetConfData(2020794).words;
            }
            else if (i == 4)
            {
                str = CSVLanguage.Instance.GetConfData(2020795).words;
            }
            text.text = str;
        }

        private void OnSelectTitleSeries()
        {
            foreach (var item in titleSeriesCeils)
            {
                if (item.Value.dataIndex != curtitleSeriesCeilIndex)
                {
                    item.Value.Release();
                }
                else
                {
                    item.Value.Select();
                }
            }
        }

        private void OnTitleCeilSelected(TitleCeil titleCeil)
        {
            curtitleSelectIndex = titleCeil.dataIndex;
            OnSelectTitle();
        }

        private void OnTitleCeilLongPressed(TitleCeil titleCeil)
        {
            UIManager.OpenUI(EUIID.UI_TitleTips, false, titleCeil.title);
        }

        private void OnSelectTitle()
        {
            foreach (var item in titleceils)
            {
                if (item.Value.dataIndex != curtitleSelectIndex)
                {
                    item.Value.Release();
                }
                else
                {
                    item.Value.Select();
                }
            }
        }

        private void OnRefreshTitleSeriesCeil()
        {
            foreach (var item in titleSeriesCeils.Values)
            {
                if (item.titleSeries != null)
                {
                    item.Refresh();
                }
            }
        }

        private void LoadTitleEffectAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetsLoaded);
        }

        private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
        {
            seriesGetEffect = handle.Result;
            if (null != seriesGetEffect)
            {
                seriesGetEffect.transform.SetParent(seriesGetEffectParent);
                RectTransform rectTransform = seriesGetEffect.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;

                timer?.Cancel();
                timer = Timer.Register(1f, () =>
                {
                    GameObject.Destroy(seriesGetEffect);
                    seriesGetEffect = null;
                });
            }
        }


        public class TitleSeriesCeil
        {
            private Transform transform;
            private GameObject imageSelect1;
            private GameObject imageSelect;
            private GameObject lockGo;
            private GameObject selectlockGo;
            private GameObject newGo;
            private GameObject EquipObj;
            private GameObject SelectEquipObj;
            private Text Equip;
            private Text SelectEquip;
            private Text name;
            private Text nameSelect;
            private Image eventBg;
            private Action<TitleSeriesCeil> onClick;
            private Action<TitleSeriesCeil> onlongPressed;
            public TitleSeries titleSeries;
            public int dataIndex;
            private bool bSelect;

            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                ParseComponent();
            }

            public void SetData(TitleSeries _titleSeries, int _dataIndex)
            {
                this.titleSeries = _titleSeries;
                this.dataIndex = _dataIndex;
                Refresh();
            }

            private void ParseComponent()
            {
                name = transform.Find("Text").GetComponent<Text>();
                nameSelect = transform.Find("Text_Select").GetComponent<Text>();
                imageSelect1 = transform.Find("Image_Select (1)").gameObject;
                imageSelect = transform.Find("Image_Select").gameObject;
                lockGo = transform.Find("Image_Lock").gameObject;
                selectlockGo = transform.Find("Image_Lock_Select").gameObject;
                newGo = transform.Find("Image_New").gameObject;
                EquipObj = transform.Find("Image_Equip").gameObject;
                SelectEquipObj = transform.Find("Image_Equip_Select").gameObject;
                Equip = transform.Find("Image_Equip/Text_Num").GetComponent<Text>();
                SelectEquip = transform.Find("Image_Equip_Select/Text_Num").GetComponent<Text>();
                eventBg = transform.GetComponent<Image>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void Refresh()
            {
                TextHelper.SetText(name, titleSeries.cSVTitleSeriesData.seriesLan);
                TextHelper.SetText(nameSelect, titleSeries.cSVTitleSeriesData.seriesLan);
                lockGo.SetActive(false);
                selectlockGo.SetActive(false);
                newGo.SetActive(titleSeries.IsFirstactive);
                int index = Sys_Title.Instance.titlePos.IndexOf(titleSeries.Id);
                EquipObj.SetActive(index != -1);
                SelectEquipObj.SetActive(index != -1);

                if (bSelect)
                {
                    if (!titleSeries.active)
                    {
                        lockGo.SetActive(false);
                        selectlockGo.SetActive(true);
                    }
                    if (index != -1)
                    {
                        EquipObj.SetActive(false);
                        SelectEquipObj.SetActive(true);
                        SetLMText(SelectEquip, index);
                    }
                }
                else
                {
                    if (!titleSeries.active)
                    {
                        lockGo.SetActive(true);
                        selectlockGo.SetActive(false);
                    }
                    if (index != -1)
                    {
                        EquipObj.SetActive(true);
                        SelectEquipObj.SetActive(false);
                        SetLMText(Equip, index);
                    }
                }

            }

            private void SetLMText(Text text, int i)
            {
                string str = string.Empty;
                if (i == 0)
                {
                    str = CSVLanguage.Instance.GetConfData(2020791).words;
                }
                else if (i == 1)
                {
                    str = CSVLanguage.Instance.GetConfData(2020792).words;
                }
                else if (i == 2)
                {
                    str = CSVLanguage.Instance.GetConfData(2020793).words;
                }
                else if (i == 3)
                {
                    str = CSVLanguage.Instance.GetConfData(2020794).words;
                }
                else if (i == 4)
                {
                    str = CSVLanguage.Instance.GetConfData(2020795).words;
                }
                text.text = str;
            }

            public void AddClickListener(Action<TitleSeriesCeil> _onClick, Action<TitleSeriesCeil> onlongPressed = null)
            {
                onClick = _onClick;
                if (onlongPressed != null)
                {
                    this.onlongPressed = onlongPressed;
                    UI_LongPressButton uI_LongPressButton = eventBg.gameObject.AddComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }

            private void OnLongPressed()
            {
                onlongPressed.Invoke(this);
            }

            public void Release()
            {
                bSelect = false;
                imageSelect1.SetActive(false);
                imageSelect.SetActive(false);
                nameSelect.gameObject.SetActive(false);
                if (titleSeries == null)
                    return;

                int index = Sys_Title.Instance.titlePos.IndexOf(titleSeries.Id);
                EquipObj.SetActive(index != -1);
                SelectEquipObj.SetActive(index != -1);

                if (!titleSeries.active)
                {
                    lockGo.SetActive(true);
                    selectlockGo.SetActive(false);
                }

                if (index != -1)
                {
                    EquipObj.SetActive(true);
                    SelectEquipObj.SetActive(false);
                    SetLMText(Equip, index);
                }
            }

            public void Select()
            {
                bSelect = true;
                imageSelect1.SetActive(true);
                imageSelect.SetActive(true);
                nameSelect.gameObject.SetActive(true);
                if (titleSeries == null)
                    return;

                int index = Sys_Title.Instance.titlePos.IndexOf(titleSeries.Id);
                EquipObj.SetActive(index != -1);
                SelectEquipObj.SetActive(index != -1);

                if (!titleSeries.active)
                {
                    lockGo.SetActive(false);
                    selectlockGo.SetActive(true);
                }

                if (index != -1)
                {
                    EquipObj.SetActive(false);
                    SelectEquipObj.SetActive(true);
                    SetLMText(SelectEquip, index);
                }
            }
        }


        public class TitleCeil
        {
            public Title title;
            private Transform transform;
            private GameObject select;
            private GameObject lockObj;
            private GameObject GetObj;
            private Image eventBg;
            public int dataIndex;
            private Action<TitleCeil> onClick;
            private Action<TitleCeil> onlongPressed;

            private Text mTitle_text1;

            private Text mTitle_text2;
            private Image mTitle_img2;

            private Image mTitle_img3;
            private Transform mTitle_Fx3parent;

            AsyncOperationHandle<GameObject> requestRef;
            private GameObject titleEffect;


            public void BindGameObject(GameObject go)
            {
                transform = go.transform;
                ParseComponent();
            }

            private void ParseComponent()
            {
                lockObj = transform.Find("State/Lock").gameObject;
                GetObj = transform.Find("State/Get").gameObject;
                eventBg = transform.GetComponent<Image>();

                mTitle_text1 = transform.Find("Title/Text").GetComponent<Text>();
                mTitle_text2 = transform.Find("Title/Image1/Text").GetComponent<Text>();
                mTitle_img2 = transform.Find("Title/Image1").GetComponent<Image>();
                mTitle_img3 = transform.Find("Title/Image2").GetComponent<Image>();
                mTitle_Fx3parent = transform.Find("Title/Image2/Fx");

                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
            }

            public void AddClickListener(Action<TitleCeil> _onClick, Action<TitleCeil> onlongPressed = null)
            {
                onClick = _onClick;
                if (onlongPressed != null)
                {
                    this.onlongPressed = onlongPressed;
                    UI_LongPressButton uI_LongPressButton = eventBg.gameObject.AddComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                onClick.Invoke(this);
            }

            private void OnLongPressed()
            {
                onlongPressed.Invoke(this);
            }

            public void SetData(Title _title, int _dataIndex)
            {
                this.title = _title;
                this.dataIndex = _dataIndex;
                Refresh();
            }

            private void Refresh()
            {
                lockObj.SetActive(!title.active);
                GetObj.SetActive(title.active);
                UpdateTitle();
            }

            public void UpdateTitle()
            {
                CSVTitle.Data cSVTitleData = title.cSVTitleData;
                if (cSVTitleData != null)
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

            private void LoadTitleEffectAssetAsyn(string path)
            {
                AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetsLoaded);
            }

            private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
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

            public void OnDispose()
            {
                AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetsLoaded);
            }

            public void Release()
            {

            }

            public void Select()
            {

            }
        }
    }
}


