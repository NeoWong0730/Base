namespace Logic
{
    internal static class SystemModuleRegister
    {
        /// <summary>
        /// 判断模块有没有注册完
        /// </summary>
        public static bool IsRegister;
        internal static void Register()
        {
            IsRegister = true;
        }
        internal static void Unregister()
        {
            IsRegister = false;
        }
    }
}