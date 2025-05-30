using Firebase.Auth;
using System.Collections;
using Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class ButtonSignup : MonoBehaviour
{
    [SerializeField]
    private Button _registrationButton;

    private Coroutine _registrationCoroutine;
    
    [SerializeField] private GameObject LogContraseña;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_InputField _emailInputField;
    [SerializeField] private TMP_InputField _passwordInputField;
    [SerializeField] private TMP_InputField _usernameInputField;
    

    private void Reset()
    {
        _registrationButton = GetComponent<Button>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _registrationButton.onClick.AddListener(HandleRegistrationButtonClick);
    }

    private void HandleRegistrationButtonClick()
    {
        string email = GameObject.Find("InputFieldEmail").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

        _registrationCoroutine = StartCoroutine(RegisterUser(email, password));

    }

    IEnumerator RegisterUser(string email, string password)
    {
        string username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        var auth = FirebaseAuth.DefaultInstance;

        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.IsCanceled)
        {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");

        }
        else if (registerTask.IsFaulted)
        {
            foreach (var innerException in registerTask.Exception.Flatten().InnerExceptions)
            {
                if (innerException is FirebaseException firebaseEx)
                {
                    if (firebaseEx.ErrorCode == (int)AuthError.EmailAlreadyInUse)
                    {
                        LogError("Este correo ya está registrado");
                        yield break; // Exit the coroutine
                    }
                }
            }
            LogError("Hubo un error en el registro, vuelve a intentarlo.");
            // Handle the error intentar crear cuenta con correo existentee
            Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + registerTask.Exception);
        }
        else
        {

            // Firebase user has been created.
            AuthResult result = registerTask.Result;
            FirebaseDatabase.DefaultInstance.RootReference
                .Child("users").Child(result.User.UserId).Child("username").SetValueAsync(username);

            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        }
    }
    void LogError(string message)
    {
        LogContraseña.SetActive(true);
        label.text = message;
        StartCoroutine(ShowErrorMessage());
    }
    IEnumerator ShowErrorMessage()
    {
        yield return new WaitForSeconds(2f);
        LogContraseña.SetActive(false);
        _emailInputField.text = "";
        _passwordInputField.text = "";
        _usernameInputField.text = "";
        Debug.Log("Error de inicio de sesión");
    }


    // Update is called once per frame
    void Update()
    {

    }
}
