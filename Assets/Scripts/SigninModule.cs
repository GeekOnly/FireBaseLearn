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
    Firebase.Auth.AuthResult user;

    bool _isLogged = false;
    bool _isValid = true;

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
                _isValid = false;
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result;
            Debug.LogFormat("Signing Sucess: {0}", user.User.UserId);
            _isLogged = true;
        });

        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadNewScene()
    {
        Debug.Log("Load New Scene");
        if (user != null)
        {
            PlayerPrefs.SetString("uid", user.User.UserId);
        }

        if(_isValid == false || (_isValid && string.IsNullOrEmpty(PlayerPrefs.GetString("uid"))))
        {
            errorText.text = "Failed Login";
        }

        yield return new WaitUntil(() => _isLogged);

        // SceneManager.LoadScene("Main", LoadSceneMode.Single);
        AsyncOperation async = SceneManager.LoadSceneAsync("Main");
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
