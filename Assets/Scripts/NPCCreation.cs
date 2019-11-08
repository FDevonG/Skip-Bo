﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCreation : MonoBehaviour
{
    public List<Sprite> maleHair = new List<Sprite>();
    public List<Sprite> femaleHair = new List<Sprite>();

    public List<Sprite> maleClothes = new List<Sprite>();
    public List<Sprite> femaleClothes = new List<Sprite>();

    public List<Sprite> face = new List<Sprite>();
    public List<Sprite> body = new List<Sprite>();

    public List<string> boyNames = new List<string>();
    public List<string> girlNames = new List<string>();

    public PhotonPlayer CreateNpc(int actorID) {
        
        Player player = new Player();
        int random = Random.Range(0, 2);
        if (random == 0) {
            player = new Player(boyNames[Random.Range(0, boyNames.Count)], maleHair[Random.Range(0, maleHair.Count)].name, face[Random.Range(0, face.Count)].name, maleClothes[Random.Range(0, maleClothes.Count)].name, body[Random.Range(0, body.Count)].name, "");
        }
        if (random == 1) {
            player = new Player(girlNames[Random.Range(0, girlNames.Count)], femaleHair[Random.Range(0, femaleHair.Count)].name, face[Random.Range(0, face.Count)].name, femaleClothes[Random.Range(0, femaleClothes.Count)].name, body[Random.Range(0, body.Count)].name, "");
        }

        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "name", player.Name }, { "hair", player.Hair }, { "face", player.Face }, { "kit", player.Kit }, { "body", player.Body } };
        PhotonPlayer photonPlayer = new PhotonPlayer(true, actorID, customProperties);
        return photonPlayer;
    }
}
