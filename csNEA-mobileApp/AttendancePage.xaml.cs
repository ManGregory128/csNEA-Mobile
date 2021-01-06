﻿using System;
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

        public AttendancePage()
        {
            Students = new ObservableCollection<Student>();
            InitializeComponent();
            SetDBinfo();
            int day = GetToday();
            string group = GetTeachingGroup(PeriodPicked, day);

            if (group != "")
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
            else
            {
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
            //send to Attendance Table
        }
        public static void SetDBinfo()
        {
            builder = new SqlConnectionStringBuilder
            {
                DataSource = Settings.CurrentDatabase,
                UserID = "SA",
                Password = "]JKfpLZSp=8Qd*NM",
                InitialCatalog = "attendanceDB"
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
    }
}