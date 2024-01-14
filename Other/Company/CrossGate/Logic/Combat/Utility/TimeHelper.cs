using System;

public static class TimeHelper
{
    //时间是毫秒
    private static readonly DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly long epoch = dateTime.Ticks;

    public static long Now()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000;
    }

    public static long ClientNowSeconds()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000000;
    }

    public static long CurrentTimeMillis()
    {
        return (long)(DateTime.UtcNow - dateTime).TotalMilliseconds;
    }

   
}