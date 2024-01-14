using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_SetKeyCode: UIComponent
    {
        List<HotKeyItemCeil> hotKeyItemList = new List<HotKeyItemCeil>();
        Transform viewPort;
        int visualGridCount;
        bool isRefreshCeil = false;  //Esc系统按键与设置有冲突，强制刷新一次
        protected override void Loaded()
        {
            viewPort = transform.Find("ScrollView_rejian/Viewport");
        }
        public override void Show()
        {
            ShowHotKeyList();
            base.Show();
        }
        public override void Hide()
        {
            Sys_SettingHotKey.Instance.CurrentFocused = -1;
            Sys_SettingHotKey.Instance.Write(Sys_Role.Instance.Role.RoleId.ToString(), "HotKey");
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_SettingHotKey.Instance.eventEmitter.Handle<int>(Sys_SettingHotKey.Events.UpdateKeyCode, UpdateKeyItem, toRegister);
        }
        public void ShowHotKeyList()
        {
            List<HotKeyData> datalist = Sys_SettingHotKey.Instance.HotKeyCodeList;
            visualGridCount = datalist.Count;
            hotKeyItemList.Clear();
            FrameworkTool.CreateChildList(viewPort, visualGridCount);
            for (int i = 0; i < visualGridCount; i++)
            {
                Transform tran = viewPort.transform.GetChild(i);
                HotKeyItemCeil ceil = PoolManager.Fetch<HotKeyItemCeil>();
                ceil.OnInit(tran);
                ceil.RefreshData(datalist[i]);
                hotKeyItemList.Add(ceil);
            }
        }

        private void UpdateKeyItem(int obj)
        {
            int index = obj - 1;
            if (index < 0 || index >= hotKeyItemList.Count)
                return;
            HotKeyItemCeil ceil = hotKeyItemList[index];
            ceil.RefreshData(Sys_SettingHotKey.Instance.HotKeyCodeList[index]);
            if (Sys_SettingHotKey.Instance.HotKeyCodeList[index].keyCode == KeyCode.Escape)
                isRefreshCeil = true;
        }

        protected override void Update()
        {
            bool isFocused = false;
            for (int i = 0; i < hotKeyItemList.Count; i++)
            {
                if (hotKeyItemList[i].keyInput.isFocused)
                {
                    isFocused = true;
                    Sys_SettingHotKey.Instance.CurrentFocused = i + 1;
                }
            }
            if (isRefreshCeil)
            {
                UpdateKeyItem(Sys_SettingHotKey.Instance.CurrentFocused);
                isRefreshCeil = false;
            }
            if (!isFocused)
                Sys_SettingHotKey.Instance.CurrentFocused = -1;
        }
    }

    public class HotKeyItemCeil
    {
        public InputField keyInput;
        private Text text;
        public void OnInit(Transform tran)
        {
            keyInput = tran.Find("InputField").GetComponent<InputField>();
            text = tran.Find("Text").GetComponent<Text>();
            tran.Find("Text (1)").gameObject.SetActive(false);
        }
        public void RefreshData(HotKeyData data)
        {
            keyInput.readOnly = true;
            keyInput.text = data.KeyViewCharacter;
            text.text = data.name;
        }

        public void Clear()
        {

        }
    }
}
