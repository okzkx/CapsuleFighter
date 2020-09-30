using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

public static class PhysicsMoveAbility {
    public struct Tag : IComponentData { }
    public struct Setting : IComponentData {
        public float Strength;
    }
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class UpdateSystem : SystemBase {
        protected override void OnUpdate() {
            float deltaTime = Time.DeltaTime;

            var pvs = GetComponentDataFromEntity<PhysicsVelocity>();
            var pms = GetComponentDataFromEntity<PhysicsMass>(true);

            Entities
                .WithReadOnly(pms)
                .WithAll<Tag, AbilityCommon.Active>()
                .ForEach((in Owner owner, in AxisInput input, in Setting setting) => {
                    float3 rawDirection = new float3(input.Value.x, 0, input.Value.y);
                    if (rawDirection.Equals(0)) {
                        return;
                    }
                    float3 direction = math.normalize(rawDirection);
                    PhysicsVelocity velocity = pvs[owner.Entity];
                    PhysicsMass physicsMass = pms[owner.Entity];
                    velocity.ApplyLinearImpulse(physicsMass, setting.Strength * direction);
                    pvs[owner.Entity] = velocity;
                }).Schedule();
        }
    }
}

