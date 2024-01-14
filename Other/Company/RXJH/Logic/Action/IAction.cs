using UnityEngine;
using System;

namespace Logic
{
    public interface IAction
    {
        Action StartAction { get; }

        Action ActionStarted { get; }

        Action ActionCompleted { get; }

        Action ActionBeInterrupted { get; }

        void OnExecute();

        void OnAutoExecute();
    }
}
