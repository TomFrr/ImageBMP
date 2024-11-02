using System;
using System.Numerics;
using System.Text;
using Pixel_Code;

namespace Fichier_Bitmap
{
    internal class MyImage
    {
        #region attributs
        private string type;
        private int size;
        private int sizeOffset;
        private int width;
        private int height;
        private int colorEncoding;
        private Pixel[,] encoding;
        #endregion

        #region constructeur
        public MyImage(string myfile)
        {
            byte[] bmp = File.ReadAllBytes(myfile);
            type = Encoding.ASCII.GetString(bmp.Take(2).ToArray());
            size = Convert_Endian_To_Int(bmp.Take(6).Skip(2).ToArray());
            sizeOffset = Convert_Endian_To_Int(bmp.Take(14).Skip(10).ToArray());
            width = Convert_Endian_To_Int(bmp.Take(22).Skip(18).ToArray());
            height = Convert_Endian_To_Int(bmp.Take(26).Skip(22).ToArray());
            colorEncoding = Convert_Endian_To_Int(bmp.Take(32).Skip(28).ToArray());

            int paddingStep = width % 4;
            encoding = new Pixel[height, width];          
            for(int i = 0; i< encoding.GetLength(0); i++)
            {
                for (int j = 0; j < encoding.GetLength(1); j++)
                {
                    int blue = Convert.ToInt32(bmp[54 + (i * encoding.GetLength(1) + j ) * 3 + i * paddingStep]);
                    int green = Convert.ToInt32(bmp[55 + (i * encoding.GetLength(1) + j ) * 3 + i * paddingStep]);
                    int red = Convert.ToInt32(bmp[56 + (i * encoding.GetLength(1) + j ) * 3 + i * paddingStep]);
                    encoding[encoding.GetLength(0)-1-i, j] = new Pixel(red,green,blue); // Start at the bottom left from Image
                }
            }
        }

        public MyImage(string type,int size,int sizeOffset,int width,int height,int colorEncoding)
        {
            this.type = type;
            this.size = size;
            this.sizeOffset = sizeOffset;
            this.width = width;
            this.height = height;
            this.colorEncoding = colorEncoding;
        }
        #endregion

        #region accesseurs
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        #endregion

        #region méthodes

        #region conversions
        public byte[] Convert_Int_To_Endian(int val)
        {
            byte[] tab = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                tab[i] = Convert.ToByte(Math.Floor(val / Math.Pow(256, i)));
                val = Convert.ToInt32(val % Math.Pow(256, i));
            }
            return tab;
        }

        public int Convert_Endian_To_Int(byte[] tab)
        {
            int val = 0;
            if (tab != null && tab.Length > 0)
            {
                for (int i = 0; i < tab.Length; i++)
                    val += Convert.ToInt32(tab[i]) * Convert.ToInt32(Math.Pow(256, i));
            }
            return val;
        }
        #endregion

        public string Ou_Exclusif(string MSB,string LSB,bool Coder)
        {
            bool[] MSBtab = new bool[8];
            bool[] LSBtab = new bool[8];
            bool[] binaryTab = new bool[8];
            string result = "";
            for (int i = 0; i < 8; i++)
            {
                if (MSB.Length + i < 8)     //MSB str to bool[]
                    MSBtab[i] = false;
                else if (MSB[i - (8 - MSB.Length)] == '1')
                    MSBtab[i] = true;
                else
                    MSBtab[i] = false;

                if (LSB.Length + i < 8)     //LSB str to bool[]
                    LSBtab[i] = false;
                else if (LSB[i - (8 - LSB.Length)] == '1')
                    LSBtab[i] = true;
                else
                    LSBtab[i] = false;


            }

            for (int i = 0; i < 8; i++)
            {
                if (!Coder)
                    binaryTab[i] = MSBtab[i] ^ LSBtab[i];
                else if (i<4)
                    binaryTab[i] = MSBtab[i];
                else
                    binaryTab[i] = MSBtab[i] ^ LSBtab[i - 4];


                if (binaryTab[i])
                    result += '1';
                else
                    result += '0';
            }       

            return result;
        }

