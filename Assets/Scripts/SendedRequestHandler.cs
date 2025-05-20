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

        var reference = mDatabaseRef.Child("users").Child(userId).Child("solicitudesRecibidas");
        reference.ChildAdded += HandleChildAdded;
        //reference.ChildRemoved += HandleChildRemoved;
    }

    //private async void HandleChildAdded(object sender, ChildChangedEventArgs args)
    //{
    //    if (args.DatabaseError != null)
    //    {
    //        Debug.LogError(args.DatabaseError.Message);
    //        return;
    //    }

    //    DataSnapshot snapshot = args.Snapshot;

    //    Debug.Log(snapshot.Key + " :Solicitud Pendiente");
    //}

    private async void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        string friendId = snapshot.Key;
        Debug.Log("Respuesta de " + friendId + " estado: " + snapshot.Value);
        int estado = int.Parse(snapshot.Value.ToString());
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        string friendUsername = (await FirebaseDatabase.DefaultInstance.GetReference("users/" + friendId + "/username").GetValueAsync()).Value?.ToString();

        //bool checkRequestId = (FirebaseDatabase.DefaultInstance.GetReference("users/" + userId + "/solicitudesEnviadas").Equals(friendId));

        ////Validamos si la respuesta es una solicitud pendiente
        //Debug.Log(checkRequestId);
        //if (checkRequestId)
        //{
        //    Debug.Log("Se descarta respuesta de solicitud de amistad con id " + friendId);
        //    EliminarSolicitud(friendId, "FriendResponse");
        //    return;
        //}

        //Estado 1 para solicitud aceptada
        if (estado == 1)
        {
            Debug.Log(friendUsername + " ha aceptado tu solicitud");
            mDatabaseRef.Child("users").Child(userId).Child("amigos").Child(friendId).SetValueAsync(friendUsername);
        }
        //Estado 2 para solicitud rechazada
        if (estado == 2)
        {
            Debug.Log(friendUsername + " ha rechazado tu solicitud");
        }
        EliminarSolicitud(friendId, "solicitudesEnviadas");
        EliminarSolicitud(friendId, "solicitudesRecibidas");
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

    private void EliminarSolicitud(string requestUserId, string requestMailbox)
    {
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        
        mDatabaseRef.Child("users").Child(userId).Child(requestMailbox).Child(requestUserId).SetValueAsync(null);
    }
}
