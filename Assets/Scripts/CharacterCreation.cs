using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class CharacterCreation : MonoBehaviour
{
    [SerializeField] GameObject avatarPanel;
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

    private User user;

    private void OnEnable() {
        StartCoroutine(BuildCharacter());
    }

    private void OnDisable() {
        nameInput.GetComponent<InputField>().text = "";
        nameInput.GetComponent<Outline>().enabled = false;
        infoText.gameObject.SetActive(false);
    }

    private IEnumerator BuildCharacter() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        avatarPanel.SetActive(true);
        user = new User();
        if (task.IsFaulted) {
            ErrorWithCharacterEdit("Failed to load profile");
        } else {
            user = JsonUtility.FromJson<User>(task.Result);
        }
        if (string.IsNullOrEmpty(user.userName)) {
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
        user.body = body[cbodyIndex].name;

        cface.sprite = face[cfaceIndex];
        user.face = face[cfaceIndex].name;

        chair.sprite = hair[chairIndex];
        user.hair = hair[chairIndex].name;

        ckit.sprite = kit[ckitIndex];
        user.kit = kit[ckitIndex].name;
    }

    private void BuildSavedCharacter() {
        //User user = FireBaseScript.GetCurrentUser();
        nameInput.GetComponent<InputField>().text = user.userName;
        
        ////set the body parts
        cbody.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        cface.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        chair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        ckit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;
    }

    //changes the hair to the next one
    public void NextHair() {
        if (chairIndex == hair.Length - 1) {
            chairIndex = 0;
        } else {
            chairIndex++;
        }
        chair.sprite = hair[chairIndex];
        user.hair = hair[chairIndex].name;
    }

    //changes the hair to the previous one
    public void PreviousHair() {
        if (chairIndex == 0) {
            chairIndex = hair.Length - 1;
        } else {
            chairIndex--;
        }
        chair.sprite = hair[chairIndex];
        user.hair = hair[chairIndex].name;
    }

    //changes the face to the next one
    public void NextFace() {
        if (cfaceIndex == face.Length - 1) {
            cfaceIndex = 0;
        } else {
            cfaceIndex++;
        }
        cface.sprite = face[cfaceIndex];
        user.face = face[cfaceIndex].name;
    }

    //changes the face to the previous face
    public void PreviousFace() {
        if (cfaceIndex == 0) {
            cfaceIndex = face.Length - 1;
        } else {
            cfaceIndex--;
        }
        cface.sprite = face[cfaceIndex];
        user.face = face[cfaceIndex].name;
    }

    //change the clothes to the next clothes
    public void NextClothes() {
        if (ckitIndex == kit.Length - 1) {
            ckitIndex = 0;
        } else {
            ckitIndex++;
        }
        ckit.sprite = kit[ckitIndex];
        user.face = kit[ckitIndex].name;
    }

    //changes the clothes to the previous clothes
    public void PreviousClothes() {
        if (ckitIndex == 0) {
            ckitIndex = kit.Length - 1;
        } else {
            ckitIndex--;
        }
        ckit.sprite = kit[ckitIndex];
        user.kit = kit[ckitIndex].name;
    }

    //changes the body type to the next body
    public void NextBody() {
        if (cbodyIndex == body.Length - 1) {
            cbodyIndex = 0;
        } else {
            cbodyIndex++;
        }
        cbody.sprite = body[cbodyIndex];
        user.body = body[cbodyIndex].name;
    }

    //changes the body type to previous sprite
    public void PreviousBody() {
        if (cbodyIndex == 0) {
            cbodyIndex = body.Length - 1;
        } else {
            cbodyIndex--;
        }
        cbody.sprite = body[cbodyIndex];
        user.body = body[cbodyIndex].name;
    }

    public void SavePlayer() {
        string playerName = nameInput.GetComponent<InputField>().text;
        if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrWhiteSpace(playerName)) {
            StartCoroutine(NameCheck(playerName));
        } else {
            ErrorWithCharacterEdit("Please enter a username");
            NameError();
        }
    }

    private IEnumerator NameCheck(string userName) {
        var task = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            ErrorWithCharacterEdit("Failed to save character");
        } else {
            bool nameBool = false;
            foreach (DataSnapshot snap in task.Result.Children) {
                User tempUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                if (tempUser.userName == userName) {
                    if (tempUser.userID != FireBaseScript.AuthenitcationKey()) {
                        nameBool = true;
                        break;
                    }
                }
            }
            if (!nameBool) {
                user.userName = userName;
                FireBaseScript.UpdateUser(user);
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            } else {
                ErrorWithCharacterEdit("Username is taken");
                NameError();
            }
        }
    }

    public void ErrorWithCharacterEdit(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }

    private void NameError() {
        nameInput.GetComponent<Outline>().enabled = true;
    }

}
