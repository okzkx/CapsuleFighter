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

    }
}


public static class PhysicsMoveAbility {
    public struct Tag : IComponentData { }
    public struct Setting : IComponentData {
        public float Speed;
    }
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class UpdateSystem : SystemBase {
        protected override void OnUpdate() {
            float deltaTime = Time.DeltaTime;

            var pvs = GetComponentDataFromEntity<PhysicsVelocity>();
            var pms = GetComponentDataFromEntity<PhysicsMass>(true);

            Entities
                .WithReadOnly(pms)
                .WithAll<Tag,AbilityCommon.Active>()
                .ForEach((in Owner owner, in AxisInput control,in Setting setting) => {
                    PhysicsVelocity velocity = pvs[owner.Entity];
                    PhysicsMass physicsMass = pms[owner.Entity];
                    velocity.ApplyLinearImpulse(physicsMass, setting.Speed * new float3(control.Value.x, 0, control.Value.y));
                    pvs[owner.Entity] = velocity;
                }).Schedule();
        }
    }
}

