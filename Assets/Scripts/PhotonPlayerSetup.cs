
public static class PhotonPlayerSetup
{
    public static void BuildPhotonPlayer(PhotonPlayer photonPlayer, User user) {
        photonPlayer.NickName = user.userID;
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "name", user.userName }, { "hair", user.hair }, { "face", user.face }, { "kit", user.kit }, { "body", user.body } };
        PhotonNetwork.player.UserId = user.userID;
        photonPlayer.SetCustomProperties(customProperties);
    }
}
