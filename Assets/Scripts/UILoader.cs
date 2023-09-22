using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;

public class UILoader : MonoBehaviour
{
    private FirebaseAuth firebaseAuth;

    public GameObject card;

    public GameObject ContentPanel;

    private void Start()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }
}
