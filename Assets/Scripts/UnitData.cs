using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitData", order = 1)]
public class UnitData : ScriptableObject
{
    [Header("Level")]
    public string shipLvl;
    public int lvlUpCost;
    [Space(20)]

    [Header("Move Speed")]
    public float moveSpeed;
    public int maxUpgradeMS;
    public float amountUpgradedMS;
    public int upgradeCostMS;
    [Space(20)]

    [Header("Health Points")]
    public float healthPoints;
    public int maxUpgradeHP;
    public float amountUpgradedHP;
    public int upgradeCostHP;
    [Space(20)]

    [Header("Attack Power")]
    public float attackPower;
    public int maxUpgradeAttack;
    public float amountUpgradedAttack;
    public int upgradeCostAttack;
    [Space(20)]

    [Header("Reload Time")]
    public float reloadTime;
    public int maxUpgradeReload;
    public float amountUpgradedReload;
    public int upgradeCostReload;
    [Space(20)]

    [Header("Shot Strength")]
    public float shotStrength;
    public int maxUpgradeSS;
    public float amountUpgradedSS;
    public int upgradeCostStrength;
    [Space(20)]

    [Header("Other")]
    public float shotOffset;
    public int worthCoins;
    public float strengthOffset;
}
