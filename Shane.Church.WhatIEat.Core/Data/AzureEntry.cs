using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using System.Runtime.Serialization;

namespace Shane.Church.WhatIEat.Core.Data
{
	[DataContract]
	public class AzureEntry
	{
		public AzureEntry()
		{

		}

		public AzureEntry(IEntry source)
		{
			Id = source.EntryId;
			EntryGuid = source.EntryGuid;
			EntryDate = source.EntryDate;
			EntryText = source.EntryText;
			CreateDateTime = source.CreateDateTime;
			EditDateTime = source.EditDateTime;
			IsDeleted = source.IsDeleted.HasValue ? source.IsDeleted.Value : false;
			MealType = source.MealType;
		}

		[DataMember(Name = "id")]
		public long Id { get; set; }
		[DataMember]
		public Guid EntryGuid { get; set; }
		[DataMember]
		public DateTime EntryDate { get; set; }
		[DataMember]
		public string EntryText { get; set; }
		[DataMember]
		public DateTimeOffset CreateDateTime { get; set; }
		[DataMember]
		public DateTimeOffset EditDateTime { get; set; }
		[DataMember]
		public string UserId { get; set; }
		[DataMember]
		public bool IsDeleted { get; set; }
		[DataMember]
		public MealType MealType { get; set; }

		public IEntry GetEntry()
		{
			IEntry entry = KernelService.Kernel.Get<IEntry>();
			entry.EntryId = Id;
			entry.EntryGuid = EntryGuid;
			entry.EntryDate = EntryDate;
			entry.EntryText = EntryText;
			entry.CreateDateTime = CreateDateTime;
			entry.EditDateTime = EditDateTime;
			entry.IsDeleted = IsDeleted;
			entry.MealType = MealType;
			return entry;
		}
	}
}
