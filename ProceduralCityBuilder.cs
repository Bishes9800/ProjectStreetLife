using UnityEngine;

public class ProceduralCityBuilder : MonoBehaviour
{
    [Header("City layout")]
    public int blocksPerSide = 5;
    public float blockSpacing = 28f;
    public float roadWidth = 8f;
    public float groundSize = 320f;
    public int randomSeed = 1234;
    public bool generateOnStart = true;

    [Header("Building heights")]
    public float minBuildingHeight = 5f;
    public float maxBuildingHeight = 24f;

    private void Start()
    {
        if (generateOnStart) GenerateCity();
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        Random.InitState(randomSeed);

        CreateCube("Ground", new Vector3(0, -0.6f, 0), new Vector3(groundSize, 1, groundSize),
            new Color(0.22f, 0.35f, 0.22f));

        for (int i = -blocksPerSide; i <= blocksPerSide; i++)
        {
            float p = i * blockSpacing;
            CreateCube("Road_NS", new Vector3(p, -0.05f, 0),
                new Vector3(roadWidth, 0.1f, groundSize), new Color(0.13f, 0.14f, 0.15f));
            CreateCube("Road_EW", new Vector3(0, -0.04f, p),
                new Vector3(groundSize, 0.1f, roadWidth), new Color(0.13f, 0.14f, 0.15f));
        }

        for (int x = -blocksPerSide; x < blocksPerSide; x++)
        for (int z = -blocksPerSide; z < blocksPerSide; z++)
        {
            float centerX = (x + 0.5f) * blockSpacing;
            float centerZ = (z + 0.5f) * blockSpacing;
            float usable = blockSpacing - roadWidth - 3f;
            float width = Random.Range(7f, usable);
            float depth = Random.Range(7f, usable);
            float height = Random.Range(minBuildingHeight, maxBuildingHeight);

            // Keep the central starting area more open.
            if (Mathf.Abs(centerX) < blockSpacing && Mathf.Abs(centerZ) < blockSpacing) continue;

            Color tint = Color.Lerp(new Color(0.35f, 0.4f, 0.45f),
                                    new Color(0.65f, 0.62f, 0.55f), Random.value);
            CreateCube("Building", new Vector3(centerX, height * 0.5f, centerZ),
                new Vector3(width, height, depth), tint);
        }
    }

    private GameObject CreateCube(string objectName, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = objectName;
        go.transform.SetParent(transform, true);
        go.transform.position = position;
        go.transform.localScale = scale;

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.color = color;
                renderer.sharedMaterial = mat;
            }
        }
        return go;
    }
}
