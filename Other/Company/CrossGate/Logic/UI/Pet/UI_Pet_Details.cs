using Logic.Core;
 
namespace Logic
{
    public class UI_Pet_Details : UIBase
    {
        public UI_Pet_Detail_View view;
        public ClientPet curClientPet;
        protected override void OnLoaded()
        {
            view = new UI_Pet_Detail_View(curClientPet);
            view.Init(gameObject.transform);
        }

        protected override void OnShow()
        {
            view.Show();
        }

        protected override void OnHide()
        {
            view.Hide();
        }

        protected override void OnDestroy()
        {
            view.OnDestroy();
        }

        protected override void OnOpen(object arg)
        {
            curClientPet = (ClientPet)arg;
        }
    }
}
