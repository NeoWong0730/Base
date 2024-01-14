using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public class NavigationComponent : MonoBehaviour, IComponent
    {
        #region Variables

        [HideInInspector]
        private NavMeshAgent _agent;
        public NavMeshAgent agent
        {
            get => _agent;
            set => _agent = value;
        }

        [SerializeField]
        [ReadOnly]
        private Vector3 _targetPoint;
        public Vector3 targetPoint
        {
            get => _targetPoint;
            set => _targetPoint = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _isNavigate;
        public bool isNavigate
        {
            get => _isNavigate;
            set
            {
                _isNavigate = value;
                agent.enabled = _isNavigate;
            }
        }

        #endregion
    }
}
