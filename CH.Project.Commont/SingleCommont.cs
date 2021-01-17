using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont
{
    public abstract class SingleCommont<T> where T : class, new()
    {
        protected readonly static object obj = new object();

        private static T Instance = new T();
        public static T CreateInstance()
        {
            lock (obj)
            {
                if (Instance == null)
                {
                    Instance = new T();
                }
            }
            return Instance;
        }
    }
}
