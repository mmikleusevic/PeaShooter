using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Physics.Authoring
{
    [BakingType]
    public struct JointEntityBaking : IComponentData
    {
        public Entity Entity;
    }

    public class BallAndSocketJoint : BaseJoint
    {
        // Editor only settings
        [HideInInspector] public bool EditPivots;

        [Tooltip("If checked, PositionLocal will snap to match PositionInConnectedEntity")]
        public bool AutoSetConnected = true;

        public float3 PositionLocal;
        public float3 PositionInConnectedEntity;

        public virtual void UpdateAuto()
        {
            if (AutoSetConnected)
            {
                RigidTransform bFromA = math.mul(math.inverse(worldFromB), worldFromA);
                PositionInConnectedEntity = math.transform(bFromA, PositionLocal);
            }
        }
    }

    public abstract class JointBaker<T> : Baker<T> where T : BaseJoint
    {
        protected PhysicsConstrainedBodyPair GetConstrainedBodyPair(BaseJoint authoring)
        {
            return new PhysicsConstrainedBodyPair(
                GetEntity(TransformUsageFlags.Dynamic),
                authoring.ConnectedBody == null
                    ? Entity.Null
                    : GetEntity(authoring.ConnectedBody, TransformUsageFlags.Dynamic),
                authoring.EnableCollision
            );
        }

        public Entity CreateJointEntity(uint worldIndex, PhysicsConstrainedBodyPair constrainedBodyPair,
            PhysicsJoint joint)
        {
            using (NativeArray<PhysicsJoint> joints = new NativeArray<PhysicsJoint>(1, Allocator.Temp) { [0] = joint })
            using (NativeList<Entity> jointEntities = new NativeList<Entity>(1, Allocator.Temp))
            {
                CreateJointEntities(worldIndex, constrainedBodyPair, joints, jointEntities);
                return jointEntities[0];
            }
        }

        public uint GetWorldIndex(Component c)
        {
            uint worldIndex = 0;
            if (c)
            {
                PhysicsBodyAuthoring physicsBody = GetComponent<PhysicsBodyAuthoring>(c);
                if (physicsBody != null) worldIndex = physicsBody.WorldIndex;
            }

            return worldIndex;
        }

        public uint GetWorldIndexFromBaseJoint(BaseJoint authoring)
        {
            PhysicsBodyAuthoring physicsBody = GetComponent<PhysicsBodyAuthoring>(authoring);
            uint worldIndex = physicsBody.WorldIndex;
            if (authoring.ConnectedBody == null) return worldIndex;

            PhysicsBodyAuthoring connectedBody = GetComponent<PhysicsBodyAuthoring>(authoring.ConnectedBody);
            if (connectedBody != null) Assert.AreEqual(worldIndex, connectedBody.WorldIndex);

            return worldIndex;
        }

        public void CreateJointEntities(uint worldIndex, PhysicsConstrainedBodyPair constrainedBodyPair,
            NativeArray<PhysicsJoint> joints, NativeList<Entity> newJointEntities)
        {
            if (!joints.IsCreated || joints.Length == 0)
                return;

            if (newJointEntities.IsCreated)
                newJointEntities.Clear();
            else
                newJointEntities = new NativeList<Entity>(joints.Length, Allocator.Temp);

            // create all new joints
            bool multipleJoints = joints.Length > 1;

            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            for (int i = 0; i < joints.Length; ++i)
            {
                Entity jointEntity = CreateAdditionalEntity(TransformUsageFlags.Dynamic);
                AddSharedComponent(jointEntity, new PhysicsWorldIndex(worldIndex));

                AddComponent(jointEntity, constrainedBodyPair);
                AddComponent(jointEntity, joints[i]);

                newJointEntities.Add(jointEntity);

                if (GetComponent<ModifyJointLimitsAuthoring>() != null)
                {
                    AddComponent(jointEntity, new JointEntityBaking
                    {
                        Entity = entity
                    });
                    AddSharedComponentManaged(jointEntity, new ModifyJointLimits());
                }
            }

            if (multipleJoints)
                // set companion buffers for new joints
                for (int i = 0; i < joints.Length; ++i)
                {
                    DynamicBuffer<PhysicsJointCompanion> companions =
                        AddBuffer<PhysicsJointCompanion>(newJointEntities[i]);
                    for (int j = 0; j < joints.Length; ++j)
                    {
                        if (i == j)
                            continue;
                        companions.Add(new PhysicsJointCompanion { JointEntity = newJointEntities[j] });
                    }
                }
        }
    }

    internal class BallAndSocketJointBaker : JointBaker<BallAndSocketJoint>
    {
        public override void Bake(BallAndSocketJoint authoring)
        {
            authoring.UpdateAuto();
            PhysicsJoint physicsJoint =
                PhysicsJoint.CreateBallAndSocket(authoring.PositionLocal, authoring.PositionInConnectedEntity);
            physicsJoint.SetImpulseEventThresholdAllConstraints(authoring.MaxImpulse);

            PhysicsConstrainedBodyPair constraintBodyPair = GetConstrainedBodyPair(authoring);

            uint worldIndex = GetWorldIndexFromBaseJoint(authoring);
            CreateJointEntity(worldIndex, constraintBodyPair, physicsJoint);
        }
    }
}