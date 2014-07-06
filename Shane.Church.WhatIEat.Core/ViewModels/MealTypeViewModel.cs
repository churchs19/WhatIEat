using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Strings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class MealTypeCollection : ObservableCollection<MealTypeViewModel>
	{
		private static MealTypeCollection _instance;

		private MealTypeCollection()
		{
			this.Add(new MealTypeViewModel(MealType.Breakfast));
			this.Add(new MealTypeViewModel(MealType.Lunch));
			this.Add(new MealTypeViewModel(MealType.Dinner));
			this.Add(new MealTypeViewModel(MealType.Snack));
			this.Add(new MealTypeViewModel(MealType.Undefined));
		}

		public static MealTypeCollection GetCollection()
		{
			if (_instance == null)
			{
				_instance = new MealTypeCollection();
			}
			return _instance;
		}
	}

	public class MealTypeViewModel : GalaSoft.MvvmLight.ObservableObject
	{
		public MealTypeViewModel()
			: this(MealType.Undefined)
		{

		}

		public MealTypeViewModel(MealType type)
		{
			MealType = type;
		}

		private MealType _type;
		public MealType MealType
		{
			get { return _type; }
			set
			{
				if (Set(() => MealType, ref _type, value))
				{
					RaisePropertyChanged(() => MealDescription);
				}
			}
		}

		public string MealDescription
		{
			get
			{
				string result;
				switch (_type)
				{
					case Data.MealType.Breakfast:
						result = Resources.MealTypeBreakfast;
						break;
					case Data.MealType.Lunch:
						result = Resources.MealTypeLunch;
						break;
					case Data.MealType.Dinner:
						result = Resources.MealTypeDinner;
						break;
					case Data.MealType.Snack:
						result = Resources.MealTypeSnack;
						break;
					default:
						result = Resources.MealTypeUndefined;
						break;
				}

				return result;
			}
		}
	}
}
