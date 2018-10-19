using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player requires the GameObject to have a PlayerController and PlayerStats component
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerStats))]
public class Player : MonoBehaviour {

    private PlayerStats _playerStats;
    public PlayerStats playerStats {
        get { return _playerStats; }
    }
    private PlayerController _playerController;
    public PlayerController playerController {
        get { return _playerController; }
    }

    private void Awake() {
        _playerStats = GetComponent<PlayerStats>();
        if (playerStats == null) // Check to ensure PlayerStats component is present, since PlayerStats is a dependency this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing PlayerStats Component");
        _playerController = GetComponent<PlayerController>();
        if (playerController == null) // Check to ensure PlayerController component is present, since PlayerController is a dependency this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing PlayerController Component");
    }

    public void HurtPlayer(int damage) {
        playerStats.health -= damage;
    }

    private void CheckDeath() {
        if (playerStats.health <= 0) {
            //Debug.Log("D E A T H ");
            Destroy(gameObject);
        }
    }

    void Update() {
        CheckDeath();
    }
}
