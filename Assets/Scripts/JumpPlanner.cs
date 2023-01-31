using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlanner : MonoBehaviour
{
    SquareGrid grid;
    LittleHorsie littleHorsie;
    LineRenderer line;

    [SerializeField, Range(-2, 2)]
    float yOffset = -1.2f;

    private void Start()
    {
        grid = FindObjectOfType<SquareGrid>();
        littleHorsie = GetComponentInParent<LittleHorsie>();
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        switch (littleHorsie.state)
        {
            case LittleHorsie.State.Planning:
                UpdatePlan();
                break;
            default:
                AutoClearPlan();
                break;
        }

        UpdateLinePositions();
    }

    int x;
    int z;

    private void UpdatePlan()
    {
        var plane = new Plane(Vector3.up, yOffset);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float enter;
        if (plane.Raycast(ray, out enter)) {
            var planePosition = ray.GetPoint(enter);
            var offset = (planePosition - littleHorsie.transform.position) / grid.gridSpacing;
            x = Mathf.RoundToInt(offset.x);
            z = Mathf.RoundToInt(offset.z);

            Debug.Log($"x:{x}, z:{z}");
        }
    }

    void UpdateLinePositions()
    {
        if (FullPlan)
        {
            line.positionCount = 3;
            if (Mathf.Abs(x) > Mathf.Abs(z))
            {
                line.SetPositions(new Vector3[] {
                        Vector3.up * yOffset,
                        new Vector3(x * grid.gridSpacing, yOffset, 0),
                        new Vector3(x * grid.gridSpacing, yOffset, z * grid.gridSpacing),
                    });
            }
            else
            {
                line.SetPositions(new Vector3[] {
                        Vector3.up * yOffset,
                        new Vector3(0, yOffset, z * grid.gridSpacing),
                        new Vector3(x * grid.gridSpacing, yOffset, z * grid.gridSpacing),
                    });
            }
            line.enabled = true;
        }
        else if (x != 0 && z == 0)
        {
            line.positionCount = 2;
            line.SetPositions(new Vector3[]
            {
                    Vector3.up * yOffset,
                    new Vector3(Mathf.Clamp(x, -2, 2) * grid.gridSpacing, yOffset, 0),
            });
            line.enabled = true;
        }
        else if (z != 0 && x == 0)
        {
            line.positionCount = 2;
            line.SetPositions(new Vector3[]
            {
                    Vector3.up * yOffset,
                    new Vector3(0, yOffset, Mathf.Clamp(z, -2, 2) * grid.gridSpacing),
            });
            line.enabled = true;
        }
    }

    public bool FullPlan
    {
        get
        {
            return Mathf.Abs(x) + Mathf.Abs(z) == 3 && x != 0 && z != 0;
        }
    }

    public List<Vector3> Jumps
    {
        get
        {
            var jumps = new List<Vector3>();

            if (!FullPlan) return jumps;

            if (Mathf.Abs(x) > Mathf.Abs(z))
            {
                jumps.Add(new Vector3(x / 2f, 0, 0));
                jumps.Add(new Vector3(x / 2f, 0, 0));
                jumps.Add(new Vector3(0, 0, z));
            } else
            {
                jumps.Add(new Vector3(0, 0, z / 2f));
                jumps.Add(new Vector3(0, 0, z / 2f));
                jumps.Add(new Vector3(x, 0, 0));
            }

            return jumps;
        }
    }

    public void ClearPlan()
    {
        line.enabled = false;
        x = 0;
        z = 0;
    }

    private void AutoClearPlan()
    {                
        if (!FullPlan)
        {
            line.enabled = false;
            x = 0;
            z = 0;
        }
    }
}
