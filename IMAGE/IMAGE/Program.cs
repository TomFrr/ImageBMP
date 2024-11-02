using System;
using System.IO;
using System.Linq;
using Fichier_Bitmap;
using System.Diagnostics;

namespace Program
{
    internal class Program
    {
        static void Main(string[] arg)
        {
            //Test();
            Menu();
            
        }

        static void ToStringHeaderFromFiles(string myfile)
        {
            MyImage I = new MyImage("BM",0,0,0,0,0);
            byte[] bmp = File.ReadAllBytes(myfile);
            for (int i = 0; i < bmp.Length; i++) // matrice image bmp.Length
            {
                if (i == 0)
                    Console.WriteLine("\nHeader");
                else if (i == 14)
                    Console.WriteLine("\nHeader Info");
                else if (i == 54)
                    Console.WriteLine("\nImage");
                else if ((i - 54) % I.Convert_Endian_To_Int(bmp.Take(22).Skip(18).ToArray()) == 0)
                    Console.Write("\n");
                Console.Write(bmp[i] + " ");
            }
        }

        static void Test()
        {
            MyImage coco = new MyImage("C:/Users/unksn/source/repos/IMAGE/IMAGE/bin/Debug/net6.0/coco.bmp");
            MyImage lac = new MyImage("C:/Users/unksn/source/repos/IMAGE/IMAGE/bin/Debug/net6.0/lac.bmp");
            //MyImage prod = new MyImage("BM", 3 * 1000 * 1000 + 54, 54, 1000, 1000, 24);
            //ToStringHeaderFromFiles("C:/Users/unksn/source/repos/IMAGE/IMAGE/bin/Debug/net6.0/Test.bmp");
            //test.ToStringHeader();
            //test.Nuance_De_Gris();
            //test.Miroir();
            //test.From_Image_To_File("Result.bmp");
            //test.Convolution(new int[,] { { -1, -1,   -1 }, { -1, 9, -1 }, { -1, -1, -1 } }).From_Image_To_File("Result.bmp");
            //test.Convolution(new int[,] { { 0, -1, 0 }, { -1, 4, -1 }, { 0, -1, 0 } }).From_Image_To_File("Result.bmp");
            //test.Agrandissement(0.5).From_Image_To_File("Result.bmp");
            coco.Coder(lac).Decoder(lac).From_Image_To_File("Result.bmp");
            //prod.Fractal(1,1);
            //prod.From_Image_To_File("Result.bmp");
            Process.Start(new ProcessStartInfo("C:/Users/unksn/source/repos/IMAGE/IMAGE/bin/Debug/net6.0/Result.bmp") { UseShellExecute = true });

            //MyImage aaa = new MyImage("C:/Users/unksn/source/repos/IMAGE/IMAGE/bin/Debug/net6.0/AAA.bmp");
            //ToStringHeaderFromFiles("result.bmp");
        }

