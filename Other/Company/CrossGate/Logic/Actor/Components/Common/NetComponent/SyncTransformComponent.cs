using UnityEngine;

namespace Logic
{
    public class SyncTransformComponent : Logic.Core.Component
    {
        public Vector3 netPos { get; private set; }
        public Vector3 netDir { get; private set; }

        //public void SyncNetPos(Vector3 pos)
        //{
        //    netPos = new Vector3(pos.x, 0, pos.z);
        //}

        public void SyncNetPos(Vector3 pos)
        {
            netPos = pos;
                //= new Vector3(pos.x, 0, pos.y);
        }

        public void SyncNetDir(Vector3 dir)
        {
            netDir = dir;
        }
    }
}
