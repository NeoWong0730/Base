using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;

namespace Logic
{
    /// <summary>
    /// 创建勇者团UI布局///
    /// </summary>
    public class UI_CreateWarriorGroup_Layout
    {
        public Transform transform;

        public Button closeButton;
        public InputField groupNameInput;
        public Button createButton;

        public PropItem costItem;

        public const int minLimit_Name = 4;
        public const int maxLimit_Name = 16;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            groupNameInput = transform.gameObject.FindChildByName("InputField").GetComponent<InputField>();
            groupNameInput.characterLimit = 0;
            groupNameInput.onValidateInput = OnValidateInput_Name;
            createButton = transform.gameObject.FindChildByName("Btn_01").GetComponent<Button>();

            costItem = new PropItem();
            costItem.BindGameObject(transform.gameObject.FindChildByName("PropItem"));
        }

        char OnValidateInput_Name(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Name)
            {
                return '\0';
            }
            return addedChar;
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            createButton.onClick.AddListener(listener.onClickCreateButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void onClickCreateButton();
        }
    }

    /// <summary>
    /// 创建勇士团界面///
    /// </summary>
    public class UI_CreateWarriorGroup : UIBase, UI_CreateWarriorGroup_Layout.IListener
    {
        UI_CreateWarriorGroup_Layout layout = new UI_CreateWarriorGroup_Layout();

        PropIconLoader.ShowItemData showItemData;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);         
        }

        protected override void OnShow()
        {
            layout.groupNameInput.text = string.Empty;
            SetCostItemData();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.CreatedSuccess, CreateSuccessCallBack, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
            Sys_Map.Instance.eventEmitter.Handle<uint, uint>(Sys_Map.EEvents.OnChangeMap, OnChangeMap, toRegister);
        }

        void OnChangeMap(uint lastMapID, uint newMapID)
        {
            CloseSelf();
        }

        void OnRefreshChangeData(int changeType, int curBoxId)
        {
            SetCostItemData();
        }

        void SetCostItemData()
        {
            uint itemID = uint.Parse(CSVParam.Instance.GetConfData(1371).str_value);
            showItemData = new PropIconLoader.ShowItemData(itemID, 1, true, false, false, false, false, true, true, true, null);
            layout.costItem.SetData(showItemData, EUIID.UI_CreateWarriorGroup);
        }

        /// <summary>
        /// 点击了关闭按钮///
        /// </summary>
        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_CreateWarriorGroup);
        }

        /// <summary>
        /// 点击了创建按钮///
        /// </summary>
        public void onClickCreateButton()
        {
            if (string.IsNullOrEmpty(layout.groupNameInput.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13557));
                return;
            }

            if (TextHelper.GetCharNum(layout.groupNameInput.text) < UI_CreateWarriorGroup_Layout.minLimit_Name || TextHelper.GetCharNum(layout.groupNameInput.text) > UI_CreateWarriorGroup_Layout.maxLimit_Name)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13544));
                return;
            }

            if (Sys_RoleName.Instance.HasBadNames(layout.groupNameInput.text) || Sys_WordInput.Instance.HasLimitWord(layout.groupNameInput.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                return;
            }

            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13539, layout.groupNameInput.text);
            PromptBoxParameter.Instance.SetConfirm(true, () => 
            {
                Sys_WarriorGroup.Instance.ReqCreateWarriorGroup(layout.groupNameInput.text);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }

        /// <summary>
        /// 创建勇士团成功回调///
        /// </summary>
        void CreateSuccessCallBack()
        {
            UIManager.CloseUI(EUIID.UI_CreateWarriorGroup);
            UIManager.OpenUI(EUIID.UI_WarriorGroup);
        }
    }
}
