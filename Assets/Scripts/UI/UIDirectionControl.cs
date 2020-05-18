using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool UseRelativeRotation = true;  


    private Quaternion RelativeRotation;     


    private void Start()
    {
        RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (UseRelativeRotation)
            transform.rotation = RelativeRotation;
    }
}
