using Firebase.Auth;
using Firebase.Database;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendFriendResponse : MonoBehaviour
{
    [SerializeField] private Button sendResponseButton;
    [SerializeField] private TMP_InputField _friendUserIdInputField;

    private void Reset()
    {
        sendResponseButton = GetComponent<Button>();
        _friendUserIdInputField = GameObject.Find("InputFieldFriendUserId").GetComponent<TMP_InputField>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sendResponseButton.onClick.AddListener(HandleSendResponseButtonClicked);
    }

    async private void HandleSendResponseButtonClicked()
    {
        string friendUserId = _friendUserIdInputField.text;

        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string friendUsername = (await FirebaseDatabase.DefaultInstance.GetReference("users/" + friendUserId + "/username").GetValueAsync()).Value?.ToString();

        //Respuesta estatica que acepta la solicitud de amistad (estado 1)
        await mDatabaseRef.Child("users").Child(friendUserId).Child("solicitudesRecibidas").Child(userId).SetValueAsync(1);

        await mDatabaseRef.Child("users").Child(userId).Child("amigos").Child(friendUserId).SetValueAsync(friendUsername);
    }
}
