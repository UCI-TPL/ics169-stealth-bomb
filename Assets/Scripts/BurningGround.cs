﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningGround : MonoBehaviour {
    
    [HideInInspector]
    public float HitCooldown { get; private set; }
    [HideInInspector]
    public object Source { get; private set; }
    public Collider hitBox;
    private Collider IgnoreCollision;
    [SerializeField]
    private new MeshRenderer renderer;
    private float endTime;

    [SerializeField]
    private Material[] materialPerPlayer;

    public delegate void HitAction(Vector3 position, GameObject target);
    public HitAction OnHit;

    private static Transform parent;
    private readonly static Queue<BurningGround> ObjectPool = new Queue<BurningGround>();

    private readonly static Dictionary<object, int> ActiveSource = new Dictionary<object, int>();
    private readonly static Dictionary<object, Queue<CooldownObject>> CooldownQueue = new Dictionary<object, Queue<CooldownObject>>();
    private readonly static Dictionary<object, HashSet<GameObject>> CooldownSet = new Dictionary<object, HashSet<GameObject>>();

    // Use this for initialization
    public void Create(HitAction onHit, Player source, Vector3 location, float duration, float hitCooldown, Collider ignoreCollision = null) {
        if (!ActiveSource.ContainsKey(source)) {
            CooldownQueue.Add(source, new Queue<CooldownObject>());
            CooldownSet.Add(source, new HashSet<GameObject>());
            ActiveSource.Add(source, 0);
        }

        BurningGround newInstance;
        if (ObjectPool.Count > 0) {
            newInstance = ObjectPool.Dequeue();
            newInstance.gameObject.SetActive(true);
            newInstance.hitBox.enabled = true;
            newInstance.transform.position = location;
        }
        else {
            if (parent == null) {
                parent = new GameObject("BurningGroundObjects").transform;
                //parent.SetParent(GameManager.instance.PersistBetweenRounds);
            }
            newInstance = Instantiate<GameObject>(gameObject, location, Quaternion.identity, parent).GetComponent<BurningGround>();
        }
        ++ActiveSource[source];
        newInstance.gameObject.SetActive(true);
        newInstance.hitBox.enabled = true;
        newInstance.StartCoroutine(newInstance.StartAnimation(1, 0.25f));
        newInstance.renderer.sharedMaterial = materialPerPlayer[source.playerNumber];
        newInstance.OnHit = onHit;
        newInstance.Source = source;
        newInstance.HitCooldown = hitCooldown;
        newInstance.IgnoreCollision = ignoreCollision;
        if (ignoreCollision != null)
            Physics.IgnoreCollision(ignoreCollision, newInstance.hitBox);

        newInstance.endTime = Time.time + duration;
    }

    private void DurationEnd() {
        StopAllCoroutines();
        if (IgnoreCollision != null)
            Physics.IgnoreCollision(IgnoreCollision, hitBox, false);
        hitBox.enabled = false;
        StartCoroutine(DestroyAnimation(0.25f));
    }

    private IEnumerator StartAnimation(float scale, float duration) {
        Vector3 velocity = Vector3.one * scale / duration;
        transform.localScale = Vector3.zero;
        while (transform.localScale.x < scale) {
            transform.localScale += velocity * Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * scale;
    }

    private IEnumerator DestroyAnimation(float duration) {
        Vector3 velocity = transform.localScale / duration;
        while (transform.localScale.x > 0) {
            transform.localScale -= velocity * Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        Requeue();
    }

    private void Requeue() {
        ObjectPool.Enqueue(this);
        if (ActiveSource.ContainsKey(Source) && --ActiveSource[Source] <= 0)
            Remove(Source);
    }

    public static void Remove(object source) {
        ActiveSource.Remove(source);
        CooldownQueue.Remove(source);
        CooldownSet.Remove(source);
    }

    private void Update() {
        if (endTime < Time.time)
            DurationEnd();
        while (CooldownQueue[Source].Count > 0 && CooldownQueue[Source].Peek().endTime <= Time.time)
            CooldownSet[Source].Remove(CooldownQueue[Source].Dequeue().gameObject);
    }

    private void OnTriggerStay(Collider other) {
        if (!CooldownSet[Source].Contains(other.gameObject)) {
            CooldownSet[Source].Add(other.gameObject);
            CooldownQueue[Source].Enqueue(new CooldownObject(other.gameObject, HitCooldown));
            OnHit(transform.position, other.gameObject);
        }
    }

    private struct CooldownObject {
        public GameObject gameObject;
        public float endTime;

        public CooldownObject(GameObject gameObject, float cooldown) {
            this.gameObject = gameObject;
            endTime = Time.time + cooldown;
        }
    }
}
