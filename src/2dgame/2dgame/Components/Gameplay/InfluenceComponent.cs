using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using _2dgame.EngineComponents;
using Barebones.Dependencies;
using Microsoft.Xna.Framework;

namespace _2dgame.Components.Gameplay
{
    class InfluenceComponent : EntityComponent, Barebones.Framework.IUpdateable
    {
        GameplayManager m_GM;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<GameplayManager>(item => m_GM = item);
        }

        public void Update(float dt)
        {
            Owner.Transform = Matrix.CreateScale(m_GM.InfluenceRadius);
        }
    }
}
