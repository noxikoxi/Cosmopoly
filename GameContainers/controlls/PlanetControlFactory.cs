using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;

namespace GameContainers.controlls
{
    public static class PlanetControlFactory
    {
        private static readonly Dictionary<Type, Func<int, int, IPlanetControl>> controlFactory = new()
        {
            { typeof(Engine.models.Singularity), (width, height) => new Singularity(width, height) },
            { typeof(Engine.models.Station),     (width, height) => new Station(width, height) },
            { typeof(Engine.models.PiratePlanet),     (width, height) => new Pirates(width, height) }
        };

        public static IPlanetControl? CreateControl(object model, int width, int height)
        {
            if (model == null) return null;

            var type = model.GetType();
            return controlFactory.TryGetValue(type, out var constructor)
                ? constructor(width, height)
                : null;
        }
    }
}
