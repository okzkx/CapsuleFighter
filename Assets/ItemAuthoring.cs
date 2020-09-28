using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ItemAuthoring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public static class Item {
    public struct Entry : IBufferElementData {
        public Entity Entity;
    }
}