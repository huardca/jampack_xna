using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Dependencies;
using _2dgame.EngineComponents;
using Microsoft.Xna.Framework;

namespace _2dgame.Components.Gameplay
{
    class Manifestant : EntityComponent, Barebones.Framework.IUpdateable
    {
        PhysicsComponent m_Physics;
        Recrutable m_Recrutable;

        float m_Force;

        public Manifestant(float force)
        {
            m_Force = force;
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<PhysicsComponent>(item => m_Physics = item);
            yield return new Dependency<Recrutable>(item => m_Recrutable = item);
        }

        public void Update(float dt)
        {
            if (m_Recrutable.Target == null)
                return;

            Vector3 playerPos = m_Recrutable.Target.GetWorldTranslation();

            Vector3 current = Owner.GetWorldTranslation();

            Vector3 toplayer3D = playerPos - current;
            Vector2 toplayer = new Vector2(toplayer3D.X, toplayer3D.Y);

            if (toplayer.Length() < 0.6f)
            {
                m_Physics.LinearVelocity = new Vector2(0,0);
            }
            else
            {
                toplayer.Normalize();
                m_Physics.ApplyForce(m_Force * toplayer);
            }
        }
    }
}
