using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float screenEdgeBuffer = 1f;           // Space between the top/bottom most target and the screen edge.
    public float minSize = 9f;                  // The smallest orthographic size the camera can be.
    [HideInInspector] public AnimalManager[] targets; // All the targets the camera needs to encompass.

    private Camera Camera;                        // Used for referencing the camera.
    private float ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 DesiredPosition;              // The position the camera is moving towards.
    public int otherPlayersTrackingRange = 45, avgCameraTargetZaxisOffset=-2;

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
        for (int i = 0; i < targets.Length; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!targets[i].instance.activeSelf || (i!=0 && targets[0].instance.activeSelf && Vector3.Distance(targets[i].instance.transform.position, targets[0].instance.transform.position) > otherPlayersTrackingRange) )
                continue;

/********************** AVERAGE POINT FINDER SEGMENT***********************************/
            // Add to the average and increment the number of targets in the average.
            averagePos += targets[i].instance.transform.position;
            numTargets++;



/********************** REQUIRED SIZE FINDER AND ZOOMER SEGMENT***********************************/
            // If the target is active, find the position of the target in the camera's local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(targets[i].instance.transform.position);

            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the animal 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the animal being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / Camera.aspect);
        }

        // Add the edge buffer to the size.
        size += screenEdgeBuffer;

        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Max (size, minSize);
        //Debug.Log("Camera fov target: "+size*3);
        Camera.fieldOfView = Mathf.SmoothDamp (Camera.fieldOfView, size*2, ref ZoomSpeed, dampTime);



/********************** AVERAGE POINT FINDER SEGMENT***********************************/

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 1)
        {
            averagePos /= numTargets;
            averagePos.z += avgCameraTargetZaxisOffset;
        }

        // Keep the same y value.
        averagePos.y = transform.position.y;

        // The desired position is the average position;
        DesiredPosition = averagePos;

        transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition, ref MoveVelocity, dampTime);
    }






    public void SetStartPositionAndSize ()
    {
        // Find the desired position.

        // Set the camera's position to the desired position without damping.

        // Find and set the required size of the camera.
        MoveAveragePositionAndZoom();
    }
}