using SQLite4Unity3d;
using UnityEngine;

namespace Logic
{
    public class Person
    {
        [PrimaryKey]
        public int ID { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public override string ToString()
        {
            return $"ID: {ID.ToString()}, Name: {Name}, Age: {Age.ToString()}";
        }
    }

    public static class SqliteTest
    {
        public static void CreateSqliteDB()
        {
            var ds = new DataService("Person.db");
            ds.CreateDB(typeof(Person));

            ds.InsertTable(new[] { new Person { ID = 1, Name = "Tom", Age = 15 }, new Person { ID = 2, Name = "Fred", Age = 16 } }, typeof(Person));
            ds.Close();
        }

        public static void LoadDB()
        {
            var personTable = new DataService("Person.db").GetTableRuntime().ToSearch<Person>();
            foreach (var person in personTable)
            {
                Debug.Log(person);
            }
        }
    }
}
