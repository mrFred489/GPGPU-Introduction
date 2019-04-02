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
    public MeshFilter meshFilter;


    public ComputeBuffer trianglesBuffer; // send and receive data using this buffer
    public ComputeBuffer indicesBuffer;
    public ComputeBuffer positionsBuffer;


    public Triangle[] triangles;
    

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        int[] indices = mesh.triangles;
        Vector3[] positions = mesh.vertices;


        int kernelId = shader.FindKernel("CSTriangles");

        indicesBuffer = new ComputeBuffer(indices.Length, sizeof(int));
        positionsBuffer = new ComputeBuffer(positions.Length, sizeof(float) * 3);
        int triangleCount = indices.Length / 3;
        trianglesBuffer = new ComputeBuffer(triangleCount, sizeof(float) * 6); // same as array, data types * number of them (3 floats per vector, two vectors, each float is 4 bytes = 24)
        triangles = new Triangle[triangleCount]; 

        
        
        indicesBuffer.SetData(indices);
        positionsBuffer.SetData(positions);
        
        shader.SetBuffer(kernelId, "ResultTriangles"/* name or id, id more efficient, shader.PropertyToId*/, trianglesBuffer);
        shader.SetBuffer(kernelId, "Indices", indicesBuffer);
        shader.SetBuffer(kernelId, "Positions", positionsBuffer);
        shader.Dispatch(kernelId, (triangleCount + 63)/64, 1, 1);

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

    private void OnDestroy(){ // use to clean buffers
        indicesBuffer.Dispose();
        positionsBuffer.Dispose();
        trianglesBuffer.Dispose();
    }
}
