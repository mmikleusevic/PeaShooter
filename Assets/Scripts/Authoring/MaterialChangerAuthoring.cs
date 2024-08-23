using Unity.Entities;
using UnityEngine;

public class MaterialChangerAuthoring : MonoBehaviour
{
    [SerializeField] private Material material;

    public class MaterialChangerBaker : Baker<MaterialChangerAuthoring>
    {
        public override void Bake(MaterialChangerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponentObject(entity, new MaterialChangerComponent
            {
                material = authoring.material
            });
        }
    }
}
