using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using _2dgame.Events;

namespace _2dgame.Components
{
    class FollowFinger : EntityComponent
    {
        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<DragMsg>(HandleDrag);

            base.OnOwnerSet();
        }

        void HandleDrag(DragMsg msg)
        {
            Matrix current = Owner.Transform;
            current.Translation += new Vector3(msg.WorldDelta.X, msg.WorldDelta.Y, 0);
            Owner.Transform = current;

            Owner.FireDown(new TransformForcedMsg());
        }
    }
}
