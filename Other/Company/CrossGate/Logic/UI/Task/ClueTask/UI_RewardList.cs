using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic {
    public class UI_RewardList {
        public Transform parent;
        private IList<ItemIdCount> rewards = new List<ItemIdCount>();

        private readonly List<PropItem> rewardGos = new List<PropItem>();
        private readonly List<PropIconLoader.ShowItemData> rewardDatas = new List<PropIconLoader.ShowItemData>();

        private readonly EUIID sourceUiId;

        public bool isEnough {
            get {
                bool ret = true;
                foreach (var ele in this.rewards) {
                    long left = Sys_Bag.Instance.GetItemCount(ele.id);
                    long right = ele.count;
                    if (left < right) {
                        ret = false;
                        break;
                    }
                }
                return ret;
            }
        }

        public void Show(bool toShow) {
            this.parent.gameObject.SetActive(toShow);
        }

        public UI_RewardList(Transform parent, EUIID sourceUiId) {
            this.parent = parent; this.sourceUiId = sourceUiId;
        }
        public UI_RewardList(IList<ItemIdCount> rewards, Transform parent) {
            this.rewards = rewards;
            this.parent = parent;
        }
        public UI_RewardList(uint dropId, Transform parent) : this(CSVDrop.Instance.GetDropItem(dropId), parent) { }
        public void SetRewardList(IList<ItemIdCount> rewards) {
            this.rewards = rewards;
        }

        public void Build(bool bUseQuailty = true, bool bBind = false, bool bNew = false, bool bUnLock = false, bool bSelected = false, bool bShowCount = false, 
            bool bShowBagCount = false, bool bUseClick = true, System.Action<PropItem> onClick = null, bool showBtnNo = false, bool bShowGot = false, bool useTip = false) {
            {
                int currentCount = this.rewardGos.Count;
                for (int i = currentCount; i < this.rewards.Count; ++i) {
                    // tips设计如此，这里将就一下， 刷新一下数据
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData();
                    PropItem item = PropIconLoader.GetAsset(itemData, this.parent);

                    this.rewardGos.Add(item);
                    this.rewardDatas.Add(itemData);
                }

                for (int i = 0; i < this.rewardGos.Count; ++i) {
                    if (i < this.rewards.Count) {
                        this.rewardGos[i].SetActive(true);
                        PropIconLoader.ShowItemData itemData = rewardDatas[i].Reset().Refresh(this.rewards[i].id, this.rewards[i].count, bUseQuailty, bBind, bNew, bUnLock, bSelected, bShowCount, bShowBagCount, bUseClick, onClick, showBtnNo, useTip);
                        itemData.EquipPara = rewards[i].equipPara;
                        this.rewardGos[i].SetData(itemData, this.sourceUiId);
                        this.rewardGos[i].SetGot(bShowGot);
                        this.rewardGos[i].EnableLongPress(false);
#if UNITY_EDITOR
                        this.rewardGos[i].transform.gameObject.name = this.rewards[i].id.ToString();
#endif
                    }
                    else {
                        this.rewardGos[i].SetActive(false);
                    }
                }
            }
        }

        public void Clear() {
            rewards.Clear();
            rewardGos.Clear();
            rewardDatas.Clear();
        }
    }
}