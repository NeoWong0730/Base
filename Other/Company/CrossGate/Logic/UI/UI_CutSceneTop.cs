using System;
using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_CutSceneTop : UIBase {
        private Button btnReturn;
        private Text text;
        private Image bg;

        private Timer timer;
        private AudioEntry audioEntry;

        private CSVCutScene.Data csvCutScene;
        private Action onFinish;

        protected override void OnLoaded() {
            this.btnReturn = this.transform.Find("Animator/SkipButton").GetComponent<Button>();
            this.text = this.transform.Find("Animator/Text").GetComponent<Text>();
            this.bg = this.transform.Find("GameObject").GetComponent<Image>();

            this.btnReturn.onClick.AddListener(this.OnBtnReturnClicked);
            this.btnReturn.gameObject.SetActive(false);
        }
        protected override void ProcessEvents(bool toRegister) {
            TimelineLifeCircle.eventEmitter.Handle<CutSceneArg>(ETimelineLifeCircle.OnBehaviourPlay, this.Start, toRegister);
            TimelineLifeCircle.eventEmitter.Handle<CutSceneArg>(ETimelineLifeCircle.OnBehaviourPause, this.End, toRegister);
            TimelineLifeCircle.eventEmitter.Handle<CutSceneArg>(ETimelineLifeCircle.SetUIAlpha, this.OnSetUIAlpha, toRegister);
        }

        private void OnBtnReturnClicked() {
            this.onFinish?.Invoke();
            this.timer?.Cancel();

            this.CloseSelf();
        }
        protected override void OnOpen(object arg) {
            Tuple<CSVCutScene.Data, Action> tp = arg as Tuple<CSVCutScene.Data, Action>;
            if (tp != null) {
                this.csvCutScene = tp.Item1;
                this.onFinish = tp.Item2;
            }
        }
        protected override void OnOpened() {
            this.btnReturn.gameObject.SetActive(false);

            float cd = this.csvCutScene.time;
            if (cd > 0) {
                this.timer?.Cancel();
                this.timer = Timer.Register(cd, () => {
                    this.btnReturn.gameObject.SetActive(true);
                });
            }
            else {
                this.btnReturn.gameObject.SetActive(true);
            }

            this.Refresh();
        }
        protected override void OnClose() {
            this.OnClearTimers();
            this.timer?.Cancel();

            if (this.text != null)
                this.text.text = null;
        }
        private void OnClearTimers() {
            if (this.audioEntry != null) {
                CSVSound.Data csvSound = CSVSound.Instance.GetConfData(this.csvCutScene.audioId);
                if (csvSound.Type != (int)AudioUtil.EAudioType.BGM) {
                    this.audioEntry.Stop();
                }
                else {
                    CSVMapInfo.Data curMap = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
                    if (curMap != null && curMap.sound_bgm != this.csvCutScene.audioId) {
                        this.audioEntry.Stop();
                        AudioUtil.PlayMapBGM();
                    }
                }
            }
        }

        private void Refresh() {
            // --------------------- 背景音效 ---------------------
            this.audioEntry = AudioUtil.PlayAudio(this.csvCutScene.audioId);

            // --------------------- 字幕 ------------------------
            if (this.csvCutScene.type == 1) {
                // 视频
            }
            else {
            }
        }

        private void Start(CutSceneArg arg) {
            if (arg.group != "CutScene") {
                return;
            }
            if (arg.tag == "PlaySubtitleAudio") {
                CSVCutSceneSubTitle.Data csvSubtitle = CSVCutSceneSubTitle.Instance.GetConfData(arg.id);
                if (csvSubtitle != null) {
                    this.text.text = LanguageHelper.GetTextContent(csvSubtitle.content);
                    AudioUtil.PlayAudio(csvSubtitle.audio);
                }
                else {
                    this.text.text = null;
                    Debug.LogFormat("Cant find cutsceneSubtitleId: {0}", arg.id);
                }
            }
        }
        private void End(CutSceneArg arg) {
            if (arg.group != "CutScene") {
                return;
            }
            if (arg.tag == "PlaySubtitleAudio") {
            }
        }

        private void OnSetUIAlpha(CutSceneArg arg) {
            Color oldColor = this.bg.color;
            this.bg.color = new Color(oldColor.r, oldColor.g, oldColor.b, arg.value / 255f);
        }
    }
}