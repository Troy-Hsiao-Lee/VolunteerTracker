# NOTE: The desktop version is outdated (though usable). For the best experience please use https://hourtrackersite.netlify.app for the web version.

(this was also made using Cursor.ai, apologies if that offputs you)

# 🚀 Volunteer Service Tracker

A modern Windows desktop application for tracking volunteer service hours with a beautiful space-themed UI and comprehensive features.

## 📋 Features

- **User Authentication**: Secure sign-up and sign-in with email verification
- **Service Entry Management**: Add, view, edit, and delete volunteer service entries
- **Digital Signatures**: Capture supervisor signatures using a signature pad
- **Service Categories**: Organize entries by service type (Community Service, Charity Work, Environmental, Education, Healthcare, etc.)
- **Hours Tracking**: Track service hours with decimal precision
- **Supervisor Management**: Record supervisor names and signatures for each entry
- **Beautiful UI**: Modern space-themed interface with gradients and animations
- **Data Persistence**: Cloud-based storage using Supabase
- **Export Capabilities**: View and manage all entries in a comprehensive grid

## 🎯 Perfect for Demo

This application is ready to run immediately - no installation or setup required!

## 🚀 Quick Start

### For Your Boss (No Technical Knowledge Required)

1. **Download the repository** (click the green "Code" button and select "Download ZIP")
2. **Extract the ZIP file** to any folder on your computer
3. **Double-click** `VolunteerTracker.exe` to launch the application
4. **Sign up** with your email address to create an account
5. **Start tracking** your volunteer service hours!

### Alternative Launch Method

If the direct .exe doesn't work, double-click `Run Volunteer Tracker.bat` instead.

## 📱 Application Screenshots

### Login Screen
- Modern space-themed design with purple gradients
- Email and password authentication
- Toggle between sign-in and sign-up modes

### Main Application
- **Left Panel**: Add new service entries with all required fields
- **Right Panel**: View all entries in a comprehensive data grid
- **Features**: Digital signature capture, service type selection, hours tracking

### Key Features Demonstrated
- **User Registration**: Email verification workflow
- **Service Entry Form**: Complete with date picker, service type dropdown, description, hours, supervisor name, and signature pad
- **Data Management**: View, edit, and delete entries
- **Total Hours Calculation**: Automatic calculation of total volunteer hours
- **Signature Verification**: Click "View Signature" to see supervisor signatures

## 🛠 Technical Details

- **Platform**: Windows Desktop Application (.NET 7.0)
- **UI Framework**: Windows Forms with custom styling
- **Database**: Supabase (PostgreSQL-based cloud database)
- **Authentication**: Supabase Auth with email verification
- **File Storage**: Supabase Storage for signature images
- **Architecture**: Clean separation of concerns with Services, Models, and Forms

## 📁 Project Structure

```
VolunteerTracker/
├── Forms/
│   ├── LoginForm.cs          # Authentication interface
│   └── MainForm.cs           # Main application interface
├── Services/
│   └── SupabaseService.cs    # Database and authentication logic
├── Models/
│   ├── UserPreferences.cs    # User settings model
│   └── VolunteerServiceEntry.cs # Service entry model
├── Controls/
│   └── SignaturePad.cs       # Custom signature capture control
└── Program.cs                # Application entry point
```

## 🔧 Development Setup

If you want to modify or extend the application:

1. **Prerequisites**: .NET 7.0 SDK
2. **Clone the repository**: `git clone https://github.com/Troy-Hsiao-Lee/VolunteerTracker.git`
3. **Open in Visual Studio**: Open `VolunteerTracker.sln`
4. **Configure Supabase**: Update `appsettings.json` with your Supabase credentials
5. **Build and run**: Press F5 to build and run the application

## 🎨 UI/UX Highlights

- **Space Theme**: Purple gradients and cosmic design elements
- **Responsive Layout**: Adapts to different screen sizes
- **Smooth Animations**: Gradient borders and hover effects
- **Professional Appearance**: Clean, modern interface suitable for professional use
- **Accessibility**: Clear labels, proper contrast, and intuitive navigation

## 📊 Database Schema

The application uses a simple but effective database structure:

- **Users**: Email, password hash, verification status
- **Service Entries**: Date, service type, description, hours, supervisor name, signature URL
- **File Storage**: Signature images stored securely in the cloud

## 🔒 Security Features

- **Email Verification**: Required for account activation
- **Secure Authentication**: Supabase handles password hashing and session management
- **Cloud Storage**: Signatures stored securely in Supabase Storage
- **Input Validation**: Comprehensive validation on all form inputs

## 🎯 Demo Scenarios

Perfect for demonstrating:
1. **User Registration Flow**: Show the complete sign-up and verification process
2. **Service Entry Creation**: Add a new volunteer service entry with all fields
3. **Signature Capture**: Demonstrate the digital signature functionality
4. **Data Management**: Show how to view, edit, and delete entries
5. **Reporting**: Display total hours and entry summaries

## 📞 Support

For questions or issues, please contact the development team or create an issue in the GitHub repository.

---

**Ready for immediate use!** 🚀 
