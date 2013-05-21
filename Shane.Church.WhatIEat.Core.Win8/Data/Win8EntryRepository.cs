using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shane.Church.WhatIEat.Core.Win8.Data
{
    public class Win8EntryRepository : IRepository<IEntry>
    {
        public IQueryable<IEntry> GetAllEntries(bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<IEntry> GetFilteredEntries(System.Linq.Expressions.Expression<Func<IEntry, bool>> filter, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntry(IEntry entry, bool hardDelete = false)
        {
            throw new NotImplementedException();
        }

        public IEntry AddOrUpdateEntry(IEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}
