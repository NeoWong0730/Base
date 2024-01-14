using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVActionState
    {
        private List<CSVActionState.Data>  _showRoleActions;

        public List<CSVActionState.Data>  ShowRoleActions
        {
            get
            {
                if (_showRoleActions == null)
                {
                    _showRoleActions = new List<CSVActionState.Data>();
                    foreach (CSVActionState.Data data in GetAll())
                    {
                        if (data.Sort != 0)
                        {
                            _showRoleActions.Add(data);
                        }
                    }

                    _showRoleActions.Sort((a, b) =>
                    {
                        if (a.Sort > b.Sort)
                            return 1;
                        else
                            return -1;
                    });
                }

                return _showRoleActions;
            }
        }
        HashSet<uint> actions = null;
        public HashSet<uint> GetHeroPreLoadActions()
        {
            if (actions != null)
                return actions;
            //List<uint> actions = new List<uint>();
            actions = new HashSet<uint>();
            foreach (CSVActionState.Data data in GetAll())
            {
                if (data.Sort != 0)
                {
                    actions.Add(data.id);
                }
            }

            if (!actions.Contains((uint)EStateType.Idle))
            {
                actions.Add((uint)EStateType.Idle);
            }

            if (!actions.Contains((uint)EStateType.Run))
            {
                actions.Add((uint)EStateType.Run);
            }

            if (!actions.Contains((uint)EStateType.Walk))
            {
                actions.Add((uint)EStateType.Walk);
            }

            if (!actions.Contains((uint)EStateType.Mining))
            {
                actions.Add((uint)EStateType.Mining);
            }

            if (!actions.Contains((uint)EStateType.Logging))
            {
                actions.Add((uint)EStateType.Logging);
            }

            if (!actions.Contains((uint)EStateType.fishing))
            {
                actions.Add((uint)EStateType.fishing);
            }

            if (!actions.Contains((uint)EStateType.hunting))
            {
                actions.Add((uint)EStateType.hunting);
            }

            if (!actions.Contains((uint)EStateType.Collection))
            {
                actions.Add((uint)EStateType.Collection);
            }

            if (!actions.Contains((uint)EStateType.Collection2))
            {
                actions.Add((uint)EStateType.Collection2);
            }

            if (!actions.Contains((uint)EStateType.Inquiry))
            {
                actions.Add((uint)EStateType.Inquiry);
            }

            if (!actions.Contains((uint)EStateType.InquiryEnd))
            {
                actions.Add((uint)EStateType.InquiryEnd);
            }

            if (!actions.Contains((uint)EStateType.NormalAttack))
            {
                actions.Add((uint)EStateType.NormalAttack);
            }

            if (!actions.Contains((uint)EStateType.Defense))
            {
                actions.Add((uint)EStateType.Defense);
            }

            if (!actions.Contains((uint)EStateType.mount_1_inquiry))
            {
                actions.Add((uint)EStateType.mount_1_inquiry);
            }

            if (!actions.Contains((uint)EStateType.mount_1_idle))
            {
                actions.Add((uint)EStateType.mount_1_idle);
            }

            if (!actions.Contains((uint)EStateType.mount_1_show_idle))
            {
                actions.Add((uint)EStateType.mount_1_show_idle);
            }

            if (!actions.Contains((uint)EStateType.mount_1_run))
            {
                actions.Add((uint)EStateType.mount_1_run);
            }

            if (!actions.Contains((uint)EStateType.mount_1_walk))
            {
                actions.Add((uint)EStateType.mount_1_walk);
            }

            if (!actions.Contains((uint)EStateType.mount_2_inquiry))
            {
                actions.Add((uint)EStateType.mount_2_inquiry);
            }

            if (!actions.Contains((uint)EStateType.mount_2_idle))
            {
                actions.Add((uint)EStateType.mount_2_idle);
            }

            if (!actions.Contains((uint)EStateType.mount_2_show_idle))
            {
                actions.Add((uint)EStateType.mount_2_show_idle);
            }

            if (!actions.Contains((uint)EStateType.mount_2_run))
            {
                actions.Add((uint)EStateType.mount_2_run);
            }

            if (!actions.Contains((uint)EStateType.mount_2_walk))
            {
                actions.Add((uint)EStateType.mount_2_walk);
            }

            if (!actions.Contains((uint)EStateType.mount_2_walk))
            {
                actions.Add((uint)EStateType.mount_2_walk);
            }

            if (!actions.Contains((uint)EStateType.CollectEnd))
            {
                actions.Add((uint)EStateType.CollectEnd);
            }

            return actions;
        }
    }
}
