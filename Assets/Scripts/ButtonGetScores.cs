using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;


public class ButtonGetScores : MonoBehaviour
{

    [SerializeField] private Button _buttonGetLeaderboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").LimitToLast(3).ValueChanged += HandleValueChanged;
    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Dictionary<string, object> userList = (Dictionary<string, object>)snapshot.Value;

        foreach (var userDoc in userList)
        {
            var userObject = (Dictionary<string, object>)userDoc.Value;
            Debug.Log(userDoc.Key);
            Debug.Log(userObject["username"] + " | " + userObject["score"]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
