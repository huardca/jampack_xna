using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class Selectable : EntityComponent
    {
        BoundingBox m_Box;

        public Selectable(BoundingBox box)
        {
            m_Box = box;
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public bool Contains(Vector3 worldpoint)
        {
            Vector3 selfpos = Owner.GetWorldTranslation();
            worldpoint -= selfpos;

            return m_Box.Contains(worldpoint) == ContainmentType.Contains;
        }
    }
}
