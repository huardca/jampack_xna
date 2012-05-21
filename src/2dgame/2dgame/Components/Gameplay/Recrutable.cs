using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Framework;
using _2dgame.EngineComponents;
using Barebones.Dependencies;
using Microsoft.Xna.Framework;

namespace _2dgame.Components.Gameplay
{
    class Recrutable : EntityComponent, Barebones.Framework.IUpdateable
    {
        public Entity Target
        { get; set; }
        GameplayManager m_GM;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<GameplayManager>(item => m_GM = item);
            yield break;
        }

        protected override void OnOwnerSet()
        {
            base.OnOwnerSet();
        }

        public void Update(float dt)
        {
            if(m_GM.Player == null)
                return;

            Vector3 playerPos = m_GM.Player.Owner.GetWorldTranslation();

            Vector3 current = Owner.GetWorldTranslation();

            Vector3 toplayer3D = playerPos - current;
            Vector2 toplayer = new Vector2(toplayer3D.X, toplayer3D.Y);

            if (toplayer.LengthSquared() < m_GM.InfluenceRadius*m_GM.InfluenceRadius)
            {
                Target = m_GM.Player.Owner;
            }
            else
            {
                Target = null;
            }
            
        }
    }
}
