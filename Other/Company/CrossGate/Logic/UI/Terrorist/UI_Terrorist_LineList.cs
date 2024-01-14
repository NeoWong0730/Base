using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Terrorist_LineList : UIComponent
    {
        #region UI

        private List<UI_Terrorist_LineTemplate> lineList = new List<UI_Terrorist_LineTemplate>();
        #endregion
        
        protected override void Loaded()
        {
            for (int i = 0; i < 3; ++i)
            {
                string str = string.Format("Button_Line{0}", i + 1);
                UI_Terrorist_LineTemplate template = AddComponent<UI_Terrorist_LineTemplate>(transform.Find(str));
                lineList.Add(template);
            }
        }

        public void UpdateLineList(CSVTerrorSeries.Data data)
        {
            for (int i = 0; i < lineList.Count; ++i)
            {
                if (i < data.line_name.Count)
                {
                    lineList[i].UpdateLineInfo(data, i);
                    lineList[i].OnDefaultSelect(false);
                    lineList[i].OnEnableLine(true);
                }
            }


            if (Sys_TerrorSeries.Instance.IsDailyTaskLineSelected(data.id))
            {
                CSVInstance.Data insInfo = CSVInstance.Instance.GetConfData(data.id);
                TerrorDailyTask dailyData = Sys_TerrorSeries.Instance.GetDailyTaskData(insInfo.PlayType);
                if (dailyData != null)
                {
                    int line = (int)dailyData.Line;

                    for (int i = 0; i < lineList.Count; ++i)
                    {
                        if (line == i)
                        {
                            lineList[i].OnDefaultSelect(true);
                        }
                        else
                        {
                            lineList[i].OnEnableLine(false);
                        }
                    }

                    return;
                }
            }

            //默认选中
            lineList[0].OnDefaultSelect(true);
        }
    }
}


