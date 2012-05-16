using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Barebones.Components;
using Microsoft.Xna.Framework;
using _2dgame.Components;
using FarseerPhysics.Factories;
using Barebones.Dependencies;
using FarseerPhysics;
using FarseerPhysics.DebugViews;
using Meat.Rendering;
using Barebones.Xna;
using FarseerPhysics.Dynamics.Joints;
using Barebones.Framework;

namespace _2dgame.EngineComponents
{
    class Physics : EngineComponent, Barebones.Framework.IUpdateable, Barebones.Framework.IDrawable
    {
        World m_World;
        DebugViewXNA m_View;

        RawRenderer m_Renderer;
        Camera m_Camera;

        public bool DebugView
        {
            get { return m_View.Enabled; }
            set { m_View.Enabled = value; }
        }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<PhysicsComponent>(item => { }, item => item.DisposePhysics(m_World));
            yield return new Dependency<RawRenderer>(item => m_Renderer = item, item => m_Renderer = null);
            yield return new Dependency<Camera>(item => m_Camera = item, item => m_Camera = null);
        }

        public Physics(Vector2 gravity, Vector2 min, Vector2 max)
        {
            m_World = new World(gravity, new FarseerPhysics.Collision.AABB(ref min, ref max));
            m_View = new DebugViewXNA(m_World);
            m_View.Flags |= DebugViewFlags.PerformanceGraph;
            m_View.AppendFlags(DebugViewFlags.Shape);
            m_View.AppendFlags(DebugViewFlags.DebugPanel);
            m_View.AppendFlags(DebugViewFlags.Joint);
            m_View.AppendFlags(DebugViewFlags.ContactPoints);
            m_View.AppendFlags(DebugViewFlags.ContactNormals);
            m_View.AppendFlags(DebugViewFlags.Controllers);
            m_View.AppendFlags(DebugViewFlags.CenterOfMass);
            m_View.AppendFlags(DebugViewFlags.AABB);

            m_View.DefaultShapeColor = Color.Green;
            m_View.SleepingShapeColor = Color.LightGray;

            //floor
            BodyFactory.CreateEdge(m_World, new Vector2(min.X, min.Y), new Vector2(max.X, min.Y));

            //right side
            BodyFactory.CreateEdge(m_World, new Vector2(max.X, min.Y), new Vector2(max.X, max.Y));

            //left side
            BodyFactory.CreateEdge(m_World, new Vector2(min.X, min.Y), new Vector2(min.X, max.Y));

            //top
            BodyFactory.CreateEdge(m_World, new Vector2(min.X, max.Y), new Vector2(max.X, max.Y)); 
        }

        public Physics(Vector2 gravity)
        {
            m_World = new World(gravity);
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<LoadContentMessage>(item =>
                {
                    m_View.LoadContent(item.Device, item.Content);
                });

            base.OnOwnerSet();
        }

        public PhysicsComponent CreateRectangle(Vector2 size, float density, BodyType bodytype)
        {
            Body body = BodyFactory.CreateRectangle(m_World, size.X, size.Y, density);
            body.BodyType = bodytype;
            return new PhysicsComponent(body);
        }

        public PhysicsComponent CreateCircle(float radius, float density, BodyType bodytype)
        {
            Body body = BodyFactory.CreateCircle(m_World, radius, density);
            body.BodyType = bodytype;
            return new PhysicsComponent(body);
        }

        public PhysicsComponent CreateCapsule(float height, float radius, float density, BodyType bodytype)
        {
            Body body = BodyFactory.CreateCapsule(m_World, height, radius, density);
            body.BodyType = bodytype;
            return new PhysicsComponent(body);
        }

        public PhysicsComponent CreateRoundedRectangle(float width, float height, float xradius, float yradius, int segments, float density, BodyType bodytype)
        {
            Body body = BodyFactory.CreateRoundedRectangle(m_World, width, height, xradius, yradius, segments, density);
            body.BodyType = bodytype;
            return new PhysicsComponent(body);
        }

        public void ConstrainAngle(float angle, float maximpulse, float softness, Entity entity)
        {
            PhysicsComponent physics = entity.GetComponent<PhysicsComponent>();
            if (physics == null)
                throw new InvalidOperationException("Joint needs a physics component to constrain");

            FixedAngleJoint joint = JointFactory.CreateFixedAngleJoint(m_World, physics.GetBody());
            joint.TargetAngle = angle;
            joint.MaxImpulse = maximpulse;
            joint.Softness = softness;

            entity.AddComponent(new JointComponent(joint));
        }

        public void Update(float dt)
        {
            m_World.Step(dt);
        }

        public void Draw()
        {
            Matrix proj = m_Renderer.Projection;
            Matrix view = m_Camera.GetView();

            m_View.RenderDebugData(ref proj, ref view);
        }
    }
}
