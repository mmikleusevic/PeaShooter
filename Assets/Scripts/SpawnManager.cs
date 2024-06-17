using Unity.Entities;
using UnityEngine;

public struct SpawnManager : IComponentData
{
    public Entity prefab;
    public Vector3 spawnPosition;
    public float nextSpawnTime;
    public float spawnRate;
}
