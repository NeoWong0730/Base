using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
	[Serializable]
	public class JsonList<T>
	{
		[SerializeField]
		private List<T> target;

		public JsonList(List<T> target)
		{
			this.target = target;
		}

		public List<T> ToList()
		{
			return target;
		}
	}
}
