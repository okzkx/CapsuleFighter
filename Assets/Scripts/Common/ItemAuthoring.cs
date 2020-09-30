using Unity.Entities;
using UnityEngine;

public static class Item {
    public struct Entry : IBufferElementData {
        public Entity Entity;
    }
}