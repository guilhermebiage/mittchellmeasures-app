using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSite6.Models
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(MMSite6Context repositoryContext)
            : base(repositoryContext)
        {
            
        }
    }
}
