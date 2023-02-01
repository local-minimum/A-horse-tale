using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LittleHorsie : MonoBehaviour
{
    [SerializeField, Range(0, 10)]
    int _age = 1;

    public int age
    {
        get
        {
            return _age;
        }
    }

    public enum State { Idle, Selected, Planning, Ready, Acting};

    [SerializeField]
    private Material defaultMat;

    [SerializeField]
    private Material hoverMat;

    [SerializeField]
    private Material creatingMat;

    [SerializeField]
    private Material disabledMat;

    private Renderer render;

    [SerializeField]
    private JumpPlanner plannerPrefab;

    private List<JumpPlanner> planners = new List<JumpPlanner>();

    private void Start()
    {
        render = GetComponent<Renderer>();
        state = State.Idle;

        InitiatePlanners();
    }

    void InitiatePlanners()
    {
        JumpPlanner previousPlanner = null;

        for (int i = 0; i<age; i++)
        {
            var currentPlanner = Instantiate(plannerPrefab);
            currentPlanner.parentPlanner = previousPlanner;
            currentPlanner.transform.SetParent(transform);
            currentPlanner.transform.localPosition = Vector3.zero;
            currentPlanner.name = $"Planner {i + 1}";

            planners.Add(currentPlanner);

            previousPlanner = currentPlanner;
        }
    }

    private State _state = State.Idle;

    public State state
    {
        get
        {
            return _state;
        }

        set {
            switch (value)
            {
                case State.Idle:
                case State.Acting:
                    render.material = defaultMat;
                    break;
                case State.Selected:
                    render.material = hoverMat ?? defaultMat;
                    break;
                case State.Planning:
                    render.material = creatingMat ?? disabledMat;
                    break;
                case State.Ready:
                    render.material = disabledMat;
                    break;
            }
            _state = value;
        }
    }

    private bool canBeSelected
    {
        get
        {
            return age > 0 && state == State.Idle;
        }
    }

    bool mouseOver;

    private void OnMouseEnter()
    {
        mouseOver = true;
        if (canBeSelected)
        {
            state = State.Selected;
        }
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        if (state == State.Selected)
        {
            state = State.Idle;
        }
    }

    private void OnMouseDown()
    {
        if (state == State.Selected)
        {
            planners[activePlanner].awaitingPlan = true;
            state = State.Planning;
        }
    }

    private int activePlanner = 0;
    private bool jumping = false;

    private void Update()
    {
        if (state == State.Ready && Input.anyKeyDown)
        {
            activePlanner = 0;
            state = State.Acting;
            StartJumping();

        } else if (state == State.Acting && !jumping)
        {
            activePlanner++;
            if (activePlanner >= planners.Count)
            {
                state = mouseOver ? State.Selected : State.Idle;
                activePlanner = 0;
            } else
            {
                StartJumping();
            }
        } else if (state == State.Planning)
        {
            var currentPlanner = planners[activePlanner];

            if (Input.GetMouseButtonUp(0))
            {
                if (currentPlanner.FullPlan)
                {
                    currentPlanner.awaitingPlan = false;

                    activePlanner++;

                    if (activePlanner >= planners.Count)
                    {
                        state = State.Ready;
                    }
                } else if (activePlanner == 0)
                {
                    currentPlanner.ClearPlan();
                    state = mouseOver ? State.Selected : State.Idle;
                } else
                {
                    currentPlanner.ClearPlan();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                currentPlanner.awaitingPlan = true;
            }
        }
    }

    void StartJumping()
    {
        var jumps = planners[activePlanner].Jumps;
        planners[activePlanner].ClearPlan();

        for (int i = activePlanner + 1, l = planners.Count; i < l; i++)
        {
            planners[i].HidePlan();
        }

        StartCoroutine(Jump(jumps));
    }

    [SerializeField, Range(0.2f, 2f)]
    float jumpDuration = 0.8f;

    [SerializeField, Range(0, 1f)]
    float betweenJump = 0.3f;

    [SerializeField]
    AnimationCurve jumpHeight;

    IEnumerator<WaitForSeconds> Jump(List<Vector3> jumps)
    {
        jumping = true;
        foreach (var jump in jumps)
        {
            float t0 = Time.timeSinceLevelLoad;
            float progress = 0;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + jump;

            while (progress < 1)
            {
                progress = Mathf.Clamp01((Time.timeSinceLevelLoad - t0) / jumpDuration);

                transform.position = Vector3.Lerp(startPosition, targetPosition, progress) 
                    + Vector3.up * jumpHeight.Evaluate(progress);


                yield return new WaitForSeconds(0.02f);
            }
            yield return new WaitForSeconds(betweenJump);
        }

        jumping = false;
    }
}
