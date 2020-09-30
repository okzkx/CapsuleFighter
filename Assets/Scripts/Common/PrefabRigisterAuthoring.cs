using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PrefabRigister : IComponentData {
    public Entity Pistol;
    public Entity Bullet;
}

public class PrefabRigisterAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs {
    public GameObject Pistol;
    public GameObject Bullet;

    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) {
        referencedPrefabs.Add(Pistol);
        referencedPrefabs.Add(Bullet);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {

        dstManager.AddComponentData(entity, new PrefabRigister {
            Pistol = conversionSystem.GetPrimaryEntity(Pistol),
            Bullet = conversionSystem.GetPrimaryEntity(Bullet)
        });
    }
}
