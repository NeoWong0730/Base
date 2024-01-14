using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityNPCChanged : UIBase {
        public Text title;
        public Text progress;
        public CP_SliderFTMLerp slider;
        public Text npcFavorability;
        public Transform fx;

        public Timer timer;

        public uint npcId;
        public uint stageId;
        public uint from;
        public uint to;
        public float current;

        protected override void OnLoaded() {
            this.title = this.transform.Find("Animator/Text_Name").GetComponent<Text>();
            this.progress = this.transform.Find("Animator/Text_Percent").GetComponent<Text>();
            this.slider = this.transform.Find("Animator/Slider_Eg").GetComponent<CP_SliderFTMLerp>();
            this.npcFavorability = this.transform.Find("Animator/Text_Number/Text").GetComponent<Text>();
            this.fx = this.transform.Find("Animator/Slider_Eg/Fx_ui_NPC_Tips_Max");

            this.slider.onChanged = this.OnChanged;
        }
        private void OnChanged(float current, float diff, float max, float currentRate) {
            bool isReachedMax = current >= max;
            //this.npcFavorability.text = UnityEngine.Mathf.CeilToInt(diff).ToString();
            this.progress.text = string.Format("{0}/{1}", UnityEngine.Mathf.CeilToInt(current), max);

            this.current = UnityEngine.Mathf.CeilToInt(current);

            if (isReachedMax) {
                if (fx != null) {
                    fx.gameObject.SetActive(true);
                }
            }
        }
        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint, uint, uint>(Sys_NPCFavorability.EEvents.OnNpcFavorabilityChanged, OnNpcFavorabilityChanged, toRegister);
        }
        protected override void OnOpen(object arg) {
            Tuple<uint, uint, uint, uint> tuple = arg as Tuple<uint, uint, uint, uint>;
            if (tuple != null) {
                this.npcId = tuple.Item1;
                this.stageId = tuple.Item2;
                this.from = tuple.Item3;
                this.to = tuple.Item4;
                this.current = from;
            }
        }
        protected override void OnClose() {
            timer?.Cancel();
        }

        protected override void OnOpened() {
            if (fx != null) {
                fx.gameObject.SetActive(false);
            }

            this.timer?.Cancel();
            this.timer = Timer.Register(3.3f, () => {
                this.CloseSelf();
            });

            this.npcFavorability.text = UnityEngine.Mathf.CeilToInt(to - from).ToString();

            uint csvStageId = FavorabilityNPC.GetFavorabilityStageId(this.npcId, this.stageId);
            CSVNPCFavorabilityStage.Data csv = CSVNPCFavorabilityStage.Instance.GetConfData(csvStageId);
            if (csv != null) {
                this.slider.Refresh(this.current, this.to, csv.FavorabilityValueMax);
            }
            CSVFavorabilityStageName.Data csvStageName = CSVFavorabilityStageName.Instance.GetConfData(this.stageId);
            if (csvStageName != null) {
                TextHelper.SetText(this.title, csvStageName.name);
            }
        }

        #region 事件处理
        private void OnNpcFavorabilityChanged(uint npcId, uint stageId, uint oldValue, uint newValue) {
            this.npcId = npcId;
            this.stageId = stageId;
            this.to = newValue;

            OnOpened();
        }
        #endregion
    }
}