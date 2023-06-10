using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FantasyGoblinsUpdated
{
    public class GeneCompatibility : DefModExtension
    {
        public List<String> overridesTag;
        public List<GeneDef> overrides;
        public List<String> overriddenByTag;
        public List<GeneDef> overriddenBy;
    }
}
