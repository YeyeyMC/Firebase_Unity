using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SendedRequestHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var reference = mDatabaseRef.Child("users").Child(userId).Child("solicitudesEnviadas");
        reference.ChildAdded += HandleChildAdded;
        reference.ChildChanged += HandleChildChanged;
        //reference.ChildRemoved += HandleChildRemoved;
    }

    private async void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        Debug.Log(snapshot.Key + " :Solicitud Pendiente");
    }

    private async void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        string friendId = snapshot.Key;
        int estado = (int)snapshot.Value;

        string friendUsername = (await FirebaseDatabase.DefaultInstance.GetReference("users/" + friendId + "/username").GetValueAsync()).Value.ToString();

        if (estado == 1)
        {
            Debug.Log(friendId + " ha aceptado tu solicitud");
            EliminarSolicitud(friendId);
            var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
            var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            mDatabaseRef.Child("users").Child(userId).Child("amigos").SetValueAsync(friendId);
        }
        if (estado == 2)
        {
            Debug.Log(friendId + " ha rechazado tu solicitud");
            EliminarSolicitud(friendId);
        }

    }

    //private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);
    //        return;
    //    }

    //    DataSnapshot snapshot = args.Snapshot;

    //    //var userObject = (Dictionary<string, object>)snapshot.Value;

    //    Debug.Log(snapshot.Value + " Se ha desconectado");

    //}

    private void EliminarSolicitud(string requestUserId)
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        
        mDatabaseRef.Child("users").Child(userId).Child("solicitudesEnviadas").Child(requestUserId).SetValueAsync(null);
    }
}
