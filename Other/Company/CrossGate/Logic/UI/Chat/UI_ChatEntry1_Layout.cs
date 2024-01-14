//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2019/12/31 14:50:53
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
namespace Logic
{

	public class UI_ChatEntry1_Layout
{
	#region UI Variable Statement 
	public GameObject mRoot { get; private set; }
	public Transform mTrans { get; private set; }
		public RectTransform rtLeft_RectTransform { get; private set; } 
		public EmojiText txtContentL_EmojiText { get; private set; } 
		public Button btn_imgIconL_Button { get; private set; } 
		public Image btn_imgIconL_Image { get; private set; } 
		public RectTransform rtRight_RectTransform { get; private set; } 
		public EmojiText txtContentR_EmojiText { get; private set; } 
		public Button btn_imgIconR_Button { get; private set; } 
		public Text txtName_Text { get; private set; } 
		public Image btn_imgIconR_Image { get; private set; } 
	#endregion
	public void Parse(GameObject root) 
	{
		mRoot = root;
		mTrans = root.transform;
		rtLeft_RectTransform = mTrans.Find("_rtLeft").GetComponent<RectTransform>(); 
		txtContentL_EmojiText = mTrans.Find("_rtLeft/rtContent/_txtContentL").GetComponent<EmojiText>(); 
		btn_imgIconL_Button = mTrans.Find("_rtLeft/rtHead/_btn_imgIconL").GetComponent<Button>(); 
		btn_imgIconL_Image = mTrans.Find("_rtLeft/rtHead/_btn_imgIconL").GetComponent<Image>(); 
		rtRight_RectTransform = mTrans.Find("_rtRight").GetComponent<RectTransform>(); 
		txtContentR_EmojiText = mTrans.Find("_rtRight/rtContent/_txtContentR").GetComponent<EmojiText>(); 
		btn_imgIconR_Button = mTrans.Find("_rtRight/rtHead/_btn_imgIconR").GetComponent<Button>(); 
		txtName_Text = mTrans.Find("_txtName").GetComponent<Text>(); 
		btn_imgIconR_Image = mTrans.Find("_rtRight/rtHead/_btn_imgIconR").GetComponent<Image>(); 
	}

	public void RegisterEvents(IListener listener) 
	{
		btn_imgIconL_Button.onClick.AddListener(listener.OnimgIconL_ButtonClicked);
		btn_imgIconR_Button.onClick.AddListener(listener.OnimgIconR_ButtonClicked);
	}

	public interface IListener
	{
		void OnimgIconL_ButtonClicked();
		void OnimgIconR_ButtonClicked();
	}
}

}
