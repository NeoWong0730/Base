using System.Collections.Generic;
using Logic.Core;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

namespace Logic
{
    public class TaskEntry_Clue : TaskEntry
    {
        public TaskEntry_Clue() { }
        public TaskEntry_Clue(uint id) : base(id) { }

        public ClueTaskPhase ownerClueTaskPhase;
        public TaskEntry_Clue SetOwner(ClueTaskPhase owner)
        {
            this.ownerClueTaskPhase = owner;
            return this;
        }

        public ClueTask ownerClueTask { get { return this.ownerClueTaskPhase?.owner; } }
    }
}