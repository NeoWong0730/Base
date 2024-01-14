using System.Collections.Generic;
using UnityEngine;
using Logic.Core;

public class UISelectableElement : UIElement
{
    protected System.Action<int, bool> onSelected = null;

    public UISelectableElement SetSelectedAction(System.Action<int, bool> onSelected)
    {
        this.onSelected = onSelected;
        return this;
    }
    public virtual void SetSelected(bool toSelected, bool force) { }
}

