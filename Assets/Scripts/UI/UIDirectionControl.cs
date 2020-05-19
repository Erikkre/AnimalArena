using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool useRelativeRotation = true;  


    private Quaternion RelativeRotation;     


    private void Start()
    {
        RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (useRelativeRotation)
            transform.rotation = RelativeRotation;
    }
}
