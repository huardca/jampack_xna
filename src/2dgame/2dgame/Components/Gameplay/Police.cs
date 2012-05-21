using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Dependencies;
using _2dgame.EngineComponents;
using Microsoft.Xna.Framework;
using Barebones.Framework;

namespace _2dgame.Components.Gameplay
{
    class Police : EntityComponent, Barebones.Framework.IUpdateable
    {
        PhysicsComponent m_Physics;
        GameplayManager m_Gameplay;
        float m_Force;

        public Police(float force)
        {
            m_Force = force;
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<PhysicsComponent>(item => m_Physics = item);
            yield return new Dependency<GameplayManager>(item => m_Gameplay = item);
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<CollisionMsg>(OnCollision);
            
            base.OnOwnerSet();
        }

        void OnCollision(CollisionMsg msg)
        {
            Entity target = msg.First == Owner ? msg.Second : msg.First;

            if (target.GetComponent<Manifestant>() != null)
            {
                target.Dispose();
                m_Gameplay.InfluenceRadius -= 0.02f;
            }
        }

        public void Update(float dt)
        {
            MainCharacter player = m_Gameplay.Player;
            Vector3 playerPos = player.Owner.GetWorldTranslation();

            Vector3 current = Owner.GetWorldTranslation();

            Vector3 toplayer3D = playerPos - current;
            Vector2 toplayer = new Vector2(toplayer3D.X, toplayer3D.Y);
            toplayer.Normalize();

            m_Physics.ApplyForce(m_Force * toplayer);
        }
    }
}
