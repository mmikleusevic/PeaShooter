using Unity.Entities;
using UnityEngine;

public class LoadNewWaveAuthoring : MonoBehaviour
{
    [SerializeField] private float loadTimerTarget;

    public class LoadNewWaveBaker : Baker<LoadNewWaveAuthoring>
    {
        public override void Bake(LoadNewWaveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new LoadNewWaveComponent
            {
                loadTimerTarget = authoring.loadTimerTarget
            });
        }
    }
}
