using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;

namespace Logic
{
    public class UIParseCommon
    {
        public GameObject gameObject
        {
            get;
            private set;
        }
        public Transform transform
        {
            get;
            private set;
        }

        public void Init(Transform trans)
        {
            this.transform = trans;
            this.gameObject = trans.gameObject;
            this.Parse();
        }

        protected virtual void Parse() { }
        public virtual void Show() {
            gameObject?.SetActive(true);
        }
        public virtual void Hide() {
            gameObject?.SetActive(false);
        }
        public virtual void OnDestroy() { }

        public virtual void UpdateInfo(ItemData item) { }
    }
}


