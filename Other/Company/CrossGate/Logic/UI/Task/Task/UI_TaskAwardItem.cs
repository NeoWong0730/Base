using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {

    public class UI_TaskAwardItem : UIComponent {
        public uint id;
        public int count;
        public Action<uint, int> onSelectedAction;

        public Text textCount;
        public Text textName;
        public Button buttonGo;
        public RectTransform transformSelected;
        public Image icon;

        protected override void Loaded() {
            this.textCount = this.transform.Find("Text_Number").GetComponent<Text>();
            this.textName = this.transform.Find("Text_Name").GetComponent<Text>();
            this.buttonGo = this.transform.Find("Image_Icon").GetComponent<Button>();
            this.icon = this.transform.Find("Image_Icon").GetComponent<Image>();
            this.transformSelected = this.transform.Find("Image_Select").GetComponent<RectTransform>();

            this.buttonGo.onClick.AddListener(this.OnClicked);
            this.SetSelected(false);
        }

        public void Refresh(uint id, int count, Action<uint, int> onSelectedAction) {
            this.id = id;
            this.count = count;
            this.onSelectedAction = onSelectedAction;

            CSVItem.Data csv = CSVItem.Instance.GetConfData(id);
            if (csv != null) {
                this.textCount.text = count.ToString();
                TextHelper.SetText(this.textName, csv.name_id);
            }
            else {
                DebugUtil.LogErrorFormat("Item csv is Error by id: {0}", id);
            }
        }

        private void OnClicked() {
            this.SetSelected(true);
            this.onSelectedAction?.Invoke(this.id, this.count);
        }

        public void SetSelected(bool toSelected) {
            this.transformSelected.gameObject.SetActive(toSelected);
        }

        public override void OnDestroy() {
            base.OnDestroy();
        }
    }
}
