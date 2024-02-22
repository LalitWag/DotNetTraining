using Microsoft.Maui.Controls;
using System;

namespace Project4.MauiApps.Views
{
    public class EditPage : ContentPage
    {
        private Entry firstNameEntry;
        private Entry lastNameEntry;
        private Picker genderPicker;
        private DatePicker dateOfBirthPicker;
        private Entry ageEntry;
        private Entry classEntry;
        private Entry addressEntry;
        private Button deleteButton;
        private Action updateCallback;
        private CommonLogic.Student existingStudent;
        private int studentId;
        private Label firstNameErrorLabel;
        private Label lastNameErrorLabel;
        private Label genderNameErrorLabel;
        private Label dateOfBirthErrorLabel;
        private Label ageEntryErrorLabel;
        public EditPage(int studentId)
        {
            //this.updateCallback = updateCallback;
            this.studentId = studentId;

            this.existingStudent = GetStudentById(studentId);

            Title = "Edit Student";

            // Create Entry controls for user input
            var mainLayout = new StackLayout();

            firstNameEntry = new Entry
            {
                Placeholder = CommonLogic.Student.FirstNamePlaceholder,
                Text = existingStudent.FirstName,
                MaxLength = 15,
                Margin = new Thickness(0, 0, 0, -10)
            };
            firstNameErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0, -10, 0, 0)
            };
            mainLayout.Children.Add(new Label
            {
                FormattedText = new FormattedString
                {
                    Spans =
        {
            new Span { Text = CommonLogic.Student.FirstNameLabelText },
            new Span { Text = " *", TextColor = Colors.Red }
        }
                }
            });
            mainLayout.Children.Add(firstNameEntry);
            mainLayout.Children.Add(firstNameErrorLabel);

