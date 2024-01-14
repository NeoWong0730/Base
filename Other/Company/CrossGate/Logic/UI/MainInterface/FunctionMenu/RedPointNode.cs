using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class RedPointNode
    {
        int _value;

        public Action OnChanged;
        RedPointNode parentNode;
        List<RedPointNode> childNodes = new List<RedPointNode>();

        public RedPointNode(RedPointNode parentNode = null)
        {
            Value = 0;
            if (parentNode != null)
            {
                this.parentNode = parentNode;
                this.parentNode.AddChild(this);
            }
        }

        /// <summary>
        /// 设置节点值
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                _value = value;
                OnChanged?.Invoke();

                if (parentNode != null)
                {
                    parentNode.SetValue();
                }
            }
        }

        /// <summary>
        /// 如果有子节点则算子节点红点总数
        /// </summary>
        private void SetValue()
        {
            int value = 0;
            for (int i = 0; i < childNodes.Count; i++)
            {
                value += childNodes[i].Value;
            }
            Value = value;
        }

        public void AddChild(RedPointNode node)
        {
            if (!childNodes.Contains(node))
            {
                node.parentNode = this;
                childNodes.Add(node);
                Value += node.Value; ;
            }
        }

        public void RemoveChild(RedPointNode node)
        {
            if (childNodes.Contains(node))
            {
                node.parentNode = null;
                childNodes.Remove(node);
                Value -= node.Value;
            }
        }
    }
}