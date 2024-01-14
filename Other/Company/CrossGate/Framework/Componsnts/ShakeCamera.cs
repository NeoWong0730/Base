using Cinemachine;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class ShakeCamera : MonoBehaviour {
    public new Camera camera;
    public CinemachineBrain brain;
    public bool resetWhenFinish = true;

    [SerializeField] private Vector3 posBeforeShake;

    private Tweener shakeTweener;

    private void Awake() {
        if (this.camera == null) {
            this.camera = this.GetComponent<Camera>();
        }
        if (this.brain == null) {
            this.brain = this.GetComponent<CinemachineBrain>();
        }
    }

    private void OnEnable() {
        if (this.camera != null) {
            this.posBeforeShake = this.camera.transform.position;
        }
    }

#if UNITY_EDITOR
    [SerializeField] private float strength_x;
    [SerializeField] private float strength_y;
    [SerializeField] private float strength_z;
    [SerializeField] private float duration;
    [SerializeField] private float vibrato;
    [SerializeField] private float randomness;
#endif

    [ContextMenu("BeginShake")]
    private void BeginShake() {
#if UNITY_EDITOR
        Vector3 strength = new Vector3(this.strength_x / 1000f, this.strength_y / 1000f, this.strength_z / 1000f);
        this.BeginShake(this.duration / 1000f, strength, (int)this.vibrato, this.randomness);
#endif
    }

    public void BeginShake(float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true) {
#if UNITY_EDITOR
        this.duration = duration;
        this.strength_x = strength.x;
        this.strength_y = strength.y;
        this.strength_z = strength.z;
        this.vibrato = vibrato;
        this.randomness = randomness;
#endif
        if (this.camera != null) {
            this.posBeforeShake = this.camera.transform.position;
        }

        if (this.shakeTweener != null) {
            this.shakeTweener.Kill(true);
        }

        if (this.brain) {
            this.brain.enabled = false;
        }

        this.shakeTweener = DOTween.Shake(this.GetShakeOffset, this.SetShakeOffset, duration, strength, vibrato, randomness, fadeOut);
        this.shakeTweener.onComplete += this.OnTweenFinish;
        this.shakeTweener.onKill += this.OnTweenFinish;
    }

    [ContextMenu("EndShake")]
    public void EndShake() {
        if (this.shakeTweener != null) {
            this.shakeTweener.Kill(true);
        }
    }

    private void OnTweenFinish() {
        if (this.resetWhenFinish) {
            this.camera.transform.position = this.posBeforeShake;
        }
        if (this.brain) {
            this.brain.enabled = true;
        }

        this.shakeTweener.onComplete -= this.OnTweenFinish;
        this.shakeTweener.onKill -= this.OnTweenFinish;
        this.shakeTweener = null;
    }

    private Vector3 shakeOffset;
    private Vector3 GetShakeOffset() {
        return this.shakeOffset;
    }
    private void SetShakeOffset(Vector3 vector) {
        this.shakeOffset = vector;
    }

    private void LateUpdate() {
        if (this.shakeOffset != Vector3.zero) {
            Vector3 pos = this.posBeforeShake + this.shakeOffset;
            this.camera.transform.position = pos;
        }
    }
}
