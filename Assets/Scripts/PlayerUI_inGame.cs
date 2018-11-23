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

    // flags for rendering player UI
    private bool HP_CoroutineActive = false;
    private Transform cameraTransform;

    void Awake()
    {
        playerCon = gameObject.GetComponent<PlayerController>();
        // To have HP bar render all the time remove this code, as well as code in the HurtPlayer method in Player.cs
        playerUI_HPCanvas.gameObject.SetActive(false);
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        playerUI_HPCanvas.gameObject.SetActive(playerCon.player.health < playerCon.player.stats.maxHealth);
        playerUI_HPCanvas.rotation = Quaternion.Euler(90f, cameraTransform.rotation.eulerAngles.y, 0f);
        playerUI_healthBar.fillAmount = playerCon.player.health / playerCon.player.stats.maxHealth;

        // Aiming Arrow Fill Amount
        playerUI_AimArrowMaskL.fillAmount = playerUI_AimArrowMaskR.fillAmount  = playerCon.Weapon.ChargeLevel;
    }
}
