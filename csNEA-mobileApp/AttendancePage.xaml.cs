using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace csNEA_mobileApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttendancePage : ContentPage
    {
        public static int PeriodPicked { get; set; }
        public ObservableCollection<Student> Students { get; set; }
        public static SqlConnectionStringBuilder builder { get; set; }
        public static bool updateRegister { get; set; }
        public string group { get; set; }
        public AttendancePage()
        {
            Students = new ObservableCollection<Student>();
            InitializeComponent();
            SetDBinfo();
            int day = GetToday();
            group = GetTeachingGroup(PeriodPicked, day);
            Students.Clear();
            if (group != "")
            {
                updateRegister = CheckInAbsences(group);
                DateTime today = DateTime.Now;
                if (updateRegister)
                {
                    Students.Clear();
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        String sql = "SELECT Attendances.StudentID, Attendances.IsPresent, Students.FirstName, Students.LastName " +
                            "FROM Attendances INNER JOIN Students ON Attendances.StudentID = Students.StudentID " +
                            "WHERE StudentGroup='" + group + "' AND Date = '" + today.ToString("yyyy-MM-dd") + "' AND Period = " + PeriodPicked + ";";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Student tempStudent = new Student(reader.GetInt32(0), reader.GetString(2), reader.GetString(3), reader.GetBoolean(1));
                                    Students.Add(tempStudent);
                                }
                                reader.Close();
                            }
                            connection.Close();
                        }
                    }
                }
                else
                {
                    Students.Clear();
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        String sql = "SELECT StudentID, FirstName, LastName FROM dbo.Students WHERE StudentGroup='" + group + "';";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Student tempStudent = new Student(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), false);
                                    Students.Add(tempStudent);
                                }
                                reader.Close();
                            }
                            connection.Close();
                        }
                    }
                }                
            }
            else
            {
                Students.Clear();
                DisplayAlert("Alert", "You do not have a lesson with a group right now.", "OK");                
            }
            this.BindingContext = this;
            lstStudents.ItemsSource = Students;
            //if period correct, check teachings then get students
        }
        private void BtnBack_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        private void btnSubmit_Clicked(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            String sql;
            if (updateRegister)
            {
                for (int i = 0; i < Students.Count; i++)
                {
                    int temp;
                    if (Students[i].IsPresent)
                        temp = 1;
                    else
                        temp = 0;
                    sql = "UPDATE dbo.Attendances SET IsPresent = " + temp + " WHERE Date = '" + today.ToString("yyyy-MM-dd") + "' AND Period = " + PeriodPicked + " AND StudentID = " + Students[i].StudentID + " AND MemberOf = '" + group + "';";
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Students.Count; i++)
                {
                    int temp;
                    if (Students[i].IsPresent)
                        temp = 1;
                    else
                        temp = 0;
                    sql = "INSERT INTO dbo.Attendances (Date, Period, StudentID, MemberOf, IsPresent) " +
                        "Values ('" + today.ToString("yyyy-MM-dd") + "', " + PeriodPicked + ", " + Students[i].StudentID + ", '"+ group +"', " + temp +");";
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
            }
            this.Navigation.PopModalAsync();
        }
        public static void SetDBinfo()
        {
            builder = new SqlConnectionStringBuilder
            {
                DataSource = Settings.CurrentDatabase,
                UserID = "adminDB",
                Password = Settings.CurrentDBPassword,
                InitialCatalog = "aradippou5"
            };
        }
        private int GetToday()
        {
            DateTime today = DateTime.Now;
            string day = today.DayOfWeek.ToString();

            switch (day)
            {
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                default:
                    return 7;
            }
        }
        private string GetTeachingGroup(int period, int day)
        {
            string output = "";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {

                String sql = "SELECT [Group] FROM dbo.Teachings WHERE PeriodID = " + period + " AND Day = " + day + " AND TeacherUsername = '" + Settings.CurrentUsername + "';";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(0))
                                output = "";
                            else
                                output = reader.GetString(0);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return output;
        }
        private bool CheckInAbsences(string group)
        {
            DateTime today = DateTime.Now;
            int records;
            String sql = "SELECT COUNT(1) FROM dbo.Attendances WHERE MemberOf = '" + group + "' AND Date = '" + today.ToString("yyyy-MM-dd") + "' AND Period = " + PeriodPicked + ";";
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    records = (int)command.ExecuteScalar();
                    connection.Close();
                }
            }
            if (records > 0)
            {
                return true;
            }
            else
                return false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}