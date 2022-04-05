using Unity.Netcode;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    private Transform cameraPlayer = null;

    public PlayerUI playerUI = null;

    public WeaponData weaponData = null;

    private bool canShoot = true;

    private bool reloading = false;

    public PlayerAnimationReplication AnimationReplication = null;

    public Animator animator = null;

    private Player player = null;

    private void Start()
    {
        if (!IsOwner)
            return;

        player = GetComponent<Player>();
        cameraPlayer = transform.GetChild(0).GetChild(0);

        weaponData = PlayerInfos.instance.currentPlayerState.weaponData;

        weaponData.currentMagazineSize = weaponData.magazineSize;
        playerUI.UpdateMagazineSize(weaponData.currentMagazineSize, weaponData.ammoPossible);
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
                int ammoToGet = weaponData.magazineSize < weaponData.ammoPossible ? weaponData.magazineSize : weaponData.ammoPossible;
                weaponData.ammoPossible -= ammoToGet;
                weaponData.currentMagazineSize = ammoToGet;
                playerUI.UpdateMagazineSize(weaponData.currentMagazineSize, weaponData.ammoPossible);
            }
        }

        if(player.GetIsInPause())
        {
            return;
        }

        if(Input.GetMouseButton(0) && !reloading && canShoot)
        {
            weaponData.currentMagazineSize--;
            playerUI.UpdateMagazineSize(weaponData.currentMagazineSize, weaponData.ammoPossible);
            canShoot = false;
            FireServerRPC(cameraPlayer.position, cameraPlayer.forward, weaponData.range, weaponData.damage, OwnerClientId);

            if (weaponData.currentMagazineSize == 0)
            {
                reloading = true;
                animator.SetTrigger("Reload");
                AnimationReplication.UpdateReload();
            }
        }
    }

    [ServerRpc]
    private void FireServerRPC(Vector3 pos, Vector3 dir, float range, float damage, ulong senderID)
    {
        Debug.DrawRay(pos, dir * range, Color.red, 50);
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
