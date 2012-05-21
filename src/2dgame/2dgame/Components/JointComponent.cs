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
        FixedAngleJoint m_Joint;

        public float TargetAngle
        { 
            get { return m_Joint.TargetAngle; }
            set { m_Joint.TargetAngle = value; }
        }
        public override IEnumerable<Barebones.Dependencies.IDependency> GetDependencies()
        {
            yield break;
        }

        public JointComponent(FixedAngleJoint joint)
        {
            m_Joint = joint;
        }

        public void DisposePhysics(World world)
        {
            world.RemoveJoint(m_Joint);
        }
    }
}
