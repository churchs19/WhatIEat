using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

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
		[Column(CanBeNull = false, DbType = "DATETIME NOT NULL")]
		public DateTime EntryDate
		{
			get { return DateTime.SpecifyKind(_entryDate, DateTimeKind.Utc); }
			set
			{
				Set(() => EntryDate, ref _entryDate, value);
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

		private DateTimeOffset _createDateTime;
		[Column(CanBeNull = false, DbType = "DATETIME NOT NULL")]
		public DateTimeOffset CreateDateTime
		{
			get { return _createDateTime; }
			set
			{
				Set(() => CreateDateTime, ref _createDateTime, value);
			}
		}

		private DateTimeOffset _editDateTime;
		[Column(CanBeNull = false, DbType = "DATETIME NOT NULL")]
		public DateTimeOffset EditDateTime
		{
			get { return _editDateTime; }
			set
			{
				Set(() => EditDateTime, ref _editDateTime, value);
			}
		}

		[Column(IsVersion = true)]
		private Binary _version;

		private bool? _isDeleted;
		[Column(CanBeNull = true)]
		public bool? IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				Set(() => IsDeleted, ref _isDeleted, value);
			}
		}

		public string ToCsvString()
		{
			return "\"" + String.Join("\",\"", EntryId, EntryGuid, EntryDate, EntryText, CreateDateTime, EditDateTime) + "\"";
		}
	}
}
