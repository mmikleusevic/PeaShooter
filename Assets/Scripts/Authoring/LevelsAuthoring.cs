using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class LevelsAuthoring : MonoBehaviour
    {
        public List<byte> levelKeys;
        public List<uint> levelValues;

        public class LevelsBaker : Baker<LevelsAuthoring>
        {
            public override void Bake(LevelsAuthoring authoring)
            {
                BlobBuilder builder = new BlobBuilder(Allocator.Temp);
                ref LevelDataBlob blobData = ref builder.ConstructRoot<LevelDataBlob>();

                BlobBuilderArray<byte> keysArray = builder.Allocate(ref blobData.levels, authoring.levelKeys.Count);
                BlobBuilderArray<uint> valuesArray =
                    builder.Allocate(ref blobData.experience, authoring.levelValues.Count);

                for (int i = 0; i < authoring.levelKeys.Count; i++)
                {
                    keysArray[i] = authoring.levelKeys[i];
                    valuesArray[i] = authoring.levelValues[i];
                }

                BlobAssetReference<LevelDataBlob> blobAsset =
                    builder.CreateBlobAssetReference<LevelDataBlob>(Allocator.Persistent);
                builder.Dispose();

                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new LevelsComponent { levels = blobAsset });
            }
        }
    }
}