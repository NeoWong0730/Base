namespace Framework
{
    public class HotFixTipWordManager : TSingleton<HotFixTipWordManager>
    {
        public HotFixTipWord hotFixTipWord { get; private set; }
        
        public void Init()
        {
            hotFixTipWord = new HotFixTipWord();
        }
    }
}