using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Functions;
using System.Threading.Tasks;

public class CharacterCreation : MonoBehaviour
{
    public Image cbody;//used to display the body
    public Image cface;//used to display the face
    public Image chair;//used to display the hair
    public Image ckit;//used to display the clothes
    public Sprite[] body;//stores the body types
    public List<Sprite> face = new List<Sprite>();//used to display the face types
    public List<Sprite> hair = new List<Sprite>();//used to display the hair types
    public List<Sprite> kit = new List<Sprite>();//used to display the clothes types

    public GameObject nameInput;

    public int cbodyIndex = 0;
    public int cfaceIndex = 0;
    public int chairIndex = 0;
    public int ckitIndex = 0;

    public Button startMenuCancelButton;

    private float timeTakenDuringLerp = 1.0f;
    private float timeStartedLerping;
    private bool imagesLerping = false;
    private Color visibleColor = new Color(1,1,1,1);
    private Color invisibleColor = new Color(1,1,1,0);

    private void OnEnable() {
        BuildCharacter();
    }

    private void OnDisable() {
        nameInput.GetComponent<InputField>().text = "";
        chair.color = invisibleColor;
        cface.color = invisibleColor;
        ckit.color = invisibleColor;
        cbody.color = invisibleColor;
    }

    private void BuildCharacter() {
        AddUnlockedAssets();
        if (string.IsNullOrEmpty(LocalUser.locUser.userName)) {
            RandomizeCharacter();
            startMenuCancelButton.gameObject.SetActive(false);
        } else {
            BuildSavedCharacter(LocalUser.locUser);
            startMenuCancelButton.gameObject.SetActive(true);
        }
    }

