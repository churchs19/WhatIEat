using Shane.Church.Utility.Core;
using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Windows;

namespace Shane.Church.WhatIEat.Core.WP.Data
{
	public class PhoneEntryRepository : IRepository<IEntry>, IDisposable
	{
		private PhoneDataContext _context;
		private static object _lock = new object();

		public PhoneEntryRepository()
		{
			_context = new PhoneDataContext();
			_context.DeferredLoadingEnabled = true;
			_context.ObjectTrackingEnabled = true;
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		public IQueryable<IEntry> GetAllEntries()
		{
			lock (_lock)
			{
				return _context.Entries.Select(it => (IEntry)it).ToList().AsQueryable();
			}
		}

		public IQueryable<IEntry> GetFilteredEntries(System.Linq.Expressions.Expression<Func<IEntry, bool>> filter)
		{
			lock (_lock)
			{
				var filterDelegate = filter.Compile();
				var results = _context.Entries.Select(it => (IEntry)it).ToList().Where(it => filterDelegate(it)).AsQueryable();
				return results;
			}
		}

		public void DeleteEntry(IEntry entry)
		{
			lock (_lock)
			{
				var pEntry = _context.Entries.Where(it => it.EntryGuid == entry.EntryGuid).FirstOrDefault();
				if (pEntry != null)
				{
					_context.Entries.DeleteOnSubmit(pEntry);
					_context.SubmitChanges();
				}
			}
		}

		public IEntry AddOrUpdateEntry(IEntry entry)
		{
			if (!string.IsNullOrWhiteSpace(entry.EntryText))
			{
				lock (_lock)
				{
					var pEntry = _context.Entries.Where(it => it.EntryGuid == entry.EntryGuid).FirstOrDefault();
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
						_context.Entries.InsertOnSubmit(pEntry);
					}
					_context.SubmitChanges();
					return pEntry;
				}
			}
			return entry;
		}
	}
}
