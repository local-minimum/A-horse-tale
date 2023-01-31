using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareGrid : MonoBehaviour
{
    [SerializeField]
    int Rows = 8;

    [SerializeField]
    int Columns = 8;

    [SerializeField]
    Position tilePrefab;

    [SerializeField]
    float _gridSpacing = 1.05f;

    public float gridSpacing
    {
        get
        {
            return _gridSpacing;
        }
    }

    [SerializeField]
    Material[] materials;

    private void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        int matIdx = 0;

        for (int z = 0; z < Rows; z++)
        {
            matIdx++;

            for (int x = 0; x < Columns; x++)
            {
                var tile = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, true);
                tile.transform.position = new Vector3(transform.position.x + (x - Columns / 2f) * gridSpacing, transform.position.y, transform.position.z + (z - Rows / 2f) * gridSpacing);

                matIdx++;
                matIdx %= materials.Length;
                tile.GetComponent<Renderer>().material = materials[matIdx];
            }
        }
    }
}
