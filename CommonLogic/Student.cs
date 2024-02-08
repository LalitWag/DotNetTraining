// Student.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CommonLogic
{
    public class Student
    {
        // Properties for label texts
        public static string FirstNameLabelText { get; set; } = "First Name";
        public static string LastNameLabelText { get; set; } = "Last Name";
        public static string GenderLabelText { get; set; } = "Gender";
        public static string DateOfBirthLabelText { get; set; } = "DateOfBirth";
        public static string AgeLabelText { get; set; } = "Age";
        public static string ClassLabelText { get; set; } = "Class";
        public static string AddressLabelText { get; set; } = "Address";
        public static string YeartextLabel{ get; set; } = " years";
        public static string OurStdudentLabel{ get; set; } = "Our Students";
        public static string AddButtonText { get; set; }= "+ Add";
        public static string SaveButtonText{ get; set; } = "Save";
        public static string DeleteButtonText { get; set; }= "Delete";
        public static string CancelButttonText{ get; set; }= "Cancel";
        public static string AddStudentLabel { get; set; }= "Add Student";

        public static string EditStudentLabel = "Edit Student";

        public static string DeleteComment = "Are you sure you want to delete this student record?";


        public const string SearchPlaceholder = "Search...";
        public const string FirstNamePlaceholder = "Please Enter Your First Name...";
        public const string LastNamePlaceholder = "Please Enter Your Last Name...";
        public const string GenderPlaceholder = "Please Select Gender...";
        public const string AgePlaceholder = "Please Enter Your Age...";
        public const string ClassPlaceholder = "Please Enter Your Class...";
        public const string AddressPlaceholder = "Please Enter Your Address...";

       

        // Define the Student class
        public int studentId { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "First Name should be between 3 and 15 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(18, MinimumLength = 2, ErrorMessage = "Last Name should be between 2 and 18 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(5, 99, ErrorMessage = "Age should be between 5 and 99")]
        public int Age { get; set; }

        public string Class { get; set; }
        public string Address { get; set; }
       

        // Methods for CRUD operations
        public static List<Student> GetAllStudents()
        {
            // Using IStudentService to retrieve students
            var studentService = new StudentService();
            return studentService.GetAllStudents();
        }

        public static void AddStudent(Student student)
        {
            // Using IStudentService to add a new student
            var studentService = new StudentService();
            studentService.AddStudent(student);
        }

        public static void UpdateStudent(Student student)
        {
            // Using IStudentService to update a student
            var studentService = new StudentService();
            studentService.UpdateStudent(student);
        }

        public static void DeleteStudent(int studentId)
        {
            // Using IStudentService to delete a student
            var studentService = new StudentService();
            studentService.DeleteStudent(studentId);
        }

        // Demo data for students
        public static List<Student> DemoStudents { get; set; } = GetDemoStudents();

        private static List<Student> GetDemoStudents()
        {
            return new List<Student>
            {
                new Student { studentId = 1, FirstName = "Rameshwar", LastName = "Mule",Gender = "Male", DateOfBirth = new DateTime(1990, 1, 1), Age = 29, Class = "10th", Address = " Mumbai"},
                new Student { studentId = 2, FirstName = "Deepak", LastName = "Gunjal", Gender = "Male", DateOfBirth = new DateTime(1990, 1, 1), Age = 22, Class = "13th", Address = "Pune"}
                // Add more demo data as needed...
            };
        }
    }

    // Interface for student service
    public interface IStudentService
    {
        List<Student> GetAllStudents();
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(int studentId);
    }

    // Implementation of student service
    public class StudentService : IStudentService
    {
        public List<Student> GetAllStudents()
        {
            // Implement logic to retrieve list of students from data source
            // For demonstration purposes, returning demo data
            return Student.DemoStudents;
        }

        public void AddStudent(Student student)
        {
            // Implement logic to add a new student to the data source
            student.studentId = GetNextId();
            Student.DemoStudents.Add(student);
        }

        public void UpdateStudent(Student student)
        {
            var existingStudent = Student.DemoStudents.FirstOrDefault(s => s.studentId == student.studentId);
            if (existingStudent != null)
            {
                existingStudent.FirstName = student.FirstName;
                existingStudent.LastName = student.LastName;
                existingStudent.Age = student.Age;
                existingStudent.Gender = student.Gender;
                existingStudent.Class = student.Class;
                existingStudent.Address = student.Address;
            }
        }

        public void DeleteStudent(int studentId)
        {
            Student.DemoStudents.RemoveAll(s => s.studentId == studentId);
        }

        private int GetNextId()
        {
            // Find the maximum existing ID and return the next ID
            return Student.DemoStudents.Count > 0 ? Student.DemoStudents.Max(s => s.studentId) + 1 : 1;
        }

    }
}
