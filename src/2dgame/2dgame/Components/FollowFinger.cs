using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;

namespace _2dgame.Components
{
    class FollowFinger : EntityComponent, Barebones.Framework.IUpdateable
    {
        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public void Update(float dt)
        {
            TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
            if (!tc.IsConnected)
                return;

            Vector3 worldTranslation = Owner.GetWorldTranslation();
            Vector2 delta = Vector2.Zero;

            // Process touch events
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State != TouchLocationState.Moved)
                    continue;

                TouchLocation prev;
                if (!tl.TryGetPreviousLocation(out prev))
                    continue;

                delta += tl.Position - prev.Position;
            }

            Matrix current = Owner.Transform;
            current.Translation += new Vector3(delta.X, -delta.Y, 0);
            Owner.Transform = current;
        }
    }
}
