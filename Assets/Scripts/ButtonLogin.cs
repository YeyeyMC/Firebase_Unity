using Firebase.Auth;
using System;
using System.Collections;
using Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ButtonLogin : MonoBehaviour
{

    [SerializeField]
    private Button _loginButton;
    [SerializeField]
    private TMP_InputField _emailInputField;
    [SerializeField]
    private TMP_InputField _passwordInputField;
    
    [SerializeField] private GameObject LogContraseña;
    [SerializeField] private TMP_Text label;

    private Coroutine coroutine; 

    private void Reset()
    {
        _loginButton = GetComponent<Button>();
        _emailInputField = GameObject.Find("InputFieldEmail").GetComponent<TMP_InputField>();
        _passwordInputField = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>(); ;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        _loginButton.onClick.AddListener(HandleLogin);
    }
    void HandleLogin()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(RegisterUser());
    }

    IEnumerator RegisterUser()
    {
        string email = _emailInputField.text;
        string password = _passwordInputField.text;

        // Basic validation
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            LogError("Ingrese correo y contraseña");
            yield break;
        }

        var auth = FirebaseAuth.DefaultInstance;
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

         yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.IsCanceled)
        {
            LogError("Login was canceled");
            yield break;
        }

        if (loginTask.IsFaulted)
        {
            string errorMessage = "Login failed";

            foreach (var innerException in loginTask.Exception.Flatten().InnerExceptions)
            {
                if (innerException is FirebaseException firebaseEx)
                {
                    switch ((AuthError)firebaseEx.ErrorCode)
                    {
                        case AuthError.InvalidEmail:
                            errorMessage = "Direccion de correo invalida";
                            break;
                        case AuthError.WrongPassword:
                            errorMessage = "Contraseña incorrecta";
                            break;
                        case AuthError.UserNotFound:
                            errorMessage = "Usuario no encontrado";
                            break;
                        case AuthError.TooManyRequests:
                            errorMessage = "Demasiadas solicitudes, intente más tarde";
                            break;
                        case AuthError.UserDisabled:
                            errorMessage = "Cuenta pringada";
                            break;
                        default:
                            errorMessage = "Login error occurred";
                            break;
                    }

                    break; // Show the first relevant error
                }
            }

            LogError(errorMessage);
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
            Debug.Log("Error de inicio de sesión");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
