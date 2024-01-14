namespace Logic
{
    public class Singleton<T> where T : new()
    {
        protected Singleton() { }

        private static T instance;

        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}
