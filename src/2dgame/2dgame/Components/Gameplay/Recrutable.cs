using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Barebones.Framework;

namespace _2dgame.Components.Gameplay
{
    class Recrutable : EntityComponent
    {
        public Entity Target
        { get; set; }

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<CollisionMsg>(OnCollision);

            base.OnOwnerSet();
        }

        void OnCollision(CollisionMsg msg)
        {
            Entity target = msg.First == Owner ? msg.Second : msg.First;
            Recruter recruter = target.GetComponent<Recruter>();
            if (recruter == null)
                return;

            Target = recruter.Owner;
        }
    }
}
