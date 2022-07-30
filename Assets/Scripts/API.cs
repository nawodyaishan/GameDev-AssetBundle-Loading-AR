using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    const string BundleFolderURL = "https://assetbundle2.s3.amazonaws.com/AssetBundles/";

    public void GetBundleObject(string assetName, UnityAction<GameObject> callback, Transform bundleParent)
    {
        StartCoroutine(GetDisplayBundleRoutine(assetName, callback, bundleParent));
    }

    IEnumerator GetDisplayBundleRoutine(string assetName, UnityAction<GameObject> callback, Transform bundleParent)
    {
        string bundleURL = BundleFolderURL + assetName + "-";

        //append platform to asset bundle name
        #if UNITY_ANDROID
        bundleURL += "Android";
        #else
        bundleURL += "IOS";
        #endif

        Debug.Log("Requesting bundle at " + bundleURL);

        //request asset bundle
        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Network error");
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            if (bundle != null)
            {
                string rootAssetPath = bundle.GetAllAssetNames()[0];
                GameObject arObject = Instantiate(bundle.LoadAsset(rootAssetPath) as GameObject, bundleParent);
                bundle.Unload(false);
                callback(arObject);
            }
            else
            {
                Debug.Log("Not a valid asset bundle");
            }
        }
    }
}