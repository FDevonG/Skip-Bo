using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShareScreenShot : MonoBehaviour
{

	public void ShareScreen()
    {
		GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 0);
		GetComponentInChildren<Text>().text = "";
        StartCoroutine(TakeScreenshotAndShare());
    }

	private IEnumerator TakeScreenshotAndShare()
	{
		yield return new WaitForEndOfFrame();
        GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().TurnOffBannerAd();
		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();
        
		string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// To avoid memory leaks
		Destroy(ss);

		GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 1);
		GetComponentInChildren<Text>().text = "Share";
		GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().TurnOnBannerAd();

		new NativeShare().AddFile(filePath)
			.SetSubject("Skip-Bo Standings").SetText("https://play.google.com/store/apps/details?id=com.FogBankGames.SkipBo")
			.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
			.Share();

		// Share on WhatsApp only, if installed (Android only)
		//if( NativeShare.TargetExists( "com.whatsapp" ) )
		//	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
	}
}
