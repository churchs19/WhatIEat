using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.WP.Data
{
	[Table]
	public class PhoneEntry : ChangingObservableObject, IEntry
	{
		public PhoneEntry() { }

		private long _entryId;
		[Column(CanBeNull = false)]
		public long EntryId
		{
			get { return _entryId; }
			set
			{
				Set(() => EntryId, ref _entryId, value);
			}
		}
		private Guid _entryGuid;
		[Column(IsPrimaryKey = true, CanBeNull = false, DbType = "UNIQUEIDENTIFIER NOT NULL")]
		public Guid EntryGuid
		{
			get { return _entryGuid; }
			set
			{
				Set(() => EntryGuid, ref _entryGuid, value);
			}
		}
		private DateTime _entryDate;
		[Column(CanBeNull = false)]
		public DateTime EntryDate
		{
			get { return _entryDate.Date; }
			set
			{
				Set(() => EntryDate, ref _entryDate, value.ToUniversalTime());
			}
		}
		private string _entryText;
		[Column(CanBeNull = false)]
		public string EntryText
		{
			get { return _entryText; }
			set
			{
				Set(() => EntryText, ref _entryText, value);
			}
		}

		private DateTime _createDateTime;
		[Column(CanBeNull = false)]
		public DateTime CreateDateTime
		{
			get { return _createDateTime + DateTimeOffset.Now.Offset; }
			set
			{
				Set(() => CreateDateTime, ref _createDateTime, value.ToUniversalTime());
			}
		}

		private DateTime _editDateTime;
		[Column(CanBeNull = false)]
		public DateTime EditDateTime
		{
			get { return _editDateTime + DateTimeOffset.Now.Offset; }
			set
			{
				Set(() => EditDateTime, ref _editDateTime, value.ToUniversalTime());
			}
		}
	}
}
