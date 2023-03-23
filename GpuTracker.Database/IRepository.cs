namespace GpuTracker.Database
{
    // T1 => Elment Type (eg gpu)
    // T2 => ID Type (string, guid, int)
    public interface IRepository<T1, T2>
    {
        T1 Create(T1 element);

        void Update(T1 element);

        void Delete(T2 id);

        public T1 Get(T2 id);

        public List<T1> Get();
    }
}
