using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class WorldBossNpc : Npc
    {
        public WorldBossComponent worldBossComponent = new WorldBossComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();
            worldBossComponent.actor = this;
            worldBossComponent.Construct();
        }

        protected override void OnDispose()
        {
            worldBossComponent.Dispose();
            base.OnDispose();
        }

        protected override void OnOtherSet()
        {
            base.OnOtherSet();

            worldBossComponent.AddWorldBossHud();
        }
    }
}