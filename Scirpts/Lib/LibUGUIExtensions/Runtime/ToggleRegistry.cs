using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ToggleRegistry : MonoBehaviour {
    public bool allowSwitchOff = false;
    public List<ToggleEx> toggles { get; private set; } = new List<ToggleEx>();
    public int currentToggleID { get; private set; } = -1;
    // newId:oldId
    public Action<int, int, bool> onToggleChange;

    private Dictionary<int, Func<bool>> conditions = new Dictionary<int, Func<bool>>();

    #region condition

    /// <summary>
    /// 需要在onLoad里面去添加条件 保证在cp_toggle enable之前添加  
    /// </summary>
    /// <param name="id"></param>
    /// <param name="condition"></param>
    public void AddCondition(int id, Func<bool> condition) {
        conditions[id] = condition;
    }

    public void ClearCondition() {
        conditions.Clear();
    }

    public void RegisterCondition(int id) {
        if (conditions.TryGetValue(id, out Func<bool> func)) {
            for (int i = 0; i < toggles.Count; i++) {
                if (toggles[i].id == id) {
                    toggles[i].RegisterCondition(func);
                }
            }
        }
    }

    public void UnRegisterCondition(int id) {
        if (conditions.TryGetValue(id, out Func<bool> func)) {
            for (int i = 0; i < toggles.Count; i++) {
                if (toggles[i].id == id) {
                    toggles[i].UnRegisterCondition();
                }
            }
        }
    }

    #endregion

    public void NotifyOtherToggleOff(ToggleEx toggleEx, bool sendMessage, bool interaction = false) {
        if (toggleEx != null) {
            int old = currentToggleID;
            currentToggleID = toggleEx.id;
            for (var i = 0; i < toggles.Count; i++) {
                if (toggles[i] == toggleEx) {
                    continue;
                }
                toggles[i].SetSelected(false, sendMessage, interaction);
            }

            onToggleChange?.Invoke(currentToggleID, old, interaction);
        }
    }

    // 设置为-1的时候，相当于设置一个不存在的item,也就相当于目前所有的注册item全部被disactive
    public void SetHighLight(int id) {
        for (var i = 0; i < toggles.Count; i++) {
            toggles[i].Highlight(toggles[i].id == id);
        }
    }

    public ToggleEx GetToggleById(int id) {
        foreach (var tg in toggles) {
            if (tg != null && tg.id == id) {
                return tg;
            }
        }

        return null;
    }

    public void SwitchTo(int id, bool interaction = false/*交互点击true，否则false*/, bool ignoreOther = true, bool sendMessage = true) {
        if (ignoreOther) {
            for (var i = 0; i < toggles.Count; i++) {
                if (toggles[i].id == id) {
                    toggles[i].SetSelected(true, sendMessage, interaction);
                    break;
                }
            }
        }
        else {
            for (var i = 0; i < toggles.Count; i++) {
                toggles[i].SetSelected(toggles[i].id == id, sendMessage, interaction);
            }
        }
    }

    public void RegisterToggle(ToggleEx toggleEx) {
        if (!toggles.Contains(toggleEx)) {
            toggles.Add(toggleEx);
        }
    }

    public void UnregisterToggle(ToggleEx toggleEx) {
        if (toggles.Contains(toggleEx)) {
            toggles.Remove(toggleEx);
        }
    }

    public bool AnyToggleOn() {
        return toggles.Find(x => x.IsOn) != null;
    }
}