using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Shane.Church.WhatIEat.Core.Data;
using Shane.Church.WhatIEat.Core.Services;
using System;
using System.Text;
using System.Windows.Input;

namespace Shane.Church.WhatIEat.Core.ViewModels
{
	public class ExcelExportViewModel : ObservableObject
	{
		protected IRepository<IEntry> _repository;
		protected ISkyDriveService _skyDriveService;
		protected ILoggingService _log;

		public ExcelExportViewModel(IRepository<IEntry> repository, ISkyDriveService skyDrive, ILoggingService log)
		{
			if (repository == null)
				throw new ArgumentNullException("repository");
			_repository = repository;
			if (skyDrive == null)
				throw new ArgumentNullException("skyDrive");
			_skyDriveService = skyDrive;
			if (log == null)
				throw new ArgumentNullException("log");
			_log = log;

			ExportCommand = new RelayCommand(CsvExport);
		}

		private bool _isUploading;
		public bool IsUploading
		{
			get { return _isUploading; }
			set
			{
				Set(() => IsUploading, ref _isUploading, value);
			}
		}

		public delegate void ExportBeginningHandler();
		public event ExportBeginningHandler ExportBeginning;

		public delegate void ExportCompletedHandler(bool isSuccess);
		public event ExportCompletedHandler ExportCompleted;

		public ICommand ExportCommand { get; private set; }

		public async void CsvExport()
		{
			bool exportResult = false;
			IsUploading = true;
			_log.LogMessage("ExportingToExcel");
			try
			{
				if (ExportBeginning != null)
					ExportBeginning();
				var entries = _repository.GetAllEntries();
				StringBuilder csvString = new StringBuilder();
				csvString.AppendLine("EntryId,EntryGuid,EntryDate,EntryText,CreateDateTime,EditDateTime");
				foreach (var e in entries)
				{
					csvString.AppendLine(e.ToCsvString());
				}

				string filenameFormat = "WhatIEatExport-{0}.csv";
				string filename = string.Format(filenameFormat, DateTime.Now.ToString("s").Replace(':', '-'));

				exportResult = await _skyDriveService.SaveToSkyDrive(new SkyDriveSaveArgs() { Filename = filename, Content = csvString.ToString(), Encoding = Encoding.Unicode });
			}
			catch (Exception ex)
			{
				_log.LogException(ex, "Excel Export Exception");
			}
			finally
			{
				IsUploading = false;
				if (ExportCompleted != null)
					ExportCompleted(exportResult);
			}
		}
	}
}
