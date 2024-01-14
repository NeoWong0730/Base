using UnityEngine;
using Logic.Core;
using Lib.Core;

namespace Logic
{
    public partial class HUD : UIBase
    {
        public void CreateOrderHUD(CreateOrderHUDEvt createOrderHUDEvt)
        {
            if (orderQueue == null)
            {
                orderQueue = new OrderQueue(GetOrderGo, PushOrderGo, GetSkillCarouselGo, PushSkillCarouselGo, template_OrderShow, template_SkillCarouselShow);
            }

            orderQueue.AddOrder(createOrderHUDEvt);
        }

        public void UndoOrderHUD(uint sender)
        {
            if (orderQueue == null)
                return;
            orderQueue.Undo(sender);
        }

        public void ClearOrder()
        {
            orderQueue?.Dispose();
        }
    }
}