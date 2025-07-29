-- Volunteer Service Tracker Database Setup Script
-- Run this script in your Supabase SQL Editor

-- Create volunteer service entries table
CREATE TABLE IF NOT EXISTS volunteer_service_entries (
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
CREATE TABLE IF NOT EXISTS user_preferences (
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

-- Drop existing policies if they exist (for re-running the script)
DROP POLICY IF EXISTS "Users can view their own entries" ON volunteer_service_entries;
DROP POLICY IF EXISTS "Users can insert their own entries" ON volunteer_service_entries;
DROP POLICY IF EXISTS "Users can update their own entries" ON volunteer_service_entries;
DROP POLICY IF EXISTS "Users can delete their own entries" ON volunteer_service_entries;

DROP POLICY IF EXISTS "Users can view their own preferences" ON user_preferences;
DROP POLICY IF EXISTS "Users can insert their own preferences" ON user_preferences;
DROP POLICY IF EXISTS "Users can update their own preferences" ON user_preferences;
DROP POLICY IF EXISTS "Users can delete their own preferences" ON user_preferences;

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

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_volunteer_service_entries_user_id ON volunteer_service_entries(user_id);
CREATE INDEX IF NOT EXISTS idx_volunteer_service_entries_date ON volunteer_service_entries(date_of_service);
CREATE INDEX IF NOT EXISTS idx_user_preferences_user_id ON user_preferences(user_id);

-- Create function to automatically update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers to automatically update updated_at
DROP TRIGGER IF EXISTS update_volunteer_service_entries_updated_at ON volunteer_service_entries;
CREATE TRIGGER update_volunteer_service_entries_updated_at
    BEFORE UPDATE ON volunteer_service_entries
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_user_preferences_updated_at ON user_preferences;
CREATE TRIGGER update_user_preferences_updated_at
    BEFORE UPDATE ON user_preferences
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Storage policies (run these after creating the storage bucket)
-- Note: You need to create the 'volunteer-signatures' bucket in the Supabase dashboard first

-- Drop existing storage policies if they exist
DROP POLICY IF EXISTS "Users can upload their own signature images" ON storage.objects;
DROP POLICY IF EXISTS "Users can view signature images" ON storage.objects;

-- Create storage policies for signature images
CREATE POLICY "Users can upload their own signature images" ON storage.objects
    FOR INSERT WITH CHECK (
        bucket_id = 'volunteer-signatures' AND 
        auth.uid()::text = (storage.foldername(name))[1]
    );

CREATE POLICY "Users can view signature images" ON storage.objects
    FOR SELECT USING (bucket_id = 'volunteer-signatures');

-- Grant necessary permissions
GRANT USAGE ON SCHEMA public TO anon, authenticated;
GRANT ALL ON ALL TABLES IN SCHEMA public TO anon, authenticated;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO anon, authenticated;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public TO anon, authenticated;

-- Enable realtime for tables (optional - for future real-time features)
ALTER PUBLICATION supabase_realtime ADD TABLE volunteer_service_entries;
ALTER PUBLICATION supabase_realtime ADD TABLE user_preferences; 