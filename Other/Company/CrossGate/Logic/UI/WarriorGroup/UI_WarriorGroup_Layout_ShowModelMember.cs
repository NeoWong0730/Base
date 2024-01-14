using Table;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_WarriorGroup_Layout
    {
        public class ShoweModelMember
        {
            public Transform transform;

            Transform transReal;
            Transform transEmpty;

            public Text nameText;
            public Image occIcon;
            public Text occText;
            public Text levelText;
            public GameObject titleRoot;
            public Text titleText;
            public Image titleImage;
            public GameObject leaderFlag;

            public Sys_WarriorGroup.WarriorInfo warriorInfo;

            public ShoweModelMember()
            {
                Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefrehedLeader, OnRefrehedLeader, true);
            }

            public void Dispose()
            {
                Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefrehedLeader, OnRefrehedLeader, false);
            }

            public ShoweModelMember Load(Transform transform)
            {
                this.transform = transform;

                transEmpty = transform.Find("empty");
                nameText = transform.gameObject.FindChildByName("Text_Name").GetComponent<Text>();
                occIcon = transform.gameObject.FindChildByName("Image_Prop").GetComponent<Image>();
                occText = transform.gameObject.FindChildByName("Text_Profession").GetComponent<Text>();
                levelText = transform.gameObject.FindChildByName("Text_Level").GetComponent<Text>();
                titleRoot = transform.gameObject.FindChildByName("Title");
                titleText = titleRoot.FindChildByName("Text").GetComponent<Text>();
                titleImage = titleRoot.FindChildByName("Image1").GetComponent<Image>();
                leaderFlag = transform.gameObject.FindChildByName("Text_Posts");

                return this;
            }

            public void Refresh(Sys_WarriorGroup.WarriorInfo warriorInfo)
            {
                if (warriorInfo != null)
                {
                    transform.gameObject.SetActive(true);
                    this.warriorInfo = warriorInfo;

                    TextHelper.SetText(nameText, warriorInfo.RoleName);
                    ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(warriorInfo.Occ).icon);
                    TextHelper.SetText(occText, CSVCareer.Instance.GetConfData(warriorInfo.Occ).name);
                    TextHelper.SetText(levelText, $"等级{warriorInfo.Level}级");
                    SetTitle(warriorInfo.TitleID, warriorInfo.GroupName, warriorInfo.IsLeader);
                    leaderFlag.SetActive(warriorInfo.RoleID == Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID);
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }

            void OnRefrehedLeader()
            {
                if (warriorInfo != null)
                    leaderFlag.SetActive(warriorInfo.RoleID == Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID);
            }

            void SetTitle(uint titleID, string groupName, bool isLeader)
            {
                var titleData = CSVTitle.Instance.GetConfData(titleID);
                string titlestr = string.Empty;
                if (titleData != null && titleData.titleShowLan != 0)
                    titlestr = LanguageHelper.GetTextContent(titleData.titleShowLan);
               
                if (titleData != null)
                {
                    if (titleData.titleGetType == 12 && string.IsNullOrEmpty(groupName) == false)
                    {
                        titlestr += $"{groupName}-";
                        if (isLeader)
                        {
                            titlestr += LanguageHelper.GetTextContent(1002820);
                        }
                        else
                        {
                            titlestr += LanguageHelper.GetTextContent(1002819);
                        }
                        SetTitile(titlestr, titleData == null ? null : titleData.titleShow);
                    }
                    else
                    {
                        CSVWordStyle.Data wordStyle = null;
                        if (CSVLanguage.Instance.TryGetValue(titleData.titleShowLan, out CSVLanguage.Data data))
                        {
                            wordStyle = CSVWordStyle.Instance.GetConfData(data.wordStyle);
                        }
                        SetTitile(titlestr, wordStyle);
                    }
                }

                if (titleData == null || titleData.titleGetType == 9)
                {
                    SetTitileIcon(0);
                }
                else
                {
                    SetTitileIcon(titleData.titleShowIcon);
                }
            }

            void SetTitileIcon(uint iconID)
            {
                if (iconID == 0)
                {
                    titleImage.gameObject.SetActive(false);
                    return;
                }
                if (titleImage.gameObject.activeSelf == false)
                    titleImage.gameObject.SetActive(false);
                ImageHelper.SetIcon(titleImage, iconID);
            }

            void SetTitile(string str, CSVWordStyle.Data wordstyle)
            {
                TextHelper.SetText(titleText, str, wordstyle);
            }

            public void SetTitile(string str, List<Color32> colors)
            {
                titleText.text = str;

                if (colors == null || colors.Count == 0)
                    return;
                TextHelper.SetTextGradient(titleText, colors[0], colors[1]);
                TextHelper.SetTextOutLine(titleText, colors[2]);
            }

            public Vector3 GetPosition()
            {
                return transEmpty.position;
            }
        }
    }
}
