using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [Serializable]
	public class ScriptableObjectDic<TKey, TValue> : ScriptableObject
	{
		[SerializeField]
		private List<TKey> keys;
		[SerializeField]
		private List<TValue> values;

		private Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                keys = new List<TKey>(target.Keys);
                values = new List<TValue>(target.Values);
            }
        }
	}
}
