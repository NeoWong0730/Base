using UnityEngine;
using System.Collections;
using Logic.Core;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Pet_BookReview_Tip : UIBase
    {
        public class UI_Pet_BookReviewTipItem : UIComponent
        {
            private Text genusNameText;
            private COWComponent<Text> textVd = new COWComponent<Text>();
            private Transform textGo;
            private Transform textParentGo;
            private List<uint> keys = new List<uint>();
            private List<List<uint>> values = new List<List<uint>>();
            private uint gensId;
            protected override void Loaded()
            {
                textParentGo = transform;
                genusNameText = transform.Find("Title/Text_Title").GetComponent<Text>();
                textGo = transform.Find("Text");                
            }

            public void SetData(uint gensId, Dictionary<uint, List<uint>> attrDics)
            {
                this.gensId = gensId;
                CSVGenus.Data csvGenus = CSVGenus.Instance.GetConfData(gensId);
                if(null != csvGenus)
                {
                    TextHelper.SetText(genusNameText, csvGenus.rale_name);
                }
                keys = new List<uint>(attrDics.Keys);
                values = new List<List<uint>>(attrDics.Values);
                textVd.TryBuildOrRefresh(textGo.gameObject, textParentGo, attrDics.Count, RefreshAttrText);
            }

            private void RefreshAttrText(Text text, int index)
            {
                uint attrId = keys[index];
                List<uint> attrValues = values[index];
                CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrId);
                if (null != attrInfo)
                {
                    CSVGenus.Data csvGenus = CSVGenus.Instance.GetConfData(gensId);
                    if (null != csvGenus)
                    {
                        float values = attrInfo.show_type == 1 ? attrValues[0]: (attrValues[0] / 100.0f);
                        text.text = LanguageHelper.GetTextContent(attrInfo.show_type == 1 ? 10956u : 10957u,
                                 LanguageHelper.GetTextContent(csvGenus.rale_name),
                                 LanguageHelper.GetTextContent(attrInfo.name),
                                 attrInfo.show_type == 1 ? attrValues[0].ToString() : (attrValues[0] / 100.0f).ToString(),
                                 attrInfo.show_type == 1 ? attrValues[1].ToString() : (attrValues[1] / 100.0f).ToString()); 

                    }

                }

            }
        }

        private Button closeBtn;

        private COWVd<UI_Pet_BookReviewTipItem> tipItemVd = new COWVd<UI_Pet_BookReviewTipItem>();
        private GameObject tipItemGo;
        private Transform parentGo;
        Dictionary<uint, Dictionary<uint, List<uint>>> attrdics;
        List<uint> genusId = new List<uint>();
        protected override void OnLoaded()
        {
            tipItemGo = transform.Find("Animator/Scroll View/Viewport/Content/Layout").gameObject;
            parentGo = transform.Find("Animator/Scroll View/Viewport/Content");
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseView);
            
        }

        protected override void OnShowEnd()
        {
            
        }

        public Dictionary<uint, Dictionary<uint, List<uint>>> GetPetGenusAttrValue()
        {
            Dictionary<uint, Dictionary<uint, List<uint>>> reDics = new Dictionary<uint, Dictionary<uint, List<uint>>>();

            var dataList = CSVPetNewLoveUp.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                CSVPetNewLoveUp.Data data = dataList[i];
                bool isActive = Sys_Pet.Instance.IsPetLoveHasEffect(data);
                uint genusId = data.RaceType;
                if (data.RaceType != 0 && null != data.RaceEffec)
                {
                    for (int j = 0; j < data.RaceEffec.Count; j++)
                    {
                        if(data.RaceEffec[j].Count >= 2)
                        {
                            uint attrid = data.RaceEffec[j][0];
                            uint attrValue = data.RaceEffec[j][1];
                            if (reDics.TryGetValue(genusId, out Dictionary<uint, List<uint>> attrDic))
                            {
                                if(attrDic.TryGetValue(attrid, out List<uint> attrlist))
                                {
                                    if(isActive)
                                    {
                                        attrlist[0] += attrValue;
                                    }
                                    attrlist[1] += attrValue;
                                }
                                else
                                {
                                    attrlist = new List<uint>();
                                    if (isActive)
                                    {
                                        attrlist.Add(attrValue);
                                    }
                                    else
                                    {
                                        attrlist.Add(0);
                                    }
                                    attrlist.Add(attrValue);
                                    attrDic.Add(attrid, attrlist);
                                }
                            }
                            else
                            {
                                List<uint> attrList = new List<uint>();
                                if (isActive)
                                {
                                    attrList.Add(attrValue);
                                }
                                else
                                {
                                    attrList.Add(0);
                                }
                                attrList.Add(attrValue);
                                attrDic = new Dictionary<uint, List<uint>>();
                                attrDic.Add(attrid, attrList);
                                reDics.Add(genusId, attrDic);
                            }
                        }
                    }                   
                }
            }

            
            return reDics;
        }

        protected override void OnShow()
        {
            attrdics = GetPetGenusAttrValue();
            genusId = new List<uint>(attrdics.Keys);
            SetView();
        }

        private void SetView()
        {
            if(null != attrdics)
            {
                tipItemVd.TryBuildOrRefresh(tipItemGo, parentGo, attrdics.Count, RefreshGenusView);
            }
        }

        private void RefreshGenusView(UI_Pet_BookReviewTipItem items, int index)
        {
            uint genid = genusId[index];
            if(attrdics.TryGetValue(genid, out Dictionary<uint, List<uint>> dic))
            {
                items.SetData(genid, dic);
            }
        }

        private void CloseView()
        {
            CloseSelf();
        }
    }
}

