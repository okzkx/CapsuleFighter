﻿using JetBrains.Annotations;
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

// 玩家福利，
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
        Entity playerEntity = GetSingletonEntity<Player.Tag>();
        if (!EntityManager.HasComponent<Inventory.Entry>(playerEntity)) {
            return;
        }

        // 赠送一把枪
        Entity InventoryEntity = EntityManager.GetComponentData<Inventory.Entry>(playerEntity).Entity;
        Entity pistolInventoryEntity = EntityManager.CreateEntity();
        EntityManager.AddComponent<Inventory.Item.Tag>(pistolInventoryEntity);
        EntityManager.AddComponentData(pistolInventoryEntity, new Owner { Entity = InventoryEntity });
        EntityManager.AddComponent<Pistol.InventoryItem.Tag>(pistolInventoryEntity);
        var itemEntry = EntityManager.GetBuffer<Inventory.Item.Entry>(InventoryEntity);
        itemEntry.Add(new Inventory.Item.Entry { Entity = pistolInventoryEntity });

        //var buffer = BeginSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        //var playerTagFormEntity = GetComponentDataFromEntity<Player.Tag>();
        //buffer.AppendToBuffer(entity, new Inventory.Item.Entry { Entity = pistolInventoryEntity });
        //Entities
        //    .WithReadOnly(playerTagFormEntity)
        //    .WithAll<Inventory.Tag>()
        //    .ForEach((Entity entity, ref Owner owner) => {
        //        if (playerTagFormEntity.HasComponent(owner.Entity)) {
        //            Entity pistolInventoryEntity = buffer.CreateEntity();
        //            buffer.AddComponent<Inventory.Item.Tag>(pistolInventoryEntity);
        //            buffer.AddComponent(pistolInventoryEntity, new Owner { Entity = entity });
        //            buffer.AddComponent<Pistol.InventoryItem.Tag>(pistolInventoryEntity);

        //            buffer.AppendToBuffer(entity, new Inventory.Item.Entry { Entity = pistolInventoryEntity });
        //        }

        //    }).Schedule();
        //Dependency.Complete();

        // 赠送移动技能（物理）
        Entity moveAbilityEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(moveAbilityEntity, new Owner { Entity = playerEntity });
        EntityManager.AddComponent<AbilityCommon.Active>(moveAbilityEntity);
        EntityManager.AddComponent<PhysicsMoveAbility.Tag>(moveAbilityEntity);
        EntityManager.AddComponent<AxisInput>(moveAbilityEntity);
        EntityManager.AddComponentData(moveAbilityEntity, new PhysicsMoveAbility.Setting { Speed = 10 });
        EntityManager.SetComponentData(playerEntity, new Character.AxisAbility { Entity = moveAbilityEntity });

        BeginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        initializationSystemGroup.RemoveSystemFromUpdateList(this);
        World.DestroySystem(this);
    }
}