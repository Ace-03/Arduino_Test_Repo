using UnityEngine;

public class AnimationRandomizer : MonoBehaviour
{
    private float targetpos = 15;
    private float speed = 1;
    private bool dirToggle = false;

    private bool onCooldown = false;

    private float cooldownTime = 0.5f;
    private float timer = 0;
    private void Start()
    {
        dirToggle = Random.Range(0, 2) == 0 ? true : false;
        transform.position = new Vector3(Random.Range(-14f, 14f), transform.position.y, transform.position.z);
        speed = Random.Range(5f, 35f);
    }

    private void Update()
    {
        MovePosition(dirToggle);

        if (onCooldown)
        {
            timer += Time.deltaTime;
            if (timer > cooldownTime)
            {
                onCooldown = false;
                timer = 0f;
            }

            return;
        }

        if (Mathf.Abs(transform.position.x) > Mathf.Abs(targetpos))
        {
            dirToggle = !dirToggle;
            onCooldown = true;
        }
    }


    private void MovePosition(bool toggle)
    {
        int dir = toggle ? 1 : -1;
        transform.position += new Vector3(Time.deltaTime * speed * dir, 0, 0);
    }
}
