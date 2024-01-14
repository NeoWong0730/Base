using System;
using System.Collections.Generic;
using Common;
using Framework.UI;
using Lib.Core;
using Logic.Core;
using Table;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Logic.UI {
    public partial class UI_CreateCharacter : UIBase {
        public class Head : VD<UI_CreateCharacter_Head_Layout> {
            public void Refresh(uint careerId, int index) {
                layout.tg.id = index;

                var csv = CSVCareer.Instance.GetConfData(careerId);
                if (csv != null) {
                    ImageHelper.SetIcon(layout.imgIcon, csv.icon);
                }
            }
        }

        public UI_CreateCharacter_Layout layout;

        public enRoleSex curSex = enRoleSex.Male;

        private List<uint> careerIds = new List<uint>();
        public COWVd<Head> heads = new COWVd<Head>();
        public PosSelector selector;

        public int selectedIndex = 0;

        public uint selectedCareerId {
            get { return careerIds[selectedIndex]; }
        }

        private float[] _radarArray;
        private Timer _roleTimer;

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_CreateCharacter_Layout>(this.transform);
            layout.Init(this.transform);
            layout.BindEvents(this);

            selector = GameObject.FindObjectOfType<PosSelector>();
            selector.InitCreate();
        }

        protected override void OnDestroy() {
            _roleTimer?.Cancel();
            selector = null;
        }

        protected override void OnOpened() {
            selector.SetStage(PosSelector.EStage.Create);
            RefreshAll();
            // TrySetExpireTimer(ref _roleTimer);
        }

        private void TrySetExpireTimer(ref Timer tmer) {
            layout.cdNode.gameObject.SetActive(false);
            long now = Sys_Time.Instance.GetServerTime();
            long expire = Sys_Login.Instance.actionExpire;
            long diff = expire - now;
            if (diff > 0) {
                void _DoCD() {
                    layout.cdNode.gameObject.SetActive(true);
                    layout.cdText.Begin(diff);
                }

                if (diff <= 300) {
                    _DoCD();
                }
                else {
                    // 大于3分钟则开启后台计时， 否则直接显示倒计时Text
                    layout.cdNode.gameObject.SetActive(false);
                    tmer = Timer.RegisterOrReuse(ref tmer, diff - 300, () => { _DoCD(); });
                }
            }
        }

        public void RefreshAll() {
            careerIds.Clear();
            var csvCareer = CSVCareer.Instance.GetAll();
            for (int i = 0, length = csvCareer.Count; i < length; ++i) {
                careerIds.Add(csvCareer[i].id);
            }

            heads.TryBuildOrRefresh(layout.proto.gameObject, layout.protoParent, careerIds.Count, _OnCareerRefresh);
            layout.centerOn.Rebuild();

            // 默认选中职业
            if (selectedIndex != 0) {
                layout.tgRegistry.SwitchTo(selectedIndex, false);
            }
            else if (heads.RealCount > 0) {
                layout.tgRegistry.SwitchTo(0, false);
            }
        }

        private void _OnCareerRefresh(Head vd, int index) {
            vd.Refresh(careerIds[index], index);
        }

        public void CtrlSexVisiblity(int sex) {
            if (sex == -1) {
                layout.tgWomen.gameObject.SetActive(false);
                layout.tgMan.gameObject.SetActive(false);
            }
            else if (sex == 0) {
                layout.tgWomen.gameObject.SetActive(true);
                layout.tgMan.gameObject.SetActive(false);
            }
            else if (sex == 1) {
                layout.tgWomen.gameObject.SetActive(false);
                layout.tgMan.gameObject.SetActive(true);
            }
            else {
                layout.tgWomen.gameObject.SetActive(true);
                layout.tgMan.gameObject.SetActive(true);
            }
        }

        private void _OnSexChanged(int sexTgId, bool interaction) {
            var sex = 1 - sexTgId;
            curSex = (enRoleSex)sex;

            // todo sex模型加载
            var careerId = selectedCareerId;
            PosSelector.CreateCfg st = default;
            if (selector.TryFindCreate(careerId, ref st)) {
                st.Visible(true);
                // todo
                PosSelector.ETimelineType aim;
                if (!interaction) {
                    aim = curSex == (int)enRoleSex.Male ? PosSelector.ETimelineType.Enter_M : PosSelector.ETimelineType.Enter_F;
                }
                else {
                    aim = curSex == (int)enRoleSex.Male ? PosSelector.ETimelineType.F_M : PosSelector.ETimelineType.M_F;
                }
                st.PlayTimeline(aim);
            }
        }

        private void _OnCareerSelected(int index, bool interaction) {
            selectedIndex = index;
            layout.centerOn.CenterOn(index, true);
            selector.CtrlCreate(int.MaxValue); // 全部隐藏
            
            var csvCareer = CSVCareer.Instance.GetConfData(selectedCareerId);
            if (csvCareer != null) {
                TextHelper.SetText<CSVLanguage>(layout.careerRadarDesc, csvCareer.radarMapDesc);
                TextHelper.SetText<CSVLanguage>(layout.careerDesc, csvCareer.desc);
                ImageHelper.SetIcon(layout.careerNameIcon, csvCareer.nameIcon);
                CtrlSexVisiblity(csvCareer.sex);
                // todo 有可能默认的sex不存在
                // 0 女  toggleId:0
                // 1 男  toggleId:1
                // 2 男女
                if (csvCareer.sex == 0) {
                    layout.sexRegistry.SwitchTo(0, false);
                }
                else {
                    layout.sexRegistry.SwitchTo(1, false);
                }

                var csvCareerAttrMax = CSVCareerAttr.Instance.GetConfData(9999);
                var csvCareerAttr = CSVCareerAttr.Instance.GetConfData(csvCareer.attr);
                if (csvCareerAttr != null) {
                    if (_radarArray == null) {
                        _radarArray = new float[8 + 1];
                    }

                    _radarArray[0] = csvCareerAttr.mp * 1f / csvCareerAttrMax.mp;
                    _radarArray[1] = csvCareerAttr.power * 1f / csvCareerAttrMax.power;
                    _radarArray[2] = csvCareerAttr.weight * 1f / csvCareerAttrMax.weight;
                    _radarArray[3] = csvCareerAttr.dex * 1f / csvCareerAttrMax.dex;
                    _radarArray[4] = csvCareerAttr.internalForce * 1f / csvCareerAttrMax.internalForce;
                    _radarArray[5] = csvCareerAttr.hp * 1f / csvCareerAttrMax.hp;
                    _radarArray[6] = csvCareerAttr.attack * 1f / csvCareerAttrMax.attack;
                    _radarArray[7] = csvCareerAttr.defense * 1f / csvCareerAttrMax.defense;
                    _radarArray[8] = csvCareerAttr.mp * 1f / csvCareerAttrMax.mp;
                    layout.radar.DrawPolygon(8, _radarArray);
                }
                else {
                    layout.radar.DrawPolygon(8);
                }
            }
            else {
                CtrlSexVisiblity(-1);
            }
        }
    }

// UI事件
    public partial class UI_CreateCharacter : UI_CreateCharacter_Layout.IListener {
        public void OnBtnClicked_btnEnter() {
            UIManager.OpenUI(EUIID.UI_MakeFace, false, new Tuple<uint, enRoleSex>(selectedCareerId, curSex));
        }

        public void OnBtnClicked_btnReturn() {
            Sys_Login.Instance.ExitGame(true);
        }

        public void OnToggleChange_sexRegistry(int newId, int oldId, bool interaction) {
            _OnSexChanged(newId, interaction);
        }

        public void OnToggleChange_tgRegistry(int newId, int oldId, bool interaction) {
            _OnCareerSelected(newId, interaction);
        }

        public void OnTimeRefresh_cdText(TextMeshProUGUI text, float time, bool isEnd) {
            if (isEnd) {
                TextHelper.SetText(text, "00:00:00");
            }
            else {
                var t = Mathf.Round(time);
                var s = TimeFormater.TimeToString((uint)t, TimeFormater.ETimeFormat.Type_1);
                TextHelper.SetText(text, s);
            }
        }
    }

// 逻辑事件
    public partial class UI_CreateCharacter {
        protected override void ProcessEvents(bool toRegister) {
        }
    }
}