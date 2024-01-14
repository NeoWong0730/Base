using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Modification_Name_Layout
    {
        public Transform transform;
        public Button cancleBtn;
        public Button confirmBtn;
        public InputField Input;
        public Button closeBtn;
        public void Init(Transform transform)
        {
            this.transform = transform;
            cancleBtn = transform.Find("Animator/Btn_02").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            Input = transform.Find("Animator/InputField").GetComponent<InputField>();
        }

        public void RegisterEvents(IListener listener)
        {
            cancleBtn.onClick.AddListener(listener.OnCancleClicked);
            confirmBtn.onClick.AddListener(listener.OnConfirmClicked);
            closeBtn.onClick.AddListener(listener.OnCloseClicked);
        }

        public interface IListener
        {
            void OnCancleClicked();
            void OnConfirmClicked();
            void OnCloseClicked();
        }
    }
    public class UI_Modification_Name : UIBase, UI_Modification_Name_Layout.IListener
    {
        private UI_Modification_Name_Layout layout = new UI_Modification_Name_Layout();
        ClientPet clientPet;
        protected override void OnLoaded()
        {
            layout.Init(gameObject.transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            layout.Input.text = Sys_Pet.Instance.GetPetName(clientPet);
        }

        protected override void OnOpen(object arg1 = null)
        {
            clientPet = arg1 as ClientPet;
        }

        public void OnCancleClicked()
        {
            CloseSelf();
        }

        public void OnCloseClicked()
        {
            CloseSelf();
        }

        public void OnConfirmClicked()
        {
            CSVParam.Data csv = CSVParam.Instance.GetConfData(1);
            int nameLenLimit = csv == null ? 10 : System.Convert.ToInt32(csv.str_value);
            string name = layout.Input.text.Trim();
            if (name == "")
            {
                // Todo:
                Sys_Hint.Instance.PushContent_Normal("名字太短");
            }
            else if (name.Length > nameLenLimit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11006));
            }
            else if (Sys_RoleName.Instance.HasBadNames(name))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101011));
            }
            else
            {
                Sys_Pet.Instance.OnPetRenameReq(clientPet.petUnit.Uid, name);
                CloseSelf();
            }
        }
    }

}