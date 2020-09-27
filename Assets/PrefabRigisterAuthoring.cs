using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PrefabRigister : IComponentData {
    public Entity Pistol;
}

public class PrefabRigisterAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs{
    public GameObject Pistol;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        referencedPrefabs.Add(Pistol);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {

        dstManager.AddComponentData(entity, new PrefabRigister {
            Pistol = conversionSystem.GetPrimaryEntity(Pistol)
        }); ;
    }
}
