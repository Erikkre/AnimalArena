using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float DampTime = 0.1f;                 // Approximate time for the camera to refocus.
    public float ScreenEdgeBuffer = 3f;           // Space between the top/bottom most target and the screen edge.
    public float MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
    [HideInInspector] public AnimalManager[] Targets; // All the targets the camera needs to encompass.

    private Camera Camera;                        // Used for referencing the camera.
    private float ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 DesiredPosition;              // The position the camera is moving towards.


    private void Awake ()
    {
        Camera = GetComponentInChildren<Camera> ();
    }


    private void FixedUpdate ()
    {
        // Move the camera towards a desired position.
        MoveAveragePositionAndZoom ();


    }



    private void MoveAveragePositionAndZoom ()
    {
/********************** AVERAGE POINT FINDER SEGMENT***********************************/
        Vector3 averagePos = new Vector3 ();
        int numTargets = 0;


/********************** REQUIRED SIZE FINDER AND ZOOMER SEGMENT***********************************/
        Vector3 desiredLocalPos = transform.InverseTransformPoint(DesiredPosition);

        // Start the camera's size calculation at zero.
        float size = 0f;

        // Go through all the targets...
        for (int i = 0; i < Targets.Length; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!Targets[i].Instance.activeSelf || (i!=0 && Targets[0].Instance.activeSelf && Vector3.Distance(Targets[i].Instance.transform.position, Targets[0].Instance.transform.position) > 40) )
                continue;

/********************** AVERAGE POINT FINDER SEGMENT***********************************/
            // Add to the average and increment the number of targets in the average.
            averagePos += Targets[i].Instance.transform.position;
            numTargets++;



/********************** REQUIRED SIZE FINDER AND ZOOMER SEGMENT***********************************/
            // If the target is active, find the position of the target in the camera's local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(Targets[i].Instance.transform.position);

            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the animal 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the animal being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / Camera.aspect);
        }

        // Add the edge buffer to the size.
        size += ScreenEdgeBuffer;

        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Max (size, MinSize);
        Camera.orthographicSize = Mathf.SmoothDamp (Camera.orthographicSize, size, ref ZoomSpeed, DampTime);



/********************** AVERAGE POINT FINDER SEGMENT***********************************/

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 0)
            averagePos /= numTargets;

        // Keep the same y value.
        averagePos.y = transform.position.y;

        // The desired position is the average position;
        DesiredPosition = averagePos;

        transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition, ref MoveVelocity, DampTime);

    }






    public void SetStartPositionAndSize ()
    {
        // Find the desired position.

        // Set the camera's position to the desired position without damping.

        // Find and set the required size of the camera.
        MoveAveragePositionAndZoom();
    }
}