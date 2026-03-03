using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOView : MonoBehaviour
{
    [SerializeField] Transform target;
    public Transform PublictTargeter
    {
        set
        {
            target = value;
        }
    }
    [SerializeField] Transform root; // Pivote de rotación (cabeza o cuerpo)
    [SerializeField] float viewDistanceMin = 10f;
    [SerializeField] float AngleMin = 90f;
    [SerializeField] LayerMask ToDetect;
    public bool isFov = false;

    bool desperate;
   
    private Coroutine facingCoroutine; // Para controlar la rotación hacia el jugador
    [SerializeField] float rotationSpeed = 5f; // Velocidad de rotación hacia el jugador (ajustable en Inspector)
    [SerializeField] float angleTolerance = 5f; // Ángulo tolerado para considerar "mirando" al jugador
    IAEnemy ia;
    void Start()
    {
        
      
    }

    void Update()
    {
        canseeplayer();
    }

    void canseeplayer()
    {
        if (target == null)
        {
            // Debug temporal
            Debug.LogWarning("Target es null en canseeplayer(). No se puede detectar jugador.");
            isFov = false;
            return;
        }

        Vector3 dir = (target.position - root.position).normalized; // Normalizar para ángulo y Ray
        float distance = Vector3.Distance(root.position, target.position); // Distancia real

        // Debug temporal: Log cada frame (comenta después de probar)
        // Debug.Log("Distancia al jugador: " + distance + ", viewDistanceMin: " + viewDistanceMin);

        bool inRange = distance < viewDistanceMin;
        if (!inRange)
        {
            // Fuera de rango: no detectar
            if (isFov)
            {
                isFov = false;
                Debug.Log("Jugador fuera de rango. isFov = false");
                // Reanudar escaneo si desperate
             
                ia.fsm.SendInput(EnemyState.Calm);
            }
            return;
        }

        // En rango: Chequear ángulo y Raycast (siempre, independientemente de isFov o desperate)
        Vector3 forward = root.forward;
        float angle = Vector3.Angle(forward, dir);

        // Debug temporal
        // Debug.Log("Ángulo al jugador: " + angle + ", AngleMin/2: " + (AngleMin / 2));

        bool inAngle = angle < AngleMin / 2;
        if (!inAngle)
        {
            // Fuera del cono: no detectar
            if (isFov)
            {
                isFov = false;
                Debug.Log("Jugador fuera del ángulo FOV. isFov = false");
                // Reanudar escaneo si desperate
               
                ia.fsm.SendInput(EnemyState.Calm);
            }
            // Si desperate y no viendo, escanear
            
            return;
        }

        // En rango y ángulo: Hacer Raycast
        Ray ray = new Ray(root.position, dir);
        RaycastHit hit;
        bool rayHit = Physics.Raycast(ray, out hit, viewDistanceMin, ToDetect); // Usar distancia real

        // Debug temporal
        if (rayHit)
        {
            Debug.Log("Raycast HIT: " + hit.collider.name + " en layer " + LayerMask.LayerToName(hit.collider.gameObject.layer) +
                      ", distancia hit: " + hit.distance + ", ToDetect layers: " + ToDetect.value);
        }
        else
        {
            Debug.Log("Raycast NO HIT (obstáculo o fuera de layers)");
        }

        if (rayHit && hit.collider != null)
        {
            // Chequear si es el layer correcto (jugador)
            if ((ToDetect & (1 << hit.collider.gameObject.layer)) != 0)
            {
                // ¡Detectado!
                if (!isFov)
                {
                    isFov = true;
                    Debug.Log("¡Jugador DETECTADO! isFov = true. Enviando InRange al FSM.");
                }

                // Detener escaneo si activo
              

                // Girar hacia el jugador si no está ya mirándolo
                float currentAngleToPlayer = Vector3.Angle(root.forward, dir);
                if (currentAngleToPlayer > angleTolerance)
                {
                    if (facingCoroutine != null)
                    {
                        StopCoroutine(facingCoroutine);
                    }
                    facingCoroutine = StartCoroutine(GirarHaciaJugador(dir));
                    Debug.Log("Iniciando giro hacia jugador. Ángulo actual: " + currentAngleToPlayer);
                }

                // Enviar estado InRange al FSM
                ia.fsm.SendInput(EnemyState.InRange);
                return; // Salir para mantener isFov = true
            }
            else
            {
                // Hit pero no layer correcto (e.g., pared)
                if (isFov)
                {
                    isFov = false;
                    Debug.Log("Raycast hit pero layer no válido (obstáculo). isFov = false");
                    // Reanudar escaneo si desperate
                    
                    ia.fsm.SendInput(EnemyState.Calm);
                }
            }
        }
        else
        {
            // No hit (e.g., obstáculo bloquea)
            if (isFov)
            {
                isFov = false;
                Debug.Log("Raycast no hit (bloqueado). isFov = false");
                // Reanudar escaneo si desperate
               
                ia.fsm.SendInput(EnemyState.Calm);
            }
        }

        // Si no desperate, forzar reset
        if (!desperate)
        {
            if (isFov)
            {
                isFov = false;
                Debug.Log("No desperate: Forzando isFov = false");
            }
           
            if (facingCoroutine != null)
            {
                StopCoroutine(facingCoroutine);
                facingCoroutine = null;
            }

            ia.fsm.SendInput(EnemyState.Calm);
        }
    }

    // Coroutine para girar suavemente hacia el jugador
    private IEnumerator GirarHaciaJugador(Vector3 direction)
    {
        Quaternion initialRotation = root.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up); // Mirar hacia la dirección, manteniendo arriba

        // Solo rotar en Y (yaw) para evitar inclinaciones
        Vector3 initialEuler = initialRotation.eulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;
        targetEuler.x = initialEuler.x; // Mantener pitch (X)
        targetEuler.z = initialEuler.z; // Mantener roll (Z)
        targetRotation = Quaternion.Euler(targetEuler);

        float time = 0f;
        while (time < 1f) // Normalizar a 0-1 para Slerp
        {
            time += Time.deltaTime * rotationSpeed;
            root.rotation = Quaternion.Slerp(initialRotation, targetRotation, time);
            yield return null;
        }

        // Asegurar llegada exacta
        root.rotation = targetRotation;
        facingCoroutine = null;
    }

    public void SetDesperate(bool value)
    {
        desperate = value;
        // Solo resetear isFov si no estamos viendo al jugador
        if (value && isFov)
        {
            // Mantener detección si ya la tenemos
            Debug.Log("Desperate activado, pero ya detectando jugador. Manteniendo isFov.");
        }
        else if (!value)
        {
            isFov = false;
            Debug.Log("Desperate desactivado. isFov = false");
        }
        if (facingCoroutine != null)
        {
            StopCoroutine(facingCoroutine);
            facingCoroutine = null;
        }
        // Si se activa desperate y no vemos al jugador, iniciar escaneo
    
    }

    private void OnDrawGizmos()
    {
        if (target != null && root != null)
        {
            Gizmos.color = isFov ? Color.green : Color.yellow; // Verde si detectado
            Gizmos.DrawLine(root.position, target.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(root.position, viewDistanceMin);

            Vector3 left = Quaternion.Euler(0, -AngleMin / 2f, 0) * root.forward;
            Vector3 right = Quaternion.Euler(0, AngleMin / 2f, 0) * root.forward;
            Gizmos.color = Color.red;

            Gizmos.DrawRay(root.position, left * viewDistanceMin);
            Gizmos.DrawRay(root.position, right * viewDistanceMin);

            // Dibujar Raycast para debug
            if (target != null)
            {
                Vector3 dir = (target.position - root.position).normalized;
                Gizmos.color = Physics.Raycast(root.position, dir, viewDistanceMin, ToDetect) ? Color.green : Color.red;
                Gizmos.DrawRay(root.position, dir * viewDistanceMin);
            }
        }
    }
}