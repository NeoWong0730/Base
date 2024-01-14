namespace NWFramework
{
    /// <summary>
    /// 游戏框架字典类(已排序)
    /// </summary>
    /// <typeparam name="Tkey">指定字典Key的元素类型</typeparam>
    /// <typeparam name="TValue">指定字典Value的元素类型</typeparam>
    public class NWFrameworkSortedDictionary<Tkey, TValue> : NWFrameworkDictionary<Tkey, TValue>
    {
        public override void Add(Tkey key, TValue item)
        {
            base.Add(key, item);
            KeyList.Sort();
        }

    }
}