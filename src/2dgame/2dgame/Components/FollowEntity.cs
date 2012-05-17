using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Framework;
using Microsoft.Xna.Framework;
using Barebones.Communication.Messages;
using Meat.Rendering;
using Barebones.Dependencies;

namespace _2dgame.Components
{
    class FollowEntity : EntityComponent
    {
        Entity m_ToFollow;
        Vector3 m_Delta;
        UserVertexModifier m_VertexModifier;

        bool m_NotX;
        bool m_NotY;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<UserVertexModifier>(item => m_VertexModifier = item);
        }

        protected override void OnOwnerSet()
        {
            Owner.Engine.Forum.RegisterListener<PreRenderMessage>(msg => UpdatePos());

            base.OnOwnerSet();
        }

        public FollowEntity(Entity tofollow, Vector3 delta)
            : this( tofollow, delta, false, false )
        {}

        public FollowEntity(Entity tofollow, Vector3 delta, bool notx, bool noty)
        {
            m_NotX = notx;
            m_NotY = noty;
            m_ToFollow = tofollow;
            m_Delta = delta;
        }

        void UpdatePos()
        {
            Vector3 desired = m_ToFollow.GetWorldTranslation() + m_Delta;

            Vector3 current = Owner.GetWorldTranslation();
            desired.Z = current.Z;

            if (m_NotX)
                desired.X = current.X;

            if (m_NotY)
                desired.Y = current.Y;

            Owner.Transform = Matrix.CreateTranslation(desired);
            //Owner.PlaceInWorld(desired);

            if (m_VertexModifier == null)
                return;
            
            Vector3 translation = desired - current;
            m_VertexModifier.TranslateTexCoordsByWorld(new Vector2(translation.X, translation.Y));
        }
    }
}
