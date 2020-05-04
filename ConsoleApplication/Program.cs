using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NinjaDomain.Classes;
using NinjaDomain.DataModel;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
            //InsertNinja();
            //InsertMultipleNinjas();
            //SimpleNinjaQueries();
            //QueryAndUpdateNinja();
            //QueryAndUpdateNinjaDisconnected();
            //RetrieveDataWithFind();
            //RetrieveDataWithStoredProc();
            InsertNinjaWithEquipment();
            Console.ReadLine();
        }

        private static void InsertNinja()
        {
            var ninja = new Ninja()
            {
                Name = "SampsonSan",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(2008, 1, 28),
                ClanId = 1
            };

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Add(ninja);
                context.SaveChanges();
            }

        }

        private static void InsertMultipleNinjas()
        {
            var ninja1 = new Ninja()
            {
                Name = "Leonardo",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1984,1,1),
                ClanId = 1
            };
            var ninja2 = new Ninja()
            {
                Name = "Raphael",
                ServedInOniwaban = false,
                DateOfBirth = new DateTime(1985, 1, 1),
                ClanId = 1
            };

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.AddRange(new List<Ninja> {ninja1, ninja2});
                context.SaveChanges();
            }
        }

        private static void SimpleNinjaQueries()
        {
            using (var context = new NinjaContext())
            {
              //  var ninjas = context.Ninjas.ToList();
                
                //var ninja = context.Ninjas
                //    .FirstOrDefault(n => n.DateOfBirth >= new DateTime(1984, 1, 1));
                var ninja = context.Ninjas
                    .Where(n => n.DateOfBirth <= new DateTime(1984, 1, 1))
                    .OrderBy(n => n.Name)
                    .Skip(1).Take(1)
                    .FirstOrDefault();
                
                //var query = context.Ninjas;
                //var someninjas = query.ToList();
                //foreach (var ninja in query)
                //{
                   Console.WriteLine(ninja.Name);
                //}
            }
        }

        private static void QueryAndUpdateNinja()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.FirstOrDefault();
                ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);
                context.SaveChanges();
            }
        }

        private static void QueryAndUpdateNinjaDisconnected()
        {
            Ninja ninja;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
            }

            ninja.ServedInOniwaban = (!ninja.ServedInOniwaban);

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //context.Ninjas.Add(ninja); // Insert
                context.Ninjas.Attach(ninja); // EF, please watch this instance
                context.Entry(ninja).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private static void RetrieveDataWithFind()
        {
            var keyval = 4;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.Find(keyval);
                Console.WriteLine("After Find#1:" + ninja.Name);

                var someNinja = context.Ninjas.Find(keyval);
                Console.WriteLine("After Find#2:" + someNinja.Name);
                ninja = null;
            }
        }

        private static void RetrieveDataWithStoredProc()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas.SqlQuery("exec GetOldNinjas");
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        private static void DeleteNinja()
        {
            Ninja ninja;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                ninja = context.Ninjas.FirstOrDefault();
                //context.Ninjas.Remove(ninja);
                //context.SaveChanges();
            }

            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                //context.Ninjas.Remove(ninja);
                //context.Ninjas.Remove(ninja);
                context.Entry(ninja).State = EntityState.Deleted;
                context.SaveChanges();

            }
        }

        private static void DeleteNinjaWithKeyValue()
        {
            var keyval = 1;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninja = context.Ninjas.Find(keyval); // DB roundtrip #1
                context.Ninjas.Remove(ninja);
                context.SaveChanges(); // DB roundtrip #2
            }
        }

        private static void DeleteNinjaViaStoredProcedure()
        {
            var keyval = 3;
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Database.ExecuteSqlCommand(
                    "exec DeleteNinjaViaId {0}", keyval);
            }
        }

        private static void InsertNinjaWithEquipment()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                var ninja = new Ninja
                {
                    Name = "Kacy Catanzaro",
                    ServedInOniwaban = false,
                    DateOfBirth = new DateTime(1990, 1, 14),
                    ClanId = 1
                };
                var muscles = new NinjaEquipment
                {
                    Name = "Muscles",
                    Type = EquipmentType.Tool
                };
                var spunk = new NinjaEquipment
                {
                    Name = "Spunk",
                    Type = EquipmentType.Weapon
                };

                context.Ninjas.Add(ninja);
                ninja.EquipmentOwned.Add(muscles);
                ninja.EquipmentOwned.Add(spunk);
                context.SaveChanges();
            }
        }

        private static void SimpleNinjaGraphQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;

                //var ninja = context.Ninjas.Include(n => n.EquipmentOwned)// Eager Loading(bring related data)
                //    .FirstOrDefault(n => n.Name.StartsWith("Kacy"));
                var ninja = context.Ninjas
                    .FirstOrDefault(n => n.Name.StartsWith("Kacy"));
                Console.WriteLine("Ninja Retrieved:" + ninja.Name);
                context.Entry(ninja).Collection(n => n.EquipmentOwned).Load(); // Explicit Loading
                // lazy loading...
            }
        }

        private static void ProjectionQuery()
        {
            using (var context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas
                    .Select(n => new {n.Name, n.DateOfBirth, n.EquipmentOwned})
                    .ToList();
            }
        }

        private static void SeedDatabase()
        {

        }
    }
}
