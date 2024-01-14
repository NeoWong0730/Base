namespace Logic.Core
{
    internal static class SystemModuleRegister
    {
        /// <summary>
        /// 判断模块有没有注册完
        /// </summary>
        public static bool IsRegister;
        internal static void Register()
        {
            SystemModuleManager.RegisterSystemModel(Sys_Net.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_ShieldedWord.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_AccountName.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_Time.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_RefreshTimerRegistry.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_Login.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_Server.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_Scene.ConstructInstance());
            SystemModuleManager.RegisterSystemModel(Sys_Task.ConstructInstance());
            
            IsRegister = true;
        }
        internal static void Unregister()
        {
            Sys_Net.DisposeInstance();
            Sys_ShieldedWord.DisposeInstance();
            Sys_AccountName.DisposeInstance();
            Sys_Time.DisposeInstance();
            Sys_RefreshTimerRegistry.DisposeInstance();
            Sys_Login.DisposeInstance();
            Sys_Server.DisposeInstance();
            Sys_Scene.DisposeInstance();
            Sys_Task.DisposeInstance();
            
            IsRegister = false;
        }
    }
}