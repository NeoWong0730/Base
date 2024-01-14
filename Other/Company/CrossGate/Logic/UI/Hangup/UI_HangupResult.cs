using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_HangupResult : UIBase, UI_HangupResult.Layout.IListener {
        public class Layout {
            public GameObject gameObject;
            public Transform transform;

            public Button btnExit;

            public Text time;
            public Text exp;
            public Transform rewardParent;
            public Transform petParent;
            public Transform petProto;

            public Transform pRewardProto;
            public Transform pRewardParent;

            public void Parse(GameObject root) {
                this.gameObject = root;
                this.transform = this.gameObject.transform;

                this.btnExit = this.transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
                this.time = this.transform.Find("Animator/Content/Time/Text_Num").GetComponent<Text>();
                this.exp = this.transform.Find("Animator/Content/EXP/Text_Num").GetComponent<Text>();
                this.rewardParent = this.transform.Find("Animator/Content/Text1/Scroll View/Viewport/Content");
                this.petProto = this.transform.Find("Animator/Content/Text2/Scroll View/Viewport/Content/PetItem03");
                this.petParent = this.petProto.parent;

                this.pRewardProto = this.transform.Find("Animator/Content/Text3/Scroll View/Viewport/Content/LIne");
                this.pRewardParent = this.pRewardProto.parent;
            }

            public void RegisterEvents(IListener listener) {
                this.btnExit.onClick.AddListener(listener.OnBtnReturnClicked);
            }

            public interface IListener {
                void OnBtnReturnClicked();
            }
        }

        public class PetVd : UIComponent {
            public Image img;

            public PetUnit pet;

            protected override void Loaded() {
                this.img = this.transform.Find("Image_Icon").GetComponent<Image>();
                Button btn = this.transform.Find("Image_Icon").GetComponent<Button>();
                btn.onClick.AddListener(this.OnBtnClicked);
            }

            private void OnBtnClicked() {
                var cp = new ClientPet(this.pet);
                bool isInBag = Sys_Pet.Instance.IsInBag(this.pet);
                Sys_Pet.Instance.ShowPetTip(cp, isInBag ? 0u : 1u);
            }

            public void Refresh(PetUnit pet) {
                this.pet = pet;

                var csvPet = CSVPetNew.Instance.GetConfData(pet.SimpleInfo.PetId);
                if (csvPet != null) {
                    ImageHelper.SetIcon(this.img, csvPet.icon_id);
                }
            }
        }

        public class PVd : UIComponent {
            public Image img;
            public Text number;
            public Text gets;

            protected override void Loaded() {
                this.img = this.transform.Find("PetItem03/Image_Icon").GetComponent<Image>();
                this.number = this.transform.Find("Num").GetComponent<Text>();
                this.gets = this.transform.Find("Text").GetComponent<Text>();
            }

            public void Refresh(HangUpAutoSoldPet pet) {
                var csvPet = CSVPetNew.Instance.GetConfData(pet.Pettid);
                if (csvPet != null) {
                    ImageHelper.SetIcon(this.img, csvPet.icon_id);
                }

                TextHelper.SetText(number, 4326, pet.Num.ToString());
                TextHelper.SetText(gets, 4327, pet.Income.ToString());
            }
        }

        private readonly Layout layout = new Layout();

        public UI_RewardList itemRewardList = null;
        public COWVd<PetVd> vds = new COWVd<PetVd>();
        public COWVd<PVd> pvds = new COWVd<PVd>();

        private CmdHangUpDataNtf ntf;

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.ntf = (CmdHangUpDataNtf) arg;
        }

        protected override void OnOpened() {
            uint.TryParse(CSVParam.Instance.GetConfData(1025).str_value, out uint arg2);
            uint time = this.ntf.LastOfflineReward.LastTime;
            if (time > arg2 * 3600) {
                time = arg2 * 3600;
                Sys_Chat.Instance.PushMessage(ChatType.System, null, LanguageHelper.GetTextContent(2104053));
            }
            
            this.layout.time.text = LanguageHelper.TimeToString(time, LanguageHelper.TimeFormat.Type_4);
            this.layout.exp.text = this.ntf.LastOfflineReward.Exp.ToString();

            if (this.itemRewardList == null) {
                this.itemRewardList = new UI_RewardList(this.layout.rewardParent, EUIID.UI_HangupResult);
            }

            var list = new List<ItemIdCount>();
            for (int i = 0, length = this.ntf.LastOfflineReward.Item.Count; i < length; ++i) {
                list.Add(new ItemIdCount(this.ntf.LastOfflineReward.Item[i].Id, this.ntf.LastOfflineReward.Item[i].Count));
            }

            this.itemRewardList.SetRewardList(list);
            this.itemRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

            this.vds.TryBuildOrRefresh(this.layout.petProto.gameObject, this.layout.petParent, this.ntf.LastOfflineReward.Pet.Count, this.OnRefresh);
            var info = this.ntf.LastOfflineReward.AutoSoldPet;
            int autoCount = 0;
            if (info != null) {
                autoCount = info.Count;
            }

            DebugUtil.LogFormat(ELogType.eTask, "info == null {0} 奖励：{1}", info == null, autoCount.ToString());
            this.pvds.TryBuildOrRefresh(this.layout.pRewardProto.gameObject, this.layout.pRewardParent, autoCount, this.OnRefreshPReward);
        }

        private void OnRefresh(PetVd vd, int index) {
            vd.Refresh(this.ntf.LastOfflineReward.Pet[index]);
        }

        private void OnRefreshPReward(PVd vd, int index) {
            vd.Refresh(this.ntf.LastOfflineReward.AutoSoldPet[index]);
        }

        public void OnBtnReturnClicked() {
            this.CloseSelf();
        }

        protected override void OnDestroy() {
            this.vds.Clear();
            pvds.Clear();
        }
    }
}