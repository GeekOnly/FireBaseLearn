using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Authentication : MonoBehaviour
{
    private FirebaseAuth firebaseAuth;
    public InputField email_input, password_input, fullname_input;
    public Text errorText;
    public string uid;

    DatabaseReference databaseReference;

    private void Start()
    {
        uid = PlayerPrefs.GetString("uid");
        firebaseAuth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Signup()
    {
        string email, password, fullname;
        email = email_input.text;
        password = password_input.text;
        fullname = fullname_input.text;

        if(string.IsNullOrEmpty(email))
        {
            errorText.text = "Please fill you Email Address!";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            errorText.text = "Please fill you Password!";
            return;
        }

        if (string.IsNullOrEmpty(fullname))
        {
            errorText.text = "Please fill you Full Name!";
            return;
        }

        firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult newUser = task.Result;
            Debug.LogFormat("Create New User Sucessfully: {0} {1}", newUser.User.DisplayName, newUser.User.UserId);

            databaseReference.Child("users").Child(newUser.User.UserId);
            databaseReference.Child("users").Child(newUser.User.UserId).Child("email").SetValueAsync(email);
            databaseReference.Child("users").Child(newUser.User.UserId).Child("fullname").SetValueAsync(fullname);
            databaseReference.Child("users").Child(newUser.User.UserId).Child("uid").SetValueAsync(newUser.User.UserId);

            // เปลี่ยน Scene ไปหน้า Login
            SceneManager.LoadScene("", LoadSceneMode.Single);
        });
    }
}
