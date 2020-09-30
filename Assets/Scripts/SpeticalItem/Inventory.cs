
using Unity.Entities;
using Unity.Rendering;
public struct Owner : IComponentData {
    public Entity Entity;
}

public static class Inventory {
    public struct Tag : IComponentData { }

    public struct Spawner : IComponentData { }

    public struct Entry : IComponentData {
        public Entity Entity;
    }

    public static class Item {
        public struct Tag : IComponentData { }

        public struct Spawner : IComponentData { }

        public struct Entry : IBufferElementData {
            public Entity Entity;
        }
    }

    public class SpawnerSystem : SystemBase {
        BeginSimulationEntityCommandBufferSystem BeginSimulationEntityCommandBufferSystem;
        protected override void OnCreate() {
            base.OnCreate();
            BeginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var buffer = BeginSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Entities
                .WithAll<Spawner>()
                .ForEach((Entity entity) => {
                    Entity inventroyEntity = buffer.CreateEntity();
                    buffer.AddComponent<Tag>(inventroyEntity);
                    buffer.AddComponent(inventroyEntity, new Owner { Entity = entity });
                    buffer.AddBuffer<Item.Entry>(inventroyEntity);

                    buffer.RemoveComponent<Spawner>(entity);
                    buffer.AddComponent(entity, new Entry { Entity = inventroyEntity });
                }).Schedule();

            BeginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}