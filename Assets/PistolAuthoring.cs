using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PistolAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<Pistol.Tag>(entity);
    }
}

public static class Pistol {
    public struct Tag : IComponentData { }

    // Tag Entry
    public static class InventoryItem {

        public struct Tag : IComponentData { }

        public class HandlePistolAbilitySpawenrSystem : SpawnerSystemBase {
            protected override void OnCreate() {
                base.OnCreate();
            }

            protected override void OnUpdate(EntityCommandBuffer.ParallelWriter buffer) {
                BufferFromEntity<AbilityCommon.Entry> abilityEntryFromEntity = GetBufferFromEntity<AbilityCommon.Entry>(true);
                //GameObjectConversionSystem gameObjectConversionSystem;
                //gameObjectConversionSystem.0
                Entities
                    .WithReadOnly(abilityEntryFromEntity)
                    .WithAll<Tag>()
                    .ForEach((Entity entity, int entityInQueryIndex) => {
                        if (!abilityEntryFromEntity.HasComponent(entity)) {
                            Entity abilityEntity = buffer.CreateEntity(entityInQueryIndex);
                            buffer.AddComponent<HandlePistolAbility>(entityInQueryIndex, abilityEntity);
                            buffer.AddComponent<AbilityCommon.Tag>(entityInQueryIndex, abilityEntity);
                            buffer.AddComponent<AbilityCommon.Idle>(entityInQueryIndex, abilityEntity);
                            buffer.AddComponent(entityInQueryIndex, abilityEntity, new Owner { Entity = entity });

                            buffer.AddBuffer<AbilityCommon.Entry>(entityInQueryIndex, entity);
                            buffer.AppendToBuffer(entityInQueryIndex, entity, new AbilityCommon.Entry { Entity = abilityEntity });
                        }
                    }).ScheduleParallel();
            }
        }


        public static class Ability {
            public class ActiveSystem : PossProcessingSystemBase {
                protected override void OnUpdate(EntityCommandBuffer buffer) {
                    Entity pistol = GetSingleton<PrefabRigister>().Pistol;
                    var prefabFromEntity = GetComponentDataFromEntity<PrefabRigister>();
                    Entities
                        .WithoutBurst()
                       .WithAll<AbilityCommon.Idle>()
                       .WithNone<AbilityCommon.Active>()
                       .ForEach((Entity entity) => {
                           var ownerEntity = entity;
                           while (!EntityManager.HasComponent<Charactor.Input>(ownerEntity)) {
                               ownerEntity = EntityManager.GetComponentData<Owner>(ownerEntity).Entity;
                           }
                           var input = EntityManager.GetComponentData<Charactor.Input>(ownerEntity);
                           if (input.Alpha1) {
                               buffer.RemoveComponent<AbilityCommon.Idle>(entity);
                               buffer.AddComponent<AbilityCommon.Active>(entity);
                               buffer.Instantiate(pistol);
                           }
                       }).Run();
                }
            }
        }
        // 令角色手持一把手枪
        public struct HandlePistolAbility : IComponentData { }
    }
}

