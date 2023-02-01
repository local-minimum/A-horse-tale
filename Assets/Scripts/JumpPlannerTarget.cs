using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpPlannerTarget : MonoBehaviour
{
    [SerializeField]
    Image image;

    public bool Visible
    {
        get
        {
            return image.enabled;
        }

        set
        {
            image.enabled = value;
        }
    }
}
