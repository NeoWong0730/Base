using UnityEngine;
using UnityEngine.UI;

// rt上添加该脚本，支持拖拽模型
[RequireComponent(typeof(Graphic))]
[DisallowMultipleComponent]
public class CP_DragTarget : MonoBehaviour {
    public Lib.Core.EventTrigger eventTrigger;

    public bool xAxis = true;
    public bool yAxis = false;
    public float speed = 1f;

    public Transform target;

    private void Awake() {
        if (this.eventTrigger == null) {
            this.eventTrigger = this.GetComponent<Lib.Core.EventTrigger>();
        }

        if (this.eventTrigger == null) {
            this.eventTrigger = this.gameObject.AddComponent<Lib.Core.EventTrigger>();
        }

        this.eventTrigger.onDrag += this.OnDrag;
    }

    private void OnDestroy() {
        if (this.eventTrigger != null) {
            this.eventTrigger.onDrag -= this.OnDrag;
        }
    }

    private void OnDrag(GameObject go, Vector2 delta) {
        if (this.target != null) {
            var euler = new Vector3(this.yAxis ? delta.y : 0f, this.xAxis ? -delta.x : 0f, 0f) * this.speed;

            // 方案1：
            // 单纯修改角度, 受到 万向锁 的影响
            // target.transform.localEulerAngles += euler;

            // 方案2：
            // 不会受到万向锁的影响, 但是始终绕着local坐标系旋转
            // target.transform.Rotate(euler, Space.Self);

            // 方案3：
            // 不会受到万向锁的影响, 类似unity的模型预览面板一样
            this.target.transform.Rotate(euler, Space.World);
        }
    }
}