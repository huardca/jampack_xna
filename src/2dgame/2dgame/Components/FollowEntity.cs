using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Framework;
using Microsoft.Xna.Framework;
using Barebones.Communication.Messages;

namespace _2dgame.Components
{
    class FollowEntity : EntityComponent
    {
        Entity m_ToFollow;
        Vector3 m_Delta;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            Owner.Engine.Forum.RegisterListener<PreRenderMessage>(msg => UpdatePos());

            base.OnOwnerSet();
        }

        public FollowEntity(Entity tofollow, Vector3 delta)
        {
            m_ToFollow = tofollow;
            m_Delta = delta;
        }

        void UpdatePos()
        {
            Vector3 desired = m_ToFollow.GetWorldTranslation();

            Vector3 current = Owner.GetWorldTranslation();
            desired.Z = current.Z;

            Owner.PlaceInWorld(desired + m_Delta);
        }
    }
}
