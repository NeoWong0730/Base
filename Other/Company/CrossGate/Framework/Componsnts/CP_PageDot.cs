using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CP_PageDot : MonoBehaviour
{
    public GameObject proto;
    public Transform parent;

    public System.Action<CP_Toggle, int> onClick;

    [SerializeField]
    private int current;
    [SerializeField]
    private int max;
    
    private List<CP_Toggle> all = new List<CP_Toggle>();

    public CP_PageDot SetMax(int max) { this.max = max; return this; }
    public CP_PageDot Build()
    {
        proto.SetActive(false);
        if (max > all.Count)
        {
            for (int i = all.Count; i < max; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(proto, parent);
                go.SetActive(true);
                CP_Toggle toggle = go.GetComponent<CP_Toggle>();
                int index = i;
                toggle.onValueChanged.AddListener((status) =>
                {
                    if (status) { onClick?.Invoke(toggle, index); }
                });
                all.Add(toggle);
            }
        }
        return this;
    }
    public void SetSelected(int index)
    {
        if (0 <= index && index < max)
        {
            all[index].SetSelected(true, true);
        }
    }
}