            lastNameEntry = new Entry
            {
                Placeholder = CommonLogic.Student.LastNamePlaceholder,
                Text = existingStudent.LastName,
                MaxLength = 18,
                Margin = new Thickness(0, -5, 0, -10)
            
            };
            lastNameErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0, -10, 0, 0)
            };
            mainLayout.Children.Add(new Label { FormattedText = new FormattedString { Spans = { new Span { Text = CommonLogic.Student.LastNameLabelText }, new Span { Text = " *", TextColor = Colors.Red } } } });
            mainLayout.Children.Add(lastNameEntry);
            mainLayout.Children.Add(lastNameErrorLabel);

            genderPicker = new Picker
            {
                Title = CommonLogic.Student.GenderPlaceholder,
                Items = { "Male", "Female", "Others" },
                SelectedItem = existingStudent.Gender,
                 Margin = new Thickness(0, -10, 0, -10)
            };
            genderNameErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0, -10, 0, 0)
            };
            mainLayout.Children.Add(new Label { FormattedText = new FormattedString { Spans = { new Span { Text = CommonLogic.Student.GenderLabelText }, new Span { Text = " *", TextColor = Colors.Red } } } });
            mainLayout.Children.Add(genderPicker);
            mainLayout.Children.Add(genderNameErrorLabel);

            dateOfBirthPicker = new DatePicker
            {
                Format = "dd-mm-yyyy",
                Date = existingStudent.DateOfBirth,
                 Margin = new Thickness(0, -10, 0, -10)
            };
            dateOfBirthErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0, -10, 0, 0)
            };
            dateOfBirthPicker.DateSelected += OnDateSelected;
            mainLayout.Children.Add(new Label { FormattedText = new FormattedString { Spans = { new Span { Text = CommonLogic.Student.DateOfBirthLabelText }, new Span { Text = " *", TextColor = Colors.Red } } } });
            mainLayout.Children.Add(dateOfBirthPicker);
            mainLayout.Children.Add(dateOfBirthErrorLabel);

            ageEntry = new Entry
            {
                Placeholder = CommonLogic.Student.AgePlaceholder,
                Keyboard = Keyboard.Numeric,
                Text = existingStudent.Age.ToString(),
                MaxLength = 2,
                Margin = new Thickness(0, -10, 0, -10)
            };
            ageEntryErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0, -10, 0, 0)
            };
            ageEntry.TextChanged += OnAgeTextChanged;
            mainLayout.Children.Add(new Label { FormattedText = new FormattedString { Spans = { new Span { Text = CommonLogic.Student.AgeLabelText }, new Span { Text = " *", TextColor = Colors.Red } } } });
            mainLayout.Children.Add(ageEntry);
            mainLayout.Children.Add(ageEntryErrorLabel);

            classEntry = new Entry
            {
                Placeholder = CommonLogic.Student.ClassPlaceholder,
                Text = existingStudent.Class
            };
            mainLayout.Children.Add(new Label { Text = CommonLogic.Student.ClassLabelText });
            mainLayout.Children.Add(classEntry);

            addressEntry = new Entry
            {
                Placeholder = CommonLogic.Student.AddressPlaceholder,
                Text = existingStudent.Address
            };
            mainLayout.Children.Add(new Label { Text = CommonLogic.Student.AddressLabelText });
            mainLayout.Children.Add(addressEntry);

            // Create a Button for submission

            // Create a Button for deletion
            deleteButton = new Button
            {
                Text = "Delete",
                BackgroundColor = Colors.Red,
                TextColor = Colors.White
            };
            deleteButton.Clicked += OnDeleteClicked;
            mainLayout.Children.Add(deleteButton);

            mainLayout.Spacing = 10;

            var submitButton = new Button
            {
                Text = "Submit"
            };
            submitButton.Clicked += OnSubmitClicked;

            mainLayout.Children.Add(submitButton);
            mainLayout.Spacing = 10;


            var cancelButton = new Button
            {
                Text = "Cancel"
            };
            cancelButton.Clicked += OnBackClicked; // Both back arrow and cancel button perform the same action

            mainLayout.Children.Add(cancelButton);

            Content = mainLayout;
            firstNameEntry.Unfocused += OnFirstNameLostFocus;
            lastNameEntry.Unfocused += OnLastNameLostFocus;
            ageEntry.Unfocused += OnAgeLostFocus;
            firstNameEntry.TextChanged += OnFirstNameTextChanged;
            lastNameEntry.TextChanged += OnLastNameTextChanged;

            // Validate form on load
            // ValidateForm();
        }
        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Calculate age based on selected date and update ageEntry
            ageEntry.Text = CalculateAge(e.NewDate).ToString();
        }

        private void OnAgeTextChanged(object sender, TextChangedEventArgs e)
        {
            // Update dateOfBirthPicker based on entered age
            if (int.TryParse(e.NewTextValue, out int age))
            {
                DateTime currentDate = DateTime.Now;
                DateTime birthDate = currentDate.AddYears(-age);
                dateOfBirthPicker.Date = birthDate;
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - birthDate.Year;

            // Adjust age if birthday hasn't occurred yet this year
            if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }
        private void OnBackClicked(object sender, EventArgs e)
        {
            // Navigate back or perform any other action as needed
            //Navigation.PopAsync();
            //updateCallback?.Invoke();
           Navigation.PushAsync(new OurStudents());
        }
        private void OnSubmitClicked(object sender, EventArgs e)
        {
            // Validate form before submission
            ValidateForm();

            // Check if there are any validation errors
            if (HasValidationErrors())
            {
                return;
            }


            // Retrieve values from input fields
            existingStudent.FirstName = firstNameEntry.Text.Trim();
            existingStudent.LastName = lastNameEntry.Text.Trim();
            existingStudent.Gender = genderPicker.SelectedItem?.ToString();
            existingStudent.DateOfBirth = dateOfBirthPicker.Date;
            existingStudent.Age = int.Parse(ageEntry.Text);
            existingStudent.Address = addressEntry.Text;
            existingStudent.Class = classEntry.Text;

            // Update the existing student using the StudentService
            CommonLogic.Student.UpdateStudent(existingStudent);

            // Navigate back or perform any other action as needed
           

            Navigation.PushAsync(new OurStudents());


                  }
        private CommonLogic.Student GetStudentById(int studentId)
        {
            return CommonLogic.Student.DemoStudents.FirstOrDefault(s => s.studentId == studentId);
        }
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Delete", "Are you sure you want to delete this student record?", "Yes", "No");

            if (result)
            {
                // Delete the existing student using the StudentService
                CommonLogic.Student.DeleteStudent(existingStudent.studentId);

                // Navigate back or perform any other action as needed
                //updateCallback?.Invoke();
                //Navigation.PopAsync();
                
                Navigation.PushAsync(new OurStudents());
            }
        }
        private void OnFirstNameLostFocus(object sender, FocusEventArgs e)
        {

            // Validate first name when the user clicks on another control (loses focus)
            ValidateFirstName();

        }

        private void OnLastNameLostFocus(object sender, FocusEventArgs e)
        {
            // Validate last name when the user clicks on another control (loses focus)
            ValidateLastName();

        }

        private void OnAgeLostFocus(object sender, FocusEventArgs e)
        {
            // Validate age when the user clicks on another control (loses focus)
            ValidateAge();
        }
        private void OnGenderSelectedIndexChanged(object sender, EventArgs e)
        {
            // Validate gender when the selected index changes
            ValidateGender();
        }

        private void ValidateForm()
        {
            ValidateFirstName();
            ValidateLastName();
            ValidateGender();
            ValidateDateOfBirth();
            ValidateAge();
        }

        private void ValidateFirstName()
        {
            // Validate first name
            var firstName = firstNameEntry.Text;

            firstName = TrimConsecutiveSpaces(firstName).Trim();

            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 3)
            {
                SetErrorLabel(firstNameErrorLabel, "First Name is required");
            }
            else if (firstName.Length < 3 || firstName.Length > 15)
            {
                SetErrorLabel(firstNameErrorLabel, "First Name should be between 3 and 15 characters");
            }
            else
            {
                SetErrorLabel(firstNameErrorLabel, string.Empty);
            }
        }

        private void ValidateLastName()
        {
            // Validate last name
            var lastName = lastNameEntry.Text;
            if (string.IsNullOrWhiteSpace(lastName))
            {
                SetErrorLabel(lastNameErrorLabel, "Last Name is required");
            }
            else if (lastName.Length < 2 || lastName.Length > 18)
            {
                SetErrorLabel(lastNameErrorLabel, "Last Name should be between 2 and 18 characters");
            }
            else
            {
                SetErrorLabel(lastNameErrorLabel, string.Empty);
            }
        }

        private void ValidateGender()
        {
            // Validate gender
            var gender = genderPicker.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(gender))
            {
                SetErrorLabel(genderNameErrorLabel, "Gender is required");
            }
            else
            {
                SetErrorLabel(genderNameErrorLabel, string.Empty);
            }
        }

        private void ValidateDateOfBirth()
        {
            // Validate date of birth
            if (dateOfBirthPicker.Date == DateTime.MinValue)
            {
                SetErrorLabel(dateOfBirthErrorLabel, "Date of Birth is required");
            }
            else
            {
                SetErrorLabel(dateOfBirthErrorLabel, string.Empty);
            }
        }
        private void OnFirstNameTextChanged(object sender, TextChangedEventArgs e)
        {
            firstNameEntry.Text = TrimConsecutiveSpaces(firstNameEntry.Text);
            ValidateFirstName();

        }

        private void OnLastNameTextChanged(object sender, TextChangedEventArgs e)
        {
            lastNameEntry.Text = TrimConsecutiveSpaces(lastNameEntry.Text);
            ValidateLastName();
        }
        private string TrimConsecutiveSpaces(string text)
        {
            if (text == null)
            {
                // Handle the case where input is null, e.g., return an empty string
                return string.Empty;
            }
            string result = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z\s]+$", "");
            

            return result;
        }
        private void ValidateAge()
        {
            // Validate age
            var ageText = ageEntry.Text;
            if (string.IsNullOrWhiteSpace(ageText))
            {
                SetErrorLabel(ageEntryErrorLabel, "Age is required");
                SetErrorLabel(dateOfBirthErrorLabel, "Date of Birth is required");
            }
            else if (!int.TryParse(ageText, out int age) || age < 5 || age > 99)
            {
                SetErrorLabel(ageEntryErrorLabel, "Age should be between 5 and 99");
                SetErrorLabel(dateOfBirthErrorLabel, "Date of Birth is required");
            }
            else
            {
                SetErrorLabel(ageEntryErrorLabel, string.Empty);
                SetErrorLabel(dateOfBirthErrorLabel, string.Empty);
            }
        }
        private bool HasValidationErrors()
        {
            // Check if any error label is visible
            return firstNameErrorLabel.Opacity == 1
                || lastNameErrorLabel.Opacity == 1
                || genderNameErrorLabel.Opacity == 1
                || dateOfBirthErrorLabel.Opacity == 1
                || ageEntryErrorLabel.Opacity == 1;
        }
        private void SetErrorLabel(Label label, string errorMessage)
        {
            label.Text = errorMessage;
            label.Opacity = string.IsNullOrEmpty(errorMessage) ? 0 : 1;
        }
        private void DisplayErrorMessage(string errorMessage)
        {
            // Reset error labels
            firstNameErrorLabel.Text = string.Empty;
            lastNameErrorLabel.Text = string.Empty;
            genderNameErrorLabel.Text = string.Empty;
            dateOfBirthErrorLabel.Text = string.Empty;
            ageEntryErrorLabel.Text = string.Empty;

            if (errorMessage.Contains("First Name"))
            {
                SetErrorLabel(firstNameErrorLabel, errorMessage);
            }
            else if (errorMessage.Contains("Last Name"))
            {
                SetErrorLabel(lastNameErrorLabel, errorMessage);
            }
            else if (errorMessage.Contains("Gender"))
            {
                SetErrorLabel(genderNameErrorLabel, errorMessage);
            }
            else if (errorMessage.Contains("Age"))
            {
                SetErrorLabel(dateOfBirthErrorLabel, errorMessage);
            }
            else if (errorMessage.Contains("Age"))
            {
                SetErrorLabel(ageEntryErrorLabel, errorMessage);
            }
            else
            {
                // If no specific error, proceed with submission
                return;
            }
        }
    }
}
