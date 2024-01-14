using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_MapFirstEnter : UIBase {
        public Animator animmator;
        public Text content;
        public CSVMapInfo.Data csvMap;

        public uint mapId;
        private Timer timer;

        protected override void OnLoaded() {
            this.animmator = this.transform.Find("Hint_MapEnter").GetComponent<Animator>();
            this.content = this.transform.Find("Hint_MapEnter/Animator/Text").GetComponent<Text>();
        }

        protected override void OnDestroy() {
            timer?.Cancel();
        }

        protected override void OnOpen(object arg) {
            this.mapId = Convert.ToUInt32(arg);
            this.csvMap = CSVMapInfo.Instance.GetConfData(this.mapId);
        }

        protected override void OnOpened() {
            this.Refresh();

            Sys_Map.Instance.firstEnterMaps.Remove(this.mapId);
        }

        private void Refresh() {
            this.timer?.Cancel();
            this.timer = Timer.RegisterOrReuse(ref this.timer, 2.5f, () => {
                this.CloseSelf();
            });
            
            if (this.csvMap != null) {
                // 重新播放动画
                this.animmator.Play("Open", -1, 0);
                TextHelper.SetText(this.content, 4568, LanguageHelper.GetTextContent(this.csvMap.name));
            }
        }

        public override void OnSetData(object arg) {
            this.mapId = Convert.ToUInt32(arg);
            this.csvMap = CSVMapInfo.Instance.GetConfData(this.mapId);
            
            Sys_Map.Instance.firstEnterMaps.Remove(this.mapId);

            if (this.gameObject != null) {
                this.Refresh();
            }
        }
    }
}