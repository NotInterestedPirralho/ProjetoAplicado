using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Image healthFill; // arraste a Image do Fill da barra
    public Enemy enemy;      // refer�ncia ao script Enemy
    public float velocidadeDesaparecer = 2f; // velocidade da anima��o da barra

    private bool inimigoMorto = false;

    void Update()
    {
        if (enemy != null && !inimigoMorto)
        {
            // Atualiza a barra normalmente
            healthFill.fillAmount = (float)enemy.health / enemy.healthMaximo;
        }
        else if (inimigoMorto)
        {
            // Faz a barra esvaziar suavemente
            healthFill.fillAmount = Mathf.MoveTowards(healthFill.fillAmount, 0f, velocidadeDesaparecer * Time.deltaTime);

            // Quando chegar a zero, destr�i a barra
            if (healthFill.fillAmount <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    // Chamado pelo Enemy quando ele morre
    public void InimigoMorreu()
    {
        inimigoMorto = true;
        enemy = null; // remove refer�ncia para n�o dar erro
    }
}
