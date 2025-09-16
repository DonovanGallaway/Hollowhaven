using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    // Maybe need to make this Game Manager and decouple GridManager functionality into its own system
    [Header("Managers")]
    public TurnManager turnManager;
    [Header("Grid Settings")]
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float squareSize = 1.0f;
    public float spacing = 0.1f;
    public GridSquare[,] gameBoard;
    [Header("Prefabs")]
    public GameObject squarePrefab;
    public Texture2D[] ashedTextures;
    public Texture2D[] kindlingTextures;
    public Texture2D[] ignitedTextures;
    public Texture2D[] uncapturedTextures;
    public Texture2D[] heightMaps;
    public Texture2D[] normalMaps;
    public Texture2D[] emissionMaps;
    public Texture2D metallicMap;

    void Start()
    {
        GenerateGrid();
    }

    public void ApplyElementTexture(GameObject square, ElementType elementType, Texture2D[] albedoMaps, bool emissions = false)
    {
        Renderer renderer = square.GetComponent<Renderer>();
        Material mat = new Material(renderer.material);

        mat.SetTexture("_MainTex", albedoMaps[(int)elementType]);
        mat.SetTexture("_BumpMap", normalMaps[(int)elementType]);
        mat.SetTexture("_ParallaxMap", heightMaps[(int)elementType]);
        if (emissions)
        {
            mat.SetTexture("_EmissionMap", emissionMaps[(int)elementType]);
        }
        else
        {
            mat.SetTexture("_EmissionMap", null);
        }

        renderer.material = mat;
    }

    void GenerateGrid()
    {
        
        gameBoard = new GridSquare[gridWidth, gridHeight];

        List<ElementType> elementDistribution = new List<ElementType>();
        int totalSquares = gridWidth * gridHeight;
        int elementsPerType = totalSquares / System.Enum.GetValues(typeof(ElementType)).Length;

        // Should always be divisible by 4 for this to work correctly
        foreach (ElementType type in System.Enum.GetValues(typeof(ElementType)))
        {
            for (int i = 0; i < elementsPerType; i++)
            {
                elementDistribution.Add(type);
            }
        }

        for (int i = 0; i < elementDistribution.Count; i++)
        {
            int randomIndex = Random.Range(i, elementDistribution.Count);
            ElementType temp = elementDistribution[i];
            elementDistribution[i] = elementDistribution[randomIndex];
            elementDistribution[randomIndex] = temp;
        }

        // Set loop to iterate over element type
        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                GameObject square = Instantiate(squarePrefab);
                // square.transform.localScale = Vector3.one * 10f;
                square.name = $"Square_{x}_{z}";
                Vector3 position = new Vector3(
                    x * (squareSize + spacing),
                    0,
                    z * (squareSize + spacing)
                );
                square.transform.position = position;
                // square.transform.localScale = Vector3.one * squareSize;
                square.transform.parent = this.transform;

                ApplyElementTexture(square, elementDistribution[index], uncapturedTextures);


                gameBoard[x, z] = new GridSquare();
                gameBoard[x, z].visualRepresentation = square;
                gameBoard[x, z].elementType = elementDistribution[index++];


                square.AddComponent<BoxCollider>();
                SquareController controller = square.AddComponent<SquareController>();
                controller.gridPosition = new Vector2Int(x, z);
                controller.gridManager = this;
            }
        }



        CenterCamera();
    }

