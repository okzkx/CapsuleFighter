using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AbilityCommon {
    public struct Tag : IComponentData { }

    public struct State : IComponentData {
        Entity owner;
    }

    public struct Entry : IBufferElementData {
        public Entity Entity;
    }

    public struct Setting : IComponentData {
        public int ActiveButton;
        public int DisactiveButton;
    }

    public struct Idle : IComponentData { }
    public struct Active : IComponentData { }
    public struct Coldown : IComponentData { }
}

public class AbilitySystem : SystemBase {
    protected override void OnUpdate() {

    }
}
