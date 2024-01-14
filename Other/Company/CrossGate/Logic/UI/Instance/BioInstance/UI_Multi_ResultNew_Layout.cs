using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Logic.Core;

namespace Logic
{
    public partial class UI_Multi_ResultNew_Layout
    {
        interface UI_IResult
        {
            void SetDesc(string tex);

            void SetName(string tex);
            void SetActive(bool b);


        }

        class RewardItem : ClickItem
        {
            PropItem m_Item;
            PropIconLoader.ShowItemData m_ItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, true, false, true, OnClickPropItem, false, false);
            public override void Load(Transform root)
            {
                base.Load(root);

                m_Item = new PropItem();

                m_Item.BindGameObject(root.gameObject);
            }

            public void SetReward(uint id, uint count,Item item)
            {
                m_ItemData.id = id;
                m_ItemData.count = count;

                if (item != null && item.Uuid > 0)
                    m_ItemData.bagData = new ItemData(0, item.Uuid, item.Id, item.Count, 0, false, false, item.Equipment, item.Essence, 0, null, item.Crystal, item.Ornament);

                m_Item.SetData(new MessageBoxEvt(EUIID.UI_Multi_Info, m_ItemData));

                var data = CSVItem.Instance.GetConfData(id);
                if (data != null)
                    m_Item.txtName.text = LanguageHelper.GetTextContent(data.name_id);
            }

            public override ClickItem Clone()
            {
                return Clone(this);
            }

            static private void OnClickPropItem(PropItem item)
            {
               var cSVItemData = CSVItem.Instance.GetConfData(item.ItemData.id);

                uint typeId = cSVItemData.type_id;
                //装备道具,单独处理
                if (typeId == (uint)EItemType.Equipment)
                {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = item.ItemData.bagData;
                    tipData.isCompare = false;
                    tipData.isShowOpBtn = false;

                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
                else if (typeId == (uint)EItemType.Crystal)
                {
                    CrystalTipsData crystalTipsData = new CrystalTipsData();
                    crystalTipsData.itemData = item.ItemData.bagData;

                    UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
                }
                else if (typeId == (uint)EItemType.Ornament)
                {
                    OrnamentTipsData tipData = new OrnamentTipsData();
                    tipData.equip = item.ItemData.bagData;
                    tipData.sourceUiId = EUIID.UI_Multi_Reward;
                    tipData.isShowOpBtn = false;
                    tipData.isShowSourceBtn = false;
                    UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                }
                else
                {
                    MessageBoxEvt boxEvt = new MessageBoxEvt();
                    boxEvt.Reset(EUIID.UI_Multi_Reward, item.ItemData);
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                }
                //else
                //{
                //    PropMessageParam propParam = new PropMessageParam();
                //    propParam.itemData = item.ItemData.bagData;
                //    propParam.showBtnCheck = false;
                //    propParam.sourceUiId = EUIID.UI_ClassicBossWarResult;
                //    UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                //}
            }
        }

        class UI_Cross: UI_IResult
        {
            private Transform mTrans;

            private Text mTexDesc;

            private Text mTexName;

            private Transform mTransFristCross;

            private ClickItemGroup<RewardItem> m_FristRewardGroup = new ClickItemGroup<RewardItem>();
            public void Load(Transform transform)
            {
                mTrans = transform;

                mTexDesc = transform.Find("Image_BG/Text_Desc").GetComponent<Text>();

                mTexName = transform.Find("Image_BG/Text_Name").GetComponent<Text>();
                SetActive(false);

                mTransFristCross = transform.Find("FirstPass");

                m_FristRewardGroup.AddChild(mTransFristCross.Find("RewardNode/Scroll_View/Viewport/PropItem"));
            }

            public void SetActive(bool b)
            {
                mTrans.gameObject.SetActive(b);

            }

            public void SetDesc(string tex)
            {
                mTexDesc.text = tex;
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }

            public void SetFristCross(bool active)
            {
                if (mTransFristCross.gameObject.activeSelf != active)
                    mTransFristCross.gameObject.SetActive(active);
            }

