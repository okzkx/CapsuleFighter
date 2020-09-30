using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class CharacterAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public Transform Hand;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<Character.Tag>(entity);
        dstManager.AddComponent<Character.Input>(entity);
        dstManager.AddComponent<RotationEulerXYZ>(entity);
        Entity hand = conversionSystem.GetPrimaryEntity(Hand);
        dstManager.AddComponentData(entity, new Character.Hand { Entity = hand });
        //dstManager.AddBuffer<AbilityCommon.Entry>(entity);

        dstManager.AddComponent<Character.AxisAbility>(entity);
        dstManager.AddComponent<Character.LeftPressAbility>(entity);
        dstManager.AddComponent<Character.RightPressAbility>(entity);
        dstManager.AddComponent<Character.MousePosAbility>(entity);
        dstManager.AddComponent<Inventory.Spawner>(entity);
    }
}

// Tag Inventory.Entry Ability.Entry
public static class Character {

    public struct Tag : IComponentData { }

    public struct Input : IComponentData {
        public float2 Axis;
        public bool Alpha1;
        public bool LeftPress;
        public float3 MousePos;
    }

    public struct AxisAbility : IComponentData {
        public Entity Entity;
    }

    public struct Hand : IComponentData {
        public Entity Entity;
    }

    public struct LeftPressAbility : IComponentData {
        public Entity Entity;
    }

    public struct RightPressAbility : IComponentData {
        public Entity Entity;
    }

    public struct MousePosAbility : IComponentData {
        public Entity Entity;
    }

    public class ActiveAbilitySystem : SystemBase {
        protected override void OnUpdate() {

            // AxisInput 向移动技能输入
            Entities
                .WithoutBurst()
                .WithAll<Character.Tag>()
                .ForEach((in AxisAbility moveAbility, in Input input) => {
                    if (EntityManager.HasComponent<AxisInput>(moveAbility.Entity)) {
                        EntityManager.SetComponentData(moveAbility.Entity, new AxisInput { Value = input.Axis });
                    }
                }).Run();

            // BoolInput 向背包第一个物品的技能输入
            var boolInputs = GetComponentDataFromEntity<BoolInput>();
            Entities
                .WithoutBurst()
                .WithAll<Character.Tag>()
                .ForEach((in Inventory.Entry inventroy, in Character.Input input, in Hand hand) => {
                    var itemBuffer = EntityManager.GetBuffer<Inventory.Item.Entry>(inventroy.Entity);
                    if (itemBuffer.Length >= 1) {
                        Entity item = itemBuffer[0].Entity;
                        var abilityBuffer = EntityManager.GetBuffer<AbilityCommon.Entry>(item);
                        if (abilityBuffer.Length >= 1) {
                            EntityManager.SetComponentData(abilityBuffer[0].Entity, new BoolInput { Value = input.Alpha1 });
                            EntityManager.SetComponentData(abilityBuffer[0].Entity, new ParentInput { Entity = hand.Entity });
                        }
                    }
                }).Run();

            // 左键技能输入
            Entities
                .WithoutBurst()
                .WithAll<Character.Tag>()
                .ForEach((in LeftPressAbility ability, in Input input) => {
                    if (EntityManager.HasComponent<BoolInput>(ability.Entity)) {
                        EntityManager.SetComponentData(ability.Entity, new BoolInput { Value = input.LeftPress });
                    }
                }).Run();

            // 鼠标位置输入
            Entities
                .WithoutBurst()
                .WithAll<Character.Tag>()
                .ForEach((in MousePosAbility ability, in Input input) => {
                    if (EntityManager.HasComponent<MousePosInput>(ability.Entity)) {
                        EntityManager.SetComponentData(ability.Entity, new MousePosInput { Value = input.MousePos });
                    }
                }).Run();
        }
    }
}

