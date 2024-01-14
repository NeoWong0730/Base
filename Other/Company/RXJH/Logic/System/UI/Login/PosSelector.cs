using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PosSelector : MonoBehaviour {
    #region Stage

    public enum EStage {
        Select, // 选角
        Create, // 创角
        Makeface, // 捏脸
        Max,
    }

    [Serializable]
    public class StageCfg {
        public EStage stage;
        public GameObject root;
        public Camera camera;
    }

    public EStage curStage;
    public StageCfg[] stageCfgs = new StageCfg[(int)EStage.Max];

    public void SetStage(EStage curStage) {
        this.curStage = curStage;

        for (int i = 0, length = stageCfgs.Length; i < length; ++i) {
            var one = stageCfgs[i];
            if (one.stage == curStage) {
                one.root.SetActive(true);
            }
            else {
                one.root.SetActive(false);
            }
        }
    }

    #endregion
    
    #region 选角stage

    [Serializable]
    public class SelectCfg {
        public Transform modelPos;
        public Transform cameraTrans;
    }

    public SelectCfg selectCfg;
    #endregion

    #region 创角stage

    public enum ETimelineType {
        Enter_M,
        Enter_F,
        M_F,
        F_M,
    }

    [Serializable]
    public class CreateCfg {
        public uint careerId;
        public Transform root;

        [HideInInspector] public Transform malePos;
        [HideInInspector] public Transform femalePos;

        [HideInInspector] public PlayableDirector enterM;
        [HideInInspector] public PlayableDirector enterF;
        [HideInInspector] public PlayableDirector m2f;
        [HideInInspector] public PlayableDirector f2m;

        [HideInInspector] public GameObject maleModel;
        [HideInInspector] public GameObject femaleModel;

        public void Init() {
            this.malePos = root.Find("Animator/Pos/Male");
            this.femalePos = root.Find("Animator/Pos/Female");

            this.enterM = root.Find("Timeline/Enter->M").GetComponent<PlayableDirector>();
            this.enterF = root.Find("Timeline/Enter->F").GetComponent<PlayableDirector>();
            this.m2f = root.Find("Timeline/M->F").GetComponent<PlayableDirector>();
            this.f2m = root.Find("Timeline/F->M").GetComponent<PlayableDirector>();
        }

        public void Visible(bool visible) {
            root.gameObject.SetActive(visible);
        }

        public void PlayTimeline(ETimelineType aim) {
            if (aim == ETimelineType.Enter_M) {
                enterM.Play();
                enterF.Stop();
                m2f.Stop();
                f2m.Stop();
            }
            else if (aim == ETimelineType.Enter_F) {
                enterF.Play();
                enterM.Stop();
                m2f.Stop();
                f2m.Stop();
            }
            else if (aim == ETimelineType.M_F) {
                m2f.Play();
                f2m.Stop();
                enterF.Stop();
                enterM.Stop();
            }
            else if (aim == ETimelineType.F_M) {
                f2m.Play();
                m2f.Stop();
                enterF.Stop();
                enterM.Stop();
            }
        }
    }


    public List<CreateCfg> createCfgs = new List<CreateCfg>();

    public void InitCreate() {
        for (int i = 0, length = createCfgs.Count; i < length; ++i) {
            var one = createCfgs[i];
            one.Init();
        }
    }

    public bool TryFindCreate(uint id, ref CreateCfg st) {
        for (int i = 0, length = createCfgs.Count; i < length; ++i) {
            if (createCfgs[i].careerId == id) {
                st = createCfgs[i];
                return true;
            }
        }

        return false;
    }

    // 传递非careerId表示全部隐藏
    public void CtrlCreate(uint id) {
        for (int i = 0, length = createCfgs.Count; i < length; ++i) {
            createCfgs[i].Visible(createCfgs[i].careerId == id);
        }
    }

    #endregion
    
    #region 捏脸stage
    [Serializable]
    public class MakefaceCfg {
        public Transform modelPos;
        public Transform farCameraPos;
        public Transform midCameraPos;
        public Transform nearCameraPos;
    }

    public MakefaceCfg makefaceCfg;

    #endregion
}