            public void SetFristRewardCount(int count)
            {
                
                m_FristRewardGroup.SetChildSize(count);


            }

            public void SetFristReward(int index, uint id, uint count,Item item0)
            {
                var item = m_FristRewardGroup.getAt(index);

                if (item == null)
                    return;

                item.SetReward((uint)id, (uint)count,item0);
            }
        }
    }

    public partial class UI_Multi_ResultNew_Layout
    {
        class UI_Success: UI_IResult
        {
            private Transform mTrans;

            private Text mTexDesc;

            private Text mTexName;
            public void Load(Transform transform)
            {
                mTrans = transform;

                mTexDesc = transform.Find("Image_BG/Text_Desc").GetComponent<Text>();

                mTexName = transform.Find("Image_BG/Text_Name").GetComponent<Text>();

                SetActive(false);
            }

            public void SetActive(bool b)
            {

                mTrans.gameObject.SetActive(b);
            }

            public void SetDesc(string tex)
            {
                mTexDesc.text = tex;
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }
        }
    }

    public partial class UI_Multi_ResultNew_Layout
    {
        class UI_Fail: UI_IResult
        {
            private Transform mTrans;

            private Text mTexDesc;

            private Text mTexName;
            public void Load(Transform transform)
            {
                mTrans = transform;

                mTexDesc = transform.Find("Image_BG/Text_Desc").GetComponent<Text>();

                mTexName = transform.Find("Image_BG/Text_Name").GetComponent<Text>();

                SetActive(false);
            }

            public void SetActive(bool b)
            {
                mTrans.gameObject.SetActive(b);

            }

            public void SetDesc(string tex)
            {
                mTexDesc.text = tex;
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }
        }
    }
    public partial class UI_Multi_ResultNew_Layout
    {

        private Button mBtnClose;


        private UI_Cross mUICross = new UI_Cross();
        private UI_Success mUISuccess = new UI_Success();
        private UI_Fail mUIFail = new UI_Fail();

        private UI_IResult mIResut;

        private IListener m_Listener;

        public void Load(Transform root)
        {
            Transform infotrans = root.Find("Animator/Scroll View01/Viewport/Content/RewardItem");

            mBtnClose = root.Find("Image_Black").GetComponent<Button>();

            mUICross.Load(root.Find("Animator/View_Go"));
            mUISuccess.Load(root.Find("Animator/View_Success"));
            mUIFail.Load(root.Find("Animator/View_Fail"));

            mBtnClose.onClick.AddListener(OnClickClose);

        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;
        }

        public void ShowMode(int mode) // 0 通关 1 胜利 2 失败
        {
            switch (mode)
            {
                case 0:
                    mIResut = mUICross;
                    break;
                case 1:
                    mIResut = mUISuccess;
                    break;
                case 2:
                    mIResut = mUIFail;
                    break;
                default:
                    break;
            }
        }


        public void SetDesc(string tex)
        {
            if (mIResut == null)
                return;
            mIResut.SetDesc(tex);
        }

        public void SetName(string tex)
        {
            if (mIResut == null)
                return;
            mIResut.SetName(tex);
        }

        public void SetActive(bool b)
        {
            if (mIResut == null)
                return;
            mIResut.SetActive(b);
        }
        private void OnClickClose()
        {
            m_Listener?.Close();
        }


        public void SetFristCross(bool active)
        {
           var value =  mIResut as UI_Cross;

            if (value == null)
                return;
            value.SetFristCross(active);
        }

        public void SetFristRewardCount(int count)
        {

            var value = mIResut as UI_Cross;

            if (value == null)
                return;
            value.SetFristRewardCount(count);



        }

        public void SetFristReward(int index, uint id, uint count,Item item)
        {
            var value = mIResut as UI_Cross;

            if (value == null)
                return;

            value.SetFristReward(index, id, count,item);
        }
        public interface IListener
        {
            void Close();
        }
    }
}
