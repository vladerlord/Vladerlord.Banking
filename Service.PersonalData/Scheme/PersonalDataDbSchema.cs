using Chassis;
using Service.Identity.Migrations;
using Service.PersonalData.Models;

namespace Service.PersonalData.Scheme;

public static class PersonalDataDbSchema
{
	public static string Table => CreatePersonalDataTable.TableName;

	public static class Columns
	{
		public static string Id { get; }
		public static string UserId { get; }
		public static string FirstName { get; }
		public static string LastName { get; }
		public static string Country { get; }
		public static string City { get; }
		public static string Iv { get; }
		public static string Status { get; }

		static Columns()
		{
			Id = nameof(PersonalDataDatabaseModel.Id).ToSnakeCase();
			UserId = nameof(PersonalDataDatabaseModel.UserId).ToSnakeCase();
			FirstName = nameof(PersonalDataDatabaseModel.FirstName).ToSnakeCase();
			LastName = nameof(PersonalDataDatabaseModel.LastName).ToSnakeCase();
			Country = nameof(PersonalDataDatabaseModel.Country).ToSnakeCase();
			City = nameof(PersonalDataDatabaseModel.City).ToSnakeCase();
			Iv = nameof(PersonalDataDatabaseModel.Iv).ToSnakeCase();
			Status = nameof(PersonalDataDatabaseModel.Status).ToSnakeCase();
		}
	}
}