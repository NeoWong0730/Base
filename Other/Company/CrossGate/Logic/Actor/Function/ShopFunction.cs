using Logic.Core;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 商城功能///
    /// </summary>
    public class ShopFunction : FunctionBase
    {
        public uint MallID
        {
            get;
            set;
        }

        public uint ShopID
        {
            get;
            set;
        }

        public override void DeserializeObjectExt(List<uint> ext)
        {
            MallID = ext[0];
            ShopID = ext[1];
        }

        protected override void OnExecute()
        {
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);

            MallPrama mallPrama = new MallPrama();
            mallPrama.mallId = MallID;
            mallPrama.shopId = ShopID;
            mallPrama.itemId = 0;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }

        protected override void OnDispose()
        {
            MallID = 0;
            ShopID = 0;

            base.OnDispose();
        }
    }
}
