using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using Shane.Church.Utility.Core.Command;
using Shane.Church.WhatIEat.Core.Services;
using Shane.Church.WhatIEat.Core.ViewModels;
using Shane.Church.WhatIEat.WP.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.PhoneTextBox;

namespace Shane.Church.WhatIEat.WP
{
	public partial class DateEdit : PhoneApplicationPage
	{
		bool colorPickerOpen = false;
		bool editColorPickerOpen = false;

		public DateEdit()
		{
			InitializeComponent();

			InitializeAdControl();
		}

		#region Ad Control
		private void InitializeAdControl()
		{
			AdControl.AdReceived += new Inneractive.Ad.InneractiveAd.IaAdReceived(AdControl_AdReceived);
			AdControl.AdFailed += new Inneractive.Ad.InneractiveAd.IaAdFailed(AdControl_AdFailed);
			AdControl.DefaultAdReceived += new Inneractive.Ad.InneractiveAd.IaDefaultAdReceived(AdControl_DefaultAdReceived);

#if PERSONAL
			AdPanel.Children.Remove(AdControl);
			AdControl = null;
#endif
		}

		void AdControl_DefaultAdReceived(object sender)
		{
			FlurryWP8SDK.Api.LogEvent("Unpaid Ad Received");
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdReceived(object sender)
		{
			FlurryWP8SDK.Api.LogEvent("Paid Ad Received");
			AdControl.Visibility = System.Windows.Visibility.Visible;
		}

		private void AdControl_AdFailed(object sender)
		{
			FlurryWP8SDK.Api.LogEvent("No Ad Received");
			AdControl.Visibility = System.Windows.Visibility.Collapsed;
		}
		#endregion

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			FlurryWP8SDK.Api.LogPageView();
			base.OnNavigatedTo(e);
		}

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
			FlurryWP8SDK.Api.LogEvent("Entry_Saved");
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