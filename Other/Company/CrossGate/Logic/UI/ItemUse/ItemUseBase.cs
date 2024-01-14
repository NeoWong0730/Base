
namespace Logic
{
    public abstract class ItemUseBase
    {
        protected ItemData _itemData;
        public ItemUseBase(ItemData itemData)
        {
            _itemData = itemData;
        }

        public virtual bool OnUse()
        {
            UnityEngine.Debug.LogError("ItemUseBase");
            return false;
        }
    }
}


