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
        Matrix m_Rot;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public RotatingComponent(float degrees_per_second)
        {
            m_Rot = Matrix.CreateRotationZ(MathHelper.ToRadians(degrees_per_second / 30.0f));
        }

        public void Update(float dt)
        {
            Owner.Transform *= m_Rot;
        }
    }
}
