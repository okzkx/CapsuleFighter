using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CharacterAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<Charactor.Tag>(entity);
        dstManager.AddComponent<Charactor.Input>(entity);
        dstManager.AddComponent<Inventory.Spawner>(entity);
        dstManager.AddBuffer<AbilityCommon.Entry>(entity);
    }
}

// Tag Inventory.Entry Ability.Entry
public static class Charactor {
    public struct Tag : IComponentData { }
    public struct Input : IComponentData {
        public float2 Axis;
        public bool Alpha1;
    }

    // Update Charactor AbilityEntry
    public class UpdateAbilityEntrySystem : SystemBase {
        protected override void OnUpdate() {
            var ieFromEntity = GetBufferFromEntity<Inventory.Entry>(true);
            var iieFromEntity = GetBufferFromEntity<Inventory.Item.Entry>(true);
            var aeFromEntity = GetBufferFromEntity<AbilityCommon.Entry>(false);
            Entities
                .WithReadOnly(ieFromEntity)
                .WithReadOnly(iieFromEntity)
                .WithAll<Tag>()
                .ForEach((Entity entity) => {
                    var entries = aeFromEntity[entity];
                    entries.Clear();

                    if (!ieFromEntity.HasComponent(entity)) {
                        return;
                    }

                    var ies = ieFromEntity[entity];
                    for (int i = 0; i < ies.Length; i++) {
                        var ie = ies[i].Entity;
                        if (!iieFromEntity.HasComponent(ie)) continue;
                        var iies = iieFromEntity[ie];
                        for (int j = 0; j < iies.Length; j++) {
                            var iie = iies[i].Entity;
                            if (!aeFromEntity.HasComponent(iie)) continue;
                            var aes = aeFromEntity[iie];
                            for (int k = 0; k < aes.Length; k++) {
                                var ae = aes[i].Entity;
                                entries.Add(new AbilityCommon.Entry { Entity = ae });
                            }
                        }
                    }

                }).Schedule();
        }
    }
}

