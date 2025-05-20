using Firebase.Auth;
using Firebase.Database;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSendRequest : MonoBehaviour
{

    [SerializeField] private Button _sendRequestButton;
    [SerializeField] private TMP_InputField _friendUserIdInputField;

    private void Reset()
    {
        _sendRequestButton = GetComponent<Button>();
        _friendUserIdInputField = GameObject.Find("InputFieldFriendUserId").GetComponent<TMP_InputField>();
    }

    void Start()
    {
        _sendRequestButton.onClick.AddListener(HandleSedRequestButtonClicked);
    }

    private void HandleSedRequestButtonClicked()
    {
        string friendUserId = _friendUserIdInputField.text;

        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var username = PlayerPrefs.GetString("username");

        mDatabaseRef.Child("users").Child(friendUserId).Child("solicitudesRecibidas").Child(userId).SetValueAsync(username).ContinueWith(t => 
        {
            //Manejar error
            mDatabaseRef.Child("users").Child(userId).Child("solicitudesEnviadas").Child(friendUserId).SetValueAsync(0);
            //Establece estado 0 para solicitud pendiente
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
