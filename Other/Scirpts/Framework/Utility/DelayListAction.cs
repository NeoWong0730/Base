using System;
using System.Collections.Generic;

namespace Framework
{
    public class DelayListAction
    {
        private Dictionary<Func<bool>, List<Action>> actions = new Dictionary<Func<bool>, List<Action>>();

        public int Count => actions.Count;

        private List<Func<bool>> toDeleteList = new List<Func<bool>>();

        public bool TryTrigger(Func<bool> canTrigger, Action triggerAction)
        {
            bool can = canTrigger != null && triggerAction != null && canTrigger.Invoke();
            if (can) // 如果可触发,则立即执行,否则Update中等待合适时机触发
            {
                triggerAction.Invoke();
            }
            else
            {
                if (!actions.TryGetValue(canTrigger, out List<Action> list) || list == null)
                {
                    list = new List<Action>();
                    actions.Add(canTrigger, list);
                }
                list.Add(triggerAction);
            }
            return can;
        }

        public bool TryUnRegister(Func<bool> canTrigger, Action triggerAction)
        {
            bool can = canTrigger != null && triggerAction != null;
            if (can)
            {
                can = actions.TryGetValue(canTrigger, out List<Action> list) && list != null;
                if (can)
                {
                    int index = list.IndexOf(triggerAction);
                    can = index >= 0;
                    if (can)
                    {
                        int finalIndex = list.Count - 1;
                        void Swap(IList<Action> ls, int leftIndex, int rightIndex)
                        {
                            if (leftIndex != rightIndex)
                            {
                                var t = ls[leftIndex];
                                ls[leftIndex] = ls[rightIndex];
                                ls[rightIndex] = t;
                            }
                        }

                        Swap(list, index, finalIndex);
                        list.RemoveAt(finalIndex);
                    }
                }
            }
            return can;
        }

        public void Clear()
        {
            actions.Clear();
        }

        public void Update()
        {
            int count = actions.Count;
            if (count > 0)
            {
                foreach (var kvp in actions)
                {
                    if (kvp.Key())
                    {
                        for (int i = 0, length = kvp.Value.Count; i < length; ++i)
                        {
                            kvp.Value[i]?.Invoke();
                        }
                        toDeleteList.Add(kvp.Key);
                    }
                }
                if (toDeleteList.Count > 0)
                {
                    for (int i = 0, length = toDeleteList.Count; i < length; ++i)
                    {
                        actions.Remove(toDeleteList[i]);
                    }
                    toDeleteList.Clear();
                }
            }
        }
    }
}