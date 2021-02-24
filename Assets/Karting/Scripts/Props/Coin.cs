using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KartGame.KartSystems;

public class Coin : MonoBehaviour
{
    //Velocidad de rotacion iddle de la moneda
    [SerializeField]
    private float rotSpeed = 5.0f;

    //Material
    Material material;

    //Particulas
    ParticleSystem particles;

    //Fuente de audio
    AudioSource audiSource;

    // Start is called before the first frame update
    void Start()
    {
        //Accede al sistema de particulas
        particles = GetComponent<ParticleSystem>();

        //Acceso al material para ocultarlo al chocarse contra el
        material = GetComponent<Renderer>().material;

        audiSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Hace girar la moneda en estado de iddle
        transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
    }


    /// <summary>
    /// Destruye la moneda de forma visual
    /// </summary>
    private void destroyCoin()
    {
        //Pone el alpha del material al 0 eliminandola visualmente
        material.SetColor("_Color", new Color(0, 0, 0, 0));
        
        //Desactiva el collider
        GetComponent<Collider>().enabled = false;

        //Emite sonido de moneda
        audiSource.Play();

        //Emite el estallido de particulas
        particles.Play();
    }

    /// <summary>
    /// Al chocarse contra algo
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //Si se choca contra un collider del coche
        if (other.gameObject.layer == LayerMask.NameToLayer("Kart"))
        {
            ArcadeKart kart = other.transform.parent.GetComponent<ArcadeKart>();

            kart.temporalCoins++;

            destroyCoin();
        }
    }

}
