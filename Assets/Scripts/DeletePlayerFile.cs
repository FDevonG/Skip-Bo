using System.IO;
using UnityEngine;

public class DeletePlayerFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        File.Delete(Application.persistentDataPath + "/player.ski");
    }

}
