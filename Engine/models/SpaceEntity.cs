using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CosmopolyEngine.models
{
    public abstract class SpaceEntity
    {
        private string name;

        private bool isHabitable;



        public SpaceEntity(string name, bool isHabitable)
        {
            this.name = name;
            this.isHabitable = isHabitable;
        }

        public bool IsHabitable()
        {
            return isHabitable;
        }

        public string GetName()
        {
            return name;
        }

        protected abstract string ToStringInternal();


        public override string ToString()
        {
            return ToStringInternal();
        }

    }
}
