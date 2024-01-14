using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using Table;
namespace Logic
{
    public partial class UI_BattlePass_VIP_Layout
    {
        Button m_BtnClose;

        RawImage m_RImgModel;

        Button m_BtnFinish;

        Transform m_TransFX;

        private Text m_TexTitleName0;
        private Text m_TextTitleName1;

        private Image m_ImgIcon;

        private Text m_TextVipInfo;

        public void OnLoaded(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();

            LoadNormal(root.Find("Animator/Image_Bg/Image_Bg_L/Pass0"));
            LoadSuper(root.Find("Animator/Image_Bg/Image_Bg_R/Pass1"));

            m_RImgModel = root.Find("Animator/Image_Bg/Image2").GetComponent<RawImage>();

            m_BtnFinish = root.Find("Animator/Finish").GetComponent<Button>();

            LoadTips(root.Find("Animator/UI_Tips"));

            m_TransFX = root.Find("Animator/Finish/Image_Title01/Fx_ui_Common");

            m_TextVipInfo = root.Find("Animator/Finish/Image_BG/PassName").GetComponent<Text>();

            m_TexTitleName0 = root.Find("Animator/Finish/Image_Title01/Text_01").GetComponent<Text>();
            m_TextTitleName1 = root.Find("Animator/Finish/Image_Title01/Text_01/Text_01").GetComponent<Text>();
            m_ImgIcon = root.Find("Animator/Finish/Image_BG/Icon").GetComponent<Image>();
        }

        public void SetListener(IListener listener)
        {

            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnNormal.onClick.AddListener(listener.OnClickSureNormal);
            m_BtnSuper.onClick.AddListener(listener.OnClickSureSuper);

            m_BtnFinish.onClick.AddListener(listener.OnClickCloseFinish);

            m_BtnTipsCancle.onClick.AddListener(listener.OnClickTipsCancle);
            m_BtnTipsSure.onClick.AddListener(listener.OnClickTipsSure);

            m_NormalGrid.onCellChange = listener.OnNormalInfinityGridCellChange;
            m_SuperGrid.onCellChange = listener.OnSuperInfinityGridCellChange;
        }

        public void SetCostIcon(uint icon)
        {
            SetNormalCostIcon(icon);
            SetSuperCostIcon(icon);
        }

        public void SetModelController(CutSceneModelShowController controller)
        {
            m_RImgModel.texture = controller.m_ShowSceneControl.GetTemporary(256, 512, 16, RenderTextureFormat.ARGB32, 1);
        }


        public void SetFinishActive(bool active)
        {
            if (m_BtnFinish.gameObject.activeSelf != active)
                m_BtnFinish.gameObject.SetActive(active);
        }

        public void SetSuperVipFxActive(bool active)
        {
            if (m_TransFX.gameObject.activeSelf != active)
                m_TransFX.gameObject.SetActive(active);
        }



        public void SetVipInfo(CSVBattlePassPurchase.Data data)
        {
            TextHelper.SetText(m_TexTitleName0, data.Activation_Des);
            TextHelper.SetText(m_TextTitleName1, data.Activation_Des);

            ImageHelper.SetIcon(m_ImgIcon, data.Icon_ID);

            TextHelper.SetText(m_TextVipInfo, data.Title_Des);
        }
    }

    public partial class UI_BattlePass_VIP_Layout
    {
        public interface IListener
        {
            void OnClickClose();

            void OnClickSureNormal();
            void OnClickSureSuper();
            void OnClickCloseFinish();

            void OnClickTipsCancle();

            void OnClickTipsSure();

            void OnNormalInfinityGridCellChange(InfinityGridCell cell,int index);

            void OnSuperInfinityGridCellChange(InfinityGridCell cell,int index);

        }
    }

    public partial class UI_BattlePass_VIP_Layout
    {
        public  class UIItemLayout
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false);

            public void Load(Transform transform)
            {

                m_Item = new PropItem();

                m_Item.BindGameObject(transform.gameObject);
            }

