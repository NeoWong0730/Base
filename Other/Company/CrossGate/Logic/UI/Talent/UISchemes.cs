using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class Scheme : UISelectableElement {
            public CP_Toggle toggle;

            // normal
            public Transform normalGo;
            public Text tabNameLight;
            public Text tabNameDark;

            public Button btnRename;
            public Transform usingGo;

            public Button btnAdd;

            public int index;

            protected override void Loaded() {
                this.normalGo = this.transform.Find("Normal");
                this.usingGo = this.transform.Find("Normal/Using");

                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Normal/Light/Text").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Normal/Dark/Text").GetComponent<Text>();

                this.btnRename = this.transform.Find("Normal/Button_Rename").GetComponent<Button>();
                btnRename.onClick.AddListener(OnBtnRenameClicked);

                this.btnAdd = this.transform.Find("Button_Add").GetComponent<Button>();
                btnAdd.onClick.AddListener(OnBtnAddClicked);
            }

            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke(this.index, true);
                }
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }

            public virtual void Refresh(int index, bool isLastIndex, bool isUsing) {
                // this.index = index;
                //
                // btnAdd.gameObject.SetActive(isLastIndex);
                // normalGo.gameObject.SetActive(!isLastIndex);
                // if (!isLastIndex) {
                //     TextHelper.SetText(tabNameDark, Sys_Talent.Instance.schemes[index].name);
                //     tabNameLight.text = tabNameDark.text;
                //
                //     usingGo.gameObject.SetActive(isUsing);
                // }
            }

            public virtual void OnBtnRenameClicked() {
                // UIManager.OpenUI(EUIID.UI_ReName);
            }

            public virtual void OnBtnAddClicked() {
                // 请求server新增方案，同时给新方案设置空的数据
            }
        }
    
    public class UISchemes<T> : UIComponent where T : Scheme, new() {
        public int selectedIndex { get; set; } = 0;

        public virtual int usingIndex {
            get;
        }

        public UIElementCollector<T> vds = new UIElementCollector<T>();

        public interface Interface {
            void OnBtnRenameClicked(int index);
            void OnSelected(int index);
            void OnBtnAddClicked(int index);
        }

        public Transform tabParent;
        public GameObject tabProto;

        protected override void Loaded() {
            tabParent = transform.Find("TabList");
            tabProto = transform.Find("TabList/Proto").gameObject;
        }

        protected void OnRefreshOne(Scheme vd, int id, int indexOfVdList) {
            vd.SetUniqueId(id);
            vd.SetSelectedAction((innerId, force) => {
                // need - 1
                this.selectedIndex = innerId;
                onSelected?.Invoke(this.selectedIndex);
            });
            vd.Refresh(indexOfVdList, schemes.Count - 1 == indexOfVdList, indexOfVdList == usingIndex);
        }

        protected List<int> schemes = new List<int>();
        public Action<int> onSelected;
        public void Refresh(int count, int curIndex, Action<int> onSelected, bool needSelect = true) {
            this.selectedIndex = curIndex;
            this.onSelected = onSelected;
            
            ListHelper.BuildList(ref schemes, 1, count + 1);
            this.vds.BuildOrRefresh<int>(this.tabProto, this.tabParent, schemes, OnRefreshOne);
            
            if(needSelect) {
                if (this.vds.TryGetVdByIndex(this.selectedIndex, out var vd)) {
                    vd.SetSelected(true, true);
                }
                else if(vds.Count > 0){
                    vds[0].SetSelected(true, true);
                }
            }
        }
    }
}