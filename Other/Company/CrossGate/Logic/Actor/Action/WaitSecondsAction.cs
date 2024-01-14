using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 寻路行为///
    /// </summary>
    public class WaitSecondsAction : ActionBase
    {
        //public const string TypeName = "Logic.WaitSecondsAction";

        public float seconds = 0.3f;
        private float startTime;

        protected override void OnDispose()
        {
            seconds = 0.3f;
            startTime = 0f;

            base.OnDispose();
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            startTime = Time.time;
        }

        public override bool IsCompleted()
        {
            return Time.time - startTime >= seconds;
        }
    }
}
