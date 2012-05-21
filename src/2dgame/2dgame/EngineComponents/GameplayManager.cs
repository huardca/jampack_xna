using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using _2dgame.Components;
using Barebones.Dependencies;
using _2dgame.Components.Gameplay;
using _2dgame.Events;
using Barebones.Framework;
using Meat.Resources.Factory;
using Microsoft.Xna.Framework;

namespace _2dgame.EngineComponents
{
    class GameplayManager : EngineComponent
    {
        MainCharacter m_Player;
        List<Recrutable> m_Recrutable = new List<Recrutable>();
        List<Recruter> m_Recruters = new List<Recruter>();
        public float InfluenceRadius
        { get; set; }

        public MainCharacter Player
        { get { return m_Player; } }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<MainCharacter>(item => m_Player = item, item => m_Player = null);
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<PlayerDeadMsg>(OnPlayerDead);
            base.OnOwnerSet();
        }

        void OnPlayerDead(PlayerDeadMsg msg)
        {
            Entity deadmessage = Owner.CreateEntity();
            Owner.GetComponent<EZBake>().MakeSprite(deadmessage, 0.004f * new Vector2(1024, 824), "charest");

            deadmessage.SetTranslation(msg.Player.GetWorldTranslation());
        }
    }
}
