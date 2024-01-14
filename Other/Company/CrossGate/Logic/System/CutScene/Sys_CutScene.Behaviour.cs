using Framework;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic {
    /// CutScene系统
    public partial class Sys_CutScene : SystemModuleBase<Sys_CutScene> {
        // Cutscene开始播放的时候执行
        private void Start(CutSceneArg arg) {
            if (arg.group != "CutScene") {
                return;
            }
            if (arg.tag == "LifeCycle") {
                this.Start();
            }
            else if (arg.tag == "PlayHud") {
                if (arg.transform != null) {
                    TriggerCutSceneBubbleEvt evt = new TriggerCutSceneBubbleEvt();
                    evt.bubbleid = arg.id;
                    evt.offest = arg.offset;
                    evt.gameObject = arg.transform.gameObject;
                    if (AutoCameraStack.TryGetActiveCamera(0, out UnityEngine.Camera camera)) {
                        evt.camera = camera;
                    }
                    Sys_HUD.Instance.eventEmitter.Trigger<TriggerCutSceneBubbleEvt>(Sys_HUD.EEvents.OnTriggerCutSceneBubble, evt);
                }
            }
            else if (arg.tag == "QTEClick") {
                System.Action action = () => {
                    this.timelineDirector.Resume();
                };
                Sys_QTE.Instance.OpenQTE(arg.id, action, arg);
            }
            else if (arg.tag == "QTELongPress") {
                System.Action action = () => {
                    this.timelineDirector.Resume();
                };
                Sys_QTE.Instance.OpenQTE(arg.id, action, arg);
            }
            else if (arg.tag == "QTESlide") {
                System.Action action = () => {
                    this.timelineDirector.Resume();
                };
                Sys_QTE.Instance.OpenQTE(arg.id, action, arg);
            }
            else if (arg.tag == "OpenUI") {
                UIManager.OpenUI((int)arg.id);
            }
            else if (arg.tag == "Fadeinout") {
                UIManager.OpenUI(EUIID.UI_FadeInOut, false, arg);
            }
            else if (arg.tag == "ShakeCamera") {
                if (arg.transform != null && arg.transform.TryGetComponent<ShakeCamera>(out ShakeCamera shakeCamera)) {
                    CSVShock.Data data = CSVShock.Instance.GetConfData(arg.id);
                    if (data != null) {
                        Vector3 strength = new Vector3(data.strength_x / 1000f, data.strength_y / 1000f, data.strength_z / 1000f);
                        shakeCamera.BeginShake(data.duration / 1000f, strength, (int)data.vibrato, data.randomness);
                    }
                }
            }
            else if (arg.tag == "Dissolve") {
                Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eCommunalAIWorkStream, "<color=yellow>Cutscene执行Dissolve</color>");

                Transform target = null;
                string targetName = arg.transform.name;
                var p = this.mainPlayerModel.transform.parent.parent;
                for (int i = 0, length = p.childCount; i < length; i++) {
                    Transform child = p.GetChild(i);
                    if (child.name == targetName) {
                        target = child;
                        break;
                    }
                }

                if (target != null) {
                    if (arg.boolValue) {
                        if (this._communalAIManagerEntity != null)
                            this._communalAIManagerEntity.Dispose();
                        this._communalAIManagerEntity = WS_CommunalAIManagerEntity.Start(1u, target.gameObject);
                    }
                    else if (this._communalAIManagerEntity != null) {
                        this._communalAIManagerEntity.Dispose();
                        this._communalAIManagerEntity = null;
                    }
                }
                //else if (this.mainPlayerModel != null) {
                //    if (arg.boolValue) {
                //        if (_communalAIManagerEntity != null)
                //            _communalAIManagerEntity.Dispose();
                //        _communalAIManagerEntity = WS_CommunalAIManagerEntity.Start(1u, mainPlayerModel);
                //    }
                //    else if (_communalAIManagerEntity != null) {
                //        _communalAIManagerEntity.Dispose();
                //        _communalAIManagerEntity = null;
                //    }
                //}
            }
        }

        private void End(CutSceneArg arg) {
            if (arg.group != "CutScene") {
                return;
            }
            if (arg.tag == "LifeCycle") {
                this.End();
            }
            else if (arg.tag == "PlayHud") {
                //Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearCutSceenBubbles);
            }
            else if (arg.tag == "QTEClick") {
            }
            else if (arg.tag == "QTELongPress") {
            }
            else if (arg.tag == "QTESlide") {
            }
            else if (arg.tag == "OpenUI") {
                UIManager.CloseUI((EUIID)arg.id);
            }
            else if (arg.tag == "Fadeinout") {
                //if (UIManager.IsOpen((EUIID)arg.id)) {
                //    UIManager.CloseUI((EUIID)arg.id);
                //}
            }
            else if (arg.tag == "ShakeCamera") {
                if (arg.transform != null && arg.transform.TryGetComponent<ShakeCamera>(out ShakeCamera shakeCamera)) {
                    shakeCamera.EndShake();
                }
            }
            else if (arg.tag == "Dissolve") {
                // 溶解成功播放完毕之后的回调
                this.EndDissolve();
            }
        }

        private void EndDissolve() {

        }
    }
}
