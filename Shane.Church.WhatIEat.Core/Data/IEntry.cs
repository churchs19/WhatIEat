using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.Data
{
	public interface IEntry
	{
		long EntryId { get; set; }
		Guid EntryGuid { get; set; }
		DateTime EntryDate { get; set; }
		string EntryText { get; set; }
		DateTime CreateDateTime { get; set; }
		DateTime EditDateTime { get; set; }
	}
}
