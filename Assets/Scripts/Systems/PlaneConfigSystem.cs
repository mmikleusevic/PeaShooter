using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PlaneConfigSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (Config.Instance == null) return;

        state.Enabled = false;

        var configEntity = state.EntityManager.CreateEntity(typeof(PlaneConfigComponent));
        float planeSize = Config.Instance.GetPlaneSize();

        state.EntityManager.SetComponentData(configEntity, new PlaneConfigComponent { planeSize = planeSize });
    }
}
