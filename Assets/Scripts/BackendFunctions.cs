using System.Threading.Tasks;
using Firebase.Functions;
using System.Collections.Generic;

public static class BackendFunctions
{
    public static Task<List<string>> NameCheck(string playerName)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("nameCheck");
        return function.CallAsync(playerName).ContinueWith((task) => {
            return (List<string>)task.Result.Data;
        });
    }

    public static Task<string>GetUser(string userID)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("getUser");
        return function.CallAsync(userID).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }

    public static Task<string> FindUser(string userName)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("findUser");
        return function.CallAsync(userName).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }

    public static Task<string> GetUsers(List<string> ids)
    {
        var functions = FirebaseFunctions.DefaultInstance;
        var function = functions.GetHttpsCallable("getUsers");
        return function.CallAsync(ids.ToArray()).ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }
}