        static void Menu()
        {
            ConsoleKey reponse;
            do
            {              
                do
                {
                    Console.Clear();
                    Console.WriteLine("1. Créer une nouvelle image" +
                        "\n2. Ouvrir une image existante" +
                        "\n3 Quitter l'application");
                    reponse = Console.ReadKey(false).Key;
                } while (reponse != ConsoleKey.D1 && 
                reponse != ConsoleKey.D2 &&
                reponse != ConsoleKey.D3);     //Opening

                if (reponse != ConsoleKey.D3)
                {
                    MyImage image = Instanciation(reponse);

                    do
                    {
                        switch (Tools())
                        {
                            case ConsoleKey.D1 :     //Nuance de gris
                                {
                                    Console.Clear();
                                    Console.WriteLine("Nuance de gris");
                                    image.Nuance_De_Gris();
                                    break;
                                }
                            case ConsoleKey.D2:     //Noir et blanc
                                {
                                    Console.Clear();
                                    Console.WriteLine("Noir et blanc");
                                    image.Noir_Et_Blanc();
                                    break;
                                }
                            case ConsoleKey.D3:     //Miroir
                                {
                                    Console.Clear();
                                    Console.WriteLine("Miroir");
                                    image.Miroir();
                                    break;
                                }
                            case ConsoleKey.D4:     //Rogner
                                {
                                    int height;
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Veuillez saisir la hauteur de l'image :");
                                        height = Convert.ToInt32(Console.ReadLine());
                                    } while (height < 1);

                                    int width;
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Veuillez saisir la largeur de l'image :");
                                        width = Convert.ToInt32(Console.ReadLine());
                                    } while (width < 1);
                                    image = image.Rogner(width, height);
                                    break;
                                }
                            case ConsoleKey.D5:     //Agrandissement
                                {
                                    double coefficient;
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Agrandissement" +
                                            "\nVeuillez saisir le coefficient d'agrandissement :");
                                        coefficient = Convert.ToDouble(Console.ReadLine());
                                    } while (coefficient < 0);
                                    image = image.Agrandissement(coefficient);
                                    break;
                                }
                            case ConsoleKey.D6:     //Rotation
                                {
                                    double angle;
                                    Console.Clear();
                                    Console.WriteLine("Veuillez saisir l'angle de l'image :");
                                    angle = Convert.ToDouble(Console.ReadLine());
                                    image = image.Rotation(angle);
                                    break;
                                }
                            case ConsoleKey.D7:     //Convolution
                                {
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Convolution" +
                                            "\nContour" +
                                            "\nContraste" +
                                            "\nRepoussage" +
                                            "\nFlou");
                                        reponse = Console.ReadKey(false).Key;
                                    } while (reponse != ConsoleKey.D1 &&
                                        reponse != ConsoleKey.D2 &&
                                        reponse != ConsoleKey.D3 &&
                                        reponse != ConsoleKey.D4);

