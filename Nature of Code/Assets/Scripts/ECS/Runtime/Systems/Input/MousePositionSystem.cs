using Unity.Entities;
using UnityEngine;

public partial class MousePositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Entities.ForEach((ref MousePosition mousePosition) =>
        {
            mousePosition.value = mousePos;

        }).Run();
    }
}
