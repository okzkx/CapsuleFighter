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

// 玩家福利，
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlayerWealSystem : SystemBase {
    BeginSimulationEntityCommandBufferSystem BeginSimulationEntityCommandBufferSystem;
    InitializationSystemGroup initializationSystemGroup;
    protected override void OnCreate() {
        base.OnCreate();
        BeginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        initializationSystemGroup = World.GetOrCreateSystem<InitializationSystemGroup>();

        Application.targetFrameRate = 60;
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

        // 赠送移动技能（Axis 输入）（物理）
        Entity moveAbilityEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(moveAbilityEntity, new Owner { Entity = playerEntity });
        EntityManager.AddComponent<AbilityCommon.Active>(moveAbilityEntity);
        EntityManager.AddComponent<PhysicsMoveAbility.Tag>(moveAbilityEntity);
        EntityManager.AddComponent<AxisInput>(moveAbilityEntity);
        EntityManager.AddComponentData(moveAbilityEntity, new PhysicsMoveAbility.Setting { Strength = 20 });
        EntityManager.SetComponentData(playerEntity, new Character.AxisAbility { Entity = moveAbilityEntity });

        // 赠送转向技能（鼠标位置输入）
        Entity rtmpEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(rtmpEntity, new RotateToMousePos.Ability.Spawn { Owner = playerEntity });

        BeginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        initializationSystemGroup.RemoveSystemFromUpdateList(this);
        World.DestroySystem(this);
    }
}