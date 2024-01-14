//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2020/2/20 12:03:04
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
namespace Logic
{

	public class UI_Horn_Layout
{
	#region UI Variable Statement 
	public GameObject mRoot { get; private set; }
	public Transform mTrans { get; private set; }
		public Button btn_close_Button { get; private set; } 
		public Button btn_Send_Button { get; private set; } 
		public InfinityGrid sv_HornList_InfinityGrid { get; private set; } 
		public Image img_box_Image { get; private set; } 
		public Text txt_content_EmojiText { get; private set; } 
		public InputField ipt_Horn_InputField { get; private set; } 
		public Button btn_Emoji_Button { get; private set; } 
		public Text txt_costNum_Text { get; private set; } 
		public Text txt_TimeNum_Text { get; private set; } 
		public CP_ToggleRegistry tg_Channel_CP_ToggleRegistry { get; private set; } 
		public RectTransform tempItem_RectTransform { get; private set; } 
	#endregion
	public void Parse(GameObject root) 
	{
		mRoot = root;
		mTrans = root.transform;
		btn_close_Button = mTrans.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>(); 
		btn_Send_Button = mTrans.Find("Animator/imgForm/_btn_Send").GetComponent<Button>(); 
		sv_HornList_InfinityGrid = mTrans.Find("Animator/imgForm/rtLeft/_sv_HornList").GetComponent<InfinityGrid>(); 
		img_box_Image = mTrans.Find("Animator/imgForm/rtRight/_img_box").GetComponent<Image>(); 
		txt_content_EmojiText = mTrans.Find("Animator/imgForm/rtRight/_img_box/_txt_content").GetComponent<Text>(); 
		ipt_Horn_InputField = mTrans.Find("Animator/imgForm/rtRight/_ipt_Horn").GetComponent<InputField>(); 
		btn_Emoji_Button = mTrans.Find("Animator/imgForm/rtRight/_btn_Emoji").GetComponent<Button>(); 
		txt_costNum_Text = mTrans.Find("Animator/imgForm/rtRight/imgCost/_txt_costNum").GetComponent<Text>(); 
		txt_TimeNum_Text = mTrans.Find("Animator/imgForm/rtRight/imgTime/_txt_TimeNum").GetComponent<Text>(); 
		tg_Channel_CP_ToggleRegistry = mTrans.Find("Animator/_tg_Channel").GetComponent<CP_ToggleRegistry>(); 
		tempItem_RectTransform = mTrans.Find("@tempItem").GetComponent<RectTransform>(); 
	}

	public void RegisterEvents(IListener listener) 
	{
		btn_close_Button.onClick.AddListener(listener.Onclose_ButtonClicked);
		btn_Send_Button.onClick.AddListener(listener.OnSend_ButtonClicked);
		ipt_Horn_InputField.onValueChanged.AddListener(listener.OnHorn_InputFieldValueChanged);
		btn_Emoji_Button.onClick.AddListener(listener.OnEmoji_ButtonClicked);
	}

	public interface IListener
	{
		void Onclose_ButtonClicked();
		void OnSend_ButtonClicked();
		void OnHorn_InputFieldValueChanged(string arg);
		void OnEmoji_ButtonClicked();
	}
}

}
