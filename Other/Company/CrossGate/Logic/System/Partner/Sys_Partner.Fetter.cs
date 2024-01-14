using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;

namespace Logic {

    public partial class Sys_Partner : SystemModuleBase<Sys_Partner> {


        private List<PartnerBond> listBonds = new List<PartnerBond>();
        private Dictionary<uint, uint> dictAttrs = new Dictionary<uint, uint>();

        private void ConstructBondsData()
        {
            listBonds.Clear();
            int count = CSVBond.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                PartnerBond bond = new PartnerBond();
                bond.Id = CSVBond.Instance.GetByIndex(i).id;
                bond.Index = 0;
                listBonds.Add(bond);
            }
        }

        private void BondsDataValuation(PartnerInfo paInfo)
        {
            if (paInfo != null)
            {
                for (int i = 0; i < paInfo.Bond.Count; ++i)
                {
                    for (int j = 0; j < listBonds.Count; ++j)
                    {
                        if (paInfo.Bond[i].Id == listBonds[j].Id)
                        {
                            listBonds[j].Index = paInfo.Bond[i].Index;
                            break;
                        }
                    }
                }
            }
        }

        private void OnBondChangeNtf(NetMsg msg)
        {
            CmdPartnerBondChangeNtf ntf = NetMsgUtil.Deserialize<CmdPartnerBondChangeNtf>(CmdPartnerBondChangeNtf.Parser, msg);

            if (ntf.Bond != null)
            {
                for (int i = 0; i < ntf.Bond.Count; ++i)
                {
                    for (int j = 0; j < listBonds.Count; ++j)
                    {
                        if (ntf.Bond[i].Id == listBonds[j].Id)
                        {
                            listBonds[j].Index = ntf.Bond[i].Index;
                            break;
                        }
                    }
                }
            }
        }

        public PartnerBond GetBondData(uint bondId)
        {
            for (int i = 0; i < listBonds.Count; ++i)
            {
                if (bondId == listBonds[i].Id)
                {
                    return listBonds[i];
                }
            }

            return null;
        }

        public  Dictionary<uint, uint> GetAllAttrs()
        {
            dictAttrs.Clear();

            for (int i = 0; i < listBonds.Count; ++i)
            {
                PartnerBond paBond = listBonds[i];
                for (int j = 0; j < paBond.Index; ++j)
                {
                    CSVBond.Data bondInfo = CSVBond.Instance.GetConfData(paBond.Id);
                    CSVBondEffect.Data effectInfo = CSVBondEffect.Instance.GetConfData(bondInfo.group_effect[j]);
                    for (int k = 0; k < effectInfo.effective.Count; ++k)
                    {
                        uint arrId = effectInfo.effective[k][0];
                        uint arrValue = effectInfo.effective[k][1];

                        if (dictAttrs.ContainsKey(arrId))
                            dictAttrs[arrId] += arrValue;
                        else
                            dictAttrs.Add(arrId, arrValue);
                    }
                }
            }

            return dictAttrs;
        }
    }
}