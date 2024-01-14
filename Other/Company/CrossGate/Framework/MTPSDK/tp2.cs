using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;

namespace tss
{
    public interface TssInfoReceiver
    {
        void onReceive(int tssInfoType, string info);
    }

    public class TssInfoPublisher
    {
        public const int TSS_INFO_TYPE_DETECT_RESULT = 1;  // 检测结果
        public const int TSS_INFO_TYPE_HEARTBEAT = 2;      // 心跳
        private TssInfoPublisher()
        {
        }
        private static volatile TssInfoPublisher mInstance = null;
        private static readonly object mSingletonLock = new object();
        public static TssInfoPublisher getInstance()
        {
            if (mInstance == null)
            {
                lock (mSingletonLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = new TssInfoPublisher();
                    }
                }

            }
            return mInstance;
        }

        private readonly object padlockReceiver = new object();
        private static List<TssInfoReceiver> mReceivers = new List<TssInfoReceiver>();
        private static Thread mTssInfoPublisherThread = null;
        private static volatile bool mTssInfoPublisherThreadStarted = false;
        public void registTssInfoReceiver(TssInfoReceiver receiver)
        {
            if (receiver == null) return;
            if (!mTssInfoPublisherThreadStarted)
            {
                lock (padlockReceiver)
                {
                    if (!mTssInfoPublisherThreadStarted)
                    {
                        mTssInfoPublisherThreadStarted = true;
                        mTssInfoPublisherThread = new Thread(getInstance().recvDataThread);
                        mTssInfoPublisherThread.IsBackground = true;
                        mTssInfoPublisherThread.Start();
                    }
                }
            }
            lock (padlockReceiver)
            {
                mReceivers.Add(receiver);
            }

        }

        private void broadcastInfo(int id, string info)
        {
            lock (padlockReceiver)
            {
                foreach (TssInfoReceiver r in mReceivers)
                {
                    r.onReceive(id, info);
                }
            }
        }

        private void recvDataThread()
        {
            int ret = openPipe();
            if (ret != 0)
            {
                mTssInfoPublisherThreadStarted = false;
                return;
            }

            try
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                    }
                    string info = recvPipe();
                    if (info == null) break;
                    int pos = info.IndexOf('|');
                    if (pos == -1) break;
                    int id = Int32.Parse(info.Substring(0, pos));
                    string msg = info.Substring(pos + 1);
                    broadcastInfo(id, msg);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                closePipe();
                mTssInfoPublisherThreadStarted = false;
            }
        }

        private static int openPipe()
        {
            string info = Tp2Sdk.Tp2Ioctl("ilc_open_pipe");
            int ret = -1;
            try
            {
                ret = Int32.Parse(info);
            }
            catch (Exception)
            {
            }
            return ret;
        }

        private static void closePipe()
        {
            Tp2Sdk.Tp2Ioctl("ilc_close_pipe");
        }

        private static string recvPipe()
        {
            String info = Tp2Sdk.Tp2Ioctl("ilc_recv_pipe");
            return info;
        }
    }
}

public static class Tp2Sdk{

	// sdk anti-data info
	[StructLayout(LayoutKind.Sequential)]
	private class AntiDataInfo
	{
		//[FieldOffset(0)]
		public ushort anti_data_len;
		//[FieldOffset(2)]
		public IntPtr anti_data;
	};

    public const int TssSDKCmd_IsEmulator = 10;
    private const int TssSDKCmd_CommQuery = 18;

    public static void Tp2RegistTssInfoReceiver(tss.TssInfoReceiver receiver)
    {
        tss.TssInfoPublisher.getInstance().registTssInfoReceiver(receiver);
    }

