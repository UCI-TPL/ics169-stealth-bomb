﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public float selectedScale = 1.2f;

    private Button b;

    void Awake() {
        b = this.gameObject.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData) {
        b.transform.localScale = new Vector3(selectedScale, selectedScale, b.transform.localScale.z);
    }

    public void OnDeselect(BaseEventData eventData) {
        b.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
