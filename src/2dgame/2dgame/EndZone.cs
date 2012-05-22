using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;

namespace _2dgame.Components.Gameplay
{
    class EndZone : EntityComponent
    {
        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<CollisionMsg>(OnCollision);

            base.OnOwnerSet();
        }

        void OnCollision(CollisionMsg message)
        {
            if (message.First.GetComponent<MainCharacter>() != null)
            {
                message.First.GetComponent<MainCharacter>().Dispose();
            }
            else if (message.Second.GetComponent<MainCharacter>() != null)
            {
                message.First.GetComponent<MainCharacter>().Dispose();
            }
        }
    }
}