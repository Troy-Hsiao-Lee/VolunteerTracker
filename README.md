# Volunteer Service Tracking App

A C# WinForms application for tracking volunteer service hours with secure authentication and image signature capture. Built with Supabase for backend services including authentication, database, and file storage.

## Features

- **Secure Authentication**: User sign up/login using Supabase Auth
- **Service Entry Management**: Add, view, and track volunteer service entries
- **Image Signature Capture**: Built-in signature pad for supervisor signatures
- **Total Hours Calculation**: Automatic calculation of total volunteer hours
- **User Data Isolation**: Each user can only access their own data (enforced by Row Level Security)
- **Modern UI**: Clean, responsive Windows Forms interface

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or Visual Studio Code
- Supabase account and project

## Setup Instructions

### 1. Supabase Project Setup

1. **Create a Supabase Project**:
   - Go to [supabase.com](https://supabase.com) and create a new project
   - Note down your project URL and anon key

2. **Enable Authentication**:
   - In your Supabase dashboard, go to Authentication > Settings
   - Enable Email authentication
   - Configure email templates if needed

3. **Create Database Tables**:

   Run the following SQL in your Supabase SQL Editor:

   ```sql
   -- Create volunteer service entries table
   CREATE TABLE volunteer_service_entries (
       id BIGSERIAL PRIMARY KEY,
       user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
       date_of_service DATE NOT NULL,
       service_type VARCHAR(100) NOT NULL,
       description TEXT NOT NULL,
       hours DECIMAL(4,1) NOT NULL CHECK (hours >= 0.1 AND hours <= 24.0),
       supervisor_name VARCHAR(100) NOT NULL,
       supervisor_signature_image_url TEXT NOT NULL,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   -- Create user preferences table
   CREATE TABLE user_preferences (
       id BIGSERIAL PRIMARY KEY,
       user_id UUID REFERENCES auth.users(id) ON DELETE CASCADE,
       display_name VARCHAR(100),
       theme VARCHAR(20) DEFAULT 'Light',
       email_notifications BOOLEAN DEFAULT true,
       created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
       updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
   );

   -- Enable Row Level Security
   ALTER TABLE volunteer_service_entries ENABLE ROW LEVEL SECURITY;
   ALTER TABLE user_preferences ENABLE ROW LEVEL SECURITY;

   -- Create RLS policies for volunteer_service_entries
   CREATE POLICY "Users can view their own entries" ON volunteer_service_entries
       FOR SELECT USING (auth.uid() = user_id);

   CREATE POLICY "Users can insert their own entries" ON volunteer_service_entries
       FOR INSERT WITH CHECK (auth.uid() = user_id);

   CREATE POLICY "Users can update their own entries" ON volunteer_service_entries
       FOR UPDATE USING (auth.uid() = user_id);

   CREATE POLICY "Users can delete their own entries" ON volunteer_service_entries
       FOR DELETE USING (auth.uid() = user_id);

   -- Create RLS policies for user_preferences
   CREATE POLICY "Users can view their own preferences" ON user_preferences
       FOR SELECT USING (auth.uid() = user_id);

   CREATE POLICY "Users can insert their own preferences" ON user_preferences
       FOR INSERT WITH CHECK (auth.uid() = user_id);

   CREATE POLICY "Users can update their own preferences" ON user_preferences
       FOR UPDATE USING (auth.uid() = user_id);

   CREATE POLICY "Users can delete their own preferences" ON user_preferences
       FOR DELETE USING (auth.uid() = user_id);
   ```

4. **Create Storage Bucket**:
   - Go to Storage in your Supabase dashboard
   - Create a new bucket named `volunteer-signatures`
   - Set the bucket to public (for signature image access)
   - Create a storage policy:

   ```sql
   CREATE POLICY "Users can upload their own signature images" ON storage.objects
       FOR INSERT WITH CHECK (
           bucket_id = 'volunteer-signatures' AND 
           auth.uid()::text = (storage.foldername(name))[1]
       );

   CREATE POLICY "Users can view signature images" ON storage.objects
       FOR SELECT USING (bucket_id = 'volunteer-signatures');
   ```

### 2. Application Configuration

1. **Clone or download the project**

2. **Update Configuration**:
   - Open `VolunteerTracker/appsettings.json`
   - Replace `YOUR_SUPABASE_URL` with your actual Supabase URL
   - Replace `YOUR_SUPABASE_ANON_KEY` with your actual Supabase anon key

   ```json
   {
     "Supabase": {
       "Url": "https://your-project-id.supabase.co",
       "AnonKey": "your-anon-key-here"
     },
     "Storage": {
       "BucketName": "volunteer-signatures"
     }
   }
   ```

### 3. Build and Run

1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the Project**:
   ```bash
   dotnet build
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

## Usage

### First Time Setup

1. **Launch the application**
2. **Create an account**:
   - Click "Need an account? Sign Up"
   - Enter your email and password
   - Click "Sign Up"
   - Check your email for verification (if required by your Supabase settings)

3. **Sign in** with your credentials

### Adding Service Entries

1. **Fill out the service entry form**:
   - Select the date of service
   - Choose service type from dropdown
   - Enter description of work performed
   - Specify hours worked (0.1 to 24.0)
   - Enter supervisor name
   - Draw supervisor signature using the signature pad

2. **Click "Add Entry"** to save the entry

### Viewing Entries

- All your service entries are displayed in the right panel
- Total hours are automatically calculated and displayed
- Entries are sorted by date (most recent first)

### Signing Out

- Click the "Sign Out" button in the top-right corner
- This will securely clear your session and return to the login screen

## Security Features

- **Row Level Security (RLS)**: Database-level security ensuring users can only access their own data
- **Secure Authentication**: Supabase Auth handles all authentication securely
- **Image Storage**: Signature images are stored securely in Supabase Storage with user-specific folders
- **Input Validation**: Client-side and server-side validation for all inputs
- **Session Management**: Secure session handling with automatic token refresh

## Project Structure

```
VolunteerTracker/
├── Models/
│   ├── VolunteerServiceEntry.cs      # Data model for service entries
│   └── UserPreferences.cs            # Data model for user preferences
├── Services/
│   └── SupabaseService.cs            # Main service for Supabase operations
├── Controls/
│   └── SignaturePad.cs               # Custom signature drawing control
├── Forms/
│   ├── LoginForm.cs                  # Authentication form
│   └── MainForm.cs                   # Main application form
├── Program.cs                        # Application entry point
├── appsettings.json                  # Configuration file
└── VolunteerTracker.csproj           # Project file
```

## Dependencies

- **supabase-csharp**: Official Supabase C# client
- **Newtonsoft.Json**: JSON serialization
- **System.Drawing.Common**: Graphics and image handling
- **Microsoft.Extensions.Configuration**: Configuration management

## Troubleshooting

### Common Issues

1. **Configuration Error**:
   - Ensure your Supabase URL and anon key are correctly set in `appsettings.json`
   - Verify your Supabase project is active

2. **Authentication Issues**:
   - Check that email authentication is enabled in Supabase
   - Verify email verification settings if required

3. **Database Errors**:
   - Ensure all SQL scripts have been executed in Supabase
   - Check that RLS policies are properly configured

4. **Image Upload Failures**:
   - Verify the storage bucket exists and is public
   - Check storage policies are correctly set

### Getting Help

- Check the Supabase documentation: [supabase.com/docs](https://supabase.com/docs)
- Review the C# SDK documentation: [github.com/supabase-community/supabase-csharp](https://github.com/supabase-community/supabase-csharp)

## Future Enhancements

- Multi-factor authentication
- Social login providers (Google, GitHub, etc.)
- Data export functionality (CSV, PDF)
- Admin dashboard for organizations
- Mobile app version
- Offline capability
- Advanced reporting and analytics

## License

This project is provided as-is for educational and demonstration purposes. 