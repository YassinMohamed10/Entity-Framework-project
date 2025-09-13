using LinQProject.Data;
using LinQProject.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace LinQProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] menu = { "Add", "Display", "Edit", "Delete", "Exit" };
            int select = 0;
            int cdist = (Console.WindowWidth - 10) / 2;
            int rdist = (Console.WindowHeight - 5) / (menu.Length + 1);
            bool flag = true;

            do
            {
                Console.Clear();
                for (int i = 0; i < menu.Length; i++)
                {
                    Console.BackgroundColor = (i == select) ? ConsoleColor.Green : ConsoleColor.Black;
                    Console.SetCursorPosition(cdist, rdist * (i + 1));
                    Console.WriteLine(menu[i]);
                }
                Console.ResetColor();

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        select = (select > 0) ? select - 1 : menu.Length - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        select = (select < menu.Length - 1) ? select + 1 : 0;
                        break;

                    case ConsoleKey.Enter:
                        switch (select)
                        {
                            case 0: ShowAddMenu(); break;
                            case 1: ShowDisplayMenu(); break;
                            case 2: ShowEditMenu(); break;
                            case 3: ShowDeleteMenu(); break;
                            case 4: flag = false; break;
                        }
                        break;
                }
            } while (flag);
        }

        //   Add
        public static void ShowAddMenu()
        {
            Console.Clear();
            Console.WriteLine("Select Adding for:");
            Console.WriteLine("------------******------------");
            Console.WriteLine("1-Employee\n\n2-Department\n\n3-project");
            Console.Write("\nSelect input:");
            int choice = int.Parse(Console.ReadLine());
            using var ctx = new CompanyDbContext();
            switch (choice)
            {
                case 1: AddEmployee(ctx); break;
                case 2: AddDepartment(ctx); break;
                case 3: AddProject(ctx); break;
            }
        }
        //   Add Employee
        public static void AddEmployee(CompanyDbContext ctx)
        {
            ctx.Database.EnsureCreated();
            Console.Clear();
            var employee = new Employee();
            Console.WriteLine("Employee data:");
            Console.WriteLine("######################");
            Console.Write("Enter first name:");
            employee.FirstName = Console.ReadLine();
            Console.WriteLine("*****************");
            Console.Write("Enter last name:");
            employee.LastName = Console.ReadLine();
            Console.WriteLine("*****************");
            Console.WriteLine("Select Dpartment:");
            Console.WriteLine("----------------");
            foreach (var dep in ctx.Departments)
            {
                Console.WriteLine($"ID:{dep.DepartmentId} ,name:{dep.Name}");
            }
            Console.Write("\nEnter ID number of selected Department:");
            int depId = int.Parse(Console.ReadLine());
            if (depId > 0)
            {
                var dep = ctx.Departments.Find(depId);
                if (dep == null)
                {
                    Console.WriteLine("Department not found, cannot assign employee.");
                    return;
                }
                employee.DepartmentId = depId;
            }
            ctx.Employees.Add(employee);
            ctx.SaveChanges();
            Console.WriteLine("\nEmployee added successfully.");
            Console.ReadKey();
        }
        //  Add department
        public static void AddDepartment(CompanyDbContext ctx)
        {
            ctx.Database.EnsureCreated();
            Console.Clear();
            var department = new Department();
            Console.WriteLine("Department data:");
            Console.WriteLine("######################");
            Console.Write("Enter name:");
            department.Name = Console.ReadLine();
            ctx.Departments.Add(department);
            ctx.SaveChanges();
            Console.WriteLine("\nDepartment added successfully.");
            Console.ReadKey();
        }
        //  Add project
        public static void AddProject(CompanyDbContext ctx)
        {
            ctx.Database.EnsureCreated();
            Console.Clear();
            var project = new Project();
            Console.WriteLine("Project data:");
            Console.WriteLine("######################");
            Console.Write("Enter Project name:");
            project.Name = Console.ReadLine();
            Console.WriteLine("*****************");
            try
            {
                Console.Write("Enter Project startDate:");
                project.StartDate = DateOnly.Parse(Console.ReadLine());
                Console.WriteLine("*****************");
                Console.Write("Enter Project endDate:");
                project.EndDate = DateOnly.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.");
                return;
            }
            ctx.Projects.Add(project);
            ctx.SaveChanges();
            Console.WriteLine("\nProject added successfully.");
            Console.ReadKey();
        }

        //  Display 
        public static void ShowDisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("Select Displaying for:");
            Console.WriteLine("------------******------------");
            Console.WriteLine("1-Employee\n\n2-Department\n\n3-project");
            Console.Write("\nSelect input:");
            int choice = int.Parse(Console.ReadLine());

            using var ctx = new CompanyDbContext();
            switch (choice)
            {
                case 1: DisplayEmployees(ctx); break;
                case 2: DisplayDepartments(ctx); break;
                case 3: DisplayProjects(ctx); break;
            }
        }
        //  Display Employees
        public static void DisplayEmployees(CompanyDbContext ctx)
        {
            Console.Clear();
            if (ctx.Employees.Count() == 0)
            {
                Console.WriteLine("No Employees to display.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Data of Employees");
            Console.WriteLine("-------******--------");
            var employees = ctx.Employees.Include(d => d.Department).Include(ep => ep.EmployeeProjects).ThenInclude(p => p.Project).ToList();

            foreach (var emp in employees)
            {

                Console.WriteLine($"ID:{emp.EmployeeId} ,name:{emp.FirstName} {emp.LastName}");
                Console.WriteLine($"DepartmentId:{emp.DepartmentId} ,DepartmentName:{emp.Department?.Name}");
                Console.WriteLine("Wroks on:");
                Console.WriteLine("**********");
                foreach (var ep in emp.EmployeeProjects)
                {

                    Console.WriteLine($"ProjectID:{ep.ProjectId} ,ProjectName:{ep.Project?.Name}");
                }
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("#################################################");
                Console.WriteLine("------------------------------------------------");
            }
            Console.ReadKey();
        }
        //  Display Departments
        public static void DisplayDepartments(CompanyDbContext ctx)
        {
            Console.Clear();
            if (ctx.Departments.Count() == 0)
            {
                Console.WriteLine("No Departments to display.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Data of Departments");
            Console.WriteLine("-------******--------");
            var deparments = ctx.Departments.Include(e => e.Employees).ToList();

            foreach (var dep in deparments)
            {
                Console.WriteLine($"ID:{dep.DepartmentId} ,name:{dep.Name}");
                Console.WriteLine("has employees:");
                Console.WriteLine("**********");
                foreach (var em in dep.Employees)
                {
                    Console.WriteLine($"ID:{em.EmployeeId} ,name:{em.FirstName} {em.LastName}");
                }
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("#################################################");
                Console.WriteLine("------------------------------------------------");
            }
            Console.ReadKey();
        }
        //  Display Projects
        public static void DisplayProjects(CompanyDbContext ctx)
        {
            Console.Clear();
            if (ctx.Projects.Count() == 0)
            {
                Console.WriteLine("No Projects to display.");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Data of Projects");
            Console.WriteLine("-------******--------");
            var projects = ctx.Projects.Include(e => e.EmployeeProjects).ThenInclude(em => em.Employee).ToList();
            foreach (var proj in projects)
            {
                Console.WriteLine($"Name:{proj.Name} ,Date:({proj.StartDate} - {proj.EndDate})");
                Console.WriteLine("worked on by:");
                Console.WriteLine("**********");
                foreach (var em in proj.EmployeeProjects)
                {
                    Console.WriteLine($"ID:{em.EmployeeId} ,name:{em.Employee?.FirstName} {em.Employee?.LastName}");
                }
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("#################################################");
                Console.WriteLine("------------------------------------------------");
            }
            Console.ReadKey();

        }

        //  Edit 
        public static void ShowEditMenu()
        {
            Console.Clear();
            Console.WriteLine("Select Editting for:");
            Console.WriteLine("------------******------------");
            Console.WriteLine("1-Employee\n\n2-Department\n\n3-project");
            Console.Write("\nSelect input:");
            int choice = int.Parse(Console.ReadLine());

            using var ctx = new CompanyDbContext();
            switch (choice)
            {
                case 1: EditEmployee(ctx); break;
                case 2: EditDepartment(ctx); break;
                case 3: EditProject(ctx); break;
            }
        }
        //  Edit Employee
        public static void EditEmployee(CompanyDbContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Employee Edit:");
            Console.WriteLine("1- Edit Employee Data");
            Console.WriteLine("2- Assign Employee to Department");
            Console.WriteLine("3- Assign/Remove Employee To/From Project");
            Console.Write("\nSelect input: ");
            int empEdit = int.Parse(Console.ReadLine());
            switch (empEdit)
            {
                case 1:  //Edit Employee Data
                    Console.Write("Enter Employee ID: ");
                    int empId = int.Parse(Console.ReadLine());
                    var emp = ctx.Employees.Find(empId);

                    if (emp != null)
                    {
                        Console.Write("Enter New First Name: ");
                        emp.FirstName = Console.ReadLine();
                        Console.Write("Enter New Last Name: ");
                        emp.LastName = Console.ReadLine();

                        ctx.SaveChanges();
                        Console.WriteLine("Employee updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Employee not found.");
                    }
                    break;

                case 2:   //Assign Employee to Department
                    Console.Write("Enter Employee ID: ");
                    int empId2 = int.Parse(Console.ReadLine());
                    var emp2 = ctx.Employees.Find(empId2);

                    Console.Write("Enter Department ID: ");
                    int depId = int.Parse(Console.ReadLine());
                    var dep = ctx.Departments.Find(depId);

                    if (emp2 != null && dep != null)
                    {
                        emp2.DepartmentId = dep.DepartmentId;
                        ctx.SaveChanges();
                        Console.WriteLine("Employee assigned to department successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Employee or Department not found.");
                    }
                    break;

                case 3:    //Assign/Remove Employee To/From Project
                    Console.Write("Enter Employee ID: ");
                    int empId3 = int.Parse(Console.ReadLine());
                    var emp3 = ctx.Employees
                        .Include(e => e.EmployeeProjects)
                        .FirstOrDefault(e => e.EmployeeId == empId3);

                    if (emp3 == null)
                    {
                        Console.WriteLine("Employee not found.");
                        break;
                    }

                    Console.WriteLine("1- Assign to Project");
                    Console.WriteLine("2- Remove from Project");
                    Console.Write("\nSelect input: ");
                    int projChoice = int.Parse(Console.ReadLine());

                    Console.Write("Enter Project ID: ");
                    int projId = int.Parse(Console.ReadLine());
                    var proj = ctx.Projects.Find(projId);

                    if (proj == null)
                    {
                        Console.WriteLine("Project not found.");
                        break;
                    }

                    if (projChoice == 1)
                    {
                        ctx.EmployeeProjects.Add(new EmployeeProject
                        {
                            EmployeeId = emp3.EmployeeId,
                            ProjectId = proj.ProjectId
                        });
                        ctx.SaveChanges();
                        Console.WriteLine("Employee assigned to project successfully.");
                    }
                    else if (projChoice == 2)
                    {
                        var relation = ctx.EmployeeProjects
                            .FirstOrDefault(ep => ep.EmployeeId == emp3.EmployeeId && ep.ProjectId == proj.ProjectId);
                        if (relation != null)
                        {
                            ctx.EmployeeProjects.Remove(relation);
                            ctx.SaveChanges();
                            Console.WriteLine("Employee removed from project.");
                        }
                        else
                        {
                            Console.WriteLine("Relation not found.");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.ReadKey();
        }
        public static void EditDepartment(CompanyDbContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Department Edit:");
            Console.WriteLine("1- Edit Department Data");
            Console.WriteLine("2- Assign Employee to Department");
            Console.Write("\nSelect input: ");
            int depEdit = int.Parse(Console.ReadLine());

            switch (depEdit)
            {
                case 1:
                    Console.Write("Enter Department ID: ");
                    int depId = int.Parse(Console.ReadLine());
                    var dep = ctx.Departments.Find(depId);

                    if (dep != null)
                    {
                        Console.Write("Enter New Department Name: ");
                        dep.Name = Console.ReadLine();
                        ctx.SaveChanges();
                        Console.WriteLine("Department updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Department not found.");
                    }
                    break;

                case 2:
                    Console.Write("Enter Department ID: ");
                    int depId2 = int.Parse(Console.ReadLine());
                    var dep2 = ctx.Departments.Find(depId2);
                    if (dep2 == null)
                    {
                        Console.WriteLine("Department not found.");
                        break;
                    }

                    Console.Write("Enter Employee ID: ");
                    int empId = int.Parse(Console.ReadLine());
                    var emp = ctx.Employees.Find(empId);
                    if (emp == null)
                    {
                        Console.WriteLine("Employee not found.");
                        break;
                    }

                    emp.DepartmentId = dep2.DepartmentId;
                    ctx.SaveChanges();
                    Console.WriteLine("Employee assigned to department successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.ReadKey();
        }
        //  Edit Project
        public static void EditProject(CompanyDbContext ctx)
        {
            Console.Clear();
            Console.WriteLine("Project Edit:");
            Console.WriteLine("1- Edit Project Data");
            Console.WriteLine("2- Assign Employee to Project");
            Console.Write("\nSelect input: ");
            int projEdit = int.Parse(Console.ReadLine());

            switch (projEdit)
            {
                case 1:
                    Console.Write("Enter Project ID: ");
                    int projId = int.Parse(Console.ReadLine());
                    var proj = ctx.Projects.Find(projId);

                    if (proj != null)
                    {
                        Console.Write("Enter New Project Name: ");
                        proj.Name = Console.ReadLine();
                        Console.Write("Enter New Start Date: ");
                        proj.StartDate = DateOnly.Parse(Console.ReadLine());
                        Console.Write("Enter New End Date: ");
                        proj.EndDate = DateOnly.Parse(Console.ReadLine());

                        ctx.SaveChanges();
                        Console.WriteLine("Project updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Project not found.");
                    }
                    break;

                case 2:
                    Console.Write("Enter Employee ID: ");
                    int empId = int.Parse(Console.ReadLine());
                    var emp = ctx.Employees.Find(empId);
                    if (emp == null)
                    {
                        Console.WriteLine("Employee not found.");
                        return;
                    }

                    Console.Write("Enter Project ID: ");
                    int projId2 = int.Parse(Console.ReadLine());
                    var proj2 = ctx.Projects.Find(projId2);
                    if (proj2 == null)
                    {
                        Console.WriteLine("Project not found.");
                        return;
                    }

                    ctx.EmployeeProjects.Add(new EmployeeProject
                    {
                        EmployeeId = emp.EmployeeId,
                        ProjectId = proj2.ProjectId
                    });

                    ctx.SaveChanges();
                    Console.WriteLine("Employee assigned to project successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.ReadKey();
        }
        // Delete               
        public static void ShowDeleteMenu()
        {
            Console.Clear();
            Console.WriteLine("Slect Deletting for:");
            Console.WriteLine("------------******------------");
            Console.WriteLine("1-Employee\n\n2-Department\n\n3-project");
            Console.Write("\nSelect input:");
            int choice = int.Parse(Console.ReadLine());

            using var ctx = new CompanyDbContext();
            switch (choice)
            {
                //  Delete Employee
                case 1:
                    Console.Write("Enter Employee ID: ");
                    int empId = int.Parse(Console.ReadLine());
                    var emp = ctx.Employees.Find(empId);
                    if (emp != null) { ctx.Employees.Remove(emp); ctx.SaveChanges(); Console.WriteLine("Deleted."); }
                    break;
                //  Delete Department
                case 2:
                    Console.Write("Enter Department ID: ");
                    int depId = int.Parse(Console.ReadLine());
                    var dep = ctx.Departments.Find(depId);
                    if (dep != null) { ctx.Departments.Remove(dep); ctx.SaveChanges(); Console.WriteLine("Deleted."); }
                    break;
                //  Delete Project
                case 3:
                    Console.Write("Enter Project ID: ");
                    int projId = int.Parse(Console.ReadLine());
                    var proj = ctx.Projects.Find(projId);
                    if (proj != null) { ctx.Projects.Remove(proj); ctx.SaveChanges(); Console.WriteLine("Deleted."); }
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.ReadKey();
        }
    }
}
