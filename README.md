# Multimedia Systems Software Project
## Overview
This project is a Windows Forms application developed for the Multimedia Computer Systems course at the University of Niš, Faculty of Electronic Engineering, Department of Computer Science. It features a custom image format supporting downsampling and compression, inspired by bmp and jpg formats. The application is designed to work with 24-bit bitmap images and includes functionality for image manipulation through various filters.

## Features
- **Custom Image Format**: Define and use a minimal yet sufficient custom image format for saving and reconstructing bitmap images.
- **Color Model Conversion**: Convert Bitmap images between RGB and YUV color models, using formulas similar to those used by the jpg format.
- **Downsampling**: Apply downsampling on U and V channels while keeping the Y channel unchanged, reducing memory requirements.
- **Compression**: Compress the processed content with or without compression in two modes - half-byte (16 values) and byte (256 values).
- **Filters**: Support several filters, divided into groups based on student assignment. These include Shannon-Fano, Contrast, Sharpen, Edge Detect, Gamma, Emboss Laplac

![Picture1](https://github.com/brankovicvukasin/Image-Filter/blob/main/pic1.png "pic1")


![Picture2](https://github.com/brankovicvukasin/Image-Filter/blob/main/pic2.png "pic2")


![Picture3](https://github.com/brankovicvukasin/Image-Filter/blob/main/pic3.png "pic3")


## Installation Guide
1. **Prerequisites**: Ensure you have .NET Framework 4.7.2 or higher installed on your Windows machine.
2. **Download**: Clone or download the ZIP file of this repository to your local machine.
3. **Build**: Open the solution file (`MultimediaSystemsSoftware.sln`) in Visual Studio. Build the solution by navigating to `Build > Build Solution`.
4. **Run**: After building, navigate to the `bin/Debug` or `bin/Release` directory inside the project folder. Run `MultimediaSystemsSoftware.exe` to start the application.

## Usage Guide
1. **Opening an Image**: Click on `File > Open` to select and open a 24-bit bitmap image.
2. **Applying Filters**: Navigate to the `Filters` menu to choose and apply different filters to the opened image.
3. **Color Model Conversion**: Use the `Color Model` option in the menu to convert the image between RGB and YUV color models.
4. **Downsampling and Compression**: Access these features under the `Image Processing` menu to apply downsampling and choose the compression mode.
5. **Saving Images**: Save your edited images using `File > Save As` by choosing the custom format or as a standard bitmap image.

For more detailed information on the features and filters, refer to the `Documentation` folder included in the project repository.


