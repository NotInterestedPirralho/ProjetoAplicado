using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Atributos do Personagem")]
    public string nome;
    public int vida;
    public float velocidade;
    public int defesa;
    public float velocidadeAtaque;

    // Método para mostrar informações no console
    public void MostrarStatus()
    {
        Debug.Log($"Personagem: {nome}\n" +
                  $"Vida: {vida}\n" +
                  $"Velocidade: {velocidade}\n" +
                  $"Defesa: {defesa}\n" +
                  $"Velocidade de Ataque: {velocidadeAtaque}");
    }
}
