using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, null, "Weapan")]
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private Slider AmmoBar;
    public UnityEngine.UI.Text AmmoText;
    public enum Type
    {
        Melee,
        Range,
        Rifle
    };
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;
    private bool Reroading = false;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    // Start is called before the first frame update
    public void Update()
    {
        if (AmmoBar != null)
        {
            AmmoBar.maxValue = maxAmmo;
            AmmoBar.value = curAmmo;
        }

        if (AmmoText != null)
            AmmoText.text = curAmmo + " / " + maxAmmo;
    }

    public bool Use()
    {
        if (Reroading)
            return false;
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
            return true;
        }
        else if (type == Type.Range && curAmmo > 0 && !Reroading)
        {
            curAmmo--;
            StartCoroutine("Shot");
            return true;
        }
        else if (type == Type.Rifle && curAmmo > 0 && !Reroading)
        {
            curAmmo--;
            StartCoroutine("Shot");
            return true;
        }

        return false;
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;
    /*GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody CaseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        CaseRigid.AddForce(caseVec, ForceMode.Impulse);
        CaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);*/
    }

    public bool TryStartReload()
    {
        if (type == Type.Melee || Reroading || curAmmo >= maxAmmo)
            return false;
        Reroading = true;
        return true;
    }

    public int CompleteReload(int availableAmmo)
    {
        if (!Reroading)
            return 0;
        int loaded = Mathf.Min(Mathf.Max(0, availableAmmo), Mathf.Max(0, maxAmmo - curAmmo));
        curAmmo += loaded;
        Reroading = false;
        return loaded;
    }

    public void CancelReload()
    {
        Reroading = false;
    }

    public void StopUse()
    {
        StopAllCoroutines();
        CancelReload();
        if (meleeArea != null)
            meleeArea.enabled = false;
        if (trailEffect != null)
            trailEffect.enabled = false;
    }

    void OnDisable()
    {
        StopUse();
    }
}
