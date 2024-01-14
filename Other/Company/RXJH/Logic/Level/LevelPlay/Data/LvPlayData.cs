using Logic;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Cinemachine;
using BehaviorDesigner.Runtime;
using UnityEngine.InputSystem;

public enum EActorType
{
    Player,
    NPC,
    Monster,
    Parnter,
}

public class ActorDataFromServer
{
    public uint uid;
    public EActorType eActorType;
    public float3 pos;
}

public class VirtualActorData
{
    public uint uid;
    public uint infoID;
    public EActorType eActorType;
    public Vector3 pos;
}

public class LvPlayData
{
    public int nLevelState;

    /// <summary>
    /// 当前玩家的ActorID
    /// </summary>
    public uint nPlayerActorID;
    public Actor mMainActor { get { mActors.TryGetValue(nPlayerActorID, out Actor actor); return actor; } }

    public List<Actor> mActorList = new List<Actor>();
    public Dictionary<uint, Actor> mActors = new Dictionary<uint, Actor>();

    public AsyncOperationHandle<GameObject> mHumanModelHandle;
    public AsyncOperationHandle<GameObject> mMainVirtualCameralHandle;
    public AsyncOperationHandle<ExternalBehaviorTree> mExternalBehaviorTreelHandle;
    public AsyncOperationHandle<GameObject> mVirtualActorlHandle;

    public @PlayerInputActions mInputDatas;
    public LvPlayCameraData mCameraData;    
    public Character mMainCharacter;
    public MainCharacterBehaviourController mainCharacterBehaviourController;

    public void OnCreate()
    {
        AddressablesUtil.LoadAssetAsync(ref mHumanModelHandle, "human", null, false);
        mHumanModelHandle.WaitForCompletion();

        mInputDatas = new PlayerInputActions();

        OnCreateCameraData();

        AddressablesUtil.LoadAssetAsync(ref mExternalBehaviorTreelHandle, "bt_main_character_1", null, false);
        mExternalBehaviorTreelHandle.WaitForCompletion();
    }

    private void OnCreateCameraData()
    {
        //AddressablesUtil.LoadAssetAsync<GameObject>(ref mMainVirtualCameralHandle, "CM_MainVM", null, false);
        AddressablesUtil.InstantiateAsync(ref mMainVirtualCameralHandle, "CM_FreeLook", null);
        mMainVirtualCameralHandle.WaitForCompletion();

        mCameraData = new LvPlayCameraData();
        mCameraData.mLookAtAim = (new GameObject("LookAtAim")).transform;
        mCameraData.CMFreeLook = mMainVirtualCameralHandle.Result.GetComponent<CinemachineFreeLook>();
    }

    public void OnDestroy()
    {
        if (mHumanModelHandle.IsValid())
        {
            AddressablesUtil.Release<GameObject>(ref mHumanModelHandle, null);
        }

        if (mMainVirtualCameralHandle.IsValid())
        {
            AddressablesUtil.ReleaseInstance(ref mMainVirtualCameralHandle, null);
        }

        if (mExternalBehaviorTreelHandle.IsValid())
        {
            AddressablesUtil.Release<ExternalBehaviorTree>(ref mExternalBehaviorTreelHandle, null);
        }

        if(mCameraData.mLookAtAim)
        {
            Object.DestroyImmediate(mCameraData.mLookAtAim.gameObject);
        }
    }

    public int CreateOrUpdateActor(ActorDataFromServer actorDataFromServer)
    {
        if (!mActors.TryGetValue(actorDataFromServer.uid, out Actor actor))
        {
            //actor = new Actor();
            actor = new Character();
            actor.uid = actorDataFromServer.uid;
            actor.mTransform = GameObject.Instantiate(mHumanModelHandle.Result, actorDataFromServer.pos, Quaternion.identity, null).transform;

            if (actorDataFromServer.uid == nPlayerActorID)
            {
                mMainCharacter = actor as Character;
                mMainCharacter.Init(Character.ECharacterNetType.Local, actorDataFromServer);
                
                mainCharacterBehaviourController = new MainCharacterBehaviourController();
                mainCharacterBehaviourController.Init(new GameObject(), mMainCharacter, mExternalBehaviorTreelHandle.Result);
            }
            else
            {
                ((Character)actor).Init(Character.ECharacterNetType.Remote, actorDataFromServer);
            }

            mActorList.Add(actor);
            mActors.Add(actor.uid, actor);
        }
        else
        {
            //Updata Pos
        }

        return 0;
    }

    public void CreateVirtualActor(VirtualActorData virtualActorData)
    {
        if (!mActors.TryGetValue(virtualActorData.uid, out Actor actor))
        {
            actor = new VirtualActor();
            actor.uid = virtualActorData.uid;

            AddressablesUtil.LoadAssetAsync<GameObject>(ref mVirtualActorlHandle, "human", (mVirtualActorlHandle) =>
            {
                actor.mTransform = GameObject.Instantiate(mVirtualActorlHandle.Result, new Vector3(virtualActorData.pos.x, 0, virtualActorData.pos.y), Quaternion.identity, null).transform;
                ((VirtualActor)actor).InitVirtualActor();
            }, false);

            mActorList.Add(actor);
            mActors.Add(actor.uid, actor);
        }
    }
}