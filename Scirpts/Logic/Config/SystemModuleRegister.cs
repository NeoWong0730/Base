namespace Logic
{
    internal static class SystemModuleRegister
    {
        /// <summary>
        /// �ж�ģ����û��ע����
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