public class FactoryMelee : Factory<Enemi>
{
    public Enemi prefab;
    public int PoolAmount = 5;

    private ObjectPool<Enemi> _pool;

    void Awake()
    {
        _pool = new ObjectPool<Enemi>(CreatePrefab, Desactive, Active, PoolAmount);
    }

    public override Enemi Create()
    {
        return _pool.Get();
    }

    public override void Return(Enemi enemi)
    {
        _pool.Return(enemi);
    }

    Enemi CreatePrefab()
    {
        Enemi newEnemi = Instantiate(prefab);
        newEnemi.transform.SetParent(transform);
        newEnemi.gameObject.SetActive(false);
        return newEnemi;
    }

    void Active(Enemi enemi)
    {
        enemi.gameObject.SetActive(true);
    }

    void Desactive(Enemi enemi)
    {
        enemi.Refresh();
        enemi.gameObject.SetActive(false);
    }
}