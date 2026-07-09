-- migration_store_settings.sql
ALTER TABLE CompanySettings 
ADD COLUMN IF NOT EXISTS StoreTheme VARCHAR(50) DEFAULT 'minimalist',
ADD COLUMN IF NOT EXISTS ShowStoreSlider BOOLEAN DEFAULT true,
ADD COLUMN IF NOT EXISTS StoreProductsPerPage INT DEFAULT 12,
ADD COLUMN IF NOT EXISTS SliderImage1 VARCHAR(500) DEFAULT 'https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?q=80&w=2070&auto=format&fit=crop',
ADD COLUMN IF NOT EXISTS SliderImage2 VARCHAR(500) DEFAULT 'https://images.unsplash.com/photo-1607082350899-7e105aa886ae?q=80&w=2070&auto=format&fit=crop',
ADD COLUMN IF NOT EXISTS SliderImage3 VARCHAR(500) DEFAULT 'https://images.unsplash.com/photo-1472851294608-062f824d29cc?q=80&w=2070&auto=format&fit=crop';
