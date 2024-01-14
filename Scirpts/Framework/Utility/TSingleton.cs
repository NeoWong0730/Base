namespace Framework
{
    public class TSingleton<T> where T : new()
    {
        protected TSingleton() { }

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