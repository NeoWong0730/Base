using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTimerComponent : AComponentRepeat, IUpdate
{
    private float _total;
    private Action _overAction;
    private float _time;

    public override void Dispose()
    {
        _overAction = null;

        base.Dispose();
    }

    public void Init(float total, Action overAction)
    {
        if (total <= 0f || overAction == null)
        {
            overAction?.Invoke();
            Dispose();
            return;
        }

        _total = total;
        _overAction = overAction;
        _time = 0f;
    }

    public void Update()
    {
        if (_time > _total)
        {
            _overAction?.Invoke();
            Dispose();
            return;
        }

        _time += Time.deltaTime;
    }
}
