using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 控制等級的星星
public class CP_StarLevel : MonoBehaviour
{
    public GameObject proto;
    public Transform parent;

    private List<GameObject> allTrans = new List<GameObject>(5);

    public void Build(int max, System.Action<GameObject, int> action)
    {
        proto.SetActive(false);
        if (max > allTrans.Count)
        {
            for (int i = allTrans.Count; i < max; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(proto, parent);
                go.SetActive(true);
                allTrans.Add(go);
            }
        }

        for (int i = 0, length = allTrans.Count; i < length; ++i) {
            if (i < max) {
                allTrans[i].SetActive(true);
                action?.Invoke(allTrans[i], i);
            }
            else {
                allTrans[i].SetActive(false);
            }

        }
    }
}
