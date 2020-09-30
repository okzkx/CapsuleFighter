using Unity.Entities;

public class AbilityCommon {
    public struct Tag : IComponentData { }

    public struct Entry : IBufferElementData {
        public Entity Entity;
    }

    public struct Idle : IComponentData { }
    public struct Active : IComponentData { }
    public struct Coldown : IComponentData { }
}

