using GameServer;
using GameServer.Service;
using Serilog;

internal class Program
{
    static void Main(string[] args)
    {
        //初始化日志环境
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Async(a => a.Console())
            .WriteTo.Async(a => a.File("logs\\log.txt", rollingInterval: RollingInterval.Day))
            .CreateLogger();

        if (!DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
            return;

        //网路服务模块
        NetService netService = new NetService();
        netService.Start();
        Log.Debug("NetService start success");

        UserService userService = UserService.Instance;
        userService.Start();
        Log.Debug("UserService start success");

        while (true)
        {
            Thread.Sleep(100);
        }
    }
}
