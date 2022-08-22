using Chassis;
using Service.Identity.Migrations;
using Service.PersonalData.Models;

namespace Service.PersonalData.Scheme;

public static class KycScanDbSchema
{
    public static string Table => CreateKycScansTable.TableName;

    public static class Columns
    {
        public static string FileName { get; }
        public static string PersonalDataId { get; }
        public static string FileExtension { get; }
        public static string OriginalName { get; }
        public static string ContentType { get; }

        static Columns()
        {
            FileName = nameof(KycScanDatabaseModel.FileName).ToSnakeCase();
            PersonalDataId = nameof(KycScanDatabaseModel.PersonalDataId).ToSnakeCase();
            FileExtension = nameof(KycScanDatabaseModel.FileExtension).ToSnakeCase();
            OriginalName = nameof(KycScanDatabaseModel.OriginalName).ToSnakeCase();
            ContentType = nameof(KycScanDatabaseModel.ContentType).ToSnakeCase();
        }
    }
}
