using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class LeftRightComponent : EntityComponent, Barebones.Framework.IUpdateable
    {
        float m_TimeAccumulator;

        float m_Min;
        float m_Max;
        float m_Speed;

        Matrix m_RotCenter;
        Matrix m_RotCenterInverse;

        public LeftRightComponent(float min_degrees, float max_degrees, float speed, Vector3 rotation_center)
        {
            m_Min = MathHelper.ToRadians(min_degrees);
            m_Max = MathHelper.ToRadians(max_degrees);
            m_Speed = speed;
            m_RotCenter = Matrix.CreateTranslation(-rotation_center);
            m_RotCenterInverse = Matrix.Invert(m_RotCenter);
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public void Update(float dt)
        {
            m_TimeAccumulator += dt;

            float ratio = 0.5f * (float)Math.Sin(m_TimeAccumulator * m_Speed * MathHelper.TwoPi) + 0.5f;
            float mult = MathHelper.Lerp(m_Min, m_Max, ratio);

            Owner.Transform = m_RotCenter * Matrix.CreateRotationZ(mult) * m_RotCenterInverse;
        }
    }
}
