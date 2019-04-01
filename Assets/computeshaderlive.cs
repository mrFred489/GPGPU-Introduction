using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class computeshaderlive : MonoBehaviour
{
    public ComputeShader cs;
    public Texture2D inputTexture;

    private RenderTexture outputTexture;

    private Matrix4x4 colorTransform = new Matrix4x4(
                                                     new Vector4(1, 0, 0, 0),
                                                     new Vector4(0, 1, 0, 0),
                                                     new Vector4(0, 0, 1, 0),
                                                     new Vector4(0, 0, 0, 1)
                                                     ); // different matrices can be used to do grayscale or sepia or other color schemes


    void Start()
    {
        int kernelID = cs.FindKernel("CSMain"); // get ID of kernel, first pragma line
        int inTextureID = Shader.PropertyToID("InputTexture");
        int textureID = Shader.PropertyToID("Result");  // get ID of texture, name in shader
        int colorMatId = Shader.PropertyToID("ColorTransform");

        outputTexture = new RenderTexture(inputTexture.width, inputTexture.height, 0, RenderTextureFormat.ARGB32);
        outputTexture.enableRandomWrite = true;
        outputTexture.filterMode = FilterMode.Point; // Create sharp edges to see pixels better
        outputTexture.Create();  // Reanable after changing settings

        cs.SetTexture(kernelID, inTextureID, inputTexture);

        cs.SetTexture(kernelID, textureID, outputTexture);
        cs.SetMatrix(colorMatId, colorTransform);
        
        cs.Dispatch(kernelID, outputTexture.width/8, outputTexture.height/8, 1);  // texture size divided by threads on that axis gives number of groups in each axis

        Material material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = outputTexture;
        GetComponent<MeshRenderer>().sharedMaterial = material;

    }


}
