using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class PhysicsMoveControlAuthoring : MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        dstManager.AddComponent<PhysicsMoveAbility.Tag>(entity);
        dstManager.AddComponent<PhysicsMoveAbility.Input>(entity);
        dstManager.AddComponent<RotationEulerXYZ>(entity);
        dstManager.AddComponentData(entity, new PhysicsMoveAbility.Setting {
            Speed = 5
        });
    }
}

public static class PhysicsMoveAbility {
    public struct Tag : IComponentData { }
    public struct Input : IComponentData {
        public float2 Value;
    }
    public struct Setting : IComponentData {
        public float Speed;
    }
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class UpdateSystem : SystemBase {
        protected override void OnUpdate() {
            float deltaTime = Time.DeltaTime;

            float speed = 10f;

            //Entities
            //    .WithAll<Tag>()
            //    .ForEach((ref Translation translation, ref PhysicsVelocity velocity, in PhysicsMass physicsMass, in Input input) => {
            //        velocity.ApplyLinearImpulse(physicsMass, speed * new float3(input.Value.x, 0, input.Value.y));
            //    }).ScheduleParallel();
            Entities
                .WithAll<Tag>()
                .ForEach((ref Translation translation, ref PhysicsVelocity velocity, in PhysicsMass physicsMass, in Charactor.Input input) => {
                    velocity.ApplyLinearImpulse(physicsMass, speed * new float3(input.Axis.x, 0, input.Axis.y));
                }).ScheduleParallel();
        }
    }

    [UpdateAfter(typeof(ExportPhysicsWorld))]
    public class FreezeRotationSystem : SystemBase {
        protected override void OnUpdate() {
            Entities
               .WithAll<Tag>()
               .ForEach((ref RotationEulerXYZ euler) => {
                   var angle = euler.Value;
                   euler.Value = angle;
               }).ScheduleParallel();
        }
    }
}

