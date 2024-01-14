using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class CollectionNpc : Npc
    {
        public CollectionComponent collectionComponent = new CollectionComponent();

        protected override void OnConstruct()
        {
            base.OnConstruct();
            collectionComponent.actor = this;
            collectionComponent.Construct();
        }

        protected override void OnDispose()
        {
            collectionComponent.Dispose();
            base.OnDispose();
        }
    }
}