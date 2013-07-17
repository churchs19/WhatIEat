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
			InitializeAdControl();
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			if (Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator)
			{
				AdControl.ApplicationId = "test_client";
				AdControl.AdUnitId = "Image480_80";
			}
			else
			{
				AdControl.ApplicationId = "111c32e1-4679-4ad0-9af0-c971de0562fa";
				AdControl.AdUnitId = "128211";
			}
#if PERSONAL
			AdControl.IsEnabled = false;
			AdControl.Height = 0;
#endif
		}

		private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
		{
			AdControl.Height = 0;
		}

		private void AdControl_AdRefreshed(object sender, EventArgs e)
		{
			AdControl.Height = 80;
		}
		#endregion

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			var selectedDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
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
				{
					this.newEntry.Text = "";
					//					this.Focus();
				}
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

		private void editTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (sender is RadTextBox)
			{
				Dispatcher.BeginInvoke(() =>
				{
					var tb = ((RadTextBox)sender);
					tb.Focus();
					tb.SelectionStart = tb.Text.Length;
				});
			}
		}

		private void Entries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			if (Entries.SelectedItem != null)
			{
				var item = Entries.GetContainerForItem(Entries.SelectedItem);
				if (item != null)
				{
					var position = e.GetPosition(item);
					if (!(position.X >= 0 && position.X <= item.ActualWidth && position.Y >= 0 && position.Y <= item.ActualHeight))
					{
						Entries.SelectedItem = null;
					}
				}
			}
		}

		private void editTextBox_ActionButtonTap(object sender, EventArgs e)
		{
			var tb = sender as RadTextBox;
			if (tb != null)
			{
				var viewModel = tb.DataContext as EntryViewModel;
				if (viewModel != null)
				{
					viewModel.SaveActionCompleted += entryModel_SaveActionCompleted;
				}
			}
		}

		private void entryModel_SaveActionCompleted(object sender, EventArgs e)
		{
			if (sender is EntryViewModel)
			{
				Dispatcher.BeginInvoke(() =>
				{
					((EntryViewModel)sender).SaveActionCompleted -= entryModel_SaveActionCompleted;
					Entries.SelectedItem = null;
				});
			}
		}
	}
}