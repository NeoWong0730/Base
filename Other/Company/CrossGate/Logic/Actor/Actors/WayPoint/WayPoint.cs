
using System.Collections.Generic;
using Framework;
using UnityEngine.AI;
using Logic.Core;
using Table;
using UnityEngine;
using Lib.Core;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class WayPoint
    {
        public float DeathTime;
        public Transform transform;

        public void Enable(bool enable)
        {
            transform.gameObject.SetActive(enable);
        }
    }
}