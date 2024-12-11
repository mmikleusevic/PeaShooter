using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class UIBarOffsetAuthoring : MonoBehaviour
    {
        [SerializeField] private Vector3 uiBarOffset;

        private class UIBarOffsetBaker : Baker<UIBarOffsetAuthoring>
        {
            public override void Bake(UIBarOffsetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new UIBarOffsetComponent
                {
                    offset = authoring.uiBarOffset
                });
            }
        }
    }
}