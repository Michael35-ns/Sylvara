using UnityEngine;

public class ZonaMusical : MonoBehaviour
{
    public enum Nivel { Nivel1, Nivel2, Nivel3, Boss}
    public Nivel nivelAsignado;
    public MusicManager musicManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (nivelAsignado)
            {
                case Nivel.Nivel1:
                    musicManager.ReproducirMusica(musicManager.musicaNivel1);
                    break;
                case Nivel.Nivel2:
                    musicManager.ReproducirMusica(musicManager.musicaNivel2);
                    break;
                case Nivel.Nivel3:
                    musicManager.ReproducirMusica(musicManager.musicaNivel3);
                    break;
                case Nivel.Boss:
                    musicManager.ReproducirMusica(musicManager.musicaJefe);
                    break;
            }
        }
    }
}