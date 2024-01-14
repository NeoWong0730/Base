using UnityEngine;

namespace Logic.Core
{
    public class SceneActorWrap : MonoBehaviour
    {
        public Actor sceneActor;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //TeleporterActor tel = sceneActor as TeleporterActor;
            //if (tel != null)
            //{
            //    Rect rectTel = tel.telCo  m.rectTel;
            //    Vector3 topLeft = new Vector3(rectTel.position.x - rectTel.size.x * 0.5f, 0f, rectTel.position.y + rectTel.size.y * 0.5f);
            //    //Debug.LogError(topLeft.ToString());
            //    Vector3 topRight = new Vector3(rectTel.position.x + rectTel.size.x * 0.5f, 0f, rectTel.position.y + rectTel.size.y * 0.5f);
            //    //Debug.LogError(topRight.ToString());
            //    Vector3 bottomRight = new Vector3(rectTel.position.x + rectTel.size.x * 0.5f, 0f, rectTel.position.y - rectTel.size.y * 0.5f);
            //    //Debug.LogError(bottomRight.ToString());
            //    Vector3 bottomLeft = new Vector3(rectTel.position.x - rectTel.size.x * 0.5f, 0f, rectTel.position.y - rectTel.size.y * 0.5f);
            //    //Debug.LogError(bottomLeft.ToString());

            //    Gizmos.DrawLine(topLeft, topRight);
            //    Gizmos.DrawLine(topRight, bottomRight);
            //    Gizmos.DrawLine(bottomRight, bottomLeft);
            //    Gizmos.DrawLine(bottomLeft, topLeft);
            //}
        }
#endif
    }
}
