using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table 
{
    public interface ILanguage
    {       
        bool TryGetLanguage(in uint id, out string words);
    }

    sealed public partial class CSVLanguage : Framework.Table.TableBase<CSVLanguage.Data>, ILanguage
    {
        public bool TryGetLanguage(in uint id, out string words)
        {            
            if (TryGetValue(id, out Data data))
            {
                words = data.words;               
                return true;
            }
            else
            {                
                words = null;
                return false;
            }
        }
    }
}