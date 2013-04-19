using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Shane.Church.WhatIEat.Core.Data
{
	public interface IRepository<T>
		where T : class
	{
		/// <summary>
		/// Get all entries.
		/// </summary>
		/// <returns></returns>
		IQueryable<T> GetAllEntries();

		/// <summary>
		/// Get filtered entries.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		IQueryable<T> GetFilteredEntries(Expression<Func<T, bool>> filter);

		/// <summary>
		/// DeleteEntry
		/// </summary>
		/// <param name="entry"></param>
		void DeleteEntry(T entry);

		/// <summary>
		/// Add or update an entry.
		/// </summary>
		/// <param name="Entry"></param>
		/// <returns></returns>
		T AddOrUpdateEntry(T entry);
	}
}
