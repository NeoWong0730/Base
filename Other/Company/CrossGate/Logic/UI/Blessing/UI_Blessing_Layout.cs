using System;
using System.Collections.Generic;

using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Blessing_Layout
    {
        public class BlessItem
        {
            public  PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, false, false, false, false, false, true, false);
            public PropItem propItem = new PropItem();

            public Text TexBtn;
            public Text TexTimes;
            public Text TexInfo;
            private Button BtnBless;
            public RawImage ImgBackGround;

            public Action<int> OnClickBtnBless;

            public int Index;

            public Transform TransRedPoint;
            public void Load(Transform root)
            {
                BtnBless = root.Find("Btn").GetComponent<Button>();
                TexBtn = root.Find("Btn/Text_01").GetComponent<Text>();
                TexTimes = root.Find("Image/Text").GetComponent<Text>();
                TexInfo = root.Find("Text_Title").GetComponent<Text>();

                propItem.BindGameObject(root.Find("PropItem").gameObject);

                ImgBackGround = root.Find("Image_Bg").GetComponent<RawImage>();

                BtnBless.onClick.AddListener(OnClickBless);

                TransRedPoint = root.Find("Btn/Image_Dot");
            }

            private void OnClickBless()
            {
                OnClickBtnBless?.Invoke(Index);
            }

            public void SetReward(uint id, long count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                propItem.SetData(m_ItemData,EUIID.UI_Blessing);
            }

            public void SetState(int state)
            {
                uint btnLid = 8361u;
                if (state >= 0)
                {
                    btnLid += (uint)state;
                }

                BtnBless.interactable = state >= 0;

                TextHelper.SetText(TexBtn, btnLid);
            }
        }
    }
    public partial class UI_Blessing_Layout
    {
        public InfinityGrid PropertyGrid;

        public UI_CurrencyTitle currencyTitle;

        Button BtnClose;

        Button BtnHelp;
        public void Load(Transform root)
        {
            PropertyGrid = root.Find("Animator/View_Content/Scroll View").GetComponent<InfinityGrid>();

            var transcurrencytitle = root.Find("Animator/UI_Property");

            currencyTitle = new UI_CurrencyTitle(transcurrencytitle.gameObject);

            BtnClose = root.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();

            BtnHelp = root.Find("Animator/View_Title01/BtnHelp_0").GetComponent<Button>();
        }

        private void OnClickBtnHelp()
        {
            CSVUIRule.Data csvRule = Table.CSVUIRule.Instance.GetConfData(1085u);
            UIManager.OpenUI(EUIID.UI_HelpRule, false, csvRule.ruleIds);
           
        }

        public interface IListener
        {
            void OnClickClose();

            void OnGridCreate(InfinityGridCell cell);
            void OnGridUpdate(InfinityGridCell cell, int index);


        }


        public void SetListener(IListener listener)
        {
            BtnClose.onClick.AddListener(listener.OnClickClose);
            PropertyGrid.onCreateCell = listener.OnGridCreate;
            PropertyGrid.onCellChange = listener.OnGridUpdate;

            BtnHelp.onClick.AddListener(OnClickBtnHelp);
        }
    }
}
