using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSMeshLive : MonoBehaviour
{

    public struct Triangle{ // order and size important. can be done with arrays if necessary, and some properties can be enforced with flags.
        public Vector3 center;
        public Vector3 normal;
    }

    public ComputeShader shader;

    public ComputeBuffer trianglesBuffer; // send and receive data using this buffer
    public Triangle[] triangles;
    

    void Start()
    {
        int kernelId = shader.FindKernel("CSTriangles");

        trianglesBuffer = new ComputeBuffer(128, sizeof(float) * 6); // same as array, data types * number of them (3 floats per vector, two vectors, each float is 4 bytes = 24)
        triangles = new Triangle[128]; // same as number of threads

        shader.SetBuffer(kernelId, "ResultTriangles"/* name or id, id more efficient, shader.PropertyToId*/, trianglesBuffer);
        shader.Dispatch(kernelId, 2, 1, 1);

        trianglesBuffer.GetData(triangles); // blocking, waiting for data

        
        
    }

    private void OnDrawGizmos(){
        if (triangles != null){
            foreach (Triangle triangle in triangles){
                Gizmos.color = Color.red;
                Gizmos.DrawLine(triangle.center, triangle.center + triangle.normal);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(triangle.center, 0.01f);
            }
        }
    }

}
