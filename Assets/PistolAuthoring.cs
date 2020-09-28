using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PistolAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<Pistol.Tag>(entity);
    }
}

public static class Pistol {
    public struct Tag : IComponentData { }
    public struct Spawn : IComponentData {
        public Entity Parent;
        public Entity Owner;
    }

    public class SpawenrSystem : SpawnerSystemBase {
        protected override void OnUpdate(EntityCommandBuffer.ParallelWriter buffer) {
            Entity pistolPrefab = GetSingleton<PrefabRigister>().Pistol;

            // Spawn
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entity, in Spawn spawn) => {
                    var pistol = EntityManager.Instantiate(pistolPrefab);
                    EntityManager.AddComponentData(pistol, new Tag { });
                    EntityManager.AddComponentData(pistol, new Owner { Entity = spawn.Owner });
                    EntityManager.AddComponentData(pistol, new Parent { Value = spawn.Parent });
                    EntityManager.AddComponentData(pistol, new LocalToParent { });

                    var ownerItemBuffer = EntityManager.GetBuffer<Item.Entry>(spawn.Owner);
                    ownerItemBuffer.Add(new Item.Entry { Entity = pistol });

                    EntityManager.DestroyEntity(entity);

                    Entity abilitySpawnEntity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(abilitySpawnEntity, new Ability.Spawn { Owner = pistol });
                }).Run();

            // TODO: Destory 
        }
    }

    public static class Ability {
        public struct Tag : IComponentData { }

        public struct Spawn : IComponentData {
            public Entity Owner;
        }

        public class SpawnSystem : SystemBase {
            protected override void OnUpdate() {
                Entities
                    .WithoutBurst()
                    .WithStructuralChanges()
                    .ForEach((Entity entity, in Spawn spawn) => {
                        var abilityEntity = EntityManager.CreateEntity();
                        EntityManager.AddComponent<AbilityCommon.Idle>(abilityEntity);
                        EntityManager.AddComponent<Tag>(abilityEntity);
                        EntityManager.AddComponentData(abilityEntity, new Owner { Entity = spawn.Owner });
                        EntityManager.AddComponent<BoolInput>(abilityEntity);

                        Entity character = SystemUtil.GetCharacter(EntityManager, abilityEntity);
                        EntityManager.SetComponentData(character, new Character.LeftPressAbility { Entity = abilityEntity });

                        EntityManager.DestroyEntity(entity);
                    }).Run();
            }
        }

        public class UpdateSystem : SystemBase {
            protected override void OnUpdate() {
                Entities
                    .WithoutBurst()
                    .WithStructuralChanges()
                    .WithAll<Tag, AbilityCommon.Idle>()
                    .ForEach((Entity entity, in BoolInput boolInput) => {
                        if (boolInput.Value) {
                            Debug.Log("fire");
                        }

                        EntityManager.RemoveComponent<AbilityCommon.Idle>(entity);
                        EntityManager.AddComponent<AbilityCommon.Coldown>(entity);
                    }).Run();

                Entities
                    .WithoutBurst()
                    .WithStructuralChanges()
                    .WithAll<Tag, AbilityCommon.Coldown>()
                    .ForEach((Entity entity) => {

                        EntityManager.RemoveComponent<AbilityCommon.Coldown>(entity);
                        EntityManager.AddComponent<AbilityCommon.Idle>(entity);
                    }).Run();

            }
        }
    }

    // Tag Entry
    public static class InventoryItem {

        public struct Tag : IComponentData { }


        public static class Ability {
            // 令角色手持一把手枪
            public struct HandlePistol : IComponentData { }

            public class SpawenrSystem : SpawnerSystemBase {
                protected override void OnUpdate(EntityCommandBuffer.ParallelWriter buffer) {
                    BufferFromEntity<AbilityCommon.Entry> abilityEntryFromEntity = GetBufferFromEntity<AbilityCommon.Entry>(true);
                    Entities
                        .WithReadOnly(abilityEntryFromEntity)
                        .WithAll<Tag>()
                        .ForEach((Entity entity, int entityInQueryIndex) => {
                            if (!abilityEntryFromEntity.HasComponent(entity)) {
                                Entity abilityEntity = buffer.CreateEntity(entityInQueryIndex);
                                buffer.AddComponent<HandlePistol>(entityInQueryIndex, abilityEntity);
                                buffer.AddComponent<AbilityCommon.Tag>(entityInQueryIndex, abilityEntity);
                                buffer.AddComponent<AbilityCommon.Idle>(entityInQueryIndex, abilityEntity);
                                buffer.AddComponent<BoolInput>(entityInQueryIndex, abilityEntity);
                                buffer.AddComponent<ParentInput>(entityInQueryIndex, abilityEntity);
                                buffer.AddBuffer<Item.Entry>(entityInQueryIndex, abilityEntity);
                                buffer.AddComponent(entityInQueryIndex, abilityEntity, new Owner { Entity = entity });

                                buffer.AddBuffer<AbilityCommon.Entry>(entityInQueryIndex, entity);
                                buffer.AppendToBuffer(entityInQueryIndex, entity, new AbilityCommon.Entry { Entity = abilityEntity });
                            }
                        }).ScheduleParallel();
                }
            }

            public class UpdateSystem : PossProcessingSystemBase {
                protected override void OnUpdate(EntityCommandBuffer buffer) {
                    Entity pistolPrefab = GetSingleton<PrefabRigister>().Pistol;
                    Entities
                       .WithoutBurst()
                       .WithAll<AbilityCommon.Idle>()
                       .WithNone<AbilityCommon.Active>()
                       .ForEach((Entity entity, in BoolInput boolInput, in ParentInput parentInput) => {
                           if (boolInput.Value) {
                               buffer.RemoveComponent<AbilityCommon.Idle>(entity);
                               buffer.AddComponent<AbilityCommon.Active>(entity);
                               var spawnerEntity = buffer.CreateEntity();
                               buffer.AddComponent(spawnerEntity, new Spawn { Owner = entity, Parent = parentInput.Entity });
                           }
                       }).Run();

                    Entities
                       .WithoutBurst()
                       .WithAll<AbilityCommon.Active>()
                       .WithNone<AbilityCommon.Coldown>()
                       .ForEach((Entity entity, in BoolInput boolInput) => {
                           if (boolInput.Value) {
                               buffer.RemoveComponent<AbilityCommon.Active>(entity);
                               buffer.AddComponent<AbilityCommon.Coldown>(entity);
                               var itemEntry = EntityManager.GetBuffer<Item.Entry>(entity);
                               buffer.DestroyEntity(itemEntry[0].Entity);
                               itemEntry.Clear();
                           }
                       }).Run();

                    Entities
                       .WithoutBurst()
                       .WithAll<AbilityCommon.Coldown>()
                       .ForEach((Entity entity, in BoolInput boolInput) => {
                           buffer.RemoveComponent<AbilityCommon.Coldown>(entity);
                           buffer.AddComponent<AbilityCommon.Idle>(entity);
                       }).Run();
                }
            }
        }
    }
}

