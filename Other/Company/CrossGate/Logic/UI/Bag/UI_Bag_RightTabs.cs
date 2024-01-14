using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Bag_RightTabs : UIComponent
    {
        private List<Toggle> tabList = new List<Toggle>();
        private int lastIndex = -1;

        protected override void Loaded()
        {
            tabList.Clear();
            lastIndex = -1;

            Toggle[] toggles = transform.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                tabList.Add(toggle);
                toggle.onValueChanged.AddListener(var =>
                {
                    if (var)
                    {
                        int index = tabList.IndexOf(toggle);
                        OnSelect(index);
                    }
                });
            }
            toggles[0].isOn = true;
        }

        private void OnSelect(int _index)
        {
            if (lastIndex != _index)
            {
                lastIndex = _index;

                Sys_Bag.Instance.eventEmitter.Trigger<EBagViewType>(Sys_Bag.EEvents.OnSelectBagViewType, (EBagViewType)(_index + 1));
            }
        }

        public void OnDefaultSelect(int defaultIndex)
        {
            Sys_Bag.Instance.eventEmitter.Trigger(Sys_Bag.EEvents.OnSelectBagViewType, (EBagViewType)defaultIndex);
            tabList[defaultIndex - 1].isOn = true;
            lastIndex = defaultIndex - 1;
        }
    }
}
