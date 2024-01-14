using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Head_LeftTabs : UIComponent
    {

        private List<CP_Toggle> tabList = new List<CP_Toggle>();
        private int lastIndex = -1;

        protected override void Loaded()
        {
            tabList.Clear();
            lastIndex = -1;

            CP_Toggle[] toggles = transform.GetComponentsInChildren<CP_Toggle>();
            foreach (CP_Toggle toggle in toggles)
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
            toggles[0].SetSelected(true, true);

        }

        private void OnSelect(int _index)
        {
            if (lastIndex != _index)
            {
                lastIndex = _index;

                Sys_Head.Instance.eventEmitter.Trigger<EHeadViewType>(Sys_Head.EEvents.OnSelectViewType, (EHeadViewType)(_index + 1));
            }
        }

        public void OnDefaultSelect(int defaultIndex)
        {
            if (defaultIndex == lastIndex + 1)
            {
                Sys_Head.Instance.eventEmitter.Trigger<EHeadViewType>(Sys_Head.EEvents.OnSelectViewType, (EHeadViewType)defaultIndex);
            }
            tabList[defaultIndex - 1].SetSelected (true, true);
        }

        public void ShowRedPoint(EHeadViewType type,bool isShow)
        {
            if (type==EHeadViewType.None)
            {
                for(int i=0;i< tabList.Count; ++i)
                {
                    tabList[i].transform.Find("Image_Dot").gameObject.SetActive(false);
                }
            }
            else
            {
                tabList[(int)type-1].transform.Find("Image_Dot").gameObject.SetActive(isShow);
            }
        }
    }

}