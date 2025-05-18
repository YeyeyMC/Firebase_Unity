using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AuthStateHandler : MonoBehaviour
{
    [SerializeField]
    GameObject _panelAuth;
    [SerializeField]
    GameObject _panelScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Reset()
    {
        _panelAuth = GameObject.Find("PanelAuth");
        _panelScore = GameObject.Find("PanelScore");

        _panelScore.SetActive(false);
    }

    void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            Invoke("SetAuth", 2f);
            setOnline();
        }
        else
        {
            _panelAuth.SetActive(true);
            _panelScore.SetActive(false);
        }
    }

    public void SetAuth()
    {
        _panelAuth.SetActive(false);
        _panelScore.SetActive(true);
    }

    private void setOnline()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + userId + "/username")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string username = snapshot.Value.ToString();
                PlayerPrefs.SetString("username", username);
                mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(username);

            }
        });

        
    }
}
