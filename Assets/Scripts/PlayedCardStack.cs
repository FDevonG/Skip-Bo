using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayedCardStack : MonoBehaviour {

    private Sounds sounds;
    public int currentCardValue = 0;//a int to store the current card value
    public int maxStackSize = 12;

    GameControl gameControl;
    GameSetup gameSetup;

    PhotonView photonView;

    private void Start() {
        gameSetup = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameSetup>();
        sounds = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<Sounds>();
        photonView = GetComponentInParent<PhotonView>();
        gameControl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>();
    }

    public void AddCardToStack() {
        if (PhotonNetwork.inRoom) {
            if (!PhotonNetwork.isMasterClient) {
                return;
            }
        }
        currentCardValue += 1;
        StackFull();
    }

    public bool StackFull() {
        bool full = false;
        if (currentCardValue >= maxStackSize) {
            full = true;
        }
        return full;
    }

    public IEnumerator ResetStack() {
        yield return new WaitForSeconds(1.0f);
        List<Transform> cardObjects = transform.Cast<Transform>().ToList();
        if (!gameControl.playerWon) {
            int countNumber = cardObjects.Count - 1;
            for (int i = countNumber; i >= 0; i--) {
                cardObjects[i].GetComponent<CardLerp>().StartLerping(gameSetup.canvas.transform, gameControl.cardSpawnLocation);//tell the card to lerp off screen
                sounds.PlayCardFlip(cardObjects[i].GetComponent<AudioSource>());
                StartCoroutine(DestroyCard(cardObjects[i].gameObject));//destroy the card
                cardObjects.Remove(cardObjects[i]);
                yield return new WaitForSeconds(0.15f);
            }
            //if (PhotonNetwork.inRoom) {
            //    if (!PhotonNetwork.isMasterClient) {
            //        yield return null;
            //    }
            //}
            currentCardValue = 0;
        }
    }

    IEnumerator DestroyCard(GameObject card) {
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.Destroy(card);
        } 
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(currentCardValue);
        } else {
            currentCardValue = (int)stream.ReceiveNext();
        }
    }
}
