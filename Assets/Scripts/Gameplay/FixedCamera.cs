using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
