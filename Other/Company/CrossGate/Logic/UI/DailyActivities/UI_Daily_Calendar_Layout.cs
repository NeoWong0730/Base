using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
namespace Logic
{
    public partial class UI_Daily_Calendar_Layout
    {
        class RankItem : IntClickItem
        {
            List<RankChildItem> rankChildItems = new List<RankChildItem>();
            public override void Load(Transform root)
            {
                base.Load(root);

                Transform grids = root.Find("TitleGrid");

                int count = grids.childCount;

                for (int i = 0; i < count; i++)
                {
                    var childTrans = root.Find("TitleGrid/Image_Title_" + i);

                    if (childTrans == null)
                        continue;

                    var value = new RankChildItem();

                    value.Load(childTrans);

                    rankChildItems.Add(value);

                    value.Index = i;

                    value.clickItemEvent.AddListener(OnClickChildItem);

                }
            }

            public override ClickItem Clone()
            {
                return Clone<RankItem>(this);
            }
            public void SetChild(int index, string tex, uint configID)
            {
                if (rankChildItems.Count <= index)
                    return;

                rankChildItems[index].SetName(tex);
                rankChildItems[index].ConfigID = configID;
            }

            private void OnClickChildItem(int index)
            {
                if (rankChildItems.Count <= index)
                    return;

                clickItemEvent.Invoke((int)rankChildItems[index].ConfigID);
            }

            public void SetChildTexColor(int index, Color color)
            {
                if (rankChildItems.Count <= index)
                    return;

                rankChildItems[index].SetTexColor(color);
            }

            public void SetChildImageColor(int index, Color color)
            {
                if (rankChildItems.Count <= index)
                    return;

                rankChildItems[index].SetImageColor(color);
            }

            public void SetChildImageSelect(int index, bool isstate)
            {
                if (rankChildItems.Count <= index)
                    return;

                rankChildItems[index].SetImageSelect(isstate);
            }
        }
    }

    public partial class UI_Daily_Calendar_Layout
    {
        class RankChildItem : ButtonIntClickItem
        {
            private Text mTexName;
            private Image mImage;

            private Image mSelectImage;
            public uint ConfigID { get; set; }
            public override void Load(Transform root)
            {
                base.Load(root);

                mTexName = root.Find("Text").GetComponent<Text>();

                mImage = root.GetComponent<Image>();

                mSelectImage = root.Find("Image_Select").GetComponent<Image>();
            }

            public void SetName(string tex)
            {
                mTexName.text = tex;
            }

            public void SetTexColor(Color color)
            {
                mTexName.color = color;
            }

            public void SetImageColor(Color color)
            {
                mImage.color = color;
            }

            public void SetImageSelect(bool state)
            {
                mSelectImage.gameObject.SetActive(state);
            }
        }
    }

    public partial class UI_Daily_Calendar_Layout
    {
        class TitleItem
        {
            private Image mImBack;
            private Text mTexText;
            private Image mImTrangle;

            private Transform mTransSelect;
            private Transform mTransSelectArrow;
            public TitleItem(Transform root)
            {
                Load(root);
            }

            public void Load(Transform root)
            {
                mImBack = root.GetComponent<Image>();
                mTexText = root.Find("Text").GetComponent<Text>();
                mImTrangle = root.Find("Image_Arrow").GetComponent<Image>();
                mTransSelect = root.Find("Image_Select");
                mTransSelectArrow = root.Find("Image_ArrowSelect");
            }

            public void SetSelectState(bool active)
            {
                mTransSelect.gameObject.SetActive(active);
                mTransSelectArrow.gameObject.SetActive(active);
            }
            public void SetBackImageColor(Color color)
            {
                mImBack.color = color;
            }

            private void SetTextColor(Color color)
            {
                mTexText.color = color;
            }

            private void SetArrowColor(Color color)
            {
                mImTrangle.color = color;
            }
        }
    }
    public partial class UI_Daily_Calendar_Layout
    {
        private ClickItemGroup<RankItem> mGroup;
        private Button mBtnClose;

        private IListener m_Listener;

