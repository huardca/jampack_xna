using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Dependencies;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class MainCharacter : EntityComponent, Barebones.Framework.IUpdateable
    {
        float m_Speed;
        PhysicsComponent m_Physics;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<PhysicsComponent>(item => m_Physics = item);
        }

        public MainCharacter(float speed)
        {
            m_Speed = speed;
        }

        public void Update(float dt)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            Vector2 direction = state.ThumbSticks.Left;

            m_Physics.LinearVelocity = m_Speed * direction;
        }
    }
}
