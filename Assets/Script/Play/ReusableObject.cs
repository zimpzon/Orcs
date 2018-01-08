using System.Collections.Generic;

namespace Assets.Script
{
    public interface IObjectFactory<T>
    {
        T CreateObject();
    }

    public class ReusableObject<T> where T : new()
    {
        List<T> objects_ = new List<T>();
        IObjectFactory<T> objectFactoryMethod_;
        int idx_;

        public ReusableObject(int startCapacity, IObjectFactory<T> objectFactoryMethod)
        {
            // Capacity is increased by 50% at a time so we need at least two or we will be stuck at one forever
            if (startCapacity < 2)
                startCapacity = 2;

            objectFactoryMethod_ = objectFactoryMethod;
            ExpandCache(startCapacity);
        }

        public T GetObject()
        {
            if (idx_ >= objects_.Count)
                ExpandCache(objects_.Count / 2);

            return objects_[idx_++];
        }

        public void ReturnObject(T obj)
        {
            objects_[--idx_] = obj;
        }

        void ExpandCache(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                objects_.Add(objectFactoryMethod_.CreateObject());
            }
        }
    }
}
