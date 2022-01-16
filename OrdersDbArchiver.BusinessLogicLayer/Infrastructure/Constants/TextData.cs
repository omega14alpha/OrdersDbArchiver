namespace OrdersDbArchiver.BusinessLogicLayer.Infrastructure.Constants
{
    internal static class ErrorTextData
    {
        public static string ArgumentCannotBeNull = "Argument '{0}' cannot be equals null.\n";

        public static string TransferringFileError = "An error occurred while transferring the file.\n";

        public static string SavingErrorAndDataRolledBack = "When saving data to the database, an error occurred, data was rolled back.\n";
    }

    internal static class MessageTextData
    {
        public static string FileHasBeenSaved = "File {0} has been saved.\n";

        public static string OperationCancel = "Operation canceled.\n";

        public static string FileDetected = "Detected file -> {0}.\n";

        public static string CheckDb = "Check database ";

        public static string StartWork = "\nStart work.\n";
    }
}
