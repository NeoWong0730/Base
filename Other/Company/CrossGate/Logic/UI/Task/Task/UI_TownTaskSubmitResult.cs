using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TownTaskSubmitResult : UIBase, UI_TownTaskSubmitResult.Layout.IListener {
        public class Layout : UILayoutBase {
            public Text title;

            public void Parse(GameObject root) {
                this.Init(root);

                this.title = this.transform.Find("Animator/Image_BG2/Content/Value").GetComponent<Text>();
            }

            public void RegisterEvents(IListener listener) {
            }

            public interface IListener {
            }
        }

        public Layout layout = new Layout();

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        private Timer timer;
        private uint id;

        protected override void OnDestroy() {
            timer?.Cancel();
        }

        protected override void OnOpen(object arg) {
            id = Convert.ToUInt32(arg);
        }

        protected override void OnOpened() {
            void Close() {
                CloseSelf();
            }

            timer = Timer.RegisterOrReuse(ref timer, 3f, Close);

            var csv = CSVTownTaskLibrary.Instance.GetConfData(id);
            if (csv != null) {
                TextHelper.SetText(layout.title, csv.TaskReward.ToString());
            }
        }
    }
}