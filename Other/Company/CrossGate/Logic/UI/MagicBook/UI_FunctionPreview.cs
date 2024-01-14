using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FunctionPreview : UIBase {
        public uint CurId;

        public Image funIcon;
        public Text funName;
        public Text funDesc;
        public Transform oneImg;
        public Text limit;
        public Button btnGoto;
        public Button btnNext;
        public Animator animator;

        public COWComponent<Transform> vds = new COWComponent<Transform>();

        protected override void OnLoaded() {
            funIcon = transform.Find("Animator/Node/View_Title/Image/Icon").GetComponent<Image>();
            funName = transform.Find("Animator/Node/View_Title/Text_Name").GetComponent<Text>();
            funDesc = transform.Find("Animator/Node/View_Title/Text_Describe").GetComponent<Text>();

            oneImg = transform.Find("Animator/Node/View_Picture/Item");
            animator = transform.Find("Animator/Node").GetComponent<Animator>();

            limit = transform.Find("Animator/Node/View_Button/Text_Tips").GetComponent<Text>();
            btnGoto = transform.Find("Animator/Node/View_Button/Btns/Btn_Check").GetComponent<Button>();
            btnNext = transform.Find("Animator/Node/View_Button/Btns/Btn_Confirm").GetComponent<Button>();

            btnGoto.onClick.AddListener(OnBtnGotoClicked);
            btnNext.onClick.AddListener(OnBtnNextClicked);
        }

        protected override void OnDestroy() {
            // this.vds?.Clear();
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_FuncPreview.Instance.eventEmitter.Handle<uint>(Sys_FuncPreview.EEvents.OnDel, (delId) => {
                var ids = Sys_FuncPreview.Instance.GetUnReadedIds(true);
                if (ids != null && ids.Count > 0) {
                    this.CurId = ids[0];
                    this.RefreshAll();
                }
                else {
                    this.CloseSelf();
                }
            }, toRegister);
        }

        private void OnBtnGotoClicked() {
            var currentSysData = CSVFunForeshow.Instance.GetConfData(this.CurId);
            if (currentSysData != null) {
                if (!Sys_Hint.Instance.PushForbidOprationInFight()) {
                    if (currentSysData.Type == 0) {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1) {
                            uint Openid = currentSysData.JumpInterface[0];
                            CSVOpenUi.Data cSVOpenUiData = CSVOpenUi.Instance.GetConfData(Openid);
                            if (null != cSVOpenUiData) {
                                uint para = cSVOpenUiData.ui_para;
                                if (para != 0) {
                                    UIManager.OpenUI((EUIID) cSVOpenUiData.Uiid, false, para);
                                }
                                else {
                                    UIManager.OpenUI((EUIID) cSVOpenUiData.Uiid);
                                }
                            }
                        }
                    }
                    else if (currentSysData.Type == 1) {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1) {
                            MessageEx practiceEx = new MessageEx {
                                messageState = (EPetMessageViewState) currentSysData.JumpInterface[0],
                            };
                            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
                        }
                    }
                    else if (currentSysData.Type == 2) {
                        if (!Sys_FunctionOpen.Instance.IsOpen(30401, true)) return;

                        Sys_Family.Instance.OpenUI_Family();
                    }
                    else if (currentSysData.Type == 3) {
                        if (!Sys_FunctionOpen.Instance.IsOpen(10304, true))
                            return;
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1) {
                            Sys_Equip.UIEquipPrama data = new Sys_Equip.UIEquipPrama();
                            data.curEquip = null;
                            data.opType = (Sys_Equip.EquipmentOperations) currentSysData.JumpInterface[0];
                            UIManager.OpenUI(EUIID.UI_Equipment, false, data);
                        }
                    }
                    else if (currentSysData.Type == 4) {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 3) {
                            //配置似乎也没必要。
                            EUIID uiId = (EUIID) currentSysData.JumpInterface[0];
                            MallPrama param = new MallPrama();
                            param.mallId = currentSysData.JumpInterface[1];
                            param.shopId = currentSysData.JumpInterface[2];
                            UIManager.OpenUI(uiId, false, param);
                        }
                    }
                    else if (currentSysData.Type == 5) // 前往寻找npc
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1) {
                            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(currentSysData.JumpInterface[0]);
                            this.CloseSelf();
                        }
                    }
                    else if (currentSysData.Type == 6) // 伙伴功能
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1) {
                            var par = new PartnerUIParam();
                            par.tabIndex = (int)currentSysData.JumpInterface[0];
                            
                            UIManager.OpenUI(EUIID.UI_Partner, false, par);
                        }
                    }
                    else if (currentSysData.Type == 7) // 技能 天赋
                    {
                        if (null != currentSysData.JumpInterface && currentSysData.JumpInterface.Count >= 1) {
                            var par = new List<int>() {
                                (int)currentSysData.JumpInterface[0],
                            };
                            UIManager.OpenUI(EUIID.UI_SkillUpgrade, false, par);
                        }
                    }
                }
            }
        }

        private void OnBtnNextClicked() {
            if (this.levelValid) {
                Sys_FuncPreview.Instance.ReqRead(this.CurId);
            }
            else {
                this.CloseSelf();
            }
        }

        protected override void OnOpen(object arg) {
            this.CurId =  Convert.ToUInt32(arg);
        }

        protected override void OnOpened() {
            this.RefreshAll();
        }

        private bool levelValid = false;

        public void RefreshAll() {
            this.animator.Play("Node", -1, 0);
                
            var csv = CSVFunForeshow.Instance.GetConfData(this.CurId);
            if (csv != null) {
                ImageHelper.SetIcon(funIcon, csv.SysIcon);
                TextHelper.SetText(funName, csv.words);
                TextHelper.SetText(funDesc, csv.SysForecast);

                this.vds.TryBuildOrRefresh(oneImg.gameObject, this.oneImg.parent, csv.Picture.Count, (t, index) => {
                    RawImage ri = t.GetChild(0).GetComponent<RawImage>();
                    ImageHelper.SetTexture(ri, csv.Picture[index]);
                });

                this.levelValid = Sys_Role.Instance.Role.Level >= csv.FunctionLv;
                if (this.levelValid) {
                    this.limit.gameObject.SetActive(false);
                    this.btnGoto.gameObject.SetActive(true);
                }
                else {
                    this.limit.gameObject.SetActive(true);
                    this.btnGoto.gameObject.SetActive(false);
                    if (csv.FunctionLv == 0) {
                        TextHelper.SetText(this.limit, csv.OpenForecastLan);
                    }
                    else {
                        TextHelper.SetText(this.limit, 12418, csv.FunctionLv.ToString());
                    }
                }
            }
            else {
                this.CloseSelf();
            }
        }
    }
}