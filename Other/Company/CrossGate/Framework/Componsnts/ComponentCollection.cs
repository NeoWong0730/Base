using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[DisallowMultipleComponent]
public class ComponentCollection : MonoBehaviour
{
    [SerializeField]
    public List<Component> components;

#if UNITY_EDITOR    
    public List<string> fieldNames;    
#endif
}
