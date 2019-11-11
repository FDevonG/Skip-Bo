
public static class PhotonPlayerSetup
{
    public static void BuildPhotonPlayer(PhotonPlayer photonPlayer, Player player) {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "name", player.Name }, { "hair", player.Hair }, { "face", player.Face }, { "kit", player.Kit }, { "body", player.Body }, { "id", player.UniqueID } };
        photonPlayer.UserId = player.UniqueID;
        photonPlayer.SetCustomProperties(customProperties);
    }
}
