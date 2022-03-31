using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    private Transform cameraPlayer = null;

    public PlayerUI playerUI = null;

    public WeaponData weaponData = null;

    private bool canShoot = true;

    private bool reloading = false;

    private void Start()
    {
        if (!IsOwner)
            return;

        cameraPlayer = transform.GetChild(0).GetChild(0);

        weaponData.currentMagazineSize = weaponData.magazineSize;
        playerUI.UpdateMagazineSize(weaponData.currentMagazineSize, weaponData.magazineSize);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if(!canShoot)
        {
            weaponData.currentRate += Time.deltaTime * weaponData.fireRate;
            if(weaponData.currentRate >= 1)
            {
                weaponData.currentRate = 0;
                canShoot = true;
            }
        }

        if(reloading)
        {
            weaponData.currentReloadTime += Time.deltaTime;
            if(weaponData.currentReloadTime >= weaponData.reloadTime)
            {
                weaponData.currentReloadTime = 0;
                reloading = false;
                weaponData.currentMagazineSize = weaponData.magazineSize;
                playerUI.UpdateMagazineSize(weaponData.currentMagazineSize, weaponData.magazineSize);
            }
        }

        if(Input.GetMouseButton(0) && !reloading && canShoot)
        {
            weaponData.currentMagazineSize--;
            playerUI.UpdateMagazineSize(weaponData.currentMagazineSize, weaponData.magazineSize);
            canShoot = false;
            FireServerRPC(cameraPlayer.position, cameraPlayer.forward, weaponData.range, weaponData.damage, OwnerClientId);

            if (weaponData.currentMagazineSize == 0)
            {
                reloading = true;
            }
        }
    }

    [ServerRpc]
    private void FireServerRPC(Vector3 pos, Vector3 dir, float range, float damage, ulong senderID)
    {
        Debug.DrawRay(pos, dir * range, Color.red, 50);
        Debug.Log("shoot");
        if (Physics.Raycast(pos, dir * range, out RaycastHit hit))
        {
            PlayerHealth playerHealth = hit.transform.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, playerHealth.OwnerClientId, senderID);
            }
        }
    }
}