            public void SetItem(uint id, uint count)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;
                m_ItemData.SetQuality(0);
                m_Item.SetData(new MessageBoxEvt() { sourceUiId = EUIID.UI_BattlePass, itemData = m_ItemData });
            }
        }
    }
    public partial class UI_BattlePass_VIP_Layout
    {
        Button m_BtnNormal;


        Text m_TexNormalCost;

        Image m_ImgNormalCost;

        InfinityGrid m_NormalGrid;

        Text m_NormalTips;
        void LoadNormal(Transform root)
        {

            m_NormalGrid = root.Find("Scroll View").GetComponent<InfinityGrid>();

            m_BtnNormal = root.Find("Btn_01").GetComponent<Button>();

            m_TexNormalCost = root.Find("Common_Cost01/Text_Num").GetComponent<Text>();

            m_ImgNormalCost = root.Find("Common_Cost01/Image_Icon").GetComponent<Image>();

            m_NormalGrid.onCreateCell = OnInfinityCreateCell;

            m_NormalTips = root.Find("Tip").GetComponent<Text>();
        }

   
        
        public void SetNormalItems(List<ItemIdCount> items)
        {
            m_NormalGrid.CellCount = items.Count;

        }

        public void SetNormalCost(uint value)
        {
            TextHelper.SetText(m_TexNormalCost, value.ToString());
        }

        public void SetNormalCostIcon(uint icon)
        {
            ImageHelper.SetIcon(m_ImgNormalCost, icon);
        }

        private void OnInfinityCreateCell(InfinityGridCell cell)
        {
            UIItemLayout uIItemLayout = new UIItemLayout();

            uIItemLayout.Load(cell.mRootTransform);

            cell.BindUserData(uIItemLayout);
        }

        public void SetNormalTips(uint lanugeid)
        {
            TextHelper.SetText(m_NormalTips, lanugeid);
        }
    }

    public partial class UI_BattlePass_VIP_Layout
    {
        Button m_BtnSuper;

        Text m_TexSuperCost;
        Image m_ImgSuperCost;

        Text m_SuperTips;
        private InfinityGrid m_SuperGrid;
        void LoadSuper(Transform root)
        {
            m_BtnSuper = root.Find("Btn_01").GetComponent<Button>();
            m_TexSuperCost = root.Find("Common_Cost01/Text_Num").GetComponent<Text>();

            m_SuperGrid = root.Find("Scroll View").GetComponent<InfinityGrid>();

            m_ImgSuperCost = root.Find("Common_Cost01/Image_Icon").GetComponent<Image>();


            m_SuperTips = root.Find("ScrollTips/Viewport/Tip").GetComponent<Text>();

            m_SuperGrid.onCreateCell = OnInfinityCreateCell;
        }

        public void SetSuperCost(uint value)
        {
            TextHelper.SetText(m_TexSuperCost, value.ToString());
        }

        public void SetSuperCostIcon(uint icon)
        {
            ImageHelper.SetIcon(m_ImgSuperCost, icon);
        }


        public void SetSuperTips(uint lanugeid)
        {
            TextHelper.SetText(m_SuperTips, lanugeid);
        }

        public void SetSuperItems(List<ItemIdCount> items)
        {
            m_SuperGrid.CellCount = items.Count;

        }
    }

    public partial class UI_BattlePass_VIP_Layout
    {
        private Transform m_TransTips;

        private Text m_TexContent;

        private Button m_BtnTipsSure;
        private Button m_BtnTipsCancle;
        void LoadTips(Transform root)
        {
            m_TransTips = root;

            m_TexContent = root.Find("Animator/Text_Tip").GetComponent<Text>();

            m_BtnTipsSure = root.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();

            m_BtnTipsCancle = root.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();


        }

        public void SetTipsActive(bool active)
        {
            if (m_TransTips.gameObject.activeSelf != active)
                m_TransTips.gameObject.SetActive(active);
        }

        public void SetTipsContext(string str)
        {

        }
    }
}
