using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Framework;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class UpDownComponent : EntityComponent, Barebones.Framework.IUpdateable
    {
        float m_TimeAccumulator;

        float m_Min;
        float m_Max;
        float m_Speed;
        Vector3 m_Axis;

        public UpDownComponent(float min, float max, float speed, Vector3 axis)
        {
            m_Min = min;
            m_Max = max;
            m_Speed = speed;
            m_Axis = axis;
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

            Owner.SetTranslation(mult * m_Axis);
        }
    }
}
