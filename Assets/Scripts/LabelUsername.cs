using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class LabelUsername : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    void Awake()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            SetUsername();
        }
    }

    private void SetUsername()
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        FirebaseDatabase.DefaultInstance
        .GetReference("users/" + userId + "/username")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                label.text = snapshot.Value.ToString();
                // Do something with snapshot...
            }
        });
    }

    private void Reset()
    {
        label = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
