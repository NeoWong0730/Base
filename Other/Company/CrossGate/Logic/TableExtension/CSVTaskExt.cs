using Logic;
using System.Collections.Generic;

namespace Table
{
    public partial class CSVTask
    {
        public List<uint> GetTaskIDsByNPCIDAndTaskType(uint npcInfoID, ETaskCategory eTaskCategory)
        {
            List<uint> taskIDs = new List<uint>();

            foreach (CSVTask.Data taskData in GetAll())
            {
                if (taskData.taskCategory == (uint)eTaskCategory && taskData.receiveNpc == npcInfoID)
                {
                    taskIDs.Add(taskData.id);
                }
            }

            return taskIDs;
        }
    }
}
