using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace _2dgame.Components
{
    class PhysicsComponent : EntityComponent, Barebones.Framework.IUpdateable
    {
        Body m_Body;

        public BodyType BodyType
        {
            get { return m_Body.BodyType; }
            set { m_Body.BodyType = value; }
        }

        public Vector2 LinearVelocity
        {
            get { return m_Body.LinearVelocity; }
            set { m_Body.LinearVelocity = value; }
        }

        public float AngularVelocity
        {
            get { return m_Body.AngularVelocity; }
            set { m_Body.AngularVelocity = value; }
        }

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
            AssignEntityTransformToBody();

            base.OnOwnerSet();
        }

        public void ApplyForce(Vector2 force)
        {
            m_Body.ApplyForce(ref force);
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            m_Body.ApplyLinearImpulse(ref impulse);
        }

        public void Update(float dt)
        {
            if (m_Body.BodyType == BodyType.Static)
                AssignEntityTransformToBody();
            else
                AssignBodyTransformToEntity();
        }

        public void DisposePhysics(World world)
        {
            world.RemoveBody(m_Body);
        }

        public Body GetBody()
        { return m_Body; }

        private void AssignEntityTransformToBody()
        {
            Matrix transform = Owner.GetWorld();
            double angle = Math.Atan2(0, 1) - Math.Atan2(transform.Up.X, transform.Up.Y);
            m_Body.SetTransform(new Vector2(transform.Translation.X, transform.Translation.Y), (float)angle);
        }

        private void AssignBodyTransformToEntity()
        {
            Transform phystrans;
            m_Body.GetTransform(out phystrans);

            Matrix desired;
            Matrix.CreateRotationZ(phystrans.R.Angle, out desired);
            desired.Translation = new Vector3(phystrans.Position, 0);

            Owner.PlaceInWorld(desired);
        }
    }
}
