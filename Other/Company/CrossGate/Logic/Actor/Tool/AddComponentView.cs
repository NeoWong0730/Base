using Framework;
using Lib.Core;
using Logic.Core;
using NaughtyAttributes;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class AddComponentView : UnityEngine.MonoBehaviour
    {
        public SceneActor sceneActor { get; set; }

        [Button("AddComponentsView")]
        public void AddComponentsView()
        {
            foreach (Component component in sceneActor.mComponents.Values)
            {
                ComponentView view = sceneActor.gameObject.AddComponent<ComponentView>();
                view.Component = component;
            }
        }

        [Button("StartCollectionTest104601")]
        public void StartCollectionTest104601()
        {
            CollectionCtrl.Instance.StartCollection(104601);
        }

        [Button("StartCollectionTest11013601")]
        public void StartCollectionTest11013601()
        {
            CollectionCtrl.Instance.StartCollection(11013601);
        }

        [Button("EnterCutScene")]
        public void EnterCutScene()
        {
            Sys_CutScene.Instance.TryDoCutScene(5006609);
        }

        [Button("SqliteTest_Create")]
        public void SqliteTest_Create()
        {
            SqliteTest.CreateSqliteDB();
        }

        [Button("Sqlite_Load")]
        public void Sqlite_Load()
        {
            SqliteTest.LoadDB();
        }

        [Button("Inquiry")]
        public void Inquiry()
        {
            Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.UIButton, new InteractiveEvtData()
            {
                eInteractiveAimType = EInteractiveAimType.NPCFunction,
                sceneActor = null,
                immediately = false,
                data = EFunctionType.Inquiry,
            });
        }

        [Button("OnMount")]
        public void OnMount()
        {
            //GameCenter.mainHero.OnMount(610505, GameCenter.mainHero.uID * 1000000 + 1);
        }

        [Button("OffMount")]
        public void OffMount()
        {
            GameCenter.mainHero.OffMount();
        }

        [Button("AddPet")]
        public void AddPet()
        {
            //没有卸载的地方
#if false
            //Pet pet = GameCenter.mainWorld.CreateActor<Pet>(99999999);            
            Pet pet = World.AllocSceneActor<Pet>(99999999, null);

            pet.csvPetData = CSVPetNew.Instance.GetConfData(640101);
            pet.HandlerHero = GameCenter.mainHero;

            //pet.followComponent = World.AddComponent<FollowComponent>(pet);
            pet.followComponent.Target = GameCenter.mainHero;
            pet.followComponent.Follow = true;
            pet.followComponent.KeepDistance = 3f;

            //pet.stateComponent = World.AddComponent<StateComponent>(pet);
            //pet.movementComponent = World.AddComponent<MovementComponent>(pet);
            pet.movementComponent.TransformToPosImmediately(GameCenter.mainHero.transform.position);

            pet.LoadModel(pet.csvPetData.model, (actor) =>
            {
                //pet.animationComponent = World.AddComponent<AnimationComponent>(pet);
                pet.animationComponent.SetSimpleAnimation(pet.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                pet.animationComponent.UpdateHoldingAnimations(pet.csvPetData.action_id);
            });
#endif
        }

        [Button("TestRoleAction")]
        public void TestRoleAction()
        {
            CmdMapRoleActionReq cmdMapRoleActionReq = new CmdMapRoleActionReq();
            cmdMapRoleActionReq.Actionid = (uint)EStateType.Logging;
            if (GameCenter.mainHero.transform.localEulerAngles.y < 0)
            {
                cmdMapRoleActionReq.Direction = (uint)((GameCenter.mainHero.transform.localEulerAngles.y + 360f) * 1000);
            }
            else
            {
                cmdMapRoleActionReq.Direction = (uint)(GameCenter.mainHero.transform.localEulerAngles.y * 1000);
            }
            NetClient.Instance.SendMessage((ushort)CmdMap.RoleActionReq, cmdMapRoleActionReq);
        }
    }
}
