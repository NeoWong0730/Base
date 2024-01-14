using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_ChangeSchemeName : UIBase, UI_ChangeSchemeName.Layout.IListener {
        public class Layout : UILayoutBase {
            public Text title;
            public InputField input;

            // public Button btnNo;
            public Button btnYes;

            public interface IListener {
                void OnBtnNoClicked();
                void OnBtnYesClicked();
            }

            public void Parse(GameObject root) {
                this.Init(root);

                this.title = this.transform.Find("Animator/View_TipsBg01_Small/Text_Title").GetComponent<Text>();
                this.input = this.transform.Find("Animator/InputField").GetComponent<InputField>();
                // this.btnNo = this.transform.Find("Animator/Btn_02").GetComponent<Button>();
                this.btnYes = this.transform.Find("Animator/Btn_01").GetComponent<Button>();
            }

            public void RegisterEvents(IListener listener) {
                // this.btnNo.onClick.AddListener(listener.OnBtnNoClicked);
                this.btnYes.onClick.AddListener(listener.OnBtnYesClicked);
            }
        }
        
        public class ChangeNameArgs {
            public int arg1;
            public int arg2;
            public string oldName;
            public Action<int, int, string> onYes;
        }

        public Layout layout = new Layout();

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        public int arg1;
        public int arg2;
        public string oldName;
        public Action<int, int, string> onYes;

        protected override void OnOpen(object arg) {
            var tp = arg as ChangeNameArgs;
            if (tp != null) {
                arg1 = tp.arg1;
                arg2 = tp.arg2;
                oldName = tp.oldName;
                onYes = tp.onYes;
            }
        }

        protected override void OnOpened() {
            // 标题是否需要变化
            // TextHelper.SetText(layout.title, "");
        }

        public void OnBtnNoClicked() {
            CloseSelf();
        }

        // copy from UI_ReName.cs
        public bool CheckValid(out string newName) {
            // 输入框内容是否合法
            newName = layout.input.text.Trim();
            CSVParam.Data csv = CSVParam.Instance.GetConfData(1);
            uint nameLenLimit = csv == null ? 10 : System.Convert.ToUInt32(csv.str_value);
            if (string.IsNullOrWhiteSpace(newName)) {
                //输入为空
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025103));
                return false;
            }
            else if (newName.Equals(oldName)) {
                //和原名相同
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025126));
                return false;
            }
            else if (newName.Length > nameLenLimit) {
                //超出名字最大长度
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101001));
                return false;
            }
            else if (Sys_RoleName.Instance.HasBadNames(newName)) {
                //名字内含有违禁字、特殊字符
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101011));
                return false;
            }

            return true;
        }

        public void OnBtnYesClicked() {
            if (CheckValid(out string newName)) {
                CloseSelf();
                onYes?.Invoke(arg1, arg2, newName);
            }
        }
    }
}