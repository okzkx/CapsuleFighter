using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InputSystem : SystemBase {
    protected override void OnUpdate() {

        int inputZ = 0;
        int inputX = 0;

        inputZ += Input.GetKey(KeyCode.W) ? 1 : 0;
        inputX -= Input.GetKey(KeyCode.A) ? 1 : 0;
        inputZ -= Input.GetKey(KeyCode.S) ? 1 : 0;
        inputX += Input.GetKey(KeyCode.D) ? 1 : 0;

        bool Alpha1 = Input.GetKeyDown(KeyCode.Alpha1);

        //Entities
        //    .WithAll<Player.Tag>()
        //    .ForEach((ref MoveAbility.Input input) => {
        //        input.Value.x = inputX;
        //        input.Value.y = inputZ;
        //    }).ScheduleParallel();

        //Entities
        //    .WithAll<Player.Tag>()
        //    .ForEach((ref PhysicsMoveAbility.Input physicsMoveAbilityInput) => {
        //        physicsMoveAbilityInput.Value.x = inputX;
        //        physicsMoveAbilityInput.Value.y = inputZ;
        //    }).ScheduleParallel();

        Entities
            .WithAll<Player.Tag>()
            .ForEach((ref Charactor.Input input) => {
                input.Axis.x = inputX;
                input.Axis.y = inputZ;
                input.Alpha1 = Alpha1;
            }).ScheduleParallel();
    }
}
