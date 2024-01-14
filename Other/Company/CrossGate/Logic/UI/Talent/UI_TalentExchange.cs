using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TalentExchange : UIBase {
        public class Tab : UIComponent {
            public GameObject lianhuaingGo;
            public Text remainTimeText;
            public Slider slider;

            public GameObject readyLianhuaGo;

            public CSVTalentExchange.Data csv;
            public long endTime;

            // 是否正在炼化
            public bool IsLianhuaing {
                get {
                    long now = Sys_Time.Instance.GetServerTime();
                    bool ret = (this.endTime - this.csv.time) < now;
                    ret &= (now < this.endTime);
                    return ret;
                }
            }
            // 是否等待炼化
            public bool IsReadyLianhua {
                get {
                    long now = Sys_Time.Instance.GetServerTime();
                    return this.endTime - this.csv.time > now;
                }
            }
            // 剩余炼化时间
            public long remainTime {
                get {
                    long now = Sys_Time.Instance.GetServerTime();
                    return this.endTime - now;
                }
            }

            protected override void Loaded() {
                this.lianhuaingGo = this.transform.Find("LianHuaing").gameObject;
                this.readyLianhuaGo = this.transform.Find("ReadyLianHua").gameObject;

                this.remainTimeText = this.transform.Find("LianHuaing/Text_Time").GetComponent<Text>();
                this.slider = this.transform.Find("LianHuaing/Slider_Eg").GetComponent<Slider>();
            }

            public void Refresh(int index) {
                this.lianhuaingGo.SetActive(false);
                this.readyLianhuaGo.SetActive(false);

                int count = Sys_Talent.Instance.lianhuaTimeStamps.Count;
                if (0 <= index && index < count) {
                    this.endTime = Sys_Talent.Instance.lianhuaTimeStamps[index];
                    uint id = (uint)(Sys_Talent.Instance.nextLianhuaId - count + index);
                    this.csv = CSVTalentExchange.Instance.GetConfData(id);
                    if (this.csv != null) {
                        bool isLianhuaing = this.IsLianhuaing;
                        this.lianhuaingGo.SetActive(isLianhuaing);
                        this.readyLianhuaGo.SetActive(this.IsReadyLianhua);
                        if (isLianhuaing) {
                            long rt = this.remainTime;
                            if (rt > 0) {
                                this.remainTimeText.text = LanguageHelper.TimeToString((uint)rt, LanguageHelper.TimeFormat.Type_4);
#if DEBUG_MODE
                                this.remainTimeText.text += " " + id;
#endif
                                this.slider.value = 1f - (1f * rt / this.csv.time);
                            }
                            else {
                                this.remainTimeText.text = "00:00:00";
                                this.slider.value = 0f;
                            }
                        }
                    }
                }
            }
        }

        public Text limitLevel;
        public Text nextTime;
        public GameObject canLianhua;

        public UI_CostItem costItem = new UI_CostItem();
        public ItemIdCount itemIdCount = new ItemIdCount();
        public Button btnExchanged;
        public Transform propItemParent;

        public PropItem propItem;
        public PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData();

        public GameObject haveLianhuaGo;
        public GameObject noLianhuaGo;

        public GameObject proto;
        public Transform protoParent;
        public List<Tab> tabVds = new List<Tab>();
        public List<string> reasons = new List<string>();

        protected override void OnLoaded() {
            this.propItemParent = this.transform.Find("Animator/View_Message/CanLianhua/Item");

            this.haveLianhuaGo = this.transform.Find("Animator/View_Message/List_Point").gameObject;
            this.noLianhuaGo = this.transform.Find("Animator/View_Message/No").gameObject;

            this.costItem.SetGameObject(this.transform.Find("Animator/View_Message/CanLianhua/Cost_Coin").gameObject);
            this.btnExchanged = this.transform.Find("Animator/View_Message/CanLianhua/Button_Exchange").GetComponent<Button>();
            this.btnExchanged.onClick.AddListener(this.OnBtnClicked);

            Button btn = this.transform.Find("Animator/View_TipsBg01_Square1/Btn_Close").GetComponent<Button>();
            btn.onClick.AddListener(() => {
                this.CloseSelf();
            });

            this.canLianhua = this.transform.Find("Animator/View_Message/CanLianhua").gameObject;
            this.limitLevel = this.transform.Find("Animator/View_Message/CanLianhua/Condition/Text_Grade/Text").GetComponent<Text>();
            this.nextTime = this.transform.Find("Animator/View_Message/CanLianhua/Condition/Text_Time").GetComponent<Text>();

            Transform t = this.haveLianhuaGo.transform;
            for (int i = 0, length = t.childCount; i < length; ++i) {
                Tab tab = new Tab();
                tab.Init(t.GetChild(i));
                this.tabVds.Add(tab);
            }
        }
        protected override void OnDestroy() {
            for (int i = 0, length = this.tabVds.Count; i < length; ++i) {
                this.tabVds[i].OnDestroy();
            }
            this.tabVds.Clear();
        }
        protected override void ProcessEvents(bool toRegister) {
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnExchangeTalentPoint, this.OnExchangeTalentPoint, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnTalentLimitChanged, this.OnTalentLimitChanged, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, this.OnItemCountChanged, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnAddExp, this.OnPlayerExpChanged, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, this.OnPlayerExpChanged, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnCanRemainTalentChanged, this.OnCanRemainTalentChanged, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnLianghuaChanged, this.OnLianghuaChanged, toRegister);
        }

        private void OnBtnClicked() {
            if (reasons.Count <= 0) {
                Sys_Talent.Instance.ReqExchangeTalentPoint();
            }
            else {
                Sys_Hint.Instance.PushContent_Normal(reasons[0]);
            }
        }

        private int maxLianhua;
        protected override void OnOpened() {
            maxLianhua = int.Parse(CSVParam.Instance.GetConfData(257).str_value);
        }
        protected override void OnShow() { this.Refresh(); }
        private void Refresh() {
            bool hasLianhua = (Sys_Talent.Instance.lianhuaTimeStamps.Count > 0);
            this.haveLianhuaGo.SetActive(hasLianhua);
            this.noLianhuaGo.SetActive(!hasLianhua);

            this.reasons.Clear();

            uint nextLianhuaId = Sys_Talent.Instance.nextLianhuaId;
            CSVTalentExchange.Data csv = CSVTalentExchange.Instance.GetConfData(nextLianhuaId);
            if (csv != null) {
                this.canLianhua.SetActive(true);

                if (this.propItem == null) {
                    this.propItem = PropIconLoader.GetAsset(this.itemData, this.propItemParent);
                    this.propItem.transform.localPosition = Vector3.zero;
                    (this.propItem.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                }

                this.itemData.Refresh(csv.item[0], csv.item[1], true, false, false, false, false, true, true, true, null, true);
                this.propItem.SetData(this.itemData, EUIID.UI_TalentExchange);

                this.propItem.SetActive(true);
                this.costItem.gameObject.SetActive(true);

                this.costItem?.Refresh(this.itemIdCount.Reset(2, csv.gold));
                this.nextTime.text = LanguageHelper.TimeToString(csv.time, LanguageHelper.TimeFormat.Type_4);

                this.limitLevel.text = csv.level.ToString();

                bool rlt = Sys_Role.Instance.Role.Level >= csv.level;
                if (!rlt) {
                    string content = LanguageHelper.GetTextContent(5524, csv.level.ToString());
                    this.reasons.Add(content);
                    return;
                }
                rlt = Sys_Talent.Instance.lianhuaingPoint < maxLianhua;
                if (!rlt) {
                    string content = LanguageHelper.GetTextContent(5527);
                    this.reasons.Add(content);
                    return;
                }
                rlt = this.propItem.ItemData.Enough;
                if (!rlt) {
                    string content = LanguageHelper.GetTextContent(5526);
                    this.reasons.Add(content);
                    return;
                }

                rlt = this.costItem.idCount.Enough;
                if (!rlt) {
                    string content = LanguageHelper.GetTextContent(5525);
                    this.reasons.Add(content);
                    return;
                }

                rlt = (Sys_Talent.Instance.currentLimitTalentPoint - Sys_Talent.Instance.lianhuaingPoint - Sys_Talent.Instance.usingScheme.withoutLianhuaing > 0);
                if (!rlt) {
                    string content = LanguageHelper.GetTextContent(5528);
                    this.reasons.Add(content);
                    return;
                }

                ButtonHelper.Enable(this.btnExchanged, rlt);
            }
            else {
                this.canLianhua.SetActive(false);

                // Debug.LogErrorFormat("找不到下一个炼化等级数据 id：{0}", nextLianhuaId);
            }
        }

        protected override void OnUpdate() {
#if USE_SPLIT_FRAME
            //if (TimeManager.updateCount % 2 == 0)
#else
            if (Time.frameCount % 2 == 0)
#endif
            {
                int count = Sys_Talent.Instance.lianhuaTimeStamps.Count;
                for (int i = 0, length = this.tabVds.Count; i < length; ++i) {
                    if (i < count) {
                        this.tabVds[i].Show();
                        this.tabVds[i].Refresh(i);
                    }
                    else {
                        this.tabVds[i].Hide();
                    }
                }
            }
        }

        // 兑换回复，需要扣除资源
        private void OnExchangeTalentPoint() {
            this.Refresh();
        }
        private void OnTalentLimitChanged() {
            this.Refresh();
        }
        private void OnItemCountChanged(int changeType, int curBoxId) {
            this.Refresh();
        }
        private void OnPlayerExpChanged() {
            this.Refresh();
        }
        private void OnCanRemainTalentChanged() {
            this.Refresh();
        }
        private void OnLianghuaChanged() {
            this.Refresh();
        }
    }
}