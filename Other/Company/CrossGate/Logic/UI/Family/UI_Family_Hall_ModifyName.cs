using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System.Text;

namespace Logic
{
    /// <summary> 家族修改名称 </summary>
    public class UI_Family_Hall_ModifyName : UIBase
    {
        #region 界面组件
        /// <summary> 输入家族名称 </summary>
        private InputField inputField_Name;
        /// <summary> 道具 </summary>
        private PropItem propItem;
        /// <summary> 货币 </summary>
        private GameObject go_Coin;
        /// <summary> 菜单组 </summary>
        private ToggleGroup toggleGroup_Cost;
        #endregion
        #region 数据定义
        /// <summary> 花费道具 </summary>
        private ItemIdCount[] array_itemIdCount;
        /// <summary> 名称最小字数 </summary>
        private const int minLimit_Name = 4;
        /// <summary> 名称最大字数 </summary>
        private const int maxLimit_Name = 12;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
            SetInitData();
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
        }
        protected override void OnOpened()
        {
            ResetView();
        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnUpdate()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            inputField_Name = transform.Find("Animator/View_Name/InputField_Describe").GetComponent<InputField>();
            GameObject go_Item = transform.Find("Animator/View_Name/Image_Item/Item").gameObject;
            propItem = new PropItem();
            propItem.BindGameObject(go_Item);

            go_Coin = transform.Find("Animator/View_Name/Image_Icon").gameObject;
            toggleGroup_Cost = transform.Find("Animator/View_Name/Toggle_Choice").GetComponent<ToggleGroup>();

            transform.Find("Animator/View_TipsBg02_Smallest/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Name/Button_Go").GetComponent<Button>().onClick.AddListener(OnClick_Modify);

            inputField_Name.characterLimit = 0;
            inputField_Name.onValidateInput = OnValidateInput_Name;
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyName, OnClick_Close, toRegister);
        }
        /// <summary>
        /// 设置初始数据
        /// </summary>
        private void SetInitData()
        {
            array_itemIdCount = new ItemIdCount[2];
            for (int i = 0; i < array_itemIdCount.Length; i++)
            {
                uint itemId = 0;
                long itemCount = 0;
                int id = i == 0 ? 754 : 753;
                string[] val = CSVParam.Instance.GetConfData((uint)id).str_value.Split('|');
                if (val.Length == 2)
                {
                    uint.TryParse(val[0], out itemId);
                    long.TryParse(val[1], out itemCount);
                }
                array_itemIdCount[i] = new ItemIdCount(itemId, itemCount);
            }
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 重置界面
        /// </summary>
        private void ResetView()
        {
            inputField_Name.text = string.Empty;
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetItem();
            SetCoin();
        }
        /// <summary>
        /// 设置道具
        /// </summary>
        private void SetItem()
        {
            ItemIdCount itemIdCount = array_itemIdCount[0];
            CSVItem.Data cSVItemData = itemIdCount.CSV;
            if (null == cSVItemData)
            {
                propItem.SetActive(false);
                return;
            }
            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false, true, true, true, null, false, true);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Family_ModifyName, itemData));
        }
        /// <summary>
        /// 设置货币
        /// </summary>
        private void SetCoin()
        {
            ItemIdCount itemIdCount = array_itemIdCount[1];
            CSVItem.Data cSVItemData = itemIdCount.CSV;
            if (null == cSVItemData)
            {
                go_Coin.SetActive(false);
                return;
            }
            Transform tr = go_Coin.transform;
            /// <summary> 文字花费 </summary>
            Text text_Cost = tr.Find("Text_Cost").GetComponent<Text>();
            if(itemIdCount.CountInBag >= itemIdCount.count)
            {
                text_Cost.text = string.Format("{0}/{1}", itemIdCount.CountInBag.ToString(), itemIdCount.count.ToString());
            }
            else
            {
                text_Cost.text = string.Format(LanguageHelper.GetTextContent(1601000004), itemIdCount.CountInBag.ToString(), itemIdCount.count.ToString());
            }
            /// <summary> 图标花费 </summary>
            Image image_Cost = tr.Find("Text_Cost/Image_Coin").GetComponent<Image>();
            ImageHelper.SetIcon(image_Cost, itemIdCount.CSV.small_icon_id);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            UIManager.HitButton(EUIID.UI_Family_ModifyName, "OnClick_Close");
            CloseSelf();
        }
        /// <summary>
        /// 修改名称
        /// </summary>
        private void OnClick_Modify()
        {
            var list = toggleGroup_Cost.ActiveToggles();
            uint selectType = 0;
            foreach (var toggle in list)
            {
                if (!toggle.isOn) continue;

                switch (toggle.name)
                {
                    case "toggle1":
                        selectType = 0;
                        break;
                    case "toggle2":
                        selectType = 1;
                        break;
                }
            }
            UIManager.HitButton(EUIID.UI_Family_ModifyName, "OnClick_Modify"+ selectType.ToString());
            ItemIdCount itemIdCount = array_itemIdCount[selectType];
            if (!itemIdCount.Enough)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10625));
                return;
            }
            if (TextHelper.GetCharNum(inputField_Name.text) < minLimit_Name)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10021));
                return;
            }
            if (Sys_RoleName.Instance.HasBadNames(inputField_Name.text) || Sys_WordInput.Instance.HasLimitWord(inputField_Name.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }

            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10037, inputField_Name.text);
            PromptBoxParameter.Instance.SetConfirm(true, () => { Sys_Family.Instance.SendGuildChangeNameReq(inputField_Name.text, selectType); });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
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
        #endregion
        #region 提供功能
        #endregion
    }
}
