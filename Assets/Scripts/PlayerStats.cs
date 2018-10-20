using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    private Player _player;
    public Player player {
        get { return _player; }
        private set { _player = value; }
    }

    private Dictionary<string, float> stats = new Dictionary<string, float>();

    private void Awake() {
        stats.Add("move_speed", 10);
        stats.Add("air_speed", 3);
        stats.Add("jump_force", 20);
        stats.Add("max_health", 100);
        player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
    }

    public float moveSpeed {
        get { return getStat("move_speed"); }
    }
    public float jumpForce {
        get { return getStat("jump_force"); }
    }
    public float maxHealth {
        get { return getStat("max_health"); }
    }

    public float airSpeed
    {
        get { return getStat("air_speed");  }
    }

    private float getStat(string name) { // Attempts to grab a stat, if that stat does not exist display error
        if (stats.ContainsKey(name))
            return stats[name];
        Debug.LogError(gameObject.name + " does not contain a stat named \"" + name + "\"");
        return 0;
    }
}
