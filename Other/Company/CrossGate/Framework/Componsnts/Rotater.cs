using UnityEngine;

public class Rotater : MonoBehaviour {
    public enum ERotateType {
        X = 0,
        Y = 1,
        Z = 2,
    }

    public ERotateType rotateType = ERotateType.Y;
    public float speed = 100f;

    private Vector3 angle;

    private void Start() {
        this.angle = this.transform.localEulerAngles;
    }

    private void Update() {
        this.angle[(int)this.rotateType] += Time.deltaTime * this.speed;
        this.transform.localEulerAngles = this.angle;
    }
}
