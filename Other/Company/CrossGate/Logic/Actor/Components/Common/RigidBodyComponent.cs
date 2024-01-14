using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public class RigidBodyComponent : Logic.Core.Component {
        public Rigidbody rd;

        protected override void OnConstruct() {
            base.OnConstruct();

            var sceneActor = actor as SceneActor;

            // 添加
            Rigidbody rd = sceneActor.gameObject.GetNeedComponent<Rigidbody>();
            rd.useGravity = false;
            rd.freezeRotation = true;
            rd.constraints |= RigidbodyConstraints.FreezePositionY;
        }

        protected override void OnDispose()
        {
            rd = null;
            base.OnDispose();
        }
    }
}
