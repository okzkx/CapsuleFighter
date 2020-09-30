using Unity.Entities;

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

