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
        const float IMAGE_SCALE = 0.01f;

        Random m_Rand = new Random();
        EZBake m_EZBakeOven;
        Physics m_Physics;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        /// <summary>
        /// Pour mieux commencer veuillez commencer par regarder la methode OnInitialise un peu plus bas pour voir
        /// comment créer ce qu'il y a l'écran.
        /// </summary>
        protected override void OnOwnerSet()
        {
            Owner.AddComponent(new ResourceLoader());

            m_EZBakeOven = new EZBake();
            Owner.AddComponent(m_EZBakeOven);

            //create renderer
            //create the projection matrix
            Matrix projection = Matrix.CreateOrthographic(IMAGE_SCALE * DESIRED_SCREEN_SIZE.X, IMAGE_SCALE * DESIRED_SCREEN_SIZE.Y, -1, 1);
            Owner.AddComponent(new RawRenderer(projection, Color.SkyBlue));

            Vector2 minWorld = -0.5f * IMAGE_SCALE * DESIRED_SCREEN_SIZE;
            minWorld.X = -1000;
            Vector2 maxWorld = 0.5f * IMAGE_SCALE * DESIRED_SCREEN_SIZE;
            maxWorld.X = 1000;
            maxWorld.Y = 1000;

            m_Physics = new Physics(-9.8f * Vector2.UnitY, minWorld, maxWorld)
                {
                    DebugView = true
                };
            Owner.AddComponent(m_Physics);

            Owner.AddComponent(new CameraMan());

            Owner.AddComponent(new TouchReader());
            Owner.AddComponent(new KeyboardReader());

            Owner.Forum.RegisterListener<InitializeMessage>(OnInitialise);
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

        void OnInitialise(InitializeMessage msg)
        {
            //add the camera to view the scene
            Entity camera = Owner.CreateEntity();
            camera.AddComponent(new Camera());

            camera.Transform = Matrix.CreateWorld(new Vector3(0, -1, 1), -Vector3.UnitZ, Vector3.UnitY);


            Entity queen = Owner.CreateEntity();

            //make camera follow queen
            camera.AddComponent(new FollowEntity(queen, 0.5f * Vector3.UnitY, false, true));

            Vector3 background_translation = -0.5f * Vector3.UnitY;
            CreateBackground(camera, background_translation);

            //create fanboys
            CreateBeefeater(new Vector3(4, -2, 0));
            CreateBeefeater(new Vector3(3, -2, 0));
            CreateBeefeater(new Vector3(2, -2, 0));
            CreateBeefeater(new Vector3(-2, -2, 0));
            CreateBeefeater(new Vector3(-3, -2, 0));
            CreateBeefeater(new Vector3(-4, -2, 0));

            //create body
            queen.Transform = Matrix.CreateTranslation(-1.5f * Vector3.UnitY);
            Vector2 bodySize = IMAGE_SCALE * new Vector2(300, 198);
            
            queen.AddComponent(m_Physics.CreateRectangle(0.5f * bodySize, 1.0f, FarseerPhysics.Dynamics.BodyType.Dynamic));
            m_Physics.ConstrainAngle(0, 0.01f, 0.4f, queen); //make body stay upright

            queen.AddComponent(new Selectable(new BoundingBox(new Vector3(-0.5f * bodySize, -2), new Vector3(0.5f * bodySize, 2))));
            queen.AddComponent(new FollowFinger());
            queen.AddComponent(new Queen(2, 10));
            m_EZBakeOven.MakeSprite(queen, bodySize, "body");

            //create neck
            Entity neck = queen.CreateChild();
            neck.Transform = Matrix.CreateTranslation(1.5f * Vector3.UnitY);

            //create head
            Entity head = neck.CreateChild();
            head.AddComponent(new LeftRightComponent(-30, 30, 0.75f, -0.5f * 1.7f * Vector3.UnitY));
            head.AddComponent(m_Physics.CreateCapsule(0.25f, 0.5f, 1.0f, FarseerPhysics.Dynamics.BodyType.Static));
            m_EZBakeOven.MakeSprite(head, IMAGE_SCALE * new Vector2(152, 175), "head");

            //create mouth hinge
            Entity mouthHinge = head.CreateChild();
            mouthHinge.Transform = Matrix.CreateTranslation(-0.85f * Vector3.UnitY);

            //create mouth
            Entity mouth = mouthHinge.CreateChild();
            mouth.AddComponent(new UpDownComponent(-0.2f, 0, 0.3f, Vector3.UnitY));
            m_EZBakeOven.MakeSprite(mouth, IMAGE_SCALE * new Vector2(79, 30), "mouth");

            //create flowers
            foreach(int i in Enumerable.Range(1, 10))
            {
                Entity flowers = Owner.CreateEntity();
                flowers.Transform = Matrix.CreateTranslation((2 + i * 1) * Vector3.UnitY + ((float)m_Rand.NextDouble() - 0.5f) * Vector3.UnitX);
                flowers.AddComponent(m_Physics.CreateTriangle(0.25f, 0.5f, 1, FarseerPhysics.Dynamics.BodyType.Dynamic));

                m_EZBakeOven.MakeSprite(flowers, IMAGE_SCALE * new Vector2(100, 100), "flowers");
            }

            //create cake
            Entity cake = Owner.CreateEntity();
            cake.Transform = Matrix.CreateTranslation(-2 * Vector3.UnitY);
            m_EZBakeOven.MakeSprite(cake, IMAGE_SCALE * new Vector2(200, 200), "cake");

            //create flame
            Entity flame = cake.CreateChild();
            flame.Transform = Matrix.CreateTranslation(Vector3.UnitY);
            m_EZBakeOven.MakeSprite(flame, IMAGE_SCALE * new Vector2(100, 100), "flame", 4, 10);
            flame.GetComponent<RenderSettings>().BlendState = BlendState.Additive;

            //add grass in front of everything
            Entity grass = Owner.CreateEntity();
            grass.Transform = Matrix.CreateTranslation(background_translation);
            grass.AddComponent(new FollowEntity(camera, Vector3.Zero, false, true));
            m_EZBakeOven.MakeParallaxSprite(grass, IMAGE_SCALE * new Vector2(800, 600), "grass", 1.0f);
        }

        private void CreateBackground(Entity camera, Vector3 translation)
        {
            Vector2 backsize = IMAGE_SCALE * new Vector2(800, 600);
            Vector3 delta_follow = Vector3.Zero;
            Matrix background_translation = Matrix.CreateTranslation(translation);

            Entity sun = Owner.CreateEntity();
            sun.Transform = background_translation;
            sun.AddComponent(new FollowEntity(camera, delta_follow, false, true));

            Entity sunjoint = sun.CreateChild();
            sunjoint.Transform = Matrix.CreateTranslation(new Vector3(-2, 1, 0));

            Vector2 sunsize = IMAGE_SCALE * new Vector2(300, 289);
            Entity sunray2 = sunjoint.CreateChild();
            sunray2.AddComponent(new RotatingComponent(-6));
            m_EZBakeOven.MakeSprite(sunray2, sunsize, "sunray2");

            Entity sunray1 = sunjoint.CreateChild();
            sunray1.AddComponent(new RotatingComponent(10));
            m_EZBakeOven.MakeSprite(sunray1, sunsize, "sunray1");

            Entity sunbody = sunjoint.CreateChild();
            sunbody.AddComponent(new RotatingComponent(180));
            m_EZBakeOven.MakeSprite(sunbody, sunsize, "sunbody");

            Entity sunface = sunjoint.CreateChild();
            m_EZBakeOven.MakeSprite(sunface, sunsize, "sunface");

            Entity backmountains = Owner.CreateEntity();
            backmountains.Transform = background_translation;
            backmountains.AddComponent(new FollowEntity(camera, delta_follow, false, true));
            m_EZBakeOven.MakeParallaxSprite(backmountains, backsize, "bumps_back", 0.5f);

            Entity trees = Owner.CreateEntity();
            trees.Transform = background_translation;
            trees.AddComponent(new FollowEntity(camera, delta_follow, false, true));
            m_EZBakeOven.MakeParallaxSprite(trees, backsize, "trees", 0.75f);

            Entity frontmountains = Owner.CreateEntity();
            frontmountains.Transform = background_translation;
            frontmountains.AddComponent(new FollowEntity(camera, delta_follow, false, true));
            m_EZBakeOven.MakeParallaxSprite(frontmountains, backsize, "bumps_front", 0.9f);
        }

        private void CreateBeefeater(Vector3 pos)
        {
            float beefeater_distance = 1.05f;

            Entity beefeater = Owner.CreateEntity();
            beefeater.Transform = Matrix.CreateScale(0.25f) * Matrix.CreateTranslation(pos);

            Entity beef_legs_joint = beefeater.CreateChild();
            beef_legs_joint.Transform = Matrix.CreateTranslation(-beefeater_distance * Vector3.UnitY);

            Entity beef_legs = beef_legs_joint.CreateChild();
            beef_legs.AddComponent(new LeftRightComponent(-10, 10, 1, -beefeater_distance * Vector3.UnitY));
            m_EZBakeOven.MakeSprite(beef_legs, IMAGE_SCALE * new Vector2(105, 219), "beefeater_legs");

            Entity beef_body_joint = beefeater.CreateChild();
            beef_body_joint.Transform = Matrix.CreateTranslation(beefeater_distance * Vector3.UnitY);

            Entity beef_body = beef_body_joint.CreateChild();
            beef_body.AddComponent(new LeftRightComponent(-10, 10, -1, beefeater_distance * Vector3.UnitY));
            m_EZBakeOven.MakeSprite(beef_body, IMAGE_SCALE * new Vector2(180, 351), "beefeater_body");
        }
    }
}
