using Framework.Core.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Core
{
    public class UIStack : Framework.Core.UI.FUIStack
    {        
        public UIStack() : base() { }

        protected override Framework.Core.UI.FUIBase CreateInstance(UIConfigData configData)
        {
            return System.Activator.CreateInstance(configData.script) as UIBase;
        }

        public void OnGUI()
        {
            GUILayout.Label("UIStack =====>");
            for (int i = mStack.Count - 1; i >= 0; --i)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Label(((EUIID)mStack[i].nID).ToString());
                GUILayout.Label(((EUIID)mStack[i].nParentID).ToString());// HasOptions(EUIOption.eHideBeforeUI).ToString()
                GUILayout.Label(mStack[i].eState.ToString());
                GUILayout.Label(mStack[i].nSortingOrder.ToString());
                GUILayout.Label(mStack[i].eOptions.ToString());// HasOptions(EUIOption.eHideBeforeUI).ToString()                
                GUILayout.EndHorizontal();
            }
            GUILayout.Label("UIStack =====<");
        }
    }
}