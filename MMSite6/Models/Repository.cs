using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMSite6.Models
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected MMSite6Context RepositoryContext { get; set; }

        public List<T> _list = new List<T>();

        public Repository(MMSite6Context repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }

        public ICollection<T> FindAll()
        {
            return this._list;
        }

        public ICollection<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _list;
        }

        public void Create(T entity)
        {
            this._list.Add(entity);
        }

        public void Update(T entity)
        {
            
        }

        public void Delete(T entity)
        {
            this._list.Remove(entity);
        }
    }
}
