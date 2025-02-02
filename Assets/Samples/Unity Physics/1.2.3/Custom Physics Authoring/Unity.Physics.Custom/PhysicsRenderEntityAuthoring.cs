using Unity.Entities;
using Unity.Physics.GraphicsIntegration;
using UnityEngine;

namespace Unity.Physics.Authoring
{
    [AddComponentMenu("Entities/Physics/Physics Render Entity")]
    [DisallowMultipleComponent]
    public sealed class PhysicsRenderEntityAuthoring : MonoBehaviour
    {
        [Tooltip(
            "Specifies an Entity in a different branch of the hierarchy that holds the graphical representation of this PhysicsShape.")]
        public GameObject RenderEntity;
    }

    internal class PhysicsRenderEntityBaker : Baker<PhysicsRenderEntityAuthoring>
    {
        public override void Bake(PhysicsRenderEntityAuthoring authoring)
        {
            PhysicsRenderEntity renderEntity = new PhysicsRenderEntity
                { Entity = GetEntity(authoring.RenderEntity, TransformUsageFlags.Dynamic) };
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, renderEntity);
        }
    }
}