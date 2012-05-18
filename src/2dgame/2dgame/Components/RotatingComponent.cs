using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class RotatingComponent : EntityComponent, Barebones.Framework.IUpdateable
    {
        float m_Radians;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public RotatingComponent(float degrees_per_second)
        {
            m_Radians = MathHelper.ToRadians(degrees_per_second);
        }

        public void Update(float dt)
        {
            Owner.Rotate(Vector3.UnitZ, m_Radians * dt);
        }
    }
}
