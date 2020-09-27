using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveControlAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<MoveAbility.Tag>(entity);
        dstManager.AddComponent<MoveAbility.Input>(entity);
        dstManager.AddComponentData(entity, new MoveAbility.Setting {
            Speed = 5
        });
    }
}

public class MoveAbility {
    public struct Tag : IComponentData { }
    public struct Input : IComponentData {
        public float2 Value;
    }
    public struct Setting : IComponentData {
        public float Speed;
    }
    public class Update : SystemBase {
        protected override void OnUpdate() {
            float deltaTime = Time.DeltaTime;

            float speed = 5f;

            Entities
                .WithAll<Tag>()
                .ForEach((ref Translation translation, ref Input input) => {
                    translation.Value.x += input.Value.x * speed * deltaTime;
                    translation.Value.z += input.Value.y * speed * deltaTime;
                }).ScheduleParallel();
        }
    }
}
