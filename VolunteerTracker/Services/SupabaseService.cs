using Supabase;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Supabase.Storage;
using VolunteerTracker.Models;
using Newtonsoft.Json;
using System.Text;

namespace VolunteerTracker.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly string _storageBucketName;
        private User? _currentUser;

        public SupabaseService(string url, string anonKey, string storageBucketName)
        {
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };

            _supabaseClient = new Supabase.Client(url, anonKey, options);
            _storageBucketName = storageBucketName;
        }

        public User? CurrentUser => _currentUser;

        public async Task<bool> SignUpAsync(string email, string password)
        {
            try
            {
                // Note: Email verification redirect URL needs to be configured in Supabase dashboard
                // Go to Authentication > URL Configuration and set Site URL to your verification page
                var session = await _supabaseClient.Auth.SignUp(email, password);
                _currentUser = session.User;
                return _currentUser != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Sign up failed: {ex.Message}");
            }
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                _currentUser = session.User;
                return _currentUser != null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Sign in failed: {ex.Message}");
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
                _currentUser = null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Sign out failed: {ex.Message}");
            }
        }

        public async Task<List<VolunteerServiceEntry>> GetUserEntriesAsync()
        {
            if (_currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            if (string.IsNullOrEmpty(_currentUser.Id))
                throw new UnauthorizedAccessException("User ID is not available");

            try
            {
                var response = await _supabaseClient
                    .From<VolunteerServiceEntry>()
                    .Filter("user_id", Postgrest.Constants.Operator.Equals, _currentUser.Id)
                    .Order("date_of_service", Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models ?? new List<VolunteerServiceEntry>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve entries: {ex.Message}");
            }
        }

        public async Task<VolunteerServiceEntry> CreateEntryAsync(VolunteerServiceEntry entry)
        {
            if (_currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            try
            {
                // Create a new entry object to avoid modifying the original
                var newEntry = new VolunteerServiceEntry
                {
                    UserId = _currentUser.Id,
                    DateOfService = entry.DateOfService,
                    ServiceType = entry.ServiceType,
                    Description = entry.Description,
                    Hours = entry.Hours,
                    SupervisorName = entry.SupervisorName,
                    SupervisorSignatureImageUrl = entry.SupervisorSignatureImageUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var response = await _supabaseClient
                    .From<VolunteerServiceEntry>()
                    .Insert(newEntry);

                return response.Models.First();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create entry: {ex.Message}");
            }
        }

        public async Task<string> UploadSignatureImageAsync(byte[] imageData, string fileName)
        {
            if (_currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            try
            {
                // Create unique filename with user ID to prevent conflicts
                var uniqueFileName = $"{_currentUser.Id}/{DateTime.UtcNow:yyyyMMddHHmmss}_{fileName}";

                var response = await _supabaseClient.Storage
                    .From(_storageBucketName)
                    .Upload(imageData, uniqueFileName);

                // Get public URL
                var publicUrl = _supabaseClient.Storage
                    .From(_storageBucketName)
                    .GetPublicUrl(uniqueFileName);

                return publicUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload signature image: {ex.Message}");
            }
        }

        public async Task<UserPreferences?> GetUserPreferencesAsync()
        {
            if (_currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            try
            {
                var response = await _supabaseClient
                    .From<UserPreferences>()
                    .Filter("user_id", Postgrest.Constants.Operator.Equals, _currentUser.Id)
                    .Get();

                return response.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve user preferences: {ex.Message}");
            }
        }

        public async Task<UserPreferences> CreateOrUpdateUserPreferencesAsync(UserPreferences preferences)
        {
            if (_currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            try
            {
                var existingPreferences = await GetUserPreferencesAsync();
                if (existingPreferences != null)
                {
                    // Update existing preferences
                    var updatedPreferences = new UserPreferences
                    {
                        Id = existingPreferences.Id,
                        UserId = _currentUser.Id,
                        DisplayName = preferences.DisplayName,
                        Theme = preferences.Theme,
                        EmailNotifications = preferences.EmailNotifications,
                        CreatedAt = existingPreferences.CreatedAt,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    var response = await _supabaseClient
                        .From<UserPreferences>()
                        .Update(updatedPreferences);

                    return response.Models.First();
                }
                else
                {
                    // Create new preferences
                    var newPreferences = new UserPreferences
                    {
                        UserId = _currentUser.Id,
                        DisplayName = preferences.DisplayName,
                        Theme = preferences.Theme,
                        EmailNotifications = preferences.EmailNotifications,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    var response = await _supabaseClient
                        .From<UserPreferences>()
                        .Insert(newPreferences);

                    return response.Models.First();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save user preferences: {ex.Message}");
            }
        }

        public double CalculateTotalHours(List<VolunteerServiceEntry> entries)
        {
            return entries.Sum(e => e.Hours);
        }

        public async Task<bool> DeleteEntryAsync(int entryId)
        {
            if (_currentUser == null)
                throw new UnauthorizedAccessException("User not authenticated");

            try
            {
                // First, get the entry to check if it belongs to the current user
                var response = await _supabaseClient
                    .From<VolunteerServiceEntry>()
                    .Filter("id", Postgrest.Constants.Operator.Equals, entryId)
                    .Filter("user_id", Postgrest.Constants.Operator.Equals, _currentUser.Id)
                    .Get();

                var entry = response.Models.FirstOrDefault();
                if (entry == null)
                {
                    throw new Exception("Entry not found or you don't have permission to delete it");
                }

                // Delete the entry
                await _supabaseClient
                    .From<VolunteerServiceEntry>()
                    .Filter("id", Postgrest.Constants.Operator.Equals, entryId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete entry: {ex.Message}");
            }
        }
    }
} 