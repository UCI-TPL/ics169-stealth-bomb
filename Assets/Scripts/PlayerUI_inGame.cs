using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI_inGame : MonoBehaviour {
    // Required Variables for the Player's UI stuff.
    private PlayerController playerCon;                         // Player Controller already gets the Player object for us, so just use that one.
    [Header("HP Bar")]
    public RectTransform playerUI_HPCanvas;
    public UnityEngine.UI.Image playerUI_healthBar;

    [Header("Aiming Arrow")]
    public UnityEngine.UI.Image playerUI_AimArrowMaskL;
    public UnityEngine.UI.Image playerUI_AimArrowMaskR;
    public UnityEngine.UI.Image playerUI_AimArrowL;
    public UnityEngine.UI.Image playerUI_AimArrowR;

    [Header("Overhead Text Notificiation")]
    public RectTransform playerUI_TextCanvas;
    public UnityEngine.UI.Text playerUI_NotifText;
    public float showForSeconds = 1.25f;

    [Header("Player Position Marker")]
    [SerializeField] private PositionMarker positionMarker;

    // flags for rendering player UI
    private bool HP_CoroutineActive = false;
    private bool notifTextActive = false;
    private Transform cameraTransform;

    //  UNITY METHODS
    void Start()
    {
        playerCon = gameObject.GetComponent<PlayerController>();
        // To have HP bar render all the time remove this code, as well as code in the HurtPlayer method in Player.cs
        playerUI_HPCanvas.gameObject.SetActive(false);
        cameraTransform = Camera.main.transform;
        playerCon.player.OnAddPowerUp += powerupHandler;

        positionMarker.color = playerCon.player.Color;
    }

    void LateUpdate()
    {
        if (!playerUI_HPCanvas)
            Debug.Log("uoo");
        playerUI_HPCanvas.gameObject.SetActive(playerCon.player.health < playerCon.player.stats.maxHealth);
        playerUI_HPCanvas.rotation = Quaternion.Euler(90f, cameraTransform.rotation.eulerAngles.y, 0f);
        playerUI_healthBar.fillAmount = playerCon.player.health / playerCon.player.stats.maxHealth;
        playerUI_TextCanvas.rotation = Quaternion.Euler(50f, cameraTransform.rotation.eulerAngles.y, 0f);

        if (playerCon.Weapon.type.Equals(Weapon.Type.Instant))
        {
            playerUI_AimArrowL.color = playerUI_AimArrowR.color = Color.cyan;       //#87c8e1
        }
        else if (!playerCon.Weapon.type.Equals(Weapon.Type.Instant))
        {
            playerUI_AimArrowL.color = playerUI_AimArrowR.color = Color.white;
        }

        playerUI_AimArrowMaskL.fillAmount = playerUI_AimArrowMaskR.fillAmount  = playerCon.Weapon.ChargeLevel;
    }

    void OnDestroy()
    {
        playerCon.player.OnAddPowerUp -= powerupHandler;
    }

    //  METHODS
    public void powerupHandler(PowerupData powerupData, Buff buff)
    {
        if (notifTextActive)
            StopAllCoroutines();        // Should be fine. change if more coroutines get added.
        StartCoroutine(notifTextRoutine(powerupData));
    }

    // COROUTINES
    IEnumerator notifTextRoutine(PowerupData pData)
    {
        notifTextActive = true;
        playerUI_NotifText.text = "Got Power-up!\n" + pData.name;
        yield return new WaitForSeconds(showForSeconds);
        playerUI_NotifText.text = "";
        notifTextActive = false;
    }
}
