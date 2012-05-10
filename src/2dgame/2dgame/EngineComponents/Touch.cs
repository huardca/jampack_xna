using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using _2dgame.Components;
using Barebones.Dependencies;
using Meat.Rendering;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using _2dgame.Events;
using Barebones.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace _2dgame.EngineComponents
{
    class Touch : EngineComponent, Barebones.Framework.IUpdateable
    {
        GraphicsDevice m_Device;
        List<Selectable> m_Selectable = new List<Selectable>();
        Camera m_Camera;
        RawRenderer m_Renderer;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield return new Dependency<Selectable>(item => m_Selectable.Add(item), item => m_Selectable.Remove(item));
            yield return new Dependency<Camera>(item => m_Camera = item, item => m_Camera = null);
            yield return new Dependency<RawRenderer>(item => m_Renderer = item, item => m_Renderer = null);
        }

        protected override void OnOwnerSet()
        {
            Owner.Forum.RegisterListener<InitializeMessage>(OnInit);

            base.OnOwnerSet();
        }

        void OnInit(InitializeMessage msg)
        {
            m_Device = msg.Device;
        }

        public void Update(float dt)
        {
            TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
            if (!tc.IsConnected)
                return;

            List<TouchMsg> touches = new List<TouchMsg>();
            List<DragMsg> drags = new List<DragMsg>();

            FindTouchesAndSwipes(touches, drags);

            foreach (TouchMsg msg in touches)
            {
                foreach (Selectable sel in m_Selectable)
                {
                    if (sel.Contains(msg.WorldPosition))
                        sel.Owner.Forum.Fire<TouchMsg>(msg);
                }

                Owner.Forum.Fire<TouchMsg>(msg);
            }

            foreach (DragMsg msg in drags)
            {
                foreach (Selectable sel in m_Selectable)
                {
                    if (sel.Contains(msg.Start.WorldPosition))
                        sel.Owner.Forum.Fire<DragMsg>(msg);
                }

                Owner.Forum.Fire<DragMsg>(msg);
            }

        }

        private void FindTouchesAndSwipes(List<TouchMsg> touches, List<DragMsg> drags)
        {
            // Process touch events
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Invalid)
                    continue;

                if (tl.State == TouchLocationState.Pressed)
                {
                    touches.Add(ToTouchMsg(tl));
                }
                else if (tl.State == TouchLocationState.Moved)
                {
                    TouchLocation prev;
                    if (!tl.TryGetPreviousLocation(out prev))
                        continue;

                    drags.Add(new DragMsg(ToTouchMsg(prev), ToTouchMsg(tl)));
                }
            }
        }

        private TouchMsg ToTouchMsg(TouchLocation loc)
        {
            return new TouchMsg()
            {
                ScreenPosition = loc.Position,
                WorldPosition = ScreenToWorld(loc.Position)
            };
        }

        private Vector3 ScreenToWorld(Vector2 screen)
        {
            return m_Device.Viewport.Unproject(new Vector3(screen, 0.5f), m_Renderer.Projection, m_Camera.GetView(), Matrix.Identity);
        }
    }
}
