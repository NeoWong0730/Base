using System.Collections.Generic;

namespace Logic
{
    public class ActorSpawnSystem : LvPlaySystemBase
    {
        public Queue<ActorDataFromServer> mWaitSpawnActor = new Queue<ActorDataFromServer>();

        public Queue<VirtualActorData> mWaitSpawnVirtualActor = new Queue<VirtualActorData>();

        public override void OnUpdate()
        {
            while (mWaitSpawnActor.TryDequeue(out ActorDataFromServer actorDataFromServer))
            {
                mData.CreateOrUpdateActor(actorDataFromServer);
            }

            while (mWaitSpawnVirtualActor.TryDequeue(out VirtualActorData virtualActorData))
            {
                mData.CreateVirtualActor(virtualActorData);
            }
        }
    }
}