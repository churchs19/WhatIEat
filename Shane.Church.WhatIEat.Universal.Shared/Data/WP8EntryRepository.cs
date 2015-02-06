using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Storage.Extensions;
using Windows.Storage;

namespace Shane.Church.WhatIEat.Core.WP8.Data
{
	public class WP8EntryRepository : IRepository<IEntry>, IDisposable
	{
		public const string DBFileName = "whatieat.db3";
		public static string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DBFileName);

		private Context _context; 
		
		private static object _lock = new object();

		public WP8EntryRepository()
		{
			_context = new Context("whatieat", DatabaseLocation.Local);
		}

		public static async Task<bool> DbExists()
		{
			try
			{
				var item = await ApplicationData.Current.LocalFolder.GetItemAsync(DBFileName);
				return true;
			}
			catch { return false; }
		}

		public void Dispose()
		{
			_context.SaveChanges();
			_context.CloseDatabase();
		}

		public IQueryable<IEntry> GetAllEntries(bool includeDeleted = false)
		{
			lock (_lock)
			{
				if (includeDeleted)
					return _context.GetAll<WP8Entry>().Select(it => (IEntry)it).ToList().AsQueryable();
				else
				{
					var entries = _context.GetAll<WP8Entry>().Where(it => !it.IsDeleted).ToList();
					return entries.Select(it => it.AsIEntry()).ToList().AsQueryable();
				}
			}
		}

		public IQueryable<IEntry> GetFilteredEntries(System.Linq.Expressions.Expression<Func<IEntry, bool>> filter, bool includeDeleted = false)
		{
			lock (_lock)
			{

				var filterDelegate = filter.Compile();
				var allResultsDb = includeDeleted ? _context.GetAll<WP8Entry>().ToList() : _context.GetAll<WP8Entry>().Where(it => !it.IsDeleted).ToList();
				var allResults = allResultsDb.Select(it => it.AsIEntry()).ToList();
				var results = allResults.Where(it => filterDelegate(it)).ToList();
				return results.AsQueryable();
			}
		}

		public void DeleteEntry(IEntry entry, bool hardDelete = false)
		{
			lock (_lock)
			{
				var pEntry = _context.GetAll<WP8Entry>().Where(it => it.EntryGuid == entry.EntryGuid).FirstOrDefault();
				if (pEntry != null)
				{
					if (hardDelete)
						_context.Delete<WP8Entry>(pEntry);
					else
					{
						pEntry.EditDateTime = DateTimeOffset.Now.UtcDateTime;
						pEntry.IsDeleted = true;
						_context.Update<WP8Entry>(pEntry);
					}
					_context.SaveChanges();
				}
			}
		}

		public IEntry AddOrUpdateEntry(IEntry entry)
		{
			if (!string.IsNullOrWhiteSpace(entry.EntryText))
			{
				lock (_lock)
				{
					var pEntry = _context.GetAll<WP8Entry>().Where(it => it.EntryGuid == entry.EntryGuid).SingleOrDefault();
					if (pEntry != null)
					{
						pEntry.EntryId = entry.EntryId;
						pEntry.EntryText = entry.EntryText;
						pEntry.EntryDate = entry.EntryDate;
						pEntry.EditDateTime = DateTimeOffset.Now.UtcDateTime;
						pEntry.MealType = entry.MealType.HasValue ? entry.MealType.Value : MealType.Undefined;
						_context.Update<WP8Entry>(pEntry);
					}
					else
					{
						pEntry = new WP8Entry();
						pEntry.EntryGuid = (entry.EntryGuid.Equals(Guid.Empty)) ? Guid.NewGuid() : entry.EntryGuid;
						pEntry.EntryId = entry.EntryId;
						pEntry.EntryText = entry.EntryText;
						pEntry.EntryDate = entry.EntryDate;
						pEntry.MealType = entry.MealType.HasValue ? entry.MealType.Value : MealType.Undefined;
						pEntry.CreateDateTime = DateTimeOffset.Now.UtcDateTime;
						pEntry.EditDateTime = DateTimeOffset.Now.UtcDateTime;
						_context.Insert<WP8Entry>(pEntry);
					}
					_context.SaveChanges();
					return pEntry.AsIEntry();
				}
			}
			return entry;
		}
	}
}
