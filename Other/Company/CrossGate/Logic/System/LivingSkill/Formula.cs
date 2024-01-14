using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;


namespace Logic
{
    /// <summary>
    /// 配方
    /// </summary>
    public class Formula 
    {
        public uint Id { get; private set; }

        public CSVFormula.Data cSVFormulaData { get; private set; }
        
        public Formula(uint id )  {this.Id = id; }
        
    }
}