        public void ToStringHeader()
        {
            Console.WriteLine("\n"+this.type + " " + this.size + " " + this.sizeOffset + " " + this.width + " " + this.height + " " + this.colorEncoding);
            Console.WriteLine(encoding[1, 10].Red + " "+encoding[1,10].Green + " "+encoding[1,10].Blue+ " ");
        }

        public void From_Image_To_File(string myfile)
        {
            // BMP file header (14 bytes)
            byte[] header = new byte[14];
            byte[] infoheader = new byte[40];
            header[0] = 0x42; // 'B'
            header[1] = 0x4D; // 'M'

            int paddingBytes = (width * 3) % 4;
            int resolution = encoding.GetLength(0) * encoding.GetLength(1);
            int imageSize = resolution * 3; // RGB color encoding
            int fileSize = header.Length + infoheader.Length + imageSize + paddingBytes * encoding.GetLength(0);
            byte[] fileSizeBytes = Convert_Int_To_Endian(fileSize);
            Array.Copy(fileSizeBytes, 0, header, 2, 4);
            header[10] = 0x36; // Offset to pixel data (always 54)

            // BMP file infoheader (40 bytes)
            infoheader[0] = 0x28; // Infoheader size
            Array.Copy(Convert_Int_To_Endian(width), 0, infoheader, 4, 4);
            Array.Copy(Convert_Int_To_Endian(height), 0, infoheader, 8, 4);

            infoheader[12] = 0x01; // Number of color planes (always 1)
            infoheader[14] = (byte)colorEncoding; // Number of bits per pixel
            
            byte[] imageSizeBytes = Convert_Int_To_Endian(imageSize);
            Array.Copy(imageSizeBytes, 0, infoheader, 20, 4);

            byte[] resolutionBytes = Convert_Int_To_Endian(resolution);
            Array.Copy(resolutionBytes, 0, infoheader, 24, 4);
            Array.Copy(resolutionBytes, 0, infoheader, 28, 4);

            using (FileStream fs = new FileStream(myfile, FileMode.Create))     //writing the BMP file
            {
                fs.Write(header, 0, header.Length);
                fs.Write(infoheader, 0, infoheader.Length);
                for (int i = height-1; i>=0; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        byte[] pixel = new byte[3];
                        pixel[0] = Convert.ToByte(encoding[i, j].Blue);
                        pixel[1] = Convert.ToByte(encoding[i, j].Green);
                        pixel[2] = Convert.ToByte(encoding[i, j].Red);
                        fs.Write(pixel, 0, 3);
                    }
                    if (paddingBytes > 0)
                    {
                        byte[] padding = new byte[4 - paddingBytes];
                        fs.Write(padding, 0, padding.Length);
                    }
                }
            }

        }

        public void Nuance_De_Gris()
        {           
            for (int i = 0; i < encoding.GetLength(0); i++)
            {
                for (int j = 0; j < encoding.GetLength(1); j++)
                {
                    int color = (encoding[i, j].Red + encoding[i, j].Green + encoding[i, j].Blue) / 3;     //average of RGB color code
                    encoding[i, j].Red = color;
                    encoding[i, j].Green = color;
                    encoding[i, j].Blue = color;
                }
            }
        }

        public void Noir_Et_Blanc()
        {
            for (int i = 0; i < encoding.GetLength(0); i++)
            {
                for (int j = 0; j < encoding.GetLength(1); j++)
                {
                    int valeur;
                    if (((encoding[i, j].Red + encoding[i, j].Green + encoding[i, j].Blue) / 3) > 127.5)
                        valeur = 255;
                    else
                        valeur = 0;
                    encoding[i, j].Red = valeur;
                    encoding[i, j].Green = valeur;
                    encoding[i, j].Blue = valeur;
                }
            }
        }

        public void Miroir()
        {
            for (int i = 0; i < encoding.GetLength(0); i++)
            {
                for (int j = 0; j < encoding.GetLength(1)/2; j++)
                {
                    Pixel a = encoding[i, j];   //temporary memory
                    encoding[i,j]=encoding[i, encoding.GetLength(1) - (j + 1)];
                    encoding[i, encoding.GetLength(1) - (j + 1)] = a;
                }
            }
        }

        public MyImage Rogner(int newWidth, int newHeight)
        {
            int fileSize = newWidth * newHeight + this.sizeOffset;
            MyImage New = new MyImage(this.type,fileSize,this.sizeOffset,newWidth,newHeight,this.colorEncoding);
            
            New.encoding = new Pixel[newHeight,newWidth];
            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                    New.encoding[i,j]= this.encoding[i,j];
            }
            return New;
        }

        public MyImage Agrandissement(double coefficient)
        {
            int newWidth = Convert.ToInt32(this.width * coefficient);
            int newHeight = Convert.ToInt32(this.Height * coefficient);

            int fileSize = newWidth * newHeight + this.sizeOffset;
            MyImage New = new MyImage(this.type, fileSize, this.sizeOffset, newWidth, newHeight, this.colorEncoding);
            New.encoding = new Pixel[newHeight, newWidth];
            
            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                    New.encoding[i, j] = this.encoding[Convert.ToInt32(Math.Floor(i / coefficient)), Convert.ToInt32(Math.Floor(j / coefficient))];
            }
            return New;
        }

        public MyImage Rotation(double angle)
        {
            angle *= (double)Math.PI / 180;     //convert degrees to radians

            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);

            int newWidth = Convert.ToInt32(Math.Abs(this.width * cos) + Math.Abs(this.height * sin));
            int newHeight = Convert.ToInt32(Math.Abs(this.height * cos) + Math.Abs(this.width * sin));
            int fileSize = newWidth * newHeight + this.sizeOffset;
            MyImage New = new MyImage(this.type, fileSize, this.sizeOffset, newWidth, newHeight, this.colorEncoding);
            New.encoding = new Pixel[newHeight, newWidth];

            int centerX = width / 2;
            int centerY = height / 2;

            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    int x = Convert.ToInt32((newWidth / 2 - j) * cos - (i - newHeight / 2) * sin + centerX);
                    int y = Convert.ToInt32((newWidth / 2 - j) * sin + (i - newHeight / 2) * cos + centerY);

                    if ((x >= 0 && x < this.encoding.GetLength(1)) && (y >= 0 && y < this.encoding.GetLength(0)))
                        New.encoding[i, j] = this.encoding[y,x];
                    else
                        New.encoding[i, j] = new Pixel(0, 0, 0);
                }
            }
            return New;
        }

        public MyImage Convolution(float[,] mask)
        {
            int newWidth = this.width - mask.GetLength(1) + 1;
            int newHeight = this.height - mask.GetLength(0) + 1;
            int fileSize = newWidth * newHeight + this.sizeOffset;

            MyImage New = new MyImage(this.type, fileSize, this.sizeOffset, newWidth, newHeight, this.colorEncoding);
            New.encoding = new Pixel[newHeight, newWidth];

            int midHeight = (mask.GetLength(0) - 1) / 2;
            int midWidth = (mask.GetLength(1) - 1) / 2;

            for (int i = midHeight; i < this.height - midHeight; i++)
            {
                for (int j = midWidth; j < this.width - midWidth; j++)
                {
                    float red = 0, green = 0, blue = 0;

                    for (int k = 0; k < mask.GetLength(0); k++)
                    {
                        for (int l = 0; l < mask.GetLength(1); l++)
                        {
                            red += mask[k, l] * this.encoding[i - midHeight + k, j - midWidth + l].Red;
                            green += mask[k, l] * this.encoding[i - midHeight + k, j - midWidth + l].Green;
                            blue += mask[k, l] * this.encoding[i - midHeight + k, j - midWidth + l].Blue;
                        }
                    }
                    red = (red < 0) ? 0 : (red > 255) ? 255 : red;
                    blue = (blue < 0) ? 0 : (blue > 255) ? 255 : blue;
                    green = (green < 0) ? 0 : (green > 255) ? 255 : green;
                    New.encoding[i - midHeight, j - midWidth] = new Pixel(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
                }
            }
            return New;
        }

        #region filtres        
        public MyImage Contour()
        {
            this.Nuance_De_Gris();
            float[,] matrice = new float[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            return this.Convolution(matrice);
        }

        public MyImage Contraste()
        {
            float[,] matrice = new float[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            return this.Convolution(matrice);
        }

        public MyImage Repoussage()
        {
            float[,] matrice = new float[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            return this.Convolution(matrice);
        }

        public MyImage Flou()
        {
            float f = (float)1 / 9;
            float[,] matrice = new float[,] { { f, f, f }, { f, f, f }, { f, f, f } };
            return this.Convolution(matrice);
        }
        #endregion

        public void Fractale(double p, ConsoleKey numero)
        {
            this.encoding = new Pixel[this.height, this.width];
            double xmin = -2, xmax = 2;
            double ymin = -2, ymax = 2;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    double a = xmin + (xmax - xmin) * j / (width - 1);
                    double b = ymin + (ymax - ymin) * i / (height - 1);
                    Complex c = new Complex(a, b);
                    Complex z = new Complex(0,0);
                    int n = 0;

                    while (n < p && Math.Pow(z.Real,2) < 4)
                    {
                        switch(numero)
                        {
                            case ConsoleKey.D1:
                                {
                                    z = Complex.Pow(z, 2) + c;
                                    break;
                                }
                            case ConsoleKey.D2:
                                {
                                    z = Complex.Cos(z / c);
                                    break;
                                }
                            case ConsoleKey.D3:
                                {
                                    z = Complex.Pow(z, 2) + Complex.Sin(Complex.Pow(c, 3));
                                    break;
                                }
                            case ConsoleKey.D4:
                                {
                                    z = Complex.Pow(z, 2) + Complex.Pow(c, 3) + new Complex(-1.401155, 0);
                                    break;
                                }
                        }
                        n++;
                    }

                    int color = Convert.ToInt32(n * 255 / p);
                    this.encoding[i, j] = new Pixel(color, color, color); ;
                }
            }
        }

        #region steganographie
        public MyImage Coder(MyImage Covert)
        {
            MyImage New = new MyImage(Covert.type, Covert.size, Covert.sizeOffset, Covert.Width, Covert.Height, Covert.colorEncoding);
            New.encoding = new Pixel[Covert.Height, Covert.Width];
            string MSB, LSB;
            int remainder;
            int widthCount = width;
            int heightCount = height;

            #region Size info
            for (int i = 0; i < 3; i++)
            {
                New.encoding[0,i] = new Pixel(0, 0, 0);

                remainder = widthCount % 16;
                widthCount = (widthCount - remainder) / 16;
                MSB = Convert.ToString(Covert.encoding[0,i].Red,2);
                LSB = Convert.ToString(remainder,2);
                New.encoding[0, i].Red = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2);

                remainder = widthCount % 16;
                widthCount = (widthCount - remainder) / 16;
                MSB = Convert.ToString(Covert.encoding[0, i].Green, 2);
                LSB = Convert.ToString(remainder, 2);
                New.encoding[0, i].Green = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2);

                remainder = widthCount % 16;
                widthCount = (widthCount - remainder) / 16;
                MSB = Convert.ToString(Covert.encoding[0, i].Blue, 2);
                LSB = Convert.ToString(remainder, 2);
                New.encoding[0, i].Blue = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2);

            }       //Width definition

            for (int i = 0; i < 3; i++)
            {
                New.encoding[0, i + 3] = new Pixel(0, 0, 0);

                remainder = heightCount % 16;
                heightCount = (heightCount - remainder) / 16;
                MSB = Convert.ToString(Covert.encoding[0, i + 3].Red, 2);
                LSB = Convert.ToString(remainder, 2);
                New.encoding[0, i + 3].Red = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2);

                remainder = heightCount % 16;
                heightCount = (heightCount - remainder) / 16;
                MSB = Convert.ToString(Covert.encoding[0, i + 3].Green, 2);
                LSB = Convert.ToString(remainder, 2);
                New.encoding[0, i + 3].Green = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2);

                remainder = heightCount % 16;
                heightCount = (heightCount - remainder) / 16;
                MSB = Convert.ToString(Covert.encoding[0, i + 3].Blue, 2);
                LSB = Convert.ToString(remainder, 2);
                New.encoding[0, i + 3].Blue = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2);

            }       //Height definition
            #endregion     

            for (int i = 6; i < Covert.Height * Covert.Width; i++)
            {
                New.encoding[i / Covert.width, i % Covert.width] = new Pixel(0, 0, 0);

                if (i < height * width + 6)
                {
                    MSB = Convert.ToString(Covert.encoding[i / Covert.width, i % Covert.width].Red, 2);
                    LSB = Convert.ToString(this.encoding[(i-6) / width, (i-6) % width].Red, 2);
                    New.encoding[i / Covert.width, i % Covert.width].Red = Convert.ToInt32(Ou_Exclusif(MSB, LSB, true), 2);

                    MSB = Convert.ToString(Covert.encoding[i / Covert.width, i % Covert.width].Green, 2);
                    LSB = Convert.ToString(this.encoding[(i - 6) / width, (i - 6) % width].Green, 2);
                    New.encoding[i / Covert.width, i % Covert.width].Green = Convert.ToInt32(Ou_Exclusif(MSB, LSB, true), 2);

                    MSB = Convert.ToString(Covert.encoding[i / Covert.width, i % Covert.width].Blue, 2);
                    LSB = Convert.ToString(this.encoding[(i - 6) / width, (i - 6) % width].Blue, 2);
                    New.encoding[i / Covert.width, i % Covert.width].Blue = Convert.ToInt32(Ou_Exclusif(MSB, LSB, true), 2);
                }       //Image and Covert
                else
                {
                    New.encoding[i / Covert.width, i % Covert.width] = Covert.encoding[i / Covert.width, i % Covert.width];
                }       //Covert
            }       //Filling
            return New;
        }

        public MyImage Decoder(MyImage Key)
        {
            string MSB, LSB;
            int newWidth = 0;
            int newHeight = 0;

            for (int i = 0; i < 3; i++)
            {
                MSB = Convert.ToString(Key.encoding[0, i].Red, 2);
                LSB = Convert.ToString(this.encoding[0, i].Red, 2);
                newWidth += Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * (int)Math.Pow(2, (3 * i ) * 4);

                MSB = Convert.ToString(Key.encoding[0, i].Green, 2);
                LSB = Convert.ToString(this.encoding[0, i].Green, 2);
                newWidth += Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * (int)Math.Pow(2, (3 * i + 1) * 4);

                MSB = Convert.ToString(Key.encoding[0, i].Blue, 2);
                LSB = Convert.ToString(this.encoding[0, i].Blue, 2);
                newWidth += Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * (int)Math.Pow(2, (3 * i + 2) * 4);
            }       //Width definition

            for (int i = 0; i < 3; i++)
            {
                MSB = Convert.ToString(Key.encoding[0, i + 3].Red, 2);
                LSB = Convert.ToString(this.encoding[0, i + 3].Red, 2);
                newHeight += Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * (int)Math.Pow(2, (3 * i ) * 4);

                MSB = Convert.ToString(Key.encoding[0, i + 3].Green, 2);
                LSB = Convert.ToString(this.encoding[0, i + 3].Green, 2);
                newHeight += Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * (int)Math.Pow(2, (3 * i + 1) * 4);

                MSB = Convert.ToString(Key.encoding[0, i + 3].Blue, 2);
                LSB = Convert.ToString(this.encoding[0, i + 3].Blue, 2);
                newHeight += Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * (int)Math.Pow(2, (3 * i + 2) * 4);
            }       //Height definition

            int fileSize = newHeight * newWidth + this.sizeOffset;
            MyImage New = new MyImage(this.type, fileSize, this.sizeOffset, newWidth, newHeight, this.colorEncoding);
            New.encoding = new Pixel[newHeight, newWidth];

            int red, green, blue;
            for (int i = 6; i < newHeight * newWidth + 6; i++)
            {
                MSB = Convert.ToString(Key.encoding[i / Width, i % Width].Red, 2);
                LSB = Convert.ToString(this.encoding[i / Width, i % Width].Red, 2);
                red = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * 16;

                MSB = Convert.ToString(Key.encoding[i / Width, i % Width].Green, 2);
                LSB = Convert.ToString(this.encoding[i / Width, i % Width].Green, 2);
                green = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * 16;

                MSB = Convert.ToString(Key.encoding[i / Width, i % Width].Blue, 2);
                LSB = Convert.ToString(this.encoding[i / Width, i % Width].Blue, 2);
                blue = Convert.ToInt32(Ou_Exclusif(MSB, LSB, false), 2) * 16;

                New.encoding[(i-6) / newWidth, (i-6) % newWidth] =new Pixel(red, green, blue);
            }
            return New;
        }
        #endregion

        #endregion
    }
}

