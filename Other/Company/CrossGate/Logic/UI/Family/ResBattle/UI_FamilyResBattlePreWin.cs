using System;
using Lib.Core;
using Logic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战top
public class UI_FamilyResBattlePreWin : UIBase {
    public class Layout : LayoutBase {
        public Text familyName;
        public Text prg;

        public void Parse(GameObject root) {
            this.Set(root);

            this.familyName = this.transform.Find("Animator/Image_BG2/Content/Text").GetComponent<Text>();
            this.prg = this.transform.Find("Animator/Image_BG2/Content/Value").GetComponent<Text>();
        }
    }

    public Layout layout = new Layout();
    public Timer timer;


    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
    }
    protected override void OnDestroy() {
        this.timer?.Cancel();
        this.timer = null;
    }

    public ulong familyId;
    public uint prg;
    protected override void OnOpen(object arg) {
        Tuple<ulong, uint> tp = arg as Tuple<ulong, uint>;
        if (tp != null) {
            this.familyId = tp.Item1;
            this.prg = tp.Item2;
        }
    }

    protected override void OnOpened() {
        this.RefreshAll();

        this.timer?.Cancel();
        this.timer = Timer.RegisterOrReuse(ref this.timer, 2f, this.OnTimeEnd);
    }
    private void OnTimeEnd() {
        this.timer?.Cancel();
        this.timer = null;

        this.CloseSelf();
    }

    public void RefreshAll() {
        if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
            return;
        }

        TextHelper.SetText(this.layout.familyName, 3230000056, Sys_FamilyResBattle.Instance.guilds[this.familyId].GuildName.ToStringUtf8());
        TextHelper.SetText(this.layout.prg, 3230000057, this.prg.ToString(), Sys_FamilyResBattle.Instance.battlewinMaxRes.ToString());
    }
}