using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Terrorist_LineInfo : UIComponent
    {
        #region LineNode
        private class LineNode
        {
            private int m_Index;

            private Text text;
            private Image imgStart;
            private Image imgCompelete;
            
            private Transform lineTrans;


            public LineNode(GameObject go, int index)
            {
                m_Index = index;

                Transform trans = go.transform;
                text = trans.Find("Text").GetComponent<Text>();
                imgStart = trans.Find("Image_select").GetComponent<Image>();
                imgCompelete = trans.Find("Image_complete").GetComponent<Image>();

                lineTrans = trans.Find("Line_select");
                if (lineTrans != null)
                    lineTrans.gameObject.SetActive(false);

                text.text = LanguageHelper.GetTextContent(1006049, (m_Index + 1).ToString());
            }

            public void UpdateState(CSVTerrorSeries.Data data, int line, bool isLineSelected)
            {
                if (lineTrans != null)
                    lineTrans.gameObject.SetActive(isLineSelected);

                //imgStart.gameObject.SetActive(false);
                imgCompelete.gameObject.SetActive(false);

                imgStart.gameObject.SetActive(Sys_TerrorSeries.Instance.IsDailyTaskStageOnGoing(data.id, (uint)line, (uint)m_Index));

                if (isLineSelected)
                {
                    imgCompelete.gameObject.SetActive(Sys_TerrorSeries.Instance.IsDailyTaskStageComplete(data.id, (uint)m_Index));
                }
                    
            }
        }
        #endregion

        #region 收益
        private class UIEarn
        {
            public Text TextInfo;
            public Text TextName;
            public Image ImgRate;
            public Button btnItem;

            private uint ItemId;

            public void UpdateInfo(uint itemId)
            {
                this.ItemId = itemId;
                btnItem.onClick.AddListener(OnClickItem);
            }

            private void OnClickItem()
            {
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = new ItemData(0, 99, this.ItemId, 1, 0, false, false, null, null, 0, null);
                propParam.showBtnCheck = false;
                propParam.sourceUiId = EUIID.UI_Terrorist;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }
        }
        #endregion

        #region UI
        private Text m_Title;
        private Text m_TaskName;
        private Text m_TaskInfo;
        #endregion

        private List<UIEarn> listEarn = new List<UIEarn>();


        private List<LineNode> lineCommons = new List<LineNode>();
        private Dictionary<int, List<LineNode>> lineDic = new Dictionary<int, List<LineNode>>();

        private CSVTerrorSeries.Data m_TerrorData;
        //private uint m_TaskId;

        private readonly int ComCount = 3;
        private readonly int LineCount = 2;        

        protected override void Loaded()
        {
            m_Title = transform.Find("Image_title/Text_title").GetComponent<Text>();
            m_TaskName = transform.Find("Image_title/content/Title/Text_Title").GetComponent<Text>();
            m_TaskInfo = transform.Find("Image_title/content/Content/Text_Content").GetComponent<Text>();

            listEarn.Clear();
            for (int i = 0; i < 3; ++i)
            {
                string imgStr = string.Format("Image_proptitle/Image_ringbg/Img_ring{0}", i);
                string txtStr = string.Format("Image_proptitle/Text_rate{0}", i);
                string btnStr = string.Format("Image_proptitle/Image_rate{0}", i);

                UIEarn earn = new UIEarn();
                earn.ImgRate = transform.Find(imgStr).GetComponent<Image>();
                earn.TextInfo = transform.Find(txtStr).GetComponent<Text>();
                earn.TextName = earn.TextInfo.transform.Find("Text").GetComponent<Text>();
                earn.btnItem = transform.Find(btnStr).GetComponent<Button>();

                listEarn.Add(earn);
            }

            //nodes commom
            lineCommons.Clear();
            for (int i = 0; i < ComCount; ++i)
            {
                string img = string.Format("Image_map/Image_point{0}", i);
                LineNode node = new LineNode(transform.Find(img).gameObject, i);
                lineCommons.Add(node);
            }

            //line dict
            for (int i = 0; i < 3; ++i)
            {
                int index = i * LineCount + ComCount;
                for (int j = 0; j < LineCount; ++j)
                {
                    string img = string.Format("Image_map/Image_point{0}", index + j);

                    LineNode node = new LineNode(transform.Find(img).gameObject, ComCount + j);

                    if (!lineDic.ContainsKey(i))
                    {
                        lineDic.Add(i, new List<LineNode>());
                    }
                    lineDic[i].Add(node);
                }
            }
        }

        public void UpdateInfo(CSVTerrorSeries.Data data, int line)
        {
            m_TerrorData = data;
            //m_TaskId = taskId;

            m_Title.text = LanguageHelper.GetTextContent(data.line_name[line]);
            m_TaskName.text = LanguageHelper.GetTextContent(data.task_name[line]);
            m_TaskInfo.text = LanguageHelper.GetTextContent(data.task_des[line]);

            List<uint> rateArray = data.award_chance[line];
            uint rate = 0;
            for (int i = 0; i < listEarn.Count; ++i)
            {
                uint itemId = data.award_item[i];
                rate += rateArray[i]; //为了控制显示

                CSVItem.Data itemInfo = CSVItem.Instance.GetConfData(itemId);
                if (itemInfo != null)
                {
                    listEarn[i].TextInfo.text = LanguageHelper.GetTextContent(1006048, rateArray[i].ToString());
                    listEarn[i].TextName.text = LanguageHelper.GetTextContent(1006074, LanguageHelper.GetTextContent(itemInfo.name_id));
                }

                listEarn[i].ImgRate.fillAmount = rate / 100.0f;

                listEarn[i].UpdateInfo(itemId);
            }

            //设置node state
            foreach (LineNode node in lineCommons)
            {
                node.UpdateState(data, line, true);
            }

            foreach (var lineData in lineDic)
            {
                foreach (var node in lineData.Value)
                {
                    node.UpdateState(data, lineData.Key, lineData.Key == line);
                }
            }
        }
    }
}


