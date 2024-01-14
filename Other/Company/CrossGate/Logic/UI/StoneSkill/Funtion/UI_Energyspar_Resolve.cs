using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Energyspar_Resolve: UIBase
    {
        private Image iconImage;
        private Text nameText;
        private Text levelText;
        private GameObject starGo;
        private Transform starGroup;
        private Transform itemGroup;
        private Button closeBtn;
        private Button resolveBtn;
        private uint currentId;
        private uint[] resolveExpToItem = new uint[2];
        private float ratio;
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_TipsBg02_Middle/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(() => { CloseSelf(); });

            iconImage = transform.Find("Animator/GameObject/Image_SkillIcon").GetComponent<Image>();
            nameText = transform.Find("Animator/GameObject/Name/Text_SkillName").GetComponent<Text>();
            levelText = transform.Find("Animator/GameObject/Text_SkillLevel/Value").GetComponent<Text>();
            starGo = transform.Find("Animator/GameObject/Name/Star_Dark").gameObject;
            starGroup = transform.Find("Animator/GameObject/Name/StarGroup");
            itemGroup = transform.Find("Animator/GameObject/ItemGroup");

            resolveBtn = transform.Find("Animator/GameObject/Button_Resolve").GetComponent<Button>();
            resolveBtn.onClick.AddListener(ResolveBtnClick);
            CSVParam.Data expData = CSVParam.Instance.GetConfData(723);
            if(null != expData)
            {
                string[] _strs1 = expData.str_value.Split('|');
                resolveExpToItem[0] = Convert.ToUInt32(_strs1[0]);
                resolveExpToItem[1] = Convert.ToUInt32(_strs1[1]);
            }
            CSVParam.Data ratioData = CSVParam.Instance.GetConfData(724);
            if (null != ratioData)
            {
                ratio = Convert.ToUInt32(ratioData.str_value) / 10000.0f;
            }

        }

        protected override void OnOpen(object arg)
        {
            currentId = (uint)arg;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            CSVStone.Data stoneData = CSVStone.Instance.GetConfData(currentId);
            if (null != stoneData)
            {
                FrameworkTool.DestroyChildren(starGroup.gameObject);
                ImageHelper.SetIcon(iconImage, stoneData.icon);
                TextHelper.SetText(nameText, stoneData.stone_name);
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(stoneData.id);
                if (null != severData)
                {
                    TextHelper.SetText(levelText, LanguageHelper.GetTextContent(2021030, severData.powerStoneUnit.Level.ToString(), stoneData.max_level.ToString()));
                    uint currentState = severData.powerStoneUnit.Stage;
                    for (int i = 0; i < stoneData.max_stage; i++)
                    {
                        bool isLight = i < currentState;
                        GameObject go = GameObject.Instantiate<GameObject>(starGo, starGroup);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        small_Star.SetState(isLight);
                    }

                    FrameworkTool.DestroyChildren(itemGroup.gameObject);
                    CSVStoneLevel.Data levelData = CSVStoneLevel.Instance.GetConfData(currentId * 1000 + severData.powerStoneUnit.Level);
                    List<ItemIdCount> dropData = new List<ItemIdCount>();
                    if (null != levelData)
                    {
                        dropData = CSVDrop.Instance.GetDropItem(levelData.decompose);                        
                    }

                    CSVStoneStage.Data stageData = CSVStoneStage.Instance.GetConfData(currentId * 1000 + severData.powerStoneUnit.Stage);
                    if (null != stageData)
                    {
                        List<ItemIdCount> temp = CSVDrop.Instance.GetDropItem(stageData.decompose);
                        for (int i = 0; i < temp.Count; i++)
                        {
                            int count = dropData.Count;
                            for (int j = 0; j < count; j++)
                            {
                                if(dropData[j].id == temp[i].id)
                                {
                                    dropData[j].count += temp[i].count;
                                    continue;
                                }
                                else
                                {
                                    if(j == count - 1)
                                    {
                                        dropData.Add(temp[i]);
                                    }
                                }
                            }
                        }
                    }                   

                    uint expToItemNum = (uint)Math.Floor((float)severData.powerStoneUnit.Exp * ratio / (float)resolveExpToItem[0]);
                    if(expToItemNum > 0)
                    {
                        for (int j = 0; j < dropData.Count; j++)
                        {
                            if (dropData[j].id == resolveExpToItem[1])
                            {
                                dropData[j].count += expToItemNum;
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < dropData.Count; i++)
                    {
                        PropIconLoader.GetAsset(new PropIconLoader.ShowItemData(dropData[i].id, dropData[i].count,
                              true, false, false, false, false, _bShowCount: true, _bShowBagCount: false), itemGroup);
                    }

                    FrameworkTool.ForceRebuildLayout(starGo.transform.parent.gameObject);
                }
            }
        }

        private void ResolveBtnClick()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2021054);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_StoneSkill.Instance.OnPowerStoneDecomposeReq(currentId);
                CloseSelf();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
    }
}

