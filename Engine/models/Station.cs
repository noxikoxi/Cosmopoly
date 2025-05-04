using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    public class Station : SpaceEntity
    {
        public Station(string name) : base(name, false)
        {
                
        }

        protected override string ToStringInternal()
        {
            return "Stacja Kosmiczna";
        }
    }
}
