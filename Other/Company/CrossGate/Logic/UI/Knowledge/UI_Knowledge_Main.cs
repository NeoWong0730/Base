using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Main : UIBase
    {
        private class KnowledgeType
        {
            private Transform transform;

            private Button _btn;
            private Text _textName;
            private Transform _transRed;

            private Sys_Knowledge.ETypes _eType = Sys_Knowledge.ETypes.None;
            private uint _funId;

            public void Init(Transform trans)
            {
                transform = trans;

                _btn = transform.GetComponent<Button>();
                _btn.onClick.AddListener(OnClick);

                _textName = transform.Find("Title").GetComponent<Text>();
                _transRed = transform.Find("Image_Red");
            }

            private void OnClick()
            {
                if (!Sys_FunctionOpen.Instance.IsOpen(_funId, true))
                    return;

                switch (_eType)
                {
                    case Sys_Knowledge.ETypes.Annals:
                        UIManager.OpenUI(EUIID.UI_Knowledge_Annals);
                        break;
                    case Sys_Knowledge.ETypes.Brave:
                        UIManager.OpenUI(EUIID.UI_Knowledge_Brave);
                        break;
                    case Sys_Knowledge.ETypes.Fragment:
                        UIManager.OpenUI(EUIID.UI_Knowledge_Fragment);
                        break;
                    case Sys_Knowledge.ETypes.Gleanings:
                        UIManager.OpenUI(EUIID.UI_Knowledge_Gleanings);
                        break;
                    case Sys_Knowledge.ETypes.Cooking:
                        UIManager.OpenUI(EUIID.UI_Cooking_Collect);
                        break;
                    case Sys_Knowledge.ETypes.Achievement:
                        UIManager.OpenUI(EUIID.UI_Achievement);
                        break;
                    default:
                        break;
                }
            }

            public void SetType(Sys_Knowledge.ETypes eType)
            {
                _eType = eType;

                _funId = 0;
                switch (_eType)
                {
                    case Sys_Knowledge.ETypes.Annals:
                        _funId = 22040;
                        break;
                    case Sys_Knowledge.ETypes.Brave:
                        _funId = 22010;
                        break;
                    case Sys_Knowledge.ETypes.Fragment:
                        _funId = 22050;
                        break;
                    case Sys_Knowledge.ETypes.Gleanings:
                        _funId = 22070;
                        break;
                    case Sys_Knowledge.ETypes.Cooking:
                        _funId = 22020;
                        break;
                    case Sys_Knowledge.ETypes.Achievement:
                        _funId = 22090;
                        break;
                    default:
                        break;
                }

                bool isActive = Sys_FunctionOpen.Instance.IsOpen(_funId);
                ImageHelper.SetImageGray(transform.GetComponent<Image>(), !isActive, true);
            }

            public void UpdateRedPoint()
            {
                if (_transRed != null)
                    _transRed.gameObject.SetActive(Sys_Knowledge.Instance.IsRedPointByType(_eType));
            }
        }

        private KnowledgeType _typeBrave;
        private KnowledgeType _typeAnnals;
        private KnowledgeType _typeFragment;
        private KnowledgeType _typeGleanings;
        private KnowledgeType _typeCooking;
        private KnowledgeType _typeAchievement;

        private Button _btnExchange;

        Transform achTrans;
        protected override void OnLoaded()
        {            
            Button btnClose = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(()=>{ this.CloseSelf(); });

            _typeBrave = new KnowledgeType();
            _typeBrave.Init(transform.Find("Animator/Button_Group/Button0"));
            _typeBrave.SetType(Sys_Knowledge.ETypes.Brave);

            _typeAnnals = new KnowledgeType();
            _typeAnnals.Init(transform.Find("Animator/Button_Group/Button3"));
            _typeAnnals.SetType(Sys_Knowledge.ETypes.Annals);

            _typeFragment = new KnowledgeType();
            _typeFragment.Init(transform.Find("Animator/Button_Group/Button4"));
            _typeFragment.SetType(Sys_Knowledge.ETypes.Fragment);

            _typeGleanings = new KnowledgeType();
            _typeGleanings.Init(transform.Find("Animator/Button_Group/Button6"));
            _typeGleanings.SetType(Sys_Knowledge.ETypes.Gleanings);

            _typeCooking = new KnowledgeType();
            _typeCooking.Init(transform.Find("Animator/Button_Group/Button1"));
            _typeCooking.SetType(Sys_Knowledge.ETypes.Cooking);

            _btnExchange = transform.Find("Animator/Button_Group/Button7").GetComponent<Button>();
            _btnExchange.onClick.AddListener(OnClickExchange);

            achTrans = transform.Find("Animator/Button_Group/Button8");
            bool isShow = Sys_Achievement.Instance.CheckAchievementIsCanShow();
            if (isShow)
            {
                achTrans.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(22090));
                _typeAchievement = new KnowledgeType();
                _typeAchievement.Init(achTrans);
                _typeAchievement.SetType(Sys_Knowledge.ETypes.Achievement);
            }
            else
                achTrans.gameObject.SetActive(false);
        }        

        protected override void OnShow()
        {         
            UpdateInfo();
        }
        protected override void ProcessEvents(bool toRegister)
        {
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, toRegister);
        }
        private void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData obj)
        {
            if (obj.id == 22090)
                achTrans.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(22090));
        }
        private void OnClickExchange()
        {
            //Debug.LogError("兑换...");
            if (Sys_FunctionOpen.Instance.IsOpen(40301, true))
            {
                MallPrama param = new MallPrama();
                param.mallId = 501u;
                param.shopId = 5013u;

                UIManager.OpenUI(EUIID.UI_PointMall, false, param);
            }
        }

        private void UpdateInfo()
        {
            _typeBrave.UpdateRedPoint();
            _typeAnnals.UpdateRedPoint();
            _typeFragment.UpdateRedPoint();
            _typeGleanings.UpdateRedPoint();
            _typeCooking.UpdateRedPoint();
            if (Sys_Achievement.Instance.CheckAchievementIsCanShow())
                _typeAchievement.UpdateRedPoint();
        }
    }
}


