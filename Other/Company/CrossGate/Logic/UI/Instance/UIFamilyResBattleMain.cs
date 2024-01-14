using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using UnityEngine.UI;

// 家族资源战
public class UIFamilyResBattleMain : UIComponent {
    public Text title;
    public Text content;

    protected override void Loaded() {
       this.transform.Find("Content_FamilyResBattle/BtnBG").GetComponent<Button>().onClick.AddListener(this.OnBtnIntroduceClicked);
        this.title = this.transform.Find("Content_FamilyResBattle/Text_Title").GetComponent<Text>();
        this.content = this.transform.Find("Content_FamilyResBattle/Text_Message1").GetComponent<Text>();

        this.transform.Find("Content_FamilyResBattle/Button_Leave").GetComponent<Button>().onClick.AddListener(this.OnBtnLeaveClicked);
    }

    public void RefreshView() {
        TextHelper.SetText(this.title, 3230000001);
        TextHelper.SetText(this.content, 3230000002);
    }

    private void OnBtnLeaveClicked() {
        Sys_FamilyResBattle.Instance.OpenLeaveMsgBox();
    }

    private void OnBtnIntroduceClicked() {
        Sys_CommonCourse.Instance.OpenCommonCourse(3, 301, 30103);
    }
}