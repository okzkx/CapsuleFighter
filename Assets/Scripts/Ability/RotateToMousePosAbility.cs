using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public static class RotateToMousePos {
    public static class Ability {
        public struct Tag : IComponentData { }
        public struct Spawn : IComponentData {
            public Entity Owner;
        }

        public class SpawnSystem : SystemBase {
            protected override void OnUpdate() {
                // Spawn
                Entities
                    .WithoutBurst()
                    .WithStructuralChanges()
                    .ForEach((Entity entity, in Spawn spawn) => {
                        var spawnedEntity = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(spawnedEntity, new Tag { });
                        EntityManager.AddComponentData(spawnedEntity, new Owner { Entity = spawn.Owner });
                        EntityManager.AddComponentData(spawnedEntity, new MousePosInput { });
                        EntityManager.AddComponentData(spawnedEntity, new AbilityCommon.Active { });

                        var characterEntity = SystemUtil.GetCharacter(EntityManager, spawn.Owner);
                        EntityManager.SetComponentData(characterEntity, new Character.MousePosAbility { Entity = spawnedEntity });

                        EntityManager.DestroyEntity(entity);
                    }).Run();
            }
        }

        public class UpdateSystem : SystemBase {
            Camera camera;
            BuildPhysicsWorld buildPhysicsWorld;
            protected override void OnCreate() {
                base.OnCreate();
                buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            }
            protected override void OnUpdate() {
                if (camera == null) {
                    camera = Camera.main;
                }

                Entities
                    .WithoutBurst()
                    .WithAll<Tag, AbilityCommon.Active>()
                    .ForEach((in Owner owner, in MousePosInput input) => {
                        var characterEntity = SystemUtil.GetCharacter(EntityManager, owner.Entity);
                        Rotation rotation = EntityManager.GetComponentData<Rotation>(characterEntity);
                        // Translation
                        var unityRay = camera.ScreenPointToRay(Input.mousePosition);
                        var ray = UnityRayToPhysicRay(unityRay);
                        ray.Filter.CollidesWith = ~(uint)0;
                        ray.Filter.BelongsTo = ~(uint)0;
                        var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
                        if (collisionWorld.CastRay(ray, out var raycastHit)) {
                            float3 pos = raycastHit.Position;
                            float3 characterPos = EntityManager.GetComponentData<Translation>(characterEntity).Value;

                            float3 lookForward = pos - characterPos;
                            lookForward.y = 0;
                            lookForward = math.normalize(lookForward);
                            //var ltw = EntityManager.GetComponentData<LocalToWorld>(characterEntity);
                            //float3 xAxis = math.cross(new float3(0, 1, 0), lookForward);
                            //ltw.Value.c0.x = xAxis.x;
                            //ltw.Value.c0.y = xAxis.y;
                            //ltw.Value.c0.z = xAxis.z;
                            //ltw.Value.c1 = new float4();
                            //ltw.Value.c2.x = lookForward.x;
                            //ltw.Value.c2.y = lookForward.y;
                            //ltw.Value.c2.z = lookForward.z;
                            //EntityManager.SetComponentData(characterEntity, ltw);


                            var re = EntityManager.GetComponentData<RotationEulerXYZ>(characterEntity);
                            re.Value.y = math.atan2(lookForward.x, lookForward.z);
                            EntityManager.SetComponentData(characterEntity, re);
                        }

                    }).Run();
            }

            public static RaycastInput UnityRayToPhysicRay(UnityEngine.Ray unityRay, float length = 1000) {
                return new RaycastInput() {
                    Start = unityRay.origin,
                    End = unityRay.direction * length + unityRay.origin
                };
            }
        }
    }
}