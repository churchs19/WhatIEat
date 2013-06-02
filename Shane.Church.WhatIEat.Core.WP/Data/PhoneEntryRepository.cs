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

		public IQueryable<IEntry> GetAllEntries(bool includeDeleted = false)
		{
			lock (_lock)
			{
				if (includeDeleted)
					return _context.Entries.Select(it => (IEntry)it).ToList().AsQueryable();
				else
					return _context.Entries.Where(it => !it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value)).Select(it => (IEntry)it).ToList().AsQueryable();
			}
		}

		public IQueryable<IEntry> GetFilteredEntries(System.Linq.Expressions.Expression<Func<IEntry, bool>> filter, bool includeDeleted = false)
		{
			lock (_lock)
			{
				var filterDelegate = filter.Compile();
				var allResults = _context.Entries.Select(it => (IEntry)it).ToList();
				var results = allResults.Where(it => includeDeleted ? filterDelegate(it) : filterDelegate(it) && (!it.IsDeleted.HasValue || (it.IsDeleted.HasValue && !it.IsDeleted.Value))).ToList();
				return results.AsQueryable();
			}
		}

		public void DeleteEntry(IEntry entry, bool hardDelete = false)
		{
			lock (_lock)
			{
				var pEntry = _context.Entries.Where(it => it.EntryGuid == entry.EntryGuid).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Entries.DeleteOnSubmit(pEntry);
					else
					{
						pEntry.EditDateTime = DateTimeOffset.Now;
						pEntry.IsDeleted = true;
					}
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
						pEntry.EditDateTime = DateTimeOffset.Now;
					}
					else
					{
						pEntry = new PhoneEntry();
						pEntry.EntryGuid = (entry.EntryGuid.Equals(Guid.Empty)) ? Guid.NewGuid() : entry.EntryGuid;
						pEntry.EntryId = entry.EntryId;
						pEntry.EntryText = entry.EntryText;
						pEntry.EntryDate = entry.EntryDate;
						pEntry.CreateDateTime = DateTimeOffset.Now;
						pEntry.EditDateTime = DateTimeOffset.Now;
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
