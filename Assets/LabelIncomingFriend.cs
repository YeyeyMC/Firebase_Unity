using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using TMPro;
using UnityEngine;

public class LabelIncomingFriend : MonoBehaviour
{
    private TMP_Text label;

    void Awake()
    {
        FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
        label = GameObject.FindGameObjectWithTag("TextFeedback").GetComponent<TMP_Text>();
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SetUsername();
        }
    }

    private void SetUsername()
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        string friendUserId = _frienUserIdInputField.text;    
            
            mDatabaseRef.Child("users").Child(userId).Child("friendRequests").Child(userId)->ChildAdded)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    label.text = snapshot.Value.ToString();
                    // Do something with snapshot...
                }
            });
    }

    private void Reset()
    {
        label = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}