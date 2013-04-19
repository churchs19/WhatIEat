using Shane.Church.Utility.Core;
using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.WP.Data
{
	public class PhoneEntryRepository : IRepository<IEntry>
	{
		public IQueryable<IEntry> GetAllEntries()
		{
			using (PhoneDataContext context = new PhoneDataContext())
			{
				return context.Entries.Select(it => (IEntry)it).ToList().AsQueryable();
			}
		}

		public IQueryable<IEntry> GetFilteredEntries(System.Linq.Expressions.Expression<Func<IEntry, bool>> filter)
		{
			var filterDelegate = filter.Compile();
			using (PhoneDataContext context = new PhoneDataContext())
			{
				var results = context.Entries.Select(it => (IEntry)it).ToList().Where(it => filterDelegate(it)).AsQueryable();
				return results;
			}
		}

		public void DeleteEntry(IEntry entry)
		{
			using (PhoneDataContext context = new PhoneDataContext())
			{
				var pEntry = context.Entries.Where(it => it.EntryGuid == entry.EntryGuid).FirstOrDefault();
				if (pEntry != null)
				{
					context.Entries.DeleteOnSubmit(pEntry);
					context.SubmitChanges();
				}
			}
		}

		public IEntry AddOrUpdateEntry(IEntry entry)
		{
			if (!string.IsNullOrWhiteSpace(entry.EntryText))
			{

				using (PhoneDataContext context = new PhoneDataContext())
				{
					var pEntry = context.Entries.Where(it => it.EntryGuid == entry.EntryGuid).FirstOrDefault();
					if (pEntry != null)
					{
						pEntry.EntryId = entry.EntryId;
						pEntry.EntryText = entry.EntryText;
						pEntry.EntryDate = entry.EntryDate;
						pEntry.EditDateTime = DateTime.Now.ToUniversalTime();
					}
					else
					{
						pEntry = new PhoneEntry();
						pEntry.EntryGuid = (entry.EntryGuid.Equals(Guid.Empty)) ? Guid.NewGuid() : entry.EntryGuid;
						pEntry.EntryId = entry.EntryId;
						pEntry.EntryText = entry.EntryText;
						pEntry.EntryDate = entry.EntryDate;
						pEntry.CreateDateTime = DateTime.Now.ToUniversalTime();
						pEntry.EditDateTime = DateTime.Now.ToUniversalTime();
						context.Entries.InsertOnSubmit(pEntry);
					}
					context.SubmitChanges();
					return pEntry;
				}
			}
			return entry;
		}
	}
}
