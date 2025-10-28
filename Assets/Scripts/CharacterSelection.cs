using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance;

    // este campo vai guardar qual prefab vamos spawnar no nível
    public GameObject chosenCharacterPrefab;

    void Awake()
    {
        // singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // chamar isto quando o jogador clicar num personagem
    public void ChooseCharacter(GameObject prefab)
    {
        chosenCharacterPrefab = prefab;
    }
}
