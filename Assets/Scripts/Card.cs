using UnityEngine;
using UnityEngine.UI;

public class Card : Photon.MonoBehaviour {
    //PhotonView photonView;

    //colors
    Color blueColor = new Color(0, 0.4117647f, 0.7372549f, 1);
    Color greenColor = new Color(0f, 0.6156863f, 0.01568628f, 1);
    Color redColor = new Color(0.9294118f, 0.1254902f, 0.1843137f, 1);
    Color orangeColor = new Color(0.9921569f, 0.7058824f, 0, 1);

    //public Sprite cardMask;

    [SerializeField] Text skipboText;
    [SerializeField] Text[] cardTexts = new Text[2];
    [SerializeField] GameObject cardColor;//this is the part thats makes up the colored background of the card

    public int cardNumber;//stores the number of the card
    public int handSlot = 100;//tells other players over the network what hand slot this card belongs in

    public void SetUpCard(int number) {
        //get the needed components from the card prefab to wire everything together
        //cardColor.SetActive(true);
        //GetComponent<Image>().sprite = cardMask;
        cardNumber = number;//store the number of the card for reference later
        skipboText.gameObject.SetActive(false);
        //change the color of the cards
        if (number >= 1 && number < 5) {
            cardColor.GetComponent<Image>().color = blueColor;
        } else if (number >= 5 && number < 9) {
            cardColor.GetComponent<Image>().color = greenColor;
        } else if (number >= 9 && number < 13) {
            cardColor.GetComponent<Image>().color = redColor;
        } else if (number == 100) {
            cardColor.GetComponent<Image>().color = orangeColor;
        }
        foreach (var text in cardTexts) {
            text.gameObject.SetActive(true);
            if (number != 100) {
                text.text = number.ToString();
            } else if (number == 100) {
                text.text = "?";
            }

        }
    }

    public void SetUpWildCard(int currentCardValue) {
        cardTexts[0].text = currentCardValue.ToString();
        cardTexts[1].text = currentCardValue.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(cardNumber);
            stream.SendNext(handSlot);
        } else {
            cardNumber = (int)stream.ReceiveNext();
            handSlot = (int)stream.ReceiveNext();
        }
    }

}
