using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Microsoft.Xna.Framework.Audio;
using Meat.Resources;
using Barebones.Dependencies;

namespace _2dgame.Components
{
    class SoundOnCollision : EntityComponent, Barebones.Framework.IUpdateable
    {
        Handle<SoundEffect> m_Sound;

        bool m_Playing;
        float m_TimeBeforeNextPlay = 0;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<Handle<SoundEffect>>(item => m_Sound = item, item => m_Sound = null);
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<CollisionMsg>(OnCollision);

            base.OnOwnerSet();
        }

        void OnCollision(CollisionMsg msg)
        {
            if (m_TimeBeforeNextPlay > 0)
                return;

            SoundEffect effect = m_Sound.Get();

            effect.Play();
            m_TimeBeforeNextPlay = (float)effect.Duration.TotalSeconds;
            m_Playing = true;
        }

        public void Update(float dt)
        {
            if (m_Playing)
            {
                m_TimeBeforeNextPlay -= dt;
                if (m_TimeBeforeNextPlay < 0)
                    m_Playing = false;
            }
        }
    }
}
