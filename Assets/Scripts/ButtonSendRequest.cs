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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            mDatabaseRef.Child("users").Child(userId).Child("solicitudesEnviadas").Child(friendUserId).SetValueAsync(0);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