    void AddUnlockedAssets()
    {
        //Add the hairs
        foreach (string itemName in LocalUser.locUser.unlockedHair)
        {
            bool itemAdded = false;
            foreach (Sprite sprite in hair)
            {
                if (string.Equals(itemName, sprite.name))
                {
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                hair.Add(Resources.Load<Sprite>(("Faces/Hairs/" + itemName)) as Sprite);
            }
        }

        //Add the Faces
        foreach (string itemName in LocalUser.locUser.unlockedFace)
        {
            bool itemAdded = false;
            foreach (Sprite sprite in face)
            {
                if (string.Equals(itemName, sprite.name))
                {
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                face.Add(Resources.Load<Sprite>(("Faces/Faces/" + itemName)) as Sprite);
            }
        }

        //add the kits
        foreach (string itemName in LocalUser.locUser.unlockedClothes)
        {
            bool itemAdded = false;
            foreach (Sprite sprite in kit)
            {
                if (string.Equals(itemName, sprite.name))
                {
                    itemAdded = true;
                    break;
                }
            }
            if (!itemAdded)
            {
                kit.Add(Resources.Load<Sprite>(("Faces/Kits/" + itemName)) as Sprite);
            }
        }
    }

    public void RandomizeCharacter() {
        //get random index of all the body parts and store them
        cbodyIndex = Random.Range(0, body.Length);
        cfaceIndex = Random.Range(0, face.Count);
        chairIndex = Random.Range(0, hair.Count);
        ckitIndex = Random.Range(0, kit.Count);

        //set the body parts
        cbody.sprite = body[cbodyIndex];
        cface.sprite = face[cfaceIndex];
        chair.sprite = hair[chairIndex];
        ckit.sprite = kit[ckitIndex];
        StartImagesLerp();
    }

    private void BuildSavedCharacter(User user) {
        nameInput.gameObject.SetActive(false);
        
        ////set the body parts
        cbody.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        cface.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        chair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        ckit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;

        StartImagesLerp();
    }

    public void StartImagesLerp() {
        imagesLerping = true;
        timeStartedLerping = Time.time;//get the time this all started
    }

    //changes the hair to the next one
    public void NextHair() {
        if (chairIndex == hair.Count - 1) {
            chairIndex = 0;
        } else {
            chairIndex++;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the hair to the previous one
    public void PreviousHair() {
        if (chairIndex == 0) {
            chairIndex = hair.Count - 1;
        } else {
            chairIndex--;
        }
        chair.sprite = hair[chairIndex];
    }

    //changes the face to the next one
    public void NextFace() {
        if (cfaceIndex == face.Count - 1) {
            cfaceIndex = 0;
        } else {
            cfaceIndex++;
        }
        cface.sprite = face[cfaceIndex];
    }

    //changes the face to the previous face
    public void PreviousFace() {
        if (cfaceIndex == 0) {
            cfaceIndex = face.Count - 1;
        } else {
            cfaceIndex--;
        }
        cface.sprite = face[cfaceIndex];
    }

    //change the clothes to the next clothes
    public void NextClothes() {
        if (ckitIndex == kit.Count - 1) {
            ckitIndex = 0;
        } else {
            ckitIndex++;
        }
        ckit.sprite = kit[ckitIndex];
    }

    //changes the clothes to the previous clothes
    public void PreviousClothes() {
        if (ckitIndex == 0) {
            ckitIndex = kit.Count - 1;
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
        if (nameInput.gameObject.GetActive())
        {
            string playerName = nameInput.GetComponent<InputField>().text;
            if (!string.IsNullOrEmpty(playerName) && !string.IsNullOrWhiteSpace(playerName))
            {
                LoadingScreen.Instance.TurnOnLoadingScreen("Saving");
                StartCoroutine(StartSaveCoroutine());
            }
            else
            {
                GetComponent<ErrorText>().SetError("Please enter a username");
            }
        }
        else
        {
            LoadingScreen.Instance.TurnOnLoadingScreen("Saving");
            StartCoroutine(SaveCharacter());
        }

    }

    IEnumerator StartSaveCoroutine()
    {

        var nameCheckTask = BackendFunctions.NameCheck(nameInput.GetComponent<InputField>().text);
        yield return new WaitUntil(() => nameCheckTask.IsCompleted);
        if (nameCheckTask.IsFaulted)
        {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(nameCheckTask.Exception));
            LoadingScreen.Instance.TurnOffLoadingScreen();
        }
        else
        {

            if (nameCheckTask.Result != null)
            {
                GetComponent<ErrorText>().SetError("Username is taken");
                LoadingScreen.Instance.TurnOffLoadingScreen();
            }
            else if (nameCheckTask.Result == null)
            {
                StartCoroutine(SaveCharacter());
            }
        }

    }

    private IEnumerator SaveCharacter() {
        bool faulted = false;
        if (nameInput.GetActive())
        {
            var userNameTask = Database.UpdateUser("userName", nameInput.GetComponent<InputField>().text);
            yield return new WaitUntil(() => userNameTask.IsCompleted);
            if (userNameTask.IsFaulted)
                faulted = true;
        }
        if (LocalUser.locUser.hair != hair[chairIndex].name && !faulted)
        {
            var hairTask = Database.UpdateUser("hair", hair[chairIndex].name);
            yield return new WaitUntil(() => hairTask.IsCompleted);
            if (hairTask.IsFaulted)
                faulted = true;
        }
        if (LocalUser.locUser.face != face[cfaceIndex].name && !faulted) {
            var faceTask = Database.UpdateUser("face", face[cfaceIndex].name);
            yield return new WaitUntil(() => faceTask.IsCompleted);
            if (faceTask.IsFaulted)
                faulted = true;
        }
        if (LocalUser.locUser.kit != kit[ckitIndex].name && !faulted) {
            var kitTask = Database.UpdateUser("kit", kit[ckitIndex].name);
            yield return new WaitUntil(() => kitTask.IsCompleted);
            if (kitTask.IsFaulted)
                faulted = true;
        }
        if (LocalUser.locUser.body != body[cbodyIndex].name && !faulted) {
            var bodyTask = Database.UpdateUser("body", body[cbodyIndex].name);
            yield return new WaitUntil(() => bodyTask.IsCompleted);
            if (bodyTask.IsFaulted)
                faulted = true;
        }
        

        if (faulted) {
            GetComponent<ErrorText>().SetError("Failed to save character");
        } else {
            if(nameInput.GetActive())
                LocalUser.locUser.userName = nameInput.GetComponent<InputField>().text;
            if (LocalUser.locUser.hair != hair[chairIndex].name)
                LocalUser.locUser.hair = hair[chairIndex].name;
            if (LocalUser.locUser.face != face[cfaceIndex].name)
                LocalUser.locUser.face = face[cfaceIndex].name;
            if (LocalUser.locUser.kit != kit[ckitIndex].name)
                LocalUser.locUser.kit = kit[ckitIndex].name;
            if (LocalUser.locUser.body != body[cbodyIndex].name)
                LocalUser.locUser.body = body[cbodyIndex].name;
            PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
            PhotonNetworking.Instance.ConnectToPhoton();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
        }
        LoadingScreen.Instance.TurnOffLoadingScreen();
    }

    private void Update() {
        if (imagesLerping) {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            chair.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            cface.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            ckit.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            cbody.color = Color.Lerp(invisibleColor, visibleColor, percentageComplete * 20);
            if (percentageComplete >= timeTakenDuringLerp) {
                imagesLerping = false;
            }
        }
    }

}
