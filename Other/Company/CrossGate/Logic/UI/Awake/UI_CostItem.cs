using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public enum ItemCostLackType {
        Normal, // left/right都是白色
        LeftLackRG, // left足够则绿色，否则红色， right白色
        Custom, // 自定义
    }

    public class CostLayout {
        public GameObject gameObject;
        public Image icon;
        public Text content;

        public void Parse(GameObject root) {
            this.gameObject = root;
            this.icon = root.GetComponent<Image>();
            this.content = root.transform.Find("Text_Cost").GetComponent<Text>();
        }
    }

    public class UI_CostItem {
        public GameObject gameObject;
        public Image icon;
        public Text content;
        public Text right;

        public ItemIdCount idCount;

        public virtual void SetGameObject(GameObject go) {
            this.gameObject = go;

            this.icon = go.GetComponent<Image>();
            this.content = go.transform.Find("Text_Cost").GetComponent<Text>();
        }

        public void Refresh(ItemIdCount idCount, ItemCostLackType lackType = ItemCostLackType.LeftLackRG, string content = null, bool useSmallIcon = true) {
            this.idCount = idCount;

            if (idCount != null) {
                if (idCount.CSV != null) {
                    ImageHelper.SetIcon(this.icon, useSmallIcon ? idCount.CSV.small_icon_id : idCount.CSV.icon_id);
                }

                uint contentId = 1001; // 白色的一个id
                if (lackType == ItemCostLackType.Normal) {
                    TextHelper.SetText(this.content,idCount.count.ToString());
                }
                else if (lackType == ItemCostLackType.LeftLackRG) {
                    contentId = idCount.Enough ? 1601000006u : 1601000004u;
                    TextHelper.SetText(this.right, idCount.count.ToString());
                    TextHelper.SetText(this.content, contentId, idCount.CountInBag.ToString(), idCount.count.ToString());
                }
                else if (lackType == ItemCostLackType.Custom) {
                    TextHelper.SetText(this.right, "");
                    TextHelper.SetText(this.content, content);
                }
            }
        }
    }
}