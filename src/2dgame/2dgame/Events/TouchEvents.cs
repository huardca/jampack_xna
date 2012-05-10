using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _2dgame.Events
{
    class TouchMsg
    {
        public Vector2 ScreenPosition
        { get; set; }

        public Vector3 WorldPosition
        { get; set; }
    }

    class DragMsg
    {
        public TouchMsg Start
        { get; set; }

        public TouchMsg End
        { get; set; }

        public Vector2 ScreenDelta
        { get; set; }

        public Vector3 WorldDelta
        { get; set; }

        public DragMsg(TouchMsg start, TouchMsg end)
        {
            Start = start;
            End = end;

            ScreenDelta = end.ScreenPosition - start.ScreenPosition;
            WorldDelta = end.WorldPosition - start.WorldPosition;
        }
    }
}
