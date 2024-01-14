using cfg;
using HotFix;
using NWFramework;
using Proto.User;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    private void Start()
    {
        Log.Info("TestBehaviour.Start()");
    }

    private void OnGUI()
    {
        #region Test

        #region Net

        if (GUI.Button(new Rect(100f, 40f, 200f, 50f), "Listen"))
        {
            MessageRouter.Instance.Subscribe<UserRegisterResponse>((conn, res) =>
            {
                Log.Debug($"Code: {res.Code}, Msg: {res.Message}");
            });

            MessageRouter.Instance.Subscribe<UserLoginResponse>((conn, res) =>
            {
                Log.Debug($"Code: {res.Code}, Msg: {res.Message}");
            });
        }

        if (GUI.Button(new Rect(100f, 100f, 200f, 50f), "Connection"))
        {
            NetClient.Instance.ConnectToServer(new NetOption() 
            { 
                host = "127.0.0.1", 
                port = 8888 
            });
            NetClient.Instance.StartSendHeartBeat();

        }

        if (GUI.Button(new Rect(100f, 160f, 200f, 50f), "Register"))
        {
            UserRegisterRequest req = new UserRegisterRequest();
            req.Username = "huang";
            req.Password = "12345";

            NetClient.Instance.Send(req);
        }

        if (GUI.Button(new Rect(100f, 220f, 200f, 50f), "Login"))
        {
            UserLoginRequest req = new UserLoginRequest();
            req.Username = "huang";
            req.Password = "12345";

            NetClient.Instance.Send(req);
        }

        if (GUI.Button(new Rect(100f, 280f, 200f, 50f), "TestLoadConfig"))
        {
            TbScene tbScene = CsvManager.Instance.GetVOData<TbScene>("tbscene");
            var infos = tbScene.DataList;
            foreach (var info in infos)
            {
                Log.Debug($"Id: {info.Id}, AssetName: {info.AssetName}");
            }
        }

        if (GUI.Button(new Rect(100f, 340f, 200f, 50f), "CreateTestObjectPool"))
        {
            TestObjectPool = GameModule.ObjectPool.CreateSingleSpawnObjectPool<TestObject>("TestObjectPool");
        }

        if (GUI.Button(new Rect(100f, 400f, 200f, 50f), "AddTestObject"))
        {
            TestObject testObject = null;
            if (TestObjectPool.CanSpawn())
            {
                testObject = TestObjectPool.Spawn();
            }
            else
            {
                GameObject go = new GameObject(Time.realtimeSinceStartup.ToString());
                testObject = TestObject.Create(Time.realtimeSinceStartup.ToString(), go);
                TestObjectPool.Register(testObject, true);
            }
        }

        if (GUI.Button(new Rect(350f, 40f, 200f, 50f), "PlayBGM"))
        {
            GameModule.Audio.Play(NWFramework.AudioType.Music, "BGM01hero", true, 1, true, false);
        }

        if (GUI.Button(new Rect(350f, 100f, 200f, 50f), "StopBGM"))
        {
            GameModule.Audio.Stop(NWFramework.AudioType.Music, true);
        }

        if (GUI.Button(new Rect(350f, 160f, 200f, 50f), "PlayUISound"))
        {
            GameModule.Audio.Play(NWFramework.AudioType.UISound, "30_Jump_03", false, 1, true, true);
        }

        if (GUI.Button(new Rect(350f, 220f, 200f, 50f), "TestUpdate"))
        {

        }

        if (GUI.Button(new Rect(350f, 280f, 200f, 50f), "LoadAdditiveScene"))
        {
            GameModule.Scene.LoadSubScene("Scene_001");
        }

        if (GUI.Button(new Rect(350f, 340f, 200f, 50f), "UnloadAdditiveScene"))
        {
            GameModule.Scene.UnloadAsync("Scene_001");
        }

        #endregion

        #endregion
    }

    IObjectPool<TestObject> TestObjectPool;

    public class TestObject : ObjectBase
    {
        protected override void Release(bool isShutdown)
        {
        }

        public static TestObject Create(string name, GameObject gameObject)
        {
            var obj = MemoryPool.Acquire<TestObject>();
            obj.Initialize(name, gameObject);
            return obj;
        }
    }
}
