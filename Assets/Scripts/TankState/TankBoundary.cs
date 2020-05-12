using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TankBoundary : MonoBehaviour
{

    public Bounds tankBounds = new Bounds(new Vector3(-0.25f, -0.5f, -0.25f), new Vector3(1f, 0.5f, 0.5f));
    public MeshFilter boundsSourceMesh;

    // Start is called before the first frame update
    void Start()
    {

        if (boundsSourceMesh != null)
        {
            tankBounds = new Bounds(boundsSourceMesh.transform.position, boundsSourceMesh.mesh.bounds.size);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(tankBounds.center, tankBounds.size);
    }

    public Vector3 RandomPosition(bool lockX = false, bool lockY = false, bool lockZ = false)
    {
        return new Vector3(
            lockX ? tankBounds.min.x : Random.Range(tankBounds.min.x, tankBounds.max.x),
            lockY ? tankBounds.min.y : Random.Range(tankBounds.min.y, tankBounds.max.y),
            lockZ ? tankBounds.min.z : Random.Range(tankBounds.min.z, tankBounds.max.z));
    }
    //internal Vector3 RandomPosition()
    //{
    //    return RandomPosition(false, false, false);
    //    //return new Vector3(
    //    //    Random.Range(tankBounds.min.x, tankBounds.max.x),
    //    //    Random.Range(tankBounds.min.y, tankBounds.max.y),
    //    //    Random.Range(tankBounds.min.z, tankBounds.max.z));
    //}
}
