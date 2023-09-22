using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//File Handle
using System.IO;
using System;
using SimpleFileBrowser;

// Firebase
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;
using Firebase.Auth;
using UnityEditor.Build.Content;

public class UploadFile : MonoBehaviour
{
    FirebaseStorage storage;
    public Image showImage;
    StorageReference storageReference;
    DatabaseReference databaseReference;
    private FirebaseAuth auth;
    public InputField title;
    public string node;

    private void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.SetExcludedExtensions(".link", ".tmp", ".rar", ".exe");
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://fir-learn-ece2b.appspot.com");
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void OnButtonClick()
    {
        StartCoroutine(showDialogUpload());
    }

    IEnumerator showDialogUpload()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files abs Folders", "Load");
        Debug.Log(FileBrowser.Success);
        if(FileBrowser.Success)
        {
            for(int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);
            }
            Debug.Log("File Selected");
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
            var newMetaData = new MetadataChange();
            newMetaData.ContentType = "image/jpeg";

            //Create Reference
            string imgName = "https://firebasestorage.googleapis.com/v0/b/fir-learn-ece2b.appspot.com/o/" + PlayerPrefs.GetString("uid") + ".jpeg" + "?alt=media&token=62fe1fda-776c-416f-94ac-1a553c8a9591";
            StorageReference uploadRef = storageReference.Child(PlayerPrefs.GetString("uid") + ".jpeg");
            Debug.Log("Upload Started");
            uploadRef.PutBytesAsync(bytes,newMetaData).ContinueWithOnMainThread((task) =>
            {
                if(task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                }else
                {
                    Debug.Log("File Uploaded Successfully!");
                    StartCoroutine(getDataBase(imgName));
                }
            });
        }
    }

    IEnumerator getDataBase(string imgPath)
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log(imgPath);
        Debug.Log("Add UID : " + PlayerPrefs.GetString("uid"));
        string value = PlayerPrefs.GetString("uid");
        node = "" + PlayerPrefs.GetString("uid");
        databaseReference.Child("data").Child(node).Child("key").SetValueAsync(node);
        databaseReference.Child("data").Child(node).Child("thumbnail").SetValueAsync(imgPath);
        Davinci.get().load(imgPath).into(showImage).start();
    }

    public void SubmitAddData()
    {
        databaseReference.Child("data").Child(node).Child("title").SetValueAsync(title.text);
        Debug.Log("Submit!!!!!!!!!");
    }
}