                                    switch (reponse)
                                    {
                                        case ConsoleKey.D1:
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Convolution" +
                                                    "\nContour");
                                                image = image.Contour();
                                                break;
                                            }
                                        case ConsoleKey.D2:
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Convolution" +
                                                    "\nContraste");
                                                image = image.Contraste();
                                                break;
                                            }
                                        case ConsoleKey.D3:
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Convolution" +
                                                    "\nRepoussage");
                                                image = image.Repoussage();
                                                break;
                                            }
                                        case ConsoleKey.D4:
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Convolution" +
                                                    "\nFlou");
                                                image = image.Flou();
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case ConsoleKey.D8:     //Fractale
                                {
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Fractale" +
                                            "\n1. " +
                                            "\n2. " +
                                            "\n3. " +
                                            "\n4. ");
                                        reponse = Console.ReadKey(false).Key;
                                    } while (reponse != ConsoleKey.D1 &&
                                        reponse != ConsoleKey.D2 &&
                                        reponse != ConsoleKey.D3 &&
                                        reponse != ConsoleKey.D4 );
                                    image.Fractale(100, reponse);
                                    break;
                                }
                            case ConsoleKey.D9:     //Stéganographie
                                {
                                    ConsoleKey numero;
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Stéganographie" +
                                            "\n1. Coder" +
                                            "\n2. Decoder");
                                        numero = Console.ReadKey(false).Key;
                                    } while (numero != ConsoleKey.D1 && numero != ConsoleKey.D2);

                                    switch (numero)
                                    {
                                        case ConsoleKey.D1:
                                            {
                                                MyImage Key;
                                                do
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Coder" +
                                                        "\nVeuillez saisir une image clé de dimensions supérieur à l'image actuelle :");
                                                    Console.ReadKey(false);
                                                    Key = Instanciation(ConsoleKey.D2); 
                                                }while (Key.Width < image.Width && Key.Height < image.Height);
                                                image = image.Coder(Key);
                                                break;
                                            }
                                        case ConsoleKey.D2:
                                            {
                                                MyImage Key;
                                                do
                                                {
                                                    Console.Clear();
                                                    Console.WriteLine("Decoder" +
                                                        "\nVeuillez saisir l'image clé :");
                                                    Console.ReadKey(false);
                                                    Key = Instanciation(ConsoleKey.D2);
                                                } while (Key.Width != image.Width && Key.Height != image.Height);
                                                image = image.Decoder(Key);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }       //Processing
                        Console.ReadKey(false);

                        image.From_Image_To_File("system.bmp");     
                        Process.Start(new ProcessStartInfo("system.bmp") { UseShellExecute = true });

                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Y. Enregistrer l'image" +
                                "\nN. Ne pas enregister l'image");
                            reponse = Console.ReadKey().Key;
                        } while (reponse != ConsoleKey.Y && reponse != ConsoleKey.N);   //Saving

                        string nom;
                        if (reponse == ConsoleKey.Y)
                        {
                            Console.Clear();
                            Console.WriteLine("Veuillez saisir le nom de l'image :");
                            nom = Console.ReadLine();
                            image.From_Image_To_File(nom+".bmp");
                        }   //File name

                        do
                        {
                            Console.Clear();
                            Console.WriteLine("1. Outils" +
                                "\n2. Menu ");
                            reponse = Console.ReadKey().Key;
                        } while (reponse != ConsoleKey.D1 && reponse != ConsoleKey.D2);     //Return

                    }while (reponse == ConsoleKey.D1);
                    
                }       //Programm

            }while (reponse != ConsoleKey.D3);
        }

        static MyImage Instanciation(ConsoleKey reponse)
        {
            MyImage image;
            
            if (reponse == ConsoleKey.D2)
            {
                string documentPath = Environment.CurrentDirectory;
                string[] path = Directory.GetFiles(documentPath, "*.bmp");

                int numero;
                do
                {
                    Console.Clear();
                    for (int i = 0; i < path.Length; i++)
                    {
                        string nom = Path.GetFileName(path[i]);
                        Console.WriteLine(i + 1 + ". " + nom.Substring(0, nom.Length - 4));
                    }
                    numero = Convert.ToInt32(Console.ReadLine());
                } while (numero < 1 || numero > path.Length);
                image = new MyImage(path[numero - 1]);
                Process.Start(new ProcessStartInfo(path[numero - 1]) { UseShellExecute = true });
            }       //Files
            else
            {
                int height;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Veuillez saisir la hauteur de l'image :");
                    height = Convert.ToInt32(Console.ReadLine());
                } while (height < 1);

                int width;
                do
                {
                    Console.Clear();
                    Console.WriteLine("Veuillez saisir la largeur de l'image :");
                    width = Convert.ToInt32(Console.ReadLine());
                } while (width < 1);
                image = new MyImage("BM", 3 * width * height + 54, 54, width, height, 24);
            }       //New

            return image;
        }

        static ConsoleKey Tools()
        {
            ConsoleKey numero;
            do
            {
                Console.Clear();
                Console.WriteLine(
                    "1. Nuance de gris" +
                    "\n2. Noir et blanc" +
                    "\n3. Miroir" +
                    "\n4. Rogner" +
                    "\n5. Agrandissement" +
                    "\n6. Rotation" +
                    "\n7. Convolution" +
                    "\n8. Fractale" +
                    "\n9. Stéganographie");
                numero = Console.ReadKey(false).Key;
            } while (numero != ConsoleKey.D1 &&
            numero != ConsoleKey.D2 &&
            numero != ConsoleKey.D3 &&
            numero != ConsoleKey.D4 &&
            numero != ConsoleKey.D5 &&
            numero != ConsoleKey.D6 &&
            numero != ConsoleKey.D7 &&
            numero != ConsoleKey.D8 &&
            numero != ConsoleKey.D9);
            return numero;
        }
    }
}