void CenterCamera()
{
    Camera.main.transform.position = new Vector3(20, 35, 5);
    Camera.main.transform.rotation = Quaternion.Euler(75, 0, 0);
}

    public void OnSquareClicked(int x, int z)
    {
        GridSquare square = gameBoard[x, z];
        if (square.playerOwnership == PlayerN.None)
        {
            square.playerOwnership = turnManager.currentPlayer.identity;
            square.manaState = ManaState.Kindling;
            UpdateSquareVisual(x, z);
        }
        else
        {
            switch (square.manaState)
            {
                case ManaState.Kindling:
                    square.manaState = ManaState.Ignited;
                    break;
                case ManaState.Ignited:
                    square.manaState = ManaState.Ashed;
                    switch (square.elementType)
                    {
                        case ElementType.Fire:
                            turnManager.currentPlayer.manaValues.fire++;
                            break;
                        case ElementType.Water:
                            turnManager.currentPlayer.manaValues.water++;
                            break;
                        case ElementType.Earth:
                            turnManager.currentPlayer.manaValues.earth++;
                            break;
                        case ElementType.Air:
                            turnManager.currentPlayer.manaValues.air++;
                            break;
                    }
                    turnManager.UpdateManaText();
                    break;
                case ManaState.Ashed:
                    square.manaState = ManaState.Kindling;
                    break;
            }
            UpdateSquareVisual(x, z);
        }
    }

    public void OnSquareRightClicked(int x, int z)
    {
        GridSquare square = gameBoard[x, z];
        if (square.playerOwnership != PlayerN.None)
        {
            // Uncapture the square
            square.playerOwnership = PlayerN.None;
            square.manaState = ManaState.Uncaptured; // Reset to uncaptured state
            
            // Remove the ownership border
            GameObject borderObj = GameObject.Find($"OwnershipBorder_{x}_{z}");
            if (borderObj != null)
            {
                DestroyImmediate(borderObj);
            }
            
            UpdateSquareVisual(x, z);
        }
    }

    void UpdateSquareVisual(int x, int z)
    {
        GridSquare square = gameBoard[x, z];
        GameObject tile = square.visualRepresentation;
        Renderer renderer = tile.GetComponent<Renderer>();
        Texture2D[] albedoMap;
        bool emissions;
        switch (square.manaState)
        {
            case ManaState.Ignited:
                albedoMap = ignitedTextures;
                emissions = true;
                break;
            case ManaState.Kindling:
                albedoMap = kindlingTextures;
                emissions = true;
                break;
            case ManaState.Ashed:
                albedoMap = ashedTextures;
                emissions = false;
                break;
            default:
                albedoMap = uncapturedTextures;
                emissions = false;
                break;
        }

        if (square.playerOwnership != PlayerN.None)
        {
            // Try to find border by name in the scene instead of as child
            GameObject borderObj = GameObject.Find($"OwnershipBorder_{x}_{z}");

            if (borderObj == null)
            {
                borderObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                borderObj.name = $"OwnershipBorder_{x}_{z}";

                borderObj.transform.position = tile.transform.position;

                Bounds tileBounds = tile.GetComponent<Renderer>().bounds;
                float borderSize = tileBounds.size.x * 1.05f;
                borderObj.transform.localScale = new Vector3(borderSize, 0.05f, borderSize);

                borderObj.transform.position = tile.transform.position + Vector3.up * 0.02f;
            }

            Renderer borderRenderer = borderObj.GetComponent<Renderer>();
            Material borderMat = new Material(Shader.Find("Standard"));

            if (square.playerOwnership == PlayerN.Player1)
            {
                borderMat.color = Color.white;
                borderMat.SetColor("_EmissionColor", Color.white * 2.0f);
                borderMat.EnableKeyword("_EMISSION");
            }
            else if (square.playerOwnership == PlayerN.Player2)
            {
                borderMat.color = Color.black;
                borderMat.SetColor("_EmissionColor", Color.gray * 0.3f);
                borderMat.EnableKeyword("_EMISSION");
            }

            borderRenderer.material = borderMat;
        }

        ApplyElementTexture(square.visualRepresentation, square.elementType, albedoMap, emissions);

    }

    public void GridMana()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Need to add different switch for Qualia active
                // Probably need to give "types" of Qualia so that we know which functions to check
                UpdateMana(gameBoard[x, z]);
                UpdateSquareVisual(x, z);
            }
        }
    }

    public void UpdateMana(GridSquare square)
    {
        if (turnManager.currentPlayer == square.playerOwnership)
        {
            if (square.manaState == ManaState.Ashed)
                {
                    square.manaState = ManaState.Kindling;
                }
                else if (square.manaState == ManaState.Kindling)
                {
                    square.manaState = ManaState.Ignited;
                }
        }

    }

    void Update()
    {

    }

}
