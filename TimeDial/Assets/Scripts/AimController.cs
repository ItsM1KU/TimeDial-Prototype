using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] RectTransform aimIndicator;
    [SerializeField] float aimRadius = 150f;

    public Vector3 AimDirectionWorld { get; private set; } = Vector3.forward;

    private void Update()
    {
        Vector2 playerScreenPos = Camera.main.WorldToScreenPoint(player.position);
        Vector2 mousePos = Mouse.current.position.ReadValue(); 

        Vector2 dir2D = mousePos - playerScreenPos;
        if (dir2D.magnitude > aimRadius)
            dir2D = dir2D.normalized * aimRadius;

        aimIndicator.position = playerScreenPos + dir2D;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, player.position.y, 0f));

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 dir = targetPoint - player.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                AimDirectionWorld = dir.normalized;
        }
    }
}
