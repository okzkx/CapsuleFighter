using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<Player.Tag>(entity);
    }
}

public static class Player {
    public struct Tag : IComponentData { }
}

// 玩家福利，赠送一把枪
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerWealSystem : SystemBase {
    BeginSimulationEntityCommandBufferSystem BeginSimulationEntityCommandBufferSystem;
    InitializationSystemGroup initializationSystemGroup;
    protected override void OnCreate() {
        base.OnCreate();
        BeginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        initializationSystemGroup = World.GetOrCreateSystem<InitializationSystemGroup>();
    }

    protected override void OnUpdate() {
        var buffer = BeginSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var playerTagFormEntity = GetComponentDataFromEntity<Player.Tag>();
        Entities
            .WithReadOnly(playerTagFormEntity)
            .WithAll<Inventory.Tag>()
            .ForEach((Entity entity, ref Owner owner) => {
                if (playerTagFormEntity.HasComponent(owner.Entity)) {
                    Entity pistolEntity = buffer.CreateEntity();
                    buffer.AddComponent<Inventory.Item.Tag>(pistolEntity);
                    buffer.AddComponent(pistolEntity, new Owner { Entity = entity });
                    buffer.AddComponent<Pistol.InventoryItem.Tag>(pistolEntity);

                    buffer.AppendToBuffer(entity, new Inventory.Item.Entry { Entity = pistolEntity });
                }

            }).Schedule();
        BeginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        initializationSystemGroup.RemoveSystemFromUpdateList(this);
        World.DestroySystem(this);
    }
}