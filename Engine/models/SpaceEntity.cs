using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.models
{
    public abstract class SpaceEntity
    {
        public string Name { get;  private set; }

        public bool IsHabitable { get; private set; }

        public SpaceEntity(string name, bool isHabitable)
        {
            Name = name;
            IsHabitable = isHabitable;
        }

        protected abstract string ToStringInternal();


        public override string ToString()
        {
            return ToStringInternal();
        }

    }
}
