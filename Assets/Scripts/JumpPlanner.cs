using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlanner : MonoBehaviour
{
    public JumpPlanner parentPlanner { get; set; }

    SquareGrid grid;
    LittleHorsie littleHorsie;
    LineRenderer line;
    JumpPlannerTarget jumpTargetUI;

    [SerializeField, Range(-2, 2)]
    float yOffset = -1.2f;

    [SerializeField, Range(0, 1)]
    float fullLineFinalLeg = 0.2f;

    private void Start()
    {
        grid = FindObjectOfType<SquareGrid>();
        littleHorsie = GetComponentInParent<LittleHorsie>();
        line = GetComponent<LineRenderer>();
        jumpTargetUI = GetComponentInChildren<JumpPlannerTarget>();
    }

    public bool awaitingPlan = false;

    private void Update()
    {
        if (awaitingPlan && (parentPlanner == null || !parentPlanner.awaitingPlan))
        {
            switch (littleHorsie.state)
            {
                case LittleHorsie.State.Planning:
                    UpdatePlan();
                    break;

            }

            UpdateLinePositions();
        }
    }

    int x;
    int z;

    private void UpdatePlan()
    {
        if (!Input.GetMouseButton(0)) return;

        var plane = new Plane(Vector3.up, yOffset);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float enter;
        if (plane.Raycast(ray, out enter))
        {
            var planePosition = ray.GetPoint(enter);
            var offset = (planePosition - (littleHorsie.transform.position + OriginOffset)) / grid.gridSpacing;
            x = Mathf.RoundToInt(offset.x);
            z = Mathf.RoundToInt(offset.z);

            ShowPlan();
        }
    }
    public bool FullPlan
    {
        get
        {
            return Mathf.Abs(x) + Mathf.Abs(z) == 3 && x != 0 && z != 0;
        }
    }

    public Vector3 Target
    {
        get
        {
            if (FullPlan)
            {
                return new Vector3(x * grid.gridSpacing, yOffset, z * grid.gridSpacing);
            }
            return Vector3.zero;
        }
    }

    public Vector3 OriginOffset
    {
        get
        {
            var parentOrigin = parentPlanner?.OriginOffset ?? Vector3.zero;

            var offset = parentPlanner?.Target ?? Vector3.zero;
            offset.y = 0;

            return offset + parentOrigin;
        }
    }


    void UpdateLinePositions()
    {
        if (x == 0 && z == 0)
        {
            HidePlan();
            return;
        }
        var offset = OriginOffset;

        var origin = Vector3.up * yOffset + offset;
        if (FullPlan)
        {
            line.positionCount = 3;
            var target = Target + offset;

            if (Mathf.Abs(x) > Mathf.Abs(z))
            {
                var middle = new Vector3(x * grid.gridSpacing, yOffset, 0) + offset;
                line.SetPositions(new Vector3[] {
                        origin,
                        middle,
                        Vector3.Lerp(middle, target, fullLineFinalLeg),
                    });
                jumpTargetUI.transform.position = transform.position + target;
            }
            else
            {
                var middle = new Vector3(0, yOffset, z * grid.gridSpacing) + offset;
                line.SetPositions(new Vector3[] {
                        origin,
                        middle,
                        Vector3.Lerp(middle, target, fullLineFinalLeg),
                    });
                jumpTargetUI.transform.position = transform.position + target;
            }            
        }
        else if (x != 0 && z == 0)
        {
            line.positionCount = 2;
            line.SetPositions(new Vector3[]
            {
                    origin,
                    new Vector3(Mathf.Clamp(x, -2, 2) * grid.gridSpacing, yOffset, 0) + offset,
            });
        }
        else if (z != 0 && x == 0)
        {
            line.positionCount = 2;
            line.SetPositions(new Vector3[]
            {
                    origin,
                    new Vector3(0, yOffset, Mathf.Clamp(z, -2, 2) * grid.gridSpacing) + offset,
            });
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
                jumps.Add(new Vector3(x / 2f, 0, 0) * grid.gridSpacing);
                jumps.Add(new Vector3(x / 2f, 0, 0) * grid.gridSpacing);
                jumps.Add(new Vector3(0, 0, z) * grid.gridSpacing);
            } else
            {
                jumps.Add(new Vector3(0, 0, z / 2f) * grid.gridSpacing);
                jumps.Add(new Vector3(0, 0, z / 2f) * grid.gridSpacing);
                jumps.Add(new Vector3(x, 0, 0) * grid.gridSpacing);
            }

            return jumps;
        }
    }

    public void ShowPlan()
    {
        jumpTargetUI.Visible = FullPlan;
        line.enabled = true;
    }

    public void ClearPlan()
    {        
        x = 0;
        z = 0;
        HidePlan();
    }

    public void HidePlan()
    {
        line.enabled = false;
        jumpTargetUI.Visible = false;
    }
}
