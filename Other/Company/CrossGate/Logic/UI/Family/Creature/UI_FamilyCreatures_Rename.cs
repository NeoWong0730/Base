using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_FamilyCreatures_Rename_Layout
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
    public class UI_FamilyCreatures_Rename : UIBase, UI_FamilyCreatures_Rename_Layout.IListener
    {
        private UI_FamilyCreatures_Rename_Layout layout = new UI_FamilyCreatures_Rename_Layout();
        private FamilyCreatureEntry familyCreatureEntry;
        protected override void OnLoaded()
        {
            layout.Init(gameObject.transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            if(null != familyCreatureEntry)
            {
                layout.Input.text = familyCreatureEntry.Name;
            }
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
            {
                this.familyCreatureEntry = arg as FamilyCreatureEntry;
            }
        }

        public void OnCancleClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Rename, "OnCancleClicked");
            CloseSelf();
        }

        public void OnCloseClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Rename, "OnCloseClicked");
            CloseSelf();
        }

        public void OnConfirmClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Rename, "OnConfirmClicked");
            //CSVParam.Data csv = CSVParam.Instance.GetConfData(1);
            //int nameLenLimit = csv == null ? 10 : System.Convert.ToInt32(csv.str_value);
            string name = layout.Input.text.Trim();
            if (name == "")
            {
            }
            else if (GetStrLength(name, 4, 12))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2023350));
            }
            else if (Sys_RoleName.Instance.HasBadNames(name))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101011));
            }
            else
            {
                Sys_Family.Instance.GuildPetChangeNameReq(familyCreatureEntry.FmilyCreatureId, name);
                CloseSelf();
            }
        }

        private bool GetStrLength(string text, int min, int max)
        {
            int wordLength = 0;
            int index;
            int textLen = text.Length;
            for (index = 0; index < textLen; index++)
            {

                if (text[index] <= 256)
                {
                    wordLength++;
                }
                else
                {
                    wordLength += 2;
                }
            }
            return wordLength > max || wordLength < min;
        }
    }
}