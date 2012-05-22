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
using Microsoft.Xna.Framework.Audio;
using _2dgame.Components.Gameplay;

namespace _2dgame
{
    class Main : EngineComponent
    {
        static readonly Vector2 DESIRED_SCREEN_SIZE = new Vector2(800, 480);
        const float IMAGE_SCALE = 0.01f;

        static readonly Vector2 WORLD_SIZE = new Vector2(2048, 1094);

        Random m_Rand = new Random();
        EZBake m_EZBakeOven;
        Physics m_Physics;

        SoundEffectInstance m_MusicInstance;

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

            Vector2 minWorld = -0.5f * IMAGE_SCALE * WORLD_SIZE;
            minWorld.X = -1000;
            Vector2 maxWorld = 0.5f * IMAGE_SCALE * WORLD_SIZE;
            maxWorld.X = 1000;
            maxWorld.Y = 1000;

            m_Physics = new Physics(Vector2.Zero, 10 * minWorld, 10 * maxWorld)
                {
                    DebugView = false
                };

            Owner.AddComponent(m_Physics);

            Owner.AddComponent(new CameraMan());

            Owner.AddComponent(new TouchReader());
            Owner.AddComponent(new KeyboardReader());
            Owner.AddComponent(new GameplayManager()
            {
                InfluenceRadius = 1f
            });

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

        //this is like the loading phase, it will let us use these items as handles on entities later on
        void RegisterResources()
        {
            ResourceLoader loader = Owner.GetComponent<ResourceLoader>();
            var thememusic = new ContentResource<SoundEffect>("theme");
            loader.AddResource(thememusic);
        }

        void OnInitialise(InitializeMessage msg)
        {
            RegisterResources();

            //add the camera to view the scene
            Entity camera = Owner.CreateEntity();
            camera.AddComponent(new Camera());

            camera.Transform = Matrix.CreateWorld(new Vector3(0, -1, 1), -Vector3.UnitZ, Vector3.UnitY);

            CreateBackground();

            Entity character = Owner.CreateEntity();
            m_EZBakeOven.MakeSprite(character, 0.007f * new Vector2(24, 27), "panda_dos",4,5);
            character.AddComponent(m_Physics.CreateCircle(0.1f, 1, FarseerPhysics.Dynamics.BodyType.Dynamic));
            character.GetComponent<PhysicsComponent>().LinearDamping = 3;
            character.GetComponent<PhysicsComponent>().AngularDamping = 1000;
            character.AddComponent(new MainCharacter(1));
            character.AddComponent(new Recruter());

            Entity radiusBillboard = character.CreateChild();
            m_EZBakeOven.MakeSprite(radiusBillboard, new Vector2(2, 2), "radius");
            radiusBillboard.AddComponent(new InfluenceComponent());

            Entity zone = Owner.CreateEntity();
            zone.Transform = Matrix.CreateTranslation(new Vector3(-5, -9, 0));
            m_EZBakeOven.MakeSprite(zone, 0.005f * new Vector2(600, 303), "finish");
            zone.AddComponent(m_Physics.CreateRectangle(new Vector2(2, 1), 1, FarseerPhysics.Dynamics.BodyType.Static));
            zone.GetComponent<PhysicsComponent>().IsSensor = true;
            zone.AddComponent(new EndZone());

            Entity characterJoint = character.CreateChild();
            characterJoint.AddComponent(m_Physics.CreateCircle(0.8f, 1, FarseerPhysics.Dynamics.BodyType.Static));
            characterJoint.GetComponent<PhysicsComponent>().IsSensor = true;
            characterJoint.AddComponent(new Recruter());

            m_Physics.ConstrainAngle(0, 0.0003f, 0.2f, character);

            camera.AddComponent(new FollowEntity(character, Vector3.Zero));

            CreateManifestants(Vector3.Zero);

            CreateManifestants(new Vector3(8, 5, 0));

            CreateManifestants(new Vector3(-5, 7, 0));

            CreateManifestants(new Vector3(-6, -7, 0));

           for(int i=0;i < 10;i++)
            {
            
               Entity carreVert = Owner.CreateEntity();
            carreVert.Transform = Matrix.CreateTranslation(new Vector3(-1.1f*i, -0.9f*i, 0));



            m_EZBakeOven.MakeSprite(carreVert, 0.006f * new Vector2(21, 25), "vert_face");
            carreVert.AddComponent(m_Physics.CreateCircle(0.1f, 1, FarseerPhysics.Dynamics.BodyType.Dynamic));
            carreVert.GetComponent<PhysicsComponent>().LinearDamping = 2;
            carreVert.AddComponent(new CarreVert());
            carreVert.AddComponent(new Recrutable());
            }
            int policecount = 10;
            Matrix pos = Matrix.CreateTranslation(new Vector3(0, 5, 0));
            Matrix rot = Matrix.CreateRotationZ(MathHelper.TwoPi / policecount);

            for (int i = 0; i < policecount; i++)
            {
                Entity police = Owner.CreateEntity();
                police.Transform = pos;
                m_EZBakeOven.MakeSprite(police, 0.001f * new Vector2(300, 289), "police_bas", 3, 2);
                police.AddComponent(m_Physics.CreateCircle(0.1f, 1, FarseerPhysics.Dynamics.BodyType.Dynamic));
                police.GetComponent<PhysicsComponent>().LinearDamping = 4;
                police.AddComponent(new Police(0.1f, 4));
                police.AddComponent(new Recrutable());

                m_Physics.ConstrainAngle(0, float.MaxValue, 0, police);

                pos *= rot;
            }

            ResourceLoader loader = Owner.GetComponent<ResourceLoader>();
            loader.ForceLoadAll(); // so as to not have glitches in the first couple seconds while all the items are loaded as they are accessed

            m_MusicInstance = loader.GetResource("theme").Get<SoundEffect>().CreateInstance();
            m_MusicInstance.IsLooped = true;
            m_MusicInstance.Play();
        }

