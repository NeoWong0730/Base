using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework {
    // 家族资源战 阻挡墙
    public class CollisionWall : MonoBehaviour {
        public static Action<bool> onAction;

        public enum ECamp {
            Left = 1,
            Right = 2,
        }

        [System.Serializable]
        public class Wall {
            // 透明
            public GameObject red;
            // 阻挡
            public GameObject blue;

            public void Ctrl(bool toActive) {
                if (this.red) {
                    this.red.SetActive(toActive);
                }
                if (this.blue) {
                    this.blue.SetActive(!toActive);
                }
            }
        }

        // 左侧
        public List<Wall> left = new List<Wall>();
        // 右侧
        public List<Wall> right = new List<Wall>();

        public static CollisionWall Instance { get; private set; } = null;

#if UNITY_EDITOR
        public bool accountIsLeftCamp;
        public bool canCross;

        [ContextMenu(nameof(Ctrl))]
        private void Ctrl() {
            this.Ctrl(this.accountIsLeftCamp, this.canCross);
        }
#endif

        public void Ctrl(bool accountIsLeftCamp, bool canCross) {
#if UNITY_EDITOR
            this.accountIsLeftCamp = accountIsLeftCamp;
            this.canCross = canCross;
#endif

            void Block(List<Wall> walls, bool toActive) {
                foreach (var one in walls) {
                    one.Ctrl(toActive);    // 墙体特效
                }
            }

            if (accountIsLeftCamp) {
                Block(this.right, false);

                if (canCross) {
                    Block(this.left, true);
                }
                else {
                    Block(this.left, false);
                }
            }
            else {
                Block(this.left, false);

                if (canCross) {
                    Block(this.right, true);
                }
                else {
                    Block(this.right, false);
                }
            }
        }

        private void Awake() {
            Instance = this;
            onAction?.Invoke(true);
        }
        private void OnDestroy() {
            Instance = null;
            onAction?.Invoke(false);
        }
    }
}