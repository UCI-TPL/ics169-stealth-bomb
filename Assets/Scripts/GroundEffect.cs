using ColorExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class GroundEffect : MonoBehaviour {
    
    [HideInInspector]
    public float HitCooldown { get; private set; }
    [HideInInspector]
    public Player Source { get; private set; }
    public Collider hitBox;
    private List<Collider> IgnoreCollision;
    [SerializeField]
    private new MeshRenderer[] renderer;
    private float endTime;

    [SerializeField]
    private LayerMask GroundMask;

    [SerializeField]
    private Type type;

    [SerializeField]
    private static Dictionary<Type, Material> defaultMaterial = new Dictionary<Type, Material>();
    private static Dictionary<Type, Material[]> _materialPerPlayer = new Dictionary<Type, Material[]>();
    private Material[] materialPerPlayer {
        get {
            if (_materialPerPlayer.ContainsKey(type))
                return _materialPerPlayer[type];
            _materialPerPlayer.Add(type, new Material[GameManager.instance.DefaultPlayerData.Colors.Length]);
            for (int i = 0; i < GameManager.instance.DefaultPlayerData.Colors.Length; ++i) {
                Material m = new Material(defaultMaterial[type]);
                m.SetColor("_TopColor", GameManager.instance.DefaultPlayerData.Colors[i].ScaleHSV(defaultMaterial[type].GetColor("_TopColor"), true));
                m.SetColor("_MiddleColor", GameManager.instance.DefaultPlayerData.Colors[i].ScaleHSV(defaultMaterial[type].GetColor("_MiddleColor"), true));
                m.SetColor("_BottomColor", GameManager.instance.DefaultPlayerData.Colors[i].ScaleHSV(defaultMaterial[type].GetColor("_BottomColor"), true));
                _materialPerPlayer[type][i] = m;
            }
            return _materialPerPlayer[type];
        }
    }

    public delegate void HitAction(Vector3 position, GameObject target);
    public HitAction OnHit;

    private static Transform parent;
    private readonly static Queue<GroundEffect> ObjectPool = new Queue<GroundEffect>();

    private readonly static Dictionary<object, int> ActiveSource = new Dictionary<object, int>();
    private readonly static Dictionary<object, Queue<CooldownObject>> CooldownQueue = new Dictionary<object, Queue<CooldownObject>>();
    private readonly static Dictionary<object, HashSet<GameObject>> CooldownSet = new Dictionary<object, HashSet<GameObject>>();

    private readonly static Vector3 planeVector = new Vector3(1, 0, 1);

    // Use this for initialization
    public void Create(HitAction onHit, Player source, Vector3 location, float size, float duration, float hitCooldown, List<Collider> ignoreCollision = null) {
        if (!defaultMaterial.ContainsKey(type))
            defaultMaterial.Add(type, renderer[0].sharedMaterial);

        if (!ActiveSource.ContainsKey(source)) {
            CooldownQueue.Add(source, new Queue<CooldownObject>());
            CooldownSet.Add(source, new HashSet<GameObject>());
            ActiveSource.Add(source, 0);
        }
        RaycastHit hit;
        if (!Physics.Raycast(location + Vector3.up * 0.25f, Physics.gravity, out hit, 1.25f, GroundMask, QueryTriggerInteraction.Ignore))
            return;
        location = hit.point;

        GroundEffect newInstance;
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
            newInstance = Instantiate<GameObject>(gameObject, location, Quaternion.identity, parent).GetComponent<GroundEffect>();
        }
        ++ActiveSource[source];
        newInstance.gameObject.SetActive(true);
        newInstance.hitBox.enabled = true;
        newInstance.StartCoroutine(newInstance.StartAnimation(size, 0.25f));
        foreach(Renderer r in newInstance.renderer)
            r.sharedMaterial = materialPerPlayer[source.colorIndex];
        newInstance.OnHit = onHit;
        newInstance.Source = source;
        newInstance.HitCooldown = hitCooldown;
        newInstance.IgnoreCollision = ignoreCollision;
        //if (ignoreCollision != null) {
        //    foreach (Collider c in ignoreCollision)
        //        if (c != null)
        //            Physics.IgnoreCollision(c, newInstance.hitBox);
        //}

        newInstance.endTime = Time.time + duration;
    }

    private void DurationEnd() {
        endTime = 0;
        StopAllCoroutines();
        if (IgnoreCollision != null)
            foreach (Collider c in IgnoreCollision)
                if (c != null)
                    Physics.IgnoreCollision(c, hitBox, false);
        hitBox.enabled = false;
        StartCoroutine(DestroyAnimation(0.25f));
    }

    private IEnumerator StartAnimation(float scale, float duration) {
        Vector3 velocity = planeVector * scale / duration;
        transform.localScale = Vector3.up;
        while (transform.localScale.x < scale) {
            transform.localScale += velocity * Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.up + planeVector * scale;
    }

    private IEnumerator DestroyAnimation(float duration) {
        Vector3 velocity = (transform.localScale / duration).Scaled(planeVector);
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
        if (endTime != 0 && endTime < Time.time)
            DurationEnd();
        while (CooldownQueue[Source].Count > 0 && CooldownQueue[Source].Peek().endTime <= Time.time)
            CooldownSet[Source].Remove(CooldownQueue[Source].Dequeue().gameObject);
    }

    private void OnTriggerStay(Collider other) {
        Player p = other.GetComponentInParent<PlayerController>()?.player;
        if (p != null && p != Source && !CooldownSet[Source].Contains(other.gameObject)) {
            CooldownSet[Source].Add(other.gameObject);
            CooldownQueue[Source].Enqueue(new CooldownObject(other.gameObject, HitCooldown));
            OnHit(transform.position, other.gameObject);
        }
    }

    // Reset pool of objects when round resets, prevents null pointers
    private void OnDestroy() {
        if (ObjectPool.Count > 0)
            ObjectPool.Clear();
    }

    private struct CooldownObject {
        public GameObject gameObject;
        public float endTime;

        public CooldownObject(GameObject gameObject, float cooldown) {
            this.gameObject = gameObject;
            endTime = Time.time + cooldown;
        }
    }

    private enum Type { fire, ice }
}
