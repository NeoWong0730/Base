using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class UI_MerchantFleet_Left
    {
        private Text txt_MerchantNum;
        private Text txt_MerchantPercent;
        private Slider sl_MerchantLV;
        private Button btn_LvAward;
        private GameObject go_BtnRedPoint;
        private GameObject go_Fx;
        private Transform sheepRoot;
        private Text txt_FunctionOpen;
        private Text txt_Tips1;
        private Text txt_Tips2;
        private Animator sheepAnimator;
        private AsyncOperationHandle<GameObject> mHandle;
        private Timer m_timer;
        public void BindGameObject(Transform trans)
        {
            txt_MerchantNum = trans.Find("Text_Level/Text_Num").GetComponent<Text>();
            txt_MerchantPercent = trans.Find("Text_Level/Text_Percent").GetComponent<Text>();
            sl_MerchantLV = trans.Find("Slider").GetComponent<Slider>();
            btn_LvAward = trans.Find("Button_Reward").GetComponent<Button>();
            go_BtnRedPoint = trans.Find("Button_Reward/Image_Dot").gameObject;
            go_Fx = trans.Find("Button_Reward/FxReward").gameObject;
            sheepRoot = trans.Find("Texture");
            txt_FunctionOpen = trans.Find("Text_FunctionOpen").GetComponent<Text>();
            txt_Tips1 = trans.Find("Text_Tips1").GetComponent<Text>();
            txt_Tips2 = trans.Find("Text_Tips2").GetComponent<Text>();
            btn_LvAward.onClick.AddListener(OnGradeAwardButtonClicked);
        }
        public void Show()
        {
            LoadSheepAsync();
        }
        public void Update()
        {
            PanelShow();
        }
        public void Destory()
        {
            m_timer?.Cancel();
            AddressablesUtil.ReleaseInstance(ref mHandle, OnSheepLoadCompleted);
        }
        private void PanelShow()
        {
            //经验条
            var data = Sys_MerchantFleet.Instance.levelData;
            if (data != null)
            {
                txt_MerchantNum.text = LanguageHelper.GetTextContent(2028603, Sys_MerchantFleet.Instance.MerchantLevel.ToString());
                var nowExp = Sys_MerchantFleet.Instance.MerchantExp;
                int count = CSVMerchantFleetLevel.Instance.Count;
                uint realExp = 1;
                if (Sys_MerchantFleet.Instance.MerchantLevel < count)
                {
                    realExp = CSVMerchantFleetLevel.Instance.GetConfData(Sys_MerchantFleet.Instance.MerchantLevel+1).singleExp;
                }
                else
                {
                    realExp = data.singleExp;
                }
                txt_MerchantPercent.text = nowExp.ToString() + "/" + realExp;
                sl_MerchantLV.value = (float)nowExp / realExp;
            }
            //下方Tips
            FunctionTips();
            var pData = CSVParam.Instance.GetConfData(1549);
            txt_Tips1.text = LanguageHelper.GetTextContent(2028606, pData.str_value);
            pData = CSVParam.Instance.GetConfData(1550);
            txt_Tips2.text = LanguageHelper.GetTextContent(2028607, pData.str_value);
            //奖励Button
            go_BtnRedPoint.SetActive(Sys_MerchantFleet.Instance.CheckMerchantGrandeAwardRedPoint());
            go_Fx.SetActive(Sys_MerchantFleet.Instance.CheckMerchantGrandeAwardRedPoint());
        }
        private void FunctionTips()
        {
            txt_FunctionOpen.gameObject.SetActive(true);
            var idata = CSVMerchantFleetLevel.Instance.GetAll();
            for (int i = 0; i < idata.Count; i++)
            {
                if (idata[i].id > Sys_MerchantFleet.Instance.MerchantLevel && idata[i].functionLan != 0)
                {
                    txt_FunctionOpen.text = LanguageHelper.GetTextContent(2028605, idata[i].id.ToString(), LanguageHelper.GetTextContent(idata[i].functionLan));
                    return;
                }
            }
            txt_FunctionOpen.gameObject.SetActive(false);
        }
        private void LoadSheepAsync()
        {
            if (Sys_MerchantFleet.Instance.levelData == null) return;
            var path = Sys_MerchantFleet.Instance.levelData.picture;
            if (path != null)
            {
                AddressablesUtil.InstantiateAsync(ref mHandle, path, OnSheepLoadCompleted, true);
            }
        }
        bool loadEnd = false;
        private void OnSheepLoadCompleted(AsyncOperationHandle<GameObject> handle)
        {
            GameObject bgGameobject = handle.Result;
            if (null != bgGameobject)
            {
                bgGameobject.transform.SetParent(sheepRoot);
                RectTransform rectTransform = bgGameobject.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
            sheepAnimator = bgGameobject.transform.Find("Animator").GetComponent<Animator>();
            loadEnd = true;
            CaculateAnimationTime();
        }
        private void CaculateAnimationTime()
        {
            AnimationClip[] clips = sheepAnimator.runtimeAnimatorController.animationClips;
            float animTime = 3.0f;
            foreach (var clip in clips)
            {
                if (clip.name.Equals("UI_MerchantFleet_Ship1_Animator_Sail"))
                {
                    animTime = clip.length;
                    break;
                }
            }
            Sys_MerchantFleet.Instance.sheepAnimationTime = animTime;
        }
        public void PlaySailAnimation()
        {
            sheepAnimator.Play("Sail", -1, 0);
            m_timer?.Cancel();
            m_timer = Timer.Register(2.0f,OnResetAnimtor);
        }
        private void OnResetAnimtor()
        {
            sheepAnimator.Play("Open");
        }
        private void OnGradeAwardButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_MerchantFleet_GradeAward);
        }
    }
}
