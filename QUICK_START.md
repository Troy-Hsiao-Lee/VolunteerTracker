# Quick Start Guide

## Immediate Setup Steps

### 1. Supabase Setup (5 minutes)

1. **Create Supabase Project**:
   - Go to [supabase.com](https://supabase.com)
   - Sign up/login and create a new project
   - Wait for project to be ready

2. **Get Your Credentials**:
   - Go to Settings > API in your Supabase dashboard
   - Copy the "Project URL" and "anon public" key

3. **Run Database Setup**:
   - Go to SQL Editor in your Supabase dashboard
   - Copy and paste the entire contents of `database-setup.sql`
   - Click "Run" to execute the script

4. **Create Storage Bucket**:
   - Go to Storage in your Supabase dashboard
   - Click "Create a new bucket"
   - Name it: `volunteer-signatures`
   - Make it public
   - Click "Create bucket"

### 2. Application Setup (2 minutes)

1. **Update Configuration**:
   - Open `VolunteerTracker/appsettings.json`
   - Replace `YOUR_SUPABASE_URL` with your actual URL
   - Replace `YOUR_SUPABASE_ANON_KEY` with your actual anon key

2. **Run the Application**:
   ```bash
   cd VolunteerTracker
   dotnet run
   ```

### 3. Test the Application

1. **Create Account**:
   - Click "Need an account? Sign Up"
   - Enter email and password
   - Click "Sign Up"

2. **Sign In**:
   - Use your email and password to sign in

3. **Add Service Entry**:
   - Fill out the service entry form
   - Draw a signature in the signature pad
   - Click "Add Entry"

4. **View Entries**:
   - Your entries will appear in the right panel
   - Total hours will be calculated automatically

## Troubleshooting

### Common Issues:

1. **"Configuration Required" Error**:
   - Make sure you've updated `appsettings.json` with your Supabase credentials

2. **Authentication Errors**:
   - Check that email authentication is enabled in Supabase Auth settings
   - Verify your email if verification is required

3. **Database Errors**:
   - Ensure you've run the `database-setup.sql` script completely
   - Check that RLS policies are created

4. **Image Upload Failures**:
   - Verify the `volunteer-signatures` bucket exists and is public
   - Check storage policies in the SQL script

### Getting Help:

- Check the full README.md for detailed instructions
- Review Supabase documentation: [supabase.com/docs](https://supabase.com/docs)
- Check the C# SDK docs: [github.com/supabase-community/supabase-csharp](https://github.com/supabase-community/supabase-csharp)

## Next Steps

Once the basic app is working:

1. **Customize the UI**: Modify colors, fonts, and layout in the Forms
2. **Add Features**: Implement data export, admin dashboard, etc.
3. **Deploy**: Consider packaging as an installer or deploying to cloud
4. **Enhance Security**: Add MFA, audit logging, etc.

## Project Structure

```
VolunteerTracker/
├── Models/           # Data models
├── Services/         # Business logic and Supabase integration
├── Controls/         # Custom UI controls (signature pad)
├── Forms/           # Windows Forms UI
├── Program.cs       # Application entry point
└── appsettings.json # Configuration
```

The application is now ready to use! 🎉 