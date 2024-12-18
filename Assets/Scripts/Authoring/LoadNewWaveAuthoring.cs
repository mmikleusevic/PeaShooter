using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
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

                AddComponent(entity, new DebugNameComponent
                {
                    entityName = "LoadNewWave"
                });
            }
        }
    }
}