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
    public enum EPetBuildList : int
    {
        Success = 0,
        Luck = 1,
        Grade = 2,
        Skill = 3,
    }

    public class UI_Pet_List : UIBase
    {
        public class UI_Pet_ListTitle
        {
            public Transform transform;
            public Text titleText;
            public void Init(Transform transform)
            {
                this.transform = transform;
                titleText = transform.Find("Text").GetComponent<Text>();
            }

            public void SetText(string text)
            {
                TextHelper.SetText(titleText, text);
            }

            public void SetActive(bool state)
            {
                transform.gameObject.SetActive(state);
            }
        }

        public class UI_Pet_ListLine
        {
            private uint itemId;
            public Transform transform;
            public Text ItemName;
            List<UI_Pet_ListTitle> titleList = new List<UI_Pet_ListTitle>();
            public void Init(Transform transform)
            {
                this.transform = transform;

                ItemName = transform.Find("Image_Item/Text").GetComponent<Text>();

                int count = CSVPetNewReBuild.Instance.Count;
                for (int i = 0; i < count; i++)
                {
                    UI_Pet_ListTitle title = new UI_Pet_ListTitle();
                    Transform tran = transform.Find($"Image{i}");
                    if (null == tran)
                    {
                        GameObject go = GameObject.Instantiate(transform.Find("Image0").gameObject, transform);
                        go.name = $"Image{i}";
                        go.transform.SetSiblingIndex(transform.childCount - 2);
                        title.Init(go.transform);
                    }
                    else
                    {
                        title.Init(tran);
                    }
                    titleList.Add(title);
                }
            }

            public void SetItemText(uint itemId)
            {
                this.itemId = itemId;
                CSVItem.Data item = CSVItem.Instance.GetConfData(itemId);
                if (null != item)
                {
                    TextHelper.SetText(ItemName, item.name_id);
                }
            }

            public void SetSkillValue(uint remakeTime)
            {
                CSVPetRemake.Data remakeData = CSVPetRemake.Instance.GetConfData(itemId * 100 + remakeTime);
                int count = 0;
                int weight = 0;
                List<uint> weight_sub = new List<uint>();
                if (null != remakeData)
                {
                    count = remakeData.skill_weight.Count;
                    for (int i = 0; i < remakeData.skill_weight.Count; i++)
                    {
                        weight += (int)remakeData.skill_weight[i];
                        weight_sub.Add(remakeData.skill_weight[i]);
                    }

                    for (int i = 0; i < titleList.Count; i++)
                    {
                        UI_Pet_ListTitle tl = titleList[i];
                        if (i < count)
                        {
                            float value = (weight_sub[i] + 0f) / weight;
                            if (value > 1)
                            {
                                value = 1;
                            }
                            string text = (value * 100).ToString("0.#") + "%";
                            tl.SetText(text);
                        }
                        tl.SetActive(i < count);
                    }
                }
                else
                {
                    for (int i = 0; i < titleList.Count; i++)
                    {
                        UI_Pet_ListTitle tl = titleList[i];
                        if (i < 4)
                        {
                            tl.SetText("/");
                        }
                        tl.SetActive(i < 4);
                    }
                }
                
            }

            public void SetGradeValue(List<uint> times)
            {
                int count = times.Count;
                for (int i = 0; i < titleList.Count; i++)
                {
                    UI_Pet_ListTitle tl = titleList[i];
                    if (i < count)
                    {
                        CSVPetRemake.Data remakeData = CSVPetRemake.Instance.GetConfData(itemId * 100 + times[i]);
                        if(null != remakeData)
                        {
                            string text = remakeData.add_attr_rate.ToString() + "%";
                            tl.SetText(text);
                        }
                        else
                        {
                            tl.SetText("/");
                        }
                        
                    }
                    tl.SetActive(i < count);
                }
            }


        }
        private Button closeBtn;
        private CP_ToggleRegistry CP_ToggleRegistryTab;
        private CP_ToggleRegistry CP_SkillToggleRegistryTab;
        private List<uint> remakeTimes;
        private List<uint> remakeBookIds =new List<uint>(5);
        private Text titleText;
        private GameObject gradeTipsGo;
        private List<UI_Pet_ListTitle> titleList = new List<UI_Pet_ListTitle>();
        private List<UI_Pet_ListLine> lineList = new List<UI_Pet_ListLine>();

        private EPetBuildList type = EPetBuildList.Grade;
        private const uint MaxSkillNumIndex = 4; // 0-3
        private uint remakeTime;
        protected override void OnLoaded()
        {
            var dataList = CSVPetNewReBuild.Instance.GetAll();
            remakeTimes = new List<uint>(dataList.Count);
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                remakeTimes.Add(dataList[i].id);
            }
            int count = CSVPetRemake.Instance.Count;

            for (int i = 0; i < CSVPetRemake.Instance.Count; i++)
            {
                var rebuildInfo = CSVPetRemake.Instance.GetByIndex(i);
                if (null != rebuildInfo && !remakeBookIds.Contains(rebuildInfo.item_id))
                {
                    remakeBookIds.Add(rebuildInfo.item_id);
                }
            }

            closeBtn = transform.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseView);
            CP_ToggleRegistryTab = transform.Find("Animator/Menu/Toggle_Group").GetComponent<CP_ToggleRegistry>();
            CP_ToggleRegistryTab.onToggleChange = OnTabChanged;

            CP_SkillToggleRegistryTab = transform.Find("Animator/Center/Toggles").GetComponent<CP_ToggleRegistry>();
            CP_SkillToggleRegistryTab.onToggleChange = OnSkillTabChanged;

            for (int i = 0; i < remakeTimes.Count; i++)
            {
                Transform tra = CP_SkillToggleRegistryTab.transform.Find($"Toggle{i}");
                CP_Toggle toggle = null;
                if (null != tra)
                {
                    toggle = tra.GetComponent<CP_Toggle>();
                }
                else
                {
                    tra = GameObject.Instantiate<GameObject>(CP_SkillToggleRegistryTab.transform.GetChild(0).gameObject, CP_SkillToggleRegistryTab.transform).transform;
                    toggle = tra.GetComponent<CP_Toggle>();
                    tra.name = "Toggle" + i.ToString();
                }
                TextHelper.SetText(tra.Find("Text").GetComponent<Text>(), LanguageHelper.GetTextContent(10780 + remakeTimes[i]));
                TextHelper.SetText(tra.Find("Checkmark/LightText").GetComponent<Text>(), LanguageHelper.GetTextContent(10780 + remakeTimes[i]));
                toggle.id = (int)remakeTimes[i];
            }

            count = Enum.GetValues(typeof(EPetBuildList)).Length;
            for (int i = 0; i < count; i++)
            {
                CP_Toggle toggle = CP_ToggleRegistryTab.transform.Find($"Toggle{i}").GetComponent<CP_Toggle>();
                toggle.id = i;
            }            

            titleText = transform.Find("Animator/Center/Text_Tips").GetComponent<Text>();
            gradeTipsGo = transform.Find("Animator/Center/Text_Tips_1").gameObject;
            count = CSVPetNewReBuild.Instance.Count;
            Transform paran = transform.Find($"Animator/Center/List/Layout_Title");
            for (int i = 0; i < count; i++)
            {
                UI_Pet_ListTitle title = new UI_Pet_ListTitle();
                Transform tran = transform.Find($"Animator/Center/List/Layout_Title/Image{i}");
                if(null == tran)
                {
                    GameObject go = GameObject.Instantiate(transform.Find($"Animator/Center/List/Layout_Title/Image0").gameObject, paran);
                    go.name = $"Image{i}";
                    go.transform.SetSiblingIndex(paran.childCount - 2);
                    title.Init(go.transform);
                }
                else
                {
                    title.Init(tran);
                }
                
                titleList.Add(title);
            }
            Transform lineParent = transform.Find("Animator/Center/List/Scroll View/Viewport/Content");
            count = remakeBookIds.Count;
            FrameworkTool.CreateChildList(lineParent, count);
            for (int i = 0; i < count; i++)
            {
                UI_Pet_ListLine line = new UI_Pet_ListLine();
                line.Init(lineParent.GetChild(i));
                line.SetItemText(remakeBookIds[i]);
                lineList.Add(line);
            }
        }

        protected override void OnShow()
        {
            type = EPetBuildList.Grade;
            CP_ToggleRegistryTab?.SwitchTo(2);
        }

        private void OnSkillTabChanged(int curToggle, int old)
        {
            if (type == EPetBuildList.Skill)
            {
                remakeTime = (uint)curToggle;
                SetSkill();
            }
        }
        
        private void OnTabChanged(int curToggle, int old)
        {
            TextHelper.SetText(titleText, (uint)curToggle + 10777u);
            type = (EPetBuildList)curToggle;
            SetToggleTipsState(type == EPetBuildList.Skill);
            if (type == EPetBuildList.Grade)
            {
                SetGrade();
            }
            else if (type == EPetBuildList.Skill)
            {
                remakeTime = remakeTimes[0];
                CP_SkillToggleRegistryTab?.SwitchTo((int)remakeTime);
            }
            SetTitle();
        }

        private void SetToggleTipsState(bool state)
        {
            gradeTipsGo.SetActive(!state);
            CP_SkillToggleRegistryTab.gameObject.SetActive(state);
        }

        private void SetTitle()
        {
            uint baseId = 10809;
            if (type == EPetBuildList.Grade)
            {
                baseId = 10799;
                for (int i = 0; i < titleList.Count; i++)
                {
                    UI_Pet_ListTitle tl = titleList[i];
                    if (i < remakeTimes.Count)
                    {
                        string langText = LanguageHelper.GetTextContent(baseId + (uint)i);
                        tl.SetText(langText);
                    }
                    tl.SetActive(i < remakeTimes.Count);
                }
            }
            else if(type == EPetBuildList.Skill)
            {
                baseId = 10809;
                for (int i = 0; i < titleList.Count; i++)
                {
                    UI_Pet_ListTitle tl = titleList[i];
                    if (i < MaxSkillNumIndex)
                    {
                        string langText = LanguageHelper.GetTextContent(baseId + (uint)i);
                        tl.SetText(langText);
                    }
                    tl.SetActive(i < MaxSkillNumIndex);
                }
            }
            
        }

        private void CloseView()
        {
            CloseSelf();
        }

        private void SetGrade()
        {
            for (int i = 0; i < lineList.Count; i++)
            {
                lineList[i].SetGradeValue(remakeTimes);
            }
        }

        private void SetSkill()
        {
            for (int i = 0; i < lineList.Count; i++)
            {
                lineList[i].SetSkillValue(remakeTime);
            }
        }
    }
}

