using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;

public class UILoader : MonoBehaviour
{
      private FirebaseAuth firebaseAuth;

      public GameObject card;

      public GameObject ContentPanel;

       void Start()
      {
        Invoke("LoadUI", 0.2f);
      }

    void LoadUI()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance.GetReference("data").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                //Handle Error
                Debug.Log("Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var record in snapshot.Children)
                {
                    GameObject newTemplate = Instantiate(card, ContentPanel.transform) as GameObject;
                    Card item = newTemplate.GetComponent<Card>();
                    //newTemplate.transform.parent = ContentPanel.transform;
                    newTemplate.transform.localScale = Vector3.one;
                    item.title.text = record.Child("title").Value.ToString();

                    // Load Image
                    string imgURL = record.Child("thumbnail").Value.ToString();
                    Davinci.get().load(imgURL).into(item.thumbnail).start();
                }
            }
        });
    }
}
