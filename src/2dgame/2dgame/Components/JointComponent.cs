using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Barebones.Components;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics;

namespace _2dgame.Components
{
    class JointComponent : EntityComponent
    {
        Joint m_Joint;

        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public JointComponent(Joint joint)
        {
            m_Joint = joint;
        }

        public void DisposePhysics(World world)
        {
            world.RemoveJoint(m_Joint);
        }
    }
}
