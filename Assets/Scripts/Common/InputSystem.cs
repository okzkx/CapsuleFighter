using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AxisInput : IComponentData {
    public float2 Value;
}

public struct BoolInput : IComponentData {
    public bool Value;
}

public struct MousePosInput : IComponentData {
    public float3 Value;
}

public struct ParentInput : IComponentData {
    public Entity Entity;
}

public class InputSystem : SystemBase {
    protected override void OnUpdate() {

        int inputZ = 0;
        int inputX = 0;

        inputZ += Input.GetKey(KeyCode.W) ? 1 : 0;
        inputX -= Input.GetKey(KeyCode.A) ? 1 : 0;
        inputZ -= Input.GetKey(KeyCode.S) ? 1 : 0;
        inputX += Input.GetKey(KeyCode.D) ? 1 : 0;

        bool Alpha1 = Input.GetKeyDown(KeyCode.Alpha1);
        bool LeftPress = Input.GetMouseButton(0);

        Entities
            .WithoutBurst()
            .WithAll<Player.Tag>()
            .ForEach((ref Character.Input input) => {
                input.Axis.x = inputX;
                input.Axis.y = inputZ;
                input.Alpha1 = Alpha1;
                input.LeftPress = LeftPress;
                input.MousePos = Input.mousePosition;
            }).Run();
    }
}
