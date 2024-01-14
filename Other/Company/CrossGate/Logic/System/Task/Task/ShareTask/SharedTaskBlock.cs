using Table;

namespace Logic
{
    public class SharedTaskBlock
    {
        public string ownerName { get; private set; }
        public TaskEntry taskEntry { get; private set; }
        public float receivedTime { get; set; } = 0;

        public SharedTaskBlock(uint id)
        {
            this.taskEntry = Sys_Task.Instance.GetTask(id);
        }
        public void SetData(string name)
        {
            this.ownerName = name;
        }
    }
}