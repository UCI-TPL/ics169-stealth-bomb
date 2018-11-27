using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour {

    public float timer;
    [HideInInspector]
    public float hitCooldown = 0.2f;
    [HideInInspector]
    public GameObject IgnoreCollision;
    [HideInInspector]
    public object source;

    public delegate void HitAction(Vector3 position, GameObject target);
    public HitAction OnHit;

    private static Dictionary<object, Queue<CooldownObject>> cooldownQueue = new Dictionary<object, Queue<CooldownObject>>();
    private static Dictionary<object, HashSet<GameObject>> cooldownSet = new Dictionary<object, HashSet<GameObject>>();

    // Use this for initialization
    void Start() {
        Destroy(gameObject, timer);
        if (!cooldownQueue.ContainsKey(source))
            cooldownQueue.Add(source, new Queue<CooldownObject>());
        if (!cooldownSet.ContainsKey(source))
            cooldownSet.Add(source, new HashSet<GameObject>());
    }

    private void Update() {
        while (cooldownQueue[source].Count > 0 && cooldownQueue[source].Peek().endTime <= Time.time)
            cooldownSet[source].Remove(cooldownQueue[source].Dequeue().gameObject);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject != IgnoreCollision && !cooldownSet[source].Contains(other.gameObject)) {
            cooldownSet[source].Add(other.gameObject);
            cooldownQueue[source].Enqueue(new CooldownObject(other.gameObject, hitCooldown));
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