    public static string Tp2DecTssInfo(string info)
    {
        string cmd = String.Format("dec_tss_info:{0}", info);
        return Tp2Ioctl(cmd);
    }
    public static void Tp2SdkInitEx(int gameId, string appKey)
    {
        Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eSdk, string.Format("初始化：{0} {1}", gameId, appKey));
        tp2_sdk_init_ex(gameId, appKey);
    }

    public static void Tp2UserLogin(int accountType, int worldId, string openId, string roleId)
    {
        Lib.Core.DebugUtil.LogFormat(Lib.Core.ELogType.eSdk, string.Format("用户登录：{0} {1} {2} {3}", accountType, worldId, openId, roleId));
        tp2_setuserinfo(accountType, worldId, openId, roleId);
    }

    public static void Tp2SetGamestatus(Tp2GameStatus status)
    {
        const int FRONTEND = 0x1000;
        const int BACKEND = 0x2000;

        //Debug.Log("设置用户状态：" + status.ToString());
        switch (status)
        {
            case Tp2GameStatus.FRONTEND:
                tp2_setoptions(FRONTEND);
                break;
            case Tp2GameStatus.BACKEND:
                tp2_setoptions(BACKEND); ;
                break;
            default:
                break;
        }
    }

    public static int Tp2SetLocale(int locale_id)
    {
        string locale_cmd = "SetLocaleId:" + locale_id;
        return (int)tp2_sdk_ioctl(TssSDKCmd_CommQuery, locale_cmd);
    }

    public static void EnableGameReport()
    {
        tp2_sdk_ioctl(TssSDKCmd_CommQuery, "EnableGameReport");
    }
    public static string Ioctl(int request, string cmd)
	{
		//调用native实现
		IntPtr addr = tp2_sdk_ioctl(request, cmd);
		if (addr == IntPtr.Zero)
		{
			return null;
		}
		//解析返回结果
		Tp2Sdk.AntiDataInfo info = new Tp2Sdk.AntiDataInfo();
		info.anti_data_len = (ushort)Marshal.ReadInt16(addr, 0);
		info.anti_data = ReadIntPtr(addr, 2);

		byte[] bytes = new byte[info.anti_data_len];
		Marshal.Copy(info.anti_data, bytes, 0, info.anti_data_len);
		
		//释放native的结果
		tp2_free_anti_data(addr);
		//
		string response_buf = System.Text.Encoding.ASCII.GetString(bytes);
		return response_buf;
	}
	public static string Tp2Ioctl(string cmd)
	{
		//调用native实现
		IntPtr addr = tp2_sdk_ioctl(TssSDKCmd_CommQuery, cmd);
		if (addr == IntPtr.Zero)
		{
			return null;
		}
		//解析返回结果
		Tp2Sdk.AntiDataInfo info = new Tp2Sdk.AntiDataInfo();
		info.anti_data_len = (ushort)Marshal.ReadInt16(addr, 0);
		info.anti_data = ReadIntPtr(addr, 2);

		byte[] bytes = new byte[info.anti_data_len];
		Marshal.Copy(info.anti_data, bytes, 0, info.anti_data_len);
		
		//释放native的结果
		tp2_free_anti_data(addr);
		//
		string response_buf = System.Text.Encoding.ASCII.GetString(bytes);
		return response_buf;
	}

	private static Boolean Is64bit()
	{
		return IntPtr.Size == 8;
	}

	private static Boolean Is32bit()
	{
		return IntPtr.Size == 4;
	}
	/**
	 * 读取指针
	 * 
	 * 说明:直接使用Marshal.ReadIntPtr是不可以的，测试的时候，在一些老版本的Unity(4.*)上，在编译成64ios工程后会编译不过
	 * 		所以需要先按机器位数(32/64)读整形，再转成IntPtr指针
	 */
	private static IntPtr ReadIntPtr(IntPtr addr, int off)
	{
		IntPtr ptr = IntPtr.Zero;
		if (Tp2Sdk.Is64bit())
		{
			Int64 v64 = Marshal.ReadInt64(addr, off);
			ptr = new IntPtr(v64);
		}
		else
		{
			Int32 v32 = Marshal.ReadInt32(addr, off);
			ptr = new IntPtr(v32);
		}
		return ptr;
	}

	public static byte[] Tp2GetReportData()
	{
		IntPtr addr = tss_get_report_data();
		if (addr == IntPtr.Zero)
		{
			return null;
		}
		ushort anti_data_len = (ushort)Marshal.ReadInt16(addr, 0);
		IntPtr anti_data = ReadIntPtr(addr, 2);

		//
		if (anti_data == IntPtr.Zero)
		{
			tss_del_report_data(addr);
			return null;
		}
		//
		byte[] data = new byte[anti_data_len];
		Marshal.Copy(anti_data, data, 0, anti_data_len);
		
		//
		tss_del_report_data(addr);
		//
		return data;
	}

	public static byte[] Tp2GetReportData2()
	{
		IntPtr addr = tss_get_report_data2();
		if (addr == IntPtr.Zero)
		{
			return null;
		}
		ushort anti_data_len = (ushort)Marshal.ReadInt16(addr, 0);
		IntPtr anti_data = ReadIntPtr(addr, 2);
		//
		if (anti_data == IntPtr.Zero || anti_data_len > 2048)
		{
			return null;
		}
		//
		byte[] data = new byte[anti_data_len];
		Marshal.Copy(anti_data, data, 0, anti_data_len);
		//
		return data;
	}

	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe2")]
	#endif
	private static extern int tp2_sdk_init_ex(int gameId, string appKey);

	#if UNITY_IOS
	[DllImport("__Internal")]
	#else	
	[DllImport("tersafe2")]
	#endif
	private static extern int tp2_setuserinfo(int accountType, int worldId, string openId, string roleId);
    
    #if UNITY_IOS
	[DllImport("__Internal")]
	#else	
	[DllImport("tersafe2")]
	#endif
	private static extern int tp2_setoptions(int options);
    
    #if UNITY_IOS
	[DllImport("__Internal")]
