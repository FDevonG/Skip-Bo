using UnityEngine;
using UnityEngine.UI;

public class ErrorPanel : MonoBehaviour
{
    [SerializeField] Button retryButton;

    // Start is called before the first frame update
    void Start()
    {
        retryButton.onClick.AddListener(() => StartCoroutine(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().DoesPlayerExist()));
    }

}
