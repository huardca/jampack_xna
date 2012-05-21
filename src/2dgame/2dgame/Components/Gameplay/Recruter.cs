using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;

namespace _2dgame.Components.Gameplay
{
    class Recruter : EntityComponent
    {
        List<Recrutable> m_Followers = new List<Recrutable>();

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public void AddFollower(Recrutable recrute)
        {
            m_Followers.Add(recrute);
        }
    }
}
