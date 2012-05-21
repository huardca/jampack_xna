using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Dependencies;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Meat.Input;
using _2dgame.Events;

namespace _2dgame.Components
{
    class MainCharacter : EntityComponent, Barebones.Framework.IUpdateable
    {
        float m_Speed;
        PhysicsComponent m_Physics;
        KeyboardReader m_Keyboard;
        JointComponent m_Joint;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<KeyboardReader>(item => m_Keyboard = item);
            yield return new Dependency<PhysicsComponent>(item => m_Physics = item);
            yield return new Dependency<JointComponent>(item => m_Joint = item);
        }

        public MainCharacter(float speed)
        {
            m_Speed = speed;
        }

        protected override void OnOwnerSet()
        {
            Owner.Engine.Forum.RegisterListener<PlayerDeadMsg>(OnPlayerDead);

            base.OnOwnerSet();
        }

        void OnPlayerDead(PlayerDeadMsg msg)
        {
            this.Dispose();
        }

        public void Update(float dt)
        {
            Vector2 vel = m_Physics.LinearVelocity;
            vel.X = 0;
            vel.Y = 0;
            
            if (m_Keyboard.IsKeyDown(Keys.Right))
                vel.X += m_Speed;

            if (m_Keyboard.IsKeyDown(Keys.Left))
                vel.X -= m_Speed;

            if (m_Keyboard.IsKeyDown(Keys.Up))
                vel.Y += m_Speed;

            if (m_Keyboard.IsKeyDown(Keys.Down))
                vel.Y -= m_Speed;
            
            if(vel.LengthSquared() > 1e-5)
                vel.Normalize();

            m_Joint.TargetAngle = (float)Math.Atan2((double)vel.Y, (double)vel.X) - (float)Math.Atan2(1, 0);

            m_Physics.LinearVelocity = vel * m_Speed;
        }
    }
}
