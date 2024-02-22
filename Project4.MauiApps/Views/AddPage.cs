namespace Project4.MauiApps.Views
{
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Maui.Controls;
    using Microsoft.Maui.Graphics.Text;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AddPage : ContentPage
    {
        private Entry firstNameEntry;
        private Entry lastNameEntry;
        private Picker genderPicker;
        private DatePicker dateOfBirthPicker;
        private Entry ageEntry;
        private Entry classEntry;
        private Entry addressEntry;
        private Action updateCallback;
        private Label firstNameErrorLabel;
        private Label lastNameErrorLabel;
        private Label genderNameErrorLabel;
        private Label dateOfBirthErrorLabel;
        private Label ageEntryErrorLabel;
        private bool spaceEntered = false;
        private bool isSubmitting = false;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public AddPage()
        {
            Title = CommonLogic.Student.AddStudentLabel;

            //this.updateCallback = updateCallback;
            
            // Create Entry controls for user input
            var mainLayout = new StackLayout();

            firstNameEntry = new Entry
            {
                Placeholder = CommonLogic.Student.FirstNamePlaceholder,
                MaxLength = 15,
                Margin = new Thickness(0, 0, 0, -10)
            };
            firstNameErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0, -10, 0, 0)
            };
            mainLayout.Children.Add(new Label {
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
                MaxLength = 18,
                Margin = new Thickness(0, -5, 0, -10)
            };
            lastNameErrorLabel = new Label
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
            new Span { Text = CommonLogic.Student.LastNameLabelText },
            new Span { Text = " *", TextColor = Colors.Red }
        }
                }
            });
            mainLayout.Children.Add(lastNameEntry);
            mainLayout.Children.Add(lastNameErrorLabel);

            genderPicker = new Picker
            {
                Title = CommonLogic.Student.GenderPlaceholder,
                Items = { "Male", "Female", "Others" },
                Margin = new Thickness(0, -10, 0, -10)
            };
            genderPicker.SelectedIndexChanged += OnGenderSelectedIndexChanged;
            genderNameErrorLabel = new Label
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
            new Span { Text = CommonLogic.Student.GenderLabelText },
            new Span { Text = " *", TextColor = Colors.Red }
        }
                }
            });
            mainLayout.Children.Add(genderPicker);
            mainLayout.Children.Add(genderNameErrorLabel);

            dateOfBirthPicker = new DatePicker
            {
                Format = "dd-MM-yyyy",
                Margin = new Thickness(0,-10, 0, -10)
            };
            dateOfBirthErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0,-10, 0, 0)
            };
            dateOfBirthPicker.DateSelected += OnDateSelected;
            mainLayout.Children.Add(new Label {FormattedText = new FormattedString { Spans = { new Span { Text = CommonLogic.Student.DateOfBirthLabelText }, new Span { Text = " *", TextColor = Colors.Red } } } });
            mainLayout.Children.Add(dateOfBirthPicker);
            mainLayout.Children.Add(dateOfBirthErrorLabel);

            ageEntry = new Entry
            {
                Placeholder = CommonLogic.Student.AgePlaceholder,
                Keyboard = Keyboard.Numeric,
                MaxLength = 2,
                Margin = new Thickness(0, -10, 0, -10)
                // Set keyboard type to numeric for age
            };
            ageEntryErrorLabel = new Label
            {
                TextColor = Colors.Red,
                Opacity = 0,
                Margin = new Thickness(0,-10, 0, 0)
            };
            ageEntry.TextChanged += OnAgeTextChanged;
            mainLayout.Children.Add(new Label {FormattedText = new FormattedString { Spans = { new Span { Text = CommonLogic.Student.AgeLabelText }, new Span { Text = " *", TextColor = Colors.Red } } } });
            mainLayout.Children.Add(ageEntry);
            mainLayout.Children.Add(ageEntryErrorLabel);

            classEntry = new Entry
            {
                Placeholder = CommonLogic.Student.ClassPlaceholder
            };
            mainLayout.Children.Add(new Label { Text = CommonLogic.Student.ClassLabelText });
            mainLayout.Children.Add(classEntry);

            addressEntry = new Entry
            {
                Placeholder = CommonLogic.Student.AddressPlaceholder
            };
            mainLayout.Children.Add(new Label { Text = CommonLogic.Student.AddressLabelText });
            mainLayout.Children.Add(addressEntry);

            // Create a Button for submission
            var submitButton = new Button
            {
                Text = CommonLogic.Student.SaveButtonText
            };
            submitButton.Clicked += OnSubmitClicked;

            mainLayout.Children.Add(submitButton);
            // Add StackLayout with labels and entries to the main content
            mainLayout.Spacing = 10;

            var cancelButton = new Button
            {
                Text = CommonLogic.Student.CancelButttonText
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

        private void OnBackClicked(object sender, EventArgs e)
        {
            // Navigate back or perform any other action as needed
            //Navigation.PopAsync();
            //updateCallback?.Invoke();

            Navigation.PushAsync(new OurStudents());
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (!await semaphoreSlim.WaitAsync(5))
            {
                // The semaphore is not available (submission is in progress), return early
                return;
            }

            try
            {
                // Validate form before submission
                ValidateForm();

                // Check if there are any validation errors
                if (HasValidationErrors())
                {
                    return;
                }

                // Retrieve values from input fields
                var firstName = firstNameEntry.Text.Trim();
                var lastName = lastNameEntry.Text.Trim();
                var gender = genderPicker.SelectedItem?.ToString();
                var studentClass = classEntry.Text;
                var dateOfBirth = dateOfBirthPicker.Date;
                var age = ageEntry != null && int.TryParse(ageEntry.Text, out var parsedAge) ? parsedAge : 0;
                var address = addressEntry.Text;

                // Create a new Student object
                var newStudent = new CommonLogic.Student
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Gender = gender,
                    DateOfBirth = dateOfBirth,
                    Age = age,
                    Class = studentClass,
                    Address = address
                };

                List<string> errorMessages;
                if (!ValidationHelper.ValidateStudent(newStudent, out errorMessages))
                {
                    // Handle validation errors by displaying error messages
                    foreach (var errorMessage in errorMessages)
                    {
                        DisplayErrorMessage(errorMessage);
                    }

                    return; // Do not proceed with submission if validation fails
                }

                // Add the new student using the StudentService
                CommonLogic.Student.AddStudent(newStudent);

                // Navigate back or perform any other action as needed
                //Navigation.PopAsync();
                //updateCallback?.Invoke();
                Navigation.PushAsync(new OurStudents());
            }
            finally
            {
                // Release the semaphore after the submission is complete
                semaphoreSlim.Release();
            }
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            // Validate date on change
            ValidateDateOfBirth();
            // Calculate age based on selected date and update ageEntry
            ageEntry.Text = CalculateAge(e.NewDate).ToString();
        }

        private void OnAgeTextChanged(object sender, TextChangedEventArgs e)
        {
            // Validate age on change
            ValidateAge();
            // Update dateOfBirthPicker based on entered age
            if (int.TryParse(e.NewTextValue, out int age))
            {
                DateTime currentDate = DateTime.Now;
                DateTime birthDate = currentDate.AddYears(-age);
                dateOfBirthPicker.Date = birthDate;
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

            if (string.IsNullOrWhiteSpace(firstName))
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
            // string result = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z\s.0-9.]+", "");
            //string result = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z\s0-9]+", "");
            //string result = System.Text.RegularExpressions.Regex.Replace(text, @"[^a-zA-Z\s0-9]+|(?<!\s)\.(?!\s)", "");
            
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
    }
}
