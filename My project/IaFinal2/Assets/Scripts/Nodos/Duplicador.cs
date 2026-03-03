using UnityEngine;

public class Duplicador : MonoBehaviour
{
    public GameObject objetoParaDuplicar;
    public string nombreBase = "Node";

    void Start()
    {
        // Si no se asigna un objeto para duplicar, usa el objeto al que está adjunto este script
        if (objetoParaDuplicar == null)
        {
            objetoParaDuplicar = this.gameObject;
        }
    }

    void Update()
    {
        // Duplica cuando se presiona la tecla D
        if (Input.GetKeyDown(KeyCode.D))
        {
            DuplicarConNombre();
        }
    }

    void DuplicarConNombre()
    {
        // Crea el nombre del nuevo objeto
        int contador = 1;
        string nuevoNombre;
        bool nombreDisponible;

        // Busca el primer nombre que no exista
        do
        {
            nuevoNombre = nombreBase + " " + contador;
            nombreDisponible = !GameObject.Find(nuevoNombre); // Busca un objeto con ese nombre
            contador++;
        } while (!nombreDisponible);

        // Clona el objeto
        GameObject nuevoObjeto = Instantiate(objetoParaDuplicar, transform.position, Quaternion.identity);
        nuevoObjeto.name = nuevoNombre;
    }
}