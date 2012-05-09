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

namespace _2dgame
{
    class Main : EngineComponent
    {
        EZBake m_EZBakeOven;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            Owner.AddComponent(new ResourceLoader());

            m_EZBakeOven = new EZBake();
            Owner.AddComponent(m_EZBakeOven);

            Owner.Forum.RegisterListener<InitializeMessage>(OnInitialiser);
            Owner.Forum.RegisterListener<CreatedMessage>(OnCreated);

            base.OnOwnerSet();
        }

        void OnCreated(CreatedMessage msg)
        {
            msg.Manager.PreferredBackBufferWidth = 800;
            msg.Manager.PreferredBackBufferHeight = 480;

            msg.Manager.SupportedOrientations = DisplayOrientation.LandscapeLeft |
                                                DisplayOrientation.LandscapeRight;
        }

        void OnInitialiser(InitializeMessage msg)
        {
            //create the projection matrix
            Matrix projection = Matrix.CreateOrthographic(msg.Device.Viewport.Width, msg.Device.Viewport.Height, -1, 1);

            //create renderer
            Owner.AddComponent(new RawRenderer(projection, Color.White));

            //add the camera to view the scene
            Entity camera = Owner.CreateEntity();
            camera.AddComponent(new Camera());
            camera.Transform = Matrix.CreateLookAt(Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);

            //create body
            Entity body = Owner.CreateEntity();
            body.Transform = Matrix.CreateTranslation(-300 * Vector3.UnitX);
            body.AddComponent(new FollowFinger());
            m_EZBakeOven.MakeSprite(body, new Vector2(300, 198), "body");

            //create neck
            Entity neck = body.CreateChild();
            neck.Transform = Matrix.CreateTranslation(60 * Vector3.UnitY);

            //create head
            Entity head = neck.CreateChild();
            head.AddComponent(new LeftRightComponent(-30, 30, 0.75f, -0.5f * 175 * Vector3.UnitY));
            m_EZBakeOven.MakeSprite(head, new Vector2(152, 175), "head");

            //create mouth hinge
            Entity mouthHinge = head.CreateChild();
            mouthHinge.Transform = Matrix.CreateTranslation(-85 * Vector3.UnitY);

            //create mouth
            Entity mouth = mouthHinge.CreateChild();
            mouth.AddComponent(new UpDownComponent(-20, 0, 3, Vector3.UnitY));
            m_EZBakeOven.MakeSprite(mouth, new Vector2(79, 30), "mouth");

            //create cake
            Entity cake = Owner.CreateEntity();
            cake.Transform = Matrix.CreateTranslation(300 * Vector3.UnitX);
            m_EZBakeOven.MakeSprite(cake, new Vector2(200, 200), "cake");

            //create flame
            Entity flame = cake.CreateChild();
            flame.Transform = Matrix.CreateTranslation(100 * Vector3.UnitY);
            m_EZBakeOven.MakeSprite(flame, new Vector2(100, 100), "flame", 4, 10);
            flame.GetComponent<RenderSettings>().Blend = BlendState.Additive;
        }
    }
}
