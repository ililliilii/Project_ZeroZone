using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, null, "Weaponpoint")]
public class WeaponPoint : MonoBehaviour
{
    public bool Gunpoint;
    public bool Rifle;
    public bool Pistol;
    public bool W_NOHA;
    public bool W_ENHA;
    public bool W_LUCY;
    public bool player;
    public Transform[] target;
    public Vector3 rotationOffset;
    public Vector3[] Weapon_positionOffset;
    void Awake()
    {
        if (player)
        {
            W_NOHA = GameData.Instance.Pick_NOHA;
            W_ENHA = GameData.Instance.Pick_ENHA;
            W_LUCY = GameData.Instance.Pick_LUCY;
        }

        if (!Gunpoint)
        {
            if (W_NOHA)
            {
                rotationOffset = new Vector3(0, 0, 0);
            }

            if (W_ENHA)
            {
                rotationOffset = new Vector3(0, 180, 0);
            }

            if (W_LUCY)
            {
                rotationOffset = new Vector3(0, 180, 0);
            }
        }

        if (Gunpoint)
        {
            return;
        }
    }

    void LateUpdate()
    {
        if (!Gunpoint)
        {
            if (target == null)
                return;
            if (W_NOHA)
            {
                transform.position = target[0].position;
                transform.rotation = target[0].rotation * Quaternion.Euler(rotationOffset);
            }

            if (W_ENHA)
            {
                transform.position = target[1].position;
                transform.rotation = target[1].rotation * Quaternion.Euler(rotationOffset);
            }

            if (W_LUCY)
            {
                transform.position = target[2].position;
                transform.rotation = target[2].rotation * Quaternion.Euler(rotationOffset);
            }
        }

        if (Gunpoint)
        {
            if (Rifle)
            {
                transform.position = target[0].position + Weapon_positionOffset[0];
            }

            if (Pistol)
            {
                transform.position = target[0].position + Weapon_positionOffset[1];
            }
        }
    }
}
