using System.Collections.Generic;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    public Image cbody;//used to display the body
    public Image cface;//used to display the face
    public Image chair;//used to display the hair
    public Image ckit;//used to display the clothes
    public Sprite[] body;//stores the body types
    public Sprite[] face;//used to display the face types
    public Sprite[] hair;//used to display the hair types
    public Sprite[] kit;//used to display the clothes types

    public GameObject nameInput;

    [SerializeField] Text infoText;

    public int cbodyIndex = 0;
    public int cfaceIndex = 0;
    public int chairIndex = 0;
    public int ckitIndex = 0;

    public Button startMenuCancelButton;

    private void OnEnable() {
        StartCoroutine(FireBaseScript.GetCurrentUser());
        if (string.IsNullOrEmpty(CurrentUser.curUser.userName)) {
            RandomizeCharacter();
            startMenuCancelButton.gameObject.SetActive(false);
        } else {
            BuildSavedCharacter();
            startMenuCancelButton.gameObject.SetActive(true);
        }
    }

    public void RandomizeCharacter() {
        //get random index of all the body parts and store them
        cbodyIndex = Random.Range(0, body.Length);
        cfaceIndex = Random.Range(0, face.Length);
        chairIndex = Random.Range(0, hair.Length);
        ckitIndex = Random.Range(0, kit.Length);

        //set the body parts
        cbody.sprite = body[cbodyIndex];
        cface.sprite = face[cfaceIndex];
        chair.sprite = hair[chairIndex];
        ckit.sprite = kit[ckitIndex];
    }

    private void BuildSavedCharacter() {
        StartCoroutine(FireBaseScript.GetCurrentUser());
        nameInput.GetComponent<InputField>().text = CurrentUser.curUser.userName;
        
        ////set the body parts
        cbody.sprite = Resources.Load<Sprite>("Faces/Bodies/" + CurrentUser.curUser.avatar["body"]) as Sprite;
        cface.sprite = Resources.Load<Sprite>("Faces/Faces/" + CurrentUser.curUser.avatar["face"]) as Sprite;
        chair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + CurrentUser.curUser.avatar["hair"]) as Sprite;
        ckit.sprite = Resources.Load<Sprite>("Faces/Kits/" + CurrentUser.curUser.avatar["kit"]) as Sprite;
    }

    //changes the hair to the next one
    public void NextHair() {
        if (chairIndex == hair.Length - 1) {
            chairIndex = 0;
        } else {
            chairIndex++;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the hair to the previous one
    public void PreviousHair() {
        if (chairIndex == 0) {
            chairIndex = hair.Length - 1;
        } else {
            chairIndex--;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the face to the next one
    public void NextFace() {
        if (cfaceIndex == face.Length - 1) {
            cfaceIndex = 0;
        } else {
            cfaceIndex++;
        }
        cface.sprite = face[cfaceIndex];
    }

    //changes the face to the previous face
    public void PreviousFace() {
        if (cfaceIndex == 0) {
            cfaceIndex = face.Length - 1;
        } else {
            cfaceIndex--;
        }
        cface.sprite = face[cfaceIndex];
    }

    //change the clothes to the next clothes
    public void NextClothes() {
        if (ckitIndex == kit.Length - 1) {
            ckitIndex = 0;
        } else {
            ckitIndex++;
        }
        ckit.sprite = kit[ckitIndex];
    }

    //changes the clothes to the previous clothes
    public void PreviousClothes() {
        if (ckitIndex == 0) {
            ckitIndex = kit.Length - 1;
        } else {
            ckitIndex--;
        }
        ckit.sprite = kit[ckitIndex];
    }

    //changes the body type to the next body
    public void NextBody() {
        if (cbodyIndex == body.Length - 1) {
            cbodyIndex = 0;
        } else {
            cbodyIndex++;
        }
        cbody.sprite = body[cbodyIndex];
    }

    //changes the body type to previous sprite
    public void PreviousBody() {
        if (cbodyIndex == 0) {
            cbodyIndex = body.Length - 1;
        } else {
            cbodyIndex--;
        }
        cbody.sprite = body[cbodyIndex];
    }

    public void SavePlayer() {
        string playerName = nameInput.GetComponent<InputField>().text;
        if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrWhiteSpace(playerName)) {
            if (!FireBaseScript.DoesUserNameExist(playerName)) {
                Dictionary<string, string> avatar = BuildAvatarDictionary();
                FireBaseScript.UpdateUserAvatar(avatar);
                FireBaseScript.UpdateUserName(playerName);
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            } else {
                ErrorWithCharacterEdit("Username is taken");
            }
        } else {
            ErrorWithCharacterEdit("Please enter a username");
        }
    }

    public void ErrorWithCharacterEdit(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
        nameInput.GetComponent<Outline>().enabled = true;
    }

    private Dictionary<string, string> BuildAvatarDictionary() {
        Dictionary<string, string> avatar = new Dictionary<string, string>();
        avatar.Add("hair", hair[chairIndex].name);
        avatar.Add("face", face[cfaceIndex].name);
        avatar.Add("kit", kit[ckitIndex].name);
        avatar.Add("body", body[cbodyIndex].name);
        return avatar;
    }

}
