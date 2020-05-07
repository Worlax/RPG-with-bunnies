using UnityEngine;

public class Lootable: MonoBehaviour
{
    // Properties //
    public Canvas canvas;
    public float lootDistance = 1f;

    public Transform windowPrefab;
    Transform window;
    Vector3 windowSpawnOffset = new Vector3(100, 0, 0);

    Transform user;

    // Functions //
    void Update()
    {
        if (user != null)
        {
            if (Vector3.Distance(user.position, transform.position) > lootDistance)
            {
                user = null;
                Close();
            }
        }
    }

    public void Open(Transform _user)
    {
        if (Vector3.Distance(_user.position, transform.position) <= lootDistance)
        {
            if (window == null)
            {
                CreateWindow();
            }

            user = _user;
            window.transform.localScale = Vector3.one;
        }
    }

    void CreateWindow()
    {
        window = Instantiate(windowPrefab);
        window.SetParent(canvas.transform);
        window.transform.position = Input.mousePosition + windowSpawnOffset;
        Close();
    }

    public void Close()
    {
        window.transform.localScale = Vector3.zero;
    }
}
