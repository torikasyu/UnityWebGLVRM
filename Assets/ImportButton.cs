using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using VRM;

public class ImportButton : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void FileImporterCaptureClick();

    public void OnButtonPointerDown()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        FileImporterCaptureClick();
#else
        LoadFromFile();
#endif
        }

    void LoadFromFile()
    {
        //VRMファイルのパスを指定します
        var path = Application.dataPath + "/Resources/vj_takagi.vrm";

        //ファイルをByte配列に読み込みます
        var bytes = System.IO.File.ReadAllBytes(path);

        var context = new VRMImporterContext();

        // GLB形式でJSONを取得しParseします
        context.ParseGlb(bytes);

        context.Load();
        OnLoaded(context);
    }


    public void FileSelected(string url)
    {
        StartCoroutine(LoadJson(url));
    }

    private IEnumerator LoadJson(string url)
    {
        print(url);
        WWW www = new WWW(url);

        while (!www.isDone)
        { 
            yield return null;
        }
        if (www.error == null)
        {
            LoadVRMClicked_WebGL(www.bytes);
        }
        else
        {
            print(www.error);
        }
    }

    void LoadVRMClicked_WebGL(Byte[] bytes)
    {
        var context = new VRMImporterContext();

        // GLB形式でJSONを取得しParseします
        context.ParseGlb(bytes);

        context.Load();
        OnLoaded(context);
    }

    private void OnLoaded(VRMImporterContext context)
    {
        //読込が完了するとcontext.RootにモデルのGameObjectが入っています
        var root = context.Root;

        //モデルをワールド上に配置します
        root.transform.position = new Vector3(0, 0, 0);
        root.transform.Rotate(new Vector3(0, 180, 0));

        //メッシュを表示します
        context.ShowMeshes();
    }
}