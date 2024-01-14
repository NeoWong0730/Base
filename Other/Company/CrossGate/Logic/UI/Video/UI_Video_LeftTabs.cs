using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Video_LeftTabs : UIComponent
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

                Sys_Video.Instance.eventEmitter.Trigger<EVideoViewType>(Sys_Video.EEvents.OnSelectViewType, (EVideoViewType)(_index + 1));

            }
        }

        public void OnDefaultSelect(int defaultIndex)
        {
            Sys_Video.Instance.eventEmitter.Trigger(Sys_Video.EEvents.OnSelectViewType, (EVideoViewType)defaultIndex);           
            tabList[defaultIndex - 1].isOn = true;
            lastIndex = defaultIndex - 1;
        }
    }
}
