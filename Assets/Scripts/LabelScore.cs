using System;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class LabelScore : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    private DatabaseReference _userScoreRef;
    private string _userId;

    void Awake()
    {
        // Inicializar referencia a la base de datos
        _userScoreRef = FirebaseDatabase.DefaultInstance.GetReference("users");
        
        // Suscribirse a cambios de estado de autenticación
        FirebaseAuth.DefaultInstance.StateChanged += HandleStateChanged;
    }

    void OnDestroy()
    {
        // Limpiar suscripción cuando el objeto se destruya
        FirebaseAuth.DefaultInstance.StateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(object sender, EventArgs e)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            _userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            LoadScore();
        }
    }

    private void LoadScore()
    {
        _userScoreRef.Child(_userId).Child("score")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error al cargar el score: " + task.Exception);
                    SetDefaultScore();
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    
                    // Si no existe el score o es null, establecer valor por defecto
                    if (!snapshot.Exists || snapshot.Value == null)
                    {
                        SetDefaultScore();
                    }
                    else
                    {
                        // Mostrar el score actualizado
                        label.text = snapshot.Value.ToString();
                    }
                }
            });
    }

    private void SetDefaultScore()
    {
        // Establecer valor por defecto en la UI
        label.text = "0";
        
        // Opcional: Guardar el valor por defecto en la base de datos
        _userScoreRef.Child(_userId).Child("score").SetValueAsync(0)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error al establecer score por defecto");
                }
            });
    }

    private void Reset()
    {
        label = GetComponent<TMP_Text>();
    }
}