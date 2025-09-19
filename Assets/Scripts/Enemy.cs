using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Comum,
        MiniBoss,
        Boss
    }

    [Header("Atributos do Inimigo")]
    public string nome;
    public int vida;
    public float velocidade;
    public int defesa;
    public float velocidadeAtaque;

    [Header("Tipo de Inimigo")]
    public EnemyType tipo;

    // Método para mostrar informações no console
    public void MostrarStatus()
    {
        Debug.Log($"Inimigo: {nome}\n" +
                  $"Tipo: {tipo}\n" +
                  $"Vida: {vida}\n" +
                  $"Velocidade: {velocidade}\n" +
                  $"Defesa: {defesa}\n" +
                  $"Velocidade de Ataque: {velocidadeAtaque}");
    }
}
