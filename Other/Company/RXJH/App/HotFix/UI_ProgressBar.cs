using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ProgressBar : MonoBehaviour
{

    public Text Tex_State;//资源状态（更新检测中）
    public Slider Slider_Jindu;
    public Text Tex_Progress;//更新进度 %50
    public Button Btn_hotfix;

    private EAppError eAppError = EAppError.None;
    private EAppState eAppState = EAppState.Invalid;
    
    private HotFixTipWord hotFixTipWord;
    void Start()
    {
        hotFixTipWord = HotFixTipWordManager.Instance.hotFixTipWord;
        Tex_State.text = "正在开启魔力世界...";
        Btn_hotfix.onClick.AddListener(OnBtnHotFix);
    }

    public void OnBtnHotFix()
    {
        //弹框
        //UI_Box.Create(UseUIBOxType.HotFixStart);
    }

    void Update()
    {
        UpdateProgress();
    }


    void UpdateProgress()
    {
        Slider_Jindu.value = AppManager.InitGameProgress;
        Tex_Progress.text = string.Format("{0:P2}", AppManager.InitGameProgress);
    }

}
