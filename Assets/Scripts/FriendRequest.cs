using Firebase.Auth;
using Firebase.Database;
using System;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class FriendRequest : MonoBehaviour
{
    void Start()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var reference = mDatabaseRef.Child("users").Child(userId).Child("solicitudesRecibidas");
        reference.ChildAdded += HandleChildAdded;
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        string friendId = snapshot.Key;
        string friendUsername = snapshot.Value.ToString();

        Debug.Log("Tienes una solicitud de amistad de " + friendUsername);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
