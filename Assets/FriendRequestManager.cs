using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendRequestManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject _requestPrefab; // Prefab a instanciar
    [SerializeField] private Transform _requestsContainer; // Contenedor padre para las solicitudes
    
    [Header("Ordenamiento")]
    [SerializeField] private bool _maintainOrder = true; // Mantener orden de la BD
    
    private DatabaseReference _requestsRef;
    private string _currentUserId;
    private Dictionary<string, GameObject> _activeRequests = new Dictionary<string, GameObject>();

    private void OnEnable()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            _currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            _requestsRef = FirebaseDatabase.DefaultInstance.GetReference($"users/{_currentUserId}/friendRequests");
            
            // Cargar solicitudes existentes
            _requestsRef.GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    foreach (DataSnapshot snapshot in task.Result.Children)
                    {
                        CreateRequestItem(snapshot.Key, snapshot.Value.ToString());
                    }
                }
                
                // Escuchar nuevas solicitudes
                _requestsRef.ChildAdded += HandleChildAdded;
                _requestsRef.ChildRemoved += HandleChildRemoved;
            });
        }
    }

    private void OnDisable()
    {
        if (_requestsRef != null)
        {
            _requestsRef.ChildAdded -= HandleChildAdded;
            _requestsRef.ChildRemoved -= HandleChildRemoved;
        }
        
        ClearAllRequests();
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        CreateRequestItem(args.Snapshot.Key, args.Snapshot.Value.ToString());
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (_activeRequests.ContainsKey(args.Snapshot.Key))
        {
            Destroy(_activeRequests[args.Snapshot.Key]);
            _activeRequests.Remove(args.Snapshot.Key);
        }
    }

    private void CreateRequestItem(string friendId, string friendUsername)
    {
        if (_activeRequests.ContainsKey(friendId))
        {
            // Actualizar solicitud existente si es necesario
            return;
        }

        GameObject requestItem = Instantiate(_requestPrefab, _requestsContainer);
        
        // Configurar el prefab con los datos
        RequestItemController itemController = requestItem.GetComponent<RequestItemController>();
        if (itemController != null)
        {
            itemController.Initialize(friendId, friendUsername, this);
        }
        
        // Mantener orden si está habilitado
        if (_maintainOrder)
        {
            requestItem.transform.SetAsLastSibling();
        }
        
        _activeRequests.Add(friendId, requestItem);
    }

    public void AcceptRequest(string friendId)
    {
        // Lógica para aceptar solicitud
        Debug.Log($"Solicitud aceptada de {friendId}");
        RemoveRequest(friendId);
        // (Opcional) Actualizar base de datos
        AddAsFriend(friendId); // Método que debes implementar
    }

    public void RejectRequest(string friendId)
    {
        
        Debug.Log($"Solicitud rechazada de {friendId}");
        RemoveRequest(friendId);
    }
    private async void AddAsFriend(string friendId)
    {
        // Ejemplo: Añadir a la lista de amigos en Firebase
        string currentUserId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DatabaseReference dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    
        await dbRef.Child("users")
            .Child(currentUserId)
            .Child("friends")
            .Child(friendId)
            .SetValueAsync(true);
    }
    private void RemoveRequest(string friendId)
    {
        // 1. Destruir el prefab si existe
        if (_activeRequests.ContainsKey(friendId))
        {
            Destroy(_activeRequests[friendId]);
            _activeRequests.Remove(friendId);
        }
    
        // 2. Eliminar de Firebase
        _requestsRef.Child(friendId).RemoveValueAsync()
            .ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error al eliminar solicitud: " + task.Exception);
                }
            });
    }

    private void ClearAllRequests()
    {
        foreach (var request in _activeRequests.Values)
        {
            Destroy(request);
        }
        _activeRequests.Clear();
    }
}