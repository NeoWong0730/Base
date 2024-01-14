using System;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic {
    public class Task_PathFindOpenUIFunction : FunctionBase {

        public CSVOpenUi.Data csvUI {
            get;
            private set;
        }

        Timer timer;

        public override void Init() {
            this.csvUI = CSVOpenUi.Instance.GetConfData(this.ID);
        }

        protected override bool CanExecute(bool CheckVisual = true) {
            if (this.csvUI == null) {
                Sys_Hint.Instance.PushContent_Normal("ui表对应的id不存在" + this.ID.ToString());
                return false;
            }
            return true;
        }

        protected override void OnExecute() {
            base.OnExecute();

            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
            this.timer?.Cancel();
            this.timer = Timer.Register(0.1f, this.TimerCallBack, null, false, true);
        }

        void TimerCallBack() {
            object args = null;
            if (this.csvUI.ui_para == 0) {
                args = new Tuple<uint, object>(this.HandlerID, null);
                UIManager.OpenUI((int)this.csvUI.Uiid, true, args);
            }
            else {
                // 摆摊特殊处理
                if ((EUIID)this.csvUI.Uiid == EUIID.UI_VendorList) {
                    if (CSVCheckseq.Instance.GetConfData(70002201).IsValid()) {
                        ulong uid = this.npc == null ? 0 : this.npc.uID;
                        args = new Tuple<uint, uint, ulong, object>(this.HandlerID, this.npc.cSVNpcData.id, uid, this.csvUI.ui_para);
                        UIManager.OpenUI((int)this.csvUI.Uiid, true, args);
                    }
                    else {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590000003));
                    }
                }
                else if ((EUIID)this.csvUI.Uiid == EUIID.UI_Map) {
                    UnityEngine.Vector3 pos = new UnityEngine.Vector3(this.csvUI.ui_para1, this.csvUI.ui_para2, 0f);
                    args = new Sys_Map.TargetMapParameter(this.csvUI.ui_para, pos);
					UIManager.OpenUI((int)this.csvUI.Uiid, true, args);
                }
                else if ((EUIID)this.csvUI.Uiid == EUIID.UI_Mall) {
                    if (this.CtrlType != ECtrlType.Auto) {
                        args = new MallPrama() {
                            mallId = this.csvUI.ui_para,
                        };
                        UIManager.OpenUI((int)this.csvUI.Uiid, false, args);
                    }
                    else {
                        args = new MallPrama() {
                            mallId = this.csvUI.ui_para,
                            shopId = (uint)this.csvUI.ui_para1,
                            itemId = (uint)this.csvUI.ui_para2,
                        };
                        UIManager.OpenUI((int)this.csvUI.Uiid, false, args);
                    }
                }
                else if ((EUIID)this.csvUI.Uiid == EUIID.UI_Knowledge_Cooking) {
                    if (this.CtrlType != ECtrlType.Auto) {
                        UIManager.OpenUI((int)this.csvUI.Uiid, false, null);
                    }
                    else {
                        args = (uint) this.csvUI.ui_para;
                        UIManager.OpenUI((int)this.csvUI.Uiid, false, args);
                    }
                }
                else if ((EUIID)this.csvUI.Uiid == EUIID.UI_Cooking_Single) {
                    if (this.CtrlType != ECtrlType.Auto) {
                        OpenCookingSingleParm openCookingSingleParm = new OpenCookingSingleParm();
                        openCookingSingleParm.cookFunId = (uint) this.csvUI.ui_para1;
                        UIManager.OpenUI((int)this.csvUI.Uiid, false, openCookingSingleParm);
                    }
                    else {
                        OpenCookingSingleParm openCookingSingleParm = new OpenCookingSingleParm();
                        openCookingSingleParm.cookFunId = (uint) this.csvUI.ui_para1;
                        openCookingSingleParm.cookId = (uint) this.csvUI.ui_para;
                        UIManager.OpenUI((int)this.csvUI.Uiid, false, openCookingSingleParm);
                    }
                }
                else if ((EUIID)this.csvUI.Uiid == EUIID.UI_SubmitPet) {
                    args = new Tuple<uint, int, uint>(this.HandlerID, (int) this.HandlerIndex, this.csvUI.ui_para);
                    UIManager.OpenUI((int)this.csvUI.Uiid, false, args);
                } else {
                    args = new Tuple<uint, object>(this.HandlerID, this.csvUI.ui_para);
                    UIManager.OpenUI((int)this.csvUI.Uiid, true, args);
                }
            }

            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            Sys_Task.Instance.InterruptCurrentTaskDoing();
        }

        protected override void OnDispose() {
            this.timer?.Cancel();
            this.timer = null;
            this.csvUI = null;

            base.OnDispose();
        }
    }
}
