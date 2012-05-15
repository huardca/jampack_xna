using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Meat.Resources;
using Barebones.Dependencies;
using Microsoft.Xna.Framework;
using Barebones.Xna;
using Meat.Rendering;
using Meat.Resources.Factory;
using Barebones.Framework;
using _2dgame.Components;
using Microsoft.Xna.Framework.Graphics;
using _2dgame.EngineComponents;
using Meat.Input;

namespace _2dgame
{
    class Main : EngineComponent
    {
        static readonly Vector2 DESIRED_SCREEN_SIZE = new Vector2(800, 480);

        Random m_Rand = new Random();
        EZBake m_EZBakeOven;
        Physics m_Physics;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            Owner.AddComponent(new ResourceLoader());

            m_EZBakeOven = new EZBake();
            Owner.AddComponent(m_EZBakeOven);

            //create renderer
            //create the projection matrix
            Matrix projection = Matrix.CreateOrthographic(0.1f * DESIRED_SCREEN_SIZE.X, 0.1f * DESIRED_SCREEN_SIZE.Y, -1, 1);
            Owner.AddComponent(new RawRenderer(projection, Color.White));

            Vector2 minWorld = -0.5f * 0.1f * DESIRED_SCREEN_SIZE;
            minWorld.X = -1000;
            Vector2 maxWorld = 0.5f * 0.1f * DESIRED_SCREEN_SIZE;
            maxWorld.X = 1000;
            maxWorld.Y = 1000;

            m_Physics = new Physics(-20f * Vector2.UnitY, minWorld, maxWorld);
            Owner.AddComponent(m_Physics);

            Owner.AddComponent(new CameraMan());

            Owner.AddComponent(new TouchReader());
            Owner.AddComponent(new KeyboardReader());

            Owner.Forum.RegisterListener<InitializeMessage>(OnInitialiser);
            Owner.Forum.RegisterListener<CreatedMessage>(OnCreated);

            base.OnOwnerSet();
        }

        void OnCreated(CreatedMessage msg)
        {
            msg.Manager.PreferredBackBufferWidth = (int)DESIRED_SCREEN_SIZE.X;
            msg.Manager.PreferredBackBufferHeight = (int)DESIRED_SCREEN_SIZE.Y;

            msg.Manager.SupportedOrientations = DisplayOrientation.LandscapeLeft |
                                                DisplayOrientation.LandscapeRight;
        }

        void OnInitialiser(InitializeMessage msg)
        {
            //add the camera to view the scene
            Entity camera = Owner.CreateEntity();
            camera.AddComponent(new Camera());
            camera.Transform = Matrix.CreateLookAt(-Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);

            //create body
            Entity body = Owner.CreateEntity();
            body.Transform = Matrix.CreateTranslation(-15 * Vector3.UnitY);
            Vector2 bodySize = new Vector2(30, 19.8f);
            
            body.AddComponent(m_Physics.CreateRectangle(0.5f * bodySize, 1.0f, FarseerPhysics.Dynamics.BodyType.Dynamic));
            m_Physics.ConstrainAngle(0, 100, 0.4f, body); //make body stay upright

            body.AddComponent(new Selectable(new BoundingBox(new Vector3(-0.5f * bodySize, -2), new Vector3(0.5f * bodySize, 2))));
            body.AddComponent(new FollowFinger());
            body.AddComponent(new Queen(20, 5000));
            m_EZBakeOven.MakeSprite(body, bodySize, "body");

            //make camera follow body
            camera.AddComponent(new FollowEntity(body, 5 * Vector3.UnitY));

            //create neck
            Entity neck = body.CreateChild();
            neck.Transform = Matrix.CreateTranslation(6 * Vector3.UnitY);

            //create head
            Entity head = neck.CreateChild();
            head.AddComponent(new LeftRightComponent(-30, 30, 0.75f, -0.5f * 17.5f * Vector3.UnitY));
            head.AddComponent(m_Physics.CreateCapsule(5, 5, 1.0f, FarseerPhysics.Dynamics.BodyType.Static));
            m_EZBakeOven.MakeSprite(head, new Vector2(15.2f, 17.5f), "head");

            //create mouth hinge
            Entity mouthHinge = head.CreateChild();
            mouthHinge.Transform = Matrix.CreateTranslation(-8.5f * Vector3.UnitY);

            //create mouth
            Entity mouth = mouthHinge.CreateChild();
            mouth.AddComponent(new UpDownComponent(-2, 0, 0.3f, Vector3.UnitY));
            m_EZBakeOven.MakeSprite(mouth, new Vector2(7.9f, 3.0f), "mouth");

            //create flowers
            foreach(int i in Enumerable.Range(1, 10))
            {
                Entity flowers = Owner.CreateEntity();
                flowers.Transform = Matrix.CreateTranslation((20 + i * 10) * Vector3.UnitY + 10 * ((float)m_Rand.NextDouble() - 0.5f) * Vector3.UnitX);
                flowers.AddComponent(m_Physics.CreateCapsule(5, 2.5f, 1, FarseerPhysics.Dynamics.BodyType.Dynamic));

                m_EZBakeOven.MakeSprite(flowers, new Vector2(10, 10), "flowers");
            }

            //create cake
            Entity cake = Owner.CreateEntity();
            cake.Transform = Matrix.CreateTranslation(-20 * Vector3.UnitY);
            m_EZBakeOven.MakeSprite(cake, new Vector2(20, 20), "cake");

            //create flame
            Entity flame = cake.CreateChild();
            flame.Transform = Matrix.CreateTranslation(10 * Vector3.UnitY);
            m_EZBakeOven.MakeSprite(flame, new Vector2(10, 10), "flame", 4, 10);
            flame.GetComponent<RenderSettings>().Blend = BlendState.Additive;
        }
    }
}
