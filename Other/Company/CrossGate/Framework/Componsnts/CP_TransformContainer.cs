using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CP_TransformContainer : MonoBehaviour
{
    public bool onlyActive = false;
    public List<Transform> actives = new List<Transform>(0);
    public List<Transform> deactives = new List<Transform>(0);

    public bool playOnEnable = false;

    private void OnEnable() {
        if (playOnEnable) {
            ShowHideBySetActive(true);
        }
    }
    private void OnDisable() {
        if (playOnEnable) {
            ShowHideBySetActive(false);
        }
    }

    public void ShowHideBySetActive(bool active)
    {
        if (!onlyActive) {
            foreach (Transform t in deactives) {
                if (t != null)
                    t.gameObject.SetActive(!active);
            }
        }
        foreach (Transform t in actives)
        {
            if(t != null)
                t.gameObject.SetActive(active);
        }
    }
}
