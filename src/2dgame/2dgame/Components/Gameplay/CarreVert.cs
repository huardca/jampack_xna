using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Barebones.Dependencies;
using Barebones.Components;
using _2dgame.EngineComponents;

namespace _2dgame.Components.Gameplay
{
    class CarreVert : EntityComponent, Barebones.Framework.IUpdateable
    {
        Recrutable m_Recrutable;
        GameplayManager m_GM;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<Recrutable>(item => m_Recrutable = item);
            yield return new Dependency<GameplayManager>(item => m_GM = item);
        }

        public void Update(float dt)
        {
            if (m_Recrutable.Target == null)
                return;

            if(m_GM.InfluenceRadius > 0.61f)
                m_GM.InfluenceRadius -= 0.001f;
        }
    }
}
