using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("¡Conectado! Ahora asignando nombre...");
        // Llamamos a la función para poner el nombre
        ActualizarNombreJugador("Jugador_Matias");
    }

    void ActualizarNombreJugador(string nombre)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nombre
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            res => Debug.Log("¡Nombre actualizado con éxito!: " + res.DisplayName),
            OnLoginFailure);
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }
}