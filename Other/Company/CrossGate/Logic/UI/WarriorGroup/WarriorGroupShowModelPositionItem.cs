namespace Logic
{
    public class WarriorGroupShowModelPositionItem
    {
        public VirtualGameObject VGO { get; set; } = null;

        public WarriorGroupModelShow ModelShow { get; private set; } = null;

        public void SetModelShow(WarriorGroupModelShow modelshow)
        {
            if (ModelShow == modelshow)
            {
                ModelShow.SetActive(true);
                return;
            }

            if (ModelShow != null)
                ModelShow.IsUsed = false;

            ModelShow = modelshow;

            if (ModelShow != null)
            {
                ModelShow.IsUsed = true;
                ModelShow.SetActive(true);
                ModelShow.SetParent(VGO);
            }
        }
    }
}