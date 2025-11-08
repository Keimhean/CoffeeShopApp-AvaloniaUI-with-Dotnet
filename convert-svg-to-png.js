const sharp = require('sharp');
const fs = require('fs');
const path = require('path');

async function convertSvgToPng(svgPath, pngPath, width = 2000) {
    try {
        console.log(`Converting ${svgPath} to ${pngPath}...`);
        
        await sharp(svgPath)
            .resize({ width: width })
            .png()
            .toFile(pngPath);
        
        console.log(`✓ Successfully converted to ${pngPath}`);
        const stats = fs.statSync(pngPath);
        console.log(`  File size: ${(stats.size / 1024).toFixed(2)} KB`);
    } catch (error) {
        console.error(`✗ Error converting ${svgPath}:`, error.message);
    }
}

async function main() {
    const docsDir = path.join(__dirname, 'Docs');
    
    // Convert OOP Principles UML
    await convertSvgToPng(
        path.join(docsDir, 'OOP_Principles_UML.svg'),
        path.join(docsDir, 'OOP_Principles_UML.png'),
        2400 // wider for better quality
    );
    
    // Convert Main Architecture UML
    await convertSvgToPng(
        path.join(docsDir, 'CoffeeShopApp_UML.svg'),
        path.join(docsDir, 'CoffeeShopApp_UML.png'),
        2000
    );
    
    console.log('\n✓ All conversions completed!');
}

main().catch(console.error);
