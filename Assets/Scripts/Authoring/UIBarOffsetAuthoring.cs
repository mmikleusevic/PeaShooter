using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class UIBarOffsetAuthoring : MonoBehaviour
    {
        [SerializeField] private Vector3 uiBarOffset;

        private class UIBarOffsetAuthoringBaker : Baker<UIBarOffsetAuthoring>
        {
            public override void Bake(UIBarOffsetAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new UIBarOffset
                {
                    offset = authoring.uiBarOffset
                });
            }
        }
    }
}