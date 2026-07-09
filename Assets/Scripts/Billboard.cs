using UnityEngine;

public class Billboard : MonoBehaviour
{
    //TODO: can be optimized more, later if needed 
    private static Transform cameraTransform;

    private void Awake()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 direction = cameraTransform.position - transform.position;
        //direction.y = 0f; // Keep text upright

        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
    }
}