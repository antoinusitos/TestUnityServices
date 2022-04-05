using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelection : NetworkBehaviour
{
    public WeaponData[] weaponDatas = null;

    public Transform weaponPanel = null;

    public Button weaponButtonPrefab = null;

    private void Start()
    {
        if (!IsOwner)
            return;
            
        for(int i = 0; i < weaponDatas.Length; i++)
        {
            Button b = Instantiate(weaponButtonPrefab, weaponPanel);
            WeaponData local = weaponDatas[i];
            b.GetComponentInChildren<Text>().text = local.weaponName;
            b.onClick.AddListener(delegate
            {
                WeaponData used = new WeaponData(ref local);
                PlayerInfos.instance.currentPlayerState.weaponData = used;
                FindObjectOfType<ObserverCamera>().ReadyToSpawn();
            });
        }
    }
}
