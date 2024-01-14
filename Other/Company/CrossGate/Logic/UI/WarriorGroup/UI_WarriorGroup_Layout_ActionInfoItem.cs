using UnityEngine;
using Lib.Core;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_WarriorGroup_Layout
    {
        /// <summary>
        /// 动态消息Item///
        /// </summary>
        public class ActionInfoItem
        {
            public Sys_WarriorGroup.ActionInfo data;

            GameObject root;

            public Text contentTxt;
            public Text dateTxt;

            public void BindGameObject(GameObject gameObject)
            {
                root = gameObject;

                contentTxt = root.FindChildByName("Text_News").GetComponent<Text>();
                dateTxt = root.FindChildByName("Text_Time").GetComponent<Text>();
            }

            public void UpdateItem(Sys_WarriorGroup.ActionInfo actionInfo)
            {
                data = actionInfo;

                TextHelper.SetText(contentTxt, data.ToDesc());
                TextHelper.SetText(dateTxt, data.ToTime());
            }
        }
    }
}
