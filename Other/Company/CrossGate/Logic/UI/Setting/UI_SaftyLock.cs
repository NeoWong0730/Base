using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Logic;

public class UI_Unlock_Prop : UIBase
{
    #region 界面显示
    public InputField inputField;
    public Button closeBtn;
    public Button confirmBtn;
    private int errorCount;

    public void Init()
    {
        inputField = transform.Find("Animator/InputField").GetComponent<InputField>();
        closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
        confirmBtn = transform.Find("Animator/Button_Confirm").GetComponent<Button>();
    }
    #endregion

    #region 系统函数
    protected override void OnLoaded()
    {
        Init();
        closeBtn.onClick.AddListener(OnCloseButtonClicked);
        confirmBtn.onClick.AddListener(OnComfiremButtonClicked);
        errorCount = Sys_SecureLock.Instance.errorCount;

    }
    #endregion

    #region Function

    private void OnCloseButtonClicked()
    {
        UIManager.CloseUI(EUIID.UI_Unlock_Prop);
    }

    private void OnComfiremButtonClicked()
    {
        if (inputField.text.Length == 0)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000008).words);
            return;

        }
        if (Sys_SecureLock.Instance.errorCount>=5)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000005).words);
        }
        else
        {
            Sys_SecureLock.Instance.SecureLockUnlockReq(inputField.text);
            UIManager.CloseUI(EUIID.UI_Unlock_Prop);
        }
        

    }

    #endregion

}

public class UI_ChangePassword_Prop : UIBase
{
    #region 界面显示
    public InputField oldPassward;
    public InputField newPassward;
    public InputField confirmPassward;
    public Button closeBtn;
    public Button confirmBtn;


    #endregion

    #region 系统函数
    public void Init()
    {
        oldPassward = transform.Find("Animator/InputField_0").GetComponent<InputField>();
        newPassward = transform.Find("Animator/InputField_1").GetComponent<InputField>();
        confirmPassward = transform.Find("Animator/InputField_2").GetComponent<InputField>();
        closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
        confirmBtn = transform.Find("Animator/Button_Confirm").GetComponent<Button>();

    }

    protected override void OnLoaded()
    {
        Init();
        closeBtn.onClick.AddListener(OnCloseButtonClicked);
        confirmBtn.onClick.AddListener(OnComfiremButtonClicked);

    }

    #endregion

    #region Function
    private void OnCloseButtonClicked()
    {
        UIManager.CloseUI(EUIID.UI_ChangePassword_Prop);
    }

    private void OnComfiremButtonClicked()
    {

        string newPwd = newPassward.text.Trim();
        string oldpwd = oldPassward.text.Trim();
        string cfmPwd = confirmPassward.text.Trim();

        if (Sys_SecureLock.Instance.errorCount >= 5)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000005).words);
            return;
        }

        if (oldpwd.Length == 0 || newPwd.Length == 0 || cfmPwd.Length == 0)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000008).words);

        }
        else if (newPwd.Length < 6)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(106201).words);//密码过短
        }
        else if (newPwd.Length > 20)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(106202).words);//密码过长
        }
        else
        {
            if (Sys_SecureLock.Instance.CheckPassWard(newPwd))
            {


                if (Sys_SecureLock.Instance.ComparePassWard(newPwd, cfmPwd))
                {
                    Sys_SecureLock.Instance.SecureLockResetPasswordReq(oldPassward.text, newPassward.text);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000003).words);//两次密码不一致
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000009).words);//字符不合法
            }
        }

    }

    #endregion
}



public class UI_SetPassword_Prop : UIBase
{
    #region 界面显示
    public InputField setPassward;
    public InputField confirmPassward;
    public Button closeBtn;
    public Button confirmBtn;


    #endregion

    #region 系统函数
    public void Init()
    {
        setPassward = transform.Find("Animator/InputField_1").GetComponent<InputField>();
        confirmPassward = transform.Find("Animator/InputField_2").GetComponent<InputField>();
        closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
        confirmBtn = transform.Find("Animator/Button_Confirm").GetComponent<Button>();

    }

    protected override void OnLoaded()
    {
        Init();
        closeBtn.onClick.AddListener(OnCloseButtonClicked);
        confirmBtn.onClick.AddListener(OnComfiremButtonClicked);

    }

    #endregion

    #region Function
    private void OnCloseButtonClicked()
    {
        UIManager.CloseUI(EUIID.UI_SetPassword_Prop);
    }

    private void OnComfiremButtonClicked()
    {

        string setPwd = setPassward.text.Trim();
        string cfmPwd = confirmPassward.text.Trim();

        if (Sys_SecureLock.Instance.errorCount >= 5)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000005).words);
            return;
        }
        if (setPwd.Length == 0 || cfmPwd.Length == 0)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000008).words);

        }
        else if (setPwd.Length < 6)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(106201).words);//密码过短
        }
        else if (setPwd.Length > 20)
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(106202).words);//密码过长
        }
        else
        {
            if (Sys_SecureLock.Instance.CheckPassWard(setPwd))
            {
                if (Sys_SecureLock.Instance.ComparePassWard(setPwd, cfmPwd))
                {
                    Sys_SecureLock.Instance.SecureLockSetPasswordReq(setPwd);
                    UIManager.CloseUI(EUIID.UI_SetPassword_Prop);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000003).words);//两次密码不一致
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000009).words);//字符不合法
            }
        }

    }


    #endregion


}



