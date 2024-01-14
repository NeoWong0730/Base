using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace Logic
{
    /// <summary> 创建家族子界面 </summary>
    public class UI_ApplyFamily_Create : UIComponent
    {
        #region 界面组件
        /// <summary> 输入家族名称 </summary>
        private InputField inputField_Name;
        /// <summary> 输入家族宣言 </summary>
        private InputField inputField_Declaration;
        /// <summary> 创建家族花费文本 </summary>
        private Text text_Cost;
        /// <summary> 创建家族花费图标 </summary>
        private Image image_Cost;
        /// <summary> 创建家族花费按钮 </summary>
        private Button button_Cost;
        #endregion
        #region 数据定义
        /// <summary> 花费道具 </summary>
        private ItemIdCount itemIdCount = new ItemIdCount();
        /// <summary> 名称最小字数 </summary>
        private const int minLimit_Name = 4;
        /// <summary> 名称最大字数 </summary>
        private const int maxLimit_Name = 12;
        /// <summary> 描述最大字数 </summary>
        private const int maxLimit_Declaration = 100;
        #endregion
        #region 系统函数
        public UI_ApplyFamily_Create()
            : base()
        {
            SetInitData();
        }
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void Show()
        {
            base.Show();
            RefreshView();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            inputField_Name = transform.Find("View_Right/Text_Name/InputField_Describe").GetComponent<InputField>();
            inputField_Declaration = transform.Find("View_Right/Text_Talk/InputField_Describe").GetComponent<InputField>();
            text_Cost = transform.Find("View_Right/Text_Cost").GetComponent<Text>();
            image_Cost = transform.Find("View_Right/Text_Cost/Image_Coin").GetComponent<Image>();
            button_Cost = transform.Find("View_Right/Button_Create").GetComponent<Button>();
            button_Cost.onClick.AddListener(OnClick_CreateFamily);
            if(Sys_Ini.Instance.Get<IniElement_Int>(750, out IniElement_Int value))
            {
                TextHelper.SetText(transform.Find("View_Left/Text_Demand/Text").GetComponent<Text>(), 10603, value.value.ToString());
            }            
            inputField_Name.characterLimit = 0;
            inputField_Name.onValidateInput = OnValidateInput_Name;
            inputField_Declaration.characterLimit = 0;
            inputField_Declaration.onValidateInput = OnValidateInput_Declaration;
        }
        /// <summary>
        /// 设置初始数据
        /// </summary>
        private void SetInitData()
        {
            uint itemId = 0;
            long itemCount = 0;
            string[] val = CSVParam.Instance.GetConfData(755).str_value.Split('|');
            if (val.Length == 2)
            {
                uint.TryParse(val[0], out itemId);
                long.TryParse(val[1], out itemCount);
            }
            itemIdCount.id = itemId;
            itemIdCount.count = (uint)itemCount;
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetCostView();
        }
        /// <summary>
        /// 设置花费界面
        /// </summary>
        private void SetCostView()
        {
            bool isEnough = itemIdCount.Enough;
            ImageHelper.SetIcon(image_Cost, itemIdCount.CSV.icon_id);
            TextHelper.SetText(text_Cost,
             string.Format("{0}/{1}", itemIdCount.CountInBag.ToString(), itemIdCount.count.ToString()),
             CSVWordStyle.Instance.GetConfData(isEnough ? (uint)19 : 20));
            ImageHelper.SetImageGray(button_Cost, !isEnough, true);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 创建家族
        /// </summary>
        private void OnClick_CreateFamily()
        {
            UIManager.HitButton(EUIID.UI_ApplyFamily, "OnClick_CreateFamily");
            bool isEnough = itemIdCount.Enough;
            if (!isEnough)
            {
                Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)itemIdCount.id, itemIdCount.count);
;                //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, itemIdCount.id);
                return;
            }
            if (TextHelper.GetCharNum(inputField_Name.text) < minLimit_Name)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10021));
                return;
            }

            if (Sys_RoleName.Instance.HasBadNames(inputField_Name.text) || Sys_WordInput.Instance.HasLimitWord(inputField_Name.text) || Sys_WordInput.Instance.HasLimitWord(inputField_Declaration.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }

            string strDeclaration = string.IsNullOrEmpty(inputField_Declaration.text) ? LanguageHelper.GetTextContent(10022) : inputField_Declaration.text;
            Sys_Family.Instance.SendGuildCreateReq(inputField_Name.text, strDeclaration);
        }
        /// <summary>
        /// 输入字数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charIndex"></param>
        /// <param name="addedChar"></param>
        /// <returns></returns>
        private char OnValidateInput_Name(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Name)
            {
                return '\0';
            }
            return addedChar;
        }
        /// <summary>
        /// 输入字数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="charIndex"></param>
        /// <param name="addedChar"></param>
        /// <returns></returns>
        private char OnValidateInput_Declaration(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Declaration)
            {
                return '\0';
            }
            return addedChar;
        }
        #endregion
        #region 提供功能
        #endregion
    }
}