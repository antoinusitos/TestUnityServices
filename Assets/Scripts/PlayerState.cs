using System;
using Unity.Netcode;

[Serializable]
public class PlayerState : INetworkSerializable
{
    public ulong clientID;
    public string clientGUID;
    public string playerName;
    public int deaths;
    public int kills;
    public int team;
    public WeaponData weaponData;

    public PlayerState()
    {
        clientID = 0;
        clientGUID = "";
        playerName = "";
        deaths = 0;
        kills = 0;
        team = 0;
        weaponData = null;
    }

    public PlayerState(ulong clientID_In = 0, string clientGUID_In = "", string playerName_In = "", int deaths_In = 0, int kills_In = 0, int team_In = 0, WeaponData weapon_In = null)
    {
        clientID = clientID_In;
        clientGUID = clientGUID_In;
        playerName = playerName_In;
        deaths = deaths_In;
        kills = kills_In;
        team = team_In;
        weaponData = weapon_In;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref clientGUID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref deaths);
        serializer.SerializeValue(ref kills);
        serializer.SerializeValue(ref team);
    }
}
