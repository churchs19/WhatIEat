using GalaSoft.MvvmLight;
using Shane.Church.WhatIEat.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Telerik.Storage.Extensions;

namespace Shane.Church.WhatIEat.Core.WP8.Data
{
	public class WP8Entry : ObservableObject
	{
		public WP8Entry() { }

		private long _entryId;
		public long EntryId
		{
			get { return _entryId; }
			set
			{
				Set(() => EntryId, ref _entryId, value);
			}
		}
		private Guid _entryGuid;
		[Key]
		public Guid EntryGuid
		{
			get { return _entryGuid; }
			set
			{
				Set(() => EntryGuid, ref _entryGuid, value);
			}
		}
		private DateTime _entryDate;
		public DateTime EntryDate
		{
			get { return DateTime.SpecifyKind(_entryDate, DateTimeKind.Utc); }
			set
			{
				Set(() => EntryDate, ref _entryDate, value);
			}
		}
		private string _entryText;
		public string EntryText
		{
			get { return _entryText; }
			set
			{
				Set(() => EntryText, ref _entryText, value);
			}
		}

		private DateTime _createDateTime;
		public DateTime CreateDateTime
		{
			get { return _createDateTime; }
			set
			{
				Set(() => CreateDateTime, ref _createDateTime, value);
			}
		}

		private DateTime _editDateTime;
		public DateTime EditDateTime
		{
			get { return _editDateTime; }
			set
			{
				Set(() => EditDateTime, ref _editDateTime, value);
			}
		}

		private bool _isDeleted=false;
		public bool IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				Set(() => IsDeleted, ref _isDeleted, value);
			}
		}

		private MealType _mealType = Core.Data.MealType.Undefined;
		public MealType MealType
		{
			get 
			{
				return _mealType;
			}
			set
			{
				Set(() => MealType, ref _mealType, value);
			}
		}

		public IEntry AsIEntry()
		{
			return new WP8IEntry()
			{
				EntryId = EntryId,
				EntryGuid = EntryGuid,
				EntryDate = EntryDate,
				EntryText = EntryText,
				CreateDateTime = new DateTimeOffset(new DateTime(CreateDateTime.Ticks, DateTimeKind.Utc)),
				EditDateTime = new DateTimeOffset(new DateTime(EditDateTime.Ticks, DateTimeKind.Utc)),
				IsDeleted = IsDeleted,
				MealType = MealType
			};
		}
	}

	[SuppressSchemaGeneration]
	public class WP8IEntry : IEntry
	{

		public long EntryId { get; set; }

		public Guid EntryGuid { get; set; }

		public DateTime EntryDate { get; set; }

		public string EntryText { get; set; }

		public DateTimeOffset CreateDateTime { get; set; }

		public DateTimeOffset EditDateTime { get; set; }

		public bool? IsDeleted { get; set; }

		public MealType? MealType { get; set; }

		public string ToCsvString()
		{
			return "\"" + String.Join("\",\"", EntryId, EntryGuid, EntryDate, EntryText, MealType.ToString(), CreateDateTime, EditDateTime) + "\"";
		}
	}
}
