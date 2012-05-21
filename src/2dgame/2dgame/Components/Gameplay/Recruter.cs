using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;

namespace _2dgame.Components.Gameplay
{
    class Recruter : EntityComponent
    {
        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }
    }
}
