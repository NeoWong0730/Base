using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class UI_WorldbossBattleResult : UIBase {
    public Transform parent;
    public Text remainTimeText;
    public GameObject ResidueGo;

    public UI_RewardList rewardList;

    private CmdBattleEndNtf ntf;
    private IList<ItemIdCount> showReward = new List<ItemIdCount>();

    protected override void OnLoaded() {
        parent = transform.Find("Animator/Image_Successbg/View_Victory/RewardNode/Scroll_View/Viewport");
        ResidueGo = transform.Find("Animator/Image_Successbg/View_Victory/Text_Residue").gameObject;
        remainTimeText = transform.Find("Animator/Image_Successbg/View_Victory/Text_Residue/Value").GetComponent<Text>();
    }

    protected override void OnOpen(object arg) {
        this.ntf = arg as CmdBattleEndNtf;
        showReward.Clear();
        for (int i = 0, length = ntf.Rewards.Items.Count; i < length; ++i) {
            showReward.Add(new ItemIdCount(ntf.Rewards.Items[i].Infoid, ntf.Rewards.Items[i].Count));
        }
    }

    protected override void OnShow() {
        if (this.rewardList == null) {
            this.rewardList = new UI_RewardList(this.parent, EUIID.UI_WorldbossBattleResult);
        }

        this.rewardList.SetRewardList(showReward);
        this.rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

        //Npc npc = GameCenter.mainWorld.GetActor(typeof(Npc), ntf.NpcId) as Npc;
        //if (npc != null) 
        var csv = CSVBOSSInformation.Instance.GetConfData(ntf.Npctid);
        if (csv != null) {
            ResidueGo.SetActive(true);
            CSVBOOSFightPlayMode.Data csvPlayMode = CSVBOOSFightPlayMode.Instance.GetConfData(csv.playMode_id);
            uint dailyId = GetDailyId(csv.playMode_id);
            long total = 0;
            long times = Sys_Daily.Instance.GetDailyTimes(dailyId);
            if (csvPlayMode != null) {
                if (csvPlayMode.rewardLimit != 0) {
                    total = (long) csvPlayMode.rewardLimit;
                    TextHelper.SetText(this.remainTimeText, 4157000019u, (total - times).ToString());
                }
                else if (csvPlayMode.rewardLimitDay != 0) {
                    total = (long) csvPlayMode.rewardLimitDay;
                    TextHelper.SetText(this.remainTimeText, 4157000020u, (total - times).ToString());
                }
                else {
                    ResidueGo.SetActive(false);
                }
            }
            else {
                ResidueGo.SetActive(false);
            }
        }
        else {
            ResidueGo.SetActive(false);
        }
    }

    public uint GetDailyId(uint playModeId) {
        uint id = 0;
        var csv = CSVBOOSFightPlayMode.Instance.GetConfData(playModeId);
        if (csv != null) {
            id = csv.dailyActivites;
        }

        return id;
    }
}