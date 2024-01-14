using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using System;
using System.Text;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{
    public class UI_TitleSeriesSelect : UIBase
    {
        private Button closeButton;
        private Button okButton;
        private Transform seriesParent;
        private uint curSelectIndex;
        private TitleSeries titleSeries;
        private List<Toggle> toggles = new List<Toggle>();


        protected override void OnOpen(object arg)
        {
            titleSeries = arg as TitleSeries;
        }


        protected override void OnLoaded()
        {
            seriesParent = transform.Find("Animator/Image_Bg/Grid");
            closeButton = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            okButton = transform.Find("Animator/Image_Bg/GameObject (1)/Btn_01").GetComponent<Button>();

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            closeButton.onClick.AddListener(()=> 
            {
                UIManager.CloseUI(EUIID.UI_TitleSeriesSelect);
            });
            okButton.onClick.AddListener(()=> 
            {
                if (!HasSelectedToggle())
                {
                    string content = CSVLanguage.Instance.GetConfData(2020729).words;
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                if (Sys_Title.Instance.titlePos.Contains(titleSeries.Id) &&Sys_Title.Instance.titlePos[(int)curSelectIndex]!=0)
                {
                    string content = CSVLanguage.Instance.GetConfData(2020747).words;
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                Sys_Title.Instance.TitleSuitChangeReq(titleSeries.Id, curSelectIndex);
                UIManager.CloseUI(EUIID.UI_TitleSeriesSelect);
            });
        }

        protected override void OnShow()
        {
            Refresh();
        }

        private void Refresh()
        {
            ClearData();
            int titlePosCount = Sys_Title.Instance.titlePos.Count;
            FrameworkTool.CreateChildList(seriesParent, titlePosCount);
            for (int i = 0; i < titlePosCount; i++)
            {
                Transform child = seriesParent.GetChild(i);
                Toggle toggle = child.GetComponent<Toggle>();
                toggles.Add(toggle);
                toggle.isOn = false;

                GameObject go1 = child.Find("Equip/Title01").gameObject;
                GameObject go2 = child.Find("Equip/Title02").gameObject;

                if (Sys_Title.Instance.titlePos[i]==0)
                {
                    go1.SetActive(false);
                    go2.SetActive(true);
                    //go2.GetComponent<Text>().text = i.ToString();
                    SetLMText(go2.GetComponent<Text>(), i);
                    TextHelper.SetText(go2.transform.Find("Text_Series").GetComponent<Text>(), 2020731);
                }
                else
                {
                    go1.SetActive(true);
                    go2.SetActive(false);
                    //go1.GetComponent<Text>().text = i.ToString();
                    SetLMText(go1.GetComponent<Text>(), i);
                    Text seriesName = go1.transform.Find("Grid/Text_Series").GetComponent<Text>();
                    Text attrName = go1.transform.Find("Grid/Text_Series/Text_Property").GetComponent<Text>();
                    Text attrNum = go1.transform.Find("Grid/Text_Series/Text_Property/Text").GetComponent<Text>();

                    uint seriesId = Sys_Title.Instance.titlePos[i];
                    CSVTitleSeries.Data cSVTitleSeriesData = CSVTitleSeries.Instance.GetConfData(seriesId);
                    TextHelper.SetText(seriesName, cSVTitleSeriesData.seriesLan);
                    TextHelper.SetText(attrName, CSVAttr.Instance.GetConfData(cSVTitleSeriesData.seriesProperty[0][0]).name);
                    attrNum.text = cSVTitleSeriesData.seriesProperty[0][1].ToString();
                }
            }

            foreach (var item in toggles)
            {
                item.onValueChanged.AddListener(val =>
                {
                    int index = toggles.IndexOf(item);
                    if (val)
                    {
                        curSelectIndex = (uint)index;
                    }
                });
            }
        }


        private void SetLMText(Text text, int i)
        {
            string str = "";
            if (i == 0)
            {
                str = CSVLanguage.Instance.GetConfData(2020791).words;
            }
            else if (i == 1)
            {
                str = CSVLanguage.Instance.GetConfData(2020792).words;
            }
            else if (i == 2)
            {
                str = CSVLanguage.Instance.GetConfData(2020793).words;
            }
            else if (i == 3)
            {
                str = CSVLanguage.Instance.GetConfData(2020794).words;
            }
            else if (i == 4)
            {
                str = CSVLanguage.Instance.GetConfData(2020795).words;
            }
            text.text = str;
        }


        private void ClearData()
        {
            curSelectIndex = 0;
            foreach (var item in toggles)
            {
                item.isOn = false;
                item.onValueChanged.RemoveAllListeners();
            }
            toggles.Clear();
        }

        private bool HasSelectedToggle()
        {
            bool selected = false;
            foreach (var item in toggles)
            {
                if (item.isOn)
                {
                    selected = true;
                    break;
                }
            }
            return selected;
        }
    }
}

