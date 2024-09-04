#if UNITY_EDITOR
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEditor;
using UnityEngine;

public class SceneLoaderAuthoring : MonoBehaviour
{
    public SceneAsset Scene;

    class SceneLoaderBaker : Baker<SceneLoaderAuthoring>
    {
        public override void Bake(SceneLoaderAuthoring authoring)
        {
            EntitySceneReference reference = new EntitySceneReference(authoring.Scene);
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SceneLoader
            {
                SceneReference = reference
            });
        }
    }
}
#endif