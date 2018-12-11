using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScreen_EXPBar : MonoBehaviour {

    [SerializeField]
    private Image Crown;
    public bool IsLeader { get { return Crown.enabled; } set { Crown.enabled = value; } }
    
}
