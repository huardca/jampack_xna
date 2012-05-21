using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Dependencies;
using _2dgame.EngineComponents;
using Microsoft.Xna.Framework;
using Barebones.Framework;
using _2dgame.Events;

namespace _2dgame.Components.Gameplay
{
    class Police : EntityComponent, Barebones.Framework.IUpdateable
    {
        PhysicsComponent m_Physics;
        Recrutable m_Recrutable;
        JointComponent m_Joint;
        GameplayManager m_GM;

        float m_Force;
        float m_Time = 0;

        readonly float m_Cooldown;


        public Police(float force, float cooldown)
        {
            m_Force = force;
            m_Cooldown = cooldown;
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<PhysicsComponent>(item => m_Physics = item);
            yield return new Dependency<Recrutable>(item => m_Recrutable = item);
            yield return new Dependency<JointComponent>(item => m_Joint = item);
            yield return new Dependency<GameplayManager>(item => m_GM = item);
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<CollisionMsg>(OnCollision);

            base.OnOwnerSet();
        }

        void OnCollision(CollisionMsg msg)
        {
            if (m_Time > 1e-5)
                return;

            Entity target = msg.First == Owner ? msg.Second : msg.First;

            if (target.GetComponent<Manifestant>() != null)
            {
                target.Dispose();
                m_GM.InfluenceRadius -= 0.02f;
                m_Time = m_Cooldown;
            }
            else if (target.GetComponent<MainCharacter>() != null)
            {
                Owner.Engine.Forum.Fire<PlayerDeadMsg>(new PlayerDeadMsg()
                    {
                        Player = target
                    });
                Owner.Dispose();
            }
        }

        public void Update(float dt)
        {
            if (m_Recrutable.Target == null || m_Time > 1e-5)
            {
                m_Time -= dt;
                return;
            }

            Vector3 playerPos = m_Recrutable.Target.GetWorldTranslation();

            Vector3 current = Owner.GetWorldTranslation();

            Vector3 toplayer3D = playerPos - current;
            Vector2 toplayer = new Vector2(toplayer3D.X, toplayer3D.Y);
            toplayer.Normalize();

            m_Physics.ApplyForce(m_Force * toplayer);

            m_Joint.TargetAngle = (float)Math.Atan2((double)toplayer.Y, (double)toplayer.X) - (float)Math.Atan2(1, 0);
        }
    }
}
