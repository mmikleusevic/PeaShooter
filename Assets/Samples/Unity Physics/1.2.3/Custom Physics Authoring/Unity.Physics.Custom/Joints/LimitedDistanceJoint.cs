using static Unity.Physics.Math;

namespace Unity.Physics.Authoring
{
    public class LimitedDistanceJoint : BallAndSocketJoint
    {
        public float MinDistance;
        public float MaxDistance;
    }

    internal class LimitedDistanceJointBaker : JointBaker<LimitedDistanceJoint>
    {
        public override void Bake(LimitedDistanceJoint authoring)
        {
            authoring.UpdateAuto();

            PhysicsJoint physicsJoint = PhysicsJoint.CreateLimitedDistance(authoring.PositionLocal,
                authoring.PositionInConnectedEntity, new FloatRange(authoring.MinDistance, authoring.MaxDistance));
            physicsJoint.SetImpulseEventThresholdAllConstraints(authoring.MaxImpulse);

            PhysicsConstrainedBodyPair constraintBodyPair = GetConstrainedBodyPair(authoring);

            uint worldIndex = GetWorldIndexFromBaseJoint(authoring);
            CreateJointEntity(worldIndex, constraintBodyPair, physicsJoint);
        }
    }
}