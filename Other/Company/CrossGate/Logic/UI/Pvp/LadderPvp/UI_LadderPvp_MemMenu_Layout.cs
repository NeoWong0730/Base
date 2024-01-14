using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
namespace Logic
{
    public class UI_LadderPvp_MemMenu_Layout
    {
        public Transform TransMenu;

        public class CommandItem : ClickItem
        {
            public Button mBtn;
            public Text mTexName;

            public int ID { get; set; }

            public Action<int> OnClickBtn;
            public override void Load(Transform root)
            {
                base.Load(root);

                mBtn = root.GetComponent<Button>();
                mTexName = root.Find("Text_01").GetComponent<Text>();
                mBtn.onClick.AddListener(OnClick);
            }

            public override ClickItem Clone()
            {
                return Clone<CommandItem>(this);
            }

            private void OnClick()
            {
                if (OnClickBtn != null)
                    OnClickBtn.Invoke(ID);
            }
        }

        public ClickItemGroup<CommandItem> mCommandGroup = new ClickItemGroup<CommandItem>();

        public Button BtnClose;
        public void Load(Transform root)
        {
            TransMenu = root.Find("group/Image_BG");

            mCommandGroup.AddChild(TransMenu.Find("Btn_01"));

            BtnClose = root.Find("Image_off").GetComponent<Button>();
        }
    }
}
