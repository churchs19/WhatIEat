using Microsoft.Phone.Data.Linq;
using Shane.Church.WhatIEat.Core.Universal.Data;
using System.Data.Linq;

public class PhoneDataContext : DataContext
{
	// Specify the connection string as a static, used in main page and app.xaml.
	public static string DBConnectionString = "Data Source=isostore:/WhatIEat.sdf";
	public static string DBFileName = "WhatIEat.sdf";
	public static int DBSchemaVersion = 4;

	// Pass the connection string to the base class.
	public PhoneDataContext()
		: base(DBConnectionString)
	{
		if (!this.DatabaseExists())
		{
			this.CreateDatabase();

			DatabaseSchemaUpdater dbUpdater = this.CreateDatabaseSchemaUpdater();
			dbUpdater.DatabaseSchemaVersion = DBSchemaVersion;
			dbUpdater.Execute();
		}
		else
		{
			// Check whether a database update is needed.
			DatabaseSchemaUpdater dbUpdater = this.CreateDatabaseSchemaUpdater();

			if (dbUpdater.DatabaseSchemaVersion < DBSchemaVersion)
			{
				if (dbUpdater.DatabaseSchemaVersion < 2)
				{
					//Perform v2 updates
					dbUpdater.AddColumn<PhoneEntry>("IsDeleted");
				}

				if (dbUpdater.DatabaseSchemaVersion < 3)
				{
					//Perform v3 updates
					dbUpdater.AddColumn<PhoneEntry>("MealType");
				}

				// Add the new database version.
				dbUpdater.DatabaseSchemaVersion = DBSchemaVersion;

				// Perform the database update in a single transaction.
				dbUpdater.Execute();
			}

		}
	}

	public Table<UniversalEntry> Entries;
}