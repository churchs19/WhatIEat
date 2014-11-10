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
		ILoggingService _log;

		public DateEdit()
		{
			InitializeComponent();

			InitializeAdControl();

			_log = KernelService.Kernel.Get<ILoggingService>();
		}

		#region Ad Control
		private void InitializeAdControl()
		{
#if !PERSONAL
            AdMediator_92B08B.AdSdkEvent += AdMediator_AdSdkEvent;
            AdMediator_92B08B.AdMediatorError += AdMediator_AdMediatorError;
            AdMediator_92B08B.AdMediatorFilled += AdMediator_AdMediatorFilled;
            AdMediator_92B08B.AdSdkError += AdMediator_AdSdkError;
#else
			AdPanel.Children.Remove(AdMediator_92B08B);
			AdMediator_92B08B = null;
#endif
        }

		void AdMediator_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
		{
			_log.LogMessage(String.Format("Ad SDK Error by {0} ErrorCode: {1} ErrorDescription: {2} Error: {3}", e.Name, e.ErrorCode, e.ErrorDescription, e.Error));
		}

		void AdMediator_AdMediatorFilled(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
		{
			_log.LogMessage(String.Format("Ad Filled:" + e.Name));
            if (AdMediator_92B08B != null)
			{
                AdMediator_92B08B.Visibility = System.Windows.Visibility.Visible;
			}
		}

		void AdMediator_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
		{
			_log.LogMessage(String.Format("AdMediatorError:" + e.Error + " " + e.ErrorCode));
            if (e.ErrorCode == Microsoft.AdMediator.Core.Events.AdMediatorErrorCode.NoAdAvailable && AdMediator_92B08B != null)
			{
				// AdMediator will not show an ad for this mediation cycle
                AdMediator_92B08B.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void AdMediator_AdSdkEvent(object sender, Microsoft.AdMediator.Core.Events.AdSdkEventArgs e)
		{
			_log.LogMessage(String.Format("AdSdk event {0} by {1}", e.EventName, e.Name));
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

            if (!model.AreAdsVisible && AdMediator_92B08B != null)
			{
                AdPanel.Children.Remove(AdMediator_92B08B);
                AdMediator_92B08B = null;
			}
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
			_log.LogMessage("Entry_Saved");
			if (sender is EntryViewModel)
			{
				Dispatcher.BeginInvoke(() =>
				{
					((EntryViewModel)sender).SaveActionCompleted -= entryModel_SaveActionCompleted;
					Entries.SelectedItem = null;
				});
			}
		}

		RadPickerBox openPicker = null;
		private void newMealTypePicker_PopupOpening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var picker = sender as RadPickerBox;
			if (picker != null)
			{
				if (openPicker != null)
				{
					openPicker.IsPopupOpen = false;
				}
				openPicker = picker;
				var transform = picker.TransformToVisual(this);
				var point = transform.Transform(new Point(0, 0));
			 
				picker.VerticalPopupOffset = picker.Margin.Top - picker.ActualHeight;
			}
		}

		private void editMealTypePicker_PopupOpening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var picker = sender as RadPickerBox;
			if (picker != null)
			{
				if (openPicker != null)
				{
					openPicker.IsPopupOpen = false;
				}
				openPicker = picker;
				var transform = picker.TransformToVisual(this);
				var point = transform.Transform(new Point(0, 0));

				picker.VerticalPopupOffset = picker.ActualHeight - 12;
			}
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var listBox = sender as ListBox;
			if (listBox != null && openPicker != null)
			{
				if (openPicker.DataContext is EntryViewModel)
				{
					((EntryViewModel)openPicker.DataContext).SaveCommand.Execute(null);
				} 
				openPicker.IsPopupOpen = false;
				openPicker = null;
			}
		}
	}
}