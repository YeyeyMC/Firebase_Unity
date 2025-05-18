using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class UsersOnline : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var reference = FirebaseDatabase.DefaultInstance.GetReference("users-online");

        reference.ChildAdded += HandleChildAdded;
        reference.ChildRemoved += HandleChildRemoved;
    }
    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Value);

        Debug.Log(snapshot.Value + " Se ha conectado");
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        //var userObject = (Dictionary<string, object>)snapshot.Value;

        Debug.Log(snapshot.Value + " Se ha desconectado");

    }

    private void OnApplicationQuit()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        mDatabaseRef.Child("users-online").Child(userId).SetValueAsync(null);
    }
}
