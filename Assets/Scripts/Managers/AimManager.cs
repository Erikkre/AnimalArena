
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimManager : MonoBehaviour
{
    #region Variables

    public Camera cam;
    public Transform reticleTrans;
    public MeshRenderer meshRend;
    public LayerMask onlyRaycastGround;
    #endregion

    // Update is called once per frame
    void FixedUpdate()
    {
        if (reticleTrans) handleInput();
    }

    protected virtual void handleInput()
    {
        Ray screenRay = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit h;

        if (Physics.Raycast(screenRay, out h, int.MaxValue, onlyRaycastGround) )//if ray from camera to mouse pos hits something with a collider, reticlePos and reticleNormal are set.
        {
            reticleTrans.position = new Vector3(h.point.x, -0.01f, h.point.z);
        }
    }
}
