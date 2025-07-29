using VolunteerTracker.Services;
using VolunteerTracker.Forms;
using Microsoft.Extensions.Configuration;

namespace VolunteerTracker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // Load configuration
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var supabaseUrl = configuration["Supabase:Url"] ?? "";
                var supabaseAnonKey = configuration["Supabase:AnonKey"] ?? "";
                var storageBucketName = configuration["Storage:BucketName"] ?? "volunteer-signatures";

                if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseAnonKey))
                {
                    MessageBox.Show(
                        "Please configure your Supabase settings in appsettings.json\n\n" +
                        "1. Replace 'YOUR_SUPABASE_URL' with your actual Supabase URL\n" +
                        "2. Replace 'YOUR_SUPABASE_ANON_KEY' with your actual Supabase anon key\n\n" +
                        "You can find these values in your Supabase project dashboard under Settings > API.",
                        "Configuration Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Initialize Supabase service
                var supabaseService = new SupabaseService(supabaseUrl, supabaseAnonKey, storageBucketName);

                // Start with login form
                var loginForm = new LoginForm(supabaseService);
                Application.Run(loginForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Application startup error: {ex.Message}\n\nPlease check your configuration and try again.",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
} 