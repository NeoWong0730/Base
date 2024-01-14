using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NWFramework
{
    /// <summary>
    /// 游戏框架链表范围
    /// </summary>
    /// <typeparam name="T">指定链表范围的元素类型</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct NWFrameworkLinkedListRange<T> : IEnumerable<T>, IEnumerable
    {
        private readonly LinkedListNode<T> _first;
        private readonly LinkedListNode<T> _terminal;

        public NWFrameworkLinkedListRange(LinkedListNode<T> first, LinkedListNode<T> terminal)
        {
            if (first == null || terminal == null || first == terminal)
            {
                throw new NWFrameworkException("Range is invalid.");
            }

            _first = first; ;
            _terminal = terminal;
        }

        /// <summary>
        /// 获取链表范围是否有效
        /// </summary>
        public bool IsValid => _first != null && _terminal != null && _first != _terminal;

        /// <summary>
        /// 获取链表范围的开始结点
        /// </summary>
        public LinkedListNode<T> First => _first;

        /// <summary>
        /// 获取链表范围的终结标记结点
        /// </summary>
        public LinkedListNode<T> Terminal => _terminal;

        /// <summary>
        /// 获取链表范围的结点数量
        /// </summary>
        public int Count
        {
            get
            {
                if (!IsValid)
                {
                    return 0;
                }

                int count = 0;
                for (LinkedListNode<T> current = _first; current != null && current != _terminal; current = current.Next)
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// 检查是否包含指定值
        /// </summary>
        /// <param name="value">要检查的值</param>
        /// <returns>是否包含指定值</returns>
        public bool Contains(T value)
        {
            for (LinkedListNode<T> current = _first; current != null && current != _terminal; current = current.Next)
            {
                if (current.Value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly NWFrameworkLinkedListRange<T> m_NWFrameworkLinkedListRange;
            private LinkedListNode<T> m_Current;
            private T m_CurrentValue;

            internal Enumerator(NWFrameworkLinkedListRange<T> range)
            {
                if (!range.IsValid)
                {
                    throw new NWFrameworkException("Range is invalid.");
                }

                m_NWFrameworkLinkedListRange = range;
                m_Current = m_NWFrameworkLinkedListRange._first;
                m_CurrentValue = default(T);
            }

            /// <summary>
            /// 获取当前结点
            /// </summary>
            public T Current => m_CurrentValue;

            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current => m_CurrentValue;

            /// <summary>
            /// 清理枚举数
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// 获取下一个结点
            /// </summary>
            /// <returns>返回下一个结点</returns>
            public bool MoveNext()
            {
                if (m_Current == null || m_Current == m_NWFrameworkLinkedListRange._terminal)
                {
                    return false;
                }

                m_CurrentValue = m_Current.Value;
                m_Current = m_Current.Next;
                return true;
            }

            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                m_Current = m_NWFrameworkLinkedListRange._first;
                m_CurrentValue = default(T);
            }
        }
    }
}
