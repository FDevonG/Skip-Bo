using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Functions;
using System.Threading.Tasks;

public class FirebaseCloudFunctions : MonoBehaviour
{
    //FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;

    public static Task<string> GetUser() {
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
         var function = functions.GetHttpsCallable("GetUser");
        return function.CallAsync().ContinueWith((task) => {
            if (task.IsFaulted) {
                Debug.Log(task.Exception.Data);
                return null;
            } else {
                return (string)task.Result.Data;
            }
        });
    }

}
