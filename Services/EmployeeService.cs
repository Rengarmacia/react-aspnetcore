using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using kuras.Models;

namespace kuras.Services
{
    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _employee;

        public EmployeeService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _employee = database.GetCollection<Employee>(settings.EmployeesCollectionName);
        }

        public List<Employee> Get() =>
            _employee.Find(employee => true).ToList();

        public Employee Get(string id) =>
            _employee.Find<Employee>(employee => employee.Id == id).FirstOrDefault();

        public Employee Create(Employee employee)
        {
            _employee.InsertOne(employee);
            return employee;
        }

        public void Update(string id, Employee employeeIn) =>
            _employee.ReplaceOne(employee => employee.Id == id, employeeIn);

        public void Remove(Employee employeeIn) =>
            _employee.DeleteOne(employee => employee.Id == employeeIn.Id);

        public void Remove(string id) => 
            _employee.DeleteOne(employee => employee.Id == id);
    }
}