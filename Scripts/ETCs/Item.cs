using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Steak,
    Cake,
    Soda
}

public class Item : MonoBehaviour
{
    public int itemID;
    public Type type;
    
    public delegate void PickedDelegate(int itemID);
    public event PickedDelegate PickedEvent;
    private Coroutine _rotateCoroutine;
    
    // Start is called before the first frame update
    public void Init(int id)
    {
        itemID = id;
        _rotateCoroutine = StartCoroutine(RotateCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        PickedEvent?.Invoke(this.itemID);
    }

    public void Picked()
    {
        StopCoroutine(_rotateCoroutine);
        gameObject.SetActive(false);
    }
    
    public IEnumerator RotateCoroutine()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * (30 * Time.deltaTime),Space.World);
            yield return null;
        }
    }
}