        private void CreateManifestants(Vector3 manifpos)
        {
            for (int i = 0; i < 50; i++)
            {
                Entity manifestant = Owner.CreateEntity();
                manifestant.Transform = Matrix.CreateTranslation(manifpos + new Vector3(-0.1f * i, -0.1f * i, 0));
                m_EZBakeOven.MakeSprite(manifestant, 0.006f * new Vector2(21, 22), "red_block_haut", 3, 5);
                manifestant.AddComponent(m_Physics.CreateCircle(0.1f, 0.1f, FarseerPhysics.Dynamics.BodyType.Dynamic));
                manifestant.GetComponent<PhysicsComponent>().LinearDamping = 2;
                m_Physics.ConstrainAngle(0, float.MaxValue, 0, manifestant);
                manifestant.AddComponent(new Manifestant(0.01f));
                manifestant.AddComponent(new Recrutable());
            }
        }

        private void CreateBackground()
        {
            Entity background = Owner.CreateEntity();
            background.Transform = Matrix.CreateRotationZ(MathHelper.ToRadians(34f));


            //background du jeux 
            Entity backgroundMilieu = background.CreateChild();
            m_EZBakeOven.MakeSprite(backgroundMilieu, IMAGE_SCALE * WORLD_SIZE, "Emily_gamelin_real");

            Entity backgroundGauche = background.CreateChild();
            backgroundGauche.Transform = Matrix.CreateTranslation(new Vector3(-1 * IMAGE_SCALE * WORLD_SIZE.X + 1.9f, 0.31f, 0));
            m_EZBakeOven.MakeSprite(backgroundGauche, IMAGE_SCALE * WORLD_SIZE, "Emily_gauche");

            Entity backgroundBasGauche = background.CreateChild();
            backgroundBasGauche.Transform = Matrix.CreateTranslation(new Vector3(-1 * IMAGE_SCALE * WORLD_SIZE.X + 1.9f, -1 * IMAGE_SCALE * WORLD_SIZE.Y, 0));
            m_EZBakeOven.MakeSprite(backgroundBasGauche, IMAGE_SCALE * WORLD_SIZE, "gauche_bas");


            //Entity backgroundDroite = background.CreateChild();
            // backgroundDroite.Transform = Matrix.CreateTranslation(new Vector3(1 * IMAGE_SCALE * WORLD_SIZE.X, .12f, 0));
            // m_EZBakeOven.MakeSprite(backgroundDroite, IMAGE_SCALE * WORLD_SIZE, "droite_emily");

            Entity backgroundHaut = background.CreateChild();
            backgroundHaut.Transform = Matrix.CreateTranslation(new Vector3(-.47f, 1 * IMAGE_SCALE * WORLD_SIZE.Y + .5f, 0));
            m_EZBakeOven.MakeSprite(backgroundHaut, IMAGE_SCALE * WORLD_SIZE, "emily_haut");


            //rectangle
            Entity building = Owner.CreateEntity();
            building.Transform = Matrix.CreateTranslation(new Vector3(4.9f, 1, 0));
            building.AddComponent(m_Physics.CreateRectangle(new Vector2(8.8f, 1.5f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building1 = Owner.CreateEntity();
            building1.Transform = Matrix.CreateTranslation(new Vector3(5.3f, -2.8f, 0));
            building1.AddComponent(m_Physics.CreateRectangle(new Vector2(8.8f, 3.5f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building2 = Owner.CreateEntity();
            building2.Transform = Matrix.CreateTranslation(new Vector3(4.9f, 3.3f, 0));
            building2.AddComponent(m_Physics.CreateRectangle(new Vector2(8.8f, 2.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building3 = Owner.CreateEntity();
            building3.Transform = Matrix.CreateTranslation(new Vector3(4.9f, 6.1f, 0));
            building3.AddComponent(m_Physics.CreateRectangle(new Vector2(8.8f, 2.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building4 = Owner.CreateEntity();
            building4.Transform = Matrix.CreateTranslation(new Vector3(-2.8f, -3f, 0));
            building4.AddComponent(m_Physics.CreateRectangle(new Vector2(4.2f, 3.9f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building5 = Owner.CreateEntity();
            building5.Transform = Matrix.CreateTranslation(new Vector3(-2.8f, -8.2f, 0));
            building5.AddComponent(m_Physics.CreateRectangle(new Vector2(4.2f, 3.9f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building6 = Owner.CreateEntity();
            building6.Transform = Matrix.CreateTranslation(new Vector3(-3.4f, 7.25f, 0));
            building6.AddComponent(m_Physics.CreateRectangle(new Vector2(5f, 4.8f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building7 = Owner.CreateEntity();
            building7.Transform = Matrix.CreateTranslation(new Vector3(4.5f, 8.7f, 0));
            building7.AddComponent(m_Physics.CreateRectangle(new Vector2(8.8f, 2.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building8 = Owner.CreateEntity();
            building8.Transform = Matrix.CreateTranslation(new Vector3(-8.5f, -2.0f, 0));
            building8.AddComponent(m_Physics.CreateRectangle(new Vector2(4.2f, 2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            //PArtie de moi
            Entity building10 = Owner.CreateEntity();
            building10.Transform = Matrix.CreateTranslation(new Vector3(-13f, -1.8f, 0));
            building10.AddComponent(m_Physics.CreateRectangle(new Vector2(8, 2.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building11 = Owner.CreateEntity();
            building11.Transform = Matrix.CreateTranslation(new Vector3(-13f, -4.5f, 0));
            building11.AddComponent(m_Physics.CreateRectangle(new Vector2(14, 2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building12 = Owner.CreateEntity();
            building12.Transform = Matrix.CreateTranslation(new Vector3(-8.6f, -8.5f, 0));
            building12.AddComponent(m_Physics.CreateRectangle(new Vector2(4.5f, 4.7f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building13 = Owner.CreateEntity();
            building13.Transform = Matrix.CreateTranslation(new Vector3(-14.5f, -8.5f, 0));
            building13.AddComponent(m_Physics.CreateRectangle(new Vector2(6f, 4.7f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building14 = Owner.CreateEntity();
            building14.Transform = Matrix.CreateTranslation(new Vector3(-18f, -7f, 0));
            building14.AddComponent(m_Physics.CreateRectangle(new Vector2(1f, 2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building15 = Owner.CreateEntity();
            building15.Transform = Matrix.CreateTranslation(new Vector3(-23f, -8f, 0));
            building15.AddComponent(m_Physics.CreateRectangle(new Vector2(8f, 6.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building16 = Owner.CreateEntity();
            building16.Transform = Matrix.CreateTranslation(new Vector3(-13f, -14.5f, 0));
            building16.AddComponent(m_Physics.CreateRectangle(new Vector2(4f, 6f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building17 = Owner.CreateEntity();
            building17.Transform = Matrix.CreateTranslation(new Vector3(-21.5f, -14.8f, 0));
            building17.AddComponent(m_Physics.CreateRectangle(new Vector2(8f, 6.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building18 = Owner.CreateEntity();
            building18.Transform = Matrix.CreateTranslation(new Vector3(-20.2f, -18f, 0));
            building18.AddComponent(m_Physics.CreateRectangle(new Vector2(8f, 6f), 1, FarseerPhysics.Dynamics.BodyType.Static));


            // buildings Kaka

            Entity building20 = Owner.CreateEntity();
            building20.Transform = Matrix.CreateTranslation(new Vector3(4.4f, 8.8f, 0));
            building20.AddComponent(m_Physics.CreateRectangle(new Vector2(8.6f, 2.4f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building21 = Owner.CreateEntity();
            building21.Transform = Matrix.CreateTranslation(new Vector3(4.2f, 12.0f, 0));
            building21.AddComponent(m_Physics.CreateRectangle(new Vector2(8.6f, 3.5f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building22 = Owner.CreateEntity();
            building22.Transform = Matrix.CreateTranslation(new Vector3(4.2f, 15.6f, 0));
            building22.AddComponent(m_Physics.CreateRectangle(new Vector2(8.6f, 2.3f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building23 = Owner.CreateEntity();
            building23.Transform = Matrix.CreateTranslation(new Vector3(-3.5f, 15.4f, 0));
            building23.AddComponent(m_Physics.CreateRectangle(new Vector2(5.0f, 2.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building24 = Owner.CreateEntity();
            building24.Transform = Matrix.CreateTranslation(new Vector3(-3.5f, 18.4f, 0));
            building24.AddComponent(m_Physics.CreateRectangle(new Vector2(5.0f, 2.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building25 = Owner.CreateEntity();
            building25.Transform = Matrix.CreateTranslation(new Vector3(-3.5f, 12.1f, 0));
            building25.AddComponent(m_Physics.CreateRectangle(new Vector2(5.6f, 3.6f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building26 = Owner.CreateEntity();
            building26.Transform = Matrix.CreateTranslation(new Vector3(-3.2f, 7.4f, 0));
            building26.AddComponent(m_Physics.CreateRectangle(new Vector2(5.4f, 5.0f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building27 = Owner.CreateEntity();
            building27.Transform = Matrix.CreateTranslation(new Vector3(-9.0f, 14.8f, 0));
            building27.AddComponent(m_Physics.CreateRectangle(new Vector2(4.0f, 1.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building28 = Owner.CreateEntity();
            building28.Transform = Matrix.CreateTranslation(new Vector3(-9.0f, 12.8f, 0));
            building28.AddComponent(m_Physics.CreateRectangle(new Vector2(3.7f, 2.0f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building29 = Owner.CreateEntity();
            building29.Transform = Matrix.CreateTranslation(new Vector3(-8.9f, 10.9f, 0));
            building29.AddComponent(m_Physics.CreateRectangle(new Vector2(4.1f, 1.2f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building30 = Owner.CreateEntity();
            building30.Transform = Matrix.CreateTranslation(new Vector3(-12.4f, 7.3f, 0));
            building30.AddComponent(m_Physics.CreateRectangle(new Vector2(11.4f, 4.8f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building31 = Owner.CreateEntity();
            building31.Transform = Matrix.CreateTranslation(new Vector3(-12.4f, 2.3f, 0));
            building31.AddComponent(m_Physics.CreateRectangle(new Vector2(11.4f, 4.0f), 1, FarseerPhysics.Dynamics.BodyType.Static));

            Entity building32 = Owner.CreateEntity();
            building32.Transform = Matrix.CreateTranslation(new Vector3(-13.8f, 11.4f, 0));
            building32.AddComponent(m_Physics.CreateRectangle(new Vector2(4.2f, 2.0f), 1, FarseerPhysics.Dynamics.BodyType.Static));
        }
    }
}
