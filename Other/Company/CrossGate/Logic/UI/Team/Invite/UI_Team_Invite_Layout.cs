using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Team_Invite_Layout
    {
        ClickItemGroup<InviteItem> m_ItemsGroup = new ClickItemGroup<InviteItem>();

        Button m_BtnClose;

        InputField m_InputField;
        Button m_BtnSreach;


        IListener m_Listener;

        private Toggle m_TogFriend;
        private Toggle m_TogFamliy;
        private Toggle m_TogNear;
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();

            m_InputField = root.Find("Animator/InputField_Describe").GetComponent<InputField>();

            m_BtnSreach = root.Find("Animator/InputField_Describe/Button_Delete").GetComponent<Button>();

            Transform itemTrans = root.Find("Animator/Scroll_View/Viewport/Item");

            m_ItemsGroup.AddChild(itemTrans);


            m_TogFriend = root.Find("Animator/ScrollView_Menu/List/Toggle0").GetComponent<Toggle>();
            m_TogFamliy = root.Find("Animator/ScrollView_Menu/List/Toggle1").GetComponent<Toggle>();
            m_TogNear = root.Find("Animator/ScrollView_Menu/List/Toggle2").GetComponent<Toggle>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_BtnSreach.onClick.AddListener(listener.OnClickSreach);

            m_InputField.onEndEdit.AddListener(listener.OnInputEnd);

            m_ItemsGroup.SetAddChildListenter(OnAddItems);

            m_TogFriend.onValueChanged.AddListener(listener.OnTogFriend);
            m_TogFamliy.onValueChanged.AddListener(listener.OnTogFamliy);
            m_TogNear.onValueChanged.AddListener(listener.OnTogNear);
        }


        private void OnAddItems(InviteItem item)
        {
            item.OnClickAction = m_Listener.OnClickInvite;
        }

        public void SetInputField(string tex)
        {
            m_InputField.text = tex;
        }
  
        public void SetItemCount(int count)
        {
            m_ItemsGroup.SetChildSize(count);
        }

        public void Set(int index, ulong roleID, uint icon, uint level, string name, uint occ,uint rank,uint headId,uint headFrameId)
        {
            var item = m_ItemsGroup.getAt(index);

            if (item == null)
                return;

            item.Set(roleID, icon, level, name, occ, rank,headId,headFrameId);
        }

        public void SetFocusTogFriend()
        {
            m_TogFriend.isOn = true;
        }

        public void SetFocusTogFamliy()
        {
            m_TogFamliy.isOn = true;
        }

        public void SetFocusTogNear()
        {
            m_TogNear.isOn = true;
        }
    }
    public partial class UI_Team_Invite_Layout
    {
        public interface IListener
        {
            void OnClickInvite(ulong id);

            void OnClickClose();

            void OnClickSreach();

            void OnInputEnd(string value);

            void OnTogFriend(bool state);
            void OnTogFamliy(bool state);
            void OnTogNear(bool state);
        }
    }

    public partial class UI_Team_Invite_Layout
    {
        private class InviteItem : ClickItem
        {
            private Button m_BtnApply;

            private Text m_TexName;

            private Image m_ImgProp;
            private Text m_TexProp;

            private Text m_TexLevel;

            private Image m_ImgRole;

            ulong m_ID;

            public Action<ulong> OnClickAction { get; set; }
            public override void Load(Transform root)
            {
                base.Load(root);

                m_BtnApply = root.Find("Button_Apply").GetComponent<Button>();

                m_TexName = root.Find("Text_Name").GetComponent<Text>();

                m_ImgProp = root.Find("Image_Prop").GetComponent<Image>();
                m_TexProp = root.Find("Text_Profession").GetComponent<Text>();

                m_TexLevel = root.Find("Text_Number").GetComponent<Text>();

                m_ImgRole = root.Find("Image_BG/Head").GetComponent<Image>();

                m_BtnApply.onClick.AddListener(OnClickApply);
            }

            public override ClickItem Clone()
            {
                return Clone<InviteItem>(this);
            }

            public void Set(ulong roleID, uint icon, uint level, string name, uint occ,uint rank,uint headId,uint headFrameId)
            {
                m_ID = roleID;

                var occIcon = OccupationHelper.GetLogonIconID(occ);

                m_ImgProp.gameObject.SetActive(occIcon == 0 ? false : true);

                if (occIcon != 0)
                    ImageHelper.SetIcon(m_ImgProp, occIcon);

                var occTex = OccupationHelper.GetTextID(occ, rank);
                m_TexProp.text = occTex == 0 ? string.Empty : LanguageHelper.GetTextContent(occTex);

                m_TexName.text = name;

                m_TexLevel.text = "Lv" + level.ToString();

               // if (icon == 0)
                 //   icon = 10050033;
                CharacterHelper.SetHeadAndFrameData(m_ImgRole, (uint)icon, headId, headFrameId);
               // ImageHelper.SetIcon(m_ImgRole, icon);
            }

            private void OnClickApply()
            {
                OnClickAction?.Invoke(m_ID);
            }
        }
    }
}
