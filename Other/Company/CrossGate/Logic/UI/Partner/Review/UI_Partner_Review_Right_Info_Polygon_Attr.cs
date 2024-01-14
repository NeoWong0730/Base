using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Table;
using System.Collections;

namespace Logic
{
    public class UI_Partner_Review_Right_Info_Polygon_Attr
    {
        private Transform transform;

        //精神，魔攻，敏捷，防御，攻击，生命
        private UIPolygon _polygon;

        private Lib.Core.CoroutineHandler _handler;

        public void Init(Transform trans)
        {
            transform = trans;

            _polygon = transform.Find("UI Polygon").GetComponent<UIPolygon>();
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
            CSVPartner.Data data = CSVPartner.Instance.GetConfData(infoId);
            if (data != null)
            {
                for (int i = 0; i < data.ability_value.Count; ++i)
                {
                    _polygon.VerticesDistances[i] = data.ability_value[i] / 100f;
                }
            }

            _polygon.enabled = false;

            if (_handler != null)
            {
                Lib.Core.CoroutineManager.Instance.Stop(_handler);
                _handler = null;
            }

            Lib.Core.CoroutineManager.Instance.StartHandler(ShowPolygon());
        }

        private IEnumerator ShowPolygon()
        {
            yield return new WaitForEndOfFrame();

            _polygon.enabled = true;
        }
    }
}


