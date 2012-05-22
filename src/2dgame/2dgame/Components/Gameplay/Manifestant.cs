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
        bool m_PickedUp = false;
        GameplayManager m_GM;

        JointComponent m_Joint;
        float m_Force;

        public Manifestant(float force)
        {
            m_Force = force;
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<PhysicsComponent>(item => m_Physics = item);
            yield return new Dependency<Recrutable>(item => m_Recrutable = item);
            yield return new Dependency<JointComponent>(item => m_Joint = item);
            yield return new Dependency<GameplayManager>(item => m_GM = item);
        }

        public void Arreter()
        {
            this.Dispose();
        }

        public void Update(float dt)
        {
            if (m_Recrutable.Target == null)
            {
                m_PickedUp = false;
                return;
            }

            if (m_PickedUp == false)
            {
                m_PickedUp = true;
                m_GM.InfluenceRadius += 0.05f;
            }

            Vector3 playerPos = m_Recrutable.Target.GetWorldTranslation();

            Vector3 current = Owner.GetWorldTranslation();

            Vector3 toplayer3D = playerPos - current;
            Vector2 toplayer = new Vector2(toplayer3D.X, toplayer3D.Y);
            
            m_Joint.TargetAngle = (float)Math.Atan2((double)toplayer.Y, (double)toplayer.X) - (float)Math.Atan2(1, 0);

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
