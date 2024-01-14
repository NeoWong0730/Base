using UnityEngine;
using UnityEngine.UI;

public class UIFontMatEffect : MonoBehaviour {
    [Range(0, 1)]
    public float scheduleRange = 1;
    public Material mat;
    private void Awake() {
        //mat.shader.GetPropertyAttributes();
    }

    private void Update()
    {
        if(null != mat)
        {
            mat.SetFloat("_Schedule", scheduleRange);
        }
    }
}
