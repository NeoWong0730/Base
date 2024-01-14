//this file is auto created by QuickCode,you can edit it 
//do not need to care initialization of ui widget any more 
//------------------------------------------------------------
/*
*   @Author:TR
*   DateTime:2020/1/2 17:06:55
*   Purpose:UI Componments Data Binding
*/
//-------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
namespace Logic
{

	public class UI_HornEntry_Layout
{
	#region UI Variable Statement 
	public GameObject mRoot { get; private set; }
	public Transform mTrans { get; private set; }
		public RectTransform PropItem_RectTransform { get; private set; } 
		public Text txt_costNum_Text { get; private set; } 
		public Text txt_Name_Text { get; private set; } 
		public Button tempItem_Button { get; private set; } 
	#endregion
	public void Parse(GameObject root) 
	{
		mRoot = root;
		mTrans = root.transform;
		PropItem_RectTransform = mTrans.Find("_PropItem").GetComponent<RectTransform>(); 
		txt_costNum_Text = mTrans.Find("imgCost/_txt_costNum").GetComponent<Text>(); 
		txt_Name_Text = mTrans.Find("_txt_Name").GetComponent<Text>(); 
		tempItem_Button = mTrans.GetComponent<Button>(); 
	}

	public void RegisterEvents(IListener listener) 
	{
		tempItem_Button.onClick.AddListener(listener.OnButtonClicked);
	}

	public interface IListener
	{
		void OnButtonClicked();
	}
}

}