#else
	[DllImport("tersafe2")]
#endif
	private static extern IntPtr tp2_sdk_ioctl(int request, string param);

	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe2")]
	#endif
	private static extern int tp2_free_anti_data(IntPtr info);
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe2")]
	#endif
	private static extern IntPtr tss_get_report_data();
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe2")]
	#endif
	public static extern void tss_del_report_data(IntPtr info);
	
	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe2")]
	#endif
	private static extern IntPtr tss_get_report_data2();
}

public enum Tp2GameStatus
{
    FRONTEND    = 1,	//前台
    BACKEND     = 2		//后台
}

public enum Tp2Entry
{
	ENTRY_ID_QZONE 		= 1,       // QQ
	ENTRY_ID_MM    		= 2,       // 微信
	ENTRT_ID_FACEBOOK  	= 3,       // facebook
	ENTRY_ID_TWITTER	= 4,       // twitter
	ENTRY_ID_LINE		= 5,       // line
	ENTRY_ID_WHATSAPP	= 6,       // whatsapp
    ENTRY_ID_GAMECENTER = 7,       // gamecenter
    ENTRY_ID_GOOGLEPLAY = 8,       // googleplay
    ENTRY_ID_VK         = 9,       // vk
    ENTRY_ID_KUAISHOU   = 10,      // kuaishou
    ENTRY_ID_APPLE      = 11,      // apple
    ENTRY_ID_NEXON      = 12,      // Nexon
    ENTRY_ID_NAVER      = 13,      // Naver
    ENTRY_ID_GARENA     = 14,      // GARENA
    ENTRY_ID_HUAWEI     = 15,      // HUAWEI
	ENTRY_ID_RIOT       = 16,      // Riot
	ENTRY_ID_NINTENDO   = 17,      // Nintendo
	ENTRY_ID_PSN        = 18,      // PSN
	ENTRY_ID_MICROSOFT  = 19,      // Microsoft
	ENTRY_ID_EA         = 20,      // EA
	ENTRY_ID_CUSTOM     = 21,      // CUSTOM
	ENTRY_ID_OTHERS		= 99       // 其他平台
}

//私有服地区设置
enum Tp2LocaleId
{
    ACE_LOCALE_CN = 1,
    ACE_LOCALE_EN = 2,
    ACE_LOCALE_RU = 3,
    ACE_LOCALE_US = 4,
    ACE_LOCALE_EU = 5,
    ACE_LOCALE_IN = 6,
    ACE_LOCALE_JP = 7,
    ACE_LOCALE_KR = 8,
    ACE_LOCALE_TW = 9,
	ACE_LOCALE_VN = 10,
    ACE_LOCAL_GLOBAL = 998,
    ACE_LOCAL_OTHERS = 999,
};
