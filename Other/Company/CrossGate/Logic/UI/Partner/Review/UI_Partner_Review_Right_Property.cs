using UnityEngine;


namespace Logic
{
    public class UI_Partner_Review_Right_Property
    {
        private Transform transform;

        private UI_Partner_Review_Right_Property_High high;

        public void Init(Transform trans)
        {
            transform = trans;

            high = new UI_Partner_Review_Right_Property_High();
            high.Init(transform);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void UpdateInfo(uint infoId)
        {
            high.UpdateInfo(infoId);
        }
    }
}


