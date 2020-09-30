using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
