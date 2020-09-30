using System;
using Unity.Entities;

public static class SystemUtil {
    internal static Entity GetCharacter(EntityManager entityManager, Entity entity) {
        while (!entityManager.HasComponent<Character.Tag>(entity)) {
            if (entity == Entity.Null) {
                throw new Exception(entity + "不属于 Character");
            }
            if (!entityManager.HasComponent<Owner>(entity)) {
                throw new Exception(entity.Index + " 没有 Owner");
            }
            entity = entityManager.GetComponentData<Owner>(entity).Entity;
        }
        return entity;
    }
}
