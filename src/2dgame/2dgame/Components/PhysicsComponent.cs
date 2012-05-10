using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class PhysicsComponent : EntityComponent, Barebones.Framework.IUpdateable
    {
        Body m_Body;

        public PhysicsComponent(Body body)
        {
            m_Body = body;
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            SyncBodyToEntity();

            base.OnOwnerSet();
        }

        public void ApplyForce(Vector2 force)
        {
            m_Body.ApplyForce(ref force);
        }

        public void SetVelocity(Vector2 velocity)
        {
            m_Body.LinearVelocity = velocity;
        }

        public void Update(float dt)
        {
            if (m_Body.BodyType == BodyType.Static)
                SyncBodyToEntity();
            else
                SyncEntityToBody();
        }

        public void DisposePhysics(World world)
        {
            world.RemoveBody(m_Body);
        }

        private void SyncBodyToEntity()
        {
            Matrix transform = Owner.GetWorld();
            double angle = Math.Atan2(0, 1) - Math.Atan2(transform.Up.X, transform.Up.Y);
            m_Body.SetTransform(new Vector2(transform.Translation.X, transform.Translation.Y), (float)angle);//TODO set angle
        }

        private void SyncEntityToBody()
        {
            Transform phystrans;
            m_Body.GetTransform(out phystrans);

            Matrix world;
            Matrix.CreateRotationZ(phystrans.R.Angle, out world);
            world.Translation = new Vector3(phystrans.Position, 0);

            Owner.Transform = Matrix.Identity;

            Matrix currentworld = Owner.GetWorld();
            Owner.Transform = Matrix.Invert(currentworld) * world;
        }
    }
}
