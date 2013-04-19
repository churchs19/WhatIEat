using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Telerik.Windows.Controls.PhoneTextBox;
using Telerik.Windows.Controls;
using Shane.Church.WhatIEat.WP.Helpers;
using Shane.Church.Utility.Core.Command;

namespace Shane.Church.WhatIEat.WP
{
	public partial class DateEdit : PhoneApplicationPage
	{
		public DateEdit()
		{
			InitializeComponent();
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			var selectedDate = DateTime.Now;
			try
			{
				selectedDate = (DateTime)PhoneApplicationService.Current.State["SelectedDate"];
			}
			catch { }

			var model = KernelService.Kernel.Get<DateEditViewModel>();
			model.LoadData(selectedDate);
			model.AddActionCompleted += (s, args) =>
			{
				var isSuccess = true;
				if (args is ValidationResultEventArgs)
					isSuccess = ((ValidationResultEventArgs)args).IsValid;

				if (isSuccess)
					this.newEntry.Text = "";
				else
					newEntry.ChangeValidationState(ValidationState.Invalid, "Required");
			};
			DataContext = model;
		}

		private void newEntry_TextChanged(object sender, TextChangedEventArgs e)
		{
			newEntry.ChangeValidationState(ValidationState.NotValidated, "");
		}

		private void Entries_SelectionChanging(object sender, Telerik.Windows.Controls.SelectionChangingEventArgs e)
		{
			var selector = this.Resources["SelectedItemSelector"] as EntryTemplateSelector;
			if (selector != null)
			{
				if (e.RemovedItems.Count > 0)
				{
					object oldItem = e.RemovedItems[0];
					RadDataBoundListBoxItem visualContainer = this.Entries.GetContainerForItem(oldItem) as RadDataBoundListBoxItem;
					if (visualContainer != null)
					{
						EntryTemplateSelectorWrapper wrapper = new EntryTemplateSelectorWrapper() { IsSelected = false, Model = oldItem };
						visualContainer.ContentTemplate = selector.SelectTemplate(wrapper, visualContainer);
						visualContainer.ApplyTemplate();
						visualContainer.UpdateLayout();
					}
				}
				if (e.AddedItems.Count > 0)
				{
					object oldItem = e.AddedItems[0];
					RadDataBoundListBoxItem visualContainer = this.Entries.GetContainerForItem(oldItem) as RadDataBoundListBoxItem;
					if (visualContainer != null)
					{
						EntryTemplateSelectorWrapper wrapper = new EntryTemplateSelectorWrapper() { IsSelected = true, Model = oldItem };
						visualContainer.ContentTemplate = selector.SelectTemplate(wrapper, visualContainer);
						visualContainer.ApplyTemplate();
						visualContainer.UpdateLayout();
					}
				}
			}
		}
	}
}