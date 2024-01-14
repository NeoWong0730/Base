//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2020/1/20 17:35:39
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_Chat_Layout : UILayout.UI_Chat_Layout
    {        
        public void RegisterEvents(IListener listener)
        {
            btn_Close_Button.onClick.AddListener(listener.OnClose_ButtonClicked);
            btn_LaBa_Button.onClick.AddListener(listener.OnLaBa_ButtonClicked);
            tog_LaBa_Toggle.onValueChanged.AddListener(listener.OnLaBa_ToggleValueChanged);
            tog_GeRen_Toggle.onValueChanged.AddListener(listener.OnGeRen_ToggleValueChanged);            
            tog_Lock_Toggle.onValueChanged.AddListener(listener.OnLock_ToggleValueChanged);
            rt_Tip_Button.onClick.AddListener(listener.OnTip_ButtonClicked);
            ipt_Word_InputField.onValueChanged.AddListener(listener.OnWord_InputFieldValueChanged);
            btn_Emoji_Button.onClick.AddListener(listener.OnEmoji_ButtonClicked);
            btn_Send_Button.onClick.AddListener(listener.OnSend_ButtonClicked);            
            btn_EnterRoom.onClick.AddListener(listener.OnEnterRoom_ButtonClicked);
            btn_ExitRoom.onClick.AddListener(listener.OnExitRoom_ButtonClicked);
            btn_OpenMic.onClick.AddListener(listener.OnOpenMic_ButtonClicked);
            btn_Setting.onClick.AddListener(listener.OnSetting_ButtonClicked);
            btn_ChangeHead.onClick.AddListener(listener.OnChangeHead_ButtonClicked);
            btn_PcExpand.onClick.AddListener(listener.OnPcExpand_ButtonClicked);
            btn_Money.onClick.AddListener(listener.OnMoney_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
            void OnLaBa_ButtonClicked();
            void OnLaBa_ToggleValueChanged(bool arg);
            void OnGeRen_ToggleValueChanged(bool arg);
            void OnLock_ToggleValueChanged(bool arg);
            void OnTip_ButtonClicked();
            void OnWord_InputFieldValueChanged(string arg);
            void OnEmoji_ButtonClicked();
            void OnSend_ButtonClicked();
            void OnEnterRoom_ButtonClicked();
            void OnExitRoom_ButtonClicked();
            void OnOpenMic_ButtonClicked();
            void OnSetting_ButtonClicked();
            void OnChangeHead_ButtonClicked();
            void OnPcExpand_ButtonClicked();
            void OnMoney_ButtonClicked();
        }
    }

}
