-- migration_theme_engine.sql

DROP TABLE IF EXISTS "ThemeSettings";
DROP TABLE IF EXISTS "Themes";

CREATE TABLE "Themes" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Code" VARCHAR(50) NOT NULL UNIQUE,
    "Description" TEXT,
    "PreviewImage" VARCHAR(500),
    "Version" VARCHAR(20) DEFAULT '1.0.0',
    "IsActive" BOOLEAN DEFAULT false,
    "InstalledDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "ThemeSettings" (
    "Id" SERIAL PRIMARY KEY,
    "ThemeId" INT NOT NULL REFERENCES "Themes"("Id") ON DELETE CASCADE,
    "PrimaryColor" VARCHAR(20) DEFAULT '#000000',
    "SecondaryColor" VARCHAR(20) DEFAULT '#ffffff',
    "FontFamily" VARCHAR(100) DEFAULT 'Inter, sans-serif',
    "BorderRadius" VARCHAR(20) DEFAULT '0px',
    "MainBannerUrl" VARCHAR(500),
    "LogoUrl" VARCHAR(500),
    "ButtonStyle" VARCHAR(20) DEFAULT 'solid',
    "ProductsPerRow" INT DEFAULT 4,
    "ProductCardStyle" VARCHAR(20) DEFAULT 'clean',
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert initial themes
INSERT INTO "Themes" ("Name", "Code", "Description", "PreviewImage") VALUES 
('Modern Store', 'modern', 'Estilo Apple/Allbirds. Minimalista, mucho espacio en blanco, enfocado en el producto.', 'https://placehold.co/600x400?text=Modern+Store'),
('Marketplace Pro', 'marketplace', 'Estilo Amazon. Optimizado para catálogos grandes, filtros avanzados y ofertas.', 'https://placehold.co/600x400?text=Marketplace+Pro'),
('Fashion Premium', 'fashion', 'Estilo Nike/KITH. Orientado a marca, imágenes inmersivas y storytelling.', 'https://placehold.co/600x400?text=Fashion+Premium'),
('Electronics Store', 'electronics', 'Estilo Samsung. Diseño tecnológico y fichas técnicas destacadas.', 'https://placehold.co/600x400?text=Electronics+Store');

-- Default settings for Modern Store
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'modern'), '#1d1d1f', '#f5f5f7', 'Inter, sans-serif', '0px', 3, 'clean');

-- Default settings for Marketplace Pro
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'marketplace'), '#232f3e', '#febd69', 'Arial, sans-serif', '4px', 5, 'bordered');

-- Default settings for Fashion Premium
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'fashion'), '#000000', '#ffffff', 'Helvetica Neue, sans-serif', '0px', 3, 'minimal');

-- Default settings for Electronics Store
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'electronics'), '#0381fe', '#000000', 'Roboto, sans-serif', '8px', 4, 'shadow');

-- Alter CompanySettings to add ActiveThemeId
ALTER TABLE companysettings ADD COLUMN IF NOT EXISTS "ActiveThemeId" INT REFERENCES "Themes"("Id");
UPDATE companysettings SET "ActiveThemeId" = (SELECT "Id" FROM "Themes" WHERE "Code" = 'modern');
