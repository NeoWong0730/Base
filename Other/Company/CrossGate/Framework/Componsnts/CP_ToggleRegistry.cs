using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

[DisallowMultipleComponent]
public class CP_ToggleRegistry : MonoBehaviour
{
    public bool allowSwitchOff = false;
    public List<CP_Toggle> toggles { get; private set; } = new List<CP_Toggle>();
    public int currentToggleID { get; private set; } = -1;
    public Action<int, int> onToggleChange;

    private Dictionary<int, Func<bool>> conditions = new Dictionary<int, Func<bool>>();

    #region condition
    /// <summary>
    /// 需要在onLoad里面去添加条件 保证在cp_toggle enable之前添加  
    /// </summary>
    /// <param name="id"></param>
    /// <param name="condition"></param>
    public void AddCondition(int id,Func<bool> condition)
    {
        conditions[id] = condition;
    }

    public void ClearCondition()
    {
        conditions.Clear();
    }

    public void RegisterCondition(int id)
    {
        if( conditions.TryGetValue(id,out Func<bool> func))
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].id == id)
                {
                    toggles[i].RegisterCondition(func);
                }
            }
        }
    }

    public void UnRegisterCondition(int id)
    {
        if (conditions.TryGetValue(id, out Func<bool> func))
        {
            for (int i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].id == id)
                {
                    toggles[i].UnRegisterCondition();
                }
            }
        }
    }

    #endregion

    public void NotifyOtherToggleOff(CP_Toggle toggle, bool sendMessage)
    {
        if (toggle != null)
        {
            int old = currentToggleID;
            currentToggleID = toggle.id;
            for (var i = 0; i < toggles.Count; i++)
            {
                if (toggles[i] == toggle) continue;
                toggles[i].SetSelected(false, sendMessage);
            }

            onToggleChange?.Invoke(currentToggleID, old);
        }
    }

    // 设置为-1的时候，相当于设置一个不存在的item,也就相当于目前所有的注册item全部被disactive
    public void SetHighLight(int id)
    {
        for (var i = 0; i < toggles.Count; i++)
        {
            toggles[i].Highlight(toggles[i].id == id);
        }
    }

    public CP_Toggle GetToggleById(int id) {
        foreach (var tg in toggles) {
            if (tg != null && tg.id == id) {
                return tg;
            }
        }

        return null;
    }

    public void SwitchTo(int id, bool ignoreOther = true, bool sendMessage = true)
    {
        if (ignoreOther)
        {
            for (var i = 0; i < toggles.Count; i++)
            {
                if (toggles[i].id == id)
                {
                    toggles[i].SetSelected(true, sendMessage);
                    break;
                }
            }
        }
        else
        {
            for (var i = 0; i < toggles.Count; i++)
            {
                toggles[i].SetSelected(toggles[i].id == id, sendMessage);
            }
        }
    }

    public void RegisterToggle(CP_Toggle toggle) { if (!toggles.Contains(toggle)) { toggles.Add(toggle); } }
    public void UnregisterToggle(CP_Toggle toggle) { if (toggles.Contains(toggle)) { toggles.Remove(toggle); } }
    public bool AnyToggleOn() { return toggles.Find(x => x.IsOn) != null; }
}
