using Firebase.Auth;
using Firebase.Database;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSetScore : MonoBehaviour
{
    [SerializeField] private Button setScoreButton;
    [SerializeField] private TMP_InputField scoreInputField;

    private void Reset()
    {
        setScoreButton = GetComponent<Button>();
        scoreInputField = GameObject.Find("InputFieldScore").GetComponent<TMP_InputField>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setScoreButton.onClick.AddListener(HandleSetScoreButtonClicked);
    }

    private void HandleSetScoreButtonClicked()
    {
        int score = int.Parse(scoreInputField.text);
        var mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        mDatabaseRef.Child("users").Child(userId).Child("score").SetValueAsync(score);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