        private List<Transform> mSelectFocus = new List<Transform>();

        private List<TitleItem> mTitles = new List<TitleItem>();
        public void Load(Transform root)
        {
            Transform rankitem = root.Find("Animator/View_Message/ScrollView_Rank/TabList/RankItem");

            mGroup = new ClickItemGroup<RankItem>(rankitem);

            mBtnClose = root.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>();

            mBtnClose.onClick.AddListener(OnClickCloseBtn);

            mGroup.SetAddChildListenter(AddRandItem);

            Transform selectParent = root.Find("Animator/View_Message/SelectGrid");

            int count = selectParent.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform child = selectParent.Find("Day_Select_" + i);

                if (child != null)
                    mSelectFocus.Add(child);
            }

            Transform titleParent = root.Find("Animator/View_Message/TitleGrid");

            int titlecount = titleParent.childCount;

            for (int i = 0; i < titlecount; i++)
            {
                Transform item = titleParent.Find("Image_Title" + i);

                var iitem = new TitleItem(item);

                mTitles.Add(iitem);
            }
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;
        }

        private void OnClickCloseBtn()
        {
            m_Listener?.OnClickClose();
        }

        public interface IListener
        {
            void OnClickClose();

            void OnClickRandChild(uint id);
        }



        public void SelectDay(int day)
        {
            int count = mSelectFocus.Count;

            for (int i = 0; i < count; i++)
            {
                mSelectFocus[i].gameObject.SetActive(i == (day - 1));
            }
        }
    }

    public partial class UI_Daily_Calendar_Layout
    {
        private void AddRandItem(RankItem item)
        {
            item.clickItemEvent.AddListener(OnClickRankChild);
        }

        private void OnClickRankChild(int id)
        {
            m_Listener?.OnClickRandChild((uint)id);
        }
        public void SetSize(int count)
        {
            mGroup.SetChildSize(count);
        }
        public void SetItem(int index, int childIndex, uint langue, uint configID)
        {
            string strname = langue == 0 ? string.Empty : LanguageHelper.GetTextContent(langue);
            SetItem(index,childIndex,strname ,configID);
        }

        public void SetItem(int index, int childIndex, string langue, uint configID)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            item.SetChild(childIndex, langue, configID);
        }

        public void SetItemState(int index, int childIndex, bool isToday)
        {
            var item = mGroup.getAt(index);

            if (item == null)
                return;

            int value0 = index % 2;

            // uint imageParam = isToday ? (uint)(value0 == 0 ? 580 : 582) : (uint)(value0 == 0 ? 579 : 581);

            item.SetChildImageSelect(childIndex,isToday);

            uint textParam = (uint)(isToday ? 584:583);

            //var imageData = CSVParam.Instance.GetConfData(imageParam);
            //string[] colorStr = imageData.str_value.Split('|');
            //if(colorStr.Length == 3)
            //item.SetChildImageColor(childIndex,new Color() { r = int.Parse(colorStr[0])/255f, g = int.Parse(colorStr[1]) / 255f, b = int.Parse(colorStr[2]) / 255f,a = 1 });

            var textData = CSVParam.Instance.GetConfData(textParam);
            string[] colortextStr = textData.str_value.Split('|');
            item.SetChildTexColor(childIndex, new Color() { r = int.Parse(colortextStr[0])/ 255f, g = int.Parse(colortextStr[1]) / 255f, b = int.Parse(colortextStr[2] ) / 255f,a= 1 });
        }

        public void SetTitleState(int day)
        {
            int count = mTitles.Count;

            for (int i = 0; i < count; i++)
            {
                var item = mTitles[i];

                item.SetSelectState(i == day);

                //uint imageParam =(uint)( day == i ? 578:577);

                //var imageData = CSVParam.Instance.GetConfData(imageParam);
                //string[] colorStr = imageData.str_value.Split('|');

                //item.SetBackImageColor(new Color() { r = int.Parse(colorStr[0]) / 255f, g = int.Parse(colorStr[1]) / 255f, b = int.Parse(colorStr[2]) / 255f, a = 1 });
            }
        }
    }
}
