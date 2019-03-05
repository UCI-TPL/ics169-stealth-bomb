using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointText : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private Animator animator;

    public void SetText(string value) {
        text.text = value;
    }

    public void SetSpeed(float value) {
        animator.speed = value;
    }

    private void DestroySelf() {
        Destroy(gameObject);
    }
}
