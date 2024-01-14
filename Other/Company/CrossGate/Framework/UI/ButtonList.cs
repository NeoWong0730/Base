using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ButtonList : MonoBehaviour
{
    [SerializeField]
    private Button[] _buttons;
    [SerializeField]
    private int4[] _args;

    public System.Action<int, int, int, int> onClick;

    private void Start()
    {
        for (int i = 0; i < _buttons.Length; ++i)
        {
            int index = i;
            _buttons[index].onClick.AddListener(() => 
            {
                onClick?.Invoke(_args[index].x, _args[index].y, _args[index].z, _args[index].w); 
            }
            );
        }
    }

    public void ShowBtn(bool show)
    {
        for (int i = 0; i < _buttons.Length; ++i)
            _buttons[i].gameObject.SetActive(show);
    }

    public void SetRuleId(int ruleId)
    {
        for (int i = 0; i < _args.Length; ++i)
            _args[i].y = ruleId;
    }

#if UNITY_EDITOR
    [ContextMenu("Bake Buttons")]
    public void Bake()
    {
        string BtnHelp = "BtnHelp_";

        Button[] btns = GetComponentsInChildren<Button>();

        List<Button> buttons = new List<Button>();
        List<int4> args = new List<int4>();

        for (int i = 0; i < btns.Length; ++i)
        {
            string btnName = btns[i].name;
            if (btnName.StartsWith(BtnHelp, System.StringComparison.Ordinal))
            {
                string v = btnName.Remove(0, BtnHelp.Length);
                if (int.TryParse(v, out int iv))
                {
                    if (iv == 0)
                    {
                        buttons.Insert(0, btns[i]);
                        args.Insert(0, new int4(0, 0, 0, 0));
                    }
                    else
                    {
                        buttons.Insert(0, btns[i]);
                        args.Add(new int4(0, iv, 0, 0));
                    }
                }
            }
        }

        _buttons = buttons.ToArray();
        _args = args.ToArray();
    }
#endif
}