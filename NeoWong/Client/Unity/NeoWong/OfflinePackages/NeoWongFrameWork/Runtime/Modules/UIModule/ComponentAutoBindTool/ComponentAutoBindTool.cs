using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWFramework
{
    /// <summary>
    /// ����Զ��󶨹���
    /// </summary>
    public class ComponentAutoBindTool : MonoBehaviour
    {
#if UNITY_EDITOR
        [Serializable]
        public class BindData
        {
            public BindData() { }

            public BindData(string name, Component bindCom, bool isGameObject = false) 
            {
                Name = name;
                BindCom = bindCom;
                IsGameObject = isGameObject;
            }

            public string Name;
            public Component BindCom;
            public bool IsGameObject;
        }

        public List<BindData> BindDatas = new List<BindData>();

        [SerializeField]
        private string m_ClassName;

        [SerializeField]
        private string m_Namespace;

        [SerializeField]
        private string m_CodePath;

        [SerializeField]
        private bool m_IsWidget;

        public string ClassName => m_ClassName;

        public string Namespace => m_Namespace;

        public string CodePath => m_CodePath;

        public bool IsWidget => m_IsWidget;

        public IAutoBindRuleHelper RuleHelper { get; set; }
#endif
        [SerializeField]
        public List<Component> bindComponents = new List<Component>();
        
        public T GetBindComponent<T>(int index) where T : Component
        {
            if (index >= bindComponents.Count)
            {
                Debug.LogError("������Ч");
                return null;
            }

            T bindCom = bindComponents[index] as T;

            if (bindCom == null)
            {
                Debug.LogError("������Ч");
                return null;
            }

            return bindCom;
        }
    }
}