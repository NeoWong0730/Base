namespace NWFramework
{
    /// <summary>
    /// 资源管理日志实现器
    /// </summary>
    internal class AssetsLogger : YooAsset.ILogger
    {
        public void Log(string message)
        {
            NWFramework.Log.Info(message);
        }

        public void Warning(string message)
        {
            NWFramework.Log.Warning(message);
        }

        public void Error(string message)
        {
            NWFramework.Log.Error(message);
        }

        public void Exception(System.Exception exception)
        {
            NWFramework.Log.Fatal(exception.Message);
        }
    }
}