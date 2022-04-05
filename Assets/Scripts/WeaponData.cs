using UnityEngine;

[CreateAssetMenu(fileName = "weaponData", menuName ="weaponData")]
public class WeaponData : ScriptableObject
{
    //number of shoot per seconds
    // auto slow = 4, sniper = 0.4f
    // let's do the mp40
    // https://medalofhonor.fandom.com/wiki/MP40
    public string weaponName = "";
    public float fireRate = 11.6f;
    [HideInInspector]
    public float currentRate = 0;
    public float damage = 25; //x1.5 in the head
    public float headMultiplier = 1.5f;
    public int magazineSize = 32;
    [HideInInspector]
    public int currentMagazineSize = 32;
    public int ammoPossible = 32;
    public float reloadTime = 3;
    [HideInInspector]
    public float currentReloadTime = 0;
    public float range = 500;
    public bool isSniper = false;

    public WeaponData(ref WeaponData reference)
    {
        weaponName = reference.weaponName;
        fireRate = reference.fireRate;
        currentRate = reference.currentRate;
        damage = reference.damage;
        headMultiplier = reference.headMultiplier;
        magazineSize = reference.magazineSize;
        ammoPossible = reference.ammoPossible;
        reloadTime = reference.reloadTime;
        range = reference.range;
        isSniper = reference.isSniper;
    }
}
