using Microsoft.Maui.Controls;
using System;
using CommonLogic;
using Project4.MauiApps.Views;
using System.Collections.Generic;
using System.Linq;

namespace Project4.MauiApps.Views
{
    public class OurStudents : ContentPage
    {
        private List<Student> students;
        private List<Student> filteredStudents = new List<Student>();
        private int highlightedRow = 0;
        private string searchText = "";
        private ListView studentListView;
        private DateTime lastTapTime = DateTime.MinValue;


        public OurStudents()
        {///
            Title = "Our Student";
            //BackgroundColor = Colors.Light;
            NavigationPage.SetHasBackButton(this, false);
            InitializeStudents();

            // Creating UI elements
            var searchEntry = new Entry
            {
                Placeholder = Student.SearchPlaceholder
            };
            searchEntry.TextChanged += SearchTextChanged;

            var addButton = new Button
            {
                Text = Student.AddButtonText
            };
            addButton.Clicked += AddStudent;
            var titleLabel = new Label

            {

                Text = "Our Student",

                FontSize = 25,

                TextColor = Colors.White,
                FontAttributes = FontAttributes.Bold,

                FontFamily = null

            };



            var titleView = new StackLayout

            {

                Children = { titleLabel },

                HorizontalOptions = LayoutOptions.CenterAndExpand,

                VerticalOptions = LayoutOptions.CenterAndExpand

            };



            //NavigationPage.SetTitleView(this, titleView);

            studentListView = new ListView
            {
                // Use a custom data template for each item in the ListView
                ItemTemplate = new DataTemplate(() =>
                {
                    // Using a ViewCell with a StackLayout to display FirstName, LastName, and Gender
                    ViewCell viewCell = new ViewCell();

                    Grid grid = new Grid { Padding = new Thickness(10), RowSpacing = 5, HeightRequest = 60 };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    Label firstNameLabel = new Label();
                    firstNameLabel.SetBinding(Label.TextProperty, "FirstName");

                    Label lastNameLabel = new Label();
                    lastNameLabel.SetBinding(Label.TextProperty, "LastName");

                    Label fullNameLabel = new Label { FontAttributes = FontAttributes.Bold };
                    fullNameLabel.SetBinding(Label.TextProperty, "FullName");

                    Label genderLabel = new Label();
                    genderLabel.SetBinding(Label.TextProperty, "Gender");

                    Label ageLabel = new Label();
                    ageLabel.SetBinding(Label.TextProperty, "Age");

                    Label classLabel = new Label();
                    classLabel.SetBinding(Label.TextProperty, "Class");

                    grid.Add(fullNameLabel, 0, 0);
                    grid.Add(genderLabel, 1, 0);
                    grid.Add(ageLabel, 0, 1);
                    grid.Add(classLabel, 1, 1);

                    // Setting the StackLayout as the content of the ViewCell
                    viewCell.View = grid;

                    return viewCell;
                })
            };

            // Assigning the item source (students) to the ListView
            studentListView.ItemsSource = students;

            // Apply row highlight (highlighting the first row by default)
            highlightedRow = 0;
            ApplyRowHighlight();

            // Adding UI elements to the content page
            Content = new VerticalStackLayout
            {
                Children = {
                    new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = Student.OurStdudentLabel },
                    new StackLayout { Children = { searchEntry, addButton } },
                    studentListView
                }
            };

            // Attach event handlers
            studentListView.ItemTapped += OnListViewSingleTapped;
            studentListView.ItemSelected += OnListViewDoubleTapped;



            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.NumberOfTapsRequired = 2;
            tapGestureRecognizer.Tapped += OnListViewDoubleTapped;
            studentListView.GestureRecognizers.Add(tapGestureRecognizer);
            this.Appearing += OurStudents_Appearing;
        }

        private void InitializeStudents()
        {
            students = Student.DemoStudents;
        }

        private void AddStudent(object sender, EventArgs e)
        {
            var newStudent = new Student();
            // Navigate to AddStudent page
            Navigation.PushAsync(new AddPage());

        }
        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            
            
                searchText = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // If the search text is empty, show all students
                filteredStudents = students.ToList();
            }
            else
            {
                filteredStudents = students.Where(student =>
                student.FirstName.ToLower().Contains(searchText.ToLower()) ||
                student.LastName.ToLower().Contains(searchText.ToLower()) ||
                student.FullName.ToLower().Contains(searchText.ToLower()) ||
                // student.Gender.ToLower().Contains(searchText.ToLower()) ||
                student.Age.ToString().Contains(searchText.ToLower())
            //  student.Class.ToLower().Contains(searchText.ToLower())
            ).ToList();
            }
            // Update the ListView with the filtered data
            studentListView.ItemsSource = filteredStudents;

            highlightedRow = 0;
            ApplyRowHighlight();
           

        }

        private void OurStudents_Appearing(object sender, EventArgs e)
        {
            // Apply row highlight (highlighting the first row by default) when the page appears
            highlightedRow = 0;
            ApplyRowHighlight();
        }

        private void OnListViewSingleTapped (object sender, ItemTappedEventArgs e)
        {
            // Get the tapped student
            var tappedStudent = (Student)e.Item;

            // Highlight the tapped row
            highlightedRow = students.IndexOf(tappedStudent);
            ApplyRowHighlight();
        }
        private void OnListViewDoubleTapped(object sender, EventArgs e)
        {
            if (studentListView.SelectedItem == null)
            {
                return; // No item selected.
            }

            var selectedStudent = (Student)studentListView.SelectedItem;

            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastTap = now - lastTapTime;

            if (timeSinceLastTap.TotalMilliseconds < 350)
            {
                // Double-tap within 500 milliseconds, navigate to the EditPage
                Navigation.PushAsync(new EditPage(selectedStudent.studentId));
            }

            lastTapTime = now;

            // Deselect the selected item to avoid highlighting
            studentListView.SelectedItem = null;
        }
        private bool IsSearching => !string.IsNullOrWhiteSpace(searchText);
        private void ApplyRowHighlight()
        {
            List<Student> sourceList = IsSearching ? filteredStudents : students;

            if (highlightedRow >= 0 && highlightedRow < sourceList.Count)
            {
                // Loop through each cell in the ListView and apply highlight color
                for (int i = 0; i < sourceList.Count; i++)
                {
                    var cell = studentListView.TemplatedItems[i] as ViewCell;

                    if (cell != null)
                    {
                        // Set the background color for the selected cell, reset others
                        cell.View.BackgroundColor = (i == highlightedRow) ? Colors.LightBlue : Colors.White;
                    }
                }
            }
        }
    }
} 
