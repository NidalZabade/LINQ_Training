using System.Reflection;

class Program
{
    class Order
    {
        public int orderId;
        public string? customerName;
        public int amount;

    }
    class Employee
    {
        public int employeeId;
        public string? employeeName;
        public int salary;
    }

    class Student
    {
        public int studentId;
        public string? studentName;
    }

    class Grade
    {
        public int studentId;
        public int score;
    }


    static void Main(string[] args)
    {

        // Basic LINQ Filtering
        var products = new List<string> { "Laptop", "Mouse", "Keyboard", "Monitor", "Speaker" };
        var startWithMLambda = products.Where(product => product.ToLower().StartsWith("m"));
        foreach (var product in startWithMLambda)
        {
            System.Console.WriteLine($"{product}");
        }

        System.Console.WriteLine("----------------------------------------");

        var startWithMQuery = from product in products
                              where product.ToLower().StartsWith('m')
                              select product;
        foreach (var product in startWithMQuery)
        {
            System.Console.WriteLine($"{product}");
        }

        System.Console.WriteLine("----------------------------------------");

        // LINQ Grouping I
        var orders = new List<Order>
        {
            new Order { orderId = 1, customerName = "Ali", amount = 250 },
            new Order { orderId = 2, customerName = "Ahmad", amount = 150 },
            new Order { orderId = 3, customerName = "Ali", amount = 300 },
            new Order { orderId = 3, customerName = "Haya", amount = 300 }
        };

        var totalAmountSpentLambda = orders
            .GroupBy(order => order.customerName)
            .Select(spentGroupe => new
            {
                customerName = spentGroupe.Key,
                totalAmount = spentGroupe.Sum(order => order.amount)
            });

        foreach (var totalSpent in totalAmountSpentLambda)
        {
            System.Console.WriteLine($"{totalSpent.customerName}: {totalSpent.totalAmount}");
        }

        System.Console.WriteLine("----------------------------------------");

        var totalAmountSpentQuery = from order in orders
                                    group order by order.customerName into spentGroupe
                                    select new
                                    {
                                        customerName = spentGroupe.Key,
                                        totalAmount = spentGroupe.Sum(order => order.amount)
                                    };

        foreach (var totalSpent in totalAmountSpentQuery)
        {
            System.Console.WriteLine($"{totalSpent.customerName}: {totalSpent.totalAmount}");
        }

        System.Console.WriteLine("----------------------------------------");

        // LINQ Grouping II
        var employees = new List<Employee>
        {
            new Employee { employeeId = 1, employeeName = "Nidal", salary = 3000 },
            new Employee { employeeId = 2, employeeName = "Sara", salary = 4000 },
            new Employee { employeeId = 3, employeeName = "Areen", salary = 3500 },
            new Employee { employeeId = 4, employeeName = "Abdullah", salary = 3000 },
            new Employee { employeeId = 5, employeeName = "Ehab", salary = 1000 }, //:p
            new Employee { employeeId = 6, employeeName = "Afaf", salary = 3500 },
            new Employee { employeeId = 7, employeeName = "Worood", salary = 3500 }
        };

        var groupBySalaryLambda = employees
            .GroupBy(e => e.salary)
            .Where(g => g.Count() > 2)
            .Select(g => new
            {
                salary = g.Key,
                employees = g.Select(e => e.employeeName).OrderBy(name => name).ToList()
            });

        foreach (var salaryGroupe in groupBySalaryLambda)
        {
            System.Console.WriteLine($"Salary: {salaryGroupe.salary}, Employees: {{{string.Join(", ", salaryGroupe.employees)}}}");
        }

        System.Console.WriteLine("----------------------------------------");

        var groupBySalaryQuery = from employee in employees
                                 group employee by employee.salary into g
                                 where g.Count() > 2
                                 select new
                                 {
                                     salary = g.Key,
                                     employees = (from e in g
                                                  orderby e.employeeName
                                                  select e.employeeName).ToList()
                                 };

        foreach (var salaryGroupe in groupBySalaryQuery)
        {
            System.Console.WriteLine($"Salary: {salaryGroupe.salary}, Employees: {{{string.Join(", ", salaryGroupe.employees)}}}");
        }

        System.Console.WriteLine("----------------------------------------");

        // LINQ Join

        var students = new List<Student>
        {
            new Student { studentId = 1, studentName = "Ali" },
            new Student { studentId = 2, studentName = "Sara" }
        };

        var grades = new List<Grade>
        {
            new Grade { studentId = 1, score = 85 },
            new Grade { studentId = 2, score = 92 }
        };

        var joinByStudentNameLambda = students.Join(grades, student => student.studentId, grade => grade.studentId, (student, grade) =>
         new
         {
             student.studentName,
             grade.score
         });

        foreach (var item in joinByStudentNameLambda)
        {
            Console.WriteLine($"{item.studentName}: {item.score}");
        }

        System.Console.WriteLine("----------------------------------------");

        var joinByStudentNameQuery = from student in students
                                     join grade in grades on student.studentId equals grade.studentId
                                     select new
                                     {
                                         student.studentName,
                                         grade.score
                                     };
        
        foreach (var item in joinByStudentNameQuery)
        {
            Console.WriteLine($"{item.studentName}: {item.score}");
        }






    }


}