using System.Collections;
using UnityEngine.Events;

namespace Framework
{
    public class ClickItemEvent : UnityEvent<int>
    {
        public ClickItemEvent() { }
    }

    public class ClickToggleItemEvent : UnityEvent<int, bool>
    {
        public ClickToggleItemEvent() { }
    }
}
