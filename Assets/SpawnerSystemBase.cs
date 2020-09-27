using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public abstract class SpawnerSystemBase : SystemBase {
    BeginSimulationEntityCommandBufferSystem BeginSimulationEntityCommandBufferSystem;
    protected override void OnCreate() {
        base.OnCreate();
        BeginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate() {
        var buffer = BeginSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        OnUpdate(buffer);

        BeginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    protected abstract void OnUpdate(EntityCommandBuffer.ParallelWriter buffer);
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public abstract class PossProcessingSystemBase : SystemBase {
    BeginSimulationEntityCommandBufferSystem BeginSimulationEntityCommandBufferSystem;
    protected override void OnCreate() {
        base.OnCreate();
        BeginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate() {
        var buffer = BeginSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        OnUpdate(buffer);

        BeginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }

    protected abstract void OnUpdate(EntityCommandBuffer buffer);
}