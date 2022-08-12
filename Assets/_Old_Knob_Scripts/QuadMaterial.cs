using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadMaterial : MonoBehaviour
{
    // Creates a material from shader and texture references.
    Shader shader;
    Texture texture;
    Color color;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        rend.material = new Material(shader);
        rend.material.mainTexture = texture;
        rend.material.color = color;
    }
}
