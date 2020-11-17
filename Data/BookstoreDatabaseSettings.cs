namespace kuras.Models
{
    public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
    {
        public string KilometersCollectionName { get; set; }
        public string BooksCollectionName { get; set; }
        public string CarsCollectionName { get; set; }
        public string CarFilesCollectionName { get; set; }
        public string EmployeesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IBookstoreDatabaseSettings
    {
        string KilometersCollectionName { get; set; }
        string CarsCollectionName { get; set; }
        string BooksCollectionName { get; set; }
        string CarFilesCollectionName { get; set; }
        string EmployeesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}