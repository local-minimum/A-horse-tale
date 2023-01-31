using System.Collections;
using System.Collections.Generic;
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
    private JumpPlanner planner;


    private void Start()
    {
        planner = GetComponentInChildren<JumpPlanner>();
        render = GetComponent<Renderer>();
        state = State.Idle;
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
            state = State.Planning;
        }
    }

    private void OnMouseUp()
    {
        if (state == State.Planning)
        {
            if (planner.FullPlan)
            {
                state = State.Ready;
            } else
            {
                state = mouseOver ? State.Selected : State.Idle;
            }            
        }
    }

    private void Update()
    {
        if (state == State.Ready && Input.anyKeyDown)
        {
            state = State.Acting;
            var jumps = planner.Jumps;
            planner.ClearPlan();
            StartCoroutine(Jump(jumps));
        }
    }

    [SerializeField, Range(0.2f, 2f)]
    float jumpDuration = 0.8f;

    [SerializeField, Range(0, 1f)]
    float betweenJump = 0.3f;

    [SerializeField]
    AnimationCurve jumpHeight;

    IEnumerator<WaitForSeconds> Jump(List<Vector3> jumps)
    {
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
        state = mouseOver ? State.Selected : State.Idle;
    }
}
