using System.Collections.Generic;
using System.Linq;
using Common;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic {
    public class Sys_Server : SystemModuleBase<Sys_Server> {
        public class TabEntry {
            public uint id;
            public string name;
            public List<ZoneEntry> zones = new List<ZoneEntry>();
        }

        public class ZoneEntry {
            public ZoneInfo svrZone;
            public uint maxLogoutTime;
            public List<ZoneRole> roles = new List<ZoneRole>();
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents {
            OnServerListChanged,
            OnSelectedServerChanged,
        }
        
        public TabEntry lastTab;
        public TabEntry recommendTab;
        public SortedDictionary<uint, TabEntry> allTabs = new SortedDictionary<uint, TabEntry>();
        
        public Dictionary<uint, ZoneEntry> allZones = new Dictionary<uint, ZoneEntry>();

        public uint selectedZoneId = 0;

        public bool isNewCharacter {
            get {
                var zone = GetSelectedZone();
                if (zone == null) {
                    return true;
                }

                return zone.roles.Count <= 0;
            }
        }

        public void ClearServerList() {
            if (lastTab != null) {
                lastTab.zones.Clear();
            }
            if (recommendTab != null) {
                recommendTab.zones.Clear();
            }
            allTabs.Clear();
            
            allZones.Clear();
        }

        public ZoneEntry GetZone(uint zoneId) {
            allZones.TryGetValue(zoneId, out ZoneEntry zone);
            return zone;
        }

        public ZoneEntry GetSelectedZone() {
            return GetZone(selectedZoneId);
        }

        public void SetServerList(GameZoneAllTab allServerTabs) {
            this.ClearServerList();

            // process tabs
            for (int i = 0, length = allServerTabs.TabList.Tabs.Count; i < length; ++i) {
                var oneTab = allServerTabs.TabList.Tabs[i];
                var tabEntry = new TabEntry() {
                    id = oneTab.TabId,
                    name = oneTab.TabName,
                };
                allTabs.Add(oneTab.TabId, tabEntry);
            }

            lastTab = new TabEntry() {
                id = int.MaxValue - 1,
                name = LanguageHelper.GetContent<CSVLanguage>(1000018)
            };
            recommendTab = new TabEntry() {
                id = int.MaxValue - 2,
                name = LanguageHelper.GetContent<CSVLanguage>(1000019)
            };

            // process zones
            for (int i = 0, length = allServerTabs.ZoneList.Zones.Count; i < length; ++i) {
                var one = allServerTabs.ZoneList.Zones[i];
                var zoneEntry = new ZoneEntry() {
                    svrZone = one,
                };
                allZones.Add(one.ZoneId, zoneEntry);

                if (allTabs.TryGetValue(one.TabId, out var tabEntry)) {
                    tabEntry.zones.Add(zoneEntry);
                }
            }

            for (int i = 0, length = allServerTabs.RoleList.Roles.Count; i < length; ++i) {
                var role = allServerTabs.RoleList.Roles[i];
                uint maxLogoutTime = 0;
                if (allZones.TryGetValue(role.ZoneId, out var zoneEntry)) {
                    lastTab.zones.Add(zoneEntry);

                    for (int j = 0; j < role.Roles.Count; ++j) {
                        zoneEntry.roles.Add(role.Roles[j]);
                        if (role.Roles[j].LastLogoutTime > maxLogoutTime) {
                            maxLogoutTime = role.Roles[j].LastLogoutTime;
                        }
                    }

                    zoneEntry.maxLogoutTime = maxLogoutTime;
                }
            }

            // 从大到小排序最近登录
            lastTab.zones.Sort((l, r) => { return (int)((long)r.maxLogoutTime - (long)l.maxLogoutTime); });

            for (int i = 0, length = allServerTabs.Recommend.ZoneIds.Count; i < length; ++i) {
                var zoneId = allServerTabs.Recommend.ZoneIds[i];
                if (allZones.TryGetValue(zoneId, out var zoneEntry)) {
                    recommendTab.zones.Add(zoneEntry);
                }
            }

            SetDefaultServer();
            SetSelected(selectedZoneId);

            eventEmitter.Trigger(EEvents.OnServerListChanged);
        }

        private void SetDefaultServer() {
            // 检测上次选中的ZoneId是否依然存在
            if (!allZones.TryGetValue(selectedZoneId, out var _)) {
                selectedZoneId = 0;
            }

            // 默认选中推荐Tab
            if (selectedZoneId == 0) {
                if (lastTab.zones.Count > 0) {
                    selectedZoneId = lastTab.zones[0].svrZone.ZoneId;
                }
                else if (recommendTab.zones.Count > 0) {
                    selectedZoneId = recommendTab.zones[0].svrZone.ZoneId;
                }
                else if (allZones.Count > 0) {
                    selectedZoneId = allZones.ElementAt(0).Value.svrZone.ZoneId;
                }
            }
            else {
                // ignore
            }
        }

        public void SetSelected(uint zoneId) {
            selectedZoneId = zoneId;

            eventEmitter.Trigger<uint>(EEvents.OnSelectedServerChanged, selectedZoneId);
        }
    }
}