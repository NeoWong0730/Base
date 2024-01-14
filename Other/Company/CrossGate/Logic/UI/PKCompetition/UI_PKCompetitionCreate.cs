using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{

    public class UI_PKCompetitionCreate : UIBase
    {
        #region 界面组件
        /// <summary> 创建战队等级 赛组Tips </summary>
        private Text t_Tips;
        /// <summary> 输入战队名字 宣言  </summary>
        private InputField inputField_Name, inputField_Declaration;
        private Text text_Cost;
        private Image image_Cost;
        private Button btn_CreateFightTeam;
        private Button btn_Exit;
        #endregion

        #region 数据定义
        /// <summary> 消耗货币 </summary>
        private ItemIdCount itemIdCount;
        /// <summary> 战队名字最小字数 </summary>
        private const int minLimit_Name = 4;
        /// <summary> 战队名字最大字数 </summary>
        private const int maxLimit_Name = 12;
        /// <summary> 宣言最大字数 </summary>
        private const int maxLimit_Declaration = 100;

        #endregion

        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
            InitData();
        }

        protected override void OnShow()
        {
            ShowInfo();
        }
        #endregion

        #region 初始化
        private void OnParseComponent()
        {
            t_Tips= transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Text_Tips").GetComponent<Text>();
            inputField_Name = transform.Find("Animator/UI_Activity_Message/Animator/View_Content/InputField_Describe1").GetComponent<InputField>();
            inputField_Declaration = transform.Find("Animator/UI_Activity_Message/Animator/View_Content/InputField_Describe2").GetComponent<InputField>();
            text_Cost = transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Cost_Coin/Text_Cost").GetComponent<Text>();
            image_Cost = transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Cost_Coin").GetComponent<Image>();
            btn_CreateFightTeam= transform.Find("Animator/UI_Activity_Message/Animator/View_Content/Btn_01").GetComponent<Button>();
            btn_Exit = transform.Find("Animator/UI_Activity_Message/Animator/Image_Bg/BtnExit").GetComponent<Button>();
            btn_CreateFightTeam.onClick.AddListener(OnClick_CreateFightTeam);
            btn_Exit.onClick.AddListener(() => { CloseSelf(); });

            inputField_Name.characterLimit = 0;
            inputField_Name.onValidateInput = OnValidateInput_Name;
            inputField_Declaration.characterLimit = 0;
            inputField_Declaration.onValidateInput = OnValidateInput_Declaration;
        }

        private void InitData()
        {
            uint id = Sys_PKCompetition.Instance.MatchID;
            uint itemId = CSVPKMatch.Instance.GetConfData(id).priceType;
            long itemCount = CSVPKMatch.Instance.GetConfData(id).price;
            itemIdCount = new ItemIdCount(itemId, itemCount);
        }

        #endregion

        private void ShowInfo()
        {
            if (itemIdCount != null)
            {
                bool isEnough = itemIdCount.Enough;
                ImageHelper.SetIcon(image_Cost, itemIdCount.CSV.icon_id);
                TextHelper.SetText(text_Cost, string.Format("{0}/{1}", Sys_Bag.Instance.GetValueFormat(itemIdCount.CountInBag), itemIdCount.count.ToString()), 
                    CSVWordStyle.Instance.GetConfData(isEnough ? (uint)19 : 20));
                ImageHelper.SetImageGray(btn_CreateFightTeam, !isEnough, true);
            }
            string str = null;
            int Lv = Sys_PKCompetition.Instance.IsGetLevelLimite();
            if (Lv == 1)
                str = LanguageHelper.GetTextContent(14006);
            else if (Lv == 2)
                str = LanguageHelper.GetTextContent(14007);
            else if (Lv == 3)
                str = LanguageHelper.GetTextContent(14008);
            t_Tips.text = LanguageHelper.GetTextContent(1001008, Sys_Role.Instance.Role.Level.ToString(), str);
        }

        #region 事件
        private char OnValidateInput_Name(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Name)
            {
                return '\0';
            }
            return addedChar;
        }

        private char OnValidateInput_Declaration(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Declaration)
            {
                return '\0';
            }
            return addedChar;
        }

        private void OnClick_CreateFightTeam()
        {
            if (itemIdCount != null && !itemIdCount.Enough)
                return;
            if (TextHelper.GetCharNum(inputField_Name.text) < minLimit_Name)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001017));
                return;
            }
            if (Sys_RoleName.Instance.HasBadNames(inputField_Name.text) || Sys_WordInput.Instance.HasLimitWord(inputField_Name.text) || Sys_WordInput.Instance.HasLimitWord(inputField_Declaration.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001018));
                return;
            }
            Sys_PKCompetition.Instance.OpenSureTip(LanguageHelper.GetTextContent(1001022, itemIdCount.count.ToString()), CreateFightTeam);          
        }

        private void CreateFightTeam()
        {
            Sys_PKCompetition.Instance.Req_CreateFightTeam(inputField_Name.text, inputField_Declaration.text);
        }
        #endregion

    }
}