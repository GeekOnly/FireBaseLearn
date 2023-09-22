using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SigninModule : MonoBehaviour
{
    private FirebaseAuth firebaseAuth;
    public InputField email_input, password_input;
    public Text errorText;

    private void Start()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    public void LoginAction()
    {
        string email, password;
        email = email_input.text;
        password = password_input.text;

        if (string.IsNullOrEmpty(email))
        {
            errorText.text = "Please fill you Email Address!";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            errorText.text = "Please fill you Password!";
            return;
        }

        firebaseAuth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
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

            Firebase.Auth.AuthResult user = task.Result;
            Debug.LogFormat("Signing Sucess: {0}", user.User.UserId);
            PlayerPrefs.SetString("uid", user.User.UserId);

            StartCoroutine(LoadNewScene());
        });
    }

    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(1f);
        AsyncOperation async = SceneManager.LoadSceneAsync("Main");
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
