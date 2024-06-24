using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class FreezeRotationAuthoring : MonoBehaviour
{
    [SerializeField] private bool3 flags;

    class FreezeRotationBaker : Baker<FreezeRotationAuthoring>
    {
        public override void Bake(FreezeRotationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FreezeRotationComponent
            {
                flags = authoring.flags
            });
        }
    }
}
