using System;
using UnityEngine;

// require: ps.main.stopAction = UnityEngine.ParticleSystemStopAction.Callback;
[RequireComponent(typeof(ParticleSystem))]
[DisallowMultipleComponent]
public class FxStopAction : MonoBehaviour {
    [Header("0表示self, 1表示第一级root...")] public uint upLevel = 0;
    public ParticleSystemStopAction stopAction = ParticleSystemStopAction.Disable;

    [SerializeField] private Transform upTr;
    [SerializeField] private ParticleSystem ps;

    private void Awake() {
        if (ps == null) {
            ps = this.GetComponent<ParticleSystem>();
        }
    }

    private void Start() {
        int up = (int) this.upLevel;
        upTr = transform;
        while (up > 0) {
            upTr = this.upTr.parent;
            if (this.upTr == null) {
                break;
            }

            up--;
        }
    }

    private void OnParticleSystemStopped() {
        if (upTr) {
            if (stopAction == ParticleSystemStopAction.Disable) {
                upTr.gameObject.SetActive(false);
            }
        }
    }
}