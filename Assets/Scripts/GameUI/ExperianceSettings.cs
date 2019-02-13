using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Experiance Settings", menuName = "Settings/Experiance Settings",order = 0)]
public class ExperianceSettings : ScriptableObject {

    [SerializeField]
    private GameManager.GameRound.BonusExperiance[] ExperianceTypes;
    // Order exp is calculated
    public readonly GameManager.GameRound.BonusExperiance.ExperianceType[] ExperianceOrder = { GameManager.GameRound.BonusExperiance.ExperianceType.Damage,
        GameManager.GameRound.BonusExperiance.ExperianceType.Kill, GameManager.GameRound.BonusExperiance.ExperianceType.Revenge,
        GameManager.GameRound.BonusExperiance.ExperianceType.LastOneStanding, GameManager.GameRound.BonusExperiance.ExperianceType.KillLeader,
        GameManager.GameRound.BonusExperiance.ExperianceType.Comeback, GameManager.GameRound.BonusExperiance.ExperianceType.NoDamageTaken };

    public GameManager.GameRound.BonusExperiance GetExperiance(GameManager.GameRound.BonusExperiance.ExperianceType type) {
        return ExperianceTypes[(int)type];
    }
}
